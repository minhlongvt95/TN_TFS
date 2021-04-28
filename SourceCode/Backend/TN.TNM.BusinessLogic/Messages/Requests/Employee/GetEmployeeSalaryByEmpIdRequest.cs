using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeSalaryByEmpIdRequest : BaseRequest<GetEmployeeSalaryByEmpIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public override GetEmployeeSalaryByEmpIdParameter ToParameter()
        {
            return new GetEmployeeSalaryByEmpIdParameter
            {
                EmployeeId = EmployeeId,
                EffectiveDate = EffectiveDate,
                UserId = UserId
            };
        }
    }
}
