using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class DeleteCustomerAdditionalRequest : BaseRequest<DeleteCustomerAdditionalParameter>
    {
        public Guid CustomerAdditionalInformationId { get; set; }
        public Guid CustomerId { get; set; }

        public override DeleteCustomerAdditionalParameter ToParameter()
        {
            return new DeleteCustomerAdditionalParameter()
            {
                CustomerAdditionalInformationId = CustomerAdditionalInformationId,
                CustomerId = CustomerId,
                UserId = UserId
            };
        }
    }
}
