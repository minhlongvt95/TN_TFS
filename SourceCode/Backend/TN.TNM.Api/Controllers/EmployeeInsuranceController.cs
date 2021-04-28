using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeInsuranceController : Controller
    {
        private readonly IEmployeeInsurance _iEmployeeInsurance;
        public EmployeeInsuranceController(IEmployeeInsurance iEmployeeInsurance)
        {
            this._iEmployeeInsurance = iEmployeeInsurance;
        }

        /// <summary>
        /// Create a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeInsurance/create")]
        //[Authorize(Policy = "Member")]
        public CreateEmployeeInsuranceResponse CreateEmployeeInsurance([FromBody]CreateEmployeeInsuranceRequest request)
        {
            return this._iEmployeeInsurance.CreateEmployeeInsurance(request);
        }
        /// <summary>
        /// Edit a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeInsurance/edit")]
        //[Authorize(Policy = "Member")]
        public EditEmployeeInsuranceResponse EditEmployeeInsurance([FromBody]EditEmployeeInsuranceRequest request)
        {
            return this._iEmployeeInsurance.EditEmployeeInsurance(request);
        }
        /// <summary>
        /// Search a new employee Insurance
        /// </summary>
        /// <param name="Insurance">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeInsurance/search")]
        //[Authorize(Policy = "Member")]
        public SearchEmployeeInsuranceResponse SearchEmployeeInsurance([FromBody]SearchEmployeeInsuranceRequest request)
        {
            return this._iEmployeeInsurance.SearchEmployeeInsurance(request);
        }
    }
}