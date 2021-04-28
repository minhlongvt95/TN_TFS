using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class CreateEmployeeRequestRequest : BaseRequest<CreateEmplyeeRequestParameter>
    {
        public EmployeeRequestModel EmployeeRequest { get; set; }
        public override CreateEmplyeeRequestParameter ToParameter() => new CreateEmplyeeRequestParameter()
        {
            EmployeeRequest = EmployeeRequest.ToEntity()
        };
    }
}
