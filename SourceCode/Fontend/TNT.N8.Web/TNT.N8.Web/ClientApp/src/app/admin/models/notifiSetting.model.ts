export class NotifiSetting {
  notifiSettingId: string;
  notifiSettingName: string;
  screenId: string;
  notifiActionId: string;
  isApproved: boolean;
  isParticipant: boolean;
  isCreated: boolean;
  isPersonIncharge: boolean;
  sendInternal: boolean;
  objectBackHourInternal: string;
  backHourInternal: number;
  isSystem: boolean;
  systemTitle: string;
  systemContent: string;
  isEmail: boolean;
  emailTitle: string;
  emailContent: string;
  isSms: boolean;
  smsTitle: string;
  smsContent: string;
  sendCustomer: boolean;
  objectBackHourCustomer: string;
  backHourCustomer: number;
  isCustomerEmail: boolean;
  customerEmailTitle: string;
  customerEmailContent: string;
  isCustomerSms: boolean;
  customerSmsTitle: string;
  customerSmsContent: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  objectBackHourInternalName: string;

  constructor() {
    this.notifiSettingId = '00000000-0000-0000-0000-000000000000';
    this.isEmail = false;
    this.isSystem = false;
    this.isSms = false;
    this.sendCustomer = false;
    this.isCustomerEmail = false;
    this.isCustomerSms = false;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
  }
}