using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PermissionMapping
    {
        public Guid PermissionMappingId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }
        public string UseFor { get; set; }
        public Guid PermissionSetId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }

        public Groups Group { get; set; }
        public PermissionSet PermissionSet { get; set; }
        public User User { get; set; }
    }
}
