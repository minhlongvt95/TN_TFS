using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class CreateProductCategoryRequest:BaseRequest<CreateProductCategoryParameter>
    {
        public string ProductCategoryName { get; set; }
        public int? ProductCategoryLevel { get; set; }
        public string ProductCategoryCode { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public override CreateProductCategoryParameter ToParameter() => new CreateProductCategoryParameter()
        {
            ProductCategoryName = ProductCategoryName,
            ProducCategoryLevel = ProductCategoryLevel,
            ProductCategoryCode = ProductCategoryCode,
            Description = Description,
            ParentId = ParentId,
            UserId=this.UserId
        };
    }
}
