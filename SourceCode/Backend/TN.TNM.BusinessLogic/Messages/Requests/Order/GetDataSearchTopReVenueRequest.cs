using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetDataSearchTopReVenueRequest: BaseRequest<GetDataSearchTopReVenueParameter>
    {
        public override GetDataSearchTopReVenueParameter ToParameter()
        {
            return new GetDataSearchTopReVenueParameter()
            {
                UserId = UserId
            };
        }
    }
}
