using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class DeleteProductCategoryRequest:BaseRequest<DeleteProductCategoryParameter>
    {
        public Guid ProductCategoryId { get; set; }
        public override DeleteProductCategoryParameter ToParameter() => new DeleteProductCategoryParameter()
        {
            ProductCategoryId = ProductCategoryId
        };
    }
}
