using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Requests.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Responses.MenuBuild;

namespace TN.TNM.Api.Controllers
{
    public class MenuBuildController : Controller
    {
        private readonly IMenuBuild _iMenuBuild;
        public MenuBuildController(IMenuBuild iMenuBuild)
        {
            this._iMenuBuild = iMenuBuild;
        }

        [HttpPost]
        [Route("api/menubuild/getMenuBuild")]
        [Authorize(Policy = "Member")]
        public GetMenuBuildResponse GetMenuBuild([FromBody]GetMenuBuildRequest request)
        {
            return this._iMenuBuild.GetMenuBuild(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/getMenuModule")]
        [Authorize(Policy = "Member")]
        public GetMenuModuleResponse GetMenuModule([FromBody]GetMenuModuleRequest request)
        {
            return this._iMenuBuild.GetMenuModule(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/createMenuBuild")]
        [Authorize(Policy = "Member")]
        public CreateMenuBuildResponse CreateMenuBuild([FromBody]CreateMenuBuildRequest request)
        {
            return this._iMenuBuild.CreateMenuBuild(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/getSubMenuModuleByMenuModuleCode")]
        [Authorize(Policy = "Member")]
        public GetSubMenuModuleByMenuModuleCodeResponse GetSubMenuModuleByMenuModuleCode([FromBody]GetSubMenuModuleByMenuModuleCodeRequest request)
        {
            return this._iMenuBuild.GetSubMenuModuleByMenuModuleCode(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/getMenuPageBySubMenuCode")]
        [Authorize(Policy = "Member")]
        public GetMenuPageBySubMenuCodeResponse GetMenuPageBySubMenuCode([FromBody]GetMenuPageBySubMenuCodeRequest request)
        {
            return this._iMenuBuild.GetMenuPageBySubMenuCode(request);
        }

        //
        [HttpPost]
        [Route("api/menubuild/updateIsPageDetail")]
        [Authorize(Policy = "Member")]
        public UpdateIsPageDetailResponse UpdateIsPageDetail([FromBody]UpdateIsPageDetailRequest request)
        {
            return this._iMenuBuild.UpdateIsPageDetail(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/menubuild/updateMenuBuild")]
        [Authorize(Policy = "Member")]
        public UpdateMenuBuildResponse UpdateMenuBuild([FromBody]UpdateMenuBuildRequest request)
        {
            return this._iMenuBuild.UpdateMenuBuild(request);
        }

    }
}