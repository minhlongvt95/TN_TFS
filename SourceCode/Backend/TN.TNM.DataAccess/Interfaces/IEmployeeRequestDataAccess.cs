using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeRequestDataAccess
    {
        CreateEmployeeRequestResult CreateEmployeeRequest(CreateEmplyeeRequestParameter parameter);
        GetAllEmployeeRequestResult GetAllEmployeeRequest(GetAllEmployeeRequestParameter parameter);
        GetEmployeeRequestByIdResult GetEmployeeRequestById(GetEmployeeRequestByIdParameter parameter);
        EditEmployeeRequestByIdResult EditEmployeeRequestById(EditEmployeeRequestByIdParameter parameter);
        SearchEmployeeRequestResult SearchEmployeeRequest(SearchEmployeeRequestParameter parameter);
        GetEmployeeRequestByEmpIdResult GetEmployeeRequestByEmpId(GetEmployeeRequestByEmpIdParameter parameter);
        CheckEmployeeCreateRequestResult CheckEmployeeCreateRequest(CheckEmployeeCreateRequestParameter parameter);
        GetDataSearchEmployeeRequestResult GetDataSearchEmployeeRequest(GetDataSearchEmployeeRequestParameter parameter);
    }
}
