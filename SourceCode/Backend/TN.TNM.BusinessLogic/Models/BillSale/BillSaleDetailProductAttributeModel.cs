using System;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.BusinessLogic.Models.BillSale
{
    public class BillSaleDetailProductAttributeModel : BaseModel<DataAccess.Databases.Entities.BillOfSaleCostProductAttribute>
    {
        public Guid? BillOfSaleCostProductAttributeId { get; set; }
        public Guid? OrderProductDetailProductAttributeValueId { get; set; }
        public Guid? OrderDetailId { get; set; }
        public Guid? BillOfSaleDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }

        public BillSaleDetailProductAttributeModel()
        {

        }

        public BillSaleDetailProductAttributeModel(OrderBillModel entity)
        {
            Mapper(entity, this);
        }

        public BillSaleDetailProductAttributeModel(BillSaleDetailProductAttributeEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.BillOfSaleCostProductAttribute ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.BillOfSaleCostProductAttribute();
            Mapper(this, entity);
            return entity;
        }

        public BillSaleDetailProductAttributeEntityModel ToEntityModel()
        {
            var entity = new BillSaleDetailProductAttributeEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
