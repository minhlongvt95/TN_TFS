using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class UserRole
    {
        public Guid UserRoleId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
