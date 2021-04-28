using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeInsuranceDataAccess
    {
        CreateEmployeeInsuranceResult CreateEmployeeInsurance(CreateEmployeeInsuranceParameter parameter);
        EditEmployeeInsuranceResult EditEmployeeInsurance(EditEmployeeInsuranceParameter parameter);
        SearchEmployeeInsuranceResult SearchEmployeeInsurance(SearchEmployeeInsuranceParameter parameter);
    }
}
