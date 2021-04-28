using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class UpdateStatusIsDefaultRequest : BaseRequest<UpdateStatusIsDefaultParameter>
    {
        public Guid CategoryId { get; set; }
        public Guid CategoryTypeId { get; set; }
        public override UpdateStatusIsDefaultParameter ToParameter()
        {
            return new UpdateStatusIsDefaultParameter()
            {
                CategoryId = CategoryId,
                CategoryTypeId = CategoryTypeId,
                UserId = UserId
            };
        }
    }
}
