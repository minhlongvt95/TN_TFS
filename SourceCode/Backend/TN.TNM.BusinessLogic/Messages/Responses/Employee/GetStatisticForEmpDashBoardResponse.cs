using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetStatisticForEmpDashBoardResponse : BaseResponse
    {
        public bool IsManager { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<EmployeeRequestModel> ListRequestInsideWeek { get; set; }
        public List<EmployeeModel> ListEmpNearestBirthday { get; set; }
        public List<EmployeeModel> ListEmpEndContract { get; set; }
    }
}
