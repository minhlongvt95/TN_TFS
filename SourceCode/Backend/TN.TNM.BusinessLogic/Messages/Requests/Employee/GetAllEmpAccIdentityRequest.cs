using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetAllEmpAccIdentityRequest : BaseRequest<GetAllEmpAccIdentityParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetAllEmpAccIdentityParameter ToParameter() => new GetAllEmpAccIdentityParameter()
        {
            EmployeeId = EmployeeId,
            UserId = UserId
        };
    }
}
