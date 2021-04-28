using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Requests.Workflow
{
    public class SearchWorkflowRequest : BaseRequest<SearchWorkflowParameter>
    {
        public string WorkflowName { get; set; }
        public List<Guid> SystemFeatureIdList { get; set; }
        public override SearchWorkflowParameter ToParameter()
        {
            return new SearchWorkflowParameter() {
                UserId = UserId,
                WorkflowName = WorkflowName,
                SystemFeatureIdList = SystemFeatureIdList
            };
        }
    }
}
