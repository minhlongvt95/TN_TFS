using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ProductBillOfMaterialsEntityModel
    {
        public Guid ProductBillOfMaterialId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductMaterialId { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        //Thông tin hiển thị bổ sung
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }
    }
}
