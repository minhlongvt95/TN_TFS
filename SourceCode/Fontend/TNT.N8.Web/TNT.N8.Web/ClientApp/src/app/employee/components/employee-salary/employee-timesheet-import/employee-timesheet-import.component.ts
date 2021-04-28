import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import { FormControl } from '@angular/forms';

import { MatDatepicker } from '@angular/material/datepicker';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';

import { EmployeeTimesheetService } from '../../../services/employee-salary/employee-timesheet.service';

export interface IDialogData {
  lstEmployeeMonthySalary: Array<any>[];
  Month: number;
  Year: number;
  isTrue: Boolean;
}
@Component({
  selector: 'app-employee-timesheet-import',
  templateUrl: './employee-timesheet-import.component.html',
  styleUrls: ['./employee-timesheet-import.component.css']
})
export class EmployeeTimesheetImportComponent implements OnInit {
  arrayMonth = [
    { value: 1, viewValue: 'Tháng 1' },
    { value: 2, viewValue: 'Tháng 2' },
    { value: 3, viewValue: 'Tháng 3' },
    { value: 4, viewValue: 'Tháng 4' },
    { value: 5, viewValue: 'Tháng 5' },
    { value: 6, viewValue: 'Tháng 6' },
    { value: 7, viewValue: 'Tháng 7' },
    { value: 8, viewValue: 'Tháng 8' },
    { value: 9, viewValue: 'Tháng 9' },
    { value: 10, viewValue: 'Tháng 10' },
    { value: 11, viewValue: 'Tháng 11' },
    { value: 12, viewValue: 'Tháng 12' },
  ];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  month: number;
  today: Date = new Date();
  //year: number=today.;
  accept = '.xls, .xlsx';
  maxSize: number = 11000000;
  files: File[] = [];
  dragFiles: any;
  lastInvalids: any;
  baseDropValid: any;
  lastFileAt: Date;
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  dialogPopup: MatDialogRef<PopupComponent>;


  getDate() {
    return new Date();
  }

  @ViewChild(MatDatepicker) picker;
  constructor(
    private employeeTimesheetService: EmployeeTimesheetService,
    public snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<EmployeeTimesheetImportComponent>,
    public dialogPop: MatDialog,
  ) { }
  date = new FormControl();

  ngOnInit() {
    //this.month = this.Current.getMonth() + 1;
    //this.year = this.Current.getFullYear();
    //this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);

  }
  onSaveClick() {
    if (this.files.length > 0) {
      var listFile: File[] = [];
      listFile.push(this.files[this.files.length - 1]);
      this.uploadFiles(this.files);
    }
  }

  uploadFiles(files: File[]) {
    this.employeeTimesheetService.uploadFile(files, this.month, this.today.getFullYear(), this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {
        let _title = "XÁC NHẬN";
        let _content = "Import thành công dữ liệu chấm công. Click 'Xác nhận' để tải xuống file nhập thông tin của các nhân viên";
        this.dialogPopup = this.dialogPop.open(PopupComponent,
          {
            width: '500px',
            height: '250px',
            autoFocus: false,
            data: { title: _title, content: _content }
          });
        var _excel = result.excelFile;
        var _nameFile = result.nameFile;
        var _lstEmployeeMonthySalary = result.lstEmployeeMonthySalary;
        var _messageCode = result.messageCode;
        this.dialogPopup.afterClosed().subscribe(result => {
          if (result) {
            const binaryString = window.atob(_excel);
            const binaryLen = binaryString.length;
            const bytes = new Uint8Array(binaryLen);
            for (let idx = 0; idx < binaryLen; idx++) {
              const ascii = binaryString.charCodeAt(idx);
              bytes[idx] = ascii;
            }
            const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            const fileName = _nameFile + ".xlsx";
            link.download = fileName;
            link.click();

            this.data.lstEmployeeMonthySalary = _lstEmployeeMonthySalary;
            this.data.isTrue = true;
            this.data.Month = this.month;
            this.data.Year = this.today.getFullYear();
            this.dialogRef.close(this.data);
            this.files = [];
            this.snackBar.openFromComponent(SuccessComponent, { data: _messageCode, ...this.successConfig });

          }
          else {
            this.data.lstEmployeeMonthySalary = _lstEmployeeMonthySalary;
            this.data.isTrue = true;
            this.data.Month = this.month;
            this.data.Year = this.today.getFullYear();
            this.dialogRef.close(this.data);
            this.files = [];
            this.snackBar.openFromComponent(SuccessComponent, { data: _messageCode, ...this.successConfig });
          }
        });
      } else {
        this.files = [];
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }

    }, error => { });
  }
  changeMonth(event) {
    this.month = event.value;
  }
  onCancelClick() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }

}
