using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Promotion
{
    public class CreateNoteForPromotionDetailResult : BaseResult
    {
        public List<NoteEntityModel> NoteHistory { get; set; }
    }
}
