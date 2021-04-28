using System;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailVendorOrderRequest : BaseRequest<SendEmailVendorOrderParameter>
    {
        public Guid VendorOrderId { get; set; }

        public override SendEmailVendorOrderParameter ToParameter()
        {
            return new SendEmailVendorOrderParameter()
            {
                VendorOrderId = VendorOrderId,
                UserId = this.UserId
            };
        }

    }
}
