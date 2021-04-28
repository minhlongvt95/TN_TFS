using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class UpdateActiveProductCategoryRequest : BaseRequest<UpdateActiveProductCategoryParameter>
    {
        public Guid ProductCategoryId { get; set; }
        public override UpdateActiveProductCategoryParameter ToParameter() => new UpdateActiveProductCategoryParameter()
        {
            ProductCategoryId = ProductCategoryId
        };
    }
}
