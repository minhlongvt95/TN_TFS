
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";



@Injectable()
export class TopRevenueService {

  constructor(private httpClient: HttpClient) { }

  getEmployeeByOrganizationId(organizationId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeByOrganizationId";
    return this.httpClient.post(url, { OrganizationId: organizationId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeByTopRevenue(startDate: Date, endDate: Date, listorganizationId: Array<string>, employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeByTopRevenue";
    return this.httpClient.post(url, {
      StartDate: startDate,
      EndDate: endDate,
      EmployeeId: employeeId,
      ListorganizationId: listorganizationId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  exportEmployeeRevenue(startDate: Date, endDate: Date, listorganizationId: Array<string>, employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/exportEmployeeRevenue";
    return this.httpClient.post(url, {
      StartDate: startDate,
      EndDate: endDate,
      EmployeeId: employeeId,
      ListorganizationId: listorganizationId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataSearchTopReVenue(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/getDataSearchTopReVenue';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchTopReVenue(filterData: any, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/searchTopReVenue';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ...filterData,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataSearchRevenueProduct(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/getDataSearchRevenueProduct';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchRevenueProduct(defaultFilter: any, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/searchRevenueProduct';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ...defaultFilter,

        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
