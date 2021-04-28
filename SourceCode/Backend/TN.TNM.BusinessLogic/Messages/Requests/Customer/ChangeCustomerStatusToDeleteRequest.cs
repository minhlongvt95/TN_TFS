using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class ChangeCustomerStatusToDeleteRequest : BaseRequest<ChangeCustomerStatusToDeleteParameter>
    {
        public Guid CustomerId { get; set; }
        public override ChangeCustomerStatusToDeleteParameter ToParameter()
        {
            return new ChangeCustomerStatusToDeleteParameter
            {
                CustomerId = this.CustomerId
            };
        }
    }
}
