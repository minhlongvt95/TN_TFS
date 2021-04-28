import { PromotionProductMapping } from './promotion-product-mapping.model';

class TypeInt {
  name: string;
  value: number;
}

class TypeBoolean {
  name: string;
  value: boolean;
}

class Product {
  productId: string;
  productName: string;
  productCode: string;
  productCodeName: string;
}

export class PromotionMapping {
  promotionMappingId: string;
  promotionId: string;
  indexOrder: number;
  hangKhuyenMai: string;
  soLuongTang: number;
  soTienTu: number;
  sanPhamMua: string;
  soLuongMua: number;
  chiChonMot: boolean;
  loaiGiaTri: boolean; //true: %, false: VNĐ
  giaTri: number;
  listPromotionProductMapping: Array<PromotionProductMapping>;

  error: boolean;
  selectedHangKhuyenMai: Product;

  selectedSanPhamMua: Product;
  selectedSanPhamGiamGia: Array<Product>;
  selectedSanPhamTang: Array<Product>;

  selectedChips: Array<string>; //list ProductCodeName của sản phẩm giảm giá hoặc sản phẩm tặng

  constructor() {
    this.soLuongTang = 0;
    this.soTienTu = 0;
    this.soLuongMua = 0;
    this.chiChonMot = false;
    this.loaiGiaTri = false;
    this.giaTri = 0;
    this.listPromotionProductMapping = [];
  }
}