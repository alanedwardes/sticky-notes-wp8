using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sticky_notes_wp8.Resources;
using sticky_notes_wp8.Services;

namespace sticky_notes_wp8
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginButton = sender as Button;

            loginProgress.IsIndeterminate = true;
            loginButton.IsEnabled = false;

            var onlineRepository = ServiceLocator.GetInstance<OnlineRepository>();
            var response = await onlineRepository.userLogin(this.username.Text, this.password.Password);

            loginButton.IsEnabled = true;
            loginProgress.IsIndeterminate = false;

            switch (response.code)
            {
                case 403:
                    MessageBox.Show("Invalid username or password.", "Incorrect Credentials", MessageBoxButton.OK);
                    break;
                case 200:
                    MessageBox.Show("Session token: " + response.data.session.id, "Login Successful", MessageBoxButton.OK);
                    //NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
                    break;
                default:
                    MessageBox.Show("An error occurred whilst logging in. Please try again.", "Login Error", MessageBoxButton.OK);
                    break;
            }
        }
    }
}