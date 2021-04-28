using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class GetCategoryByIdRequest: BaseRequest<GetCategoryByIdParameter>
    {
        public Guid CategoryId { get; set; }

        public override GetCategoryByIdParameter ToParameter()
        {
            return new GetCategoryByIdParameter()
            {
                CategoryId = CategoryId,
                UserId = UserId
            };
        }
    }
}
