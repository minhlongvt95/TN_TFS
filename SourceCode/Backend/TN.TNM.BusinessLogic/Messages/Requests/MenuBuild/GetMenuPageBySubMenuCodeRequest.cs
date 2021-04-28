using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class GetMenuPageBySubMenuCodeRequest : BaseRequest<GetMenuPageBySubMenuCodeParameter>
    {
        public string SubMenuCode { get; set; }

        public override GetMenuPageBySubMenuCodeParameter ToParameter()
        {
            return new GetMenuPageBySubMenuCodeParameter()
            {
                UserId = UserId,
                SubMenuCode = SubMenuCode
            };
        }
    }
}
