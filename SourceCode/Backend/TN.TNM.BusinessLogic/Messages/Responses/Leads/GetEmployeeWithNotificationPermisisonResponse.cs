using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetEmployeeWithNotificationPermisisonResponse : BaseResponse
    {
        public List<EmployeeModel> EmployeeList { get; set; }
    }
}
