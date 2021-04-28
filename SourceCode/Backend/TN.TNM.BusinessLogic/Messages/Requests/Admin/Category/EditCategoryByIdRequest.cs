using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class EditCategoryByIdRequest : BaseRequest<EditCategoryByIdParameter>
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public override EditCategoryByIdParameter ToParameter()
        {
            return new EditCategoryByIdParameter()
            {
                CategoryId = CategoryId,
                CategoryName = CategoryName,
                CategoryCode = CategoryCode,
                UserId = UserId
            };
        }
    }
}
