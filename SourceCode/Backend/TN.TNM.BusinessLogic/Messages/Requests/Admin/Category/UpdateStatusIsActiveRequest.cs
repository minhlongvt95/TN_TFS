using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Category
{
    public class UpdateStatusIsActiveRequest : BaseRequest<UpdateStatusIsActiveParameter>
    {
        public Guid CategoryId { get; set; }
        public bool? Active { get; set; }
        public override UpdateStatusIsActiveParameter ToParameter()
        {
            return new UpdateStatusIsActiveParameter()
            {
                CategoryId = CategoryId,
                Active = Active,
                UserId = UserId
            };
        }
    }
}
