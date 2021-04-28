import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Contract, ContractCostDetail, ContractDetail, ContractProductDetailProductAttributeValue } from '../models/contract.model';
import { AdditionalInformationModel } from '../../shared/models/additional-information.model';
import { map } from 'rxjs/operators';


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

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Injectable({
  providedIn: 'root'
})
export class ContractService {

  constructor(private httpClient: HttpClient) { }

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  getMasterDataContract(contractId: string, quoteId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/contract/getMaterDataContract";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { ContractId: contractId, QuoteId: quoteId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListMainContract(employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/contract/getListMainContract";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateContractAsync(contract: Contract, contractDetail: Array<ContractDetail>, listAdditionalInformation: Array<AdditionalInformationModel>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/contract/createOrUpdateContract";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { Contract: contract, ContractDetail: contractDetail, ListAdditionalInformation: listAdditionalInformation }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateContract(contract: Contract, listAdditionalInformation: Array<AdditionalInformationModel>, contractDetail: Array<ContractDetail>, listContractCost: Array<ContractCostDetail>, file: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/createOrUpdateContract';
    return this.httpClient.post(url, {
      Contract: contract,
      ContractDetail: contractDetail,
      ListAdditionalInformation: listAdditionalInformation,
      ListContractCost: listContractCost,
      ListFormFile: file,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createCloneContract(oldContract: Contract, listAdditionalInformation: Array<AdditionalInformationModel>, 
                      contractDetail: Array<ContractDetail>, listContractCost: Array<ContractCostDetail>, 
                      contractid: string, userId: string){
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/createCloneContract';
    return this.httpClient.post(url, {
      Contract: oldContract,
      listAdditionalInformation: listAdditionalInformation,
      ContractDetail: contractDetail,
      ListContractCost: listContractCost,
      ContractId: contractid,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      })
    );
  
  }

  createOrUpdateContractFromForm(contract: Contract, listAdditionalInformation: Array<AdditionalInformationModel>, contractDetail: Array<ContractDetail>,
    listContractCost: Array<ContractCostDetail>, file: File[], listFile: Array<FileInFolder>, isCreate: boolean, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/createOrUpdateContract';
    let formData: FormData = new FormData();
    var index = 0;
    formData.append("UserId", userId);
     for (let i = 0; i < file.length; i++) {
       formData.append("ListFormFile", file[i]);
     }
    formData.append("Contract.ContractId", contract.ContractId);
    formData.append("Contract.QuoteId", contract.QuoteId);
    formData.append("Contract.CustomerId", contract.CustomerId);
    formData.append("Contract.ContractCode", contract.ContractCode);
    formData.append("Contract.ContractTypeId", contract.ContractTypeId);
    formData.append("Contract.EmployeeId", contract.EmployeeId);
    formData.append("Contract.MainContractId", contract.MainContractId);
    formData.append("Contract.ContractNote", contract.ContractNote);
    formData.append("Contract.ContractDescription", contract.ContractDescription);
    formData.append("Contract.ValueContract", contract.ValueContract == null ? null : contract.ValueContract.toString());
    formData.append("Contract.PaymentMethodId", contract.PaymentMethodId);
    formData.append("Contract.BankAccountId", contract.BankAccountId);
    formData.append("Contract.EffectiveDate", contract.EffectiveDate == null ? null : new Date(contract.EffectiveDate).toUTCString());
    formData.append("Contract.ExpiredDate", contract.ExpiredDate == null ? null : new Date(contract.ExpiredDate).toUTCString());
    formData.append("Contract.ContractTime", contract.ContractTime == null ? null : contract.ContractTime.toString());
    formData.append("Contract.ContractTimeUnit", contract.ContractTimeUnit == null ? null : contract.ContractTimeUnit);
    formData.append("Contract.PaymentMethodId", contract.PaymentMethodId);
    formData.append("Contract.CreatedById", contract.CreatedById);
    formData.append("Contract.CreatedDate", contract.CreatedDate == null ? null : new Date(contract.CreatedDate).toString());
    formData.append("Contract.DiscountType", contract.DiscountType == true ? "true" : "false");
    formData.append("Contract.DiscountValue", contract.DiscountValue == null ? null : contract.DiscountValue.toString());
    formData.append("Contract.Amount", contract.Amount == null ? null : contract.Amount.toString());
    formData.append("Contract.StatusId", contract.StatusId == null ? null : contract.StatusId);

    index = 0;
    contractDetail.forEach(item => {
      formData.append("ContractDetail[" + index + "].CurrencyUnit", item.CurrencyUnit);
      formData.append("ContractDetail[" + index + "].VendorId", item.VendorId);
      formData.append("ContractDetail[" + index + "].ProductId", item.ProductId);
      formData.append("ContractDetail[" + index + "].Quantity", item.Quantity.toString());
      formData.append("ContractDetail[" + index + "].UnitPrice", item.UnitPrice.toString());
      formData.append("ContractDetail[" + index + "].Vat", item.Vat.toString());
      formData.append("ContractDetail[" + index + "].DiscountType", item.DiscountType == true ? "true" : "false");
      formData.append("ContractDetail[" + index + "].DiscountValue", item.DiscountValue.toString());
      formData.append("ContractDetail[" + index + "].Description", item.Description == null ? "" : item.Description.trim());
      formData.append("ContractDetail[" + index + "].OrderDetailType", item.OrderDetailType.toString());
      formData.append("ContractDetail[" + index + "].UnitId", item.UnitId);
      formData.append("ContractDetail[" + index + "].ProductName", item.ProductName == null ? "" : item.ProductName.toString());
      formData.append("ContractDetail[" + index + "].NameMoneyUnit", item.NameMoneyUnit);
      formData.append("ContractDetail[" + index + "].NameVendor", item.NameVendor);
      formData.append("ContractDetail[" + index + "].ExchangeRate", item.ExchangeRate == null ? "1" : item.ExchangeRate.toString());
      formData.append("ContractDetail[" + index + "].ProductNameUnit", item.ProductNameUnit);
      formData.append("ContractDetail[" + index + "].SumAmount", item.SumAmount.toString());
      formData.append("ContractDetail[" + index + "].IncurredUnit", item.IncurredUnit == null ? "" : item.IncurredUnit.toString());
      formData.append("ContractDetail[" + index + "].GuaranteeTime", item.GuaranteeTime == null ? "" : item.GuaranteeTime.toString());
      formData.append("ContractDetail[" + index + "].OrderNumber", item.OrderNumber.toString());
      item.ContractProductDetailProductAttributeValue = item.ContractProductDetailProductAttributeValue == null ? new Array<ContractProductDetailProductAttributeValue>() : item.ContractProductDetailProductAttributeValue;
      for (let i = 0; i < item.ContractProductDetailProductAttributeValue.length; i++) {
        formData.append("ContractDetail[" + index + "].ContractProductDetailProductAttributeValue[" + i + "].ContractDetailProductAttributeId", item.ContractProductDetailProductAttributeValue[i].ContractProductDetailProductAttributeValueId);
        formData.append("ContractDetail[" + index + "].ContractProductDetailProductAttributeValue[" + i + "].ContractDetailId", item.ContractProductDetailProductAttributeValue[i].ContractDetailId);
        formData.append("ContractDetail[" + index + "].ContractProductDetailProductAttributeValue[" + i + "].ProductId", item.ContractProductDetailProductAttributeValue[i].ProductId);
        formData.append("ContractDetail[" + index + "].ContractProductDetailProductAttributeValue[" + i + "].ProductAttributeCategoryId", item.ContractProductDetailProductAttributeValue[i].ProductAttributeCategoryId);
        formData.append("ContractDetail[" + index + "].ContractProductDetailProductAttributeValue[" + i + "].ProductAttributeCategoryValueId", item.ContractProductDetailProductAttributeValue[i].ProductAttributeCategoryValueId);
      }
      index++;
    });
    index = 0;
    listAdditionalInformation.forEach(item => {
      formData.append("ListAdditionalInformation[" + index + "].AdditionalInformationId", item.additionalInformationId);
      formData.append("ListAdditionalInformation[" + index + "].ObjectId", item.objectId);
      formData.append("ListAdditionalInformation[" + index + "].ObjectType", item.objectType);
      formData.append("ListAdditionalInformation[" + index + "].Title", item.title);
      formData.append("ListAdditionalInformation[" + index + "].Content", item.content);
      formData.append("ListAdditionalInformation[" + index + "].Ordinal", item.ordinal == null ? null : item.ordinal.toString());
      formData.append("ListAdditionalInformation[" + index + "].Active", item.active == true ? "true" : "false");
      formData.append("ListAdditionalInformation[" + index + "].CreatedDate", item.createdDate == null ? null : new Date(item.createdDate).toUTCString());
      formData.append("ListAdditionalInformation[" + index + "].CreatedById", item.createdById);
      formData.append("ListAdditionalInformation[" + index + "].OrderNumber", item.orderNumber.toString());
      index++;
    });

    index = 0;
    listContractCost.forEach(item => {
      formData.append("ListContractCost[" + index + "].ContractCostDetailId", item.ContractCostDetailId);
      formData.append("ListContractCost[" + index + "].CostId", item.CostId);
      formData.append("ListContractCost[" + index + "].ContractId", item.ContractId);
      formData.append("ListContractCost[" + index + "].Quantity", item.Quantity == null ? null : item.Quantity.toString());
      formData.append("ListContractCost[" + index + "].UnitPrice", item.UnitPrice == null ? null : item.UnitPrice.toString());
      formData.append("ListContractCost[" + index + "].CreatedById", item.CreatedById);
      formData.append("ListContractCost[" + index + "].CreatedDate", item.CreatedDate == null ? null : new Date(item.CreatedDate).toUTCString());
      formData.append("ListContractCost[" + index + "].Active", item.Active == true ? "true" : "false");
      index++;
    });

     index = 0;
     listFile.forEach(item => {
       formData.append("ListFile[" + index + "].FileInFolderId", item.fileInFolderId);
       formData.append("ListFile[" + index + "].FolderId", item.folderId);
       formData.append("ListFile[" + index + "].FileName", item.fileName);
       formData.append("ListFile[" + index + "].ObjectId", item.objectId);
       formData.append("ListFile[" + index + "].ObjectType", item.objectType);
       formData.append("ListFile[" + index + "].Size", item.size);
       formData.append("ListFile[" + index + "].Active", item.active == true ? "true" : "false");
       formData.append("ListFile[" + index + "].FileExtension", item.fileExtension);
       formData.append("ListFile[" + index + "].CreatedById", item.createdById);
       formData.append("ListFile[" + index + "].CreatedDate", item.createdDate == null ? null : new Date(item.createdDate).toUTCString());
       index++;
     });

    formData.append("IsCreate", isCreate == true ? "true" : "false"); 

    return this.httpClient.post(url, formData, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  uploadFile(folderType: string, listFile: Array<FileUploadModel>, objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/uploadFile';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("ObjectId", objectId);
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  deleteFile(fileInFolderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/deleteFile';
    return this.httpClient.post(url, {
      FileInFolderId: fileInFolderId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  getMasterDataSearchContract() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/contract/getMasterDataSearchContract";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searhcContractAsync(contractCode: string, quoteCode: string, listCustomer: Array<string>, listEmployee: Array<string>, listProduct: Array<string>,
    fromeDate: Date, toDate: Date) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/contract/searchContract";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ContractCode: contractCode,
        QuoteCode: quoteCode,
        ListProductId: listProduct,
        ListEmployeeId: listEmployee,
        ListCutomerId: listCustomer,
        FromDate: fromeDate,
        ToDate: toDate
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searhcContract(contractCode: string, quoteCode: string, listCustomer: Array<string>, listEmployee: Array<string>, listProduct: Array<string>,
    fromeDate: Date, toDate: Date, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/searchContract';
    return this.httpClient.post(url, {
      ContractCode: contractCode,
      QuoteCode: quoteCode,
      ListProductId: listProduct,
      ListEmployeeId: listEmployee,
      ListCutomerId: listCustomer,
      FromDate: fromeDate,
      ToDate: toDate,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeContractStatus(contractId: string, statusId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/changeContractStatus';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ContractId: contractId,
        StatusId: statusId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataDashboardContract(numerMonth: number, contractCode: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/getMasterDataDashboardContract';
    return this.httpClient.post(url, {
      NumberMonth: numerMonth,
      ContractCode: contractCode,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteContract(contractId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/deleteContract';
    return this.httpClient.post(url, {
      ContractId: contractId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
