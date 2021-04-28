using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InventoryDeliveryVoucher
    {
        public Guid InventoryDeliveryVoucherId { get; set; }
        public string InventoryDeliveryVoucherCode { get; set; }
        public Guid StatusId { get; set; }
        public int InventoryDeliveryVoucherType { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid ObjectId { get; set; }
        public string Receiver { get; set; }
        public string Reason { get; set; }
        public DateTime? InventoryDeliveryVoucherDate { get; set; }
        public TimeSpan? InventoryDeliveryVoucherTime { get; set; }
        public int LicenseNumber { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? PartnersId { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}
