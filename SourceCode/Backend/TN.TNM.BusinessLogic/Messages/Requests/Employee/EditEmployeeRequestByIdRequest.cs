using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeRequestByIdRequest : BaseRequest<EditEmployeeRequestByIdParameter>
    {
        public EmployeeRequestModel EmployeeRequest { get; set; }

        public override EditEmployeeRequestByIdParameter ToParameter()
        {
            return new EditEmployeeRequestByIdParameter()
            {
                EmployeeRequest = EmployeeRequest.ToEntity()
            };
        }
    }
}
