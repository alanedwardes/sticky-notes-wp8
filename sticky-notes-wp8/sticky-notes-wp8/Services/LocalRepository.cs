using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sticky_notes_wp8.Services;
using sticky_notes_wp8.Data;

namespace sticky_notes_wp8.Services
{
    public class LocalRepository
    {
        private StickyNotesDataContext dataContext;

        public LocalRepository(StickyNotesDataContext context)
        {
            this.dataContext = context;
        }

        public virtual List<Board> GetBoard()
        {
            return this.dataContext.Boards.ToList();
        }

        public virtual void ClearBoard()
        {
            this.dataContext.Boards.DeleteAllOnSubmit(this.dataContext.Boards);
        }

        public virtual void StoreBoard(Board board)
        {
            this.dataContext.Boards.InsertOnSubmit(board);
        }

        public virtual void StoreBoard(List<Board> boards)
        {
            this.dataContext.Boards.InsertAllOnSubmit<Board>(boards);
        }

        public virtual List<Note> GetNote()
        {
            return this.dataContext.Notes.ToList();
        }

        public virtual void ClearNote()
        {
            this.dataContext.Notes.DeleteAllOnSubmit(this.dataContext.Notes);
        }

        public virtual void ClearNote(List<Note> notes)
        {
            this.dataContext.Notes.DeleteAllOnSubmit(notes);
        }

        public virtual void StoreNote(Note note)
        {
            this.dataContext.Notes.InsertOnSubmit(note);
        }

        public virtual void StoreNote(List<Note> notes)
        {
            this.dataContext.Notes.InsertAllOnSubmit<Note>(notes);
        }

        public virtual void Commit()
        {
            this.dataContext.SubmitChanges();
        }

        public virtual void Rollback()
        {
            this.dataContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, this.dataContext.Notes);
            this.dataContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, this.dataContext.Boards);
        }
    }
}
