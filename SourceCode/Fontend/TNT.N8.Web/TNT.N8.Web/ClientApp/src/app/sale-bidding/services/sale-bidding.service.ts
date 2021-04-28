import { share, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { CostsQuocte } from "../models/costs-quocte.model";

class SaleBidding {
  saleBiddingId: string;
  saleBiddingName: string;
  leadId: string;
  customerId: string;
  valueBid: number;
  startDate: Date;
  address: string;
  bidStartDate: Date;
  personInChargeId: string;
  effecTime: number;
  endDate: Date;
  typeContractId: string;
  formOfBid: string;
  currencyUnitId: string;
  ros: number;
  typeContractName: string;
  slowDay: number;
  saleBiddingCode: string;
  provisionalGrossProfit: number;
  employeeId: string;
  statusId: string;
}

class SaleBiddingDetail {
  saleBiddingDetailId: string
  saleBiddingId: string;
  category: string;
  content: string;
  note: string;
  file: File[];
  listFile: Array<FileInFolder>;
  index: number;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
}


class SaleBiddingDetailProductAttribute {
  leadDetailId: string;
  saleBiddingDetailId: string;
  productId: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
}

class StatusSaleBiddingModel {
  saleBiddingId: string;
  statusId: string;
  note: string
}

class LeadDetail {
  taxMoney: number;
  currencyUnit: string;
  description: string;
  discountType: boolean;
  discountValue: number;
  incurredUnit: string;
  leadDetailId: string;
  leadId: string;
  leadProductDetailProductAttributeValue: Array<LeadProductDetailProductAttributeValue>;
  nameMoneyUnit: string;
  nameVendor: string;
  orderDetailType: number;
  productCode: string;
  productId: string;
  productName: string;
  productNameUnit: string;
  quantity: number;
  sumAmount: number;
  unitId: string;
  unitPrice: number;
  vat: number;
  vendorId: string;
  index: number;
  active: boolean;
  exchangeRate: number;
}
class LeadProductDetailProductAttributeValue {
  leadDetailId: string;
  leadProductDetailProductAttributeValue1: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
  productId: string;
}

class Employee {
  employeeId: string;
  employeeName: string;
}

@Injectable()

export class SaleBiddingService {

  constructor(private httpClient: HttpClient) { }
  refreshLead: boolean = false;
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;
  

  getMasterDataSaleBiddingDashboard(userId: string, effectiveDate: string, saleBiddingName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getMasterDateSaleBiddingDashboard';
    return this.httpClient.post(url, { UserId: userId, EffectiveDate: effectiveDate, SaleBiddingName: saleBiddingName }).pipe(map((response: Response) => {
      return response;
    }));
  }


  getMasterDataCreateSaleBidding(leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getMasterDataCreateSaleBidding';
    return this.httpClient.post(url, {
      LeadId: leadId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createSaleBidding(saleBidding: SaleBidding, listEmployee: Array<Employee>, listCost: Array<LeadDetail>, listQuocte: Array<LeadDetail>, listSaleBiddingDetail: Array<SaleBiddingDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/createSaleBidding';
    let formData: FormData = new FormData();
    var index = 0;
    formData.append("UserId", this.userId);
    for (var pair of listSaleBiddingDetail) {
      formData.append("ListSaleBiddingDetail[" + index + "].SaleBiddingDetailId", pair.saleBiddingDetailId);
      formData.append("ListSaleBiddingDetail[" + index + "].SaleBiddingId", pair.saleBiddingId);
      formData.append("ListSaleBiddingDetail[" + index + "].Category", pair.category);
      formData.append("ListSaleBiddingDetail[" + index + "].Content", pair.content);

      for (let i = 0; i < pair.file.length; i++) {
        formData.append("ListSaleBiddingDetail[" + index + "].ListFormFile", pair.file[i]);
      }
      index++;
    }

    formData.append("SaleBidding.SaleBiddingId", saleBidding.saleBiddingId);
    formData.append("SaleBidding.SaleBiddingName", saleBidding.saleBiddingName);
    formData.append("SaleBidding.LeadId", saleBidding.leadId);
    formData.append("SaleBidding.CustomerId", saleBidding.customerId);
    formData.append("SaleBidding.ValueBid", saleBidding.valueBid == null ? null : saleBidding.valueBid.toString());
    formData.append("SaleBidding.StartDate", saleBidding.startDate == null ? null : saleBidding.startDate.toUTCString());
    formData.append("SaleBidding.Address", saleBidding.address == null ? "" : saleBidding.address);
    formData.append("SaleBidding.BidStartDate", saleBidding.bidStartDate == null ? null : saleBidding.bidStartDate.toUTCString());
    formData.append("SaleBidding.PersonInChargeId", saleBidding.personInChargeId);
    formData.append("SaleBidding.EffecTime", saleBidding.effecTime == null ? null : saleBidding.effecTime.toString());
    formData.append("SaleBidding.EndDate", saleBidding.endDate == null ? null : saleBidding.endDate.toUTCString());
    formData.append("SaleBidding.FormOfBid", saleBidding.formOfBid);
    formData.append("SaleBidding.CurrencyUnitId", saleBidding.currencyUnitId);
    formData.append("SaleBidding.TypeContractId", saleBidding.typeContractId);
    formData.append("SaleBidding.StatusId", saleBidding.statusId);
    formData.append("SaleBidding.EmployeeId", saleBidding.employeeId);

    listEmployee.forEach(item => {
      formData.append("ListEmployee", item.employeeId);
    })
    index = 0;
    listCost.forEach(item => {
      formData.append("ListCost[" + index + "].CurrencyUnit", item.currencyUnit);
      formData.append("ListCost[" + index + "].VendorId", item.vendorId);
      formData.append("ListCost[" + index + "].ProductId", item.productId);
      formData.append("ListCost[" + index + "].Quantity", item.quantity.toString());
      formData.append("ListCost[" + index + "].UnitPrice", item.unitPrice.toString());
      formData.append("ListCost[" + index + "].Vat", item.vat.toString());
      formData.append("ListCost[" + index + "].ExchangeRate", item.exchangeRate.toString());
      formData.append("ListCost[" + index + "].DiscountType", item.discountType == true ? "true" : "false");
      formData.append("ListCost[" + index + "].DiscountValue", item.discountValue.toString());
      formData.append("ListCost[" + index + "].Description", item.description == null ? "" : item.description.trim());
      formData.append("ListCost[" + index + "].OrderDetailType", item.orderDetailType.toString());
      formData.append("ListCost[" + index + "].UnitId", item.unitId);
      formData.append("ListCost[" + index + "].ProductName", item.productName == null ? "" : item.productName.toString());
      formData.append("ListCost[" + index + "].NameMoneyUnit", item.nameMoneyUnit);
      formData.append("ListCost[" + index + "].NameVendor", item.nameVendor);
      formData.append("ListCost[" + index + "].ProductCode", item.productCode);
      formData.append("ListCost[" + index + "].ProductNameUnit", item.productNameUnit);
      formData.append("ListCost[" + index + "].SumAmount", item.sumAmount.toString());
      formData.append("ListCost[" + index + "].IncurredUnit", item.incurredUnit == null ? "" : item.incurredUnit.toString());
      for (let i = 0; i < item.leadProductDetailProductAttributeValue.length; i++) {
        formData.append("ListCost[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].LeadProductDetailProductAttributeValue1", item.leadProductDetailProductAttributeValue[i].leadProductDetailProductAttributeValue1);
        formData.append("ListCost[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].LeadDetailId", item.leadProductDetailProductAttributeValue[i].leadDetailId);
        formData.append("ListCost[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].ProductId", item.leadProductDetailProductAttributeValue[i].productId);
        formData.append("ListCost[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].ProductAttributeCategoryId", item.leadProductDetailProductAttributeValue[i].productAttributeCategoryId);
        formData.append("ListCost[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].ProductAttributeCategoryValueId", item.leadProductDetailProductAttributeValue[i].productAttributeCategoryValueId);
      }
      index++;
    });

    index = 0;
    listQuocte.forEach(item => {
      formData.append("ListQuocte[" + index + "].CurrencyUnit", item.currencyUnit);
      formData.append("ListQuocte[" + index + "].VendorId", item.vendorId);
      formData.append("ListQuocte[" + index + "].ProductId", item.productId);
      formData.append("ListQuocte[" + index + "].Quantity", item.quantity.toString());
      formData.append("ListQuocte[" + index + "].UnitPrice", item.unitPrice.toString());
      formData.append("ListQuocte[" + index + "].Vat", item.vat.toString());
      formData.append("ListQuocte[" + index + "].DiscountType", item.discountType == true ? "true" : "false");
      formData.append("ListQuocte[" + index + "].DiscountValue", item.discountValue.toString());
      formData.append("ListQuocte[" + index + "].Description", item.description == null ? "" : item.description.trim());
      formData.append("ListQuocte[" + index + "].OrderDetailType", item.orderDetailType.toString());
      formData.append("ListQuocte[" + index + "].UnitId", item.unitId);
      formData.append("ListQuocte[" + index + "].ProductName", item.productName == null ? "" : item.productName.toString());
      formData.append("ListQuocte[" + index + "].NameMoneyUnit", item.nameMoneyUnit);
      formData.append("ListQuocte[" + index + "].NameVendor", item.nameVendor);
      formData.append("ListQuocte[" + index + "].ProductCode", item.productCode);
      formData.append("ListQuocte[" + index + "].ProductNameUnit", item.productNameUnit);
      formData.append("ListQuocte[" + index + "].SumAmount", item.sumAmount.toString());
      formData.append("ListQuocte[" + index + "].IncurredUnit", item.incurredUnit == null ? "" : item.incurredUnit.toString());
      for (let i = 0; i < item.leadProductDetailProductAttributeValue.length; i++) {
        formData.append("ListQuocte[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].LeadProductDetailProductAttributeValue1", item.leadProductDetailProductAttributeValue[i].leadProductDetailProductAttributeValue1);
        formData.append("ListQuocte[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].LeadDetailId", item.leadProductDetailProductAttributeValue[i].leadDetailId);
        formData.append("ListQuocte[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].ProductId", item.leadProductDetailProductAttributeValue[i].productId);
        formData.append("ListQuocte[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].ProductAttributeCategoryId", item.leadProductDetailProductAttributeValue[i].productAttributeCategoryId);
        formData.append("ListQuocte[" + index + "].LeadProductDetailProductAttributeValue[" + i + "].ProductAttributeCategoryValueId", item.leadProductDetailProductAttributeValue[i].productAttributeCategoryValueId);
      }
      index++;
    });



    return this.httpClient.post(url, formData, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  searchSaleBiddingApproved(saleBiddingName: string, customerId: string, employeeId: Array<string>, bidStartDateForm: Date, bidStartDateTo: Date, isApproved: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/searchSaleBiddingApproved';
    return this.httpClient.post(url, {
      SaleBiddingName: saleBiddingName,
      CustomerName: customerId,
      EmployeeId: employeeId,
      BidStartDateForm: bidStartDateForm,
      BidStartDateTo: bidStartDateTo,
      IsApproved: isApproved
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getMasterDataSaleBiddingApproved() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getMasterDataSaleBiddingApproved';
    return this.httpClient.post(url, {
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getMasterDataSearchSaleBidding(userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getMasterDataSearchSaleBidding';
    return this.httpClient.post(url, {
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getCustomerByEmployeeId(employeeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getCustomerByEmployeeId';
    return this.httpClient.post(url, {
      EmployeeId: employeeId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getPersonInChargeByCustomerId(customerId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getPersonInChargeByCustomerId';
    return this.httpClient.post(url, {
      CustomerId: customerId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  sendEmailEmployee(saleBiddingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/sendEmailEmployee';
    return this.httpClient.post(url, {
      SaleBiddingId: saleBiddingId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchSaleBidding(saleBiddingName: string, cusName: string, phone: string, email: string, listTypeCustomer: Array<string>,
    listContractId: Array<string>, listPersonalInChangeId: Array<string>, listStatusId: Array<string>,
    fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/searchSaleBidding';
    return this.httpClient.post(url, {
      SaleBiddingName: saleBiddingName,
      CusName: cusName,
      Phone: phone,
      Email: email,
      ListTypeCustomer: listTypeCustomer,
      ListContractId: listContractId,
      ListPersonalInChangeId: listPersonalInChangeId,
      ListStatusId: listStatusId,
      FromDate: fromDate,
      ToDate: toDate
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSaleBiddingAddEditProductDialog() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getMasterDataSaleBiddingAddEditProductDialog';
    return this.httpClient.post(url, {
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getVendorByProductId(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getVendorByProductId';
    return this.httpClient.post(url, {
      ProductId: productId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVendorMapping(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getVendorMapping';
    return this.httpClient.post(url, {
      ProductId: productId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  downloadTemplateProduct() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/downloadTemplateProduct';

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataSaleBiddingDetail(saleBiddingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/getMasterDataSaleBiddingDetail';
    return this.httpClient.post(url, {
      SaleBiddingId: saleBiddingId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusSaleBidding(listStaus: Array<StatusSaleBiddingModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/updateStatusSaleBidding';
    return this.httpClient.post(url, {
      ListStaus: listStaus
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editSaleBidding(saleBidding: SaleBidding, listEmployee: Array<Employee>, listCost: Array<CostsQuocte>, listQuocte: Array<CostsQuocte>, listSaleBiddingDetail: Array<SaleBiddingDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/saleBidding/editSaleBidding';
    let formData: FormData = new FormData();
    var index = 0;

    formData.append("UserId", this.userId);
    for (var pair of listSaleBiddingDetail) {
      formData.append("ListSaleBiddingDetail[" + index + "].SaleBiddingDetailId", pair.saleBiddingDetailId);
      formData.append("ListSaleBiddingDetail[" + index + "].SaleBiddingId", pair.saleBiddingId);
      formData.append("ListSaleBiddingDetail[" + index + "].Category", pair.category);
      formData.append("ListSaleBiddingDetail[" + index + "].Content", pair.content);
      pair.listFile = pair.listFile == null ? [] : pair.listFile;
      for (let j = 0; j < pair.listFile.length; j++) {
        if (pair.listFile[j].fileInFolderId != null) {
          formData.append("ListSaleBiddingDetail[" + index + "].ListFile[" + j + "].FileInFolderId", pair.listFile[j].fileInFolderId);
        }
      }
      pair.file = pair.file == null ? [] : pair.file;
      for (let i = 0; i < pair.file.length; i++) {
        formData.append("ListSaleBiddingDetail[" + index + "].ListFormFile", pair.file[i]);
      }
      index++;
    }
    formData.append("SaleBidding.SaleBiddingId", saleBidding.saleBiddingId);
    formData.append("SaleBidding.SaleBiddingName", saleBidding.saleBiddingName);
    formData.append("SaleBidding.SaleBiddingCode", saleBidding.saleBiddingCode);
    formData.append("SaleBidding.LeadId", saleBidding.leadId);
    formData.append("SaleBidding.CustomerId", saleBidding.customerId);
    formData.append("SaleBidding.ValueBid", saleBidding.valueBid == null ? null : saleBidding.valueBid.toString());
    formData.append("SaleBidding.StartDate", saleBidding.startDate == null ? null : saleBidding.startDate.toUTCString());
    formData.append("SaleBidding.Address", saleBidding.address == null ? "" : saleBidding.address);
    formData.append("SaleBidding.BidStartDate", saleBidding.bidStartDate == null ? null : saleBidding.bidStartDate.toUTCString());
    formData.append("SaleBidding.PersonInChargeId", saleBidding.personInChargeId);
    formData.append("SaleBidding.EffecTime", saleBidding.effecTime == null ? null : saleBidding.effecTime.toString());
    formData.append("SaleBidding.EndDate", saleBidding.endDate == null ? null : saleBidding.endDate.toUTCString());
    formData.append("SaleBidding.FormOfBid", saleBidding.formOfBid);
    formData.append("SaleBidding.CurrencyUnitId", saleBidding.currencyUnitId);
    formData.append("SaleBidding.TypeContractId", saleBidding.typeContractId);
    formData.append("SaleBidding.StatusId", saleBidding.statusId);
    formData.append("SaleBidding.EmployeeId", saleBidding.employeeId);

    listEmployee.forEach(item => {
      formData.append("ListEmployee", item.employeeId);
    })
    index = 0;
    listCost.forEach(item => {
      formData.append("ListCost[" + index + "].CurrencyUnit", item.currencyUnit);
      formData.append("ListCost[" + index + "].ExchangeRate", item.exchangeRate.toString());
      formData.append("ListCost[" + index + "].VendorId", item.vendorId);
      formData.append("ListCost[" + index + "].ProductId", item.productId);
      formData.append("ListCost[" + index + "].Quantity", item.quantity.toString());
      formData.append("ListCost[" + index + "].UnitPrice", item.unitPrice.toString());
      formData.append("ListCost[" + index + "].Vat", item.vat.toString());
      formData.append("ListCost[" + index + "].DiscountType", item.discountType == true ? "true" : "false");
      formData.append("ListCost[" + index + "].DiscountValue", item.discountValue.toString());
      formData.append("ListCost[" + index + "].Description", item.description);
      formData.append("ListCost[" + index + "].OrderDetailType", item.orderDetailType.toString());
      formData.append("ListCost[" + index + "].UnitId", item.unitId);
      formData.append("ListCost[" + index + "].ProductName", item.productName == null ? "" : item.productName.toString());
      formData.append("ListCost[" + index + "].NameMoneyUnit", item.nameMoneyUnit);
      formData.append("ListCost[" + index + "].NameVendor", item.nameVendor);
      formData.append("ListCost[" + index + "].ProductCode", item.productCode);
      formData.append("ListCost[" + index + "].ProductNameUnit", item.productNameUnit);
      formData.append("ListCost[" + index + "].SumAmount", item.sumAmount.toString());
      formData.append("ListCost[" + index + "].IncurredUnit", item.incurredUnit == null ? "" : item.incurredUnit.toString());
      for (let i = 0; i < item.saleBiddingDetailProductAttribute.length; i++) {
        formData.append("ListCost[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].SaleBiddingDetailProductAttributeId", item.saleBiddingDetailProductAttribute[i].saleBiddingDetailProductAttributeId);
        formData.append("ListCost[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].SaleBiddingDetailId", item.saleBiddingDetailProductAttribute[i].saleBiddingDetailId);
        formData.append("ListCost[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].ProductId", item.saleBiddingDetailProductAttribute[i].productId);
        formData.append("ListCost[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].ProductAttributeCategoryId", item.saleBiddingDetailProductAttribute[i].productAttributeCategoryId);
        formData.append("ListCost[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].ProductAttributeCategoryValueId", item.saleBiddingDetailProductAttribute[i].productAttributeCategoryValueId);
      }
      index++;
    });

    index = 0;
    listQuocte.forEach(item => {
      formData.append("ListQuocte[" + index + "].CurrencyUnit", item.currencyUnit);
      formData.append("ListQuocte[" + index + "].ExchangeRate", item.exchangeRate.toString());
      formData.append("ListQuocte[" + index + "].VendorId", item.vendorId);
      formData.append("ListQuocte[" + index + "].ProductId", item.productId);
      formData.append("ListQuocte[" + index + "].Quantity", item.quantity.toString());
      formData.append("ListQuocte[" + index + "].UnitPrice", item.unitPrice.toString());
      formData.append("ListQuocte[" + index + "].Vat", item.vat.toString());
      formData.append("ListQuocte[" + index + "].DiscountType", item.discountType == true ? "true" : "false");
      formData.append("ListQuocte[" + index + "].DiscountValue", item.discountValue.toString());
      formData.append("ListQuocte[" + index + "].Description", item.description);
      formData.append("ListQuocte[" + index + "].OrderDetailType", item.orderDetailType.toString());
      formData.append("ListQuocte[" + index + "].UnitId", item.unitId);
      formData.append("ListQuocte[" + index + "].ProductName", item.productName == null ? "" : item.productName.toString());
      formData.append("ListQuocte[" + index + "].NameMoneyUnit", item.nameMoneyUnit);
      formData.append("ListQuocte[" + index + "].NameVendor", item.nameVendor);
      formData.append("ListQuocte[" + index + "].ProductCode", item.productCode);
      formData.append("ListQuocte[" + index + "].ProductNameUnit", item.productNameUnit);
      formData.append("ListQuocte[" + index + "].SumAmount", item.sumAmount.toString());
      formData.append("ListQuocte[" + index + "].IncurredUnit", item.incurredUnit == null ? "" : item.incurredUnit.toString());
      for (let i = 0; i < item.saleBiddingDetailProductAttribute.length; i++) {
        formData.append("ListQuocte[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].SaleBiddingDetailProductAttributeId", item.saleBiddingDetailProductAttribute[i].saleBiddingDetailProductAttributeId);
        formData.append("ListQuocte[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].SaleBiddingDetailId", item.saleBiddingDetailProductAttribute[i].saleBiddingDetailId);
        formData.append("ListQuocte[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].ProductId", item.saleBiddingDetailProductAttribute[i].productId);
        formData.append("ListQuocte[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].ProductAttributeCategoryId", item.saleBiddingDetailProductAttribute[i].productAttributeCategoryId);
        formData.append("ListQuocte[" + index + "].SaleBiddingDetailProductAttribute[" + i + "].ProductAttributeCategoryValueId", item.saleBiddingDetailProductAttribute[i].productAttributeCategoryValueId);
      }
      index++;
    });

    return this.httpClient.post(url, formData, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
