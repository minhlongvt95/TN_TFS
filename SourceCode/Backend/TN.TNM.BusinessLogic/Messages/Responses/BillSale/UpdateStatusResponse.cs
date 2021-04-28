using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.BillSale
{
    public class UpdateStatusResponse:BaseResponse
    {
        public NoteModel Note { get; set; }
    }
}
