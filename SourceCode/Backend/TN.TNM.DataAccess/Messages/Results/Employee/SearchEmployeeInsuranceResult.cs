using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class SearchEmployeeInsuranceResult : BaseResult
    {
        public List<EmployeeInsurance> ListEmployeeInsurance { get; set; }
    }
}
