using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuoteDetailModel : BaseModel<QuoteDetail>
    {
        public Guid QuoteDetailId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid QuoteId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public Guid? UnitId { get; set; }
        public string IncurredUnit { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal? PriceInitial { get; set; }
        public bool? IsPriceInitial { get; set; }
        public string ProductName { get; set; }
        public int? OrderNumber { get; set; }

        public string NameVendor { get; set; }
        public string NameMoneyUnit { get; set; }
        public string NameGene { get; set; }
        public string NameProductUnit { get; set; }
        public string ProductNameUnit { get; set; }
        public string NameProduct { get; set; }
        public string ProductCode { get; set; }
        public decimal SumAmount { get; set; }

        public List<QuoteProductDetailProductAttributeValueModel> QuoteProductDetailProductAttributeValue { get; set; }

        public QuoteDetailModel() { }

        public QuoteDetailModel(QuoteDetail entity) : base(entity)
        {
            // Mapper(entity, this);
        }
        public QuoteDetailModel(QuoteDetailEntityModel model)
        {
            Mapper(model, this);
        }

        public override QuoteDetail ToEntity()
        {
            var entity = new QuoteDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
