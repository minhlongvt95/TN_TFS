using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.District;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.District;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.District;

namespace TN.TNM.Api.Controllers
{
    public class DistrictController : Controller
    {
        private readonly IDistrict iDistrict;

        public DistrictController(IDistrict _iDistrict)
        {
            this.iDistrict = _iDistrict;
        }

        /// <summary>
        /// Get all District info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/district/getAllDistrictByProvinceId")]
        [Authorize(Policy = "Member")]
        public GetAllDistrictByProvinceIdResponse GetAllDistrictByProvinceId([FromBody]GetAllDistrictByProvinceIdRequest request)
        {
            return this.iDistrict.GetAllDistrictByProvinceId(request);
        }
    }
}