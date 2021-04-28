import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MenuBuild } from '../models/menu-build.model';

@Injectable()
export class MenuBuildService {
  constructor(private httpClient: HttpClient) { }

  getMenuModule() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/getMenuModule';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createMenuBuild(menuBuild: MenuBuild) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/createMenuBuild';
    return this.httpClient.post(url, { MenuBuild: menuBuild }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateMenuBuild(menuBuild: MenuBuild) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/updateMenuBuild';
    return this.httpClient.post(url, { MenuBuild: menuBuild }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteMenuBuild(menuBuildId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/deleteMenuBuild';
    return this.httpClient.post(url, { MenuBuildId: menuBuildId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getSubMenuModuleByMenuModuleCode(menuModuleCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/getSubMenuModuleByMenuModuleCode';
    return this.httpClient.post(url, { MenuModuleCode: menuModuleCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMenuPageBySubMenuCode(subMenuCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/getMenuPageBySubMenuCode';
    return this.httpClient.post(url, { SubMenuCode: subMenuCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateIsPageDetail(menuBuildId: string, isPageDetail: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/menubuild/updateIsPageDetail';
    return this.httpClient.post(url, { MenuBuildId: menuBuildId, IsPageDetail: isPageDetail }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}