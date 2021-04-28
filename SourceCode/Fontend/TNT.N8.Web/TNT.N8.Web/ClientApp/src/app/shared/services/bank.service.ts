
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BankModel } from '../models/bank.model';

interface BankAccount {
  bankAccountId: string,
  objectId: string,
  objectType: string,
  bankName: string, //Tên ngân hàng
  accountNumber: string,  //Số tài khoản
  accountName: string,  //Chủ tài khoản
  branchName: string, //Chi nhánh
  bankDetail: string,  //Diễn giải
  createdById: string,
  createdDate: Date
}

@Pipe({ name: 'BankService' })
@Injectable()
export class BankService {

  constructor(private httpClient: HttpClient) { }

  createBank(bankAccount: BankAccount) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/createBankAccount";
      return this.httpClient.post(url, { BankAccount: bankAccount }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editBankById(bankAccount: BankModel, userId: string) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/editBankAccount";
      return this.httpClient.post(url, { BankAccount: bankAccount, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteBankById(bankId: string, objectId: string, objectType: string) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/deleteBankAccount";
      return this.httpClient.post(url, { 
        BankAccountId: bankId,
        ObjectId: objectId,
        ObjectType: objectType
      }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getBankById(bankId: string) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/getBankAccountById";
      return this.httpClient.post(url, { BankAccountId: bankId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllBankAccountByObject(objectId: string, objectType: string) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/getAllBankAccountByObject";
      return this.httpClient.post(url, { objectId: objectId, ObjectType: objectType }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllBankAccountByObjectAsync(objectId: string, objectType: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/getAllBankAccountByObject";
    return new Promise((resolve, reject) => {
        return this.httpClient.post(url, { objectId: objectId, ObjectType: objectType }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getCompanyBankAccount(userId: string) {
      let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/getCompanyBankAccount";
      return this.httpClient.post(url, { UserId: userId }).pipe(
          map((response: Response) => {
              return response;
          }));
  }

  getCompanyBankAccountAsync(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/bankAccount/getCompanyBankAccount";
    return new Promise((resolve, reject) => {
        return this.httpClient.post(url, { UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
