using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Email
{
    public class CreateUpdateEmailTemplateRequest: BaseRequest<CreateUpdateEmailTemplateParameter>
    {
        public DataAccess.Databases.Entities.EmailTemplate EmailTemplateEntityModel { get; set; }
        public List<string> ListEmailToCC { get; set; }
        public override CreateUpdateEmailTemplateParameter ToParameter()
        {
            return new CreateUpdateEmailTemplateParameter
            {
                EmailTemplateEntityModel = EmailTemplateEntityModel,
                ListEmailToCC = ListEmailToCC,
                UserId = UserId
            };
        }

    }
}
