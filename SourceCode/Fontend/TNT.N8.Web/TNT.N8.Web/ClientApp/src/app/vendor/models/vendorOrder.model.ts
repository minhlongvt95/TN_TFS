export class VendorOrderModel {
  vendorOrderId: string;
  vendorOrderCode : string;
  vendorOrderDate: Date;
  customerOrderId: string;
  orderer : string;
  description : string;
  note : string;
  vendorId: string;
  vendorContactId: string;
  paymentMethod : string;
  bankAccountId : string;
  receivedDate: Date;
  receivedHour: Date;
  recipientName : string;
  locationOfShipment : string;
  shippingNote : string;
  recipientPhone : string;
  recipientEmail : string;
  placeOfDelivery : string;
  amount: number;
  discountValue : number;
  statusId: string;
  createdById : string;
  createdDate : Date;
  updatedById : string;
  updatedDate: Date;
  active : boolean;
  discountType: boolean;
  createdByName: string;
  avatarUrl: string;
  statusName: string;
  contractId: string;
  warehouseId: string;
  shipperName: string;
  typeCost: string;

  constructor() {
    this.vendorOrderId = null,
    this.vendorOrderCode = null,
    this.vendorOrderDate = null,
    this.customerOrderId = null,
    this.orderer = null,
    this.description = null,
    this.note = null,
    this.vendorId = null,
    this.vendorContactId = null,
    this.paymentMethod = null,
    this.bankAccountId = null,
    this.receivedDate = null,
    this.receivedHour = null,
    this.locationOfShipment = null,
    this.shippingNote = null,
    this.recipientPhone = null,
    this.recipientEmail = null,
    this.placeOfDelivery = null,
    this.amount = null,
    this.discountValue = null,
    this.statusId = null,
    this.createdById = null,
    this.createdDate = null,
    this.updatedById = null,
    this.updatedDate = null,
    this.active = null,
    this.discountType = null,
    this.createdByName = null,
    this.avatarUrl = null,
    this.statusName = null,
    this.contractId = null,
    this.warehouseId = null,
    this.shipperName = null
  }
}
