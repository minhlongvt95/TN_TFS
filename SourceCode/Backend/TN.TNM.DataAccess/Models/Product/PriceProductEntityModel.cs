using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class PriceProductEntityModel
    {
        public Guid PriceProductId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal PriceVnd { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? PriceForeignMoney { get; set; }
        public Guid? CustomerGroupCategory { get; set; }
        public string CustomerGroupCategoryName { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
