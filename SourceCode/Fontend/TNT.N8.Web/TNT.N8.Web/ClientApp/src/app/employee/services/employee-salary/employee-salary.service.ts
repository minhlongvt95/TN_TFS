
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeSalaryModel } from '../../models/employee-salary.model';
import { Observable } from 'rxjs';

@Injectable()
export class EmployeeSalaryService {

  constructor(private httpClient: HttpClient) {
  }
  getEmpSalaryByEmpId(empId : string, effectiveDate: Date, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/getEmployeeSalaryByEmpId";
    return this.httpClient.post(url, { EmployeeId: empId, EffectiveDate: effectiveDate, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  creatEmpSalary(employeeSalary : EmployeeSalaryModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/createEmpSalary";
    return this.httpClient.post(url, { EmployeeSalary: employeeSalary, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  downloadTemplateTimeSheet() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/downloadEmployeeTimeSheetTemplate";
    return this.httpClient.post(url,{ }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  //findEmployeeMonthySalary(employeeName: string, employeeCode: string, employeeUnit: string, employeeBranch: string, employeePostionId: string, month: number, year: number, userId: string) {
  //  let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/findEmployeeMonthySalary";
  //  return this.httpClient.post(url, { EmployeeName: employeeName, EmployeeCode: employeeCode, EmployeeUnit: employeeUnit, EmployeeBranch: employeeBranch, EmployeePostionId: employeePostionId, Month: month, Year: year, UserId: userId })
  //    .map((response: Response) => {
  //      return response;
  //    });
  //}
  findEmployeeMonthySalary(employeeName: string, employeeCode: string, employeeUnit: string, employeeBranch: string, employeePostionId: string, lstEmployeeUnitId: Array<string>, month: number, year: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/findEmployeeMonthySalary";
    return this.httpClient.post(url, { EmployeeName: employeeName, EmployeeCode: employeeCode, EmployeeUnit: employeeUnit, EmployeeBranch: employeeBranch, EmployeePostionId: employeePostionId, lstEmployeeUnitId: lstEmployeeUnitId, Month: month, Year: year, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeSalaryStatus(commonId: string, userId: string){
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/getEmployeeSalaryStatus";
    return this.httpClient.post(url, { CommonId: commonId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  exportEmployeeSalary(lstEmpMonthySalary: Array<string>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeSalary/exportEmployeeSalary";
    return this.httpClient.post(url, { lstEmpMonthySalary: lstEmpMonthySalary, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
