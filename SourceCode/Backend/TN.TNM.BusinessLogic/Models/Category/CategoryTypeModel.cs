using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Category
{
    public class CategoryTypeModel : BaseModel<CategoryType>
    {
        public Guid CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public string CategoryTypeCode { get; set; }
        public List<CategoryModel> CategoryList { get; set; }
        public bool? Active { get; set; }

        public CategoryTypeModel(CategoryType entity) : base(entity)
        {

        }

        public CategoryTypeModel(CategoryTypeEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override CategoryType ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new CategoryType();
            Mapper(this, entity);
            return entity;
        }
    }
}
