using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class ExportTeacherSalaryRequest : BaseRequest<ExportTeacherSalaryParameter>
    {
        public List<Guid> lstEmpMonthySalary { get; set; }

        public override ExportTeacherSalaryParameter ToParameter()
        {
            return new ExportTeacherSalaryParameter()
            {
                lstEmpMonthySalary = this.lstEmpMonthySalary,
                UserId = this.UserId
            };
        }
    }
}
