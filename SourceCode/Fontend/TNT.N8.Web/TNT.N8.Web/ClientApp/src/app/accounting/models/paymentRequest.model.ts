export class PaymentRequestModel {
  RequestPaymentId: string;
  RequestPaymentCode: string;
  RequestPaymentNote: string;
  RequestPaymentCreateDate: Date;
  RequestEmployee: string;
  RequestEmployeePhone: string;
  RequestBranch: string;
  ApproverId: string;
  PostionApproverId: string;
  TotalAmount: Number;
  PaymentType: string;
  Description: string;
  NumberCode: Number;
  YearCode: Number;
  StatusID: string;
  CreateDate: Date;
  CreateById: string;
  UpdateDate: Date;
  UpdateById: string;
  RequestEmployeeName: string;
  ApprovedName: string;
  OrganizationName: string;
  PaymentTypeName: string;
  StatusName: string;
  constructor() { }

}
