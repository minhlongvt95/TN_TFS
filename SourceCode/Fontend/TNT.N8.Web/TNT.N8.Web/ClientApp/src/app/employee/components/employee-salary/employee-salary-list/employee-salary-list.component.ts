import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatCheckboxChange } from '@angular/material/checkbox';

import { SelectionModel } from '@angular/cdk/collections';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { EmployeeTimesheetImportComponent } from '../employee-timesheet-import/employee-timesheet-import.component';
import { EmployeeSalaryHandmadeImportComponent } from '../employee-salary-handmade-import/employee-salary-handmade-import.component';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { OrgSelectMultiDialogComponent } from '../../org-select-multi-dialog/org-select-multi-dialog.component';

import { EmployeeSalaryService } from '../../../services/employee-salary/employee-salary.service';
import { PositionService } from '../../../../shared/services/position.service';
import { EmailService } from '../../../../shared/services/email.service';
import { WorkflowService } from '../../../../admin/components/workflow/services/workflow.service';
import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";

import { isNullOrUndefined } from 'util';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { WarningComponent } from '../../../../shared/toast/warning/warning.component';

@Component({
  selector: 'app-employee-salary-list',
  templateUrl: './employee-salary-list.component.html',
  styleUrls: ['./employee-salary-list.component.css']
})
export class EmployeeSalaryListComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns = ['select', 'employeecode', 'employeename', 'employeepostionid', 'employeeunit', 'employeebranch', 'basedsalary', 'monthlyworkingday', 'unPaidLeaveDay', 'vacationDay', 'overtime', 'actualWorkingDay', 'actualOfSalary', 'overtimeOfSalary',
    'enrollmentSalary', 'retentionSalary', 'fuelAllowance', 'phoneAllowance', 'lunchAllowance', 'otherAllowance', 'socialInsuranceSalary', 'totalInsuranceCompanyPaid', 'totalInsuranceEmployeePaid', 'desciplineAmount',
    'reductionAmount', 'additionalAmount', 'bankAccountCode', 'bankAccountName', 'branchOfBank', 'totalIncome', 'actualPaid'];

  arrayPostion: Array<any>;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  dataSource: any = [];
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  dialogImportTimeSheet: MatDialogRef<EmployeeTimesheetImportComponent>;
  dialogImportSalaryHandmade: MatDialogRef<EmployeeSalaryHandmadeImportComponent>;
  dialogOrg: MatDialogRef<OrgSelectMultiDialogComponent>;

  actionImport: boolean = true;
  actionDownload: boolean = true;
  actionEmail: boolean = true;
  actionEdit: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  selection: SelectionModel<any>;
  selectedEmpMonthySalaryList: any = [];
  selectedRowIndex: string;

  employeeName: string = "";
  employeeCode: string = "";
  employeeUnit: string = "";
  employeePostionId: string = "";
  empOrganizationNameDisplay = '';
  lstOrganizationId: SelectionModel<any>;
  selectedOrganizationId: Array<string> = [];


  month: number;
  year: number;
  Current: Date = new Date();
  IO: string = "";
  featureCode: string = 'PDLNV';
  isApproved: boolean = false;
  isRejected: boolean = false;
  isInApprovalProgress: boolean = false;
  isApprover: boolean = false;
  isPosition: boolean = false;
  dialogConfirmPopup: MatDialogRef<PopupComponent>;

  commonId: string = '00000000-0000-0000-0000-000000000000';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  statusName: string = '';
  noItem: boolean = true;
  notes: any = [];

  @ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  loadingConfig: any = {
    'animationType': ngxLoadingAnimationTypes.circle,
    'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
    'backdropBorderRadius': '4px',
    'primaryColour': '#ffffff',
    'secondaryColour': '#999999',
    'tertiaryColour': '#ffffff'
  }
  loading: boolean = true;

  //khai bao parameter lay tu man hinh dashboardRequest
  MonthSalaryRequestParam: number;

  constructor(
    public cdRef: ChangeDetectorRef,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    public dialog: MatDialog,
    private employeeSalaryService: EmployeeSalaryService,
    private positionService: PositionService,
    private workflowService: WorkflowService,
    public snackBar: MatSnackBar,
    private router: Router,
    private emailService: EmailService,
  ) { }

  ngOnInit() {
    let resource = "hrm/employee/employee-salary/list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("email") == -1) {
        this.actionEmail = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      this.lstOrganizationId = new SelectionModel(true, []);
      this.getAllPostion();
      this.month = this.Current.getMonth() + 1;
      this.year = this.Current.getFullYear();
      this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);
      this.route.params.subscribe(params => { this.MonthSalaryRequestParam = params['MonthSalaryRequestParam']; });
      this.search();
    }
  }

  employeeSalaryView: string = 'employee-salary/view';
  employeeSalaryImport: string = 'employee-salary/import ';
  employeeSalaryExport: string = 'employee-salary/export '
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  
  getAllPostion() {
    this.positionService.getAllPosition().subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.arrayPostion = result.listPosition;
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

  importTimeSheet() {
    this.dialogImportTimeSheet = this.dialog.open(EmployeeTimesheetImportComponent,
      {
        width: '600px',
        height: '580',
        autoFocus: false,
        data: {}
      });
    this.dialogImportTimeSheet.afterClosed().subscribe(result => {
      if (typeof result !== "undefined") {
        if (result.isTrue) {
          this.loading = true;
          this.month = result.Month;
          this.year = result.Year;
          this.IO = this.year.toString() + '-' + (this.month.toString()[1] ? this.month.toString() : "0" + this.month.toString()[0]);

          this.dataSource = new MatTableDataSource<any>(result.lstEmployeeMonthySalary);
          this.dataSource.paginator = this.paginator;
          if (result.lstEmployeeMonthySalary != null && result.lstEmployeeMonthySalary.length > 0) {
            this.commonId = result.lstEmployeeMonthySalary[0].commonId;
            this.noItem = false;
          } else {
            this.commonId = this.emptyGuid;
            this.noItem = true;
          }

          this.workflowService.nextWorkflowStep(this.featureCode, this.commonId, '', this.isRejected,
            '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
              const presult = <any>response;
              if (presult.statusCode === 202 || presult.statusCode === 200) {
                this.checkButton(this.commonId);
                this.loading = false;
              }
            }, error => {
              this.loading = false;
            });
        }
      }
    }, error => {
      this.loading = false;
    });

  }

  importSalaryHandmade() {
    var array = this.IO.split("-");
    this.year = array[0] ? parseInt(array[0]) : null;
    this.month = array[1] ? parseInt(array[1]) : null;

    this.dialogImportSalaryHandmade = this.dialog.open(EmployeeSalaryHandmadeImportComponent,
      {
        width: '600px',
        height: '580',
        autoFocus: false,
        data: { Month: this.month, Year: this.year }
      });
    this.dialogImportSalaryHandmade.afterClosed().subscribe(result => {
      if (typeof result !== "undefined") {
        if (result.isTrue) {
          this.loading = true;
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
          this.loading = false;
        }
      }
    }, error => { });

  }


  // isSticky(column: string): boolean {
  //   return column === 'employeecode' ? true : false;
  // }

  search() {
    this.loading = true;
    var array = this.IO.split("-");
    this.year = array[0] ? parseInt(array[0]) : null;
    this.month = array[1] ? parseInt(array[1]) : null;
    if (!isNullOrUndefined(this.MonthSalaryRequestParam)) {
      this.month = this.MonthSalaryRequestParam;
    }

    this.employeeSalaryService.findEmployeeMonthySalary(this.employeeName, this.employeeCode, this.employeeUnit, this.employeeUnit, this.employeePostionId, this.selectedOrganizationId, this.month, this.year, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.dataSource = new MatTableDataSource<any>(result.lstEmployeeMonthySalary);
        if (result.lstEmployeeMonthySalary != null && result.lstEmployeeMonthySalary.length > 0) {
          this.commonId = result.lstEmployeeMonthySalary[0].commonId;
          this.noItem = false;
          this.notes = result.notes;
        } else {
          this.commonId = this.emptyGuid;
          this.noItem = true;
          this.notes = result.notes;
        }

        this.checkButton(this.commonId);
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

        this.loading = false;
      }
    }, error => {
      this.loading = false;
    });
  }

  downloadTemplateTimeSheet() {
    this.loading = true;
    this.employeeSalaryService.downloadTemplateTimeSheet().subscribe(response => {
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
        this.loading = false;
      }
    }, error => {
      this.loading = false;
    });
  }

  changeEmployeePostion(event) {
    this.employeePostionId = event.value;
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
    this.loading = true;
    var t = this.selectedEmpMonthySalaryList;
    if (this.selectedEmpMonthySalaryList.length == 0) {
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selectedEmpMonthySalaryList.push(item.employeeMonthySalaryId);
      });
    }
    this.employeeSalaryService.exportEmployeeSalary(this.selectedEmpMonthySalaryList, this.auth.UserId).subscribe(response => {
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
        this.loading = false;
      }
    }, error => {
      this.loading = false;
    });
  }

  sendApprove() {
    this.loading = true;
    this.isInApprovalProgress = true;
    this.isRejected = false;
    this.workflowService.nextWorkflowStep(this.featureCode, this.commonId, '', this.isRejected,
      '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          this.search();
        }
      }, error => {
        this.loading = false;
      });
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
      if (result != undefined && (result || result.ok)) {
        this.loading = true;
        this.isInApprovalProgress = false;
        let msg = result.message != null ? result.message : '';
        this.workflowService.nextWorkflowStep(this.featureCode, this.commonId, msg, this.isRejected,
          '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
            const presult = <any>response;
            if (presult.statusCode === 202 || presult.statusCode === 200) {
              this.snackBar.openFromComponent(SuccessComponent, { data: presult.messageCode, ...this.successConfig });
              this.search();
            } else {
              this.loading = false;
            }
          }, error => {
            this.loading = false;
          });
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
  sendEmailEmployeePayslip() {
    this.loading = true;
    var t = this.selectedEmpMonthySalaryList;
    if (this.selectedEmpMonthySalaryList.length == 0) {
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selectedEmpMonthySalaryList.push(item.employeeMonthySalaryId);
      });
    }
    if (this.selectedEmpMonthySalaryList.length > 0) {
      this.emailService.sendEmailEmployeePayslip(this.selectedEmpMonthySalaryList).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: "Đã gửi payslip tới nhân viên", ...this.successConfig });
        }
        else {
          this.snackBar.openFromComponent(FailComponent, { data: "Đã có lỗi xảy ra trong quá trình gửi payslip", ...this.failConfig });
        }

        this.loading = false;
      }, error => {
        this.loading = false;
      });
    }
  }
  // Open dialog select org
  openOrgDialog() {
    this.dialogOrg = this.dialog.open(OrgSelectMultiDialogComponent,
      {
        width: '500px',
        autoFocus: false,
        data: { lstSelection: this.lstOrganizationId }
      });

    this.dialogOrg.afterClosed().subscribe(result => {
      if (true) {
        this.lstOrganizationId == new SelectionModel(true, []);
        this.lstOrganizationId = result.lstSelection;
        //this.employeeModel.OrganizationId = result.selectedId;
        this.empOrganizationNameDisplay = '';
        this.lstOrganizationId.selected.forEach(rows => {
          this.empOrganizationNameDisplay = this.empOrganizationNameDisplay + rows.organizationName + ';';
          this.selectedOrganizationId.push(rows.organizationId);
        });
        if (this.lstOrganizationId.selected.length == 0) {
          this.selectedOrganizationId = [];
        }
      }
    });
  }
}
