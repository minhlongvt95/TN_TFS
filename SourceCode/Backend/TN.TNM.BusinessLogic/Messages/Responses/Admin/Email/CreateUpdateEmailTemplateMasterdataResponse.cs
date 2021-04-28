using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Results.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Email
{
    public class CreateUpdateEmailTemplateMasterdataResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListEmailType { get; set; }
        public List<Models.Category.CategoryModel> ListEmailStatus { get; set; }
        public Models.Email.EmailTemplateModel EmailTemplateModel { get; set; }
        public List<Models.Email.EmailTemplateTokenModel> ListEmailTemplateToken { get; set; }
        public List<string> ListEmailToCC { get; set; }
    }
}
