using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailTeacherPayslipParameter:BaseParameter
    {
        public List<Guid> lstEmpMonthySalary { get; set; }
    }
}
