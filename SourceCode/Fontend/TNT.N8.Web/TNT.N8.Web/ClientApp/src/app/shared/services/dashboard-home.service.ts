
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { start } from 'repl';

@Pipe({ name: 'DashboardHomeService' })
export class DashboardHomeService {

  constructor(private httpClient: HttpClient) { }

  getDataDashboardHome() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/order/getDataDashboardHome";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateCustomerMeeting(customerMeetingId: string, startDate: Date, endDate: Date, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateCustomerMeeting";
    return this.httpClient.post(url, {
      CustomerMeetingId: customerMeetingId,
      StartDate: startDate,
      EndDate: endDate,
      UserId : userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
