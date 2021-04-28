using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Vendor
{
    public class VendorOrderProductDetailProductAttributeValueModel : BaseModel<VendorOrderProductDetailProductAttributeValue>
    {
        public Guid VendorOrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }
        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }
        public VendorOrderProductDetailProductAttributeValueModel() { }
        public VendorOrderProductDetailProductAttributeValueModel(VendorOrderProductDetailProductAttributeValue entity) : base(entity) {

        }

        public VendorOrderProductDetailProductAttributeValueModel(VendorOrderProductDetailProductAttributeValueEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override VendorOrderProductDetailProductAttributeValue ToEntity()
        {
            var entity = new VendorOrderProductDetailProductAttributeValue();
            Mapper(this, entity);
            return entity;
        }
    }
}
