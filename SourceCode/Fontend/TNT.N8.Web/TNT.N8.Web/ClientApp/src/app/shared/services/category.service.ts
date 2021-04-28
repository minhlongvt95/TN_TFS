
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'CategoryService' })

@Injectable()
export class CategoryService {

  constructor(private httpClient: HttpClient) { }

  getAllCategoryByCategoryTypeCode(categoryTypeCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/getAllCategoryByCategoryTypeCode";
    return this.httpClient.post(url, { CategoryTypeCode: categoryTypeCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getAllCategoryByCategoryTypeCodeAsyc(categoryTypeCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/getAllCategoryByCategoryTypeCode";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CategoryTypeCode: categoryTypeCode }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllCategoryType() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/getAllCategory";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllCategory() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/getAllCategory";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCategoryById(categoryId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/getCategoryById";
    return this.httpClient.post(url, { CategoryId: categoryId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCategoryByIdAsync(categoryId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/getCategoryById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CategoryId: categoryId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createCategory(categoryName: string, categoryCode: string, categoryTypeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/createCategory";
    return this.httpClient.post(url, { CategoryName: categoryName, CategoryCode: categoryCode, CategoryTypeId: categoryTypeId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteCategoryById(categoryId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/deleteCategoryById";
    return this.httpClient.post(url, { CategoryId: categoryId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editCategoryById(categoryId: string, categoryName: string, categoryCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/editCategoryById";
    return this.httpClient.post(url, { CategoryId: categoryId, CategoryName: categoryName, CategoryCode: categoryCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusIsActive(categoryId: string, active: Boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/updateStatusIsActive";
    return this.httpClient.post(url, { CategoryId: categoryId, Active: active }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusIsDefault(categoryId: string, categoryTypeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/category/updateStatusIsDefault";
    return this.httpClient.post(url, { CategoryId: categoryId, CategoryTypeId: categoryTypeId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
