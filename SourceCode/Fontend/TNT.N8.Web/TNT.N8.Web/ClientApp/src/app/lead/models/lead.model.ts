export class LeadModel {
  LeadId: string;
  ContactId: string;
  FullName: string;
  Name: string;
  Email: string;
  Phone: string;
  Address: string;
  StatusId: any;
  Requirement: string;
  InterestedGroupId: any;
  RequirementDetail: string;
  PaymentMethodId: string;
  PotentialId: any;
  PersonInChargeId: any;
  CompanyId: string;
  CompanyName: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Status: string;
  StatusCode: string;
  AvatarUrl: string;
  PersonInChargeAvatarUrl: string;
  PersonInChargeFullName: string;
  PotentialName: string;
  Active: boolean;
  Role: string;
  WaitingForApproval: boolean;
  LeadTypeId: string;
  LeadGroupId: string;
  LeadCode: string;
  CustomerId: string;
  BusinessTypeId: string;
  InvestmentFundId: string;
  ProbabilityId: string;
  ExpectedSale: number;
  GeographicalAreaId: string;
  constructor() { }
}

//model response
export class leadByIdModel {
  public leadId: string;
  public leadCode: string;
  public requirementDetail: string;
  public potentialId: string;
  public interestedGroupId: string;
  public personInChargeId: string;
  public companyId: string;
  public companyName: string;
  public statusId: string;
  public paymentMethodId: string;
  public createdById: string;
  public createdDate: Date;
  public updatedById: string;
  public updatedDate: Date;
  public active: boolean;
  public role: string;
  public fullAddress: string;
  public fullName: string;
  public personInChargeFullName: string;
  public personInChargeAvatarUrl: string;
  public email: string;
  public phone: string;
  public statusName: string;
  public statusCode: string;
  public avatarUrl: string;
  public noActivePic: boolean;
  public waitingForApproval: boolean;
  public leadTypeId: string;
  public leadGroupId: string;
  public customerId: string;
  public businessTypeId: string;
  public investmentFundId: string;
  public probabilityId: string;
  public expectedSale: number;
  public geographicalAreaId: string;
}

export class contactLeadByIdModel {
  public contactId: string;
  public objectId: string;
  public objectType: string;
  public firstName: string;
  public lastName: string
  public gender: string
  public dateOfBirth: Date;
  public phone: string;
  public workPhone: string;
  public otherPhone: string
  public email: string
  public workEmail: string
  public otherEmail: string
  public identityId: string
  public avatarUrl: string
  public address: string
  public maritalStatusId: string;
  public countryId: string;
  public provinceId: string
  public districtId: string;
  public wardId: string;
  public postCode: string
  public websiteUrl: string
  public socialUrl: string
  public createdById: string
  public createdDate: Date
  public updatedById: string;
  public updatedDate: Date;
  public active: boolean;
  public fullName: string;
  public personInCharge: string;
  public status: string;
  public note: string;
  public role: string;
  public taxCode: string;
}

export class LeadImportModel {
  public FullName: string;
  public Gender: string;
  public Phone: string;
  public Identity: string;
  public Address: string;
  public ProvinceId: string;
  public DistrictId: string;
  public WardId: string;
  public Email: string;
  public InterestedGroupId: string;
  public PotentialId: string;
  public PaymentMethodId: string;
  public CompanyName: string;
}

export class ReportLeadModel{
  public leadName: string;
  public potentialSource: string;
  public provincial: string;
  public picCode: string;
  public picName: string;
  public month: string;
  public dayCount: number;
  public winCount: number;
  public loseCount: number;
  public undefinedCount: number;
  public sumAmount: number;
  public probabilityName: string;
  public statusName: string;
  public monthTime: Date;

  constructor() {
    this.leadName = ''
    this.potentialSource = '';
    this.provincial = '';
    this.picCode = '';
    this.picName = '';
    this.month = '';
    this.dayCount = 0;
    this.winCount = 0;
    this.loseCount = 0;
    this.undefinedCount = 0;
    this.sumAmount = 0;
    this.probabilityName = '';
    this.statusName = '';
    this.monthTime = null;
  }
}

export class TimeSearchModel{
  code: string;
  year: number;
  fromDate: Date;
  toDate: Date;
}
