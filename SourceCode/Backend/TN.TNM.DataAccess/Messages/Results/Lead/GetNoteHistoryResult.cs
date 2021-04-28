using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetNoteHistoryResult : BaseResult
    {
        public List<NoteEntityModel> ListNode { get; set; }
    }
}
