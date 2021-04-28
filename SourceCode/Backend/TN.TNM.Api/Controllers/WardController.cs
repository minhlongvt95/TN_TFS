using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Ward;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Ward;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Ward;

namespace TN.TNM.Api.Controllers
{
    public class WardController : Controller
    {
        private readonly IWard iward;

        public WardController(IWard _iward)
        {
            this.iward = _iward;
        }

        /// <summary>
        /// Get all Ward info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ward/getAllWardByDistrictId")]
        [Authorize(Policy = "Member")]
        public GetAllWardByDistrictIdResponse GetAllWardByDistrictId([FromBody]GetAllWardByDistrictIdRequest request)
        {
            return this.iward.GetAllWardByDistrictId(request);
        }
    }
}