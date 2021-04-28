using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetEmployeeSellerResponse : BaseResponse
    {
        public List<EmployeeModel> ListEmployee { get; set; }
    }
}
