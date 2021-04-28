import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Inject } from '@angular/core';

import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';

import { TeacherSalaryService } from '../../../services/teacher-salary/teacher-salary.service';
//lstEmployeeMonthySalary: Array<any>[];

export interface IDialogData {
  lstResult: Array<any>;
  lstColumn: Array<string>;
  Month: number;
  Year: number;
  isTrue: Boolean;
}

@Component({
  selector: 'app-teacher-sapi-get',
  templateUrl: './teacher-sapi-get.component.html',
  styleUrls: ['./teacher-sapi-get.component.css']
})
export class TeacherSapiGetComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  month: number;
  year: number;
  Current: Date = new Date();
  IO: string = "";
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  dialogPopup: MatDialogRef<PopupComponent>;

  constructor(
    public cdRef: ChangeDetectorRef,
    public dialog: MatDialog,
    private teacherSalaryService: TeacherSalaryService,
    public snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<TeacherSapiGetComponent>,
    public dialogPop: MatDialog,
  ) { }

  ngOnInit() {
    this.month = this.Current.getMonth() + 1;
    this.year = this.Current.getFullYear();
    this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);

  }
  onSaveClick() {
    var array = this.IO.split("-");
    this.year = array[0] ? parseInt(array[0]) : null;
    this.month = array[1] ? parseInt(array[1]) : null;

    this.teacherSalaryService.getTeacherSalary(this.month, this.year, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {

        let _title = "XÁC NHẬN";
        let _content = "Lấy dữ liệu chấm công giảng viên thành công.Click 'Xác nhận' để tải xuống file nhập thông tin của các giảng viên";
        this.dialogPopup = this.dialogPop.open(PopupComponent,
          {
            width: '500px',
            height: '250px',
            autoFocus: false,
            data: { title: _title, content: _content }
          });
        var _excel = result.excelFile;
        var _nameFile = result.nameFile;
        var _lstResult = result.lstResult;
        var _lstColumn = result.lstColumn;

        this.dialogPopup.afterClosed().subscribe(resultP => {
          if (resultP) {
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

            this.data.lstResult = _lstResult;
            this.data.lstColumn = _lstColumn;
            this.data.Month = this.month;
            this.data.Year = this.year;
            this.data.isTrue = true;
            this.dialogRef.close(this.data);
          }
          else {
            this.data.lstResult = _lstResult;
            this.data.lstColumn = _lstColumn;
            this.data.isTrue = true;
            this.data.Month = this.month;
            this.data.Year = this.year;
            this.dialogRef.close(this.data);
          }
        });
      }
      else {
        this.snackBar.openFromComponent(FailComponent, { data: "Không có thông tin lương giảng viên ", ...this.failConfig });
      }
    }, error => { });
  }

  onCancelClick() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }
}
