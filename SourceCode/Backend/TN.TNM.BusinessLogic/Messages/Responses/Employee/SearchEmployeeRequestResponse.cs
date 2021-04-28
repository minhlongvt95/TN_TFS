using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class SearchEmployeeRequestResponse : BaseResponse
    {
        public List<EmployeeRequestModel> EmployeeRequestList { get; set; }
        public int AmountAbsentWithPermission { get; set; }
        public int AmountAbsentWithoutPermission { get; set; }
    }
}
