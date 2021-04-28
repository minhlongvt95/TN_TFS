using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetStatisticForEmpDashBoardRequest : BaseRequest<GetStatisticForEmpDashBoardParameter>
    {
        public string KeyName { get; set; }
        public DateTime FirstOfWeek { get; set; }
        public DateTime LastOfWeek { get; set; }
        public override GetStatisticForEmpDashBoardParameter ToParameter()
        {
            return new GetStatisticForEmpDashBoardParameter()
            {
                UserId = UserId,
                KeyName = KeyName,
                FirstOfWeek = FirstOfWeek,
                LastOfWeek = LastOfWeek
            };
        }
    }
}
