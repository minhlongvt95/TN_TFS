using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InventoryDetail
    {
        public Guid InventoryDetailId { get; set; }
        public Guid InventoryId { get; set; }
        public DateTime InventoryDetailInputDate { get; set; }
        public DateTime InventoryDetailOutputDate { get; set; }
        public decimal? InventoryDetailProductPrice { get; set; }
        public Guid InventoryDetailProductId { get; set; }
        public decimal InventoryDetailProductQuantity { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }

        public Inventory Inventory { get; set; }
        public Product InventoryDetailProduct { get; set; }
    }
}
