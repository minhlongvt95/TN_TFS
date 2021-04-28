using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class StockTake
    {
        public Guid StockTakeId { get; set; }
        public string StockTakeCode { get; set; }
        public Guid Storekeeper { get; set; }
        public DateTime StockTakeDate { get; set; }
        public TimeSpan StockTakeTime { get; set; }
        public Guid StatusId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid? PeopleBalance { get; set; }
        public DateTime? BalanceDate { get; set; }
        public decimal TotalQuantityActual { get; set; }
        public decimal TotalQuantityDeflectionIncreases { get; set; }
        public decimal TotalQuantityDeviationDecreases { get; set; }
        public decimal TotalQuantityDeviation { get; set; }
        public decimal TotalMoneyActual { get; set; }
        public decimal TotalMoneyDeflectionIncreases { get; set; }
        public decimal TotalMoneyDeviationDecreases { get; set; }
        public decimal TotalMoneyDeviationDeviation { get; set; }
        public Guid UnitPriceId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
