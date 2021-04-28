using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CategoryType
    {
        public CategoryType()
        {
            Category = new HashSet<Category>();
        }

        public Guid CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public string CategoryTypeCode { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<Category> Category { get; set; }
    }
}
