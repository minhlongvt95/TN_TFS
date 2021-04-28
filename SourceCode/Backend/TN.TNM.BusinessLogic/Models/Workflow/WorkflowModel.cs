using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Workflow;

namespace TN.TNM.BusinessLogic.Models.Workflow
{
    public class WorkflowModel : BaseModel<WorkFlows>
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
        public List<WorkflowStepModel> WorkflowStepList { get; set; }
        public string SystemFeatureName { get; set; }
        public string StatusName { get; set; }
        public WorkflowModel() { }

        public WorkflowModel(WorkflowEntityModel model)
        {
            Mapper(model, this);
            if(model.WorkflowStepList != null)
            {
                var lst = new List<WorkflowStepModel>();
                model.WorkflowStepList.ForEach(step => {
                    lst.Add(new WorkflowStepModel(step));
                });

                this.WorkflowStepList = lst;
            }
        }

        public WorkflowModel(WorkFlows entity): base(entity)
        {

        }

        public override WorkFlows ToEntity()
        {
            var entity = new WorkFlows();
            Mapper(this, entity);
            return entity;
        }
        public WorkflowEntityModel ToEntityModel()
        {
            var entityModel = new WorkflowEntityModel();
            Mapper(this, entityModel);
            if (this.WorkflowStepList != null)
            {
                var lst = new List<WorkflowStepEntityModel>();
                this.WorkflowStepList.ForEach(step => {
                    lst.Add(step.ToEntityModel());
                });

                entityModel.WorkflowStepList = lst;
            }
            return entityModel;
        }
    }
}
