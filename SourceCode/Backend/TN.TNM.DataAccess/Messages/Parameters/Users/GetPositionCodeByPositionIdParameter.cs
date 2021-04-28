using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Users
{
    public class GetPositionCodeByPositionIdParameter : BaseParameter
    {
        public Guid PositionId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
