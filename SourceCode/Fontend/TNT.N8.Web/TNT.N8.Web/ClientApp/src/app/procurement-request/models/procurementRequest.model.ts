export class ProcurementRequestModel{
    procurementRequestId: string;
    procurementCode: string;
    procurementContent: string;
    requestEmployeeId: string;
    employeePhone: string;
    unit: string;
    approverId: string;
    approverPostion: string;
    explain: string;
    statusId: string;
    createdById: string;
    createdDate: Date;
    updatedById: string;
    updatedDate: Date;
    requestEmployeeName: string;
    approverName: string;
    statusName: string;
    organizationName: string;
    totalMoney: Number;
    orderId: string;
    active: boolean;
}