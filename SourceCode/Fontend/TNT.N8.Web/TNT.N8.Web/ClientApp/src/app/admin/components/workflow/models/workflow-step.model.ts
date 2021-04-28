export class WorkflowStepModel{
	WorkflowStepId: string;
	StepNumber: number;
	NextStepNumber: number;
	ApprovebyPosition: boolean;
	ApproverPositionId: string;
	ApproverId: string;
	WorkflowId: string;
	BackStepNumber: number;
	ApprovedText: string;
	NotApprovedText: string;
	RecordStatus: string;
}