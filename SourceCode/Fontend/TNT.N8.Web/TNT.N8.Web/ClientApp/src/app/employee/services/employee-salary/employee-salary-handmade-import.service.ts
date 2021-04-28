
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class EmployeeSalaryHandmadeImportService {

  constructor(private httpClient: HttpClient) { }

  uploadFile(fileList: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employeeSalary/employeeSalaryHandmadeImport';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }
    formData.append('UserId', userId);
    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
