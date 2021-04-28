using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.SaleBidding
{
    public class CostQuoteModel
    {
        public Guid CostsQuoteId { get; set; }
        public Guid SaleBiddingId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string NameMoneyUnit { get; set; }
        public string ProductNameUnit { get; set; }
        public string NameVendor { get; set; }
        public short? OrderDetailType { get; set; }
        public Guid? UnitId { get; set; }
        public string IncurredUnit { get; set; }
        public short? CostsQuoteType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public decimal? SumAmount { get; set; }
        public List<SaleBiddingDetailProductAttributeEntityModel> SaleBiddingDetailProductAttribute { get; set; }
    }
}
