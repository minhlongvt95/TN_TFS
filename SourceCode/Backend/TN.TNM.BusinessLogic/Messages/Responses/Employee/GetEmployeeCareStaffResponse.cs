using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeCareStaffResponse : BaseResponse
    {
        public List<dynamic> employeeList { get; set; }
    }
}
