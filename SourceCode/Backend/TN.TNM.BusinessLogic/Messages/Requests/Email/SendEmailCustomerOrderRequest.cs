using System;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailCustomerOrderRequest : BaseRequest<SendEmailCustomerOrderParameter>
    {
        public Guid OrderId { get; set; }

        public override SendEmailCustomerOrderParameter ToParameter()
        {
            return new SendEmailCustomerOrderParameter()
            {
                OrderId = OrderId,
                UserId = UserId
            };
        }
    }
}
