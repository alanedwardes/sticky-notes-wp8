using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace sticky_notes_wp8.Data
{
    class StickyNotesDataContext : DataContext
    {
        public const string DBConnectionString = "Data Source=isostore:/StickyNotes.sdf";

        public StickyNotesDataContext(string connectionString) : base(connectionString) { }

        public Table<Note> Notes;
        public Table<Board> Boards;
    }
}
