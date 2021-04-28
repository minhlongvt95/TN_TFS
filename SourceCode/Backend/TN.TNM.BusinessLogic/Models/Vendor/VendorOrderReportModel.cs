using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorOrderReportModel : BaseModel<VendorOrderReportEntityModel>
    {
        public int Stt { get; set; }
        public Guid VendorOrderId { get; set; }
        public string VendorOrderCode { get; set; }
        public DateTime? VendorOrderDate { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }

        public VendorOrderReportModel() { }

        public VendorOrderReportModel(VendorOrderReportEntityModel model)
        {
            Mapper(model, this);
        }

        public override VendorOrderReportEntityModel ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new VendorOrderReportEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
