using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeDataAccess
    {
        CreateEmployeeResult CreateEmployee(CreateEmployeeParameter parameter);
        SearchEmployeeResult SearchEmployee(SearchEmployeeParameter parameter);
        GetAllEmployeeResult GetAllEmployee(GetAllEmployeeParameter parameter);
        GetEmployeeByIdResult GetEmployeeById(GetEmployeeByIdParameter parameter);
        EditEmployeeByIdResult EditEmployeeById(EditEmployeeByIdParameter parameter);
        GetAllEmpAccountResult GetAllEmpAccount(GetAllEmpAccountParameter parameter);
        GetAllEmployeeAccountResult GetAllEmployeeAccount();
        GetAllEmpIdentityResult GetAllEmpIdentity(GetAllEmpIdentityParameter parameter);
        EditEmployeeDataPermissionResult EditEmployeeDataPermission(EditEmployeeDataPermissionParameter parameter);
        EmployeePermissionMappingResult EmployeePermissionMapping(EmployeePermissionMappingParameter parameter);
        GetEmployeeByPositionCodeResult GetEmployeeByPositionCode(GetEmployeeByPositionCodeParameter parameter);
        GetEmployeeHighLevelByEmpIdResult GetEmployeeHighLevelByEmpId(GetEmployeeHighLevelByEmpIdParameter parameter);
        GetEmployeeByOrganizationIdResult GetEmployeeByOrganizationId(GetEmployeeByOrganizationIdParameter parameter);
        GetEmployeeByTopRevenueResult GetEmployeeByTopRevenue(GetEmployeeByTopRevenueParameter parameter);
        ExportEmployeeRevenueResult ExportEmployeeRevenue(ExportEmployeeRevenueParameter parameter);
        GetStatisticForEmpDashBoardResult GetStatisticForEmpDashBoard(GetStatisticForEmpDashBoardParameter parameter);
        GetEmployeeCareStaffResult GetEmployeeCareStaff(GetEmployeeCareStaffParameter parameter);
        SearchEmployeeFromListResult SearchEmployeeFromList(SearchEmployeeFromListParameter parameter);
        GetAllEmpAccIdentityResult GetAllEmpAccIdentity(GetAllEmpAccIdentityParameter parameter);
        DisableEmployeeResult DisableEmployee(DisableEmployeeParameter parameter);
        CheckAdminLoginResult CheckAdminLogin(CheckAdminLoginParameter parameter);
    }
}
