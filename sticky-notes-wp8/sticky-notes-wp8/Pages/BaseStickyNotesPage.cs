namespace sticky_notes_wp8.Pages
{
    using Microsoft.Phone.Controls;
    using sticky_notes_wp8.Data;
    using sticky_notes_wp8.Services;
    using System.ComponentModel;

    public abstract class BaseStickyNotesPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public void InitializeDataContext()
        {
            this.DataContext = this;
        }

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        public OnlineRepository OnlineRepository
        {
            get { return Locator.Instance<OnlineRepository>(); }
        }

        public LocalRepository LocalRepository
        {
            get { return Locator.Instance<LocalRepository>(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
