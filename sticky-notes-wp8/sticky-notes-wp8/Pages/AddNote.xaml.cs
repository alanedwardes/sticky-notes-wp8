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
    using sticky_notes_wp8.Pages;

    public partial class AddNote : BaseStickyNotesPage
    {
        private Note currentNote;
        public Note CurrentNote
        {
            get { return currentNote; }
            set { currentNote = value; NotifyPropertyChanged("CurrentNote"); }
        }

        private Board currentBoard;
        public bool InEditMode;

        public AddNote()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);
            this.LocalRepository.Commit();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            string noteId;
            if (NavigationContext.QueryString.TryGetValue("noteId", out noteId))
            {
                this.CurrentNote = this.LocalRepository.GetNote().Where(n => n.LocalStorageId == int.Parse(noteId)).Single();
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
                this.currentBoard = this.LocalRepository.GetBoard().Where(b => b.LocalStorageId == int.Parse(boardId)).Single();
            }
            else
            {
                this.currentBoard = null;
            }
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NoteBody.Text))
            {
                if (!this.InEditMode)
                {
                    this.CurrentNote.Created = DateTime.Now;
                    this.LocalRepository.StoreNote(currentNote);
                }

                this.LocalRepository.Commit();

                if (this.currentBoard != null)
                {
                    this.OnlineRepository.NotesSave(this.SettingsManager.SessionToken, this.currentNote, this.currentBoard.Id);

                    NavigationService.Navigate(new Uri(string.Format("/Pages/NoteList.xaml?boardId={0}", this.currentBoard.Id), UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
                }
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