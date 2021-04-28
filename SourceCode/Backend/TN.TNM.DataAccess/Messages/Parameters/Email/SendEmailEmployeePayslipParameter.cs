using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailEmployeePayslipParameter:BaseParameter
    {
        public List<Guid> lstEmpMonthySalary { get; set; }
    }
}
