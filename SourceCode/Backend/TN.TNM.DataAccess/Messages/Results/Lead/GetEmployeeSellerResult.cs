using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetEmployeeSellerResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; } //List người bán hàng
    }
}
