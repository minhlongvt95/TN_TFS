using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteForOrderDetailResponse : BaseResponse
    {
        public List<NoteModel> ListNote { get; set; }
        public Guid NoteId { get; set; }
    }
}
