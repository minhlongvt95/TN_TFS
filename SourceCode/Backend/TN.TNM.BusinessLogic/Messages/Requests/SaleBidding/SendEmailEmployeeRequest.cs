using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class SendEmailEmployeeRequest:BaseRequest<SendEmailEmployeeParameter>
    {
        public Guid SaleBiddingId { get; set; }
        public override SendEmailEmployeeParameter ToParameter()
        {
            return new SendEmailEmployeeParameter()
            {
                SaleBiddingId = SaleBiddingId,
                UserId =UserId
            };
        }
    }
}
