using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class InventoryReportByProductIdEntityModel
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
        public decimal? StartQuantity { get; set; }
        public decimal? QuantityMaximum { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string Note { get; set; }
        public List<SerialEntityModel> ListSerial { get; set; }
    }
}
