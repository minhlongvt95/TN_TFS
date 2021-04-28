using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class FindTeacherMonthySalaryRequest : BaseRequest<FindTeacherMonthySalaryParameter>
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override FindTeacherMonthySalaryParameter ToParameter()
        {
            return new FindTeacherMonthySalaryParameter()
            {
                EmployeeName = EmployeeName,
                EmployeeCode = EmployeeCode,
                EmployeePostionId = EmployeePostionId,
                Month = Month,
                Year = Year,
                UserId = UserId,
            };
        }
    }
}
