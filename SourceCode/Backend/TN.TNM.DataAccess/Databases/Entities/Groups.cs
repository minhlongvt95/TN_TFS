using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Groups
    {
        public Groups()
        {
            GroupUser = new HashSet<GroupUser>();
            PermissionMapping = new HashSet<PermissionMapping>();
        }

        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<GroupUser> GroupUser { get; set; }
        public ICollection<PermissionMapping> PermissionMapping { get; set; }
    }
}
