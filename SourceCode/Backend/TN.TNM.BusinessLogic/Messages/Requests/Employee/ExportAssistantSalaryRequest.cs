using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class ExportAssistantSalaryRequest : BaseRequest<ExportAssistantSalaryParameter>
    {
        public List<Guid> lstEmpMonthySalary { get; set; }

        public override ExportAssistantSalaryParameter ToParameter()
        {
            return new ExportAssistantSalaryParameter()
            {
                lstEmpMonthySalary = this.lstEmpMonthySalary,
                UserId = this.UserId
            };
        }
    }
}
