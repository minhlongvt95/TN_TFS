using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class EditProductCategoryRequest : BaseRequest<EditProductCategoryParameter>
    {
        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        //public bool Active { get; set; }
        //public Guid UpdatedById { get; set; }

        public override EditProductCategoryParameter ToParameter() => new EditProductCategoryParameter()
        {
            ProductCategoryId = ProductCategoryId,
            ProductCategoryName = ProductCategoryName,
            ProductCategoryCode = ProductCategoryCode,
            Description = Description,
            UserId=this.UserId
            //Active = Active,
            //UpdatedById = UpdatedById
        };
    }
}
