
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'ProvinceService' })

@Injectable()
export class ProvinceService {

  constructor(private httpClient: HttpClient) { }

  getAllProvince() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/province/getAllProvince";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
