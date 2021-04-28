using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataDashboardPotentialCustomerRequest : BaseRequest<GetDataDashboardPotentialCustomerParameter>
    {
        public override GetDataDashboardPotentialCustomerParameter ToParameter()
        {
            return new GetDataDashboardPotentialCustomerParameter
            {
                UserId = UserId
            };
        }
    }
}
