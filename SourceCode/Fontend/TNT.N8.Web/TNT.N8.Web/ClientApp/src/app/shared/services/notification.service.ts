
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Pipe({ name: 'NotificationService' })
export class NotificationService {

  constructor(private httpClient: HttpClient) { }

  getNotification(employeeId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/notification/getNotification";
    return this.httpClient.post(url, { EmployeeId: employeeId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  removeNotification(notificationId, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/notification/removeNotification";
    return this.httpClient.post(url, { NotificationId: notificationId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}