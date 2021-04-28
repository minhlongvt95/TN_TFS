using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetEmployeeSellerParameter : BaseParameter
    {
        public List<EmployeeEntityModel> ListEmployeeByAccount { get; set; } //List người bán hàng
        public Guid EmployeeId { get; set; }
    }
}
