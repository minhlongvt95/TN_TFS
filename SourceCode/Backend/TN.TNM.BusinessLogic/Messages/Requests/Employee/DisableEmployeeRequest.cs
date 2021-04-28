using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class DisableEmployeeRequest : BaseRequest<DisableEmployeeParameter>
    {
        public Guid EmployeeId { get; set; }
        public override DisableEmployeeParameter ToParameter()
        {
            return new DisableEmployeeParameter()
            {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
