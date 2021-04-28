
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeSalaryModel } from '../../models/employee-salary.model';
import { Observable } from 'rxjs';

@Injectable()
export class AssistantSalaryFindService {

  constructor(private httpClient: HttpClient) { }

  findAssistantMonthySalary(employeeName: string, employeeCode: string, employeeUnit: string, employeeBranch: string, employeePostionId: string, month: number, year: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/findAssistantMonthySalary";
    return this.httpClient.post(url, { EmployeeName: employeeName, EmployeeCode: employeeCode, EmployeeUnit: employeeUnit, EmployeeBranch: employeeBranch, EmployeePostionId: employeePostionId, Month: month, Year: year, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
