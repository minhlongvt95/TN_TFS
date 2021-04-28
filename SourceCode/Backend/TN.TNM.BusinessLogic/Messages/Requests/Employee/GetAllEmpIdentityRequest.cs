using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetAllEmpIdentityRequest : BaseRequest<GetAllEmpIdentityParameter>
    {
        public Guid CurrentEmpId { get; set; }
        public override GetAllEmpIdentityParameter ToParameter()
        {
            return new GetAllEmpIdentityParameter() {
                UserId = UserId,
                CurrentEmpId = CurrentEmpId
            };
        }
    }
}
