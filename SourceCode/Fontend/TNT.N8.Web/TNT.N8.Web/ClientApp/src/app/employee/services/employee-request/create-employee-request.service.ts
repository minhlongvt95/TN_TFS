
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeRequestModel} from '../../models/employee-request.model';

@Injectable()
export class CreateEmployeeRequestService {

  constructor(private httpClient: HttpClient) { }
  
  createEmployeeRequest(empRequest: EmployeeRequestModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/create";
    return this.httpClient.post(url, { EmployeeRequest: empRequest}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateEmployeeRequest(empRequest: EmployeeRequestModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/editEmployeeRequestById";
    return this.httpClient.post(url, { EmployeeRequest: empRequest}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeRequestById(id: string){
  	let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/getEmployeeRequestById";
    return this.httpClient.post(url, { EmployeeRequestId: id}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
