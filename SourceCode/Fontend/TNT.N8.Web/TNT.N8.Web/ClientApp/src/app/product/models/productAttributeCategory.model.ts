export class ProductAttributeCategoryModel {
  ProductAttributeCategoryId: string;
  ProductAttributeCategoryName: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
  ProductAttributeCategoryValue: Array<ProductAttributeCategoryValueModel>;
  ValueString: string;
  DefaultValue: string;
}

export class ProductAttributeCategoryValueModel {
  ProductAttributeCategoryValueId: string;
  ProductAttributeCategoryValue1: string;
  ProductAttributeCategoryId: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
}

