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

namespace sticky_notes_wp8.Views
{
    public partial class AddNote : PhoneApplicationPage, INotifyPropertyChanged
    {
        private StickyNotesDataContext notesData;
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

        public bool InEditMode;

        public AddNote()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            notesData = ServiceLocator.GetInstance<StickyNotesDataContext>();
            this.DataContext = this;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);

            // Save changes to the database.
            notesData.SubmitChanges();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            string noteId;
            if (NavigationContext.QueryString.TryGetValue("noteId", out noteId))
            {
                this.CurrentNote = this.notesData.Notes.Where(n => n.Id == int.Parse(noteId)).Single();
                this.PageTitle.Text = "edit note";
                this.InEditMode = true;
            }
            else
            {
                this.CurrentNote = new Note();
                this.PageTitle.Text = "new note";
                this.InEditMode = false;
            }
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NoteBody.Text))
            {
                if (!this.InEditMode)
                {
                    this.CurrentNote.Created = DateTime.Now;
                    notesData.Notes.InsertOnSubmit(currentNote);
                }

                NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
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
    }
}