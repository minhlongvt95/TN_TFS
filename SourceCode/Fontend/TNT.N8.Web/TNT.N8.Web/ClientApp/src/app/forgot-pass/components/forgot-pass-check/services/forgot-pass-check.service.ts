
import {map} from 'rxjs/operators';
import { Pipe, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { ForgotPassModel } from '../../../models/forgotPass.model';


import { Observable } from 'rxjs';

@Injectable()
export class ForgotPassCheckService {

  constructor(private httpClient: HttpClient) { }

  check_user(user_name: ForgotPassModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getCheckUserName';
    return this.httpClient.post(url, { UserName: user_name.UserName }).pipe(
      map((response: Response) => {
        let result = <any>response;
        return result;
      }));
  }

  send_email(user_info: ForgotPassModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailForgotPass';
    return this.httpClient.post(url, user_info).pipe(
      map((response: Response) => {
        let result = <any>response;
        return result;
      }))
  }
}
