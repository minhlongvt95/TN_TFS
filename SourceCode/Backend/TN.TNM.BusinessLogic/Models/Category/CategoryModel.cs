using System;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Category
{
    public class CategoryModel : BaseModel<DataAccess.Databases.Entities.Category>
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public Guid CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsDefauld { get; set; }
        public string CategoryTypeCode { get; set; }
        public int CountCategoryById { get; set; }

        public CategoryModel() { }

        public CategoryModel(CategoryEntityModel entity)
        {
            Mapper(entity, this);
        }

        public CategoryModel(DataAccess.Databases.Entities.Category entity) : base(entity)
        {

        }

        public override DataAccess.Databases.Entities.Category ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Category();
            Mapper(this, entity);
            return entity;
        }
    }
}
