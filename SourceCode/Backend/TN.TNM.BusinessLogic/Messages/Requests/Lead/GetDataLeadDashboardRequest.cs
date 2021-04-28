using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataLeadDashboardRequest : BaseRequest<GetDataLeadDashboardParameter>
    {
        public override GetDataLeadDashboardParameter ToParameter()
        {
            return new GetDataLeadDashboardParameter()
            {
                UserId = UserId,
            };
        }
    }
}
