using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class DeleteRoleRequest : BaseRequest<DeleteRoleParameter>
    {
        public Guid RoleId { get; set; }
        public override DeleteRoleParameter ToParameter()
        {
            return new DeleteRoleParameter()
            {
                UserId = UserId,
                RoleId = RoleId,
            };
        }
    }
}
