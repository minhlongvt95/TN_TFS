
import {map} from 'rxjs/operators';
import { Pipe, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { CustomerServiceLevelModel } from '../models/customerservicelevel.model';
// tslint:disable-next-line:use-pipe-transform-interface
@Pipe({ name: 'CustomerlevelService' })

@Injectable()
export class CustomerlevelService {

  constructor(private httpClient: HttpClient) { }

  addLevelCustomer(customerServiceLevel: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/admin/customerservicelevel';
    return this.httpClient.post(url, { CustomerServiceLevel: customerServiceLevel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getLevelCustomer() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/admin/getCustomerservicelevel';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateLevelCustomer(id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/admin/updateConfigCustomer';
    return this.httpClient.post(url, { CustomerLevelId: id }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
