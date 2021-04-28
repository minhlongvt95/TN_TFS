using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetOrganizationByUserRequest: BaseRequest<GetOrganizationByUserParameter>
    {
        public override GetOrganizationByUserParameter ToParameter()
        {
            return new GetOrganizationByUserParameter()
            {
                UserId = UserId,
            };
        }
    }
}
