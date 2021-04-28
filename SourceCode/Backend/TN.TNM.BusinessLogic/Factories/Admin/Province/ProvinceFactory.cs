using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Province;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Province;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Province;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Province
{
    public class ProvinceFactory : BaseFactory, IProvince
    {
        private IProvinceDataAccess iProvinceDataAccess;

        public ProvinceFactory(IProvinceDataAccess _iProvinceDataAccess, ILogger<ProvinceFactory> _logger)
        {
            this.iProvinceDataAccess = _iProvinceDataAccess;
            this.logger = _logger;
        }

        public GetAllProvinceResponse GetAllProvince(GetAllProvinceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Organization");
                var parameter = request.ToParameter();
                var result = iProvinceDataAccess.GetAllProvince(parameter);
                var response = new GetAllProvinceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListProvince = new List<ProvinceModel>()
                };
                result.ListProvince.ForEach(provinceEntity =>
                {
                    response.ListProvince.Add(new ProvinceModel(provinceEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllProvinceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
