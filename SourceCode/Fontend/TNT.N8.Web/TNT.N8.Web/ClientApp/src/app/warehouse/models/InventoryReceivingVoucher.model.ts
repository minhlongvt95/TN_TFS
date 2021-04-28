export class InventoryReceivingVoucherModel {
  inventoryReceivingVoucherId: string;
  inventoryReceivingVoucherCode: string;
  statusId: string;
  inventoryReceivingVoucherType: number;
  warehouseId: string;
  shiperName: string;
  storekeeper: string;
  inventoryReceivingVoucherDate: Date;
  inventoryReceivingVoucherTime: string;
  licenseNumber: number;
  expectedDate: Date; //Ngày dự kiến nhập
  description: string; //Diễn giải
  note: string; //Ghi chú
  partnersId: string; //Đối tác
  createdDate: Date;
  createdById: string;
  active: boolean;

  constructor() {
    this.active = true;
  }
}
