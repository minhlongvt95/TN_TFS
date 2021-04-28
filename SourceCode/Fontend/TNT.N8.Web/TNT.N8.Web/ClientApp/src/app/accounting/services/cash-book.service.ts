
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CashBookService {

  constructor(private httpClient: HttpClient) { }

  getSurplusCashBookPerMonth(userId: string, fromDate: Date, toDate: Date, organizationList: any) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/cashbook/getSurplusCashBookPerMonth';

    return this.httpClient.post(url, {
      UserId: userId, FromDate: fromDate, ToDate: toDate, OrganizationList: organizationList
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getSurplusCashBookPerMonthAsync(userId: string, fromDate: Date, toDate: Date, organizationList: any) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/cashbook/getSurplusCashBookPerMonth';

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId, FromDate: fromDate, ToDate: toDate, OrganizationList: organizationList
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });

  }


}
