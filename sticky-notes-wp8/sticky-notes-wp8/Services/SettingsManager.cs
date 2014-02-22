using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace sticky_notes_wp8.Services
{
    static class SettingsManager
    {
        public const string SESSION_TOKEN = "session_token";

        private static IsolatedStorageSettings GetIsolatedStorageSettings()
        {
            return IsolatedStorageSettings.ApplicationSettings;
        }

        public static void SaveSetting<T>(string key, T value)
        {
            var storage = GetIsolatedStorageSettings();

            if (!storage.Contains(key))
                storage.Add(key, value);
            else
                storage[key] = value;

            storage.Save();
        }

        public static T GetSetting<T>(string key)
        {
            var storage = GetIsolatedStorageSettings();
            if (storage.Contains(key))
                return (T)storage[key];
            else
                return default(T);
        }
    }
}
