using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Responses.MenuBuild;

namespace TN.TNM.BusinessLogic.Interfaces.MenuBuild
{
    public interface IMenuBuild
    {
        GetMenuBuildResponse GetMenuBuild(GetMenuBuildRequest request);
        GetMenuModuleResponse GetMenuModule(GetMenuModuleRequest request);
        CreateMenuBuildResponse CreateMenuBuild(CreateMenuBuildRequest request);

        GetSubMenuModuleByMenuModuleCodeResponse GetSubMenuModuleByMenuModuleCode(
            GetSubMenuModuleByMenuModuleCodeRequest request);

        GetMenuPageBySubMenuCodeResponse GetMenuPageBySubMenuCode(GetMenuPageBySubMenuCodeRequest request);
        UpdateIsPageDetailResponse UpdateIsPageDetail(UpdateIsPageDetailRequest request);
        UpdateMenuBuildResponse UpdateMenuBuild(UpdateMenuBuildRequest request);
    }
}
