using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.DashboardRequest;
using TN.TNM.BusinessLogic.Messages.Requests.DashboardRequest;
using TN.TNM.BusinessLogic.Messages.Responses.DashboardRequest;

namespace TN.TNM.Api.Controllers
{
    
    public class DashboardRequestController : Controller
    {
        private readonly IDashboardRequest _iDashboardRequest;
        public DashboardRequestController(IDashboardRequest iDashboardRequest)
        {
            this._iDashboardRequest = iDashboardRequest;
        }

        [HttpPost]
        [Route("api/dashboardRequest/getAllRequest")]
        [Authorize(Policy = "Member")]
        public GetAllRequestResponse GetAllRequest([FromBody]GetAllRequestRequest request)
        {
            return this._iDashboardRequest.GetAllRequest(request);
        }

        [HttpPost]
        [Route("api/dashboardRequest/searchAllRequest")]
        [Authorize(Policy = "Member")]
        public SearchAllRequestResponse SearchAllRequest([FromBody]SearchAllRequestRequest request)
        {
            return this._iDashboardRequest.SearchAllRequest(request);
        }

    }
}