using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class UpdateStatusResult:BaseResult
    {
        public NoteEntityModel Note { get; set; }
    }
}
