
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeModel } from "../../models/employee.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { UserModel } from '../../../shared/models/user.model';

@Injectable()
export class EmployeeListService {

  constructor(private httpClient: HttpClient) { }

  searchEmployeeFromList(isManager: boolean, employeeId: string, firstName: string, lastName: string, userName: string, identityId: string, position: Array<any>, organizationId: string,
                    fromContractExpiryDate: Date, toContractExpiryDate: Date, fromBirthDay: Date, toBirthDay: Date, isQuitWork: boolean) {
    
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/searchFromList';
    return this.httpClient.post(url, {
      IsManager: isManager,
      EmployeeId: employeeId,
      FirstName: firstName,
      LastName: lastName,
      UserName: userName,
      IdentityId: identityId,
      ListPosition: position,
      OrganizationId: organizationId,
      FromContractExpiryDate: fromContractExpiryDate,
      ToContractExpiryDate: toContractExpiryDate,
      FromBirthDay: fromBirthDay,
      ToBirthDay: toBirthDay,
      isQuitWork: isQuitWork,
      UserId: JSON.parse(currentUser).UserId
      }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  disableEmployee(employeeId: string) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/disableEmployee';
    return this.httpClient.post(url, {
      EmployeeId: employeeId,
      UserId: JSON.parse(currentUser).UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
