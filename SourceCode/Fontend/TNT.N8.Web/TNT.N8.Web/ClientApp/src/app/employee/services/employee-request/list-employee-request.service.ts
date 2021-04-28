
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeRequestModel} from '../../models/employee-request.model';

@Injectable()
export class ListEmployeeRequestService {

	constructor(private httpClient: HttpClient) { }

	listAllEmployeeRequest(userId: string) {
	    let url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/getAllEmployeeRequest';
	    return new Promise((resolve, reject) => { return this.httpClient.post(url, {UserId: userId}).toPromise()
	      .then((response: Response) => {
	        resolve(response);
	      });
	    });
	}
	//searchEmployeeRequest(employeeRequestCode:string ,offerEmployeeCode : string , offerEmployeeName : string ,
	//offerOrganizationId : string , listTypeRequestId : Array<any> , listStatusId:Array<any> , startDate : Date, endDate:Date, userId: string)
	//{
	//	 let url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/search';
	//    return new Promise((resolve, reject) => { return this.httpClient.post(url, {EmployeeRequestCode:employeeRequestCode ,OfferEmployeeCode : offerEmployeeCode , OfferEmployeeName : offerEmployeeName,
	//			OfferOrganizationId : offerOrganizationId , ListTypeRequestId : listTypeRequestId , ListStatusId:listStatusId , StartDate : startDate,
	//			 EndDate:endDate, UserId: userId}).toPromise()
	//      .then((response: Response) => {
	//        resolve(response);
	//      });
	//    });
		 
	//}
  searchEmployeeRequest(employeeRequestCode: string, offerEmployeeCode: string, offerEmployeeName: string,
    offerOrganizationId: string, listTypeRequestId: Array<any>, listStatusId: Array<any>, startDate: Date, endDate: Date, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employeeRequest/search';
    return this.httpClient.post(url, {
      EmployeeRequestCode: employeeRequestCode, OfferEmployeeCode: offerEmployeeCode, OfferEmployeeName: offerEmployeeName,
      OfferOrganizationId: offerOrganizationId, ListTypeRequestId: listTypeRequestId, ListStatusId: listStatusId, StartDate: startDate,
      EndDate: endDate, UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


	getEmployeeRequestByEmpId(empId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/getEmployeeRequestByEmpId";
    return this.httpClient.post(url, { EmployeeId: empId, UserId: userId}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkEmployeeCreateRequest() {
    const currentUser = <any>localStorage.getItem('auth');
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/checkEmployeeCreateRequest";
    return this.httpClient.post(url, { EmployeeId: JSON.parse(currentUser).EmployeeId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataSearchEmployeeRequest(userId : string){
    let url = localStorage.getItem('ApiEndPoint') + "/api/employeeRequest/getDataSearchEmployeeRequest";
    return this.httpClient.post(url, { UserId: userId}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
