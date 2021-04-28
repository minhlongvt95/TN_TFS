export class ProductCategoryModel {
  ProductCategoryId: string;
  Name: string;
  Level: number;
  ProductCategoryCode: string;
  Description:string;
  ParentID: string;
  Active: Boolean;
  ProductCategoryChildList: Array<any>;
}
