using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailAfterCreateNoteRequest : BaseRequest<SendEmailAfterCreateNoteParameter>
    {
        public List<string> EmployeeIdList { get; set; }
        public Guid LeadId { get; set; }
        public string CurrentUrl { get; set; }
        public string CurrentUsername { get; set; }
        public string NoteContent { get; set; }
        public List<string> FileNameArray { get; set; }
        public override SendEmailAfterCreateNoteParameter ToParameter()
        {
            return new SendEmailAfterCreateNoteParameter()
            {
                UserId = UserId,
                EmployeeIdList = EmployeeIdList,
                LeadId = LeadId,
                CurrentUrl = CurrentUrl,
                CurrentUsername = CurrentUsername,
                NoteContent = NoteContent,
                FileNameArray = FileNameArray
            };
        }
    }
}
