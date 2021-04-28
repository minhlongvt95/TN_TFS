using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetMasterDataOrderDetailRequest : BaseRequest<GetMasterDataOrderDetailParameter>
    {
        public Guid OrderId { get; set; }

        public override GetMasterDataOrderDetailParameter ToParameter()
        {
            return new GetMasterDataOrderDetailParameter()
            {
                UserId = UserId,
                OrderId = OrderId
            };
        }
    }
}
