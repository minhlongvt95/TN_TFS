
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'WardService' })
@Injectable()
export class WardService {

  constructor(private httpClient: HttpClient) { }

  getAllWardByDistrictId(districtId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/ward/getAllWardByDistrictId";
    return this.httpClient.post(url, { DistrictId: districtId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllWardByDistrictIdAsync(districtId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/ward/getAllWardByDistrictId";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { DistrictId: districtId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
