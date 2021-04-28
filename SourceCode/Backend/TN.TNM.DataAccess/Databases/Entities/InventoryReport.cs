using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InventoryReport
    {
        public Guid InventoryReportId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityMinimum { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? StartQuantity { get; set; }
        public decimal? QuantityMaximum { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string Note { get; set; }
    }
}
