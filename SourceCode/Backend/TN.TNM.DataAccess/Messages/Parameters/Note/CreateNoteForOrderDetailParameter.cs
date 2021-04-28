using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteForOrderDetailParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
        public List<NoteDocumentEntityModel> ListNoteDocument { get; set; }
    }
}
