using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IEmployeeRequest
    {
        CreateEmployeeRequestResponse CreateEmployeeRequest(CreateEmployeeRequestRequest request);
        SearchEmployeeRequestResponse SearchEmployeeRequest(SearchEmployeeRequestRequest request);
        GetAllEmployeeRequestResponse GetAllEmployeeRequest(GetAllEmployeeRequestRequest request);
        GetEmployeeRequestByIdResponse GetEmployeeRequestById(GetEmployeeRequestByIdRequest request);
        EditEmployeeRequestByIdResponse EditEmployeeRequestById(EditEmployeeRequestByIdRequest request);
        GetEmployeeRequestByEmpIdResponse GetEmployeeRequestByEmpId(GetEmployeeRequestByEmpIdRequest request);
        CheckEmployeeCreateRequestResponse CheckEmployeeCreateRequest(CheckEmployeeCreateRequest request);
        GetDataSearchEmployeeRequestResponse GetDataSearchEmployeeRequest(GetDataSearchEmployeeRequestRequest request);
    }
}
