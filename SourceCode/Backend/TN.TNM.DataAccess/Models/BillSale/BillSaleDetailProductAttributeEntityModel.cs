using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.BillSale
{
    public class BillSaleDetailProductAttributeEntityModel
    {
        public Guid? BillOfSaleCostProductAttributeId { get; set; }
        public Guid? OrderProductDetailProductAttributeValueId { get; set; }
        public Guid? OrderDetailId { get; set; }
        public Guid? BillOfSaleDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
    }
}
