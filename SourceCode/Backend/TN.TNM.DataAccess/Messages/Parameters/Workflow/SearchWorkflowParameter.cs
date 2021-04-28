using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Workflow
{
    public class SearchWorkflowParameter : BaseParameter
    {
        public string WorkflowName { get; set; }
        public List<Guid> SystemFeatureIdList { get; set; }
    }
}
