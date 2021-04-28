using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetEmployeeManagerResponse : BaseResponse
    {
        public List<EmployeeModel> ManagerList { get; set; }
    }
}
