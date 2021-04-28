using System;

namespace TN.TNM.DataAccess.Models
{
    public class CategoryEntityModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public Guid CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDefault { get; set; }
        public string CategoryTypeCode { get; set; }
        public int CountCategoryById { get; set; }
    }
}
