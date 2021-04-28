using System;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductPrice
    {
        public Guid ProductPriceId { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public Product Product { get; set; }
    }
}
