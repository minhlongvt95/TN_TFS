using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class ChangeStatusSupportRequest : BaseRequest<ChangeStatusSupportParameter>
    {
        public Guid CustomerId { get; set; }
        public Guid StatusSupportId { get; set; }

        public override ChangeStatusSupportParameter ToParameter()
        {
            return new ChangeStatusSupportParameter()
            {
                UserId = UserId,
                CustomerId = CustomerId,
                StatusSupportId = StatusSupportId
            };
        }
    }
}
