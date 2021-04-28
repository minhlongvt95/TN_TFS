using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class CreateMenuBuildRequest : BaseRequest<CreateMenuBuildParameter>
    {
        public MenuBuildEntityModel MenuBuild { get; set; }

        public override CreateMenuBuildParameter ToParameter()
        {
            return new CreateMenuBuildParameter()
            {
                UserId = UserId,
                MenuBuild = MenuBuild
            };
        }
    }
}
