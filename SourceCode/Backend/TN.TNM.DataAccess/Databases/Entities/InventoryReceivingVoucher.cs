using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InventoryReceivingVoucher
    {
        public Guid InventoryReceivingVoucherId { get; set; }
        public string InventoryReceivingVoucherCode { get; set; }
        public Guid StatusId { get; set; }
        public int InventoryReceivingVoucherType { get; set; }
        public Guid WarehouseId { get; set; }
        public string ShiperName { get; set; }
        public Guid? Storekeeper { get; set; }
        public DateTime InventoryReceivingVoucherDate { get; set; }
        public TimeSpan InventoryReceivingVoucherTime { get; set; }
        public int LicenseNumber { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? PartnersId { get; set; }
    }
}
