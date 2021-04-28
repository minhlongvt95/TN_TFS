using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{

    public class EmployeeAllowanceController : Controller
    {
        private readonly IEmployeeAllowance _iEmployeeAllowance;
        public EmployeeAllowanceController(IEmployeeAllowance iEmployeeAllowance)
        {
            this._iEmployeeAllowance = iEmployeeAllowance;
        }

        /// <summary>
        /// getEmployeeRequestById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAllowance/getEmployeeAllowanceById")]
        [Authorize(Policy = "Member")]
        public GetEmployeeAllowanceByEmpIdResponse GetEmployeeRequestById([FromBody]GetEmployeeAllowanceByEmpIdRequest request)
        {
            return this._iEmployeeAllowance.GetEmployeeAllowanceByEmpId(request);
        }
        /// <summary>
        /// getEmployeeRequestById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAllowance/editEmployeeAllowance")]
        [Authorize(Policy = "Member")]
        public EditEmployeeAllowanceResponse EditEmployeeAllowance([FromBody]EditEmployeeAllowanceRequest request)
        {
            return this._iEmployeeAllowance.EditEmployeeAllowance(request);
        }
    }
}