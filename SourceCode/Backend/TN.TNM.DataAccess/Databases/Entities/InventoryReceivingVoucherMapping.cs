using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InventoryReceivingVoucherMapping
    {
        public Guid InventoryReceivingVoucherMappingId { get; set; }
        public Guid InventoryReceivingVoucherId { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid ProductId { get; set; }
        public decimal QuantityRequest { get; set; }
        public decimal QuantityActual { get; set; }
        public decimal QuantitySerial { get; set; }
        public decimal PriceProduct { get; set; }
        public Guid WarehouseId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? UnitId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal QuantityReservation { get; set; }
        public bool PriceAverage { get; set; }
        public Guid? ObjectDetailId { get; set; }
    }
}
