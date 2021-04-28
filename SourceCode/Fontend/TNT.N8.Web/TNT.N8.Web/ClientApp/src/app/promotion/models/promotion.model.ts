export class Promotion {
  promotionId: string;
  promotionCode: string;
  promotionName: string;
  effectiveDate: Date;
  expirationDate: Date;
  description: string;
  conditionsType: number;
  propertyType: number;
  filterContent: string;
  filterQuery: string;
  notMultiplition: boolean;
  note: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  customerHasOrder: boolean;

  constructor() {
    this.active = true;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.customerHasOrder = false;
  }
}