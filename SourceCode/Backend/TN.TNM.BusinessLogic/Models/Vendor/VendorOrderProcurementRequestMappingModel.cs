using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorOrderProcurementRequestMappingModel : BaseModel<VendorOrderProcurementRequestMappingEntityModel>
    {
        public Guid VendorOrderProcurementRequestMappingId { get; set; }
        public Guid? VendorOrderId { get; set; }
        public Guid? ProcurementRequestId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public VendorOrderProcurementRequestMappingModel() { }

        public VendorOrderProcurementRequestMappingModel(VendorOrderProcurementRequestMappingEntityModel model)
        {
            Mapper(model, this);
        }

        public override VendorOrderProcurementRequestMappingEntityModel ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new VendorOrderProcurementRequestMappingEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
