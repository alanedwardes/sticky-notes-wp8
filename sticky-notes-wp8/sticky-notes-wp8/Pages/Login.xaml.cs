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

namespace sticky_notes_wp8
{
    public partial class Login : PhoneApplicationPage, INotifyPropertyChanged
    {
        private string redirectUri;

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Login()
        {
            InitializeComponent();

            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            this.DataContext = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginButton = sender as Button;

            loginProgress.IsIndeterminate = true;
            loginButton.IsEnabled = false;

            var onlineRepository = Locator.Instance<OnlineRepository>();
            var response = await onlineRepository.UserLogin(this.username.Text, this.password.Password);

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
            Locator.Instance<StickyNotesSettingsManager>().SessionToken = response.data.session.id;

            if (redirectUri != null)
            {
                NavigationService.Navigate(new Uri(redirectUri, UriKind.Relative));
            }
        }
    }
}