using System;
using System.Collections.Generic;
using System.Text;
using Entities=TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class InventoryReceivingVoucherModel
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

        public Guid VendorId { get; set; }
        public Guid CustomerId { get; set; }
        public string NameStorekeeper { get; set; }
        public string NameVendor { get; set; }
        public string CustomerName { get; set; }
        public string NameCreate { get; set; }
        public string NameStatus { get; set; }
        public List<Entities.VendorOrder> ListVendorOrder { get; set; }
        public List<Entities.CustomerOrder> ListCustomerOrder { get; set; }

    }
}
