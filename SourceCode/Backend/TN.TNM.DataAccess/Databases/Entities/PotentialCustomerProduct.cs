using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PotentialCustomerProduct
    {
        public Guid PotentialCustomerProductId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool? IsInTheSystem { get; set; }
        public string ProductName { get; set; }
        public string ProductUnit { get; set; }
        public decimal? ProductFixedPrice { get; set; }
        public decimal? ProductUnitPrice { get; set; }
        public string ProductNote { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
