using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Email
{
    public class CreateUpdateEmailTemplateMasterdataRequest: BaseRequest<CreateUpdateEmailTemplateMasterdataParameter>
    {
        public Guid? EmailTemplateId { get; set; }
        public override CreateUpdateEmailTemplateMasterdataParameter ToParameter()
        {
            return new CreateUpdateEmailTemplateMasterdataParameter()
            {
               EmailTemplateId = EmailTemplateId,           
            };
        }
    }
}
