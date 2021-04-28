using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class InventoryDeliveryVoucherEntityModel
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

        public string NameObject { get; set; }
        public string NameOutOfStock { get; set; }
        public string NameCreate { get; set; }
        public string NameStatus { get; set; }
        public Guid? VendorId { get; set; }
        public string VedorName { get; set; }
    }
}
