using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetNoteHistoryResponse : BaseResponse
    {
        public List<NoteModel> ListNote { get; set; }
    }
}
