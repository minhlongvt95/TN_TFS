using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllEmployeeRequestResponse : BaseResponse
    {
        public List<EmployeeRequestModel> EmployeeRequestList { get; set; }
        public List<OrganizationDetail> OrganizationList { get; set; }
    }
}
