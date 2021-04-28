
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";



@Injectable()
export class SalesDashboardService {

  constructor(private httpClient: HttpClient) { }

  //không dùng
  // get_positionCode_by_positionId(userId: string, positionId: string, employeeId: string) {
  //   let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getPositionCodeByPositionId';
  //   return new Promise((resolve, reject) => {
  //     return this.httpClient.post(url, { UserId: userId, PositionId: positionId, EmployeeId: employeeId }).toPromise()
  //       .then((response: Response) => {
  //         resolve(response);
  //       });
  //   });
  // }

  getChildrenOrganizationById(employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/organization/getChildrenOrganizationById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  // Get all customer order list
  getCustomerOrderBySellerList(Seller: string, orderDateStart: Date, orderDateEnd: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getCustomerOrderBySeller';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Seller: Seller,
        OrderDateStart: orderDateStart,
        OrderDateEnd: orderDateEnd
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  // Get Top3New customer order list
  getTop3NewCustomerOrderList(orderCode: string, customerName: string, productCode: string,
    seller: string, orderDateStart: Date, orderDateEnd: Date, vat: number, top3NewOrder: number, ListorganizationId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getAllOrder';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        OrderCode: orderCode,
        CustomerName: customerName,
        ProductCode: productCode,
        Seller: seller,
        OrderDateStart: orderDateStart,
        OrderDateEnd: orderDateEnd,
        Vat: vat,
        Top3NewOrder: top3NewOrder,
        ListOrganizationId: ListorganizationId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getEmployeeListByOrganizationId(userId: string, orderDateStart: Date, orderDateEnd: Date, organizationId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getEmployeeListByOrganizationId';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId,
        OrderDateStart: orderDateStart,
        OrderDateEnd: orderDateEnd,
        OrganizationId: organizationId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMonthsList(userId: string, orderDateTo: Date, monthAdd: number, organizationId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMonthsList';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId,
        MonthAdd: monthAdd,
        OrderDateTo: orderDateTo,
        OrganizationId: organizationId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductCategoryGroupByLevel(Seller: string, orderDateStart: Date, orderDateEnd: Date, productCategoryLevel: number) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/getProductCategoryGroupByLevel';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Seller: Seller,
        OrderDateStart: orderDateStart,
        OrderDateEnd: orderDateEnd,
        ProductCategoryLevel: productCategoryLevel
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductCategoryGroupByManager(userId: string, orderDateStart: Date, orderDateEnd: Date, organizationId: string,
    productCategoryLevel: number) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/getProductCategoryGroupByManager';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId,
        OrderDateStart: orderDateStart,
        OrderDateEnd: orderDateEnd,
        OrganizationId: organizationId,
        ProductCategoryLevel: productCategoryLevel
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
