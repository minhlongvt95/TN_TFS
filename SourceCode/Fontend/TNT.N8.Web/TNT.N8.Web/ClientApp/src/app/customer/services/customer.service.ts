
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerModel } from "../models/customer.model";
import { ContactModel } from "../../shared/models/contact.model";

interface CustomerCareFeedBack {
  customerCareFeedBackId: string,
  feedBackFromDate: Date,
  feedBackToDate: Date,
  feedBackType: string,
  feedBackCode: string,
  feedBackContent: string,
  customerId: string,
  customerCareId: string
}

interface CustomerMeeting {
  customerMeetingId: string,
  customerId: string,
  employeeId: string,
  title: string,
  locationMeeting: string,
  startDate: Date,
  startHours: Date,
  content: string,
}

@Injectable()
export class CustomerService {

  constructor(private httpClient: HttpClient) { }

  createCustomer(cus: CustomerModel, contact: ContactModel, customerContactList: Array<ContactModel>, userId: string, createByLead: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/createCustomer";
    return this.httpClient.post(url, { Customer: cus, Contact: contact, CustomerContactList: customerContactList, UserId: userId, CreateByLead: createByLead }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createCustomerAsync(cus: CustomerModel, contact: ContactModel, customerContactList: Array<ContactModel>, userId: string, createByLead: boolean, IsFromLead: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/createCustomer";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Customer: cus,
        Contact: contact,
        CustomerContactList: customerContactList,
        UserId: userId,
        CreateByLead: createByLead,
        IsFromLead: IsFromLead

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchCustomer(
    noPic: boolean,
    isBusinessCus: boolean,
    isPersonalCus: boolean,
    statusId: string,
    customerGroupIdList: Array<string>,
    personInChargeIdList: Array<string>,
    // sourceId: Array<string>,
    area: string,
    fromDate: Date,
    toDate: Date,
    firstName: string,
    lastName: string,
    phone: string,
    email: string,
    address: string,
    // customerServiceLevelIdList: Array<string>,
    // isFreeCus : boolean ,
    // isIdentificationCus : boolean,
    // isHKDCus:boolean ,
    // customerCode: string,
    // taxCode: string,
    userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/searchCustomer";
    return this.httpClient.post(url, {
      NoPic: noPic,
      IsBusinessCus: isBusinessCus,
      IsPersonalCus: isPersonalCus,
      StatusCareId: statusId,
      CustomerGroupIdList: customerGroupIdList,
      PersonInChargeIdList: personInChargeIdList,
      // SourceId: sourceId,
      AreaId: area,
      FromDate: fromDate,
      ToDate: toDate,
      FirstName: firstName,
      LastName: lastName,
      Phone: phone,
      Email: email,
      Address: address,
      UserId: userId
      // CustomerServiceLevelIdList: customerServiceLevelIdList,
      // IsIdentificationCus : isIdentificationCus,
      // IsFreeCus : isFreeCus,
      // IsHKDCus: isHKDCus,
      // CustomerCode: customerCode,
      // TaxCode: taxCode,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchCustomerApproval(firstName: string, lastName: string, phone: string, email: string, customerServiceLevelIdList: Array<string>,
    customerGroupIdList: Array<string>, personInChargeIdList: Array<string>, noPic: boolean,
    isBusinessCus: boolean, isPersonalCus: boolean, isIdentificationCus: boolean, isFreeCus: boolean, isHKDCus: boolean, customerCode: string, taxCode: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/searchCustomerApproval";
    return this.httpClient.post(url, {
      firstName: firstName, LastName: lastName, Phone: phone, Email: email, CustomerServiceLevelIdList: customerServiceLevelIdList,
      CustomerGroupIdList: customerGroupIdList, PersonInChargeIdList: personInChargeIdList,
      NoPic: noPic, IsBusinessCus: isBusinessCus, IsPersonalCus: isPersonalCus, IsIdentificationCus: isIdentificationCus, IsFreeCus: isFreeCus
      , IsHKDCus: isHKDCus, CustomerCode: customerCode, TaxCode: taxCode, UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerFromOrderCreate(firstName: string, lastName: string, phone: string, email: string, customerServiceLevelIdList: Array<string>,
    customerGroupIdList: Array<string>, personInChargeIdList: Array<string>, noPic: boolean,
    isBusinessCus: boolean, isPersonalCus: boolean, isHKDCus: boolean, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/getCustomerFromOrderCreate";
    return this.httpClient.post(url, {
      firstName: firstName, LastName: lastName, Phone: phone, Email: email, CustomerServiceLevelIdList: customerServiceLevelIdList,
      CustomerGroupIdList: customerGroupIdList, PersonInChargeIdList: personInChargeIdList,
      NoPic: noPic, IsBusinessCus: isBusinessCus, IsPersonalCus: isPersonalCus, IsHKDCus: isHKDCus, UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllCustomerServiceLevel(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/getAllCustomerServiceLevel";
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerById(customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/getCustomerById";
    return this.httpClient.post(url, { CustomerId: customerId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerByIdAsync(customerId: string, contactId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/getCustomerById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CustomerId: customerId, ContactId: contactId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  editCustomerById(cus: CustomerModel, contact: ContactModel, customerContactList: Array<ContactModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/editCustomerById";
    return this.httpClient.post(url, { Customer: cus, Contact: contact, ContactList: customerContactList, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getNoteHistory(leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/getNoteHistory';
    return this.httpClient.post(url, { LeadId: leadId }).pipe(map((response: Response) => {
      return response;
    }));
  }

  getNoteHistoryAsync(leadId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/lead/getNoteHistory";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { LeadId: leadId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllCustomer() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/getAllCustomer';
    return this.httpClient.post(url, {}).pipe(map((response: Response) => {
      return response;
    }));
  }

  getAllCustomerAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/getAllCustomer";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  changeCustomerStatusToDelete(customerId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/changeCustomerStatusToDelete';
    return this.httpClient.post(url, { CustomerId: customerId }).pipe(map((response: Response) => {
      return response;
    }));
  }

  quickCreateCustomer(userId: string, customerType: number, customerCode: string, customerGroupId: string,
    paymentId: string, customerName: string, maximumDebtValue: number, maximumDebtDays: number, createdDate: Date) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/quickCreateCustomer";
    return this.httpClient.post(url, {
      UserId: userId, CustomerType: customerType, CustomerCode: customerCode, CustomerGroupId: customerGroupId,
      PaymentId: paymentId, CustomerName: customerName, MaximumDebtValue: maximumDebtValue, MaximumDebtDays: maximumDebtDays, CreatedDate: createdDate
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  searchCustomerAsync(firstName: string, lastName: string, phone: string, email: string, customerServiceLevelIdList: Array<string>,
    customerGroupIdList: Array<string>, personInChargeIdList: Array<string>, noPic: boolean,
    isBusinessCus: boolean, isPersonalCus: boolean, isHKDCus: boolean, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/searchCustomer";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        firstName: firstName, LastName: lastName, Phone: phone, Email: email, CustomerServiceLevelIdList: customerServiceLevelIdList,
        CustomerGroupIdList: customerGroupIdList, PersonInChargeIdList: personInChargeIdList,
        NoPic: noPic, IsBusinessCus: isBusinessCus, IsPersonalCus: isPersonalCus, IsHKDCus: isHKDCus, UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllCustomerCode(userId: string, mode: string, code: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getAllCustomerCode';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: userId, Mode: mode, Code: code }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  importCustomer(fileList: File[], customerType: number, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/importCustomer';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('FileList', fileList[i]);
    }
    formData.append('CustomerType', customerType.toString());
    formData.append('UserId', userId);
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  downloadTemplateCustomer(customerType: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/downloadTemplateCustomer';
    return this.httpClient.post(url, { CustomerType: customerType, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  updateCustomerDuplicate(LstcontactCustomerDuplicate: Array<CustomerModel>, LstcontactContactDuplicate: Array<ContactModel>, LstcontactContact_CON_Duplicate: Array<ContactModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/updateCustomerDuplicate';
    return this.httpClient.post(url, { lstcontactCustomerDuplicate: LstcontactCustomerDuplicate, lstcontactContactDuplicate: LstcontactContactDuplicate, lstcontactContact_CON_Duplicate: LstcontactContact_CON_Duplicate, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getStatisticCustomerForDashboard(keyName: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/getStatisticCustomerForDashboard';
    return this.httpClient.post(url, { KeyName: keyName, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  getListCustomeSaleToprForDashboard(keyName: string, month: number, year: number, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/getListCustomeSaleToprForDashboard';
    return this.httpClient.post(url, { KeyName: keyName, Month: month, Year: year, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  checkDuplicateCustomer(leadId: string, email: string, phone: string, taxcode: string, customerType: number, userId: string, checkByEmail: boolean, checkByPhone: boolean, checkCreateForm: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/checkDuplicateCustomer';
    return this.httpClient.post(url, { LeadId: leadId, Email: email, Phone: phone, Taxcode: taxcode, CustomerType: customerType, UserId: userId, CheckByEmail: checkByEmail, CheckByPhone: checkByPhone, CheckCreateForm: checkCreateForm }).pipe(map((response: Response) => {
      return response;
    }));
  }
  checkDuplicatePersonalCustomer(cus: CustomerModel, contact: ContactModel, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/checkDuplicatePersonalCustomer';
    return this.httpClient.post(url, { Customer: cus, Contact: contact, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  checkDuplicatePersonalCustomerByEmailOrPhone(leadId: string, email: string, phone: string, userId: string, checkByPhone: boolean, checkByWorkPhone: boolean, checkByOtherPhone: boolean, checkByEmail: boolean, checkByWorkEmail: boolean, checkByOtherEmail: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/checkDuplicatePersonalCustomerByEmailOrPhone';
    return this.httpClient.post(url, { LeadId: leadId, Email: email, Phone: phone, UserId: userId, CheckByPhone: checkByPhone, CheckByWorkPhone: checkByWorkPhone, CheckByOtherPhone: checkByOtherPhone, CheckByEmail: checkByEmail, CheckByWorkEmail: checkByWorkEmail, CheckByOtherEmail: checkByOtherEmail }).pipe(map((response: Response) => {
      return response;
    }));
  }
  getAllCustomerAdditionalByCustomerId(customerId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getAllCustomerAdditionalByCustomerId';
    return this.httpClient.post(url, { CustomerId: customerId, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  getAllCustomerAdditionalByCustomerIdAsync(customerId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getAllCustomerAdditionalByCustomerId';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, { CustomerId: customerId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  createCustomerAdditional(customerAdditionalInformationId: string, question: string, answer: string, customerId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/createCustomerAdditional';
    return this.httpClient.post(url, {
      CustomerAdditionalInformationId: customerAdditionalInformationId,
      Question: question, Answer: answer, CustomerId: customerId, UserId: userId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  deleteCustomerAdditional(customerAdditionalInformationId: string, customerId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/deleteCustomerAdditional';
    return this.httpClient.post(url, {
      CustomerAdditionalInformationId: customerAdditionalInformationId,
      CustomerId: customerId,
      UserId: userId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  deleteListCustomerAdditional(listCusAddInfId: Array<string>, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/deleteListCustomerAdditional';
    return this.httpClient.post(url, {
      ListCusAddInfId: listCusAddInfId,
      CustomerId: customerId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }
  editCustomerAdditional(customerAdditionalInformationId: string, question: string, answer: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/editCustomerAdditional';
    return this.httpClient.post(url, { CustomerAdditionalInformationId: customerAdditionalInformationId, Question: question, Answer: answer, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  createListQuestion(customerId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/createListQuestion';
    return this.httpClient.post(url, { CustomerId: customerId, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  searchQuestionAnswer(textSearch: string, customerId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getListQuestionAnswerBySearch';
    return this.httpClient.post(url, { TextSearch: textSearch, CustomerId: customerId, UserId: userId }).pipe(map((response: Response) => {
      return response;
    }));
  }
  getAllHistoryProductByCustomerIdAsync(customerId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getAllHistoryProductByCustomerId';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, { CustomerId: customerId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDashBoardCustomer(customerName: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDashBoardCustomer';
    return this.httpClient.post(url,
      {
        CustomerName: customerName
      }).pipe(map((response: Response) => {
        return response;
      }));
  }
  /* api/customer/getListCustomer */
  getListCustomerAsync(CategoryTypeCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getListCustomer';
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        CategoryTypeCode: CategoryTypeCode
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  /* api/customer/createCustomerMasterData */
  createCustomerMasterDataAsync(EmployeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/createCustomerMasterData';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  /* api/customer/checkDuplicateCustomerAllType */
  checkDuplicateCustomerAllType(contactModel: ContactModel, isCheckOnSave, isCheckedLead, employeeId: string, customerType: any) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/checkDuplicateCustomerAllType';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerType: customerType,
        ContactModel: contactModel,
        IsCheckOnSave: isCheckOnSave,
        IsCheckedLead: isCheckedLead,
        EmployeeId: employeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateCustomerById(customerModel: CustomerModel, contactModel: ContactModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/updateCustomerById';
    return this.httpClient.post(url,
      {
        CustomerModel: customerModel,
        ContactModel: contactModel
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  approvalOrRejectCustomer(listCusId: Array<string>, isApprove: boolean, isFressCus: boolean, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/approvalOrRejectCustomer';
    return this.httpClient.post(url,
      {
        ListCustomerId: listCusId,
        IsApproval: isApprove,
        IsFreeCus: isFressCus,
        UserId: userId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  sendApprovalCustomer(customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/sendApprovalCustomer';
    return this.httpClient.post(url,
      {
        CustomerId: customerId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getHistoryCustomerCare(month: number, year: number, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getHistoryCustomerCare';
    return this.httpClient.post(url,
      {
        Month: month,
        Year: year,
        CustomerId: customerId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getDataPreviewCustomerCare(mode: string, customerId: string, customerCareId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataPreviewCustomerCare';
    return this.httpClient.post(url,
      {
        Mode: mode,
        CustomerId: customerId,
        CustomerCareId: customerCareId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getDataCustomerCareFeedBack(customerCareId: string, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataCustomerCareFeedBack';
    return this.httpClient.post(url,
      {
        CustomerId: customerId,
        CustomerCareId: customerCareId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  saveCustomerCareFeedBack(customerCareFeedBack: CustomerCareFeedBack) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/saveCustomerCareFeedBack';
    return this.httpClient.post(url,
      {
        CustomerCareFeedBack: customerCareFeedBack
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getCustomerImportDetaiAsync() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getCustomerImportDetai';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importListCustomer(listCustomer: Array<CustomerModel>, listContact: Array<ContactModel>, listContactAdditional: Array<ContactModel>, isPotentialCustomer: boolean, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/importListCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListCustomer: listCustomer,
        ListContact: listContact,
        ListContactAdditional: listContactAdditional,
        IsPotentialCustomer: isPotentialCustomer,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataCustomerMeetingById(customerMeetingId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataCustomerMeetingById';
    return this.httpClient.post(url,
      {
        CustomerMeetingId: customerMeetingId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  createCustomerMeeting(customerMeeting: CustomerMeeting) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/createCustomerMeeting';
    return this.httpClient.post(url,
      {
        CustomerMeeting: customerMeeting
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getHistoryCustomerMeeting(month: number, year: number, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getHistoryCustomerMeeting';
    return this.httpClient.post(url,
      {
        Month: month,
        Year: year,
        CustomerId: customerId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getDataCreatePotentialCustomer(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataCreatePotentialCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataDetailPotentialCustomer(CustomerId: string, UserId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataDetailPotentialCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerId,
        UserId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updatePotentialCustomer(
    CustomerModel: any,
    ContactModel: any,
    listDocumentIdNeedRemove: Array<string>,
    listDocumentLink: Array<any>,
    listCustomerProduct: Array<any>,
    listContact: Array<any>,
    UserId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/updatePotentialCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerModel,
        ContactModel,
        ListDocumentIdNeedRemove: listDocumentIdNeedRemove,
        ListLinkOfDocument: listDocumentLink,
        ListCustomerProduct: listCustomerProduct,
        ListContact: listContact,
        UserId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataSearchPotentialCustomer(EmployeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataSearchPotentialCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchPotentialCustomer(searchPotentialCustomerModel: any, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/searchPotentialCustomer";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        FullName: searchPotentialCustomerModel.FullName || "",
        Phone: searchPotentialCustomerModel.Phone || "",
        Email: searchPotentialCustomerModel.Email || "",
        Adress: searchPotentialCustomerModel.Adress || "",
        InvestmentFundId: searchPotentialCustomerModel.InvestmentFundId || [],
        PersonInChargeId: searchPotentialCustomerModel.PersonInChargeId || [],
        ListCareStateId: searchPotentialCustomerModel.ListCareStateId || [],
        ListCusTypeId: searchPotentialCustomerModel.ListCusTypeId || [],
        ListCusGroupId: searchPotentialCustomerModel.ListCusGroupId || [],
        ListAreaId: searchPotentialCustomerModel.ListAreaId || [],
        IsConverted: searchPotentialCustomerModel.IsConverted,
        StartDate: searchPotentialCustomerModel.StartDate,
        EndDate: searchPotentialCustomerModel.EndDate,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataDashboardPotentialCustomer(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/getDataDashboardPotentialCustomer";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  convertPotentialCustomer(CustomerId: string, IsCreateCustomer: boolean, IsCreateLead: boolean,
    PersonalInChargeId: string, LeadName: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customer/convertPotentialCustomer";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerId, IsCreateCustomer, IsCreateLead, PersonalInChargeId, LeadName: LeadName
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  DownloadTemplatePotentialCustomerAsync() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/downloadTemplatePotentialCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataImportPotentialCustomer(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/getDataImportPotentialCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateImportCustomer(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/downloadTemplateImportCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchContactCustomer(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/searchContactCustomer';
    return this.httpClient.post(url,
      {
        UserId: userId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  checkDuplicateInforCustomer(customerId: string, checkType: number, email: string, phone: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/checkDuplicateInforCustomer';
    return this.httpClient.post(url,
      {
        CustomerId: customerId,
        CheckType: checkType,
        Email: email,
        phone: phone
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  changeStatusSupport(customerId: string, statusSupportId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/customer/changeStatusSupport';
    return this.httpClient.post(url,
      {
        CustomerId: customerId,
        StatusSupportId: statusSupportId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

}
