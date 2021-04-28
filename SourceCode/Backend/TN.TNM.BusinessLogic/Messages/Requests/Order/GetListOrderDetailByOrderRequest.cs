using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetListOrderDetailByOrderRequest : BaseRequest<GetListOrderDetailByOrderParameter>
    {
        public Guid OrderId { get; set; }

        public override GetListOrderDetailByOrderParameter ToParameter()
        {
            return new GetListOrderDetailByOrderParameter()
            {
                UserId = UserId,
                OrderId = OrderId
            };
        }
    }
}
