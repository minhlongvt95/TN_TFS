using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Lead;
using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Lead
{
    public class LeadDashboardFactory : BaseFactory, ILeadDashboard
    {
        private ILeadDashboardDataAccess ILeadDashboardDataAccess;
        public LeadDashboardFactory(ILeadDashboardDataAccess iLeadDashboardDataAccess, ILogger<LeadDashboardFactory> _logger)
        {
            this.ILeadDashboardDataAccess = iLeadDashboardDataAccess;
            this.logger = _logger;
        }

        public GetConvertRateResponse GetConvertRate(GetConvertRateRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = this.ILeadDashboardDataAccess.GetConvertRate(parameter);
                if(!result.Status)
                {
                    this.logger.LogError(result.Message);
                }
                var response = new GetConvertRateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    TotalCount = result.TotalCount,
                    TotalCountPotentialRate = result.TotalCountPotentialRate,
                    TotalCountRequirementRate = result.TotalCountRequirementRate,
                    LeadPotentialRateList = new List<LeadPotentialRateModel>(),
                    LeadRequirementRateList = new List<LeadRequirementRateModel>(),
                    LeadConvertRateList = new List<LeadConvertRateModel>(),
                    ListCHOLead=new List<LeadModel>(),
                    ListMOILead=new List<LeadModel>(),
                    ListNDOLead=new List<LeadModel>()
                };
                result.LeadPotentialRateList.ForEach(item => {
                    response.LeadPotentialRateList.Add(new LeadPotentialRateModel(item));
                });
                result.LeadRequirementRateList.ForEach(item => {
                    response.LeadRequirementRateList.Add(new LeadRequirementRateModel(item));
                });
                result.LeadConvertRateList.ForEach(item=> {
                    response.LeadConvertRateList.Add(new LeadConvertRateModel(item));
                });
                result.ListCHOLead.ForEach(item => {
                    response.ListCHOLead.Add(new LeadModel(item));
                });
                result.ListMOILead.ForEach(item => {
                    response.ListMOILead.Add(new LeadModel(item));
                });
                result.ListNDOLead.ForEach(item => {
                    response.ListNDOLead.Add(new LeadModel(item));
                });
                return response;
            }
            catch(Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetConvertRateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetPotentialRateResponse GetPotentialRate(GetPotentialRateRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = this.ILeadDashboardDataAccess.GetPotentialRate(parameter);
                if (!result.Status)
                {
                    this.logger.LogError(result.Message);
                }
                var response = new GetPotentialRateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    TotalCount=result.TotalCount,
                    LeadPotentialRateList = new List<LeadPotentialRateModel>()
                };
                result.LeadPotentialRateList.ForEach(item=> {
                    response.LeadPotentialRateList.Add(new LeadPotentialRateModel(item));
                });
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetPotentialRateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetRequirementRateResponse GetRequirementRate(GetRequirementRateRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = this.ILeadDashboardDataAccess.GetRequirementRate(parameter);
                if (!result.Status)
                {
                    this.logger.LogError(result.Message);
                }
                var response = new GetRequirementRateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    TotalCount = result.TotalCount,
                    LeadRequirementRateList = new List<LeadRequirementRateModel>()
                };
                result.LeadRequirementRateList.ForEach(item=> {
                    response.LeadRequirementRateList.Add(new LeadRequirementRateModel(item));
                });
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetRequirementRateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetTopLeadResponse GetTopLead(GetTopLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = this.ILeadDashboardDataAccess.GetTopLead(parameter);
                if (!result.Status)
                {
                    this.logger.LogError(result.Message);
                }
                var response = new GetTopLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListLead = new List<LeadModel>()
                };
                result.ListLead.ForEach(leadEntityModel =>
                {
                    response.ListLead.Add(new LeadModel(leadEntityModel));
                });
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetTopLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetDataLeadDashboardResponse GetDataLeadDashboard(GetDataLeadDashboardRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = this.ILeadDashboardDataAccess.GetDataLeadDashboard(parameter);
              
                var response = new GetDataLeadDashboardResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    LeadDashBoard = result.LeadDashBoard
                };
             
                return response;
            }
            catch (Exception ex)
            {
                return new GetDataLeadDashboardResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message.ToString()
                };
            }
        }
    }
}
