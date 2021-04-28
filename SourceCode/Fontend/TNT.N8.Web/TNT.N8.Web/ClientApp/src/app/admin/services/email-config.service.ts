import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { EmailTemplateModel } from '../models/createEmailTemplate.model';
import { SendEmailModel } from '../models/sendEmail.model';

@Injectable()
export class EmailConfigService {

  constructor(private httpClient: HttpClient) { }

  createUpdateEmailTemplateMasterdata(emailTemplateId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/emailConfig/createUpdateEmailTemplateMasterdata';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmailTemplateId: emailTemplateId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchEmailConfigMasterdata() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/emailConfig/searchEmailConfigMasterdata';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
   
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createUpdateEmailTemplate(emailTemplateModel: EmailTemplateModel, listCCEmail: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/emailConfig/createUpdateEmailTemplate';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmailTemplateEntityModel: emailTemplateModel,
        ListEmailToCC: listCCEmail,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchEmailTemplate(emailTemplateName: string, emailTemplateTitle: string, listEmailTemplateTypeId: Array<string>, listEmailTemplateStatusId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/emailConfig/searchEmailTemplate';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmailTemplateName: emailTemplateName,
        EmailTemplateTitle: emailTemplateTitle,
        ListEmailTemplateTypeId: listEmailTemplateTypeId,
        ListEmailTemplateStatusId: listEmailTemplateStatusId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  sendEmail(type: number, sendEmailModel: SendEmailModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/emailConfig/sendEmail';
    return this.httpClient.post(url, {
      SendType: type,
      SendEmailEntityModel: sendEmailModel
     }).pipe(map((response: Response) => {
      return response;
    }));
  }

  getTokenForEmailTypeId(emailTemplateTypeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/emailConfig/getTokenForEmailTypeId';
    return this.httpClient.post(url, {
      EmailTemplateTypeId: emailTemplateTypeId
     }).pipe(map((response: Response) => {
      return response;
    }));
  }
}
