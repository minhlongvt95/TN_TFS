using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Promotion
{
    public class PromotionProductMappingApplyModel
    {
        public Guid? PromotionMappingId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }

        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }
    }
}
