
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'PositionService' })
@Injectable()
export class PositionService {

    constructor(private httpClient: HttpClient) { }

  getAllPosition() {
      let url = localStorage.getItem('ApiEndPoint') + "/api/position/getAllPosition";
      return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
