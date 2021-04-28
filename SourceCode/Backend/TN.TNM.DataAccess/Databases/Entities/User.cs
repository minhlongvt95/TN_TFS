using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class User
    {
        public User()
        {
            GroupUser = new HashSet<GroupUser>();
            PermissionMapping = new HashSet<PermissionMapping>();
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool Disabled { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public DateTime? ResetCodeDate { get; set; }
        public string ResetCode { get; set; }
        public bool? IsAdmin { get; set; }
        public Guid? TenantId { get; set; }

        public Employee Employee { get; set; }
        public ICollection<GroupUser> GroupUser { get; set; }
        public ICollection<PermissionMapping> PermissionMapping { get; set; }
    }
}
