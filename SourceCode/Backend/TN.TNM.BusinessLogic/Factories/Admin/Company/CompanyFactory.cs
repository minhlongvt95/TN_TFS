using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Requests.CompanyConfig;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Company;
using TN.TNM.BusinessLogic.Messages.Responses.CompanyConfig;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Company;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Company
{
    public class CompanyFactory : BaseFactory, ICompany
    {
        private ICompanyDataAccess iCompanyDataAccess;
        public CompanyFactory(ICompanyDataAccess _iCompanyDataAccess, ILogger<CompanyFactory> _logger)
        {
            this.iCompanyDataAccess = _iCompanyDataAccess;
            this.logger = _logger;
        }

        public ChangeSystemParameterResponse ChangeSystemParameter(ChangeSystemParameterRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCompanyDataAccess.ChangeSystemParameter(parameter);
                var response = new ChangeSystemParameterResponse
                {
                    StatusCode = result.Status == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    SystemParameterList = result.SystemParameterList
                };
                return response;
            }
            catch (Exception e)
            {
                return new ChangeSystemParameterResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public EditCompanyConfigResponse EditCompanyConfig(EditCompanyConfigRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Company Config");
                var parameter = request.ToParameter();
                var result = iCompanyDataAccess.EditCompanyConfig(parameter);
                var response = new EditCompanyConfigResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CompanyID=result.CompanyID
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new EditCompanyConfigResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetAllCompanyResponse GetAllCompany(GetAllCompanyRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Company");
                var parameter = request.ToParameter();
                var result = iCompanyDataAccess.GetAllCompany(parameter);
                var response = new GetAllCompanyResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Company = new List<CompanyModel>()
                };
                result.Company.ForEach(companyEntity =>
                {
                    response.Company.Add(new CompanyModel(companyEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllCompanyResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetAllSystemParameterResponse GetAllSystemParameter(GetAllSystemParameterRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All System Parameter");
                var parameter = request.ToParameter();
                var result = iCompanyDataAccess.GetAllSystemParameter(parameter);
                var response = new GetAllSystemParameterResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    systemParameterList = new List<SystemParameterModel>()
                };
                result.systemParameterList.ForEach(systemParameter =>
                {
                    response.systemParameterList.Add(new SystemParameterModel(systemParameter));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllSystemParameterResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetCompanyConfigResponse GetCompanyConfig(GetCompanyConfigRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Company Config");
                var parameter = request.ToParameter();
                var result = iCompanyDataAccess.GetCompanyConfig(parameter);
                List<BankAccountModel> lst = new List<BankAccountModel>();
                result.ListBankAccount.ForEach(item => {
                    lst.Add(new BankAccountModel(item));
                });
                return new GetCompanyConfigResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CompanyConfig = new CompanyConfigModel(result.CompanyConfig),
                    ListBankAccount=lst
                };
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetCompanyConfigResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

    }
}
