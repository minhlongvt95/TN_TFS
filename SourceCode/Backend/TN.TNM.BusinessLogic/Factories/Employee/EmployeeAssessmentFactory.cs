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
    public class EmployeeAssessmentFactory : BaseFactory, IEmployeeAssessment
    {
        private IEmployeeAssessmentDataAccess iEmployeeAssessmentDataAccess;
        public EmployeeAssessmentFactory(IEmployeeAssessmentDataAccess _iEmployeeAssessmentDataAccess, ILogger<EmployeeAssessmentFactory> _logger)
        {
            iEmployeeAssessmentDataAccess = _iEmployeeAssessmentDataAccess;
            logger = _logger;
        }
        public SearchEmployeeAssessmentResponse SearchEmployeeAssessment(SearchEmployeeAssessmentRequest request)
        {
            try
            {
                logger.LogInformation("SearchEmployeeAssessment");
                var parameter = request.ToParameter();
                var result = iEmployeeAssessmentDataAccess.SearchEmployeeAssessment(parameter);
                var response = new SearchEmployeeAssessmentResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.GET_FAIL,
                };
                response.ListEmployeeAssessment = new List<EmployeeAssessmentModel>();
                result.ListEmployeeAssessment.ForEach(ea =>
                {
                    response.ListEmployeeAssessment.Add(new EmployeeAssessmentModel(ea));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchEmployeeAssessmentResponse()
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public GetAllYearToAssessmentResponse GetAllYearToAssessment(GetAllYearToAssessmentRequest request)
        {
            try
            {
                logger.LogInformation("GetAllYearToAssessment");
                var parameter = request.ToParameter();
                var result = iEmployeeAssessmentDataAccess.GetAllYearToAssessment(parameter);
                var response = new GetAllYearToAssessmentResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.GET_FAIL,
                    ListYear = result.ListYear
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllYearToAssessmentResponse()
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public EditEmployeeAssessmentResponse EditEmployeeAssessment(EditEmployeeAssessmentRequest request)
        {
            try
            {
                logger.LogInformation("EditEmployeeAssessment");
                var parameter = request.ToParameter();
                var result = iEmployeeAssessmentDataAccess.EditEmployeeAssessment(parameter);
                var response = new EditEmployeeAssessmentResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.Employee.GET_FAIL,
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditEmployeeAssessmentResponse()
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
