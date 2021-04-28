using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class GetOrderByOrderIdRequest:BaseRequest<GetOrderByOrderIdParameter>
    {
        public Guid OrderId { get; set; }

        public override GetOrderByOrderIdParameter ToParameter()
        {
            return new GetOrderByOrderIdParameter()
            {
                OrderId = OrderId,
                UserId =UserId
            };
        }
    }
}
