export class InventoryReceivingVoucherMapping {
  inventoryReceivingVoucherMappingId: string;
  inventoryReceivingVoucherId: string;
  objectId: string;
  objectDetailId: string;
  productId: string;
  quantityRequest: number;
  quantityActual: number;
  quantityReservation: number;
  quantitySerial: number;
  priceProduct: number;
  priceAverage: boolean;
  warehouseId: string;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;

  constructor() {
    this.active = true;
    this.priceAverage = false;
    this.createdDate = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
  }
}