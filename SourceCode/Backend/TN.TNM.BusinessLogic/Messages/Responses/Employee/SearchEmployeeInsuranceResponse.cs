using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class SearchEmployeeInsuranceResponse : BaseResponse
    {
        public List<EmployeeInsuranceModel> ListEmployeeInsurance { get; set; }
    }
}
