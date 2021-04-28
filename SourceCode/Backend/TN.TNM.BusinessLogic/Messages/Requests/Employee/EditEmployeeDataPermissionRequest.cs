using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeDataPermissionRequest : BaseRequest<EditEmployeeDataPermissionParameter>
    {
        public Guid EmployeeId { get; set; }
        public Boolean IsManager { get; set; }
        public override EditEmployeeDataPermissionParameter ToParameter()
        {
            return new EditEmployeeDataPermissionParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId,
                IsManager = IsManager
            };
        }
    }
}
