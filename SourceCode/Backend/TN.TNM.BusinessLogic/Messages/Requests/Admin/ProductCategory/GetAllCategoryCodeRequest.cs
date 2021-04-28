using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class GetAllCategoryCodeRequest : BaseRequest<GetAllCategoryCodeParameter>
    {
        public override GetAllCategoryCodeParameter ToParameter() => new GetAllCategoryCodeParameter() { };
    }
}
