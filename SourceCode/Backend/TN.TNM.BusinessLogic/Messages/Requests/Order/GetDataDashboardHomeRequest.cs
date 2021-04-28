using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetDataDashboardHomeRequest : BaseRequest<GetDataDashboardHomeParameter>
    {
        public override GetDataDashboardHomeParameter ToParameter()
        {
            return new GetDataDashboardHomeParameter()
            {
                UserId = UserId
            };
        }
    }
}
