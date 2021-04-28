using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeAllowanceByEmpIdRequest : BaseRequest<GetEmployeeAllowanceByEmpIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetEmployeeAllowanceByEmpIdParameter ToParameter()
        {
            return new GetEmployeeAllowanceByEmpIdParameter {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
