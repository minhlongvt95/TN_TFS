using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models
{
    public class PermissionMappingEntityModel
    {
        public Guid PermissionMappingId { get; set; }
        public string PermissionMappingName { get; set; }
        public string PermissionMappingCode { get; set; }
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }
        public string UseFor { get; set; }
        public Guid PermissionId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public List<Guid> PermissionIdList { get; set; }
    }
}
