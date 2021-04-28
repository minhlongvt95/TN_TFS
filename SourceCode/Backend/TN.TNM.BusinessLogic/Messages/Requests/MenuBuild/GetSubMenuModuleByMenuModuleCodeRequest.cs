using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class GetSubMenuModuleByMenuModuleCodeRequest : BaseRequest<GetSubMenuModuleByMenuModuleCodeParameter>
    {
        public string MenuModuleCode { get; set; }

        public override GetSubMenuModuleByMenuModuleCodeParameter ToParameter()
        {
            return new GetSubMenuModuleByMenuModuleCodeParameter()
            {
                UserId = UserId,
                MenuModuleCode = MenuModuleCode
            };
        }
    }
}
