using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.CashBook;
using TN.TNM.BusinessLogic.Messages.Requests.CashBook;
using TN.TNM.BusinessLogic.Messages.Responses.CashBook;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.CashBook
{
    public class CashBookFactory:BaseFactory, ICashBook
    {
        private ICashBookDataAccess iCashBookDataAccess;
        public CashBookFactory(ICashBookDataAccess _iCashBookDataAccess, ILogger<CashBookFactory> _logger)
        {
            iCashBookDataAccess = _iCashBookDataAccess;
            logger = _logger;
        }

        public GetDataSearchCashBookResponse GetDataSearchCashBook(GetDataSearchCashBookRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCashBookDataAccess.GetDataSearchCashBook(parameter);
                var response = new GetDataSearchCashBookResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = new List<Models.Employee.EmployeeModel>(),
                    ListOrganization = new List<Models.Admin.OrganizationModel>(),
                };
                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new Models.Employee.EmployeeModel(item));
                });

                result.ListOrganization.ForEach(item =>
                {
                    response.ListOrganization.Add(new Models.Admin.OrganizationModel(item));
                });

                return response;
            }
            catch(Exception ex)
            {
                return new GetDataSearchCashBookResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public GetSurplusCashBookPerMonthResponse GetSurplusCashBookPerMonth (GetSurplusCashBookPerMonthRequest request)
        {
            try
            {
                logger.LogInformation("Search Cash Book");
                var parameter = request.ToParameter();
                var result = iCashBookDataAccess.GetSurplusCashBookPerMonth(parameter);
                return new GetSurplusCashBookPerMonthResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ClosingSurplus = result.ClosingSurplus,
                    OpeningSurplus = result.OpeningSurplus
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetSurplusCashBookPerMonthResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.BankBook.SEARCH_FAIL
                };
            }
        }
    }
}
