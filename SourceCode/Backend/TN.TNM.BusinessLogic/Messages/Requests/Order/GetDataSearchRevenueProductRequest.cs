using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetDataSearchRevenueProductRequest: BaseRequest<GetDataSearchRevenueProductParameter>
    {
        public override GetDataSearchRevenueProductParameter ToParameter()
        {
            return new GetDataSearchRevenueProductParameter()
            {
                UserId = UserId
            };
        }
    }
}
