using System;

namespace TN.TNM.DataAccess.Models.Document
{
    public class DocumentEntityModel
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
        public DocumentEntityModel(Databases.Entities.Document documentEntity)
        {
            DocumentId = documentEntity.DocumentId;
            Name = documentEntity.Name;
            ObjectId = documentEntity.ObjectId;
            Extension = documentEntity.Extension;
            Size = documentEntity.Size;
            Header = documentEntity.Header;
            ContentType = documentEntity.ContentType;
            DocumentUrl = documentEntity.DocumentUrl;
            CreatedById = documentEntity.CreatedById;
            CreatedDate = documentEntity.CreatedDate;
            UpdatedById = documentEntity.UpdatedById;
            UpdatedDate = documentEntity.UpdatedDate;
            Active = documentEntity.Active;
        }
    }
}
