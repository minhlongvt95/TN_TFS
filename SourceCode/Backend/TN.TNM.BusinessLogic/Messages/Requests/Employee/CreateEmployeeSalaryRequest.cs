using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class CreateEmployeeSalaryRequest : BaseRequest<CreateEmployeeSalaryParameter>
    {
        public EmployeeSalaryModel EmployeeSalary { get; set; }
        public override CreateEmployeeSalaryParameter ToParameter()
        {
            return new CreateEmployeeSalaryParameter()
            {
                EmployeeSalary = EmployeeSalary.ToEntity(),
                UserId = UserId
            };
        }
    }
}
