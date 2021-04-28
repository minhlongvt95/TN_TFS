using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendMailSupportSaleBiddingRequest:BaseRequest<SendMailSupportSaleBiddingParameter>
    {
        public string SaleBiddingCode { get; set; }
        public string SaleBiddingName { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public override SendMailSupportSaleBiddingParameter ToParameter()
        {
            return new SendMailSupportSaleBiddingParameter()
            {
                ListEmployeeId = ListEmployeeId,
                SaleBiddingCode = SaleBiddingCode,
                SaleBiddingName = SaleBiddingName,
                UserId = UserId
            };
        }
    }
}
