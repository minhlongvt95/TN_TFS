using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class GetAllProductCategoryRequest : BaseRequest<GetAllProductCategoryParameter>
    {
        public override GetAllProductCategoryParameter ToParameter() => new GetAllProductCategoryParameter() { };
    }
}
