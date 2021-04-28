using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.ProcurementPlan;
using TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan;
using TN.TNM.BusinessLogic.Messages.Responses.ProcurementPlan;

namespace TN.TNM.Api.Controllers
{
    
    public class ProcurementPlanController : Controller
    {
        private readonly IProcurementPlan _iProcurementPlan;
        public ProcurementPlanController(IProcurementPlan iProcurementPlan)
        {
            this._iProcurementPlan = iProcurementPlan;
        }

        [HttpPost]
        [Route("api/budget/editById")]
        [Authorize(Policy = "Member")]
        public EditProcurementPlanByIdResponse EditProcurementPlanById([FromBody]EditProcurementPlanByIdRequest request)
        {
           return  this._iProcurementPlan.EditProcurementPlanById(request);
        }

        [HttpPost]
        [Route("api/budget/create")]
        [Authorize(Policy = "Member")]
        public CreateProcurementPlanResponse CreateProcurementPlan([FromBody]CreateProcurementPlanRequest request)
        {
            return this._iProcurementPlan.CreateProcurementPlan(request);
        }

        [HttpPost]
        [Route("api/budget/getAllBudget")]
        [Authorize(Policy = "Member")]
        public GetAllProcurementPlanResponse GetAllProcurementPlan([FromBody]GetAllProcurementPlanRequest request)
        {
            return this._iProcurementPlan.GetAllProcurementPlan(request);
        }

        [HttpPost]
        [Route("api/budget/search")]
        [Authorize(Policy = "Member")]
        public SearchProcurementPlanResponse SearchEmployeeRequest([FromBody]SearchProcurementPlanRequest request)
        {
            return this._iProcurementPlan.SearchProcurementPlan(request);
        }
        [HttpPost]
        [Route("api/budget/getBudgetById")]
        [Authorize(Policy = "Member")]
        public GetProcurementPlanByIdRespone GetAllProcurementPlanById([FromBody]GetProcurementPlanByIdRequest request)
        {
            return this._iProcurementPlan.GetProcurementPlanById(request);
        }
    }
}