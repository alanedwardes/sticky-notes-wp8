using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace sticky_notes_wp8.Services
{
    public class StickyNotesSettingsManager: INotifyPropertyChanged
    {
        private const string SESSION_TOKEN = "session_token";
        private const string TEXT_SIZE = "text_size";
        private const double TEXT_SIZE_DEFAULT = 10d;

        public double TextSize
        {
            get
            {
                var textSize = SettingsManager.GetSetting<double>(TEXT_SIZE);
                return textSize > 0 ? textSize : TEXT_SIZE_DEFAULT;
            }
            set
            {
                SettingsManager.SaveSetting<double>(TEXT_SIZE, value);
                NotifyPropertyChanged("TextSize");
                NotifyPropertyChanged("CaptionTextSize");
            }
        }

        public double CaptionTextSize
        {
            get
            {
                return TextSize * 0.75d;
            }
        }

        public string SessionToken
        {
            get
            {
                return SettingsManager.GetSetting<string>(SESSION_TOKEN, null);
            }
            set
            {
                SettingsManager.SaveSetting<string>(SESSION_TOKEN, value);
                NotifyPropertyChanged("SessionToken");
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
