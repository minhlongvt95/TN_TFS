using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IEmployee
    {
        CreateEmployeeResponse CreateEmployee(CreateEmployeeRequest request);
        SearchEmployeeResponse SearchEmployee(SearchEmployeeRequest request);
        GetAllEmployeeResponse GetAllEmployee(GetAllEmployeeRequest request);
        GetEmployeeByIdResponse GetEmployeeById(GetEmployeeByIdRequest request);
        EditEmployeeByIdResponse EditEmployeeById(EditEmployeeByIdRequest request);
        GetAllEmpAccountResponse GetAllEmpAccount(GetAllEmpAccountRequest request);
        GetAllEmployeeAccountResponse GetAllEmployeeAccount();
        GetAllEmpIdentityResponse GetAllEmpIdentity(GetAllEmpIdentityRequest request);
        EditEmployeeDataPermissionResponse EditEmployeeDataPermission(EditEmployeeDataPermissionRequest request);
        EmployeePermissionMappingResponse EmployeePermissionMapping(EmployeePermissionMappingRequest request);
        GetEmployeeByPositionCodeResponse GetEmployeeByPositionCode(GetEmployeeByPositionCodeRequest request);
        GetEmployeeHighLevelByEmpIdResponse GetEmployeeHighLevelByEmpId(GetEmployeeHighLevelByEmpIdRequest request);
        GetEmployeeByOrganizationIdResponse GetEmployeeByOrganizationId(GetEmployeeByOrganizationIdRequest request);
        GetEmployeeByTopRevenueResponse GetEmployeeByTopRevenue(GetEmployeeByTopRevenueRequest request);
        ExportEmployeeRevenueResponse ExportEmployeeRevenue(ExportEmployeeRevenueRequest request);
        GetStatisticForEmpDashBoardResponse GetStatisticForEmpDashBoard(GetStatisticForEmpDashBoardRequest request);
        GetEmployeeCareStaffResponse GetEmployeeCareStaff(GetEmployeeCareStaffRequest request);
        SearchEmployeeFromListResponse SearchEmployeeFromList(SearchEmployeeFromListRequest request);
        GetAllEmpAccIdentityResponse GetAllEmpAccIdentity(GetAllEmpAccIdentityRequest request);
        DisableEmployeeResponse DisableEmployee(DisableEmployeeRequest request);
        CheckAdminLoginResponse CheckAdminLogin(CheckAdminLoginRequest request);
    }
}
