using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Email
{
    public class SearchEmailConfigMasterdataRequest: BaseRequest<SearchEmailConfigMasterdataParameter>
    {
        public override SearchEmailConfigMasterdataParameter ToParameter()
        {
            return new SearchEmailConfigMasterdataParameter
            {
                UserId = UserId
            };
        }
    }
}
