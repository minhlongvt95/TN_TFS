using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeRequestController : Controller
    {
        private readonly IEmployeeRequest _iEmployeeRequest;
        public EmployeeRequestController(IEmployeeRequest iEmployeeRequest)
        {
            this._iEmployeeRequest = iEmployeeRequest;
        }

        /// <summary>
        /// Create a new employee request
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/create")]
        [Authorize(Policy = "Member")]
        public CreateEmployeeRequestResponse CreateEmployeeRequest([FromBody]CreateEmployeeRequestRequest request)
        {
            return this._iEmployeeRequest.CreateEmployeeRequest(request);
        }

        /// <summary>
        /// Search employee request
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/search")]
        [Authorize(Policy = "Member")]
        public SearchEmployeeRequestResponse SearchEmployeeRequest([FromBody]SearchEmployeeRequestRequest request)
        {
            return this._iEmployeeRequest.SearchEmployeeRequest(request);
        }

        /// <summary>
        /// Get all employee Request
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/getAllEmployeeRequest")]
        [Authorize(Policy = "Member")]
        public GetAllEmployeeRequestResponse GetAllEmployeeRequest([FromBody]GetAllEmployeeRequestRequest request)
        {
            return this._iEmployeeRequest.GetAllEmployeeRequest(request);
        }

        /// <summary>
        /// Get employee request by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/getEmployeeRequestById")]
        [Authorize(Policy = "Member")]
        public GetEmployeeRequestByIdResponse GetEmployeeRequestById([FromBody]GetEmployeeRequestByIdRequest request)
        {
            return this._iEmployeeRequest.GetEmployeeRequestById(request);
        }

        /// <summary>
        /// Edit employee Request by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/editEmployeeRequestById")]
        [Authorize(Policy = "Member")]
        public EditEmployeeRequestByIdResponse EditEmployeeRequestById([FromBody]EditEmployeeRequestByIdRequest request)
        {
            return this._iEmployeeRequest.EditEmployeeRequestById(request);
        }
        /// <summary>
        /// Edit employee Request by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/getEmployeeRequestByEmpId")]
        [Authorize(Policy = "Member")]
        public GetEmployeeRequestByEmpIdResponse GetEmployeeRequestByEmpId ([FromBody]GetEmployeeRequestByEmpIdRequest request)
        {
            return this._iEmployeeRequest.GetEmployeeRequestByEmpId(request);
        }

        /// <summary>
        /// CheckEmployeeCreateRequest
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/checkEmployeeCreateRequest")]
        [Authorize(Policy = "Member")]
        public CheckEmployeeCreateRequestResponse CheckEmployeeCreateRequest([FromBody]CheckEmployeeCreateRequest request)
        {
            return this._iEmployeeRequest.CheckEmployeeCreateRequest(request);
        }

        /// <summary>
        /// GetDataSearchEmployeeRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeRequest/getDataSearchEmployeeRequest")]
        [Authorize(Policy = "Member")]
        public GetDataSearchEmployeeRequestResponse GetDataSearchEmployeeRequest([FromBody]GetDataSearchEmployeeRequestRequest request)
        {
            return this._iEmployeeRequest.GetDataSearchEmployeeRequest(request);
        }
    }
}