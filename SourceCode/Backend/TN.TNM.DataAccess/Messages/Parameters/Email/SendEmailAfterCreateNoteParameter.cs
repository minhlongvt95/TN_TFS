using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailAfterCreateNoteParameter : BaseParameter
    {
        public List<string> EmployeeIdList { get; set; }
        public Guid LeadId { get; set; }
        public string CurrentUrl { get; set; }
        public string CurrentUsername { get; set; }
        public string NoteContent { get; set; }
        public List<string> FileNameArray { get; set; }
    }
}
