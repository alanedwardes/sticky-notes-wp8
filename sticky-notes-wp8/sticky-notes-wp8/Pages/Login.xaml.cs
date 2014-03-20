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
using sticky_notes_wp8.Resources;
using sticky_notes_wp8.Services;

using sticky_notes_wp8.Pages;

namespace sticky_notes_wp8
{
    public partial class Login : BaseStickyNotesPage
    {
        private string redirectUri;

        public Login()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginButton = sender as Button;

            loginProgress.IsIndeterminate = true;
            loginButton.IsEnabled = false;

            var response = await this.OnlineRepository.UserLogin(this.username.Text, this.password.Password);

            loginButton.IsEnabled = true;
            loginProgress.IsIndeterminate = false;

            if (response.WasSuccessful())
            {
                SaveUserTokenFromLoginResponse(response);
            }
            else
            {
                ShowMessageBasedOnResponseCode(response.code);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string redirectTo;
            if (NavigationContext.QueryString.TryGetValue("redirectTo", out redirectTo))
            {
                NavigationService.RemoveBackEntry();
                redirectUri = redirectTo;
            }
        }

        private void ShowMessageBasedOnResponseCode(int code)
        {
            switch (code)
            {
                case 403:
                    MessageBox.Show("Invalid username or password.", "Incorrect Credentials", MessageBoxButton.OK);
                    break;
                default:
                    MessageBox.Show("An error occurred whilst logging in. Please try again.", "Login Error", MessageBoxButton.OK);
                    break;
            }
        }

        private void SaveUserTokenFromLoginResponse(OnlineRepository.RepositoryResponse<OnlineRepository.LoginResponse> response)
        {
            this.SettingsManager.SessionToken = response.data.session.id;

            if (redirectUri != null)
            {
                NavigationService.Navigate(new Uri(redirectUri, UriKind.Relative));
            }
        }
    }
}