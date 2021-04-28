
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { ForgotPassModel } from '../../../models/forgotPass.model';


import { Observable } from 'rxjs';

@Injectable()
export class ForgotPassChangeService {

  constructor(private httpClient: HttpClient) { }

  check_code(code: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getCheckResetCodeUser';
    return this.httpClient.post(url, { Code: code }).pipe(
      map((response: Response) => {
        let result = <any>response;
        return result;
      }));
  }

  reset_pass(userId: string, password: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/resetPassword';
    return this.httpClient.post(url, { UserId: userId, Password: password }).pipe(
      map((response: Response) => {
        let result = <any>response;
        return result;
      }));
  }
}
