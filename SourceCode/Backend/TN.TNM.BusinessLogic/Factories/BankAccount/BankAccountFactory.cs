using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.BankAccount;
using TN.TNM.BusinessLogic.Messages.Requests.BankAccount;
using TN.TNM.BusinessLogic.Messages.Responses.BankAccount;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.BankAccount
{
    public class BankAccountFactory : BaseFactory, IBankAccount
    {
        private IBankAccountDataAccess iBankAccountDataAccess;
        public BankAccountFactory(IBankAccountDataAccess _iBankAccountDataAccess, ILogger<BankAccountFactory> _logger)
        {
            iBankAccountDataAccess = _iBankAccountDataAccess;
            logger = _logger;
        }
        public CreateBankAccountResponse CreateBankAccount(CreateBankAccountRequest request)
        {
            try
            {
                logger.LogInformation("Create/Update Bank");
                var parameter = request.ToParameter();
                var result = iBankAccountDataAccess.CreateBankAccount(parameter);
                var response = new CreateBankAccountResponse() {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListBankAccount = new List<BankAccountModel>()
                };

                result.ListBankAccount.ForEach(item =>
                {
                    response.ListBankAccount.Add(new BankAccountModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateBankAccountResponse() {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetBankAccountByIdResponse GetBankAccountById(GetBankAccountByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Bank");
                var parameter = request.ToParameter();
                var result = iBankAccountDataAccess.GetBankAccountById(parameter);
                return new GetBankAccountByIdResponse() {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    BankAccount = new BankAccountModel(result.BankAccount)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetBankAccountByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetAllBankAccountByObjectResponse GetAllBankAccountByObject(GetAllBankAccountByObjectRequest request)
        {
            try
            {
                logger.LogInformation("Get Bank");
                var parameter = request.ToParameter();
                var result = iBankAccountDataAccess.GetAllBankAccountByObject(parameter);
                var response = new GetAllBankAccountByObjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    BankAccountList = result.BankAccountList
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllBankAccountByObjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public EditBankAccountResponse EditBankAccount(EditBankAccountRequest request)
        {
            try
            {
                logger.LogInformation("Edit Bank");
                var parameter = request.ToParameter();
                var result = iBankAccountDataAccess.EditBankAccount(parameter);
                return new EditBankAccountResponse() {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditBankAccountResponse() {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.BankAccount.EDIT_BANK_FAIL
                };
            }
        }

        public DeleteBankAccountByIdResponse DeleteBankAccount(DeleteBankAccountByIdRequest request)
        {
            try
            {
                logger.LogInformation("Delete Bank");
                var parameter = request.ToParameter();
                var result = iBankAccountDataAccess.DeleteBankAccountById(parameter);
                var response = new DeleteBankAccountByIdResponse() {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListBankAccount = new List<BankAccountModel>()
                };

                result.ListBankAccount.ForEach(item =>
                {
                    response.ListBankAccount.Add(new BankAccountModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DeleteBankAccountByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.BankAccount.DELETE_BANK_FAIL
                };
            }
        }

        public GetCompanyBankAccountResponse GetCompanyBankAccount(GetCompanyBankAccountRequest request)
        {
            try
            {
                logger.LogInformation("Get all Company Bank");
                var parameter = request.ToParameter();
                var result = iBankAccountDataAccess.GetCompanyBankAccount(parameter);
                var response = new GetCompanyBankAccountResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    BankList = new List<BankAccountModel>()
                };
                result.BankList.ForEach(bank => {
                    response.BankList.Add(new BankAccountModel(bank));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCompanyBankAccountResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
    }
}
