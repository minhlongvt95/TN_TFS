using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class CreateCategoryRequest : BaseRequest<CreateCategoryParameter>
    {
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public Guid CategoryTypeId { get; set; }
        public override CreateCategoryParameter ToParameter()
        {
            return new CreateCategoryParameter()
            {
                CategoryTypeId = CategoryTypeId,
                CategoryName = CategoryName,
                CategoryCode = CategoryCode,
                UserId = UserId
            };
        }
    }
}
