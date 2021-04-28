
import { share, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeadModel, LeadImportModel, TimeSearchModel } from '../models/lead.model';
import { ContactModel } from '../../shared/models/contact.model';

import { LeadDetailModel, LeadProductDetailProductAttributeValue, leadDetailModel, leadProductDetailProductAttributeValue } from './../models/leadDetail.Model';

interface LeadCareFeedBack {
  leadCareFeedBackId: string,
  feedBackFromDate: Date,
  feedBackToDate: Date,
  feedBackType: string,
  feedBackCode: string,
  feedBackContent: string,
  leadId: string,
  leadCareId: string
}
@Injectable()
export class LeadService {

  constructor(private httpClient: HttpClient) { }
  refreshLead: boolean = false;
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  createLead(lead: LeadModel, contact: ContactModel, isCreateCompany: Boolean, companyName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/create';
    return this.httpClient.post(url, { Lead: lead, Contact: contact, IsCreateCompany: isCreateCompany, CompanyName: companyName }).pipe(
      map((response: Response) => {
        return response;
      }), share());
  }

  getAllLead(contact: ContactModel, lead: LeadModel, noActivePic: Boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getAllLead';
    return this.httpClient.post(url, {
      FirstName: contact.FirstName,
      LastName: contact.LastName,
      Phone: contact.Phone,
      Email: contact.Email,
      PotentialId: lead.PotentialId,
      StatusId: lead.StatusId,
      InterestedGroupId: lead.InterestedGroupId,
      PersonInChargeId: lead.PersonInChargeId,
      NoActivepic: noActivePic,
      UserId: this.userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllLeadAsync(contact: ContactModel, lead: LeadModel, noActivePic: Boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getAllLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        FirstName: contact.FirstName,
        LastName: contact.LastName,
        Phone: contact.Phone,
        Email: contact.Email,
        PotentialId: lead.PotentialId,
        StatusId: lead.StatusId,
        InterestedGroupId: lead.InterestedGroupId,
        PersonInChargeId: lead.PersonInChargeId,
        NoActivepic: noActivePic,
        UserId: this.userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  // Get lead by id
  getLeadById(leadId: string, contactId: string, statusCode: string, potentialCode: string, interestedCode: string, paymentCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getLeadById';
    return this.httpClient.post(url, {
      LeadId: leadId, ContactId: contactId,
      PotentialCode: potentialCode, StatusCode: statusCode,
      InterestedCode: interestedCode, PaymentCode: paymentCode,
      UserId: this.userId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  // Edit lead by id
  editLeadById(lead: LeadModel, contact: ContactModel,
    listInterestedId: Array<string>, listContact: Array<ContactModel>,
    listLeadDetail: Array<LeadDetailModel>,
    listDocumentIdNeedRemove: Array<string>, listDocumentLink: Array<any>,
    isGetNoti: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/editLeadById';
    return this.httpClient.post(url, {
      Lead: lead,
      Contact: contact,
      ListInterestedId: listInterestedId,
      ListContact: listContact,
      ListLeadDetail: listLeadDetail,
      ListDocumentIdNeedRemove: listDocumentIdNeedRemove,
      ListLinkOfDocument: listDocumentLink,
      IsGetNoti: isGetNoti,
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  // Get Note history
  getNoteHistory(leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getNoteHistory';
    return this.httpClient.post(url, { LeadId: leadId }).pipe(map((response: Response) => {
      return response;
    }));
  }

  //Get Unfollow Lead
  getLeadByStatus(statusName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getLeadByStatus';
    return this.httpClient.post(url, {
      StatusName: statusName,
      UserId: this.userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  //Get Lead by Name
  getLeadByName(name: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getLeadByName';
    return this.httpClient.post(url, {
      Name: name,
      UserId: this.userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeWithNotificationPermisison() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getEmployeeWithNotificationPermisison';
    return this.httpClient.post(url, {
      UserId: this.userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeLeadStatusToUnfollow(array: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/changeLeadStatusToUnfollow';
    return this.httpClient.post(url, {
      UserId: this.userId,
      LeadIdList: array
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  approveRejectUnfollowLead(array: Array<any>, isApprove: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/approveRejectUnfollowLead';
    return this.httpClient.post(url, {
      UserId: this.userId,
      LeadIdList: array,
      IsApprove: isApprove
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeManager() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getEmployeeManager';
    return this.httpClient.post(url, {
      UserId: this.userId
    }).pipe(
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

  importLead(fileList: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/importLead';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('FileList', fileList[i]);
    }
    formData.append('UserId', userId);
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  updateLeadDuplicate(LstcontactLeadDuplicate: Array<LeadModel>, LstcontactContactDuplicate: Array<ContactModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/updateLeadDuplicate';
    return this.httpClient.post(url, { lstcontactLeadDuplicate: LstcontactLeadDuplicate, lstcontactContactDuplicate: LstcontactContactDuplicate, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  downloadTemplateLead(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/downloadTemplateLead';
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  checkDuplicateCustomer(email: string, phone: string, leadId: string, isUpdateLead: boolean, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/checkDuplicateCustomerPhoneOrEmail';
    return this.httpClient.post(url, { Email: email, Phone: phone, LeadId: leadId, IsUpdateLead: isUpdateLead, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }

  changeLeadStatusToDelete(leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/changeLeadStatusToDelete';
    return this.httpClient.post(url, {
      UserId: this.userId,
      LeadId: leadId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkPhoneLead(phonenumber: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/checkPhoneLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: this.userId, PhoneNumber: phonenumber
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  checkEmailLead(email: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/checkEmailLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: this.userId, Email: email
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getPersonInCharge() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getPersonInCharge';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  editPersonInCharge(listLeadId: Array<string>, employeeId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/editPersonInCharge';
    return this.httpClient.post(url, {
      ListLeadId: listLeadId,
      EmployeeId: employeeId,
      UserId: this.userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editLeadStatusById(leadId: string, leadStatusCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/editLeadStatusById';
    return this.httpClient.post(url, {
      LeadId: leadId,
      LeadStatusCode: leadStatusCode
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListCustomerByType(userId: string, customerType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getListCustomerByType';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId,
        CustomerType: customerType
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataCreateLead(userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDataCreateLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDatEditLead(leadId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDatEditLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LeadId: leadId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


  createLeadAsync(lead: LeadModel, contact: ContactModel, isCreateCompany: Boolean, companyName: string,
    listInterestedId: Array<string>, listContact: Array<ContactModel>,
    listLeadDetail: Array<LeadDetailModel>
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/create';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Lead: lead, Contact: contact, IsCreateCompany: isCreateCompany, CompanyName: companyName,
        ListInterestedId: listInterestedId,
        ListContact: listContact,
        ListLeadDetail: listLeadDetail
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataSearchLead(userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDataSearchLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchLeadAsync(
    contact: ContactModel,
    listPotentialId: Array<string>,
    listStatusId: Array<string>,
    listInterestedGroupId: Array<string>,
    listPersonInChargeId: Array<string>,
    listLeadTypeId: Array<string>,
    noActivePic: Boolean,
    fromDate: Date,
    toDate: Date,
    waitingForApproval: boolean,
    userId: string,
    listSourceId: Array<string>,
    listAreaId: Array<string>,
    listCusGroupId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getAllLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        FirstName: contact.FirstName,
        LastName: contact.LastName,
        Phone: contact.Phone,
        Email: contact.Email,
        PotentialId: listPotentialId,
        StatusId: listStatusId,
        InterestedGroupId: listInterestedGroupId,
        PersonInChargeId: listPersonInChargeId,
        NoActivepic: noActivePic,
        LeadType: listLeadTypeId,
        FromDate: fromDate,
        ToDate: toDate,
        WaitingForApproval: waitingForApproval,
        UserId: userId,
        ListSourceId: listSourceId,
        ListAreaId: listAreaId,
        ListCusGroupId: listCusGroupId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  approveOrRejectLeadUnfollow(leadLeadId: Array<string>, isApprove: boolean, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/approveOrRejectLeadUnfollow';
    return this.httpClient.post(url, {
      LeadIdList: leadLeadId,
      IsApprove: isApprove,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  setPersonalInChange(userId: string, customerId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/setPersonalInChange';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId,
        customerId: customerId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  unfollowListLead(listLeadId: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/unfollowListLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLeadId: listLeadId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importLeadDetail() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/importLeadDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importListLead(listLead: Array<LeadImportModel>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/importListLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListImportLead: listLead,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataLeadProductDialog() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDataLeadProductDialog';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getVendorByProductId(productId: string, customerGroupId?: string, orderDate?: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getVendorByProductId';

    return this.httpClient.post(url, {
      ProductId: productId,
      CustomerGroupId: customerGroupId,
      OrderDate: orderDate
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  changeLeadStatus(leadId: string, statusId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/changeLeadStatus';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LeadId: leadId,
        StatusId: statusId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getHistoryLeadCareAsync(month: number, year: number, leadId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/getHistoryLeadCare';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Month: month,
        Year: year,
        LeadId: leadId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getHistoryLeadCare(month: number, year: number, leadId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/getHistoryLeadCare';
    return this.httpClient.post(url,
      {
        Month: month,
        Year: year,
        LeadId: leadId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getDataPreviewLeadCare(mode: string, leadId: string, leadCareId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDataPreviewLeadCare';
    return this.httpClient.post(url,
      {
        Mode: mode,
        LeadId: leadId,
        LeadCareId: leadCareId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getDataLeadCareFeedBack(leadCareId: string, leadId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDataLeadCareFeedBack';
    return this.httpClient.post(url,
      {
        LeadId: leadId,
        LeadCareId: leadCareId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }


  saveLeadCareFeedBack(leadCareFeedBack: LeadCareFeedBack) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/saveLeadCareFeedBack';
    return this.httpClient.post(url,
      {
        LeadCareFeedBack: leadCareFeedBack
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getHistoryLeadMeeting(month: number, year: number, leadId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/getHistoryLeadMeeting';
    return this.httpClient.post(url,
      {
        Month: month,
        Year: year,
        LeadId: leadId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getDataLeadMeetingById(leadMeetingId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/getDataLeadMeetintById';
    return this.httpClient.post(url,
      {
        LeadMeetingId: leadMeetingId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  cloneLeadAsync(leadId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/cloneLead';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LeadId: leadId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getEmployeeSale(listEmp: any[], employeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getEmployeeSeller';

    return this.httpClient.post(url, { ListEmployeeByAccount: listEmp, EmployeeId: employeeId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getEmployeeByPersonInCharge(employeeId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getEmployeeByPersonInCharge';

    return this.httpClient.post(url, { EmployeeId: employeeId, UserId: userId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  reportLead(userId: string, reportCode: string, listEmployeeId: Array<string>, listSourceId: Array<string>, listGeographicalAreaId: Array<string>, timeParameter: TimeSearchModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/reportLead';

    return this.httpClient.post(url, { UserId: userId, ReportCode: reportCode, ListEmployeeId: listEmployeeId,
      ListSourceId: listSourceId, ListGeographicalAreaId: listGeographicalAreaId, TimeParameter: timeParameter }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataReportLead(userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getMasterDataReportLead';

    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  changeLeadStatusSupport(leadId: string, statusSupportId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/changeLeadStatusSupport';
    return this.httpClient.post(url,
      {
        LeadId: leadId,
        StatusSupportId: statusSupportId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }
}
