
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class EmployeeMonthySalaryService {

  constructor(private httpClient: HttpClient) { }

  getEmpMonthySalaryByEmpId(empId : string, year: Number, month: Number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeMonthySalary/search";
    return this.httpClient.post(url, { EmployeeId: empId, Year: year, Month: month }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
