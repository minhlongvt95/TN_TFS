using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class SearchEmailTemplateResult:BaseResult
    {
        public List<DataAccess.Databases.Entities.EmailTemplate> ListEmailTemplate { get; set; }
    }
}
