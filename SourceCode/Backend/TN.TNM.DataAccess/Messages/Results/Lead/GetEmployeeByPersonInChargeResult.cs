using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetEmployeeByPersonInChargeResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
