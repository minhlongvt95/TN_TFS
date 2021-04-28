using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetListOrderByLocalPointRequest : BaseRequest<GetListOrderByLocalPointParameter>
    {
        public Guid LocalPointId { get; set; }
        public override GetListOrderByLocalPointParameter ToParameter()
        {
            return new GetListOrderByLocalPointParameter()
            {
                UserId = UserId,
                LocalPointId = LocalPointId
            };
        }
    }
}
