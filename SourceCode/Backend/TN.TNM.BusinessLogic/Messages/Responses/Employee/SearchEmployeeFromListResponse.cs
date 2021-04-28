using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class SearchEmployeeFromListResponse : BaseResponse
    {
        public List<EmployeeModel> EmployeeList { get; set; }
        public Guid CurrentOrganizationId { get; set; }
    }
}
