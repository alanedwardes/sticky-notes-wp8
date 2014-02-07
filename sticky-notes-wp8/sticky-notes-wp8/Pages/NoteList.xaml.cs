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

namespace sticky_notes_wp8
{
    public partial class NoteList : PhoneApplicationPage, INotifyPropertyChanged
    {
        public NoteList()
        {
            InitializeComponent();

            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            notesData = ServiceLocator.GetInstance<StickyNotesDataContext>();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AddNote.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var notesFromDb = from Note note in notesData.Notes select note;
            Notes = new ObservableCollection<Note>(notesFromDb);
            base.OnNavigatedTo(e);
        }

        // Data context for the local database
        private StickyNotesDataContext notesData;

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}