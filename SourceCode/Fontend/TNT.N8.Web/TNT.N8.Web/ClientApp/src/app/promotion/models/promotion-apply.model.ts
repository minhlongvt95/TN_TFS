import { PromotionProductApply } from './promotion-product-apply.model';
import { PromotionProductMappingApply } from './promotion-product-mapping-apply.model';

export class PromotionApply {
  promotionId: string;
  promotionName: string;
  conditionsType: number;
  propertyType: number;
  propertyTypeName: string;
  notMultiplition: boolean;
  listPromotionProductApply: Array<PromotionProductApply>;
  selectedPromotionProductApply: Array<PromotionProductApply>;

  //Trường hợp khuyến mãi theo sản phẩm (mua hàng giảm giá hàng, mua hàng tặng hàng)
  promotionMappingId: string;
  soLuongTang: number; //Tổng số lượng giảm giá (tặng) tối đa
  chiChonMot: boolean;
  listPromotionProductMappingApply: Array<PromotionProductMappingApply>;
  selectedPromotionProductMappingApply: Array<PromotionProductMappingApply>;
  selectedDetail: string;
}