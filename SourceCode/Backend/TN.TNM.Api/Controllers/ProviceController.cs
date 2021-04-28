using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Province;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Province;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Province;

namespace TN.TNM.Api.Controllers
{
    public class ProviceController : Controller
    {
        private readonly IProvince iProvince;

        public ProviceController(IProvince _iProvince)
        {
            this.iProvince = _iProvince;
        }

        /// <summary>
        /// Get all Province info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/province/getAllProvince")]
        [Authorize(Policy = "Member")]
        public GetAllProvinceResponse GetAllProvince([FromBody]GetAllProvinceRequest request)
        {
            return this.iProvince.GetAllProvince(request);
        }
    }
}