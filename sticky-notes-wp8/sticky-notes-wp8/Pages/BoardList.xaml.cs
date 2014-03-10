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
using Microsoft.Phone.Net.NetworkInformation;

namespace sticky_notes_wp8
{
    public partial class BoardList : PhoneApplicationPage, INotifyPropertyChanged
    {
        private string sessionToken;

        public BoardList()
        {
            InitializeComponent();

            InitializeDataContext();

            sessionToken = Locator.Instance<StickyNotesSettingsManager>().SessionToken;
        }

        private void InitializeDataContext()
        {
            this.DataContext = this;
        }

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.RefreshBoards();
            base.OnNavigatedTo(e);
        }

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
            var localRepository = Locator.Instance<LocalRepository>();
            var boards = localRepository.GetBoard();

            if (!string.IsNullOrWhiteSpace(query))
            {
                boards = boards.Where(n => n.Name.Contains(this.SearchBox.Text)).ToList();
            }

            boards = boards.OrderBy(n => n.Name).ToList();
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

        private async void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var board = frameworkElement.DataContext as Board;

            NavigationService.Navigate(new Uri(string.Format("/Pages/NoteList.xaml?boardId={0}", board.Id),
                UriKind.Relative));
        }

        private void BoardsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (sessionToken == null)
            {
                NavigationService.Navigate(new Uri("/Pages/Login.xaml?redirectTo=/Pages/BoardList.xaml", UriKind.Relative));
            }

            var available = DeviceNetworkInformation.IsNetworkAvailable;
            if (!available)
            {
                return;
            }

            LoadingProgress.IsIndeterminate = true;

            var onlineRepository = Locator.Instance<OnlineRepository>();

            var boardsResponse = await onlineRepository.BoardsList(sessionToken);
            if (boardsResponse.code != 200)
            {
                LoadingProgress.IsIndeterminate = false;
                return;
            }

            if (boardsResponse.data.boards == null)
            {
                LoadingProgress.IsIndeterminate = false;
                return;
            }

            var localRepository = Locator.Instance<LocalRepository>();
            localRepository.ClearBoard();
            localRepository.StoreBoard(boardsResponse.data.boards);

            foreach (var board in boardsResponse.data.boards)
            {
                var notesResponse = await onlineRepository.NotesList(sessionToken, board.Id);
                foreach (var note in notesResponse.data.notes)
                {
                    var notesToDelete = localRepository.GetNote().Where(n => n.BoardId == board.Id).ToList();
                    localRepository.ClearNote(notesToDelete);
                    localRepository.StoreNote(note);
                }
            }

            localRepository.Commit();
            this.RefreshBoards();

            LoadingProgress.IsIndeterminate = false;
        }

        private void NotesOnPhoneButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
        }
    }
}