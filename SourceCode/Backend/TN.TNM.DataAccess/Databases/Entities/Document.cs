using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Document
    {
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public Guid? ObjectId { get; set; }
        public string Extension { get; set; }
        public long? Size { get; set; }
        public string DocumentUrl { get; set; }
        public string Header { get; set; }
        public string ContentType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }
    }
}
