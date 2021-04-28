using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetEmployeeSaleParameter : BaseParameter
    {
        public List<EmployeeEntityModel> ListEmployeeByAccount { get; set; } //List người bán hàng
        public Guid EmployeeId { get; set; }
    }
}
