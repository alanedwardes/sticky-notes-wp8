using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.ComponentModel;
using sticky_notes_wp8.Data;
using sticky_notes_wp8.Services;

namespace sticky_notes_wp8.Views
{
    public partial class AddNote : PhoneApplicationPage, INotifyPropertyChanged
    {
        private Note currentNote;
        public Note CurrentNote
        {
            get { return currentNote; }
            set
            {
                if (currentNote != value)
                {
                    currentNote = value;
                    NotifyPropertyChanged("CurrentNote");
                }
            }
        }

        private Board currentBoard;

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        public bool InEditMode;

        public AddNote()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            this.DataContext = this;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);

            var localRepository = Locator.Instance<LocalRepository>();
            localRepository.Commit();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var localRepository = Locator.Instance<LocalRepository>();

            base.OnNavigatedTo(e); 
            string noteId;
            if (NavigationContext.QueryString.TryGetValue("noteId", out noteId))
            {
                this.CurrentNote = localRepository.GetNote().Where(n => n.LocalStorageId == int.Parse(noteId)).Single();
                this.PageTitle.Text = "edit note";
                this.InEditMode = true;
            }
            else
            {
                this.CurrentNote = new Note();
                this.PageTitle.Text = "new note";
                this.InEditMode = false;
            }

            string boardId;
            if (NavigationContext.QueryString.TryGetValue("boardId", out boardId))
            {
                this.currentBoard = localRepository.GetBoard().Where(b => b.LocalStorageId == int.Parse(boardId)).Single();
            }
            else
            {
                this.currentBoard = null;
            }
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            var localRepository = Locator.Instance<LocalRepository>();
            var onlineRepository = Locator.Instance<OnlineRepository>();
            var settingsManager = Locator.Instance<StickyNotesSettingsManager>();

            if (!string.IsNullOrWhiteSpace(NoteBody.Text))
            {
                if (!this.InEditMode)
                {
                    this.CurrentNote.Created = DateTime.Now;
                    localRepository.StoreNote(currentNote);
                }

                localRepository.Commit();

                if (this.currentBoard != null)
                {
                    onlineRepository.NotesSave(settingsManager.SessionToken, this.currentNote, this.currentBoard.Id);

                    NavigationService.Navigate(new Uri(string.Format("/Pages/NoteList.xaml?boardId={0}", this.currentBoard.Id), UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NoteBody_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.SaveNote.Focus();
            }
        }

        private void NoteTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.NoteBody.Focus();
            }
        }
    }
}