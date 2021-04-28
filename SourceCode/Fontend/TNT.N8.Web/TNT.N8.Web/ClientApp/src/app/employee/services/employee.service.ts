
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EmployeeModel } from "../models/employee.model";
import { ContactModel } from "../../shared/models/contact.model";
import { UserModel } from '../../shared/models/user.model';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

@Injectable()
export class EmployeeService {

  constructor(private httpClient: HttpClient) { }

  createEmployee(emp: EmployeeModel, contact: ContactModel, user: UserModel, isAccessable: Boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/create";
    return this.httpClient.post(url, { Employee: emp, Contact: contact, User: user, IsAccessable: isAccessable }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchEmployee(firstName: string, lastName: string, userName: string, identityId: string, position: Array<any>, organizationId: string) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/search';
    return this.httpClient.post(url, {
      FirstName: firstName,
      LastName: lastName,
      UserName: userName,
      IdentityId: identityId,
      ListPosition: position,
      OrganizationId: organizationId,
      UserId: JSON.parse(currentUser).UserId
      }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  searchEmployeeAsync(firstName: string, lastName: string, userName: string, identityId: string, organizationId: string) {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getAllEmployee';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: JSON.parse(currentUser).UserId}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });

  }

  getAllEmployee() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployee";
    return this.httpClient.post(url, {  }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllEmployeeAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployee";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getEmployeeById(employeeId: string, contactId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeById";
    return this.httpClient.post(url, { EmployeeId: employeeId, ContactId: contactId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeByIdAsync(employeeId: string, contactId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId, ContactId: contactId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  editEmployeeById(employee: EmployeeModel, contact: ContactModel, user: UserModel, isReset: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/editEmployeeById";
    return this.httpClient.post(url, { Employee: employee, Contact: contact, User: user, IsResetPass: isReset }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllEmpAccount() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmpAccount";
    return new Promise((resolve, reject) => { return this.httpClient.post(url, {}).toPromise()
      .then((response: Response) => {
        resolve(response);
      });
    });
  }

  
  //getAllEmployeeAccount() {
  //  let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployeeAccount";
  //  return new Promise((resolve, reject) => { return this.httpClient.post(url, {}).toPromise()
  //    .then((response: Response) => {
  //      resolve(response);
  //    });
  //  });
  //}
  getAllEmployeeAccount() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmployeeAccount";
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
    //return new Promise((resolve, reject) => {
    //  return this.httpClient.post(url, {}).toPromise()
    //    .then((response: Response) => {
    //      resolve(response);
    //    });
    //});
  }

  getAllEmpIdentity(currentEmpId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmpIdentity";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CurrentEmpId: currentEmpId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllEmpAccIdentity(employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllEmpAccIdentity";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  sendEmail(email: string, fullName: string, userName: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/email/sendEmail";
    return this.httpClient.post(url, { EmailAddress: email, FullName: fullName, UserName: userName }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editEmployeeDataPermission(empId: string, isManager: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/editEmployeeDataPermission";
    return this.httpClient.post(url, { EmployeeId: empId, IsManager: isManager }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  employeePermissionMapping(perSetId: string, empId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/employeePermissionMapping';
    return this.httpClient.post(url, { PermissionSetId: perSetId, EmployeeId: empId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeByPositionCode(positionCode: string){
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getEmployeeByPositionCode';
    return this.httpClient.post(url, { PositionCode: positionCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getStatisticForEmpDashBoard(firstOfWeek,lastOfWeek,keyname,userId){
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getStatisticForEmpDashBoard';
    return this.httpClient.post(url, { FirstOfWeek: firstOfWeek,LastOfWeek: lastOfWeek, KeyName: keyname, UserId: userId
     }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getEmployeeToApprove(empId: string, moduleCode: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/employee/getEmployeeApprove';
    return this.httpClient.post(url, { EmployeeId: empId, ModuleCode: moduleCode }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  //Lấy danh sách nhân viên chăm sóc khách hàng theo quyền của người đang đăng nhập
  getEmployeeCareStaff(isManager: boolean, employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeCareStaff";
    return this.httpClient.post(url, { IsManager: isManager, EmployeeId: employeeId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getEmployeeCareStaffAsyc(isManager: boolean, employeeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getEmployeeCareStaff";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { IsManager: isManager, EmployeeId: employeeId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  fileType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
  fileExtension = '.xlsx';

  public exportExcel(jsonData: any[], fileName: string): void {

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(jsonData);
    const wb: XLSX.WorkBook = { Sheets: { 'Danh sách nhân viên': ws }, SheetNames: ['Danh sách nhân viên'] };
    const excelBuffer: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    this.saveExcelFile(excelBuffer, fileName);
  }

  private saveExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], { type: this.fileType });
    FileSaver.saveAs(data, fileName + this.fileExtension);
  }

  checkAdminLogin() {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/checkAdminLogin';
    return this.httpClient.post(url, {
      UserId: JSON.parse(currentUser).UserId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkAdminLoginAsync() {
    const currentUser = <any>localStorage.getItem('auth');
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/checkAdminLogin';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: JSON.parse(currentUser).UserId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });

  }
}
