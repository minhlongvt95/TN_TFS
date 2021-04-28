
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductCategoryModel } from '../models/productcategory.model';

@Pipe ({name: "ProductCategoryService"})
@Injectable()
export class ProductCategoryService {

  constructor(private httpClient: HttpClient) { }

  createProductCategory(name: string, level: number, productCategoryCode: string, description: string, parentId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/ProductCategory/CreateProductCategory";
    return this.httpClient.post(url, { ProductCategoryName: name, ProductCategoryLevel: level, ProductCategoryCode: productCategoryCode, Description: description, ParentId: parentId}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllProductCategory() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/getAllProductCategory";
    return this.httpClient.post(url, {}).pipe(
        map((response: Response) => {
            return response;
        }));
  }

  getAllProductCategoryAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/productCategory/getAllProductCategory';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductCategoryById(ProductCategoryId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/getProductCategoryById";
    return this.httpClient.post(url, { ProductCategoryId: ProductCategoryId}).pipe(
        map((response: Response) => {
            return response;
        }));
  }

  editProductCategory(ProductCategoryId: string,ProductCategoryName: string, ProductCategoryCode: string, Description: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/editProductCategory";
    return this.httpClient.post(url, { ProductCategoryId: ProductCategoryId,ProductCategoryName:ProductCategoryName,ProductCategoryCode:ProductCategoryCode,Description:Description}).pipe(
        map((response: Response) => {
            return response;
        }));
  }

  deleteProductCategory(ProductCategoryId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/deleteProductCategory";
    return this.httpClient.post(url, { ProductCategoryId: ProductCategoryId}).pipe(
        map((response: Response) => {
            return response;
        }));
  }

  searchProductCategory(Keyword_name: string, Keyword_code : string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/searchProductCategory";
    return this.httpClient.post(url, { Keyword_name: Keyword_name, Keyword_code: Keyword_code}).pipe(
        map((response: Response) => {
            return response;
        }));
  }
  //api/productCategory/getNameTreeProductCategory
  getNameTreeProductCategory(productCategoryID: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/getNameTreeProductCategory";
    return this.httpClient.post(url, { ProductCategoryID: productCategoryID }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getListCodeAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/getAllCategoryCode";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateActiveProductCategory(ProductCategoryId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/productCategory/updateActiveProductCategory";
    return this.httpClient.post(url, { ProductCategoryId: ProductCategoryId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
