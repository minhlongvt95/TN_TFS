
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'DistrictService' })

@Injectable()
export class DistrictService {

  constructor(private httpClient: HttpClient) { }

  getAllDistrictByProvinceId(provinceId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/district/getAllDistrictByProvinceId";
    return this.httpClient.post(url, { ProvinceId: provinceId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllDistrictByProvinceIdAsync(provinceId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/district/getAllDistrictByProvinceId";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { ProvinceId: provinceId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
