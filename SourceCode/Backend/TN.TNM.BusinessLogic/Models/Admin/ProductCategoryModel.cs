using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ProductCategory;


namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class ProductCategoryModel : BaseModel<ProductCategory>
    {
        public Guid ProductCategoryId { get; set; }
        public String ProductCategoryName { get; set; }
        public int? ProductCategoryLevel { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public List<ProductCategoryModel> ProductCategoryChildList { get; set; }
        public int CountProductCategory { get; set; }
        public ProductCategoryModel(ProductCategoryEntityModel model)
        {
            Mapper(model, this);
            if (model.ProductCategoryChildList != null)
            {
                var cList = new List<ProductCategoryModel>();
                model.ProductCategoryChildList.ForEach(child =>
                {
                    cList.Add(new ProductCategoryModel(child));
                });
                this.ProductCategoryChildList = cList;
            }
        }
        public ProductCategoryModel(ProductCategory entity): base(entity) { }
        public override ProductCategory ToEntity()
        {
            var entity = new ProductCategory();
            Mapper(this, entity);
            return entity;
        }

    }
}
