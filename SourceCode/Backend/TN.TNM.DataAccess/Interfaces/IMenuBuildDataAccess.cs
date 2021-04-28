using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;
using TN.TNM.DataAccess.Messages.Results.MenuBuild;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IMenuBuildDataAccess
    {
        GetMenuBuildResult GetMenuBuild(GetMenuBuildParameter parameter);
        GetMenuModuleResult GetMenuModule(GetMenuModuleParameter parameter);
        CreateMenuBuildResult CreateMenuBuild(CreateMenuBuildParameter parameter);

        GetSubMenuModuleByMenuModuleCodeResult GetSubMenuModuleByMenuModuleCode(
            GetSubMenuModuleByMenuModuleCodeParameter parameter);

        GetMenuPageBySubMenuCodeResult GetMenuPageBySubMenuCode(GetMenuPageBySubMenuCodeParameter parameter);
        UpdateIsPageDetailResult UpdateIsPageDetail(UpdateIsPageDetailParameter parameter);
        UpdateMenuBuildResult UpdateMenuBuild(UpdateMenuBuildParameter parameter);
    }
}
