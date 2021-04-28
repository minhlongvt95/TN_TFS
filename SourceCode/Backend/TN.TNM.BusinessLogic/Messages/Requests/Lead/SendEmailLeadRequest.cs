using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class SendEmailLeadRequest : BaseRequest<SendEmailLeadParameter>
    {
        public string Title { get; set; }
        public string SendContent { get; set; }
        public bool IsSendEmailNow { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public DateTime? SendEmailHour { get; set; }
        public List<Guid> LeadIdList { get; set; }

        public override SendEmailLeadParameter ToParameter()
        {
            return new SendEmailLeadParameter()
            {
                Title = Title,
                SendContent = SendContent,
                IsSendEmailNow = IsSendEmailNow,
                SendEmailDate = SendEmailDate,
                SendEmailHour = SendEmailHour,
                LeadIdList = LeadIdList,
                UserId = UserId
            };
        }
    }
}
