using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sticky_notes_wp8.Data;

namespace sticky_notes_wp8.Views
{
    public partial class AddNote : PhoneApplicationPage
    {
        private StickyNotesDataContext notesData;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NoteBody.Text))
            {
                Note note = new Note { Body = NoteBody.Text };
                notesData.Notes.InsertOnSubmit(note);
                NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
            }
        }

    }
}