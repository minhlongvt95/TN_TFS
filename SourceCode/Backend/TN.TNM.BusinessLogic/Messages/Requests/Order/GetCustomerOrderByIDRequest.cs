using System;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetCustomerOrderByIDRequest : BaseRequest<GetCustomerOrderByIDParameter>
    {
        public Guid CustomerOrderId { get; set; }

        public override GetCustomerOrderByIDParameter ToParameter() {
            return new GetCustomerOrderByIDParameter
            {
                CustomerOrderId = CustomerOrderId,
                UserId = this.UserId
            };

        }
    }
}
