
import {map} from 'rxjs/operators';
import { Pipe, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { PermissionModel } from '../models/permission.model';
import { MenuBuild } from '../../admin/models/menu-build.model';

@Pipe({ name: 'PermissionService' })
@Injectable()
export class PermissionService {

  constructor(private httpClient: HttpClient) { }

  createPermission(permissionIdList: Array<any>, permissionSetName: string, permissionSetCode: string, permissionSetDescription: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/createPermission';
    return this.httpClient.post(url, { PermissionIdList: permissionIdList, PermissionSetName: permissionSetName, PermissionSetCode: permissionSetCode, PermissionSetDescription: permissionSetDescription }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createRoleAndPermission(roleValue: string, roleDescription: string, listActionResource: Array<string>, listMenuBuild: Array<MenuBuild>) 
  {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/createRoleAndPermission';
    return this.httpClient.post(url, { 
            RoleValue: roleValue,
            Description: roleDescription, 
            ListActionResource: listActionResource,
            ListMenuBuild: listMenuBuild
           }).pipe(
            map((response: Response) => 
            {
              return response;
            }));
  }

  editPermissionById(permissionSetId: string, permissionSetName: string, permissionSetDes: string, permissionSetCode: string, permissionIdList: Array<any>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/editPermissionById';
    return this.httpClient.post(url, { PermissionSetId: permissionSetId, PermissionSetName: permissionSetName, PermissionSetDescription: permissionSetDes, PermissionSetCode: permissionSetCode, PermissionIdList: permissionIdList }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deletePermissionById(permissionSetId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/deletePermissionById';
    return this.httpClient.post(url, { PermissionSetId: permissionSetId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getPermissionById(permissionSetId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getPermissionById';
    return this.httpClient.post(url, { PermissionSetId: permissionSetId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllPermission() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getAllPermission';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getPermissionByCode(code: Array<any>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getPermissionByCode';
    return this.httpClient.post(url, { PerCode: code }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getModuleByPermissionSetId(permissionSetId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getModuleByPermissionSetId';
    return this.httpClient.post(url, { PermissionSetId: permissionSetId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllPermissionSetNameAndCode() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getAllPermissionSetNameAndCode';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllRole() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getAllRole';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCreatePermission() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getCreatePermission';
    return this.httpClient.post(url, { }).pipe(
    map((response: Response) => {
      return response;
    }));
  }

  getDetailPermission(roleId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/getDetailPermission';
    return this.httpClient.post(url, { RoleId: roleId }).pipe(
    map((response: Response) => {
      return response;
    }));
  }

  editRoleAndPermission(roleId: string, roleValue: string, roleDescription: string, listActionResource: Array<string>, listMenuBuild: Array<MenuBuild>) 
  {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/editRoleAndPermission';
    return this.httpClient.post(url, { 
            RoleId: roleId,
            RoleValue: roleValue,
            Description: roleDescription, 
            ListActionResource: listActionResource,
            ListMenuBuild: listMenuBuild
           }).pipe(
            map((response: Response) => 
            {
              return response;
            }));
  }

  addUserRole(employeeId: string, roleId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/addUserRole';
    return this.httpClient.post(url, { EmployeeId: employeeId, RoleId: roleId, UserId: userId }).pipe(
    map((response: Response) => {
      return response;
    }));
  }

  deleteRole(roleId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/auth/deleteRole';
    return this.httpClient.post(url, { RoleId: roleId, UserId: userId }).pipe(
    map((response: Response) => {
      return response;
    }));
  }
}
