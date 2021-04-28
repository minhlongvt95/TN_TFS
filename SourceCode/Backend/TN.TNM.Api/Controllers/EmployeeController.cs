using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployee _iEmployee;
        public EmployeeController(IEmployee iEmployee)
        {
            this._iEmployee = iEmployee;
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/create")]
        [Authorize(Policy = "Member")]
        public CreateEmployeeResponse CreateEmployee([FromBody]CreateEmployeeRequest request)
        {
            return this._iEmployee.CreateEmployee(request);
        }

        /// <summary>
        /// Search employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/search")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeResponse SearchEmployee([FromBody]SearchEmployeeRequest request)
        {
            return this._iEmployee.SearchEmployee(request);
        }

        /// <summary>
        /// Search employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmployee")]
        [Authorize(Policy = "Member")]
        public GetAllEmployeeResponse GetAllEmployee([FromBody]GetAllEmployeeRequest request)
        {
            return this._iEmployee.GetAllEmployee(request);
        }

        /// <summary>
        /// Get employee by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeById")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByIdResponse GetEmployeeById([FromBody]GetEmployeeByIdRequest request)
        {
            return this._iEmployee.GetEmployeeById(request);
        }

        /// <summary>
        /// Edit employee by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/editEmployeeById")]
        [Authorize(Policy = "Member")]
        public EditEmployeeByIdResponse EditEmployeeById([FromBody]EditEmployeeByIdRequest request)
        {
            return this._iEmployee.EditEmployeeById(request);
        }

        /// <summary>
        /// Get All Emp Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmpAccount")]
        [Authorize(Policy = "Member")]
        public GetAllEmpAccountResponse GetAllEmpAccount([FromBody]GetAllEmpAccountRequest request)
        {
            return this._iEmployee.GetAllEmpAccount(request);
        }

        /// <summary>
        /// Get All Employee Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmployeeAccount")]
        [Authorize(Policy = "Member")]
        public GetAllEmployeeAccountResponse GetAllEmployeeAccount()
        {
            return this._iEmployee.GetAllEmployeeAccount();
        }

        /// <summary>
        /// Get All Emp Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmpIdentity")]
        [Authorize(Policy = "Member")]
        public GetAllEmpIdentityResponse GetAllEmpIdentity([FromBody]GetAllEmpIdentityRequest request)
        {
            return this._iEmployee.GetAllEmpIdentity(request);
        }

        /// <summary>
        /// Get All Emp Account
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/editEmployeeDataPermission")]
        [Authorize(Policy = "Member")]
        public EditEmployeeDataPermissionResponse EditEmployeeDataPermission([FromBody]EditEmployeeDataPermissionRequest request)
        {
            return this._iEmployee.EditEmployeeDataPermission(request);
        }

        /// <summary>
        /// Grand employee module permisison
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/employeePermissionMapping")]
        [Authorize(Policy = "Member")]
        public EmployeePermissionMappingResponse EmployeePermissionMapping([FromBody]EmployeePermissionMappingRequest request)
        {
            return this._iEmployee.EmployeePermissionMapping(request);
        }

        /// <summary>
        /// GetEmployeeByPositionCode
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeByPositionCode")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByPositionCodeResponse GetEmployeeByPositionCode([FromBody]GetEmployeeByPositionCodeRequest request)
        {
            return this._iEmployee.GetEmployeeByPositionCode(request);
        }
        /// <summary>
        /// GetEmployeeApprove
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeApprove")]
        [Authorize(Policy = "Member")]
        public GetEmployeeHighLevelByEmpIdResponse GetEmployeeHighLevelByEmpId([FromBody]GetEmployeeHighLevelByEmpIdRequest request)
        {
            return this._iEmployee.GetEmployeeHighLevelByEmpId(request);
        }

        /// <summary>
        /// GetEmployeeByOrganizationId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeByOrganizationId")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByOrganizationIdResponse GetEmployeeByOrganizationId([FromBody]GetEmployeeByOrganizationIdRequest request)
        {
            return this._iEmployee.GetEmployeeByOrganizationId(request);
        }

        /// <summary>
        /// GetEmployeeByTopRevenue
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeByTopRevenue")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByTopRevenueResponse GetEmployeeByTopRevenue([FromBody]GetEmployeeByTopRevenueRequest request)
        {
            return this._iEmployee.GetEmployeeByTopRevenue(request);
        }

        /// <summary>
        /// ExportEmployeeRevenue
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/exportEmployeeRevenue")]
        [Authorize(Policy = "Member")]
        public ExportEmployeeRevenueResponse ExportEmployeeRevenue([FromBody]ExportEmployeeRevenueRequest request)
        {
            return this._iEmployee.ExportEmployeeRevenue(request);
        }
        /// <summary>
        /// GetStatisticForEmpDashBoard
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getStatisticForEmpDashBoard")]
        [Authorize(Policy = "Member")]
        public GetStatisticForEmpDashBoardResponse GetStatisticForEmpDashBoard([FromBody]GetStatisticForEmpDashBoardRequest request)
        {
            return this._iEmployee.GetStatisticForEmpDashBoard(request);
        }

        /// <summary>
        /// GetEmployeeCareStaff
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getEmployeeCareStaff")]
        [Authorize(Policy = "Member")]
        public GetEmployeeCareStaffResponse GetEmployeeCareStaff([FromBody]GetEmployeeCareStaffRequest request)
        {
            return this._iEmployee.GetEmployeeCareStaff(request);
        }

        /// <summary>
        /// Search employee from list
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/searchFromList")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeFromListResponse SearchEmployeeFromList([FromBody]SearchEmployeeFromListRequest request)
        {
            return this._iEmployee.SearchEmployeeFromList(request);
        }

        /// <summary>
        /// Get All Employee Account Identity
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/getAllEmpAccIdentity")]
        [Authorize(Policy = "Member")]
        public GetAllEmpAccIdentityResponse GetAllEmpAccIdentity([FromBody]GetAllEmpAccIdentityRequest request)
        {
            return this._iEmployee.GetAllEmpAccIdentity(request);
        }

        /// <summary>
        /// Disable Employee
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/disableEmployee")]
        [Authorize(Policy = "Member")]
        public DisableEmployeeResponse DisableEmployee([FromBody]DisableEmployeeRequest request)
        {
            return this._iEmployee.DisableEmployee(request);
        }

        /// <summary>
        /// CheckAdminLogin
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/checkAdminLogin")]
        [Authorize(Policy = "Member")]
        public CheckAdminLoginResponse CheckAdminLogin([FromBody]CheckAdminLoginRequest request)
        {
            return this._iEmployee.CheckAdminLogin(request);
        }
    }
}

