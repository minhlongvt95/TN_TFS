using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CreateNoteForPromotionDetailParameter  : BaseParameter
    {
        public NoteEntityModel Note { get; set; }
    }
}
