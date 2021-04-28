using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetPermissionByCodeRequest : BaseRequest<GetPermissionByCodeParameter>
    {
        public List<string> PerCode { get; set; }
        public override GetPermissionByCodeParameter ToParameter()
        {
            return new GetPermissionByCodeParameter()
            {
                PerCode = PerCode,
                UserId = UserId
            };
        }
    }
}
