using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetStatisticForEmpDashBoardResult : BaseResult
    {
        public bool IsManager { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<EmployeeRequestEntityModel> ListRequestInsideWeek { get; set; }
        public List<EmployeeEntityModel> ListEmpNearestBirthday { get; set; }
        public List<EmployeeEntityModel> ListEmpEndContract { get; set; }

    }
}
