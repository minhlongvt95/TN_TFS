using System;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Employee
{
    public class EmployeeMonthySalaryFactory : BaseFactory, IEmployeeMonthySalary
    {
        private IEmployeeMonthySalaryDataAccess iEmployeeMonthySalaryDataAccess;
        public EmployeeMonthySalaryFactory(IEmployeeMonthySalaryDataAccess _iEmployeeMonthySalaryDataAccess, ILogger<EmployeeMonthySalaryFactory> _logger)
        {
            iEmployeeMonthySalaryDataAccess = _iEmployeeMonthySalaryDataAccess;
            logger = _logger;
        }
        public GetEmployeeMonthySalaryByEmpIdResponse GetEmployeeMonthySalaryByEmpId(GetEmployeeMonthySalaryByEmpIdRequest request)
        {
            try
            {
                logger.LogInformation("GetEmployeeMonthySalaryByEmpId");
                var parameter = request.ToParameter();
                var result = iEmployeeMonthySalaryDataAccess.GetEmployeeMonthySalaryByEmpId(parameter);
                var response = new GetEmployeeMonthySalaryByEmpIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.GET_FAIL,
                    EmployeeMonthySalary = new EmployeeMonthySalaryModel(result.EmployeeMonthlySalary)
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeMonthySalaryByEmpIdResponse()
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
