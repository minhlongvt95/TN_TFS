using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Models.SaleBidding
{
    public class SaleBiddingDetailProductAttributeModel : BaseModel<SaleBiddingDetailProductAttribute>
    {
        public Guid SaleBiddingDetailProductAttributeId { get; set; }
        public Guid? SaleBiddingDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }

        public SaleBiddingDetailProductAttributeModel(SaleBiddingModel entity)
        {
            Mapper(entity, this);
        }

        public SaleBiddingDetailProductAttributeModel(SaleBiddingDetailProductAttributeEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.SaleBiddingDetailProductAttribute ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.SaleBiddingDetailProductAttribute();
            Mapper(this, entity);
            return entity;
        }

        public SaleBiddingDetailProductAttributeEntityModel ToEntityModel()
        {
            var entity = new SaleBiddingDetailProductAttributeEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
