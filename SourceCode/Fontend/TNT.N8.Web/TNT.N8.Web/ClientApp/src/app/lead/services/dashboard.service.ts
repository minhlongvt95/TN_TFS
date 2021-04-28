
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';
import { LeadModel } from "../models/lead.model";

@Injectable()
export class DashboardService {

  constructor(private httpClient: HttpClient) { }

  getTopLead(code:string): Observable<any> {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/dashboard/gettoplead';
    return this.httpClient.post(url, { Count: 3, StatusCode: code }).pipe(
      map((response: Response) => {
        var result = <any>response;        
        return result;
      }));
  }
  getConvertRate(month, year): Observable<any> {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/dashboard/getconvertrate';
    return this.httpClient.post(url, {
      Month: month,
      year: year
    }).pipe(
      map((response: Response) => {
        var result = <any>response;
        return result;
      }));
  }
  getRequirementRate(): Observable<any> {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/dashboard/getrequirementrate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        var result = <any>response;
        return result;
      }));
  }
  getPotentialRate(): Observable<any> {
    let url = localStorage.getItem('ApiEndPoint') + '/api/lead/dashboard/getpotentialrate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        var result = <any>response;
        return result;
      }));
  }

  getDataLeadDashboard() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/lead/dashboard/getDataLeadDashboard';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

}
