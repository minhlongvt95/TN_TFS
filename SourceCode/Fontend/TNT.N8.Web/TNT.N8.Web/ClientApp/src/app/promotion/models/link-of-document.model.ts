export class LinkOfDocument {
  linkOfDocumentId: string;
  linkName: string;
  linkValue: string;
  objectId: string;
  objectType: string;
  active: boolean;
  createdById: string;
  createdDate: Date;

  constructor() {
    this.active = true;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
  }
}