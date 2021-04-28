using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class SendSMSLeadRequest : BaseRequest<SendSMSLeadParameter>
    {
        public string SendContent { get; set; }
        public bool IsSendSMSNow { get; set; }
        public DateTime? SendSMSDate { get; set; }
        public DateTime? SendSMSHour { get; set; }
        public List<Guid> LeadIdList { get; set; }

        public override SendSMSLeadParameter ToParameter()
        {
            return new SendSMSLeadParameter()
            {
                SendContent = SendContent,
                IsSendSMSNow = IsSendSMSNow,
                SendSMSDate = SendSMSDate,
                SendSMSHour = SendSMSHour,
                LeadIdList = LeadIdList,
                UserId = UserId
            };
        }
    }
}
