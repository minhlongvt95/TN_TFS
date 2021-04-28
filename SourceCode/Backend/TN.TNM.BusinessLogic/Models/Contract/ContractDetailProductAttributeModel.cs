using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Models.Contract
{
    public class ContractDetailProductAttributeModel : BaseModel<ContractDetailProductAttribute>
    {
        public Guid ContractDetailProductAttributeId { get; set; }
        public Guid? ContractDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }
        public ContractDetailProductAttributeModel() { }

        public ContractDetailProductAttributeModel(DataAccess.Databases.Entities.ContractDetailProductAttribute entity) : base(entity)
        {

        }

        public ContractDetailProductAttributeModel(ContractDetailProductAttributeEntityModel model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ContractDetailProductAttribute ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.ContractDetailProductAttribute();
            Mapper(this, entity);
            return entity;
        }

        public ContractDetailProductAttributeEntityModel ToEntityModel()
        {
            var entity = new ContractDetailProductAttributeEntityModel();
            Mapper(this, entity);
            return entity;
        }

    }
}
