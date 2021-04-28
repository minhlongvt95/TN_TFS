using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Email;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Email
{
    public class GetTokenForEmailTypeIdResponse : BaseResponse
    {
        public List<EmailTemplateTokenModel> ListEmailTemplateToken { get; set; }
    }
}
