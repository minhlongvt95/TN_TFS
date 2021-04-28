using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.Api.Controllers
{
    public class EmployeeSalaryController : Controller
    {
        private readonly IEmployeeSalary _iEmployeeSalary;
        public EmployeeSalaryController(IEmployeeSalary iEmployeeSalary)
        {
            this._iEmployeeSalary = iEmployeeSalary;
        }
        /// <summary>
        /// import time sheet
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/employeeTimeSheetImport")]
        [Authorize(Policy = "Member")]
        public EmployeeTimeSheetImportResponse EmployeeTimeSheetImport(EmployeeTimeSheetImportRequest request)
        {
            return this._iEmployeeSalary.EmployeeTimeSheetImport(request);
        }
        /// <summary>
        /// EmployeeSalaryHandmadeImport
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/employeeSalaryHandmadeImport")]
        [Authorize(Policy = "Member")]
        public EmployeeSalaryHandmadeResponse EmployeeSalaryHandmadeImport(EmployeeSalaryHandmadeRequest request)
        {
            return this._iEmployeeSalary.EmployeeSalaryHandmadeImport(request);
        }
        /// <summary>
        /// Get empsalary by emp id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/getEmployeeSalaryByEmpId")]
        [Authorize(Policy = "Member")]
        public GetEmployeeSalaryByEmpIdResponse GetEmployeeSalaryByEmpId([FromBody]GetEmployeeSalaryByEmpIdRequest request)
        {
            return this._iEmployeeSalary.GetEmployeeSalaryByEmpId(request);
        }
        /// <summary>
        /// Create empsalary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/createEmpSalary")]
        [Authorize(Policy = "Member")]
        public CreateEmployeeSalaryResponse CreateEmployeeSalary([FromBody]CreateEmployeeSalaryRequest request)
        {
            return this._iEmployeeSalary.CreateEmployeeSalary(request);
        }
        /// <summary>
        /// Download Employee TimeSheet Template
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/downloadEmployeeTimeSheetTemplate")]
        [Authorize(Policy = "Member")]
        public DownloadEmployeeTimeSheetTemplateResponse DownloadEmployeeTimeSheetTemplate([FromBody]DownloadEmployeeTimeSheetTemplateRequest request)
        {
            return this._iEmployeeSalary.DownloadEmployeeTimeSheetTemplate(request);
        }
        /// <summary>
        /// Find Employee Monthy Salary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/findEmployeeMonthySalary")]
        [Authorize(Policy = "Member")]
        public FindEmployeeMonthySalaryResponse FindEmployeeMonthySalary([FromBody]FindEmployeeMonthySalaryRequest request)
        {
            return this._iEmployeeSalary.FindEmployeeMonthySalary(request);
        }

        /// <summary>
        /// Get Teacher Salary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/getTeacherSalary")]
        [Authorize(Policy = "Member")]
        public GetTeacherSalaryResponse GetTeacherSalary([FromBody]GetTeacherSalaryRequest request)
        {
            return this._iEmployeeSalary.GetTeacherSalary(request);
        }
        /// <summary>
        /// TeacherSalaryHandmadeImport
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/teacherSalaryHandmadeImport")]
        [Authorize(Policy = "Member")]
        public TeacherSalaryHandmadeResponse TeacherSalaryHandmadeImport(TeacherSalaryHandmadeRequest request)
        {
            return this._iEmployeeSalary.TeacherSalaryHandmadeImport(request);
        }
        /// <summary>
        /// FindTeacherMonthySalary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/findTeacherMonthySalary")]
        [Authorize(Policy = "Member")]
        public FindTeacherMonthySalaryResponse FindTeacherMonthySalary([FromBody]FindTeacherMonthySalaryRequest request)
        {
            return this._iEmployeeSalary.FindTeacherMonthySalary(request);
        }
        /// <summary>
        /// ExportAssistant
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/exportAssistant")]
        [Authorize(Policy = "Member")]
        public ExportAssistantResponse ExportAssistant([FromBody]ExportAssistantRequest request)
        {
            return this._iEmployeeSalary.ExportAssistant(request);
        }
        /// <summary>
        /// AssistantSalaryHandmadeImport
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/assistantSalaryHandmadeImport")]
        [Authorize(Policy = "Member")]
        public AssistantSalaryHandmadeResponse AssistantSalaryHandmadeImport(AssistantSalaryHandmadeRequest request)
        {
            return this._iEmployeeSalary.AssistantSalaryHandmadeImport(request);
        }
        /// <summary>
        /// FindAssistantMonthySalary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/findAssistantMonthySalary")]
        [Authorize(Policy = "Member")]
        public FindAssistantMonthySalaryResponse FindAssistantMonthySalary([FromBody]FindAssistantMonthySalaryRequest request)
        {
            return this._iEmployeeSalary.FindAssistantMonthySalary(request);
        }
        /// <summary>
        /// ExportEmployeeSalary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/exportEmployeeSalary")]
        [Authorize(Policy = "Member")]
        public ExportEmployeeSalaryResponse ExportEmployeeSalary([FromBody]ExportEmployeeSalaryRequest request)
        {
            return this._iEmployeeSalary.ExportEmployeeSalary(request);
        }
        /// <summary>
        /// ExportTeacherSalary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/exportTeacherSalary")]
        [Authorize(Policy = "Member")]
        public ExportTeacherSalaryResponse FindAssistantMonthySalary([FromBody]ExportTeacherSalaryRequest request)
        {
            return this._iEmployeeSalary.ExportTeacherSalary(request);
        }
        /// <summary>
        /// ExportAssistantSalary
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/exportAssistantSalary")]
        [Authorize(Policy = "Member")]
        public ExportAssistantSalaryResponse ExportAssistantSalary([FromBody]ExportAssistantSalaryRequest request)
        {
            return this._iEmployeeSalary.ExportAssistantSalary(request);
        }
        /// <summary>
        /// GetEmployeeSalaryStatus
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employeeSalary/getEmployeeSalaryStatus")]
        [Authorize(Policy = "Member")]
        public GetEmployeeSalaryStatusResponse GetEmployeeSalaryStatus([FromBody]GetEmployeeSalaryStatusRequest request)
        {
            return this._iEmployeeSalary.GetEmployeeSalaryStatus(request);
        }
    }
}