using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Admin.Ward;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Ward;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Ward;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.Ward
{
    public class WardFactory : BaseFactory, IWard
    {
        public IWardDataAccess iWardDataAccess;
        public WardFactory(IWardDataAccess _iWardDataAccess, ILogger _logger)
        {
            this.iWardDataAccess = _iWardDataAccess;
            this.logger = _logger;
        }

        public GetAllWardByDistrictIdResponse GetAllWardByDistrictId(GetAllWardByDistrictIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Ward");
                var parameter = request.ToParameter();
                var result = iWardDataAccess.GetAllWardByDistrictId(parameter);
                var response = new GetAllWardByDistrictIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListWard = new List<WardModel>()
                };
                result.ListWard.ForEach(wardEntity =>
                {
                    response.ListWard.Add(new WardModel(wardEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllWardByDistrictIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
