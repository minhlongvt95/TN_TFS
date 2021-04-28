using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class SearchEmployeeRequestResult : BaseResult
    {
        public List<EmployeeRequestEntityModel> EmployeeRequestList { get; set; }
        public int AmountAbsentWithPermission { get; set; }
        public int AmountAbsentWithoutPermission { get; set; }
    }
}
