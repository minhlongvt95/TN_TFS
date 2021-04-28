
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeSalaryModel } from '../../models/employee-salary.model';
import { Observable } from 'rxjs';

@Injectable()
export class AssistantExportListService {

  constructor(private httpClient: HttpClient) { }

  exportAssistant(month: number, year: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/exportAssistant";
    return this.httpClient.post(url, { Month: month, Year: year, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  exportAssistantSalary(lstEmpMonthySalary: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/exportAssistantSalary";
    return this.httpClient.post(url, { lstEmpMonthySalary: lstEmpMonthySalary, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
