using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Employee
{
    public class EmployeeInsuranceFactory : BaseFactory, IEmployeeInsurance
    {
        private IEmployeeInsuranceDataAccess iEmployeeInsuranceDataAccess;
        public EmployeeInsuranceFactory(IEmployeeInsuranceDataAccess _iEmployeeInsuranceDataAccess, ILogger<EmployeeInsuranceFactory> _logger)
        {
            iEmployeeInsuranceDataAccess = _iEmployeeInsuranceDataAccess;
            logger = _logger;
        }
        public CreateEmployeeInsuranceResponse CreateEmployeeInsurance(CreateEmployeeInsuranceRequest request)
        {
            try
            {
                logger.LogInformation("Create Employee Insurance");
                var parameter = request.ToParameter();
                var result = iEmployeeInsuranceDataAccess.CreateEmployeeInsurance(parameter);
                var response = new CreateEmployeeInsuranceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.CREATE_FAIL,                   
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateEmployeeInsuranceResponse()
                {
                    MessageCode = CommonMessage.Employee.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public SearchEmployeeInsuranceResponse SearchEmployeeInsurance(SearchEmployeeInsuranceRequest request)
        {
            try
            {
                logger.LogInformation("Search Employee Insurance");
                var parameter = request.ToParameter();
                var result = iEmployeeInsuranceDataAccess.SearchEmployeeInsurance(parameter);
                var response = new SearchEmployeeInsuranceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.GET_FAIL,                   
                };
                response.ListEmployeeInsurance = new List<EmployeeInsuranceModel>();
                result.ListEmployeeInsurance.ForEach(empisr =>
                {
                    response.ListEmployeeInsurance.Add(new EmployeeInsuranceModel(empisr));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchEmployeeInsuranceResponse()
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public EditEmployeeInsuranceResponse EditEmployeeInsurance(EditEmployeeInsuranceRequest request)
        {
            try
            {
                logger.LogInformation("Edit Employee Insurance");
                var parameter = request.ToParameter();
                var result = iEmployeeInsuranceDataAccess.EditEmployeeInsurance(parameter);
                var response = new EditEmployeeInsuranceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.EDIT_FAIL,
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditEmployeeInsuranceResponse()
                {
                    MessageCode = CommonMessage.Employee.EDIT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
