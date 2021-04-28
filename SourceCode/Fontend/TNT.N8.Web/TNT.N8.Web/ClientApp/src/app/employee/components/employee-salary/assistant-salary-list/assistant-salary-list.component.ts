import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';

import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatCheckboxChange } from '@angular/material/checkbox';

import { SelectionModel } from '@angular/cdk/collections';

import { AssistantSalaryExportComponent } from '../assistant-salary-export/assistant-salary-export.component';
import { AssistantSalaryHandmadeImportComponent } from '../assistant-salary-handmade-import/assistant-salary-handmade-import.component';
import { OrgSelectMultiDialogComponent } from '../../org-select-multi-dialog/org-select-multi-dialog.component';

import { PositionService } from '../../../../shared/services/position.service';
import { AssistantSalaryFindService } from '../../../services/assistant-salary/assistant-salary-find.service';
import { AssistantExportListService } from '../../../services/assistant-salary/assistant-export-list.service';
import { WorkflowService } from '../../../../admin/components/workflow/services/workflow.service';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { EmployeeSalaryService } from '../../../services/employee-salary/employee-salary.service';
import { EmailService } from '../../../../shared/services/email.service';

import * as $ from 'jquery';
@Component({
  selector: 'app-assistant-salary-list',
  templateUrl: './assistant-salary-list.component.html',
  styleUrls: ['./assistant-salary-list.component.css']
})
export class AssistantSalaryListComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns = ['select', 'employeename', 'employeecode', 'employeepostionid', 'employeeunit', 'basedsalary', 'actualWorkingDay',
    'reductionAmount', 'additionalAmount', 'actualPaid', 'description'];
  arrayPostion: Array<any>;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  dataSource: any = [];
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  dialogExportAssistant: MatDialogRef<AssistantSalaryExportComponent>;
  dialogImportSalaryHandmade: MatDialogRef<AssistantSalaryHandmadeImportComponent>;
  dialogOrg: MatDialogRef<OrgSelectMultiDialogComponent>;


  selection: SelectionModel<any>;
  selectedEmpMonthySalaryList: any = [];
  selectedRowIndex: string;

  employeeName: string = "";
  employeeCode: string = "";
  employeeUnit: string = "";
  employeePostionId: string = "";
  month: number;
  year: number;
  Current: Date = new Date();
  IO: string = "";

  featureCode: string = 'PDLTG';
  isApproved: boolean = false;
  isRejected: boolean = false;
  isInApprovalProgress: boolean = false;
  isApprover: boolean = false;
  isPosition: boolean = false;
  dialogConfirmPopup: MatDialogRef<PopupComponent>;
  commonId: string = '00000000-0000-0000-0000-000000000000';
  statusName: string = '';
  noItem: boolean = true;
  notes: any = [];

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(
    public cdRef: ChangeDetectorRef,
    public dialog: MatDialog,
    private positionService: PositionService,
    private assistantSalaryFindService: AssistantSalaryFindService,
    private assistantExportListService: AssistantExportListService,
    private workflowService: WorkflowService,
    public snackBar: MatSnackBar,
    private employeeSalaryService: EmployeeSalaryService,
    private emailService: EmailService,
  ) { }

  ngOnInit() {
    this.getAllPostion();
    this.month = this.Current.getMonth() + 1;
    this.year = this.Current.getFullYear();
    this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);
    this.search();
  }


  assistantSalaryView: string = 'assistant-salary/view';
  assistantSalaryImport: string = 'assistant-salary/import';
  assistantSalaryExport: string = 'assistant-salary/explort'
  userPermission: any = localStorage.getItem("UserPermission").split(',');
 
  exportAssistant() {
    this.dialogExportAssistant = this.dialog.open(AssistantSalaryExportComponent,
      {
        width: '600px',
        height: '580',
        autoFocus: false,
        data: {}
      });
    this.dialogExportAssistant.afterClosed().subscribe(result => {
      if (typeof result !== "undefined") {
        if (result.isTrue) {
          if (result.lstEmployeeMonthySalary != null && result.lstEmployeeMonthySalary.length > 0) {
            this.commonId = result.lstEmployeeMonthySalary[0].commonId;
            this.noItem = false;

            this.employeeName = "";
            this.employeeCode = "";
            this.employeeUnit = "";
            this.employeePostionId = undefined;
            this.month = result.Month;
            this.year = result.Year;
            this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);

            this.dataSource = new MatTableDataSource<any>(result.lstEmployeeMonthySalary);
            this.dataSource.paginator = this.paginator;
            this.selection = new SelectionModel(true, []);
            this.paginator._intl.itemsPerPageLabel = 'Số đơn hàng mỗi trang: ';
            this.paginator._intl.getRangeLabel = (page, pageSize, length) => {
              if (length === 0 || pageSize === 0 || length === null) {
                return '0 trên ' + length;
              }
              length = Math.max(length, 0);
              const startIndex = page * pageSize;
              // If the start index exceeds the list length, do not try and fix the end index to the end.
              const endIndex = startIndex < length ?
                Math.min(startIndex + pageSize, length) :
                startIndex + pageSize;
              return startIndex + 1 + ' - ' + endIndex + ' trên ' + length;
            };
          }
          this.workflowService.nextWorkflowStep(this.featureCode, this.commonId, '', this.isRejected,
            '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
              const presult = <any>response;
              if (presult.statusCode === 202 || presult.statusCode === 200) {
                this.checkButton(this.commonId);
              }
            }, error => { });
        }
      }
    }, error => { });

  }
  getAllPostion() {
    this.positionService.getAllPosition().subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.arrayPostion = result.listPosition;
      }
    }, error => { });
  }
  importSalaryHandmade() {
    var array = this.IO.split("-");
    this.year = array[0] ? parseInt(array[0]) : null;
    this.month = array[1] ? parseInt(array[1]) : null;

    this.dialogImportSalaryHandmade = this.dialog.open(AssistantSalaryHandmadeImportComponent,
      {
        width: '600px',
        height: '580',
        autoFocus: false,
        data: { Month: this.month, Year: this.year }
      });
    this.dialogImportSalaryHandmade.afterClosed().subscribe(result => {
      if (typeof result !== "undefined") {
        if (result.isTrue) {
          this.dataSource = new MatTableDataSource<any>(result.lstEmployeeMonthySalary);
          this.dataSource.paginator = this.paginator;
          this.selection = new SelectionModel(true, []);
          this.paginator._intl.itemsPerPageLabel = 'Số đơn hàng mỗi trang: ';
          this.paginator._intl.getRangeLabel = (page, pageSize, length) => {
            if (length === 0 || pageSize === 0 || length === null) {
              return '0 trên ' + length;
            }
            length = Math.max(length, 0);
            const startIndex = page * pageSize;
            // If the start index exceeds the list length, do not try and fix the end index to the end.
            const endIndex = startIndex < length ?
              Math.min(startIndex + pageSize, length) :
              startIndex + pageSize;
            return startIndex + 1 + ' - ' + endIndex + ' trên ' + length;
          };
        }
      }
    }, error => { });
  }
  refreshParameter() {
    this.employeeName = "";
    this.employeeCode = "";
    this.employeeUnit = "";
    this.employeePostionId = undefined;
    this.month = this.Current.getMonth() + 1;
    this.year = this.Current.getFullYear();
    this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);
    this.search();
  }
  search() {
    var array = this.IO.split("-");
    this.year = array[0] ? parseInt(array[0]) : null;
    this.month = array[1] ? parseInt(array[1]) : null;

    this.assistantSalaryFindService.findAssistantMonthySalary(this.employeeName, this.employeeCode, this.employeeUnit, this.employeeUnit, this.employeePostionId, this.month, this.year, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.dataSource = new MatTableDataSource<any>(result.lstEmployeeMonthySalary);
        this.dataSource.paginator = this.paginator;
        if (result.lstEmployeeMonthySalary != null && result.lstEmployeeMonthySalary.length > 0) {
          this.commonId = result.lstEmployeeMonthySalary[0].commonId;
          this.noItem = false;
          this.notes = result.notes;
        }
        else {
          this.notes = [];
        }
        this.checkButton(this.commonId);
        this.selection = new SelectionModel(true, []);
        this.paginator._intl.itemsPerPageLabel = 'Số đơn hàng mỗi trang: ';
        this.paginator._intl.getRangeLabel = (page, pageSize, length) => {
          if (length === 0 || pageSize === 0 || length === null) {
            return '0 trên ' + length;
          }
          length = Math.max(length, 0);
          const startIndex = page * pageSize;
          // If the start index exceeds the list length, do not try and fix the end index to the end.
          const endIndex = startIndex < length ?
            Math.min(startIndex + pageSize, length) :
            startIndex + pageSize;
          return startIndex + 1 + ' - ' + endIndex + ' trên ' + length;
        };
      }
    }, error => { });
  }
  // Functions select checkbox on Grid
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
      this.selectedEmpMonthySalaryList = [];
    } else {

      this.selectedEmpMonthySalaryList = [];
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selection.select(item);
        this.selectedEmpMonthySalaryList.push(item.employeeMonthySalaryId);
      });
    }
  }

  highlight(row) {
    this.selectedRowIndex = row.employeeMonthySalaryId;
  }

  rowCheckboxClick(event: MatCheckboxChange, row) {
    if (event) {
      this.selection.toggle(row);
    }

    if (event.checked) {
      this.selectedEmpMonthySalaryList.push(row.employeeMonthySalaryId);
    } else {
      this.selectedEmpMonthySalaryList.splice(this.selectedEmpMonthySalaryList.indexOf(row.employeeMonthySalaryId), 1);
    }
  }

  export() {
    var t = this.selectedEmpMonthySalaryList;
    if (this.selectedEmpMonthySalaryList.length == 0) {
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selectedEmpMonthySalaryList.push(item.employeeMonthySalaryId);
      });
    }
    this.assistantExportListService.exportAssistantSalary(this.selectedEmpMonthySalaryList, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.excelFile);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.nameFile + ".xlsx";
        link.download = fileName;
        link.click();
      }
    }, error => { });
  }

  sendApprove() {
    this.isInApprovalProgress = true;
    this.isRejected = false;
    this.workflowService.nextWorkflowStep(this.featureCode, this.commonId, '', this.isRejected,
      '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          this.search();
        }
      }, error => { });
  }

  goToNextStep(step: string) {
    let _title = "XÁC NHẬN";
    let _content = "";
    let _mode = "";
    let _height = "";
    if (step == 'approve') {
      this.isApproved = true;
      _content = "Bạn có chắc chắn muốn phê duyệt bảng lương này.";
      _height = "250px";
    } else {
      this.isRejected = true;
      _mode = "reject";
      _content = "Bạn có chắc chắn muốn từ chối bảng lương này.";
      _height = "300px";
    }

    this.dialogConfirmPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: _height,
        autoFocus: false,
        data: { title: _title, content: _content, mode: _mode }
      });

    this.dialogConfirmPopup.afterClosed().subscribe(result => {
      if (result || result.ok) {
        this.isInApprovalProgress = false;
        let msg = result.message != null ? result.message : '';
        this.workflowService.nextWorkflowStep(this.featureCode, this.commonId, msg, this.isRejected,
          '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
            const presult = <any>response;
            if (presult.statusCode === 202 || presult.statusCode === 200) {
              this.snackBar.openFromComponent(SuccessComponent, { data: presult.messageCode, ...this.successConfig });
              this.search();
            }
          }, error => { });
      }
    });
  }

  checkButton(id: string) {
    this.employeeSalaryService.getEmployeeSalaryStatus(id, this.auth.Userd).subscribe(response => {
      const result = <any>response;
      this.isApproved = result.isApproved;
      this.isInApprovalProgress = result.isInApprovalProgress;
      this.isRejected = result.isRejected;
      this.statusName = result.statusName;
      this.isApprover = result.approverId == this.auth.EmployeeId;
      this.isPosition = result.positionId == this.auth.PositionId;
    });
  }
  sendEmailAssistantPayslip() {
    var t = this.selectedEmpMonthySalaryList;
    if (this.selectedEmpMonthySalaryList.length == 0) {
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selectedEmpMonthySalaryList.push(item.employeeMonthySalaryId);
      });
    }
    if (this.selectedEmpMonthySalaryList.length > 0) {
      this.emailService.sendEmailAssistantPayslip(this.selectedEmpMonthySalaryList).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: "Đã gửi payslip tới nhân viên", ...this.successConfig });
        }
        else {
          this.snackBar.openFromComponent(FailComponent, { data: "Đã có lỗi xảy ra trong quá trình gửi payslip", ...this.failConfig });
        }
      }, error => { });
    }
  }
  // Open dialog select org
  //openOrgDialog() {
  //  this.dialogOrg = this.dialog.open(OrgSelectMultiDialogComponent,
  //    {
  //      width: '500px',
  //      autoFocus: false,
  //      data: { lstSelection: this.lstOrganizationId }
  //    });

  //  this.dialogOrg.afterClosed().subscribe(result => {
  //    if (true) {
  //      this.lstOrganizationId == new SelectionModel(true, []);
  //      this.lstOrganizationId = result.lstSelection;
  //      //this.employeeModel.OrganizationId = result.selectedId;
  //      this.empOrganizationNameDisplay = '';
  //      this.lstOrganizationId.selected.forEach(rows => {
  //        this.empOrganizationNameDisplay = this.empOrganizationNameDisplay + rows.organizationName + ';';
  //        this.selectedOrganizationId.push(rows.organizationId);
  //      });
  //      if (this.lstOrganizationId.selected.length == 0) {
  //        this.selectedOrganizationId = [];
  //      }
  //    }
  //  });
  //}
}
