using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.District;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.District;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.District;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.District
{
    public class DistrictFactory : BaseFactory, IDistrict
    {
        private IDistrictDataAccess iDistrictDataAccess;

        public DistrictFactory(IDistrictDataAccess _iDistrictDataAccess, ILogger _logger)
        {
            this.iDistrictDataAccess = _iDistrictDataAccess;
            this.logger = _logger;
        }

        public GetAllDistrictByProvinceIdResponse GetAllDistrictByProvinceId(GetAllDistrictByProvinceIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All District");
                var parameter = request.ToParameter();
                var result = iDistrictDataAccess.GetAllDistrictByProvinceId(parameter);
                var response = new GetAllDistrictByProvinceIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListDistrict = new List<DistrictModel>()
                };
                result.ListDistrict.ForEach(districtEntity =>
                {
                    response.ListDistrict.Add(new DistrictModel(districtEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllDistrictByProvinceIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }
    }
}
