using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataCreateLeadRequest: BaseRequest<GetDataCreateLeadParameter>
    {
        public override GetDataCreateLeadParameter ToParameter()
        {
            return new GetDataCreateLeadParameter
            {
                UserId = UserId
            };
        }
    }
}
