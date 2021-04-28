export class CustomerCareFeedBack {
  CustomerCareFeedBackId: string;
  FeedBackFromDate: Date;
  FeedBackToDate: Date;
  FeedbackType: string;
  FeedBackCode: string;
  FeedBackContent: string;
  CustomerId: string;
  CustomerCareId: string;
  CreateDate: Date;
  CreateById: string;
  UpdateDate: Date;
  UpdateById: string;

  constructor() {
    this.CustomerCareFeedBackId = '00000000-0000-0000-0000-000000000000';
    this.FeedBackFromDate = null;
    this.FeedBackToDate = null;
    this.FeedbackType = null;
    this.FeedBackCode = null;
    this.FeedBackContent = '';
    this.CustomerId = null;
    this.CustomerCareId = null;
    this.CreateDate = null;
    this.CreateById = null;
    this.UpdateDate = null;
    this.UpdateById = null;
  }
}
