using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorOrderDetailModel : BaseModel<VendorOrderDetail>
    {
        public Guid VendorOrderDetailId { get; set; }
        public Guid VendorId { get; set; }
        public Guid VendorOrderId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid? UnitId { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public string IncurredUnit { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public string ProcurementCode { get; set; }
        public decimal? Cost { get; set; }
        public decimal? PriceWarehouse { get; set; }
        public decimal? PriceValueWarehouse { get; set; }
        public bool? IsEditCost { get; set; }
        public Guid? ProcurementRequestItemId { get; set; }

        public string NameVendor { get; set; }
        public string NameMoneyUnit { get; set; }
        public string NameGene { get; set; }
        public string NameProductUnit { get; set; }
        public string NameProduct { get; set; }
        public decimal SumAmount { get; set; }
        public int? OrderNumber { get; set; }
        public List<VendorOrderProductDetailProductAttributeValueModel> VendorOrderProductDetailProductAttributeValue { get; set; }

        public VendorOrderDetailModel() { }
        public VendorOrderDetailModel(VendorOrderDetail entity) : base(entity) {}
        public VendorOrderDetailModel(VendorOrderDetailEntityModel model){
            Mapper(model, this);
        }
        public override VendorOrderDetail ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new VendorOrderDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
