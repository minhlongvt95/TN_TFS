using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetDetailPermissionRequest : BaseRequest<GetDetailPermissionParameter>
    {
        public Guid RoleId { get; set; }

        public override GetDetailPermissionParameter ToParameter()
        {
            return new GetDetailPermissionParameter
            {
                RoleId = RoleId,
                UserId = UserId
            };
        }
    }
}
