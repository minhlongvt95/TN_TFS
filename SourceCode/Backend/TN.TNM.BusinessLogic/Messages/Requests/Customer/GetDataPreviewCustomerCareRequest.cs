using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataPreviewCustomerCareRequest : BaseRequest<GetDataPreviewCustomerCareParameter>
    {
        public string Mode { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerCareId { get; set; }
        public override GetDataPreviewCustomerCareParameter ToParameter()
        {
            return new GetDataPreviewCustomerCareParameter()
            {
                UserId = UserId,
                Mode = Mode,
                CustomerId = CustomerId,
                CustomerCareId = CustomerCareId
            };
        }
    }
}
