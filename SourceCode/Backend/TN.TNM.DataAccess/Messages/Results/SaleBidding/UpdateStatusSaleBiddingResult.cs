using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class UpdateStatusSaleBiddingResult:BaseResult
    {
        public NoteEntityModel Note { get; set; }
    }
}
