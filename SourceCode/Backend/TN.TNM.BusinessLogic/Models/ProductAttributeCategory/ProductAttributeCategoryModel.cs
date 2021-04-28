using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ProductAttributeCategoryValue;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;

namespace TN.TNM.BusinessLogic.Models.ProductAttributeCategory
{
    public class ProductAttributeCategoryModel:BaseModel<DataAccess.Databases.Entities.ProductAttributeCategory>
    {
        public Guid ProductAttributeCategoryId { get; set; }
        public string ProductAttributeCategoryName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public List<ProductAttributeCategoryValueModel> ProductAttributeCategoryValue { get; set; }
        public string ValueString { get; set; }



        public ProductAttributeCategoryModel() { }

        public ProductAttributeCategoryModel(ProductAttributeCategoryEntityModel model)
        {
            Mapper(model, this);
        }

        public ProductAttributeCategoryModel(DataAccess.Databases.Entities.ProductAttributeCategory entity) : base(entity)
        {
        }

        public override DataAccess.Databases.Entities.ProductAttributeCategory ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProductAttributeCategory();
            Mapper(this, entity);
            return entity;
        }
    }
}
