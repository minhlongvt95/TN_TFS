using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Folder
{
    public class FolderEntityModel
    {
        public Guid FolderId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsDelete { get; set; }
        public bool Active { get; set; }
        public string FolderType { get; set; }
        public bool HasChild { get; set; }
        public int FolderLevel { get; set; }
        public List<FileInFolderEntityModel> ListFile { get; set; }
    }
}
