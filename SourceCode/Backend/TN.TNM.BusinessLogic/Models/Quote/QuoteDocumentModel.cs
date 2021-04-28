using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuoteDocumentModel : BaseModel<QuoteDocument>
    {
        public Guid QuoteDocumentId { get; set; }
        public Guid QuoteId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
        public string DocumentUrl { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public QuoteDocumentModel() { }

        public QuoteDocumentModel(QuoteDocument entity) : base(entity)
        {
             //Mapper(entity, this);
        }
        public QuoteDocumentModel(QuoteDocumentEntityModel model)
        {
            Mapper(model, this);
        }
        
        public override QuoteDocument ToEntity()
        {
            var entity = new QuoteDocument();
            Mapper(this, entity);
            return entity;
        }
    }
}
