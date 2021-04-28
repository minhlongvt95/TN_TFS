using System;
using TN.TNM.DataAccess.Models.Document;

namespace TN.TNM.BusinessLogic.Models.Document
{
    public class DocumentModel : BaseModel<DataAccess.Databases.Entities.Document>
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

        public DocumentModel(DocumentEntityModel entity)
        {
            Mapper(entity, this);
        }
        public DocumentModel()
        {

        }
        public override DataAccess.Databases.Entities.Document ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Document();
            Mapper(this, entity);
            return entity;
        }
    }
}
