using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeMonthySalaryController : Controller
    {
        private readonly IEmployeeMonthySalary _iEmployeeMonthySalary;
        public EmployeeMonthySalaryController(IEmployeeMonthySalary iEmployeeMonthySalary)
        {
            this._iEmployeeMonthySalary = iEmployeeMonthySalary;
        }

        /// <summary>
        /// Create a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeMonthySalary/search")]
        //[Authorize(Policy = "Member")]
        public GetEmployeeMonthySalaryByEmpIdResponse GetEmployeeMonthySalaryByEmpId([FromBody]GetEmployeeMonthySalaryByEmpIdRequest request)
        {
            return this._iEmployeeMonthySalary.GetEmployeeMonthySalaryByEmpId(request);
        }
    }
}