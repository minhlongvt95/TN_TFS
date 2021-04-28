using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class GetAllCategoryRequest : BaseRequest<GetAllCategoryParameter>
    {
        public override GetAllCategoryParameter ToParameter()
        {
            return new GetAllCategoryParameter(){
            };
        }
    }
}
