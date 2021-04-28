using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Permission
    {
        public Guid PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public Guid? ParentId { get; set; }
        public string PermissionDescription { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public bool Active { get; set; }
        public byte? Sort { get; set; }
        public Guid? TenantId { get; set; }
    }
}
