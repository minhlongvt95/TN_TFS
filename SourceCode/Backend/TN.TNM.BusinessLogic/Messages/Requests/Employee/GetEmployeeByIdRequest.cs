using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeByIdRequest : BaseRequest<GetEmployeeByIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public Guid ContactId { get; set; }

        public override GetEmployeeByIdParameter ToParameter()
        {
            return new GetEmployeeByIdParameter()
            {
                EmployeeId = EmployeeId,
                ContactId = ContactId,
                UserId = UserId
            };
        }
    }
}
