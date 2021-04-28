using System;
using TN.TNM.DataAccess.Models.ProductAttributeCategoryValue;

namespace TN.TNM.BusinessLogic.Models.ProductAttributeCategoryValue
{
    public class ProductAttributeCategoryValueModel:BaseModel<DataAccess.Databases.Entities.ProductAttributeCategoryValue>
    {
        public Guid ProductAttributeCategoryValueId { get; set; }
        public string ProductAttributeCategoryValue1 { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }

        public ProductAttributeCategoryValueModel() { }

        public ProductAttributeCategoryValueModel(ProductAttributeCategoryValueEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public ProductAttributeCategoryValueModel(DataAccess.Databases.Entities.ProductAttributeCategoryValue entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.ProductAttributeCategoryValue ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProductAttributeCategoryValue();
            Mapper(this, entity);
            return entity;
        }

    }
}
