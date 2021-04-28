
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CashReceiptModel } from '../models/cashReceipt.model';
import { CashPaymentModel } from '../models/cashPayment.model';
import { CashPaymentMappingModel } from '../models/cashPaymentMapping.model';
import { CashReceiptMappingModel } from '../models/cashReceiptMapping.model';
import { BankPaymentModel } from '../models/bankPayment.model';
import { BankPaymentMappingModel } from '../models/bankPaymentMapping.model';
import { BankReceiptModel } from '../models/bankReceipt.model';
import { BankReceiptMappingModel } from '../models/bankReceiptMapping.model';

@Injectable()
export class AccountingService {
  constructor(private httpClient: HttpClient) { }

  createPayableInvoice(invoice: CashPaymentModel, payableInvoiceMapping: CashPaymentMappingModel, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/create';
    return this.httpClient.post(url,
      {
        PayableInvoice: invoice,
        PayableInvoiceMapping: payableInvoiceMapping,
        UserId: userId
      }).pipe(
        map((response: Response) => {
          return response;
        }));
  }

  createReceiptInvoice(invoice: CashReceiptModel, receiptInvoiceMapping: CashReceiptMappingModel, userId: string, listOrder: any[]) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/create';
    return this.httpClient.post(url, { ReceiptInvoice: invoice, ReceiptInvoiceMapping: receiptInvoiceMapping, UserId: userId, ReceiptOrderHistory: listOrder }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createBankReceiptInvoice(invoice: BankReceiptModel, receiptInvoiceMapping: BankReceiptMappingModel, userId: string, listOrder: any[]) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/createBankReceiptInvoice';
    return this.httpClient.post(url, { BankReceiptInvoice: invoice, BankReceiptInvoiceMapping: receiptInvoiceMapping, UserId: userId, ReceiptOrderHistory: listOrder }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  searchBankReceiptInvoice(userId: string, code: string, receiptReasonIdList: Array<string>, createdByIdList: Array<string>,
    fromDate: Date, toDate: Date, sttList: Array<string>, objectLst: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchBankReceiptInvoice';

    return this.httpClient.post(url, {
      UserId: userId, ReceiptInvoiceCode: code, ReceiptReasonIdList: receiptReasonIdList,
      CreatedByIdList: createdByIdList, FromDate: fromDate, ToDate: toDate, SttList: sttList, ObjectIdList: objectLst
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchBankReceiptInvoiceAsync(userId: string, code: string, receiptReasonIdList: Array<string>, createdByIdList: Array<string>,
    fromDate: Date, toDate: Date, sttList: Array<string>, objectLst: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchBankReceiptInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId, ReceiptInvoiceCode: code, ReceiptReasonIdList: receiptReasonIdList,
        CreatedByIdList: createdByIdList, FromDate: fromDate, ToDate: toDate, SttList: sttList, ObjectIdList: objectLst
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  searchPayableInvoice(userId: string, code: string, payableReasonIdList: Array<string>, createdByIdList: Array<string>,
    fromDate: Date, toDate: Date, sttList: Array<string>, objectLst: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchPayableInvoice';

    return this.httpClient.post(url, {
      UserId: userId, PayableInvoiceCode: code, PayableReasonIdList: payableReasonIdList,
      CreatedByIdList: createdByIdList, FromDate: fromDate, ToDate: toDate, SttList: sttList, ObjectIdList: objectLst
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchPayableInvoiceAsync(userId: string, code: string, payableReasonIdList: Array<string>, createdByIdList: Array<string>,
    fromDate: Date, toDate: Date, sttList: Array<string>, objectLst: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchPayableInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId, PayableInvoiceCode: code, PayableReasonIdList: payableReasonIdList,
        CreatedByIdList: createdByIdList, FromDate: fromDate, ToDate: toDate, SttList: sttList, ObjectIdList: objectLst
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchCashBookPayableInvoiceAsync(userId: string, createdByIdList: Array<string>,
    fromPaidDate: Date, toPaidDate: Date, organizationList: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchCashBookPayableInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId,
        CreatedByIdList: createdByIdList,
        FromPaidDate: fromPaidDate,
        ToPaidDate: toPaidDate,
        OrganizationList: organizationList
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchCashBookPayableInvoice(userId: string, createdByIdList: Array<string>,
    fromPaidDate: Date, toPaidDate: Date, organizationList: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchCashBookPayableInvoice';
    return this.httpClient.post(url, {
      UserId: userId,
      CreatedByIdList: createdByIdList,
      FromPaidDate: fromPaidDate,
      ToPaidDate: toPaidDate,
      OrganizationList: organizationList
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getPayableInvoiceById(payableInvoiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/getPayableInvoiceById';
    return this.httpClient.post(url, { PayableInvoiceId: payableInvoiceId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getReceiptInvoiceById(receiptInvoiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/getReceiptInvoiceById';
    return this.httpClient.post(url, { ReceiptInvoiceId: receiptInvoiceId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchReceiptInvoice(receiptInvoiceCode: string, receiptInvoiceReason: Array<string>,
    createById: Array<string>, createDateFrom: Date, createDateTo: Date, status: Array<string>, objectReceipt: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchReceiptInvoice';
    return this.httpClient.post(url, {
      ReceiptInvoiceCode: receiptInvoiceCode,
      ReceiptInvoiceReason: receiptInvoiceReason,
      CreateById: createById,
      CreateDateFrom: createDateFrom,
      CreateDateTo: createDateTo,
      Status: status,
      ObjectReceipt: objectReceipt,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchReceiptInvoiceAsync(receiptInvoiceCode: string, receiptInvoiceReason: Array<string>,
    createById: Array<string>, createDateFrom: Date, createDateTo: Date, status: Array<string>, objectReceipt: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchReceiptInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ReceiptInvoiceCode: receiptInvoiceCode,
        ReceiptInvoiceReason: receiptInvoiceReason,
        CreateById: createById,
        CreateDateFrom: createDateFrom,
        CreateDateTo: createDateTo,
        Status: status,
        ObjectReceipt: objectReceipt,
        UserId: userId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchCashBookReceiptInvoiceAsync(createById: Array<string>, receiptDateFrom: Date, receiptDateTo: Date, organizationList: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchCashBookReceiptInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CreateById: createById,
        ReceiptDateFrom: receiptDateFrom,
        ReceiptDateTo: receiptDateTo,
        OrganizationList: organizationList,
        UserId: userId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchCashBookReceiptInvoice(createById: Array<string>, receiptDateFrom: Date, receiptDateTo: Date, organizationList: Array<string>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchCashBookReceiptInvoice';
    return this.httpClient.post(url, {
      CreateById: createById,
      ReceiptDateFrom: receiptDateFrom,
      ReceiptDateTo: receiptDateTo,
      OrganizationList: organizationList,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Get report receivable vendor
  searchReceivableVendorReport(createdDateTo: Date, codeVendor: Array<string>, vendorName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receivableVendor/searchReceivableVendorReport';
    return this.httpClient.post(url, {
      ReceivalbeDateTo: createdDateTo,
      VendorCode: codeVendor,
      VendorName: vendorName
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Get detail receivable vendor
  searchReceivableVendorDetail(vendorId: string, createdDateFrom: Date, createdDateTo: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receivableVendor/searchReceivableVendorDetail';
    return this.httpClient.post(url, {
      VendorId: vendorId,
      ReceivalbeDateFrom: createdDateFrom,
      ReceivalbeDateTo: createdDateTo,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Get report receivable customer
  searchReceivableCustomerReport(customerNameOrCustomerCode: string, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receivableCustomer/searchReceivableCustomerReport';
    return this.httpClient.post(url, {
      CustomerNameOrCustomerCode: customerNameOrCustomerCode,
      FromDate: fromDate,
      ToDate: toDate
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Get detail receivable customer
  searchReceivableCustomerDetail(customerId: any, receivalbeDateFrom: Date, receivalbeDateTo: Date): any {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receivableCustomer/searchReceivableCustomerDetail';
    return this.httpClient.post(url, {
      CustomerId: customerId,
      FromDate: receivalbeDateFrom,
      ToDate: receivalbeDateTo
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createBankPayableInvoice(invoice: BankPaymentModel, payableInvoiceMapping: BankPaymentMappingModel, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/createBankPayableInvoice';
    return this.httpClient.post(url, { BankPayableInvoice: invoice, BankPayableInvoiceMapping: payableInvoiceMapping, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchBankPayableInvoice(userId: string, code: string, payableReasonIdList: Array<string>, createdByIdList: Array<string>,
    fromDate: Date, toDate: Date, sttList: Array<string>, objectLst: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchBankPayableInvoice';

    return this.httpClient.post(url, {
      UserId: userId, PayableInvoiceCode: code, PayableReasonIdList: payableReasonIdList,
      CreatedByIdList: createdByIdList, FromDate: fromDate, ToDate: toDate, SttList: sttList, ObjectIdList: objectLst
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchBankPayableInvoiceAsync(userId: string, code: string, payableReasonIdList: Array<string>, createdByIdList: Array<string>,
    fromDate: Date, toDate: Date, sttList: Array<string>, objectLst: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchBankPayableInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId, PayableInvoiceCode: code, PayableReasonIdList: payableReasonIdList,
        CreatedByIdList: createdByIdList, FromDate: fromDate, ToDate: toDate, SttList: sttList, ObjectIdList: objectLst
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchBankBookPayableInvoice(userId: string, fromPaidDate: Date, toPaidDate: Date, bankAccountId: Array<string>, listCreateById: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchBankBookPayableInvoice';
    return this.httpClient.post(url, {
      UserId: userId, FromPaidDate: fromPaidDate, ToPaidDate: toPaidDate, BankAccountId: bankAccountId, ListCreateById: listCreateById
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchBankBookPayableInvoiceAsync(userId: string, fromPaidDate: Date, toPaidDate: Date, bankAccountId: Array<string>, listCreateById: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/searchBankBookPayableInvoice';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId, FromPaidDate: fromPaidDate, ToPaidDate: toPaidDate, BankAccountId: bankAccountId, ListCreateById: listCreateById
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  searchBankBookReceipt(userId: string, fromPaidDate: Date, toPaidDate: Date, bankAccountId: Array<string>, listCreateById: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchBankBookReceipt';
    return this.httpClient.post(url, {
      UserId: userId, FromPaidDate: fromPaidDate, ToPaidDate: toPaidDate, BankAccountId: bankAccountId, ListCreateById: listCreateById
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  searchBankBookReceiptAsync(userId: string, fromPaidDate: Date, toPaidDate: Date, bankAccountId: Array<string>, listCreateById: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/searchBankBookReceipt';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId, FromPaidDate: fromPaidDate, ToPaidDate: toPaidDate, BankAccountId: bankAccountId, ListCreateById: listCreateById
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getBankPayableInvoiceById(bankPayableInvoiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/getBankPayableInvoiceById';
    return this.httpClient.post(url, { BankPayableInvoiceId: bankPayableInvoiceId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getBankReceiptInvoiceById(bankReceiptInvoiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/getBankReceiptInvoice';
    return this.httpClient.post(url, { BankReceiptInvoiceId: bankReceiptInvoiceId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Export Pdf receipt invoice
  exportPdfReceiptInvoice(id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/exportPdfReceiptInvoice';
    return this.httpClient.post(url, { ReceiptInvoiceId: id }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  exportBankPayableInvoice(id: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/exportBankPayableInvoice';
    return this.httpClient.post(url, { BankPayableInvId: id, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  exportPayableInvoice(id: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/payableInvoice/exportPayableInvoice';
    return this.httpClient.post(url, { PayableInvoiceId: id, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  exportBankReceiptInvoice(id: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/exportBankReceiptInvoice';
    return this.httpClient.post(url, { BankReceiptInvoiceId: id }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchSalesReport(userId: string, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salesReport/searchSalesReport';
    return this.httpClient.post(url, { UserId: userId, FromMonth: fromDate, ToMonth: toDate }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getOrderByCustomerId(userId: string, customerId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/getOrderByCustomerId';
    return this.httpClient.post(url, { CustomerId: customerId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getOrderByCustomerIdAsync(userId: string, customerId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receiptInvoice/getOrderByCustomerId';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CustomerId: customerId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  //Export Excel
  exportExcelReceivableReport(array: Array<any>, array2: Array<any>, userId: string, customerId: string, fromdate: Date, toDate: Date, totalReceivableBefore: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/receivableCustomer/exportExcelReceivableReport';
    return this.httpClient.post(url, {
      ReceivableCustomerDetail: array, ReceiptsList: array2, UserId: userId, CustomerId: customerId,
      Fromdate: fromdate, ToDate: toDate, TotalReceivableBefore: totalReceivableBefore
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataPayableInvoice() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/payableInvoice/getMasterDataPayableInvoice";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataBankPayableInvoice() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/payableInvoice/getMasterDataBankPayableInvoice";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataPayableInvoiceSearch() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/payableInvoice/getMasterDataPayableInvoiceSearch";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchBankPayableInvoice() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/payableInvoice/getMasterDataSearchBankPayableInvoice";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchBankReceiptInvoice() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/receiptInvoice/getMasterDateSearchBankBookReceipt";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataReceiptInvoice() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/receiptInvoice/getMasterDataBookReceipt";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataSearchReceiptInvoice() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/receiptInvoice/GetMasterDataSearchReceiptInvoice";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchBankBook() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/bankbook/getMasterDataSearchBankBook";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetDataSearchReceivableVendor() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/receivableVendor/getDataSearchReceivableVendor";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetDataSearchCashBook() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/cashbook/getDataSearchCashBook";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createCost(costCode: string, costName: string, organzationId: string, statusId: string, userId : string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/createCost';
    return this.httpClient.post(url, { CostCode : costCode, CostName : costName, OrganzationId : organzationId, StatusId : statusId,
    UserId : userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateCost(costId : string ,costCode: string, costName: string, organzationId: string, statusId: string, userId : string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateCost';
    return this.httpClient.post(url, {CostId : costId, CostCode : costCode, CostName : costName, OrganzationId : organzationId, StatusId : statusId,
    UserId : userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetMasterDataCreateCost() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/quote/getMasterDataCreateCost";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}