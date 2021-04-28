using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Workflow
{
    public class WorkflowEntityModel
    {
        public Guid WorkFlowId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SystemFeatureId { get; set; }
        public string WorkflowCode { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<WorkflowStepEntityModel> WorkflowStepList { get; set; }
        public string SystemFeatureName { get; set; }
        public string StatusName { get; set; }
    }
}
