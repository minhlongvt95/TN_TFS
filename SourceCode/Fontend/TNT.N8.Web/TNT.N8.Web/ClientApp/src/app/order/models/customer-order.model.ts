import { Timestamp } from "rxjs";
import { CustomerOrderDetail} from "./customer-order-detail.model";
var emptyGuid: string = '00000000-0000-0000-0000-000000000000';
export class CustomerOrder {
  OrderId: string;
  OrderCode: string;
  OrderDate: Date;
  Seller: string;
  Description: string;
  Note: string;
  CustomerId: string;
  CustomerContactId: string;
  PaymentMethod: string;
  DiscountType: boolean;
  BankAccountId: string;
  DaysAreOwed: number;
  MaxDebt: number;
  ReceivedDate: Date;
  ReceivedHour: Date;
  RecipientName : string;
  LocationOfShipment: string;
  ShippingNote: string;
  RecipientPhone : string;
  RecipientEmail: string;
  PlaceOfDelivery: string;
  Amount: number;
  DiscountValue: number;
  ReceiptInvoiceAmount: number;
  StatusId: string;//Guid?
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  QuoteId: string;
  ReasonCancel: string;
  IsAutoGenReceiveInfor: boolean;
  CustomerName: string;
  CustomerAddress: string;
  OrderContractId: string;
  WarehouseId: string;

  constructor() {
    this.OrderId = emptyGuid,
    this.OrderCode = null,
    this.OrderDate = null,
    this.Seller = null,
    this.Description = null,
    this.Note = null,
    this.CustomerId = null,
    this.CustomerContactId = null,
    this.PaymentMethod = null,
    this.DiscountType = true,
    this.BankAccountId = null,
    this.DaysAreOwed = null,
    this.MaxDebt = null,
    this.ReceivedDate = null,
    this.ReceivedHour = null,
    this.RecipientName = null,
    this.LocationOfShipment = null,
    this.ShippingNote = null,
    this.RecipientPhone = null,
    this.RecipientEmail = null,
    this.PlaceOfDelivery = null,
    this.Amount = 0,
    this.DiscountValue = 0,
    this.ReceiptInvoiceAmount = null,
    this.StatusId = null,
    this.Active = true,
    this.CreatedById = emptyGuid,
    this.CreatedDate = new Date(),
    this.UpdatedById = null,
    this.UpdatedDate = null,
    this.QuoteId = null,
    this.ReasonCancel = null,    
    this.CustomerName = null;
    this.CustomerAddress = null;
    this.OrderContractId = null;
    this.WarehouseId = null;
  }
}
