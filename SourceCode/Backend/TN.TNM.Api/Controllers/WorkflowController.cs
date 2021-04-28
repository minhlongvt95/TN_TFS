using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Workflow;
using TN.TNM.BusinessLogic.Messages.Requests.Workflow;
using TN.TNM.BusinessLogic.Messages.Responses.Workflow;

namespace TN.TNM.Api.Controllers
{
    public class WorkflowController : Controller
    {
        private readonly IWorkflow _iWorkflow;
        public WorkflowController(IWorkflow iWorkflow)
        {
            this._iWorkflow = iWorkflow;
        }
        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/create")]
        [Authorize(Policy = "Member")]
        public CreateWorkflowResponse CreateWorkflow([FromBody]CreateWorkflowRequest request)
        {
            return this._iWorkflow.CreateWorkflow(request);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/getAllWorkflowCode")]
        [Authorize(Policy = "Member")]
        public GetAllWorkflowCodeResponse GetAllWorkflowCode([FromBody]GetAllWorkflowCodeRequest request)
        {
            return this._iWorkflow.GetAllWorkflowCode(request);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/getAllSystemFeature")]
        [Authorize(Policy = "Member")]
        public GetAllSystemFeatureResponse GetAllSystemFeature([FromBody]GetAllSystemFeatureRequest request)
        {
            return this._iWorkflow.GetAllSystemFeature(request);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/searchWorkflow")]
        [Authorize(Policy = "Member")]
        public SearchWorkflowResponse SearchWorkflow([FromBody]SearchWorkflowRequest request)
        {
            return this._iWorkflow.SearchWorkflow(request);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/getWorkflowById")]
        [Authorize(Policy = "Member")]
        public GetWorkflowByIdResponse GetWorkflowById([FromBody]GetWorkflowByIdRequest request)
        {
            return this._iWorkflow.GetWorkflowById(request);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/updateWorkflowById")]
        [Authorize(Policy = "Member")]
        public UpdateWorkflowByIdResponse UpdateWorkflowById([FromBody]UpdateWorkflowByIdRequest request)
        {
            return this._iWorkflow.UpdateWorkflowById(request);
        }

        /// <summary>
        /// Next step of workflow
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/workflow/nextWorkflowStep")]
        [Authorize(Policy = "Member")]
        public NextWorkflowStepResponse NextWorkflowStep([FromBody]NextWorkflowStepRequest request)
        {
            return this._iWorkflow.NextWorkflowStep(request);
        }

        //
        [HttpPost]
        [Route("api/workflow/getMasterDataCreateWorkflow")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateWorkflowResponse GetMasterDataCreateWorkflow([FromBody]GetMasterDataCreateWorkflowRequest request)
        {
            return this._iWorkflow.GetMasterDataCreateWorkflow(request);
        }
    }
}