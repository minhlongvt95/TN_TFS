using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class StockTakeProductMapping
    {
        public Guid StockTakeProductMappingId { get; set; }
        public Guid StockTakeId { get; set; }
        public Guid ProductId { get; set; }
        public decimal QuantityInventory { get; set; }
        public decimal QuantityActual { get; set; }
        public decimal QuantityDeviation { get; set; }
        public decimal PriceDeviation { get; set; }
        public Guid UnitPriceId { get; set; }
        public string Description { get; set; }
        public Guid StatusId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
