using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Email
{
    public class SearchEmailTemplateParameter: BaseParameter
    {
        public string EmailTemplateName { get; set; }
        public string EmailTemplateTitle { get; set; }
        public List<Guid> ListEmailTemplateTypeId { get; set; }
        public List<Guid> ListEmailTemplateStatusId { get; set; }
    }
}
