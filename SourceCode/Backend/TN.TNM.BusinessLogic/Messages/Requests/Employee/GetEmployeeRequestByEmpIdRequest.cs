using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeRequestByEmpIdRequest : BaseRequest<GetEmployeeRequestByEmpIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetEmployeeRequestByEmpIdParameter ToParameter()
        {
            return new GetEmployeeRequestByEmpIdParameter()
            {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
