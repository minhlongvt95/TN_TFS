using System;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.BankBook;
using TN.TNM.BusinessLogic.Messages.Requests.BankBook;
using TN.TNM.BusinessLogic.Messages.Responses.BankBook;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.BankBook
{
    public class BankBookFactory : BaseFactory, IBankBook
    {
        private IBankBookDataAccess iBankBookDataAccess;
        public BankBookFactory(IBankBookDataAccess _iBankBookDataAccess, ILogger<BankBookFactory> _logger)
        {
            iBankBookDataAccess = _iBankBookDataAccess;
            logger = _logger;
        }
        public SearchBankBookResponse SearchBankBook(SearchBankBookRequest request)
        {
            try
            {
                logger.LogInformation("Search Bank Book");
                var parameter = request.ToParameter();
                var result = iBankBookDataAccess.SearchBankBook(parameter);
                return new SearchBankBookResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ClosingBalance = result.ClosingBalance,
                    OpeningBalance = result.OpeningBalance
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchBankBookResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.BankBook.SEARCH_FAIL
                };
            }

        }

        public GetMasterDataSearchBankBookResponse GetMasterDataSearchBankBook(GetMasterDataSearchBankBookRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iBankBookDataAccess.GetMasterDataSearchBankBook(parameter);

                var response = new GetMasterDataSearchBankBookResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListBankAccount = new System.Collections.Generic.List<Models.BankAccount.BankAccountModel>(),
                    ListEmployee = new System.Collections.Generic.List<Models.Employee.EmployeeModel>()
                };

                result.ListBankAccount.ForEach(item =>
                {
                    response.ListBankAccount.Add(new Models.BankAccount.BankAccountModel(item));
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new Models.Employee.EmployeeModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchBankBookResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

    }
}
