using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeCareStaffRequest : BaseRequest<GetEmployeeCareStaffParameter>
    {
        public bool IsManager { get; set; }
        public Guid EmployeeId { get; set; }

        public override GetEmployeeCareStaffParameter ToParameter()
        {
            return new GetEmployeeCareStaffParameter()
            {
                IsManager = IsManager,
                EmployeeId = EmployeeId
            };
        }
    }
}
