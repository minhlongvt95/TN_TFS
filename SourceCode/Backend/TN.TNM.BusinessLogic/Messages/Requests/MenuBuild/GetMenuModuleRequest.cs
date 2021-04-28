using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class GetMenuModuleRequest : BaseRequest<GetMenuModuleParameter>
    {
        public override GetMenuModuleParameter ToParameter()
        {
            return new GetMenuModuleParameter()
            {
                UserId = UserId
            };
        }
    }
}
