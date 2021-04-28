using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class ExportEmployeeSalaryParameter:BaseParameter
    {
        public List<Guid> lstEmpMonthySalary { get; set; }
    }
}
