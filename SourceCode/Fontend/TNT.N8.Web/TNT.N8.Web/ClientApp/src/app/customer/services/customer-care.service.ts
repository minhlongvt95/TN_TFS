
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerCareModel } from "../models/customer-care.model";
import { CustomerCareFeedBack } from "../models/customer-care-feed-back.model";
import { QueueModel } from '../models/queue.model';

@Injectable()
export class CustomerCareService {

  constructor(private httpClient: HttpClient) { }

  createCustomerCare(customerCare: CustomerCareModel, customerId: Array<string>, listTypeCustomer: Array<string>, queryFilter: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/createCustomerCare";

    return this.httpClient.post(url, {
      CustomerCare: customerCare,
      CustomerId: customerId,
      ListTypeCustomer: listTypeCustomer,
      QueryFilter: queryFilter,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  updateCustomerCare(customerCare: CustomerCareModel, customerId: Array<string>, queryFilter: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateCustomerCare";

    return this.httpClient.post(url, {
      CustomerCare: customerCare,
      CustomerId: customerId,
      QueryFilter: queryFilter,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getCustomerCareById(customerCareId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareById";

    return this.httpClient.post(url, { CustomerCareId: customerCareId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTotalInteractive(month: number, year: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getTotalInteractive";

    return this.httpClient.post(url, { Month: month, Year: year }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerCareActive() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareActive";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCharCustomerCS() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCharCustomerCS";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerNewCS() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerNewCS";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerBirthDay() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerBirthDay";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createCustomerCareFeedBack(customerCareFeedBack: CustomerCareFeedBack, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/createCustomerCareFeedBack";

    return this.httpClient.post(url, { CustomerCareFeedBack: customerCareFeedBack, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  updateCustomerCareFeedBack(customerCareFeedBack: CustomerCareFeedBack, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateCustomerCareFeedBack";

    return this.httpClient.post(url, { CustomerCareFeedBack: customerCareFeedBack, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  filterCustomer(sqlQuery: string, typeCustomer: Array<any>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/filterCustomer";

    return this.httpClient.post(url, {
      SqlQuery: sqlQuery,
      TypeCustomer: typeCustomer,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerCareByIdAsync(customerCareId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerCareId: customerCareId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  searchCustomerCare(fromDate: Date, toDate: Date, customerCareTitle: string, picName: Array<string>, status: Array<string>, customerCareContent: string, programType: Array<number>, userId: string, customerCareCode: string, typeCusCare: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/searchCustomerCare";

    return this.httpClient.post(url, { FromDate: fromDate, ToDate: toDate, CustomerCareTitle: customerCareTitle, PicName: picName, Status: status, CustomerCareContent: customerCareContent, ProgramType: programType, UserId: userId, CustomerCareCode: customerCareCode, ListTypeCusCareId: typeCusCare }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusCustomerCareCustomer(customerCareCustomerId: string, statusId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCustomerCareCustomerById";

    return this.httpClient.post(url, { CustomerCareCustomerId: customerCareCustomerId, StatusId: statusId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusCustomerCareCustomerAsync(customerCareCustomerId: string, statusId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCustomerCareCustomerById";

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CustomerCareCustomerId: customerCareCustomerId, StatusId: statusId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getTimeLineCustomerCareByCustomerId(customerId: string, first_day: Date, last_day: Date) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getTimeLineCustomerCareByCustomerId";

    return this.httpClient.post(url, { CustomerId: customerId, First_day: first_day, Last_day: last_day }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTimeLineCustomerCareByCustomerIdAsync(customerId: string, first_day: Date, last_day: Date) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getTimeLineCustomerCareByCustomerId";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CustomerId: customerId, First_day: first_day, Last_day: last_day }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getCustomerCareFeedBackByCusIdAndCusCareId(customerId: string, customerCareId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareFeedBackByCusIdAndCusCareId";

    return this.httpClient.post(url, { CustomerId: customerId, CustomerCareId: customerCareId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendQuickEmail(queue: QueueModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/sendQuickEmail";

    return this.httpClient.post(url, { Queue: queue }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendQuickSMS(queue: QueueModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/sendQuickSMS";

    return this.httpClient.post(url, { Queue: queue }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendQuickGift(title: string, giftCustomerType1: number, giftTypeId1: string, giftTotal1: number,
    giftCustomerType2: number, giftTypeId2: string, giftTotal2: number, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/sendQuickGift";

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

  updateStatusCustomerCare(customerCareId: string, statusId: string, isSendNow: boolean, sendDate: Date, sendHour: Date, typeCusCareCode: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCustomerCare";
    return this.httpClient.post(url,
      {
        CustomerCareId: customerCareId,
        StatusId: statusId,
        IsSendNow: isSendNow,
        SendDate: sendDate,
        SendHour: sendHour,
        TypeCusCareCode: typeCusCareCode,
        UserId: userId
      }).pipe(
        map((response: Response) => {
          return response;
        }));
  }

  getMasterDataCustomerCareList(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getMasterDataCustomerCareList";
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCustomerCareListAsync(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getMasterDataCustomerCareList";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateStatusCusCare(userId: string, customerCareId: string, statusId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCusCare";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: userId, CustomerCareId: customerCareId, StatusId: statusId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  removeCustomerMeeting(customerMeetingId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/removeCustomerMeeting";
    return this.httpClient.post(url, { CustomerMeetingId: customerMeetingId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
