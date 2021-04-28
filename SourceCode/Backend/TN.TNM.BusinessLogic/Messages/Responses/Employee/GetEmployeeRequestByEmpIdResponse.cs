using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeRequestByEmpIdResponse : BaseResponse
    {
        public List<EmployeeRequestModel> ListEmployeeRequest { get; set; }
        public double amountAbsentWithPermission { get; set; }
        public double amountAbsentWithoutPermission { get; set; }
    }
}
