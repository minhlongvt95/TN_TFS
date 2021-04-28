using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Lead;
using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;

namespace TN.TNM.Api.Controllers
{
    public class LeadDashboardController : Controller
    {
        public ILeadDashboard ILeadDashboard;
        public LeadDashboardController(ILeadDashboard iLeadDashboard) {
            this.ILeadDashboard = iLeadDashboard;
        }

        /// <summary>
        /// Get top Lead
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/lead/dashboard/gettoplead")]
        [Authorize(Policy = "Member")]
        public GetTopLeadResponse GetTopLead([FromBody]GetTopLeadRequest request)
        {
            return this.ILeadDashboard.GetTopLead(request);
        }

        /// <summary>
        /// Get convert rate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/lead/dashboard/getconvertrate")]
        [Authorize(Policy = "Member")]
        public GetConvertRateResponse GetConvertRate([FromBody]GetConvertRateRequest request)
        {
            return this.ILeadDashboard.GetConvertRate(request);
        }
        /// <summary>
        /// Get requirement rate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/lead/dashboard/getrequirementrate")]
        [Authorize(Policy = "Member")]
        public GetRequirementRateResponse GetRequirementRate([FromBody]GetRequirementRateRequest request)
        {
            return this.ILeadDashboard.GetRequirementRate(request);
        }
        /// <summary>
        /// Get potential rate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/lead/dashboard/getpotentialrate")]
        [Authorize(Policy = "Member")]
        public GetPotentialRateResponse GetPotentialRate([FromBody]GetPotentialRateRequest request)
        {
            return this.ILeadDashboard.GetPotentialRate(request);
        }

        [Route("/api/lead/dashboard/getDataLeadDashboard")]
        [Authorize(Policy = "Member")]
        public GetDataLeadDashboardResponse GetDataLeadDashboard([FromBody]GetDataLeadDashboardRequest request)
        {
            return this.ILeadDashboard.GetDataLeadDashboard(request);
        }
    }
}