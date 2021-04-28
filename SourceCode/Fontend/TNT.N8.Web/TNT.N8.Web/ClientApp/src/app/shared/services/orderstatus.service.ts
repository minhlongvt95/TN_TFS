
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'OrderstatusService' })

@Injectable()
export class OrderstatusService {

  constructor(private httpClient: HttpClient) { }
  getAllOrderStatus() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/orderstatus/getAllOrderStatus";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getOrderStatusByID() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/orderstatus/getOrderStatusByID";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
