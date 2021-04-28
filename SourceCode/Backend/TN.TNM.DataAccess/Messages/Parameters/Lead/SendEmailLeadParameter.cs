using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class SendEmailLeadParameter : BaseParameter
    {
        public string Title { get; set; }
        public string SendContent { get; set; }
        public bool IsSendEmailNow { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public DateTime? SendEmailHour { get; set; }
        public List<Guid> LeadIdList { get; set; }
    }
}
