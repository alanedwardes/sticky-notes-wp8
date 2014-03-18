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
using sticky_notes_wp8.Services;

namespace sticky_notes_wp8.Pages
{
    public partial class AddBoard : BaseStickyNotesPage
    {
        public AddBoard()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private Board currentBoard;

        public Board CurrentBoard
        {
            get { return currentBoard; }
            set { currentBoard = value; NotifyPropertyChanged("CurrentBoard"); }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.CurrentBoard = new Board();
        }

        private async void AddBoardButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await this.OnlineRepository.BoardsSave(this.SettingsManager.SessionToken, this.CurrentBoard);
            if (response.code != 201)
            {
                MessageBox.Show("Unable to create board!");
            }
            else
            {
                NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
            }
        }
    }
}