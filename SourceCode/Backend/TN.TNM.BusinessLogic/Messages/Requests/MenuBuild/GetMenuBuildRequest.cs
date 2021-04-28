using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class GetMenuBuildRequest : BaseRequest<GetMenuBuildParameter>
    {
        public override GetMenuBuildParameter ToParameter()
        {
            return new GetMenuBuildParameter()
            {
                UserId = UserId
            };
        }
    }
}
