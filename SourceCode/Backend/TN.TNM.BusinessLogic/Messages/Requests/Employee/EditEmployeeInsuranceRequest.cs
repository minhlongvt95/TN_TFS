using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeInsuranceRequest : BaseRequest<EditEmployeeInsuranceParameter>
    {
        public EmployeeInsuranceModel EmployeeInsurance { get; set; }
        public override EditEmployeeInsuranceParameter ToParameter()
        {
            return new EditEmployeeInsuranceParameter(){
                EmployeeInsurance = EmployeeInsurance.ToEntity(),
                UserId = UserId
            };
        }
    }
}
