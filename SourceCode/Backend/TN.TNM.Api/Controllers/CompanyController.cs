using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.CompanyConfig;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Responses.CompanyConfig;

namespace TN.TNM.Api.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompany iCompany;
        public CompanyController(ICompany _iCompany)
        {
            this.iCompany = _iCompany;
        }

        /// <summary>
        /// Get all company info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/getAllCompany")]
        [Authorize(Policy = "Member")]
        public GetAllCompanyResponse GetAllCompany(GetAllCompanyRequest request)
        {
            return this.iCompany.GetAllCompany(request);
        }
        /// <summary>
        /// Get all company info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/getCompanyConfig")]
        [Authorize(Policy = "Member")]
        public GetCompanyConfigResponse GetCompanyConfig(GetCompanyConfigRequest request)
        {
            return this.iCompany.GetCompanyConfig(request);
        }
        /// <summary>
        /// Edit Company Config
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/editCompanyConfig")]
        [Authorize(Policy = "Member")]
        public EditCompanyConfigResponse EditCompanyConfig([FromBody]EditCompanyConfigRequest request)
        {
            return this.iCompany.EditCompanyConfig(request);
        }
        /// <summary>
        /// Get All System Parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/getAllSystemParameter")]
        [Authorize(Policy = "Member")]
        public GetAllSystemParameterResponse GetAllSystemParameter([FromBody]GetAllSystemParameterRequest request)
        {
            return this.iCompany.GetAllSystemParameter(request);
        }
        /// <summary>
        /// Change System Parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/changeSystemParameter")]
        [Authorize(Policy = "Member")]
        public ChangeSystemParameterResponse ChangeSystemParameter([FromBody]ChangeSystemParameterRequest request)
        {
            return this.iCompany.ChangeSystemParameter(request);
        }
    }
}