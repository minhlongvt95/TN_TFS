import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Promotion } from '../models/promotion.model';
import { PromotionMapping } from '../models/promotion-mapping.model';
import { LinkOfDocument } from '../models/link-of-document.model';
import { NoteModel } from '../../shared/models/note.model';

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Injectable()
export class PromotionService {
  constructor(private httpClient: HttpClient) { }
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  getMasterDataCreatePromotion() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/getMasterDataCreatePromotion";
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createPromotion(promotion: Promotion, listPromotionMapping: Array<PromotionMapping>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/createPromotion";
    return this.httpClient.post(url, { 
      Promotion: promotion,
      ListPromotionMapping: listPromotionMapping
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataListPromotion() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/getMasterDataListPromotion";
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchListPromotion(promotionCode: string, 
    promotionName: string, 
    expirationDateFrom: Date,
    expirationDateTo: Date, 
    quantityOrder: number, 
    quantityQuote: number, 
    quantityContract: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/searchListPromotion";
    return this.httpClient.post(url, {
      PromotionCode: promotionCode,
      PromotionName: promotionName,
      ExpirationDateFrom: expirationDateFrom,
      ExpirationDateTo: expirationDateTo,
      QuantityOrder: quantityOrder,
      QuantityQuote: quantityQuote,
      QuantityContract: quantityContract
     }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataDetailPromotion() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/getMasterDataDetailPromotion";
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDetailPromotion(promotionId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/getDetailPromotion";
    return this.httpClient.post(url, {
      PromotionId: promotionId
     }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deletePromotion(promotionId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/deletePromotion";
    return this.httpClient.post(url, {
      PromotionId: promotionId
     }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updatePromotion(promotion: Promotion, listPromotionMapping: Array<PromotionMapping>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/updatePromotion";
    return this.httpClient.post(url, { 
      Promotion: promotion,
      ListPromotionMapping: listPromotionMapping
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createLinkForPromotion(linkOfDocument: LinkOfDocument) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/createLinkForPromotion";
    return this.httpClient.post(url, { 
      LinkOfDocument: linkOfDocument
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteLinkFromPromotion(linkOfDocumentId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/deleteLinkFromPromotion";
    return this.httpClient.post(url, { 
      LinkOfDocumentId: linkOfDocumentId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createFileForPromotion(folderType: string, objectId: string, listFile: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/promotion/createFileForPromotion';
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

  deleteFileFromPromotion(fileInFolderId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/deleteFileFromPromotion";
    return this.httpClient.post(url, { 
      FileInFolderId: fileInFolderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForPromotionDetail(noteModel: NoteModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/createNoteForPromotionDetail";
    return this.httpClient.post(url, { 
      Note: noteModel
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkPromotionByCustomer(customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/checkPromotionByCustomer";
    return new Promise((resolve, reject) => {
    return this.httpClient.post(url, { CustomerId: customerId })
      .toPromise()
      .then((response: Response) => {
        resolve(response);
      });
    });
  }

  getApplyPromotion(conditionsType: number, customerId: string, amount: number, productId: string, quantity: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/getApplyPromotion";
    return this.httpClient.post(url, { 
      ConditionsType: conditionsType,
      CustomerId: customerId,
      Amount: amount,
      ProductId: productId,
      Quantity: quantity
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkPromotionByAmount(amount: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/checkPromotionByAmount";
    return this.httpClient.post(url, { 
      Amount: amount,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkPromotionByProduct(productId: string, quantity: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/promotion/checkPromotionByProduct";
    return new Promise((resolve, reject) => {
    return this.httpClient.post(url, { ProductId: productId, Quantity: quantity })
      .toPromise()
      .then((response: Response) => {
        resolve(response);
      });
    });
  }
}