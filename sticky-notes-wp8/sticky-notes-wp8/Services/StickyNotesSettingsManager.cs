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

        public double MinTextSize { get { return 30d; } }
        public double MaxTextSize { get { return 50d; } }

        public double TextSize
        {
            get
            {
                var textSize = SettingsManager.GetSetting<double>(TEXT_SIZE, MinTextSize);
                return textSize < MinTextSize ? MinTextSize : textSize;
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
