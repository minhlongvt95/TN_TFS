using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Models.ProcurementRequest
{
    public class ProcurementRequestItemModel : BaseModel<ProcurementRequestItem>
    {
        public Guid ProcurementRequestItemId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal? Quantity { get; set; }
        public Guid Unit { get; set; }
        public decimal? UnitPrice { get; set; }
        public string UnitName { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public Guid? ProcurementPlanId { get; set; }
        public string ProcurementPlanCode { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public Guid? ProductUnitId { get; set; }
        public string ProductUnit { get; set; }
        public decimal? QuantityApproval { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? Vat { get; set; }
        public string IncurredUnit { get; set; }
        public int? OrderNumber { get; set; }

        public ProcurementRequestItemModel() { }
        public ProcurementRequestItemModel(ProcurementRequestItem entity) : base(entity) { }
        public ProcurementRequestItemModel(ProcurementRequestItemEntityModel model) {
            Mapper(model, this);
        }
        public override ProcurementRequestItem ToEntity()
        {
            var entity = new ProcurementRequestItem();
            Mapper(this, entity);
            return entity;
        }
    }
}
