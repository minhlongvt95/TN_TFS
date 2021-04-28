
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
@Pipe({ name: 'EmailService' })

@Injectable()
export class EmailService {

  constructor(private httpClient: HttpClient) { }

  sendEmailAfterEditPic(picId, currentUserId, leadId, contactId, currentUrl, empCCIdList, potentialName, statusName, picName) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmailAfterEditPic";
    return this.httpClient.post(url, {
        PicId: picId, UserId: currentUserId, LeadId: leadId, ContactId: contactId, CurrentUrl: currentUrl, EmpCCIdList: empCCIdList,
        PotentialName: potentialName, StatusName: statusName, PicName: picName
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  
  sendEmailAfterCreatedLead(currentUsername, currentUserEmail, currentUrl, userId, leadId) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmailAfterCreatedLead";
    return this.httpClient.post(url, { CurrentUsername: currentUsername, CurrentUserEmail: currentUserEmail, CurrentUrl: currentUrl, UserId: userId, LeadId: leadId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendEmailAfterCreateNote(employeeIdList, currentUsername, currentUrl, userId, leadId, noteContent, fileNameArray) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmailAfterCreateNote";
    return this.httpClient.post(url, {
      CurrentUsername: currentUsername, EmployeeIdList: employeeIdList, CurrentUrl: currentUrl,
      UserId: userId, LeadId: leadId, NoteContent: noteContent, FileNameArray: fileNameArray
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailEmployeePayslip(lstEmpMonthySalary: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmailEmployeePayslip";
    return this.httpClient.post(url, { lstEmpMonthySalary: lstEmpMonthySalary}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailTeacherPayslip(lstEmpMonthySalary: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmailTeacherPayslip";
    return this.httpClient.post(url, { lstEmpMonthySalary: lstEmpMonthySalary }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailAssistantPayslip(lstEmpMonthySalary: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmailAssistantPayslip";
    return this.httpClient.post(url, { lstEmpMonthySalary: lstEmpMonthySalary }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailLead(title: string, sendContent: string, isSendEmailNow: boolean, sendEmailDate: Date,
    sendEmailHour: any, leadIdList: Array<any>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/sendEmailLead';
    return this.httpClient.post(url, {
      Title: title,
      SendContent: sendContent,
      IsSendEmailNow: isSendEmailNow,
      SendEmailDate: sendEmailDate,
      SendEmailHour: sendEmailHour,
      LeadIdList: leadIdList,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendSMSLead(sendContent: string, isSendSMSNow: boolean, sendSMSDate: Date,
    sendSMSHour: any, leadIdList: Array<any>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/sendSMSLead';
    return this.httpClient.post(url, {
      SendContent: sendContent,
      IsSendSMSNow: isSendSMSNow,
      SendSMSDate: sendSMSDate,
      SendSMSHour: sendSMSHour,
      LeadIdList: leadIdList,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailPersonApprove(approveId: string, requestId: string, fullNameRequest: string,
    activeRequest: string, accountApprove: string, fullNameApprove: string, dateCreate: string, requestType: string,
    dateStart: string, caStart: string, dateEnd: string, caEnd: string, note: string, listFullNameNotify: string, requestEmId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailPersonApprove';
    return this.httpClient.post(url, {
      ApproveId: approveId,
      RequestId: requestId,
      FullNameRequest: fullNameRequest,
      ActiveRequest: activeRequest,
      AccountApprove: accountApprove,
      FullNameApprove: fullNameApprove,
      DateCreate: dateCreate,
      RequestType: requestType,
      DateStart: dateStart,
      CaStart: caStart,
      DateEnd: dateEnd,
      CaEnd: caEnd,
      Note: note,
      ListFullNameNotify: listFullNameNotify,
      RequestEmployeeId: requestEmId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailPersonCreate(createId: string, fullNameCreate: string, requestId: string, fullNameRequest: string,
    activeRequest: string, accountApprove: string, fullNameApprove: string, dateCreate: string, requestType: string,
    dateStart: string, caStart: string, dateEnd: string, caEnd: string, note: string, listFullNameNotify: string, requestEmId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailPersonCreate';
    return this.httpClient.post(url, {
      CreateId: createId,
      FullNameCreate: fullNameCreate,
      RequestId: requestId,
      FullNameRequest: fullNameRequest,
      ActiveRequest: activeRequest,
      AccountApprove: accountApprove,
      FullNameApprove: fullNameApprove,
      DateCreate: dateCreate,
      RequestType: requestType,
      DateStart: dateStart,
      CaStart: caStart,
      DateEnd: dateEnd,
      CaEnd: caEnd,
      Note: note,
      ListFullNameNotify: listFullNameNotify,
      RequestEmployeeId: requestEmId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailPersonNotify(notifyId: string[], accountCreate: string,
    fullNameCreate: string, requestId: string, fullNameRequest: string,
    accountApprove: string, fullNameApprove: string,
    dateCreate: string, requestType: string, dateStart: string, caStart: string,
    dateEnd: string, caEnd: string, note: string, listFullNameNotify: string, requestEmId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailPersonNotify';
    return this.httpClient.post(url, {
      notifyId: notifyId,
      AccountCreate: accountCreate,
      FullNameCreate: fullNameCreate,
      RequestId: requestId,
      FullNameRequest: fullNameRequest,
      AccountApprove: accountApprove,
      FullNameApprove: fullNameApprove,
      DateCreate: dateCreate,
      RequestType: requestType,
      DateStart: dateStart,
      CaStart: caStart,
      DateEnd: dateEnd,
      CaEnd: caEnd,
      Note: note,
      ListFullNameNotify: listFullNameNotify,
      RequestEmployeeId: requestEmId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
