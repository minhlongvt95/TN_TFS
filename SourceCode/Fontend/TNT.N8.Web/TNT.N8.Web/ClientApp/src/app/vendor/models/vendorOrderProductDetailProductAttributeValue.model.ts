export class VendorOrderProductDetailProductAttributeValueModel {
  vendorOrderDetailId: string;
  productId: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
  vendorOrderProductDetailProductAttributeValueId: string;
  orderProductDetailProductAttributeValueId: string;
  productAttributeCategoryName: string;
  productAttributeCategoryValue: Array<ProductAttributeCategoryValueModel>;

  constructor() {
    this.productAttributeCategoryValue = [];
  }
}

class ProductAttributeCategoryValueModel {
  productAttributeCategoryValueId: string;
  productAttributeCategoryValue1: string;
}
