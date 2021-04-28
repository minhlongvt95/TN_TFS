using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class DisableNoteParameter : BaseParameter
    {
        public Guid? NoteId { get; set; }
    }
}
