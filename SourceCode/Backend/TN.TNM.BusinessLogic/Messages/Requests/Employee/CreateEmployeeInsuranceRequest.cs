using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class CreateEmployeeInsuranceRequest : BaseRequest<CreateEmployeeInsuranceParameter>
    {
        public EmployeeInsuranceModel EmployeeInsurance { get; set; }
        public override CreateEmployeeInsuranceParameter ToParameter()
        {
            return new CreateEmployeeInsuranceParameter()
            {
                EmployeeInsurance = EmployeeInsurance.ToEntity(),
                UserId = UserId
            };
        }
    }
}
