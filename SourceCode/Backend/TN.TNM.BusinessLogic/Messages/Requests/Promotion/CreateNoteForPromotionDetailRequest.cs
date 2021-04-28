using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CreateNoteForPromotionDetailRequest : BaseRequest<CreateNoteForPromotionDetailParameter>
    {
        public NoteEntityModel Note { get; set; }

        public override CreateNoteForPromotionDetailParameter ToParameter()
        {
            return new CreateNoteForPromotionDetailParameter()
            {
                UserId = UserId,
                Note = Note
            };
        }
    }
}
