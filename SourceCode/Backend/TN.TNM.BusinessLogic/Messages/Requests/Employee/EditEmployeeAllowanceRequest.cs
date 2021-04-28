using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeAllowanceRequest : BaseRequest<EditEmployeeAllowanceParameter>
    {
        public EmployeeAllowanceModel EmployeeAllowance { get; set; }
        public override EditEmployeeAllowanceParameter ToParameter()
        {
            return new EditEmployeeAllowanceParameter()
            {
                EmployeeAllowance = EmployeeAllowance.ToEntity(),
                UserId = UserId
            };
        }
    }
}
