import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import * as _moment from 'moment';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { default as _rollupMoment } from 'moment';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';

const moment = _rollupMoment || _moment;
import { EmployeeSalaryHandmadeImportService } from '../../../services/employee-salary/employee-salary-handmade-import.service';

export interface IDialogData {
  lstEmployeeMonthySalary: Array<any>[];
  Month: number;
  Year: number;
  isTrue: Boolean;
}

@Component({
  selector: 'app-employee-salary-handmade-import',
  templateUrl: './employee-salary-handmade-import.component.html',
  styleUrls: ['./employee-salary-handmade-import.component.css']
})
export class EmployeeSalaryHandmadeImportComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  accept = '.xls, .xlsx';
  maxSize: number = 11000000;
  files: File[] = [];
  dragFiles: any;
  lastInvalids: any;
  baseDropValid: any;
  lastFileAt: Date;
  month: number;
  year: number;
  Current: Date = new Date();
  IO: string = "";

  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(private employeeSalaryHandmadeImportService: EmployeeSalaryHandmadeImportService,
    public snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<EmployeeSalaryHandmadeImportComponent>,
  ) { }

  ngOnInit() {
    this.month = this.data.Month != null ? this.data.Month : this.Current.getMonth() + 1;
    this.year = this.data.Year != null ? this.data.Year : this.Current.getFullYear();
    this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);
  }
  onSaveClick() {
    if (this.files.length > 0) {
      var listFile: File[] = [];
      listFile.push(this.files[this.files.length - 1]);
      this.uploadFiles(this.files);
    }
  }
  uploadFiles(files: File[]) {
    this.employeeSalaryHandmadeImportService.uploadFile(files, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      //result.excelFile != null &&
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.data.lstEmployeeMonthySalary = result.lstEmployeeMonthySalary;
        this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
        this.data.isTrue = true;
        this.dialogRef.close(this.data);
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }

    }, error => {
      const result = <any>error;
      this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
    });
  }
  onCancelClick() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }

}
