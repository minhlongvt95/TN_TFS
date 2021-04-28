using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;
using TN.TNM.DataAccess.Messages.Results.Admin.Email;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmailConfigurationDataAccess
    {
        CreateUpdateEmailTemplateMasterdataResult CreateUpdateEmailTemplateMasterdata(
            CreateUpdateEmailTemplateMasterdataParameter parameter);

        CreateUpdateEmailTemplateResult CreateUpdateEmailTemplate(CreateUpdateEmailTemplateParameter parameter);
        SearchEmailConfigMasterdataResult SearchEmailConfigMasterdata(SearchEmailConfigMasterdataParameter parameter);
        SearchEmailTemplateResult SearchEmailTemplate(SearchEmailTemplateParameter parameter);
        SendEmailResult SendEmail(SendEmailParameter parameter);
        GetTokenForEmailTypeIdResult GetTokenForEmailTypeId(GetTokenForEmailTypeIdParameter parameter);
    }
}
