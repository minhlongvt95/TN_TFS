using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IEmployeeInsurance
    {
        CreateEmployeeInsuranceResponse CreateEmployeeInsurance(CreateEmployeeInsuranceRequest request);
        SearchEmployeeInsuranceResponse SearchEmployeeInsurance(SearchEmployeeInsuranceRequest request);
        EditEmployeeInsuranceResponse EditEmployeeInsurance(EditEmployeeInsuranceRequest request);
    }
}
