using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models
{
    public class PermissionEntityModel
    {
        public Guid? PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public Guid? ParentId { get; set; }
        public string PermissionDescription { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public bool? Active { get; set; }
        public List<PermissionEntityModel> PermissionChildList { get; set; }
        public int NumberOfUserHasPermission { get; set; }
        public byte? Sort { get; set; }
    }
}
