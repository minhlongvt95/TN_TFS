using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class CheckEmployeeCreateRequest : BaseRequest<CheckEmployeeCreateRequestParameter>
    {
        public Guid EmployeeId { get; set; }

        public override CheckEmployeeCreateRequestParameter ToParameter() => new CheckEmployeeCreateRequestParameter()
        {
            EmployeeId = this.EmployeeId,
        };
    }
}
