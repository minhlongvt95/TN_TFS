import { Component, OnInit, ViewChild, ElementRef, Inject, ChangeDetectorRef } from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';

import { AssistantExportListService } from '../../../services/assistant-salary/assistant-export-list.service';

export interface IDialogData {
  lstEmployeeMonthySalary: Array<any>[];
  Month: number;
  Year: number;
  isTrue: Boolean;
}

@Component({
  selector: 'app-assistant-salary-export',
  templateUrl: './assistant-salary-export.component.html',
  styleUrls: ['./assistant-salary-export.component.css']
})
export class AssistantSalaryExportComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  displayedColumns = ['employeename', 'employeecode', 'employeepostionid', 'employeeunit', 'employeebranch', 'basedsalary', 'monthlyworkingday', 'unPaidLeaveDay', 'overtime', 'actualWorkingDay', 'actualOfSalary', 'overtimeOfSalary',
    'enrollmentSalary', 'retentionSalary', 'fuelAllowance', 'phoneAllowance', 'lunchAllowance', 'otherAllowance', 'socialInsuranceSalary', 'socialInsuranceCompanyPaid', 'healthInsuranceCompanyPaid',
    'unemploymentinsuranceCompanyPaid', 'totalInsuranceCompanyPaid', 'socialInsuranceEmployeePaid', 'healthInsuranceEmployeePaid', 'unemploymentinsuranceEmployeePaid', 'totalInsuranceEmployeePaid', 'desciplineAmount',
    'reductionAmount', 'additionalAmount', 'bankAccountCode', 'bankAccountName', 'branchOfBank', 'totalIncome', 'actualPaid'];
  arrayPostion: Array<any>;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  dataSource: any = [];
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  employeeName: string = "";
  employeeCode: string = "";
  employeeUnit: string = "";
  employeePostionId: string = "";
  month: number;
  year: number;
  Current: Date = new Date();
  IO: string = "";
  dialogPopup: MatDialogRef<PopupComponent>;

  constructor(
    public cdRef: ChangeDetectorRef,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<AssistantSalaryExportComponent>,
    private assistantExportListService: AssistantExportListService,
    public dialogPop: MatDialog,)
  {

  }

  ngOnInit() {
    this.month = this.Current.getMonth() + 1;
    this.year = this.Current.getFullYear();
    this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);

  }
  onSaveClick() {
    var array = this.IO.split("-");
    this.year = array[0] ? parseInt(array[0]) : null;
    this.month = array[1] ? parseInt(array[1]) : null;

    this.assistantExportListService.exportAssistant(this.month, this.year, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {

        let _title = "XÁC NHẬN";
        let _content = " Export thành công danh sách trợ giảng.Click 'Xác nhận' để tải xuống file nhập thông tin của các trợ giảng";
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
            this.data.Year = this.year;
            this.dialogRef.close(this.data);
          }
          else {
            this.data.lstEmployeeMonthySalary =_lstEmployeeMonthySalary;
            this.data.isTrue = true;
            this.data.Month = this.month;
            this.data.Year = this.year;
            this.dialogRef.close(this.data);
          }
        });


      }
      else {
        this.snackBar.openFromComponent(FailComponent, { data: "Không có danh sách trợ giảng", ...this.failConfig });
      }

    }, error => { });
  }
  onCancelClick() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }

}
