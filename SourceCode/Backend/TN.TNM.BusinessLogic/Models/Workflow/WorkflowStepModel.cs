using System;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Workflow;

namespace TN.TNM.BusinessLogic.Models.Workflow
{
    public class WorkflowStepModel : BaseModel<WorkFlowSteps>
    {
        public Guid WorkflowStepId { get; set; }
        public int StepNumber { get; set; }
        public int? NextStepNumber { get; set; }
        public bool ApprovebyPosition { get; set; }
        public Guid? ApproverPositionId { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid WorkflowId { get; set; }
        public int? BackStepNumber { get; set; }
        public string ApprovedText { get; set; }
        public string NotApprovedText { get; set; }
        public string RecordStatus { get; set; }

        public WorkflowStepModel() { }
        public WorkflowStepModel(WorkFlowSteps entity) : base(entity) {
        }
        public WorkflowStepModel(WorkflowStepEntityModel model) {
            Mapper(model, this);
        }
        public override WorkFlowSteps ToEntity()
        {
            var entity = new WorkFlowSteps();
            Mapper(this, entity);
            return entity;
        }

        public WorkflowStepEntityModel ToEntityModel()
        {
            var entityModel = new WorkflowStepEntityModel();
            Mapper(this, entityModel);
            return entityModel;
        }
    }
}
