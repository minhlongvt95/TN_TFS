
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductVendorMappingModel, SuggestedSupplierQuotesDetailModel, SuggestedSupplierQuotesModel, VendorModel } from '../models/vendor.model';
import { VendorOrderModel } from '../models/vendorOrder.model';
import { VendorOrderDetailModel } from '../models/vendorOrderDetail.model';
import { ContactModel } from '../../shared/models/contact.model';
import { VendorBankAccountModel } from '../models/vendorBankAccount.model';

class VendorOrderProcurementRequestMapping {
  vendorOrderProcurementRequestMappingId: string;
  vendorOrderId: string;
  procurementRequestId: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
}

class VendorOrderCostDetail {
  vendorOrderCostDetailId: string;
  costId: string;
  vendorOrderId: string;
  unitPrice: number;
  costName: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
}

@Injectable()
export class VendorService {

  constructor(private httpClient: HttpClient) { }

  createVendor(emp: VendorModel, contact: ContactModel, vendorContactList: Array<ContactModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendor';
    return this.httpClient.post(url, { Vendor: emp, Contact: contact, VendorContactList: vendorContactList, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchVendor(vendorName: string, vendorCode: string, vendorGroupIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendor';
    return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchVendorAsync(vendorName: string, vendorCode: string, vendorGroupIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendor';
    // return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).pipe(
    //     map((response: Response) => {
    //         return response;
    //     }));
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchVendorQuoteAsync(vendorName: string, vendorCode: string, vendorGroupIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataSearchVendorQuote';
    // return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).pipe(
    //     map((response: Response) => {
    //         return response;
    //     }));
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createVendorQuote(suggestedSupplierQuoteList: any[], suggestedSupplierQuote: any, userId: string, index: number) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorQuote';
    return this.httpClient.post(url, {
      // SuggestedSupplierQuoteList: suggestedSupplierQuoteList,
      SuggestedSupplierQuotes: suggestedSupplierQuote,
      SuggestedSupplierQuoteDetailList: suggestedSupplierQuoteList,
      Index: index,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createVendorQuoteAsyn(suggestedSupplierQuoteList: any[], suggestedSupplierQuote: any, userId: string, index: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorQuote';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        SuggestedSupplierQuotes: suggestedSupplierQuote,
        SuggestedSupplierQuoteDetailList: suggestedSupplierQuoteList,
        UserId: userId,
        Index: index
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllVendorCode(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getAllVendorCode';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getVendorById(vendorId: string, contactId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getVendorById';
    return this.httpClient.post(url, { VendorId: vendorId, ContactId: contactId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVendorByIdAsync(vendorId: string, contactId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/vendor/getVendorById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorId: vendorId, ContactId: contactId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateVendorById(vendor: VendorModel, contact: ContactModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/updateVendorById';
    return this.httpClient.post(url, { Vendor: vendor, Contact: contact, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createVendorBank(vendorBankAccount: VendorBankAccountModel) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorBank';
    return this.httpClient.post(url, { VendorBank: vendorBankAccount }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editVendorBankById(vendorBankAccount: VendorBankAccountModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/editVendorBankById';
    return this.httpClient.post(url, { VendorBank: vendorBankAccount, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteVendorBankById(vendorBankId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/bankAccount/deleteBankAccount';
    return this.httpClient.post(url, { VendorBankId: vendorBankId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVendorBankById(vendorBankId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getVendorBankById';
    return this.httpClient.post(url, { VendorBankId: vendorBankId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllVendor(vendorName: string, vendorCode: string, vendorGroupIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorName: vendorName, VendorCode: vendorCode, VendorGroupIdList: vendorGroupIdList, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  quickCreateVendor(vendor: VendorModel, email: string, phone: string, address: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/quickCreateVendor';
    return this.httpClient.post(url, {
      Vendor: vendor,
      Email: email,
      Phone: phone,
      Address: address
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createVendorOrder(vendorOrder: VendorOrderModel, vendorOrderDetail: Array<VendorOrderDetailModel>,
    listVendorOrderProcurementRequestMapping: Array<VendorOrderProcurementRequestMapping>,
    listVendorOrderCostDetail: Array<VendorOrderCostDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorOrder';
    return this.httpClient.post(url, {
      VendorOrder: vendorOrder,
      VendorOrderDetail: vendorOrderDetail,
      ListVendorOrderProcurementRequestMapping: listVendorOrderProcurementRequestMapping,
      ListVendorOrderCostDetail: listVendorOrderCostDetail
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchVendorOrder(vendorIdList: Array<string>, vendorCode: string, fromDate: Date, toDate: Date,
    statusIdList: Array<string>, createByIds: Array<string>, userId: string,
    listProcurementRequestId: Array<string>, listProductId: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendorOrder';
    return this.httpClient.post(url, {
      VendorIdList: vendorIdList,
      VendorModelCode: vendorCode,
      VendorOrderDateFrom: fromDate,
      VendorOrderDateTo: toDate,
      StatusIdList: statusIdList,
      CreateByIds: createByIds,
      UserId: userId,
      ListProcurementRequestId: listProcurementRequestId,
      ListProductId: listProductId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchVendorOrderAsync(vendorIdList: Array<string>, vendorCode: string, fromDate: Date, toDate: Date, statusIdList: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendorOrder';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorIdList: vendorIdList,
        VendorModelCode: vendorCode,
        CreatedDateFrom: fromDate,
        CreatedDateTo: toDate,
        StatusIdList: statusIdList,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });

  }

  getAllVendorToPay() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getAllVendor';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getAllVendorToPayAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/vendor/getAllVendor";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getVendorOrderById(vendorOrderId: string, customerOrderId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getVendorOrderById';
    return this.httpClient.post(url, { VendorOrderId: vendorOrderId, CustomerOrderId: customerOrderId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateVendorOrderById(vendorOrder: VendorOrderModel, vendorOrderDetail: Array<VendorOrderDetailModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/updateVendorOrderById';
    return this.httpClient.post(url, { VendorOrder: vendorOrder, VendorOrderDetail: vendorOrderDetail, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateActiveVendor(vendorId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/updateActiveVendor';
    return this.httpClient.post(url, { VendorId: vendorId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  quickCreateVendorMasterdata() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/quickCreateVendorMasterdata';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataCreateVendor(useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataCreateVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: useId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataSearchVendor(useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataSearchVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: useId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataEditVendor(vendorId: string, useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataEditVendor';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorId: vendorId,
        UserId: useId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createVendorContact(contact: ContactModel, isUpdate: boolean, useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorContact';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorContactModel: contact,
        IsUpdate: isUpdate,
        UserId: useId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataCreateVendorOrder(useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataCreateVendorOrder';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: useId }).toPromise().then((response: Response) => {
        resolve(response);
      });
    });
  }

  getDataAddVendorOrderDetail(useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataAddVendorOrderDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: useId
      }).toPromise().then((response: Response) => {
        resolve(response);
      });
    });
  }

  createVendorOrderAsync(vendorOrder: VendorOrderModel, vendorOrderDetail: Array<VendorOrderDetailModel>,
    listVendorOrderProcurementRequestMapping: Array<VendorOrderProcurementRequestMapping>,
    listVendorOrderCostDetail: Array<VendorOrderCostDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorOrder';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorOrder: vendorOrder,
        VendorOrderDetail: vendorOrderDetail,
        ListVendorOrderProcurementRequestMapping: listVendorOrderProcurementRequestMapping,
        ListVendorOrderCostDetail: listVendorOrderCostDetail
      }).toPromise().then((response: Response) => {
        resolve(response);
      });
    });
  }

  getDataEditVendorOrder(vendorOrderId: string, useId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getDataEditVendorOrder';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorOrderId: vendorOrderId,
        UserId: useId
      }).toPromise().then((response: Response) => {
        resolve(response);
      });
    });
  }

  updateVendorOrderByIdAsync(vendorOrder: VendorOrderModel, vendorOrderDetail: Array<VendorOrderDetailModel>,
    userId: string, isApproval: boolean,
    listVendorOrderProcurementRequestMapping: Array<VendorOrderProcurementRequestMapping>,
    listVendorOrderCostDetail: Array<VendorOrderCostDetail>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/updateVendorOrderById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorOrder: vendorOrder,
        VendorOrderDetail: vendorOrderDetail,
        UserId: userId,
        IsSendApproval: isApproval,
        ListVendorOrderProcurementRequestMapping: listVendorOrderProcurementRequestMapping,
        ListVendorOrderCostDetail: listVendorOrderCostDetail
      }).toPromise().then((response: Response) => {
        resolve(response);
      });
    });
  }

  getMasterDataSearchVendorOrder() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getMasterDataSearchVendorOrder';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchVendorProductPrice(productName: string, vendorName: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendorProductPrice';
    return this.httpClient.post(url, { ProductName: productName, VendorName: vendorName }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createVendorProductPrice(productVendorMapping: ProductVendorMappingModel, listSuggestedSupplierQuoteId: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createVendorProductPrice';
    return this.httpClient.post(url, { ProductVendorMapping: productVendorMapping, ListSuggestedSupplierQuoteId: listSuggestedSupplierQuoteId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteVendorProductPrice(productVendorMappingId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/deleteVendorProductPrice';
    return this.httpClient.post(url, { ProductVendorMappingId: productVendorMappingId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  downloadTemplateVendorProductPrice(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/downloadTemplateVendorProductPrice';
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  importVendorProductPrice(listProductVendorMapping: Array<ProductVendorMappingModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/importVendorProductPrice';
    return this.httpClient.post(url, { ListProductVendorMapping: listProductVendorMapping, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  approvalOrRejectVendorOrder(vendorOrderId: string, isApproval: boolean, description: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/approvalOrRejectVendorOrder';
    return this.httpClient.post(url, {
      VendorOrderId: vendorOrderId,
      IsAprroval: isApproval,
      Description: description,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateSuggestedSupplierQuote(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getMasterDataCreateSuggestedSupplierQuote';
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateSuggestedSupplierQuoteAsync(suggestedSupplierQuoteId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getMasterDataCreateSuggestedSupplierQuote';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { SuggestedSupplierQuoteId: suggestedSupplierQuoteId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  createOrUpdateSuggestedSupplierQuote(suggestedSupplierQuotes: SuggestedSupplierQuotesModel, listSuggestedSupplierQuotesDetail: SuggestedSupplierQuotesDetailModel[], userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/createOrUpdateSuggestedSupplierQuote';
    return this.httpClient.post(url, { SuggestedSupplierQuotes: suggestedSupplierQuotes, ListSuggestedSupplierQuotesDetail: listSuggestedSupplierQuotesDetail, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteSuggestedSupplierQuoteRequest(suggestedSupplierQuotesId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/deleteSuggestedSupplierQuoteRequest';
    return this.httpClient.post(url, { SuggestedSupplierQuoteId: suggestedSupplierQuotesId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeStatusVendorQuoteAsync(suggestedSupplierQuotesId: string, statusId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/changeStatusVendorQuote';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        SuggestedSupplierQuoteId: suggestedSupplierQuotesId, StatusId: statusId, UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataAddEditCostVendorOrder() {
    const url = localStorage.getItem('ApiEndPoint') + "/api/vendor/getDataAddEditCostVendorOrder";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendEmailVendorQuote(lstEmail: string[], titleEmail: string, contentEmail: string, suggestedSupplierQuoteId: string, base64: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/sendEmailVendorQuote';

    return this.httpClient.post(url, { ListEmail: lstEmail, TitleEmail: titleEmail, ContentEmail: contentEmail, SuggestedSupplierQuoteId: suggestedSupplierQuoteId, Base64Pdf: base64 }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  removeVendorOrder(vendorOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/removeVendorOrder';

    return this.httpClient.post(url, { VendorOrderId: vendorOrderId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  cancelVendorOrder(vendorOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/cancelVendorOrder';

    return this.httpClient.post(url, { VendorOrderId: vendorOrderId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  draftVendorOrder(vendorOrderId: string, isCancelApproval: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/draftVendorOrder';

    return this.httpClient.post(url, { VendorOrderId: vendorOrderId, IsCancelApproval: isCancelApproval }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataVendorOrderReport() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getMasterDataVendorOrderReport';

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchVendorOrderReport(vendorOrderCode: string, listSelectedVendorId: Array<any>, fromDate: Date, toDate: Date,
    productCode: string, listSelectedStatusId: Array<any>, listSelectedProcurementRequestId: Array<any>,
    purchaseContractName: string, description: string, listSelectedEmployeeId: Array<string>
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/searchVendorOrderReport';

    return this.httpClient.post(url, {
      VendorOrderCode: vendorOrderCode,
      ListSelectedVendorId: listSelectedVendorId,
      FromDate: fromDate,
      ToDate: toDate,
      ProductCode: productCode,
      ListSelectedStatusId: listSelectedStatusId,
      ListSelectedProcurementRequestId: listSelectedProcurementRequestId,
      PurchaseContractName: purchaseContractName,
      Description: description,
      ListSelectedEmployeeId: listSelectedEmployeeId
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getQuantityApproval(vendorOrderDetailId: string, procurementRequestItemId: string, productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/vendor/getQuantityApproval';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        VendorOrderDetailId: vendorOrderDetailId,
        ProcurementRequestItemId: procurementRequestItemId,
        ProductId: productId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

}
