using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PriceProduct
    {
        public Guid PriceProductId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal PriceVnd { get; set; }
        public decimal? PriceForeignMoney { get; set; }
        public Guid? CustomerGroupCategory { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal MinQuantity { get; set; }
    }
}
