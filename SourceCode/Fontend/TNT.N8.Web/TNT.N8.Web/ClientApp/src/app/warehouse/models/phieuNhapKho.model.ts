export class PhieuNhapKho {
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
  active: boolean;
  createdDate: Date;
  createdById: string;
  updatedDate: Date;
  updatedById: string;
  expectedDate: Date;
  description: string;
  note: string;
  partnersId: string; //Id đối tác
  statusName: string;
  statusCode: string;
  employeeCodeName: string; //Người lập phiếu
  totalQuantityActual: number;   //Tổng số lượng thực nhập

  constructor() {
    this.active = true;
  }
  
}