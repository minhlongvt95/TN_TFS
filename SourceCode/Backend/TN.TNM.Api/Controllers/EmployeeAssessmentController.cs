using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeAssessmentController : Controller
    {
        private readonly IEmployeeAssessment _iEmployeeAssessment;
        public EmployeeAssessmentController(IEmployeeAssessment iEmployeeAssessment)
        {
            this._iEmployeeAssessment = iEmployeeAssessment;
        }

        /// <summary>
        /// getEmployeeRequestById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAssessment/searchEmployeeAssessment")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeAssessmentResponse GetEmployeeRequestById([FromBody]SearchEmployeeAssessmentRequest request)
        {
            return this._iEmployeeAssessment.SearchEmployeeAssessment(request);
        }
        /// <summary>
        /// getAllYearToAssessment
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAssessment/getAllYearToAssessment")]
        [Authorize(Policy = "Member")]
        public GetAllYearToAssessmentResponse GetAllYearToAssessment([FromBody]GetAllYearToAssessmentRequest request)
        {
            return this._iEmployeeAssessment.GetAllYearToAssessment(request);
        }
        /// <summary>
        /// getAllYearToAssessment
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeAssessment/editEmployeeAssessment")]
        [Authorize(Policy = "Member")]
        public EditEmployeeAssessmentResponse EditEmployeeAssessment([FromBody]EditEmployeeAssessmentRequest request)
        {
            return this._iEmployeeAssessment.EditEmployeeAssessment(request);
        }
    }
}