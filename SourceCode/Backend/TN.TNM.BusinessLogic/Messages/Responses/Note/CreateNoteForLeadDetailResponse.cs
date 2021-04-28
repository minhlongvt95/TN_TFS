using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteForLeadDetailResponse: BaseResponse
    {
        public List<NoteModel> ListNote { get; set; }
    }
}
