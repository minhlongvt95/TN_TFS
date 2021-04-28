
import {map} from 'rxjs/operators';
import { Pipe, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";


import { Observable } from 'rxjs';

import { UserModel } from '../models/user.model';
import { AuthModel } from '../models/auth.model';
import { PermissionModel } from '../models/permission.model';

@Pipe({ name: 'AuthenticationService' })
@Injectable()
export class AuthenticationService {

  constructor(private httpClient: HttpClient) { }

  currentUser = <any>localStorage.getItem('auth');

  login(user: UserModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth';
    return this.httpClient.post(url, { User: user}).pipe(
      map((response: Response) => {        
        let result = <any>response;
        // login successful if there's a jwt token in the response
        if (result && result.statusCode === 202) {
          let authModel = <AuthModel>({
            UserId: result.currentUser.userId,
            EmployeeId: result.currentUser.employeeId,
            PositionId: result.currentUser.positionId,
            Token: result.currentUser.token,
            Logintime: new Date()
          });
          
          localStorage.setItem('auth', JSON.stringify(authModel));
        }

        return result;
      }));
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('auth');
  }

  // getMenuByModuleCode(moduleCode: any): Observable<any> {
  //   let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getmenubymodulecode';
  //   return this.httpClient.post(url, { ModuleCode: moduleCode }).pipe(
  //     map((response: Response) => {        
  //       var result = <any>response;
  //       var permissions: Array<PermissionModel> = [];
  //       result.permissions.forEach(obj => {
  //         let ram = <PermissionModel>({
  //           PermissionId:obj.permissionId,
  //           PermissionCode: obj.permissionCode,
  //           PermissionName: obj.permissionName,
  //           IconClass: obj.iconClass
  //         });
  //         permissions.push(ram);          
  //       });
  //       return permissions;
  //     }));
  // }

  getUserPermission(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getUserPermission';
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changePassword(userId: string, oldpass: string, newpass: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/changePassword';
    return this.httpClient.post(url, { UserId: userId, OldPassword: oldpass, NewPassword: newpass }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editUserProfile(userId: string, username: string, firstname: string, lastname: string, email: string, avatar: string) {
      let url = localStorage.getItem('ApiEndPoint') + '/api/auth/editUserProfile';
    return this.httpClient.post(url, {
      UserId: userId,
      Username: username,
      FirstName: firstname,
      LastName: lastname,
      Email: email,
      AvatarUrl: avatar
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getUserProfile(userId: string) {
      let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getUserProfile';
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getAllUser() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getAllUser';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
