using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class SetPersonalInChangeResponse: BaseResponse
    {
        public List<Models.Employee.EmployeeModel> ListPersonalInChange { get; set; }
    }
}
