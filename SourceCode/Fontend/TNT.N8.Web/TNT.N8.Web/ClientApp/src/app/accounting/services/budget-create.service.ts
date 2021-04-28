
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BudgetRequestModel } from '../models/budget-request.model';

@Injectable()
export class BudgetCreateService {

  constructor(private httpClient : HttpClient) { }
  
  //hàm tạo mới một đối tượng budget
  createBudget(budget: BudgetRequestModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/budget/create";
    return this.httpClient.post(url,{ProcurementPlan : budget }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  editBudgetById(procurementPlan : BudgetRequestModel){
    let url = localStorage.getItem('ApiEndPoint') + "/api/budget/editById";
    return this.httpClient.post(url,{ProcurementPlan : procurementPlan }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
