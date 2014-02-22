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
        private string redirectUri;

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

            ShowMessageBasedOnResponseCode(response.code);

            if (response.code == 200)
            {
                SaveUserTokenFromLoginResponse(response);
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
                case 200:
                    break;
                default:
                    MessageBox.Show("An error occurred whilst logging in. Please try again.", "Login Error", MessageBoxButton.OK);
                    break;
            }
        }

        private void SaveUserTokenFromLoginResponse(OnlineRepository.RepositoryResponse<OnlineRepository.LoginResponse> response)
        {
            SettingsManager.SaveSetting(SettingsManager.SESSION_TOKEN, response.data.session.id);

            // SettingsManager.GetSetting<string>(SettingsManager.SESSION_TOKEN);
            if (redirectUri != null)
            {
                NavigationService.Navigate(new Uri(redirectUri, UriKind.Relative));
            }
        }
    }
}