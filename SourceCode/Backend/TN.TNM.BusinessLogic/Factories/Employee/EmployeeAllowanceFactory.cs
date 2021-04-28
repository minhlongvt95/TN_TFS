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
    public class EmployeeAllowanceFactory : BaseFactory, IEmployeeAllowance
    {
        private IEmployeeAllowanceDataAccess iEmployeeAllowanceDataAccess;
        public EmployeeAllowanceFactory(IEmployeeAllowanceDataAccess _iEmployeeAllowanceDataAccess, ILogger<EmployeeAllowanceFactory> _logger)
        {
            iEmployeeAllowanceDataAccess = _iEmployeeAllowanceDataAccess;
            logger = _logger;
        }
        public GetEmployeeAllowanceByEmpIdResponse GetEmployeeAllowanceByEmpId(GetEmployeeAllowanceByEmpIdRequest request)
        {
            try
            {
                logger.LogInformation("GetEmployeeAllowanceByEmpId");
                var parameter = request.ToParameter();
                var result = iEmployeeAllowanceDataAccess.GetEmployeeAllowanceByEmpId(parameter);
                var response = new GetEmployeeAllowanceByEmpIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.GET_FAIL,
                    EmployeeAllowance = new EmployeeAllowanceModel(result.EmployeeAllowance)
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeAllowanceByEmpIdResponse()
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public EditEmployeeAllowanceResponse EditEmployeeAllowance(EditEmployeeAllowanceRequest request)
        {
            try
            {
                logger.LogInformation("EditEmployeeAllowance");
                var parameter = request.ToParameter();
                var result = iEmployeeAllowanceDataAccess.EditEmployeeAllowance(parameter);
                var response = new EditEmployeeAllowanceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.EDIT_FAIL
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditEmployeeAllowanceResponse()
                {
                    MessageCode = CommonMessage.Employee.EDIT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
