using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Document
{
    public class LinkOfDocumentEntityModel
    {
        public Guid LinkOfDocumentId { get; set; }
        public string LinkName { get; set; }
        public string LinkValue { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public bool IsNewLink { get; set; }// phân biệt link mới hoặc link cũ
    }
}
