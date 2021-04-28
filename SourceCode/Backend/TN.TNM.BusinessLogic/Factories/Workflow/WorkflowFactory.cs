using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Workflow;
using TN.TNM.BusinessLogic.Messages.Requests.Workflow;
using TN.TNM.BusinessLogic.Messages.Responses.Workflow;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Workflow;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Workflow
{
    public class WorkflowFactory : BaseFactory, IWorkflow
    {
        private IWorkflowDataAccess iWorkflowDataAccess;
        public WorkflowFactory(IWorkflowDataAccess _iWorkflowDataAccess, ILogger<WorkflowFactory> _logger)
        {
            this.iWorkflowDataAccess = _iWorkflowDataAccess;
            this.logger = _logger;
        }
        public CreateWorkflowResponse CreateWorkflow(CreateWorkflowRequest request)
        {
            try
            {
                this.logger.LogInformation("Create new Workflow");
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.CreateWorkflow(parameter);
                return new CreateWorkflowResponse() {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateWorkflowResponse() {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Workflow.CREATE_FAIL
                };
            }
        }

        public SearchWorkflowResponse SearchWorkflow(SearchWorkflowRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Workflow");
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.SearchWorkflow(parameter);
                var response = new SearchWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    WorkflowList = new List<WorkflowModel>()
                };
                result.WorkflowList.ForEach(item => {
                    response.WorkflowList.Add(new WorkflowModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SearchWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Workflow.SEARCH_FAIL
                };
            }
        }

        public GetWorkflowByIdResponse GetWorkflowById(GetWorkflowByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Workflow by Id");
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.GetWorkflowById(parameter);
                var response = new GetWorkflowByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Workflow = new WorkflowModel(result.Workflow)
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetWorkflowByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Workflow.GET_FAIL
                };
            }
        }

        public UpdateWorkflowByIdResponse UpdateWorkflowById(UpdateWorkflowByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Update new Workflow");
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.UpdateWorkflowById(parameter);
                return new UpdateWorkflowByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new UpdateWorkflowByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Workflow.EDIT_FAIL
                };
            }
        }

        public GetAllWorkflowCodeResponse GetAllWorkflowCode(GetAllWorkflowCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Workflow code");
                return new GetAllWorkflowCodeResponse() { };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllWorkflowCodeResponse() { };
            }
        }

        public GetAllSystemFeatureResponse GetAllSystemFeature(GetAllSystemFeatureRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all System Feature");
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.GetAllSystemFeature(parameter);
                var lst = new List<SystemFeatureModel>();
                result.SystemFeatureList.ForEach(sf => {
                    lst.Add(new SystemFeatureModel(sf));
                });

                return new GetAllSystemFeatureResponse() {
                    SystemFeatureList = lst,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllSystemFeatureResponse() {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public NextWorkflowStepResponse NextWorkflowStep(NextWorkflowStepRequest request)
        {
            try
            {
                this.logger.LogInformation("Next workflow step");
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.NextWorkflowStep(parameter);
                var lst = new List<SystemFeatureModel>();

                return new NextWorkflowStepResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                return new NextWorkflowStepResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Workflow.CHANGE_FAIL
                };
            }
        }

        public GetMasterDataCreateWorkflowResponse GetMasterDataCreateWorkflow(GetMasterDataCreateWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWorkflowDataAccess.GetMasterDataCreateWorkflow(parameter);

                var response = new GetMasterDataCreateWorkflowResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee,
                    ListStatus = result.ListStatus,
                    ListPosition = new List<PositionModel>(),
                    ListSystemFeature = new List<SystemFeatureModel>()
                };

                result.ListPosition.ForEach(item =>
                {
                    response.ListPosition.Add(new PositionModel(item));
                });

                result.ListSystemFeature.ForEach(item =>
                {
                    response.ListSystemFeature.Add(new SystemFeatureModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Workflow.CHANGE_FAIL
                };
            }
        }
    }
}
