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

using sticky_notes_wp8.Data;
using sticky_notes_wp8.Services;

namespace sticky_notes_wp8
{
    public partial class NoteList : PhoneApplicationPage, INotifyPropertyChanged
    {
        private Board filterBoard;
        private string pageTitle;

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        public string PageTitle
        {
            get
            {
                return this.pageTitle;
            }
            set
            {
                this.pageTitle = value;
                NotifyPropertyChanged("PageTitle");
            }
        }

        public NoteList()
        {
            InitializeComponent();

            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var localRepository = Locator.Instance<LocalRepository>();

            base.OnNavigatedTo(e);

            string boardId;
            if (NavigationContext.QueryString.TryGetValue("boardId", out boardId))
            {
                this.filterBoard = localRepository.GetBoard(int.Parse(boardId));
            }
            else
            {
                this.filterBoard = null;
            }

            this.RefreshNotes();
        }

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<Note> notes;
        public ObservableCollection<Note> Notes
        {
            get
            {
                return notes;
            }
            set
            {
                if (notes != value)
                {
                    notes = value;
                    NotifyPropertyChanged("Notes");
                }
            }
        }

        private void RefreshNotes(string query = "")
        {
            var localRepository = Locator.Instance<LocalRepository>();
            var notes = localRepository.GetNote().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                notes = notes.Where(n => n.Body.Contains(this.SearchBox.Text));
            }

            if (this.filterBoard != null)
            {
                PageTitle = filterBoard.Name;
                notes = notes.Where(n => n.BoardId == this.filterBoard.Id);
            }
            else
            {
                PageTitle = "my notes";
                notes = notes.Where(n => n.BoardId == 0);
            }

            notes = notes.OrderByDescending(n => n.Created);
            Notes = new ObservableCollection<Note>(notes);
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
            this.RefreshNotes();
        }

        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.RefreshNotes(this.SearchBox.Text);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var note = frameworkElement.DataContext as Note;

            var addNoteRedirect = "/Pages/AddNote.xaml?noteId=" + note.LocalStorageId;
            if (this.filterBoard != null)
            {
                addNoteRedirect += "&boardId=" + filterBoard.LocalStorageId;
            }

            NavigationService.Navigate(new Uri(addNoteRedirect, UriKind.Relative));
        }

        private void TextBlock_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var localRepository = Locator.Instance<LocalRepository>();

            var frameworkElement = sender as FrameworkElement;
            var note = frameworkElement.DataContext as Note;

            MessageBoxResult result = MessageBox.Show(string.Format("Would you like to delete note \"{0}\"?", note.Body),
                "Delete Note", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                localRepository.ClearNote(note);
                localRepository.Commit();
                this.RefreshNotes();
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var addNoteRedirect = "/Pages/AddNote.xaml";
            if (this.filterBoard != null)
            {
                addNoteRedirect += "?boardId=" + filterBoard.LocalStorageId;
            }
            NavigationService.Navigate(new Uri(addNoteRedirect, UriKind.Relative));
        }

        private void BoardsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
        }
    }
}