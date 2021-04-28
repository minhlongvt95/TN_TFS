using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;


namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuoteProductDetailProductAttributeValueModel : BaseModel<QuoteProductDetailProductAttributeValue>
    {
        public Guid QuoteDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid QuoteProductDetailProductAttributeValueId { get; set; }

        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }

        public QuoteProductDetailProductAttributeValueModel() { }

        public QuoteProductDetailProductAttributeValueModel(QuoteProductDetailProductAttributeValue entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public QuoteProductDetailProductAttributeValueModel(QuoteProductDetailProductAttributeValueEntityModel model)
        {
            Mapper(model, this);
        }

        public override QuoteProductDetailProductAttributeValue ToEntity()
        {
            var entity = new QuoteProductDetailProductAttributeValue();
            Mapper(this, entity);
            return entity;
        }
    }
}
