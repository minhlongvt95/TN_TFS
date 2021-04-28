using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class ExportEmployeeSalaryRequest : BaseRequest<ExportEmployeeSalaryParameter>
    {
        public List<Guid> lstEmpMonthySalary { get; set; }

        public override ExportEmployeeSalaryParameter ToParameter()
        {
            return new ExportEmployeeSalaryParameter()
            {
                lstEmpMonthySalary=this.lstEmpMonthySalary,
                UserId=this.UserId
            };
        }
    }
}
