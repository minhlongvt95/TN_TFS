using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class ExportTeacherSalaryParameter:BaseParameter
    {
        public List<Guid> lstEmpMonthySalary { get; set; }
    }
}
