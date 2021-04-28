using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class GetProductCategoryByIdRequest:BaseRequest<GetProductCategoryByIdParameter>
    {
        public Guid ProductCategoryId { get; set; }
        public override GetProductCategoryByIdParameter ToParameter() => new GetProductCategoryByIdParameter()
        {
            ProductCategoryId = ProductCategoryId
        };
    }
}
