import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { QueueModel } from '../models/queue.model';

interface LeadMeeting {
  leadMeetingId: string,
  leadId: string,
  employeeId: string,
  title: string,
  locationMeeting: string,
  startDate: Date,
  startHours: Date,
  content: string,
  participant: string;
}

@Injectable()
export class LeadCareService {

  constructor(private httpClient: HttpClient) { }
  
  sendQuickLeadEmail(listQueue : Array<QueueModel>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/lead/sendEmailSupportLead";
    return this.httpClient.post(url, { ListQueue: listQueue }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendQuickLeadSMS(listQueue : Array<QueueModel>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/lead/sendSMSSupportLead";
    return this.httpClient.post(url, { ListQueue: listQueue }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendGiftSupportLead(title: string, giftCustomerType1: number, giftTypeId1: string, giftTotal1: number,
    giftCustomerType2: number, giftTypeId2: string, giftTotal2: number, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/lead/sendGiftSupportLead";

    return this.httpClient.post(url,
      {
        Title: title,
        GiftCustomerType1: giftCustomerType1,
        GiftTypeId1: giftTypeId1,
        GiftTotal1: giftTotal1,
        GiftCustomerType2: giftCustomerType2,
        GiftTypeId2: giftTypeId2,
        GiftTotal2: giftTotal2,
        CustomerId: customerId
      }).pipe(
        map((response: Response) => {
          return response;
        }));
  }

  createLeadMeeting(leadMeeting: LeadMeeting) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/createLeadMeeting';
    return this.httpClient.post(url, 
      {
        LeadMeeting: leadMeeting
      }).pipe(map((response: Response) => {
        return response;
    }));
  }

}
