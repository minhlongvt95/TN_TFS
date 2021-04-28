
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeAllowanceModel } from '../../models/employee-allowance.model';

@Injectable()
export class EmployeeAllowanceService {

  constructor(private httpClient: HttpClient) { }

  getEmpAllowanceByEmpId(empId : string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeAllowance/getEmployeeAllowanceById";
    return this.httpClient.post(url, { EmployeeId: empId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editEmpAllowance(empAllowance : EmployeeAllowanceModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeAllowance/editEmployeeAllowance";
    return this.httpClient.post(url, { EmployeeAllowance: empAllowance, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmpAllowanceByEmpIdAsync(empId : string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeAllowance/getEmployeeAllowanceById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: empId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

}
