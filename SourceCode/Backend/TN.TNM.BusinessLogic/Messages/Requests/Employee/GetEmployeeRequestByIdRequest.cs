using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeRequestByIdRequest : BaseRequest<GetEmployeeRequestByIdParameter>
    {
        public Guid EmployeeRequestId { get; set; }

        public override GetEmployeeRequestByIdParameter ToParameter()
        {
            return new GetEmployeeRequestByIdParameter()
            {
                EmployeeRequestId = EmployeeRequestId
            };
        }
    }
}
