using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Collections.ObjectModel;
using sticky_notes_wp8.Services;
using sticky_notes_wp8.Data;

namespace sticky_notes_wp8
{
    public partial class BoardList : PhoneApplicationPage, INotifyPropertyChanged
    {
        private string sessionToken;

        public BoardList()
        {
            InitializeComponent();

            InitializeDataContext();

            sessionToken = SettingsManager.GetSetting<string>(SettingsManager.SESSION_TOKEN);
        }

        private void InitializeDataContext()
        {
            boardsData = ServiceLocator.GetInstance<StickyNotesDataContext>();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.RefreshBoards();
            base.OnNavigatedTo(e);
        }

        // Data context for the local database
        private StickyNotesDataContext boardsData;

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<Board> boards;
        public ObservableCollection<Board> Boards
        {
            get
            {
                return boards;
            }
            set
            {
                if (boards != value)
                {
                    boards = value;
                    NotifyPropertyChanged("Boards");
                }
            }
        }

        private void RefreshBoards(string query = "")
        {
            IQueryable<Board> boards = boardsData.Boards;
            if (!string.IsNullOrWhiteSpace(query))
            {
                boards = boards.Where(n => n.Name.Contains(this.SearchBox.Text));
            }
            //boards = boards.OrderByDescending(n => n.Created);
            Boards = new ObservableCollection<Board>(boards);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.SearchBox.Text = string.Empty;
            this.RefreshBoards();
        }

        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.RefreshBoards(this.SearchBox.Text);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var note = frameworkElement.DataContext as Note;

            NavigationService.Navigate(new Uri("/Pages/AddNote.xaml?noteId=" + note.Id, UriKind.Relative));
        }

        private void BoardsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (sessionToken == null)
            {
                NavigationService.Navigate(new Uri("/Pages/Login.xaml?redirectTo=/Pages/BoardList.xaml", UriKind.Relative));
            }
        }

        private async void ReloadButton_Click(object sender, EventArgs e)
        {
            var onlineRepository = ServiceLocator.GetInstance<OnlineRepository>();

            var boardsResponse = await onlineRepository.boardsList(sessionToken);
            var boards = boardsResponse.data;
            Boards.Clear();
            foreach (var board in boards)
            {
                Boards.Add(board);
            }
        }
    }
}