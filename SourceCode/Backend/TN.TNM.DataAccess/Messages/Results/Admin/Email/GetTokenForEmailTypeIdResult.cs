using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class GetTokenForEmailTypeIdResult : BaseResult
    {
        public List<EmailTemplateToken> ListEmailTemplateToken { get; set; }
    }
}
