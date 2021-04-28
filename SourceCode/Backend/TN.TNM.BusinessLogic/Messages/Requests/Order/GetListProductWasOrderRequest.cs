using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetListProductWasOrderRequest : BaseRequest<GetListProductWasOrderParameter>
    {
        public Guid LocalPointId { get; set; }

        public override GetListProductWasOrderParameter ToParameter()
        {
            return new GetListProductWasOrderParameter()
            {
                UserId = UserId,
                LocalPointId = LocalPointId
            };
        }
    }
}
