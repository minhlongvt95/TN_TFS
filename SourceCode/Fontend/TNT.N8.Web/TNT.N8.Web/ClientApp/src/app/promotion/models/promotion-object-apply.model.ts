import { PromotionObjectApplyMapping } from './promotion-object-apply-mapping.model';

export class PromotionObjectApply {
  promotionObjectApplyId: string;
  objectId: string;
  objectType: string;
  promotionId: string;
  promotionName: string;
  conditionsType: number;
  propertyType: number;
  propertyTypeName: string;
  notMultiplition: boolean;
  promotionMappingId: string;
  productId: string;
  productUnitName: string;
  promotionProductName: string;
  promotionProductNameConvert: string;
  soLuongTang: number;
  loaiGiaTri: boolean;
  giaTri: number;
  amount: number;
  soTienTu: number;

  //Theo sản phẩm: Mua hàng giảm giá hàng, Mua háng tặng hàng
  selectedDetail: string;
  selectedPromotionObjectApplyMapping: Array<PromotionObjectApplyMapping>;
  //End

  constructor() {
    this.amount = 0;
    this.soTienTu = 0;
    this.selectedDetail = '';
  }
}