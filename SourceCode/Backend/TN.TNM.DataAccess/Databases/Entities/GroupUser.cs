using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class GroupUser
    {
        public Guid GroupUserId { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }

        public Groups Group { get; set; }
        public User User { get; set; }
    }
}
