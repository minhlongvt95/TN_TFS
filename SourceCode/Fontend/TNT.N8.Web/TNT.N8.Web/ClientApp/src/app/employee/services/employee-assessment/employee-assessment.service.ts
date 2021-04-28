
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeAssessmentModel } from '../../models/employee-assessment.model'

@Injectable()
export class EmployeeAssessmentService {

  constructor(private httpClient: HttpClient) { }

  getEmpAssessmentByEmpId(empId : string,year : Number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeAssessment/searchEmployeeAssessment";
    return this.httpClient.post(url, { EmployeeId: empId,Year: year , UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getAllYearToAssessment(empId : string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeAssessment/getAllYearToAssessment";
    return this.httpClient.post(url, { EmployeeId: empId , UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  editEmployeeAssessment(listEmpAssessment :EmployeeAssessmentModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeAssessment/editEmployeeAssessment";
    return this.httpClient.post(url, { ListEmployeeAssessment: listEmpAssessment , UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
