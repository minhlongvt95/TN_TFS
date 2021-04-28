using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class FileInFolder
    {
        public Guid FileInFolderId { get; set; }
        public Guid FolderId { get; set; }
        public string FileName { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Size { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string FileExtension { get; set; }
    }
}
