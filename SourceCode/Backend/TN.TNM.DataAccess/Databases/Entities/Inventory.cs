using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Inventory
    {
        public Inventory()
        {
            InventoryDetailNavigation = new HashSet<InventoryDetail>();
        }

        public Guid InventoryId { get; set; }
        public Guid VendorId { get; set; }
        public string ProductService { get; set; }
        public string InventoryDetail { get; set; }
        public int? InventoryQuantity { get; set; }
        public decimal? InventoryTotalAmount { get; set; }
        public Guid? InventoryCurrency { get; set; }
        public Guid InventoryStatus { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }

        public Category InventoryStatusNavigation { get; set; }
        public Vendor Vendor { get; set; }
        public ICollection<InventoryDetail> InventoryDetailNavigation { get; set; }
    }
}
