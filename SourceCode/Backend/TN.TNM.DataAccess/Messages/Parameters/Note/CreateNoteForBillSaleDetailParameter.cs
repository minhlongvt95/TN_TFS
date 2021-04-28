using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteForBillSaleDetailParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
        public List<NoteDocumentEntityModel> ListNoteDocument { get; set; }
    }
}
