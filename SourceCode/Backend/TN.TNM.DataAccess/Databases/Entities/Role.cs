using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Role
    {
        public Guid RoleId { get; set; }
        public string RoleValue { get; set; }
        public string Description { get; set; }
        public Guid? TenantId { get; set; }
    }
}
