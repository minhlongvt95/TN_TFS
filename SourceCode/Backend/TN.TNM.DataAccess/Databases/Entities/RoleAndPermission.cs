using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class RoleAndPermission
    {
        public Guid RoleAndPermissionId { get; set; }
        public Guid? ActionResourceId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
