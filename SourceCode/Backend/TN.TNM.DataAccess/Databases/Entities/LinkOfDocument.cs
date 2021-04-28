using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LinkOfDocument
    {
        public Guid LinkOfDocumentId { get; set; }
        public string LinkName { get; set; }
        public string LinkValue { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
