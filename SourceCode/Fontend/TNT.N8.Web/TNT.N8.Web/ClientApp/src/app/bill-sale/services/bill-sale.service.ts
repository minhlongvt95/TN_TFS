import { share, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

class BillSale {
  billOfSaLeId: string;
  billOfSaLeCode: string;
  orderId: string;
  billDate: Date;
  endDate: Date;
  statusId: string;
  termsOfPaymentId: string;
  customerId: string;
  customerName: string;
  debtAccountId: string;
  mst: string;
  paymentMethodId: string;
  employeeId: string;
  customerAddress:string;
  description: string;
  note: string;
  accountBankId: string;
  invoiceSymbol: string;
  discountType:boolean;
  discountValue:number;
  listCost: Array<BillSaleCostModel>;
  listBillSaleDetail: Array<BillSaleDetailModel>;
}


class BillSaleCostModel {
  billOfSaleCostId: string;
  billOfSaleId: string;
  orderCostId: string;
  orderId: string;
  costId: string;
  quantity: number;
  unitPrice: number;
  costName: string;
  costCode:string;
  sumAmount:number;
}

class BillSaleDetailModel {
  billOfSaleDetailId: string;
  billOfSaleId: string;
  vendorId: string;
  vendorName: string;
  productId: string;
  productCode: string;
  quantity: number;
  unitPrice: number;
  currencyUnit: string;
  exchangeRate: number;
  warehouseId: string;
  warehouseCode: string;
  moneyForGoods: number;
  accountId: string;
  accountDiscountId: string;
  vat: number;
  discountType: boolean;
  discountValue: number;
  description: string;
  orderDetailType:number;
  unitId: string;
  unitName: string;
  businessInventory: number;
  productName: string;
  actualInventory: number;
  incurredUnit: string;
  costsQuoteType: number;
  orderDetailId: string;
  orderId: string;
  explainStr:string;
  listBillSaleDetailProductAttribute: Array<BillSaleDetailProductAttributeModel> = [];
}

class BillSaleDetailProductAttributeModel {
  billOfSaleCostProductAttributeId: string;
  orderProductDetailProductAttributeValueId: string;
  orderDetailId: string;
  billOfSaleDetailId: string;
  productId: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
}
@Injectable()
export class BillSaleService {
  constructor(private httpClient: HttpClient) { }
  refreshLead: boolean = false;
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;


  getMasterDataBillSaleCreateEdit(isCreate: boolean, objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/getMasterDataBillSaleCreateEdit';
    return this.httpClient.post(url, {
       IsCreate: isCreate,
       ObjectId: objectId 
      }).pipe(map((response: Response) => {
      return response;
    }));
  }

  getMasterDataSearchBillSale() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/getMasterDataSearchBillSale';
    return this.httpClient.post(url, {
      }).pipe(map((response: Response) => {
      return response;
    }));
  }

  updateStatus(billSaleId: string, statusId: string, note: string,) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/updateStatus';
    return this.httpClient.post(url, {
      BillSaleId:billSaleId,
      StatusId:statusId,
      Note:note
      }).pipe(map((response: Response) => {
      return response;
    }));
  }
  deleteBillSale(billSaleId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/deleteBillSale';
    return this.httpClient.post(url, {
      BillSaleId:billSaleId
      }).pipe(map((response: Response) => {
      return response;
    }));
  }

  searchBillSale(billSaleCode: string, orderCode: string, customerName: string, listProductId: Array<string>, listStatusId: Array<string>,
    fromDate: Date, toDate: Date){
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/searchBillSale';
    return this.httpClient.post(url, {
      BillOfSaleCode: billSaleCode,
      OrderCode: orderCode,
      CustomerName: customerName,
      ListProductId: listProductId,
      ListStatusId: listStatusId,
      FromDate: fromDate,
      ToDate: toDate,
      }).pipe(map((response: Response) => {
      return response;
    }));
  }
  addOrEditBillSale(isCreate: boolean, billSale: BillSale) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/addOrEditBillSale';

    return this.httpClient.post(url, {
      IsCreate: isCreate,
       BillSale: billSale ,
       UserId:this.userId
      }).pipe(map((response: Response) => {
      return response;
    }));
  }
  getOrderByOrderId(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/billSale/getOrderByOrderId';
    return this.httpClient.post(url, {
      OrderId: orderId
      }).pipe(map((response: Response) => {
      return response;
    }));
  }
}
