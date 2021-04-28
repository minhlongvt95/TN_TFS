using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Email;

namespace TN.TNM.Api.Controllers
{
    public class EmailConfigController : Controller
    {
        private readonly IEmailConfig iEmailConfig;
        public EmailConfigController(IEmailConfig _iEmailConfig)
        {
            this.iEmailConfig = _iEmailConfig;
        }

        [HttpPost]
        [Route("api/emailConfig/createUpdateEmailTemplateMasterdata")]
        [Authorize(Policy = "Member")]
        public CreateUpdateEmailTemplateMasterdataResponse CreateUpdateEmailTemplateMasterdata([FromBody]CreateUpdateEmailTemplateMasterdataRequest request)
        {
            return this.iEmailConfig.CreateUpdateEmailTemplateMasterdata(request);
        }

        [HttpPost]
        [Route("api/emailConfig/createUpdateEmailTemplate")]
        [Authorize(Policy = "Member")]
        public CreateUpdateEmailTemplateResponse CreateUpdateEmailTemplate([FromBody]CreateUpdateEmailTemplateRequest request)
        {
            return this.iEmailConfig.CreateUpdateEmailTemplate(request);
        }

        [HttpPost]
        [Route("api/emailConfig/searchEmailConfigMasterdata")]
        [Authorize(Policy = "Member")]
        public SearchEmailConfigMasterdataResponse SearchEmailConfigMasterdata([FromBody]SearchEmailConfigMasterdataRequest request)
        {
            return this.iEmailConfig.SearchEmailConfigMasterdata(request);
        }

        [HttpPost]
        [Route("api/emailConfig/searchEmailTemplate")]
        [Authorize(Policy = "Member")]
        public SearchEmailTemplateResponse SearchEmailTemplate([FromBody]SearchEmailTemplateRequest request)
        {
            return this.iEmailConfig.SearchEmailTemplate(request);
        }

        [HttpPost]
        [Route("api/emailConfig/sendEmail")]
        [Authorize(Policy = "Member")]
        public SendEmailResponse SendEmail([FromBody]SendEmailRequest request)
        {
            return this.iEmailConfig.SendEmail(request);
        }
        
        [HttpPost]
        [Route("api/emailConfig/getTokenForEmailTypeId")]
        [Authorize(Policy = "Member")]
        public GetTokenForEmailTypeIdResponse GetTokenForEmailTypeId([FromBody]GetTokenForEmailTypeIdRequest request)
        {
            return this.iEmailConfig.GetTokenForEmailTypeId(request);
        }
    }
}