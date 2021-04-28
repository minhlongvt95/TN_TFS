using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models
{
    public class CategoryTypeEntityModel
    {
        public Guid CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public string CategoryTypeCode { get; set; }
        public List<CategoryEntityModel> CategoryList { get; set; }
        public bool? Active { get; set; }
    }
}
