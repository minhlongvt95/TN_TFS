using System;
using TN.TNM.DataAccess.Messages.Parameters.Users;

namespace TN.TNM.BusinessLogic.Messages.Requests.Users
{
    public class GetPositionCodeByPositionIdRequest : BaseRequest<GetPositionCodeByPositionIdParameter>
    {
        public Guid PositionId { get; set; }
        public Guid EmployeeId { get; set; }
        public override GetPositionCodeByPositionIdParameter ToParameter()
        {
            return new GetPositionCodeByPositionIdParameter()
            {
                UserId = UserId,
                PositionId = PositionId,
                EmployeeId = EmployeeId
            };
        }
    }
}
