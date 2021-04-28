
import {map} from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CompanyConfigModel } from '../models/companyConfig.model';

@Pipe({ name: 'CompanyService' })
export class CompanyService {

  constructor(private httpClient: HttpClient) { }

  getAllCompany() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/company/getAllCompany";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCompanyConfig() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/company/getCompanyConfig";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  
  editCompanyConfig(companyConfigurationObject: CompanyConfigModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/company/editCompanyConfig";
    return this.httpClient.post(url, { CompanyConfigurationObject: companyConfigurationObject}).pipe(
      map((response: Response) => {
        return response;
      }));

  }
}
