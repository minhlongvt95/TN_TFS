using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class GetAllCategoryByCategoryTypeCodeRequest : BaseRequest<GetAllCategoryByCategoryTypeCodeParameter>
    {
        public string CategoryTypeCode { get; set; }

        public override GetAllCategoryByCategoryTypeCodeParameter ToParameter() => new GetAllCategoryByCategoryTypeCodeParameter
        {
            CategoryTypeCode = CategoryTypeCode,
            UserId = UserId
        };
    }
}
