import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NotifiSetting } from '../models/notifiSetting.model';
import { NotifiSpecial } from '../models/notifiSpecial.model';
import { Screen } from '../models/screen.model';

@Injectable()
export class NotifiSettingService {

  constructor(private httpClient: HttpClient) { }

  getMasterDataNotifiSettingCreate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/getMasterDataNotifiSettingCreate';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNotifiSetting(notifiSetting: NotifiSetting, listNotifiSpecial: Array<NotifiSpecial>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/createNotifiSetting';
    return this.httpClient.post(url, { 
      NotifiSetting: notifiSetting, 
      ListNotifiSpecial: listNotifiSpecial 
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataNotifiSettingDetail(notifiSettingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/getMasterDataNotifiSettingDetail';
    return this.httpClient.post(url, { NotifiSettingId: notifiSettingId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateNotifiSetting(notifiSetting: NotifiSetting, listNotifiSpecial: Array<NotifiSpecial>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/updateNotifiSetting';
    return this.httpClient.post(url, { 
      NotifiSetting: notifiSetting, 
      ListNotifiSpecial: listNotifiSpecial 
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchNotifiSetting() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/getMasterDataSearchNotifiSetting';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchNotifiSetting(notifiSettingName: string, listScreen: Array<Screen>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/searchNotifiSetting';
    return this.httpClient.post(url, { 
      NotifiSettingName: notifiSettingName,
      ListScreen: listScreen
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeActive(notifiSettingId: string, active: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/changeActive';
    return this.httpClient.post(url, { 
      NotifiSettingId: notifiSettingId,
      Active: active
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeSendInternal(notifiSettingId: string, sendInternal: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/changeSendInternal';
    return this.httpClient.post(url, { 
      NotifiSettingId: notifiSettingId,
      SendInternal: sendInternal
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeIsSystem(notifiSettingId: string, isSystem: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/changeIsSystem';
    return this.httpClient.post(url, { 
      NotifiSettingId: notifiSettingId,
      IsSystem: isSystem
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeIsEmail(notifiSettingId: string, isEmail: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/changeIsEmail';
    return this.httpClient.post(url, { 
      NotifiSettingId: notifiSettingId,
      IsEmail: isEmail
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeIsSms(notifiSettingId: string, isSms: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/changeIsSms';
    return this.httpClient.post(url, { 
      NotifiSettingId: notifiSettingId,
      IsSms: isSms
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeBackHourInternal(notifiSettingId: string, backHourInternal: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/notifisetting/changeBackHourInternal';
    return this.httpClient.post(url, { 
      NotifiSettingId: notifiSettingId,
      BackHourInternal: backHourInternal
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
