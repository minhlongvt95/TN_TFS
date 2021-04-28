using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class FindAssistantMonthySalaryRequest : BaseRequest<FindAssistantMonthySalaryParameter>
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeUnit { get; set; }
        public string EmployeeBranch { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override FindAssistantMonthySalaryParameter ToParameter()
        {
            return new FindAssistantMonthySalaryParameter()
            {
                EmployeeName=this.EmployeeName,
                EmployeeBranch=this.EmployeeBranch,
                EmployeeCode=this.EmployeeCode,
                EmployeePostionId=this.EmployeePostionId,
                EmployeeUnit=this.EmployeeUnit,
                Month=this.Month,
                Year=this.Year,
                UserId=this.UserId
            };
        }
    }
}
