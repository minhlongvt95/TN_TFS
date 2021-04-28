using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.CustomerServiceLevel
{
    public class CustomerServiceLevelFactory : BaseFactory, ICustomerServiceLevel
    {
        private ICustomerServiceLevelDataAccess iCustomerServiceLevelDataAccess;
        public CustomerServiceLevelFactory(ICustomerServiceLevelDataAccess _iCustomerServiceLevelDataAccess, ILogger<CustomerServiceLevelFactory> _logger)
        {
            this.iCustomerServiceLevelDataAccess = _iCustomerServiceLevelDataAccess;
            this.logger = _logger;
        }

        public AddLevelCustomerResponse AddLevelCustomer(AddLevelCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerServiceLevelDataAccess.AddLevelCustomer(parameter);
                return new AddLevelCustomerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Thêm mới thành công!"
                };
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new AddLevelCustomerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetConfigCustomerServiceLevelResponse GetConfigCustomerServiceLevel(GetConfigCustomerServiceLevelRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerServiceLevelDataAccess.GetConfigCustomerServiceLevel(parameter);
                var response = new GetConfigCustomerServiceLevelResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerServiceLevel = new List<CustomerServiceLevelModel>()
                };
                result.CustomerServiceLevel.ForEach(customer => {
                    response.CustomerServiceLevel.Add(new CustomerServiceLevelModel(customer));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetConfigCustomerServiceLevelResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public UpdateConfigCustomerResponse UpdateConfigCustomer(UpdateConfigCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerServiceLevelDataAccess.UpdateConfigCustomer(parameter);
                return new UpdateConfigCustomerResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công!"
                };
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new UpdateConfigCustomerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "Xóa thất bại!"
                };
            }
        }
    }
}
