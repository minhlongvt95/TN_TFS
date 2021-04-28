using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetDataSearchLeadRequest : BaseRequest<GetDataSearchLeadParameter>
    {
        public override GetDataSearchLeadParameter ToParameter()
        {
            return new GetDataSearchLeadParameter
            {
                UserId = UserId
            };
        }
    }
}
