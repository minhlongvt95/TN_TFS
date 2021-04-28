using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class CreateNoteForPromotionDetailResponse : BaseResponse
    {
        public List<NoteEntityModel> NoteHistory { get; set; }
    }
}
