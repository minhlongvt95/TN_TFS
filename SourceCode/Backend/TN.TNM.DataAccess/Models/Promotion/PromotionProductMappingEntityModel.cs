using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Promotion
{
    public class PromotionProductMappingEntityModel
    {
        public Guid? PromotionProductMappingId { get; set; }
        public Guid? PromotionMappingId { get; set; }
        public Guid ProductId { get; set; }
    }
}
