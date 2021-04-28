using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Email
{
    public class SearchEmailTemplateRequest: BaseRequest<SearchEmailTemplateParameter>
    {
        public string EmailTemplateName { get; set; }
        public string EmailTemplateTitle { get; set; }
        public List<Guid> ListEmailTemplateTypeId { get; set; }
        public List<Guid> ListEmailTemplateStatusId { get; set; }

        public override SearchEmailTemplateParameter ToParameter()
        {
            return new SearchEmailTemplateParameter()
            {
                EmailTemplateName = EmailTemplateName,
                EmailTemplateTitle = EmailTemplateTitle,
                ListEmailTemplateStatusId = ListEmailTemplateStatusId,
                ListEmailTemplateTypeId = ListEmailTemplateTypeId
            };
        }
    }
}
