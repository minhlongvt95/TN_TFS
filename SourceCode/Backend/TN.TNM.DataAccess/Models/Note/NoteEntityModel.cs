using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Models.Note
{
    public class NoteEntityModel
    {
        public Guid NoteId { get; set; }
        public string NoteTitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ResponsibleName { get; set; }
        public string ResponsibleAvatar { get; set; }
        public List<NoteDocumentEntityModel> NoteDocList { get; set; }
        public List<FileInFolderEntityModel> ListFile { get; set; }
    }
}
