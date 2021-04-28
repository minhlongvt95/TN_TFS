export class CustomerCareModel {
  CustomerCareId: string;
  CustomerCareCode: string;
  EmployeeCharge: string;
  EffecttiveFromDate: Date;
  EffecttiveToDate: Date;
  CustomerCareTitle: string;
  CustomerCareContent: string;
  CustomerCareContactType: string;
  CustomerCareContentSMS: string;
  ExpectedAmount: number;
  IsSendNow: boolean;
  IsEvent: boolean;
  SendDate: Date;
  SendHour: Date;
  CustomerCareEvent: string;
  CustomerCareEventHour: Date;
  CustomerCareContentEmail: string;
  IsSendEmailNow: boolean;
  SendEmailDate: Date;
  SendEmailHour: Date;
  CustomerCareVoucher: string;
  DiscountAmount: number;
  PercentDiscountAmount: number;
  GiftCustomerType1: number;
  GiftTypeId1: string;
  GiftTotal1: number;
  GiftCustomerType2: number;
  GiftTypeId2: string;
  GiftTotal2: number;
  CustomerCareType: number;
  StatusId: string;
  StatusCode: string;
  CreateDate: Date;
  CreateById: string;
  UpdateDate: Date;
  UpdateById: string;
  CustomerCareContactTypeName: string;
  StatusName: string;
  EmployeeChargeName: string;
  IsEditStatus: boolean;
  TypeCustomer: number;

  constructor() {
    this.CustomerCareId = '00000000-0000-0000-0000-000000000000';
    this.CustomerCareCode = '';
    this.EmployeeCharge = null;
    this.EffecttiveFromDate = new Date();
    this.EffecttiveToDate = new Date(d.setDate(d.getDate() + 7));
    this.CustomerCareTitle = null;
    this.CustomerCareContent = null;
    this.CustomerCareContactType = null;
    this.CustomerCareContentSMS = "";
    this.ExpectedAmount = 0;
    this.IsSendNow = null;
    this.IsEvent = null;
    this.SendDate = null;
    this.SendHour = null;
    this.CustomerCareEvent = null;
    this.CustomerCareEventHour = null;
    this.CustomerCareContentEmail = "";
    this.IsSendEmailNow = true;
    this.SendEmailDate = null;
    this.SendEmailHour = null;
    this.CustomerCareVoucher = null;
    this.DiscountAmount = null;
    this.PercentDiscountAmount = null;
    this.GiftCustomerType1 = null;
    this.GiftTypeId1 = null;
    this.GiftTotal1 = null;
    this.GiftCustomerType2 = null;
    this.GiftTypeId2 = null;
    this.GiftTotal2 = null;
    this.CustomerCareType = null;
    this.StatusId = null;
    this.CreateDate = new Date();
    this.CreateById = null;
    this.UpdateDate = new Date();
    this.UpdateById = null;
    this.CustomerCareContactTypeName = null;
    this.StatusName = null;
    this.EmployeeChargeName = null;
    this.IsEditStatus = null;
    this.StatusCode = null;
    this.TypeCustomer = 1;
  }
}

let d = new Date();

export class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  /**
   *
   */
  constructor() {
    this.categoryId = '00000000-0000-0000-0000-000000000000';
    this.categoryCode = '';
    this.categoryName = '';
  }
}

export class CustomerCareModel2{
  index: number;
  customerCareId: string;
  customerCareCode: string;
  employeeCharge: string;
  effecttiveFromDate: Date;
  effecttiveToDate: Date;
  customerCareTitle: string;
  customerCareContent: string;
  customerCareContactType: string;
  customerCareContentSMS: string;
  expectedAmount: number;
  isSendNow: boolean;
  isEvent: boolean;
  sendDate: Date;
  sendHour: Date;
  customerCareEvent: string;
  customerCareEventHour: Date;
  customerCareContentEmail: string;
  isSendEmailNow: boolean;
  sendEmailDate: Date;
  sendEmailHour: Date;
  customerCareVoucher: string;
  discountAmount: number;
  percentDiscountAmount: number;
  giftCustomerType1: number;
  giftTypeId1: string;
  giftTotal1: number;
  giftCustomerType2: number;
  giftTypeId2: string;
  giftTotal2: number;
  customerCareType: number;
  statusId: string;
  statusCode: string;
  createDate: Date;
  createById: string;
  updateDate: Date;
  updateById: string;
  customerCareContactTypeName: string;
  statusName: string;
  employeeChargeName: string;
  isEditStatus: boolean;
  typeCustomer: number;

  status: CategoryModel;
  listStatus: Array<CategoryModel>;

  constructor() {
    this.index = 1;
    this.customerCareId = '00000000-0000-0000-0000-000000000000';
    this.customerCareCode = '';
    this.employeeCharge = null;
    this.effecttiveFromDate = new Date();
    this.effecttiveToDate = new Date(d.setDate(d.getDate() + 7));
    this.customerCareTitle = null;
    this.customerCareContent = null;
    this.customerCareContactType = null;
    this.customerCareContentSMS = "";
    this.expectedAmount = 0;
    this.isSendNow = null;
    this.isEvent = null;
    this.sendDate = null;
    this.sendHour = null;
    this.customerCareEvent = null;
    this.customerCareEventHour = null;
    this.customerCareContentEmail = "";
    this.isSendEmailNow = true;
    this.customerCareVoucher = null;
    this.discountAmount = null;
    this.percentDiscountAmount = null;
    this.giftCustomerType1 = null;
    this.giftTypeId1 = null;
    this.giftTotal1 = null;
    this.giftCustomerType2 = null;
    this.giftTypeId2 = null;
    this.giftTotal2 = null;
    this.customerCareType = null;
    this.statusId = null;
    this.createDate = new Date();
    this.createById = null;

    this.customerCareContactTypeName = null;
    this.statusName = null;
    this.employeeChargeName = null;
    this.isEditStatus = null;
    this.statusCode = null;
    this.typeCustomer = 1;
    this.status = null;
    this.listStatus = [];
  }
}
