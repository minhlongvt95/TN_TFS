using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetLocalPointByLocalAddressRequest : BaseRequest<GetLocalPointByLocalAddressParameter>
    {
        public Guid LocalAddressId { get; set; }

        public override GetLocalPointByLocalAddressParameter ToParameter()
        {
            return new GetLocalPointByLocalAddressParameter()
            {
                UserId = UserId,
                LocalAddressId = LocalAddressId
            };
        }
    }
}
