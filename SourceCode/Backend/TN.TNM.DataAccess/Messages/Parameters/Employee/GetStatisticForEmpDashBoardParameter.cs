using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetStatisticForEmpDashBoardParameter : BaseParameter
    {
        public string KeyName { get; set; }
        public DateTime FirstOfWeek { get; set; }
        public DateTime LastOfWeek { get; set; }
    }
}
