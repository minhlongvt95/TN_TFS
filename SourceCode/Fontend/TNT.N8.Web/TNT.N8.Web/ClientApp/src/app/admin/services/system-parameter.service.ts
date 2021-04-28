
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SystemParameterService {

  constructor(private httpClient: HttpClient) { }

  GetAllSystemParameter() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/company/getAllSystemParameter';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
        
      }));
  }

  ChangeSystemParameter(systemKey: string, systemValue: any, systemValueString: any, description: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/company/changeSystemParameter';
    return this.httpClient.post(url, {
        SystemKey: systemKey,
        SystemValue: systemValue,
        SystemValueString: systemValueString,
        Description: description
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
