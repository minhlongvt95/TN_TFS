using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Requests.MenuBuild;
using TN.TNM.BusinessLogic.Messages.Responses.MenuBuild;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.MenuBuild
{
    public class MenuBuildFactory : BaseFactory, IMenuBuild
    {
        private IMenuBuildDataAccess iMenuBuildDataAccess;

        public MenuBuildFactory(IMenuBuildDataAccess _iMenuBuildDataAccess)
        {
            this.iMenuBuildDataAccess = _iMenuBuildDataAccess;
        }

        public GetMenuBuildResponse GetMenuBuild(GetMenuBuildRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.GetMenuBuild(parameter);
                var response = new GetMenuBuildResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new GetMenuBuildResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMenuModuleResponse GetMenuModule(GetMenuModuleRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.GetMenuModule(parameter);
                var response = new GetMenuModuleResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListMenuModule = result.ListMenuModule
                };
                return response;
            }
            catch (Exception e)
            {
                return new GetMenuModuleResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateMenuBuildResponse CreateMenuBuild(CreateMenuBuildRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.CreateMenuBuild(parameter);
                var response = new CreateMenuBuildResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new CreateMenuBuildResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetSubMenuModuleByMenuModuleCodeResponse GetSubMenuModuleByMenuModuleCode(
            GetSubMenuModuleByMenuModuleCodeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.GetSubMenuModuleByMenuModuleCode(parameter);
                var response = new GetSubMenuModuleByMenuModuleCodeResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListSubMenuModule = result.ListSubMenuModule
                };
                return response;
            }
            catch (Exception e)
            {
                return new GetSubMenuModuleByMenuModuleCodeResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMenuPageBySubMenuCodeResponse GetMenuPageBySubMenuCode(GetMenuPageBySubMenuCodeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.GetMenuPageBySubMenuCode(parameter);
                var response = new GetMenuPageBySubMenuCodeResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListMenuPage = result.ListMenuPage
                };
                return response;
            }
            catch (Exception e)
            {
                return new GetMenuPageBySubMenuCodeResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateIsPageDetailResponse UpdateIsPageDetail(UpdateIsPageDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.UpdateIsPageDetail(parameter);
                var response = new UpdateIsPageDetailResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new UpdateIsPageDetailResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateMenuBuildResponse UpdateMenuBuild(UpdateMenuBuildRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iMenuBuildDataAccess.UpdateMenuBuild(parameter);
                var response = new UpdateMenuBuildResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new UpdateMenuBuildResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

    }
}
