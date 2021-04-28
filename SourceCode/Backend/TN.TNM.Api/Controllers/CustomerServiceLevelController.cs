using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.CustomerServiceLevel;

namespace TN.TNM.Api.Controllers
{
    public class CustomerServiceLevelController
    {
        private readonly ICustomerServiceLevel _iCustomerServiceLevel;
        public CustomerServiceLevelController(ICustomerServiceLevel iCustomerServiceLevel)
        {
            this._iCustomerServiceLevel = iCustomerServiceLevel;
        }

        /// <summary>
        /// AddLevelCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/admin/customerservicelevel")]
        [Authorize(Policy = "Member")]
        public AddLevelCustomerResponse AddLevelCustomer([FromBody]AddLevelCustomerRequest  request)
        {
            return this._iCustomerServiceLevel.AddLevelCustomer(request);
        }
        /// <summary>
        /// GetConfigCustomerServiceLevel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/admin/getCustomerservicelevel")]
        [Authorize(Policy = "Member")]
        public GetConfigCustomerServiceLevelResponse GetConfigCustomerServiceLevel([FromBody]GetConfigCustomerServiceLevelRequest  request)
        {
            return this._iCustomerServiceLevel.GetConfigCustomerServiceLevel(request);
        }
        /// <summary>
        /// UpdateConfigCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/admin/updateConfigCustomer")]
        [Authorize(Policy = "Member")]
        public UpdateConfigCustomerResponse UpdateConfigCustomer([FromBody]UpdateConfigCustomerRequest  request)
        {
            return this._iCustomerServiceLevel.UpdateConfigCustomer(request);
        }
    }
}
