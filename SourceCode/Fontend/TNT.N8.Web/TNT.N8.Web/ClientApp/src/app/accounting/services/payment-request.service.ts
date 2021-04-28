
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PaymentRequestModel } from '../models/paymentRequest.model'
import { Document } from '../../shared/models/document.model';
@Pipe({ name: 'PaymentRequestService' })

@Injectable()
export class PaymentRequestService {

  constructor(private httpClient: HttpClient) { }

  findRequestPayment(requestCode: string, statusList: Array<string>, startDate: Date, endDate: Date, 
  	employeeId: string, paymentId: string, orgId: string, userId: string){
  	const url = localStorage.getItem('ApiEndPoint') + '/api/requestPayment/findRequestPayment';
    return this.httpClient.post(url, { RequestCode: requestCode, StatusList: statusList, StartDate: startDate, EndDate: endDate, 
    	EmployeeId: employeeId , PaymentId: paymentId, OrganizationId: orgId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createRequestPayment(fileList: File[], requestPayment: PaymentRequestModel, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/requestPayment/createRequestPayment';
    let formData: FormData = new FormData();

    formData.append('RequestPayment[RequestPaymentId]', requestPayment.RequestPaymentId);
    formData.append('RequestPayment[RequestPaymentCreateDate]', requestPayment.RequestPaymentCreateDate.toUTCString());
    formData.append('RequestPayment[RequestEmployee]', requestPayment.RequestEmployee);
    formData.append('RequestPayment[RequestPaymentNote]', requestPayment.RequestPaymentNote);
    formData.append('RequestPayment[RequestEmployeePhone]', requestPayment.RequestEmployeePhone);
    formData.append('RequestPayment[RequestBranch]', requestPayment.RequestBranch);
    formData.append('RequestPayment[ApproverId]', requestPayment.ApproverId);
    formData.append('RequestPayment[PostionApproverId]', requestPayment.PostionApproverId);
    formData.append('RequestPayment[TotalAmount]', requestPayment.TotalAmount.toString());
    formData.append('RequestPayment[PaymentType]', requestPayment.PaymentType);
    formData.append('RequestPayment[Description]', requestPayment.Description);
    formData.append('RequestPayment[NumberCode]', null);
    formData.append('RequestPayment[MonthCode]', null);
    formData.append('RequestPayment[StatusID]', null);
    formData.append('RequestPayment[CreateDate]', null);
    formData.append('RequestPayment[CreateById]', null);
    formData.append('RequestPayment[UpdateDate]', null);
    formData.append('RequestPayment[UpdateById]', null);

    for (var i = 0; i < fileList.length; i++) {
      formData.append('FileList', fileList[i]);
    }
    formData.append('UserId',userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getRequestPaymentById(requestPaymentId: string,userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/requestPayment/getRequestPaymentById';
    return this.httpClient.post(url, {
      RequestPaymentId: requestPaymentId, UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editRequestPayment(fileList: File[], requestPayment: PaymentRequestModel, lstDocument: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/requestPayment/editRequestPayment';
    let formData: FormData = new FormData();

    formData.append('RequestPayment[RequestPaymentId]', requestPayment.RequestPaymentId);
    formData.append('RequestPayment[RequestPaymentCreateDate]', requestPayment.RequestPaymentCreateDate.toString());
    formData.append('RequestPayment[RequestPaymentCode]', requestPayment.RequestPaymentCode);
    formData.append('RequestPayment[RequestEmployee]', requestPayment.RequestEmployee);
    formData.append('RequestPayment[RequestPaymentNote]', requestPayment.RequestPaymentNote);
    formData.append('RequestPayment[RequestEmployeePhone]', requestPayment.RequestEmployeePhone);
    formData.append('RequestPayment[RequestBranch]', requestPayment.RequestBranch);
    formData.append('RequestPayment[ApproverId]', requestPayment.ApproverId);
    formData.append('RequestPayment[PostionApproverId]', requestPayment.PostionApproverId);
    formData.append('RequestPayment[TotalAmount]', requestPayment.TotalAmount.toString());
    formData.append('RequestPayment[PaymentType]', requestPayment.PaymentType);
    formData.append('RequestPayment[Description]', requestPayment.Description);
    formData.append('RequestPayment[NumberCode]', requestPayment.NumberCode.toString());
    formData.append('RequestPayment[YearCode]', requestPayment.YearCode.toString());
    formData.append('RequestPayment[StatusID]', requestPayment.StatusID);
    formData.append('RequestPayment[CreateDate]', requestPayment.CreateDate.toString());
    formData.append('RequestPayment[CreateById]', requestPayment.CreateById);

    for (var i = 0; i < fileList.length; i++) {
      formData.append('FileList', fileList[i]);
    }
    for (var i = 0; i < lstDocument.length; i++) {
      formData.append('lstDocument', lstDocument[i]);
    }
    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));

  }
}
