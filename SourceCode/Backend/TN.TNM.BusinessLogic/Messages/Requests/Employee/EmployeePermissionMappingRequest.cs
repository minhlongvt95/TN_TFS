using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EmployeePermissionMappingRequest : BaseRequest<EmployeePermissionMappingParameter>
    {
        public Guid PermissionSetId { get; set; }
        public Guid EmployeeId { get; set; }
        public override EmployeePermissionMappingParameter ToParameter()
        {
            return new EmployeePermissionMappingParameter()
            {
                UserId = UserId,
                PermissionSetId = PermissionSetId,
                EmployeeId = EmployeeId
            };
        }
    }
}
