using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Email;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Email
{
    public interface IEmailConfig
    {
        CreateUpdateEmailTemplateMasterdataResponse CreateUpdateEmailTemplateMasterdata(CreateUpdateEmailTemplateMasterdataRequest request);
        CreateUpdateEmailTemplateResponse CreateUpdateEmailTemplate(CreateUpdateEmailTemplateRequest request);
        SearchEmailConfigMasterdataResponse SearchEmailConfigMasterdata(SearchEmailConfigMasterdataRequest request);
        SearchEmailTemplateResponse SearchEmailTemplate(SearchEmailTemplateRequest request);
        SendEmailResponse SendEmail(SendEmailRequest request);
        GetTokenForEmailTypeIdResponse GetTokenForEmailTypeId(GetTokenForEmailTypeIdRequest request);
    }
}
