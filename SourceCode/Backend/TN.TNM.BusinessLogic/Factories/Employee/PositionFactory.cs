using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Position;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Position;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Employee
{
    public class PositionFactory : BaseFactory, IPosition
    {
        private IPositionDataAccess iPositionDataAccess;
        public PositionFactory(IPositionDataAccess _iPositionDataAccess, ILogger<EmployeeFactory> _logger)
        {
            iPositionDataAccess = _iPositionDataAccess;
            logger = _logger;
        }

        public GetAllPositionResponse GetAllPosition(GetAllPositionRequest request)
        {
            try
            {
                logger.LogInformation("Get All Position");
                var parameter = request.ToParameter();
                var result = iPositionDataAccess.GetAllPosition(parameter);
                var response = new GetAllPositionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListPosition = new List<PositionModel>()
                };
                result.ListPosition.ForEach(positionEntity =>
                {
                    response.ListPosition.Add(new PositionModel(positionEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllPositionResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
