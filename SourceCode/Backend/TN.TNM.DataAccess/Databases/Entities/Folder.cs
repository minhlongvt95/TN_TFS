using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Folder
    {
        public Guid FolderId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsDelete { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string FolderType { get; set; }
        public int FolderLevel { get; set; }
    }
}
