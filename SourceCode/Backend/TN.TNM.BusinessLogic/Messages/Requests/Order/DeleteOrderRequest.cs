using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class DeleteOrderRequest : BaseRequest<DeleteOrderParameter>
    {
        public Guid OrderId { get; set; }

        public override DeleteOrderParameter ToParameter()
        {
            return new DeleteOrderParameter()
            {
                OrderId = OrderId,
                UserId = UserId
            };
        }
    }
}
