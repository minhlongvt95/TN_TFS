using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization;

namespace TN.TNM.Api.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IOrganization iOrganization;
        public OrganizationController(IOrganization _iOrganization)
        {
            this.iOrganization = _iOrganization;
        }

        /// <summary>
        /// Get all Organization
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getAllOrganization")]
        [Authorize(Policy = "Member")]
        public GetAllOrganizationResponse GetAllOrganization(GetAllOrganizationRequest request)
        {
            return this.iOrganization.GetAllOrganization(request);
        }

        /// <summary>
        /// Create new Organization
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/createOrganization")]
        [Authorize(Policy = "Member")]
        public CreateOrganizationResponse CreateOrganization([FromBody]CreateOrganizationRequest request)
        {
            return this.iOrganization.CreateOrganization(request);
        }

        /// <summary>
        /// Get organization info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getOrganizationById")]
        [Authorize(Policy = "Member")]
        public GetOrganizationByIdResponse GetOrganizationById([FromBody]GetOrganizationByIdRequest request)
        {
            return this.iOrganization.GetOrganizationById(request);
        }

        /// <summary>
        /// Edit an organization
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/editOrganizationById")]
        [Authorize(Policy = "Member")]
        public EditOrganizationByIdResponse EditOrganizationById([FromBody]EditOrganizationByIdRequest request)
        {
            return this.iOrganization.EditOrganizationById(request);
        }

        /// <summary>
        /// Delete an organization
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/deleteOrganizationById")]
        [Authorize(Policy = "Member")]
        public DeleteOrganizationByIdResponse DeleteOrganizationById([FromBody]DeleteOrganizationByIdRequest request)
        {
            return this.iOrganization.DeleteOrganizationById(request);
        }

        /// <summary>
        /// Get all Organization code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getAllOrganizationCode")]
        [Authorize(Policy = "Member")]
        public GetAllOrganizationCodeResponse GetAllOrganizationCode([FromBody]GetAllOrganizationCodeRequest request)
        {
            return this.iOrganization.GetAllOrganizationCode(request);
        }

        /// <summary>
        /// Get all Financialindependence Organization
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getFinancialindependenceOrg")]
        [Authorize(Policy = "Member")]
        public GetFinancialindependenceOrgResponse GetFinancialindependenceOrg([FromBody]GetFinancialindependenceOrgRequest request)
        {
            return this.iOrganization.GetFinancialindependenceOrg(request);
        }

        /// <summary>
        /// Get Children Organization By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getChildrenOrganizationById")]
        [Authorize(Policy = "Member")]
        public GetChildrenOrganizationByIdResponse GetChildrenOrganizationById([FromBody]GetChildrenOrganizationByIdRequest request)
        {
            return this.iOrganization.GetChildrenOrganizationById(request);
        }

        /// <summary>
        /// Get Organization By EmployeeId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getOrganizationByEmployeeId")]
        [Authorize(Policy = "Member")]
        public GetOrganizationByEmployeeIdResponse GetOrganizationByEmployeeId([FromBody]GetOrganizationByEmployeeIdRequest request)
        {
            return this.iOrganization.GetOrganizationByEmployeeId(request);
        }

        /// <summary>
        /// Get Children By OrganizationId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/organization/getChildrenByOrganizationId")]
        [Authorize(Policy = "Member")]
        public GetChildrenByOrganizationIdResponse GetChildrenByOrganizationId([FromBody]GetChildrenByOrganizationIdRequest request)
        {
            return this.iOrganization.GetChildrenByOrganizationId(request);
        }

        // 
        [HttpPost]
        [Route("api/organization/UpdateOrganizationById")]
        [Authorize(Policy = "Member")]
        public UpdateOrganizationByIdResponse UpdateOrganizationById([FromBody]UpdateOrganizationByIdRequest request)
        {
            return this.iOrganization.UpdateOrganizationById(request);
        }

        // 
        [HttpPost]
        [Route("api/organization/getOrganizationByUser")]
        [Authorize(Policy = "Member")]
        public GetOrganizationByUserResponse GetOrganizationByUser([FromBody]GetOrganizationByUserRequest request)
        {
            return this.iOrganization.GetOrganizationByUser(request);
        }
    }
}
