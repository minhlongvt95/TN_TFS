using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuoteCostDetailModel : BaseModel<QuoteCostDetail>
    {
        public Guid QuoteCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid QuoteId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public string CostCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public QuoteCostDetailModel() { }

        public QuoteCostDetailModel(QuoteCostDetail entity) : base(entity)
        {
            // Mapper(entity, this);
        }

        public QuoteCostDetailModel(QuoteCostDetailEntityModel model)
        {
            Mapper(model, this);
        }

        public override QuoteCostDetail ToEntity()
        {
            var entity = new QuoteCostDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
