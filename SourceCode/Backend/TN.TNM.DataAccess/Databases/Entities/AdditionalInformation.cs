using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class AdditionalInformation
    {
        public Guid AdditionalInformationId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? Ordinal { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public int? OrderNumber { get; set; }
    }
}
