using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Messages.Results.MenuBuild
{
    public class GetSubMenuModuleByMenuModuleCodeResult : BaseResult
    {
        public List<MenuBuildEntityModel> ListSubMenuModule { get; set; }
    }
}
