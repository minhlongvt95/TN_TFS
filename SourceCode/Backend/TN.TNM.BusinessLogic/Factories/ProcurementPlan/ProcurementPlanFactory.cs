using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.ProcurementPlan;
using TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan;
using TN.TNM.BusinessLogic.Messages.Responses.ProcurementPlan;
using TN.TNM.BusinessLogic.Models.ProcurementPlan;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.ProcurementPlan
{
    public class ProcurementPlanFactory : BaseFactory, IProcurementPlan
    {
        private IProcurementPlanDataAccess iProcurementPlanDataAccess;

        public ProcurementPlanFactory(IProcurementPlanDataAccess _iProcurementPlanDataAccess, ILogger<ProcurementPlanFactory> _logger)
        {
            this.iProcurementPlanDataAccess = _iProcurementPlanDataAccess;
            logger = _logger;
        }
        public CreateProcurementPlanResponse CreateProcurementPlan(CreateProcurementPlanRequest request)
        {
            try
            {
                logger.LogInformation("Create ProcurementPlan");
                var parameter = request.ToParameter();
                var result = iProcurementPlanDataAccess.CreateProcurementPlan(parameter);
                var response = new CreateProcurementPlanResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ProcurementPlanId = result.ProcurementPlanId
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateProcurementPlanResponse()
                {
                    MessageCode = CommonMessage.Customer.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
            

        }

        public EditProcurementPlanByIdResponse EditProcurementPlanById(EditProcurementPlanByIdRequest request)
        {
                try
                {
                    logger.LogInformation("Edit Employee by Id");
                    var parameter = request.ToParameter();
                    var result = iProcurementPlanDataAccess.EditProcurementPlanById(parameter);
                    var response = new EditProcurementPlanByIdResponse();
                    if (!result.Status)
                    {
                        response.StatusCode = HttpStatusCode.ExpectationFailed;
                        response.MessageCode = CommonMessage.Employee.EDIT_FAIL;
                    }
                    else
                    {
                        response.StatusCode = HttpStatusCode.OK;
                        response.MessageCode = result.Message;
                    }

                    return response;
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return new EditProcurementPlanByIdResponse
                    {
                        MessageCode = CommonMessage.Employee.EDIT_FAIL,
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
            
        }

        public GetAllProcurementPlanResponse GetAllProcurementPlan(GetAllProcurementPlanRequest request)
        {
          
            try
            {
                logger.LogInformation("Get ProcurementPlan");
                var parameter = request.ToParameter();
                var result = iProcurementPlanDataAccess.GetAllProcurementPlan(parameter);
                var response = new GetAllProcurementPlanResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ProcurementPlanList = new List<ProcurementPlanModel>(),

                };
                result.ProcurementPlanList.ForEach(procurementPlanEntity =>
                {
                    response.ProcurementPlanList.Add(new ProcurementPlanModel(procurementPlanEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllProcurementPlanResponse()
                {
                    MessageCode = CommonMessage.Employee.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
            
        }

        public GetProcurementPlanByIdRespone GetProcurementPlanById(GetProcurementPlanByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get ProcurementPlan by Id");
                var parameter = request.ToParameter();
                var result = iProcurementPlanDataAccess.GetProcurementPlanById(parameter);
                var response = new GetProcurementPlanByIdRespone()
                {
                    StatusCode = HttpStatusCode.OK,
                    ProcurementPlan = new ProcurementPlanModel(result.ProcurementPlan)

                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetProcurementPlanByIdRespone
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchProcurementPlanResponse SearchProcurementPlan(SearchProcurementPlanRequest request)
        {
            try
            {
                logger.LogInformation("Get ProcurementPlan");
                var parameter = request.ToParameter();
                var result = iProcurementPlanDataAccess.SearchProcurementPlan(parameter);
                var response = new SearchProcurementPlanResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ProcurementPlanList = new List<ProcurementPlanModel>(),

                };
                result.ProcurementPlanList.ForEach(procurementPlanEntity =>
                {
                    response.ProcurementPlanList.Add(new ProcurementPlanModel(procurementPlanEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchProcurementPlanResponse()
                {
                    MessageCode = CommonMessage.Employee.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

    }
    
}

