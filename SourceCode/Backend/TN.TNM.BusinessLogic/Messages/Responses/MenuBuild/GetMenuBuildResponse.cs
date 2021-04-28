using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Responses.MenuBuild
{
    public class GetMenuBuildResponse : BaseResponse
    {
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }
    }
}
