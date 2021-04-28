
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeInsuranceModel } from '../../models/employee-insurance.model';

@Injectable()
export class EmployeeInsuranceService {

  constructor(private httpClient: HttpClient) { }

  searchEmployeeInsurance(empId : string, empInsr : string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeInsurance/search";
    return this.httpClient.post(url, { EmployeeId: empId, EmployeeInsuranceId: empInsr, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  editEmployeeInsurance(empInsr : EmployeeInsuranceModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeInsurance/edit";
    return this.httpClient.post(url, { EmployeeInsurance: empInsr, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createEmployeeInsurance(empInsr : EmployeeInsuranceModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeInsurance/create";
    return this.httpClient.post(url, { EmployeeInsurance: empInsr, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
