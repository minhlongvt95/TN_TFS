
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { BudgetRequestModel } from '../models/budget-request.model';
@Injectable()
export class BudgetListService {

  constructor(private httpClient : HttpClient) { }

  getAllBudget() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/budget/getAllBudget';
    return new Promise((resolve, reject) => { return this.httpClient.post(url, {}).toPromise()
      .then((response: Response) => {
        resolve(response);
      });
    });
  }
  searchBudget(procurementPlanYear : string , procurementPlanMonth : string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/budget/search';
    return new Promise((resolve, reject) => { return this.httpClient
    .post(url, {ProcurementPlanYear:procurementPlanYear,ProcurementPlanMonth:procurementPlanMonth})
    .toPromise()
      .then((response: Response) => {
        resolve(response);
      });
    });
  }
  getBudgetById(id : string){
    let url = localStorage.getItem('ApiEndPoint') + '/api/budget/getBudgetById';
    //return new Promise((resolve, reject) => { return this.httpClient
    //.post(url, {ProcurementPlanId : id})
    //.toPromise()
    //  .then((response: Response) => {
    //    resolve(response);
    //  });
    //});
    return this.httpClient.post(url, { ProcurementPlanId: id }).pipe(
      map((response: Response) => {
        return response;
      }));
   }
}
