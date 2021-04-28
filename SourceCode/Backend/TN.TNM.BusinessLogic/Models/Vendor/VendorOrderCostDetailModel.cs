using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorOrderCostDetailModel : BaseModel<VendorOrderCostDetailEntityModel>
    {
        public Guid VendorOrderCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid VendorOrderId { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CostCode { get; set; }

        public VendorOrderCostDetailModel() { }

        public VendorOrderCostDetailModel(VendorOrderCostDetailEntityModel model)
        {
            Mapper(model, this);
        }

        public override VendorOrderCostDetailEntityModel ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new VendorOrderCostDetailEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
