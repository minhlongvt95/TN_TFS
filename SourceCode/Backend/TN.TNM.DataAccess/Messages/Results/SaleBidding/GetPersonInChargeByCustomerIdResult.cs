using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetPersonInChargeByCustomerIdResult:BaseResult
    {
        public List<EmployeeEntityModel> ListPersonInCharge { get; set; }
    }
}
