using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class SetPersonalInChangeResult: BaseResult
    {
        public List<Models.Employee.EmployeeEntityModel> ListPersonalInChange { get; set; }
    }
}
