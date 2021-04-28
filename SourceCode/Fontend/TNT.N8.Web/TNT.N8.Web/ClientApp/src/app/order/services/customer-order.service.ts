
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerOrder } from '../models/customer-order.model';
import { CustomerOrderDetail } from '../models/customer-order-detail.model';
import { ContactModel } from '../../shared/models/contact.model';
import { OrderCostDetail } from '../models/customer-order-cost-detail.model';
@Injectable()
export class CustomerOrderService {

  constructor(private httpClient: HttpClient) { }

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;
  CreateCustomerOrder(
    customerOrder: CustomerOrder,
    customerOrderDetail: Array<CustomerOrderDetail>,
    orderCostDetail: Array<OrderCostDetail>,
    typeAccount: number,
    contact: ContactModel,
    quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createCustomerOrder';
    return this.httpClient.post(url, {
      CustomerOrder: customerOrder,
      CustomerOrderDetail: customerOrderDetail,
      TypeAccount: typeAccount,
      OrderCostDetail: orderCostDetail,
      Contact: contact,
      QuoteId: quoteId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  UpdateCustomerOrder(customerOrder: CustomerOrder, customerOrderDetail: Array<CustomerOrderDetail>, listOrderCostDetailModel: Array<OrderCostDetail>, typeAccount: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateCustomerOrder';
    return this.httpClient.post(url, {
      CustomerOrder: customerOrder,
      CustomerOrderDetail: customerOrderDetail,
      OrderCostDetail: listOrderCostDetailModel,
      TypeAccount: typeAccount,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  checkBeforCreateOrUpdateOrder(CustomerId: string, MaxDebt: number, AmountOrder: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkBeforCreateOrUpdateOrder';
    return this.httpClient.post(url, {
      AmountOrder: AmountOrder,
      CustomerId: CustomerId,
      MaxDebt: MaxDebt,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusOrder(customerOrderId: string, objectType: string, userId: string, description: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateStatusOrder';
    return this.httpClient.post(url, {
      CustomerOrderId: customerOrderId,
      ObjectType: objectType,
      Description: description,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetCustomerOrderByID(customerOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getCustomerOrderByID';
    return this.httpClient.post(url, {
      CustomerOrderId: customerOrderId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  ExportPdfCustomerOrder(customerOrderId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/exportPdfCustomerOrder';
    return this.httpClient.post(url, {
      CustomerOrderId: customerOrderId,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendEmailCustomerOrder(orderId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailCustomerOrder';
    return this.httpClient.post(url, {
      OrderId: orderId,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderSearch() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderSearch';
    return this.httpClient.post(url, {
      UserId: this.userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchOrder(orderCode: string, customerName: string, listStatusId: Array<string>, phone: string, fromDate: Date, toDate: Date, vat: number, productId: string, quoteId: string, contractId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/searchOrder';
    return this.httpClient.post(url, {
      OrderCode: orderCode,
      CustomerName: customerName,
      ListStatusId: listStatusId,
      Phone: phone,
      FromDate: fromDate,
      ToDate: toDate,
      ProductId: productId,
      QuoteId: quoteId,
      ContractId: contractId,
      Vat: vat
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchProfitAccordingCustomers(orderCode: string, customerName: string, listStatusId: Array<string>, fromDate: Date, toDate: Date, productId: string, quoteId: string, seller: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/searchProfitAccordingCustomers';
    return this.httpClient.post(url, {
      OrderCode: orderCode,
      CustomerName: customerName,
      ListStatusId: listStatusId,
      FromDate: fromDate,
      ToDate: toDate,
      ProductId: productId,
      QuoteId: quoteId,
      Seller: seller
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderCreate(createType: number, createObjectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderCreate';
    return this.httpClient.post(url, {
      CreateType: createType,
      CreateObjectId: createObjectId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderDetailDialog() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderDetailDialog';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVendorByProductId(productId: string, customerGroupId: string, orderDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getVendorByProductId';

    return this.httpClient.post(url, {
      ProductId: productId,
      CustomerGroupId: customerGroupId,
      OrderDate: orderDate,
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataOrderDetail(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderDetail';
    return this.httpClient.post(url, { OrderId: orderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/deleteOrder';
    return this.httpClient.post(url, { OrderId: orderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkReceiptOrderHistory(orderId: string, moneyOrder: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkReceiptOrderHistory';
    return this.httpClient.post(url, {
      OrderId: orderId,
      MoneyOrder: moneyOrder
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderServiceCreate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderServiceCreate';
    return this.httpClient.post(url, {  }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrderService(customerOrder: CustomerOrder, listCustomerOrderDetail: Array<CustomerOrderDetail>, listLocalPointId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createOrderService';
    return this.httpClient.post(url, {
      CustomerOrder: customerOrder,
      ListCustomerOrderDetail: listCustomerOrderDetail,
      ListLocalPointId: listLocalPointId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataPayOrderService() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataPayOrderService';
    return this.httpClient.post(url, {  }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListOrderByLocalPoint(localPointId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getListOrderByLocalPoint';
    return this.httpClient.post(url, { LocalPointId: localPointId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  payOrderByLocalPoint(listOrderId: Array<string>, customerId: string, customerName: string, customerPhone: string,
    discountType: boolean, discountValue: number, point: number, payPoint: number)
  {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/payOrderByLocalPoint';
    return this.httpClient.post(url, {
      ListOrderId: listOrderId,
      CustomerId: customerId,
      CustomerName: customerName,
      CustomerPhone: customerPhone,
      DiscountType: discountType,
      DiscountValue: discountValue,
      Point: point,
      PayPoint: payPoint
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkExistsCustomerByPhone(customerPhone: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkExistsCustomerByPhone';
    return this.httpClient.post(url, { CustomerPhone: customerPhone }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  refreshLocalPoint() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/refreshLocalPoint';
    return this.httpClient.post(url, {  }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getLocalPointByLocalAddress(localAddressId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getLocalPointByLocalAddress';
    return this.httpClient.post(url, { LocalAddressId: localAddressId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListOrderDetailByOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getListOrderDetailByOrder';
    return this.httpClient.post(url, { OrderId: orderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListProductWasOrder(localPointId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getListProductWasOrder';
    return this.httpClient.post(url, { LocalPointId: localPointId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateCustomerService(orderId: string, listCustomerOrderDetail: Array<CustomerOrderDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateCustomerService';
    return this.httpClient.post(url, { OrderId: orderId, ListCustomerOrderDetail: listCustomerOrderDetail }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataProfitByCustomer(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/getDataProfitByCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchProfitCustomer(filterData: any,userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/searchProfitCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ...filterData,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }


}
