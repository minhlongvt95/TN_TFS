using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class SendApprovalRequest : BaseRequest<SendApprovalParameter>
    {
        public Guid CustomerId { get; set; }

        public override SendApprovalParameter ToParameter()
        {
            return new SendApprovalParameter
            {
                CustomerId = this.CustomerId,
                UserId = this.UserId,
            };
        }
    }
}
