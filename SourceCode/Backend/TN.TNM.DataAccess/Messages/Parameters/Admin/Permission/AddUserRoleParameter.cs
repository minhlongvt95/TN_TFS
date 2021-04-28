using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class AddUserRoleParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public Guid RoleId { get; set; }
    }
}
