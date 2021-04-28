using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetEmployeeByPersonInChargeResponse : BaseResponse
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
