import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AdditionalInformationModel } from "../../shared/models/additional-information.model";
import { Quote } from "../models/quote.model";
import { QuoteDetail } from "../models/quote-detail.model";
import { QuoteCostDetail } from '../models/quote-cost-detail.model';
import { PromotionObjectApply } from '../../promotion/models/promotion-object-apply.model';

@Injectable()
export class QuoteService {

  constructor(private httpClient: HttpClient) { }

  CreateQuote(quote: Quote, quoteDetail: Array<QuoteDetail>, typeAccount: number, fileList: Array<any>, listAdditionalInformation: Array<AdditionalInformationModel>,quoteCostDetail: Array<QuoteCostDetail>,
    isClone: boolean, quoteIdClone: string, listParticipant: Array<string>, listPromotionObjectApply: Array<PromotionObjectApply>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/createQuote';
    return this.httpClient.post(url, {
      Quote: quote,
      QuoteDetail: quoteDetail,
      QuoteCostDetail: quoteCostDetail,
      TypeAccount: typeAccount,
      FileList: fileList,
      isClone: isClone,
      QuoteIdClone: quoteIdClone,
      ListAdditionalInformation: listAdditionalInformation,
      ListParticipant: listParticipant,
      ListPromotionObjectApply: listPromotionObjectApply
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  UpdateQuote(quote: Quote, quoteDetail: Array<QuoteDetail>, typeAccount: number, fileList: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateQuote';
    return this.httpClient.post(url, {
      Quote: quote,
      QuoteDetail: quoteDetail,
      TypeAccount: typeAccount,
      FileList: fileList,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetQuoteByID(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getQuoteByID';
    return this.httpClient.post(url, {
      QuoteId: quoteId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetAllQuote(quoteCode: string, productCode: string,quoteStatusId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getAllQuote';

    return this.httpClient.post(url,
      {
        QuoteCode: quoteCode,
        ProductCode: productCode,
        QuoteStatusId: quoteStatusId,
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchCustomerAsync(firstName: string, lastName: string, phone: string, email: string, customerServiceLevelIdList: Array<string>,
    customerGroupIdList: Array<string>, personInChargeIdList: Array<string>, noPic: boolean,
   isBusinessCus: boolean, isPersonalCus: boolean, isHKDCus:boolean ,userId: string) {
     let url = localStorage.getItem('ApiEndPoint') + "/api/customer/searchCustomer";
     return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        firstName: firstName, LastName: lastName, Phone: phone, Email: email, CustomerServiceLevelIdList: customerServiceLevelIdList,
        CustomerGroupIdList: customerGroupIdList, PersonInChargeIdList: personInChargeIdList,
        NoPic: noPic, IsBusinessCus: isBusinessCus, IsPersonalCus: isPersonalCus, IsHKDCus: isHKDCus ,UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
     });
 }

  GetTop3QuotesOverdue(personInChangeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTop3QuotesOverdue';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  GetTop3WeekQuotesOverdue(personInChangeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTop3WeekQuotesOverdue';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  GetTop3PotentialCustomers(personInChangeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTop3PotentialCustomers';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  GetTotalAmountQuote(personInChangeId: string, month: number, year: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTotalAmountQuote';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId,
        MonthQuote: month,
        YearQuote: year
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }
  GetDashBoardQuote(personInChangeId: string, month: number, year: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDashBoardQuote';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId,
        MonthQuote: month,
        YearQuote: year
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataQuoteToPieChart(month: number, year: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataQuoteToPieChart';

    return this.httpClient.post(url,
    {
      MonthQuote: month,
      YearQuote: year
    }).pipe(
    map((response: Response) => {
      return <any>response;
    }));
  }

  updateActiveQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateActiveQuote';

    return this.httpClient.post(url,
      {
        QuoteId: quoteId
      }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchQuote(quoteCode: string, startDate: Date, endDate: Date, listStatusQuote: Array<string>,
                isOutOfDate: boolean, isCompleteInWeek: boolean, quoteName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/searchQuote';

    return this.httpClient.post(url,
    {
      QuoteCode: quoteCode,
      StartDate: startDate,
      EndDate: endDate,
      ListStatusQuote: listStatusQuote,
      IsOutOfDate: isOutOfDate,
      QuoteName: quoteName,
      IsCompleteInWeek: isCompleteInWeek
    }).pipe(
    map((response: Response) => {
      return <any>response;
    }));
  }

  searchQuoteApproval(quoteCode: string, startDate: Date, endDate: Date, listStatusQuote: Array<string>,
    isOutOfDate: boolean, isCompleteInWeek: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/searchQuoteApproval';

    return this.httpClient.post(url,
    {
    QuoteCode: quoteCode,
    StartDate: startDate,
    EndDate: endDate,
    ListStatusQuote: listStatusQuote,
    IsOutOfDate: isOutOfDate,
    IsCompleteInWeek: isCompleteInWeek
    }).pipe(
    map((response: Response) => {
    return <any>response;
    }));
  }

  getDataCreateUpdateQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataCreateUpdateQuote';

    return this.httpClient.post(url, { QuoteId: quoteId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getEmployeeSale(listEmp: any[], employeeId: string){
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getEmployeeSale';

    return this.httpClient.post(url, { ListEmployeeByAccount: listEmp, EmployeeId: employeeId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  downloadTemplateProduct(){
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/downloadTemplateProduct';

    return this.httpClient.post(url, {  }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getMasterDataCreateCost(userId) {
    const url = localStorage.getItem('ApiEndPoint') + "/api/quote/getMasterDataCreateCost";
    return this.httpClient.post(url, {UserId: userId}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataQuoteAddEditProductDialog() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataQuoteAddEditProductDialog';

    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getDataQuoteAddEditProductDialogAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataQuoteAddEditProductDialog';

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { })
        .toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getVendorByProductId(productId: string, orderDate?: Date, customerGroupId?: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getVendorByProductId';

    return this.httpClient.post(url, { ProductId: productId, OrderDate: orderDate, CustomerGroupId: customerGroupId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  updateStatusQuote(quoteId: string, userId: string, objectType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateStatusQuote';

    return this.httpClient.post(url, { QuoteId: quoteId, UserId: userId, ObjectType: objectType }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  sendEmailCustomerQuote(lstEmail: string[], titleEmail: string, contentEmail: string, quoteId: string, base64: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/sendEmailCustomerQuote';

    return this.httpClient.post(url, { ListEmail: lstEmail, TitleEmail: titleEmail, ContentEmail: contentEmail, QuoteId: quoteId, Base64Pdf: base64 }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  approvalOrRejectQuote(listQuoteId: string[], isApproval: boolean, userId: string, description: string, rejectReason: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/approvalOrRejectQuote';

    return this.httpClient.post(url, { ListQuoteId: listQuoteId, IsApproval: isApproval, UserId: userId, Description: description, RejectReason: rejectReason }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getDataExportExcelQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataExportExcelQuouter';
    return this.httpClient.post(url, { QuoteId: quoteId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getMasterDataCreateQuote(objectId: string, objectType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getMasterDataCreateQuote';

    return this.httpClient.post(url, { ObjectId: objectId, ObjectType: objectType }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getEmployeeByPersonInCharge(employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/quote/getEmployeeByPersonInCharge";

    return new Promise((resolve, reject) => {
    return this.httpClient.post(url, { EmployeeId: employeeId })
      .toPromise()
      .then((response: Response) => {
        resolve(response);
      });
    });
  }

  getMasterDataUpdateQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getMasterDataUpdateQuote';

    return this.httpClient.post(url, { QuoteId: quoteId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }
}
