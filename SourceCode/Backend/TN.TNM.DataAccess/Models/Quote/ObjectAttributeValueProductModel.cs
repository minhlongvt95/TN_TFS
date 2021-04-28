using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class ObjectAttributeValueProductModel
    {
        public Guid ProductAttributeCategoryValueId { get; set; }
        public string ProductAttributeCategoryValue { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
    }
}
