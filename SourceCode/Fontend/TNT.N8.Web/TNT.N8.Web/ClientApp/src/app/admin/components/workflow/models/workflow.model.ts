export class WorkflowModel{
	WorkFlowId: string;
	Name: string;
	Description: string;
	SystemFeatureId: string;
	WorkflowCode: string;
	StatusId: string;
	CreatedDate: Date;
	CreatedBy: string;
	UpdatedDate: Date;
	UpdatedBy: string;
	WorkflowSteps: Array<any>;
}