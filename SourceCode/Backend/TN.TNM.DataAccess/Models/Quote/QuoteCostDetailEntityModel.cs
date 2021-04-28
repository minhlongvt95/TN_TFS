using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteCostDetailEntityModel
    {
        public Guid QuoteCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid QuoteId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public string CostCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }
}
