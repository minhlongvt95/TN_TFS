using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class DeleteCategoryByIdRequest : BaseRequest<DeleteCategoryByIdParameter>
    {
        public Guid CategoryId { get; set; }
        public override DeleteCategoryByIdParameter ToParameter()
        {
            return new DeleteCategoryByIdParameter()
            {
                CategoryId = CategoryId,
                UserId = UserId
            };
        }
    }
}
