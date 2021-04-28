using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class SendSMSLeadParameter : BaseParameter
    {
        public string SendContent { get; set; }
        public bool IsSendSMSNow { get; set; }
        public DateTime? SendSMSDate { get; set; }
        public DateTime? SendSMSHour { get; set; }
        public List<Guid> LeadIdList { get; set; }//LeadId or CustomerId
    }
}
