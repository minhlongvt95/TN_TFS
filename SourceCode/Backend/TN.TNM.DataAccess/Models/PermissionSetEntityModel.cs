using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models
{
    public class PermissionSetEntityModel
    {
        public Guid PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionSetCode { get; set; }
        public string PermissionSetDescription { get; set; }
        public string PermissionId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public int NumberOfUserHasPermission { get; set; }
        public List<PermissionEntityModel> PermissionList { get; set; }
    }
}
