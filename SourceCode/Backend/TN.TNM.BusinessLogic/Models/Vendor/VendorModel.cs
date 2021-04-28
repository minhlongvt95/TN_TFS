using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorModel : BaseModel<DataAccess.Databases.Entities.Vendor>
    {
        public Guid? VendorId { get; set; }
        public Guid? ContactId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public Guid VendorGroupId { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentName { get; set; }
        public decimal? TotalPurchaseValue { get; set; }
        public decimal? TotalPayableValue { get; set; }
        public DateTime? NearestDateTransaction { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public string VendorGroupName { get; set; }
        public int CountVendorInformation { get; set; }
        public bool CanDelete { get; set; }
        public bool HasProduct { get; set; }
        public List<Guid> ListProductId { get; set; }
        public string VendorCodeName { get; set; }

        public VendorModel() { }

        public VendorModel(VendorEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public VendorModel(DataAccess.Databases.Entities.Vendor entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.Vendor ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Vendor();
            Mapper(this, entity);
            return entity;
        }
    }
}
