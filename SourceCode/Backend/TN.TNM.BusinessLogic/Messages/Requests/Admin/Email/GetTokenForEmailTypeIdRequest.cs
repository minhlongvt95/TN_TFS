using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Email
{
    public class GetTokenForEmailTypeIdRequest : BaseRequest<GetTokenForEmailTypeIdParameter>
    {
        public Guid EmailTemplateTypeId { get; set; }

        public override GetTokenForEmailTypeIdParameter ToParameter()
        {
            return new GetTokenForEmailTypeIdParameter()
            {
                UserId = UserId,
                EmailTemplateTypeId = EmailTemplateTypeId
            };
        }
    }
}
