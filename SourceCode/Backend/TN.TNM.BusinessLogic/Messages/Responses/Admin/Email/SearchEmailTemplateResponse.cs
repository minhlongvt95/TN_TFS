using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Email;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Email
{
    public class SearchEmailTemplateResponse: BaseResponse
    {
        public List<Models.Email.EmailTemplateModel> ListEmailTemplateModel { get; set; }
    }
}
