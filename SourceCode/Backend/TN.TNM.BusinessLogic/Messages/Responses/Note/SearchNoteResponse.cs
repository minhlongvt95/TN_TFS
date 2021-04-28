using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class SearchNoteResponse : BaseResponse
    {
        public List<NoteModel> NoteList { get; set; }
    }
}
