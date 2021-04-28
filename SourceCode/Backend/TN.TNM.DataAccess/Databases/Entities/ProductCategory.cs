using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            InverseParent = new HashSet<ProductCategory>();
            ProcurementPlan = new HashSet<ProcurementPlan>();
            Product = new HashSet<Product>();
        }

        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public int? ProductCategoryLevel { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }

        public ProductCategory Parent { get; set; }
        public ICollection<ProductCategory> InverseParent { get; set; }
        public ICollection<ProcurementPlan> ProcurementPlan { get; set; }
        public ICollection<Product> Product { get; set; }
    }
}
