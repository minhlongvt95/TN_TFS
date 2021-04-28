
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeSalaryModel } from '../../models/employee-salary.model';
import { Observable } from 'rxjs';

@Injectable()
export class TeacherSalaryService {

  constructor(private httpClient: HttpClient) { }

  getTeacherSalary(month: number, year: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/getTeacherSalary";
    return this.httpClient.post(url, { Month: month,Year: year ,UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  findTeacherMonthySalary(employeeName: string, employeeCode: string, employeePostionId: string, month: number, year: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/findTeacherMonthySalary";
    return this.httpClient.post(url, { EmployeeName: employeeName, EmployeeCode: employeeCode, EmployeePostionId: employeePostionId, Month: month, Year: year,UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  exportTeacherSalary(lstEmpMonthySalary: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/exportTeacherSalary";
    return this.httpClient.post(url, { lstEmpMonthySalary: lstEmpMonthySalary, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
