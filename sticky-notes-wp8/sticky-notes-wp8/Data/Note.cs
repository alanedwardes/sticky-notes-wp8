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
    public class Note : INotifyPropertyChanged
    {
        private int localStorageId;
        [Column(IsPrimaryKey = true,
            IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int LocalStorageId
        {
            get { return localStorageId; }
            set { localStorageId = value; NotifyPropertyChanged("LocalStorageId"); }
        }

        private int id;
        [Column(DbType = "INT")]
        public int Id
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("Id"); }
        }

        private int boardId;
        [Column(DbType = "INT")]
        public int BoardId
        {
            get { return boardId; }
            set { boardId = value; NotifyPropertyChanged("BoardId"); }
        }

        private string body;
        [Column(DbType = "NVarChar(1024) NOT NULL", CanBeNull = false)]
        public string Body
        {
            get { return body; }
            set { body = value; NotifyPropertyChanged("Body"); }
        }

        private DateTime created;
        [Column(DbType = "DATETIME")]
        public DateTime Created
        {
            get { return created; }
            set { created = value; NotifyPropertyChanged("Created"); }
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
