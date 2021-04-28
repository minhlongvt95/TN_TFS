export class OrganizationModel {
  OrganizationId: string;
  OrganizationName: string;
  OrganizationCode: string;
  Address: string;
  Phone: string;
  ParentId: string;
  ParentName: string;
  Level: Number;
  IsFinancialIndependence: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  constructor() { }
}
