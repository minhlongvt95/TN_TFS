using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.DashboardRequest;
using TN.TNM.BusinessLogic.Messages.Requests.DashboardRequest;
using TN.TNM.BusinessLogic.Messages.Responses.DashboardRequest;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Results.DashboardRequest;

namespace TN.TNM.BusinessLogic.Factories.DashboardRequest
{
    public class DashboardRequestFactory : BaseFactory, IDashboardRequest
    {
        private IDashboardRequestDataAccess iDashboardRequestDataAccess;

        public DashboardRequestFactory(IDashboardRequestDataAccess _iDashboardRequestDataAccess, ILogger<DashboardRequestFactory> _logger)
        {
            iDashboardRequestDataAccess = _iDashboardRequestDataAccess;
            logger = _logger;
        }
        public GetAllRequestResponse GetAllRequest(GetAllRequestRequest request)
        {
            try
            {
                logger.LogInformation("Get All Dashboard Request");
                var parameter = request.ToParameter();
                var result = iDashboardRequestDataAccess.GetAllRequest(parameter);
                var response = new GetAllRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    RequestList = new List<RequestDetail>()
                };
                result.RequestList.ForEach(RequestDetail =>
                {
                    response.RequestList.Add(RequestDetail);
                });

                
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllRequestResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchAllRequestResponse SearchAllRequest(SearchAllRequestRequest request)
        {
            try
            {
                logger.LogInformation("Search All Dashboard Request");
                var parameter = request.ToParameter();
                var result = iDashboardRequestDataAccess.SearchAllRequest(parameter);
                var response = new SearchAllRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    RequestList = new List<RequestDetail>()
                };
                result.RequestList.ForEach(RequestDetail =>
                {
                    response.RequestList.Add(RequestDetail);
                });


                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchAllRequestResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }

        }
    }
}
