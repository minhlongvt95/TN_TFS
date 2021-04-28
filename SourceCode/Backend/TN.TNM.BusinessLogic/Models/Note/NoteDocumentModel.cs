using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Models.Note
{
    public class NoteDocumentModel : BaseModel<NoteDocument>
    {
        public Guid NoteDocumentId { get; set; }
        public Guid NoteId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
        public string DocumentUrl { get; set; }
        public string Base64Url { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public NoteDocumentModel() { }

        public NoteDocumentModel(NoteDocumentEntityModel model)
        {
            Mapper(model, this);
        }

        public NoteDocumentModel(NoteDocument entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override NoteDocument ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new NoteDocument();
            Mapper(this, entity);
            return entity;
        }
    }
}
