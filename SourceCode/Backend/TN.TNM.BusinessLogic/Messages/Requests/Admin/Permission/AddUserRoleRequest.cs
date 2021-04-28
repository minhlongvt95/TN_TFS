using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class AddUserRoleRequest : BaseRequest<AddUserRoleParameter>
    {
        public Guid EmployeeId { get; set; }
        public Guid RoleId { get; set; }
        public override AddUserRoleParameter ToParameter()
        {
            return new AddUserRoleParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId,
                RoleId = RoleId
            };
        }
    }
}
