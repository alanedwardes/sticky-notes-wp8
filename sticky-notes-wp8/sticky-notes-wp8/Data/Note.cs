using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace sticky_notes_wp8.Data
{
    [Table]
    public class Note : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private int id;

        [Column(IsPrimaryKey = true,
            IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    NotifyPropertyChanging("Id");
                    id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string body;

        [Column(DbType = "NVarChar(1024) NOT NULL",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public string Body
        {
            get { return body; }
            set
            {
                if (body != value)
                {
                    NotifyPropertyChanging("Body");
                    body = value;
                    NotifyPropertyChanged("Body");
                }
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

        public event PropertyChangingEventHandler PropertyChanging;
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}
