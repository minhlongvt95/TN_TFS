using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class DeleteRoleParameter : BaseParameter
    {
        public Guid RoleId { get; set; }
    }
}
