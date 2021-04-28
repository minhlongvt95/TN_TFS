import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';

class VendorModel {
  VendorId: string;
  VendorName: string;
  VendorCode: string;
  VendorGroupId: string;
  PaymentId: string;
  TotalPurchaseValue: number;
  TotalPayableValue: number;
  NearestDateTransaction: Date;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: boolean;
}

@Pipe({ name: 'VendorService' })
@Injectable()
export class VendorService {

  constructor(private httpClient: HttpClient) { }

  quickCreateVendorMasterdata() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/vendor/quickCreateVendorMasterdata';
    return this.httpClient.post(url, 
      { }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  quickCreateVendor(vendor: VendorModel, email: string, phone: string, address: string) {
    const url = localStorage.getItem('ApiEndPoint') + "/api/vendor/quickCreateVendor";
    return this.httpClient.post(url, 
      { 
        Vendor: vendor,
        Email: email,
        Phone: phone,
        Address: address
      }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}