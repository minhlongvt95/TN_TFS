using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeCareStaffResult : BaseResult
    {
        public List<dynamic> employeeList { get; set; }
    }
}
