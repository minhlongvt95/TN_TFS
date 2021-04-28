using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Responses.MenuBuild
{
    public class GetMenuModuleResponse : BaseResponse
    {
        public List<MenuBuildEntityModel> ListMenuModule { get; set; }
    }
}
