using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class FindEmployeeMonthySalaryParameter:BaseParameter
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeUnit { get; set;}
        public string EmployeeBranch { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public List<Guid?> lstEmployeeUnitId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
