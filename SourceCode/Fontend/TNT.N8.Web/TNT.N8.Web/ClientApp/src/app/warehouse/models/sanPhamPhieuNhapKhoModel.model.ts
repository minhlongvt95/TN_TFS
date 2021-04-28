export class SanPhamPhieuNhapKhoModel {
  inventoryReceivingVoucherMappingId: string;
  inventoryReceivingVoucherId: string;
  objectId: string;
  objectDetailId: string;
  productId: string;
  quantityRequest: number;
  quantityReservation: number;
  quantityActual: string;
  priceAverage: boolean;
  priceProduct: string; //Giá nhập
  warehouseId: string;

  orderCode: string;
  productCode: string;
  description: string;
  unitName: string;
  warehouseName: string;
  warehouseCodeName: string;
  amount: number; //Thành tiền = Giá nhập * Số lượng
  index: number;

  /*Các thuộc tính xử lý riêng ở frontend*/
  error: boolean;
  /*End*/

  constructor() {
    this.error = false;
    this.priceAverage = false;
  }
  
}