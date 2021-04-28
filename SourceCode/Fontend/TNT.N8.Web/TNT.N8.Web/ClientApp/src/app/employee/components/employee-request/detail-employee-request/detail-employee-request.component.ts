import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatAutocompleteSelectedEvent, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { MatSelectChange } from '@angular/material/select';
import { MatChipInputEvent } from '@angular/material/chips';


import { EmployeeService } from "../../../services/employee.service";
import { TranslateService } from '@ngx-translate/core';
import { EmployeeRequestModel } from '../../../models/employee-request.model';
import { CreateEmployeeRequestService } from '../../../services/employee-request/create-employee-request.service';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { CategoryService } from '../../../../shared/services/category.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from "rxjs/operators";
import { Router, ActivatedRoute } from '@angular/router';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { WorkflowService } from '../../../../admin/components/workflow/services/workflow.service';
import { PositionService } from "../../../../shared/services/position.service";
import { AppDateAdapter, APP_DATE_FORMATS } from '../../../../shared/services/date.adapter.service';
import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { GetPermission } from '../../../../shared/permission/get-permission';
import { WarningComponent } from '../../../../shared/toast/warning/warning.component';
import { EmailService } from '../../../../shared/services/email.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-detail-employee-request',
  templateUrl: './detail-employee-request.component.html',
  styleUrls: ['./detail-employee-request.component.css'],
  providers: [
    {
      provide: DateAdapter, useClass: AppDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS
    }
  ]
})
export class DetailEmployeeRequestComponent implements OnInit {
  // @HostListener('document:keyup', ['$event'])
  // handleDeleteKeyboardEvent(event: KeyboardEvent) {
  // 	if (event.key === 'Enter') {
  // 		this.updateEmployeeRequest(false);
  // 	}
  // }
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
  loading: boolean = false;

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  employeeRequestCreatePermission: string = 'employeerequest/create-empRequest'
  employeeRequestEditPermission: string = 'employeerequest/update-empRequest'

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  // Khai báo chung
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = JSON.parse(localStorage.getItem("auth")).UserId;

  selectable: boolean = true;
  removable: boolean = true;

  featureCode: string = 'PDDXXN';
  isApproved: boolean = false;
  isRejected: boolean = false;
  isInApprovalProgress: boolean = false;
  isApprover: boolean = false;
  isPosition: boolean = false;
  dialogConfirmPopup: MatDialogRef<PopupComponent>;
  commonId: string = '00000000-0000-0000-0000-000000000000';
  statusName: string = '';

  // Khai báo các mảng chứa Master Data
  postionList: Array<any> = [];
  createEmpAccountList: Array<any> = [];
  offerEmpAccountList: Array<any> = [];
  managerEmpAccountList: Array<any> = [];
  approveEmpAccountList: Array<any> = [];
  notifyEmpAccountList: Array<any> = [];
  typeReasons: Array<any> = [];
  selectedNotifier: Array<any> = [];

  // Khai bao Observable cho autoComplete
  filteredOptionsCreateEmpAccount: Observable<any>;
  filteredOptionsOfferEmpAccount: Observable<any>;
  filteredOptionsManageEmpAccount: Observable<any>;
  filteredOptionsApproveEmpAccount: Observable<any>;
  filteredOptionsNotifyEmpAccount: Observable<any>;

  startDate: Date;
  endDate: Date;
  selectedStartShift: string = '';
  selectedEndShift: string = '';
  shifts: Array<any> = [];
  createdName = '';
  isCreater: boolean = false;

  // Khai báo các Form Group
  empRequestFormGroup: FormGroup;

  actionEdit: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  // Khai báo các Model
  employeeRequestModel = new EmployeeRequestModel();
  requestId: string = '';
  isViewMode: boolean = true;
  dialogPopup: MatDialogRef<PopupComponent>;
  notes: any = [];
  currentUser: any;

  listIdNotify: string[];
  fullNameNotify: string;
  accountCreate: string;
  fullNameCreate: string;
  requestIdmail: string;
  fullNameRequest: string;
  activeRequest: string;
  accountApprove: string;
  fullNameApprove: string;
  dateCreate: string = '';
  requestType: string;
  dateStart: string = '';
  caStart: string;
  dateEnd: string = '';
  caEnd: string;
  note: string;
  listFullNameNotify: string = '';

  constructor(private translate: TranslateService,
    private datePipe: DatePipe,
    private emailService: EmailService,
    private getPermission: GetPermission,
    private catService: CategoryService,
    public snackBar: MatSnackBar,
    private router: Router,
    public dialogPop: MatDialog,
    private route: ActivatedRoute,
    public dialog: MatDialog,
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private workflowService: WorkflowService,
    private positionService: PositionService,
    private employeeRequestService: CreateEmployeeRequestService) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    let resource = "hrm/employee/request/detail/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      this.loading = true;
      this.empRequestFormGroup = this.formBuilder.group({
        OfferEmployeeName: ['', [Validators.required]],
        typeRequestFormCtr: ['', [Validators.required]],
        startDateFormCtr: ['', [Validators.required]],
        endDateFormCtr: ['', [Validators.required]],
        amShift: [''],
        pmShift: [''],
        amountAbsentFormCtr: [''],
        detailFormCtr: [''],
        ApproverEmployeeName: ['', [Validators.required]],
        notifyListFormCtr: ['']
      });
      this.route.params.subscribe(params => { this.requestId = params['requestId']; });

      this.controlManagement();
      this.getRequestById(this.requestId);
      this.getAllCategory();
      this.getAllEmployeeAccount();
      this.loading = false;
    }
  }

  controlManagement() {
    if (this.isViewMode) {
      this.empRequestFormGroup.controls['OfferEmployeeName'].disable();
      this.empRequestFormGroup.controls['typeRequestFormCtr'].disable();
      this.empRequestFormGroup.controls['startDateFormCtr'].disable();
      this.empRequestFormGroup.controls['endDateFormCtr'].disable();
      this.empRequestFormGroup.controls['amShift'].disable();
      this.empRequestFormGroup.controls['pmShift'].disable();
      this.empRequestFormGroup.controls['amountAbsentFormCtr'].disable();
      this.empRequestFormGroup.controls['detailFormCtr'].disable();
      this.empRequestFormGroup.controls['ApproverEmployeeName'].disable();
      this.empRequestFormGroup.controls['notifyListFormCtr'].disable();
    } else {
      this.empRequestFormGroup.controls['OfferEmployeeName'].enable();
      this.empRequestFormGroup.controls['typeRequestFormCtr'].enable();
      this.empRequestFormGroup.controls['startDateFormCtr'].enable();
      this.empRequestFormGroup.controls['endDateFormCtr'].enable();
      this.empRequestFormGroup.controls['amShift'].enable();
      this.empRequestFormGroup.controls['pmShift'].enable();
      this.empRequestFormGroup.controls['amountAbsentFormCtr'].enable();
      this.empRequestFormGroup.controls['detailFormCtr'].enable();
      this.empRequestFormGroup.controls['ApproverEmployeeName'].enable();
      this.empRequestFormGroup.controls['notifyListFormCtr'].enable();
    }
  }

  getRequestById(id: string) {
    this.notes = [];
    this.employeeRequestService.getEmployeeRequestById(id).subscribe(response => {

      var result = <any>response;
      this.employeeRequestModel.NotifyList = "";
      this.employeeRequestModel = <EmployeeRequestModel>({
        EmployeeRequestId: result.employeeRequest.employeeRequestId,
        EmployeeRequestCode: result.employeeRequest.employeeRequestCode,
        ApproverId: result.employeeRequest.approverId,
        CreateById: result.employeeRequest.createById,
        CreateDate: result.employeeRequest.createDate,
        CreateEmployeeCode: result.employeeRequest.createEmployeeCode,
        CreateEmployeeId: result.employeeRequest.createEmployeeId,
        Detail: result.employeeRequest.detail,
        EnDate: new Date(result.employeeRequest.enDate),
        EndTypeTime: result.employeeRequest.endTypeTime,
        ManagerId: result.employeeRequest.managerId,
        NotifyList: result.employeeRequest.notifyList == null ? '' : result.employeeRequest.notifyList,
        OfferEmployeeCode: result.employeeRequest.offerEmployeeCode,
        OfferEmployeeId: result.employeeRequest.offerEmployeeId,
        RequestDate: result.employeeRequest.requestDate,
        StartDate: new Date(result.employeeRequest.startDate),
        StartTypeTime: result.employeeRequest.startTypeTime,
        StatusId: result.employeeRequest.statusId,
        TypeReason: result.employeeRequest.typeReason,
        TypeRequest: result.employeeRequest.typeRequest,
        UpdateById: result.employeeRequest.updateById,
        UpdateDate: result.employeeRequest.updateDate,
        ShiftName: result.employeeRequest.shiftName
      });

      this.isApproved = result.isApproved;
      this.isInApprovalProgress = result.isInApprovalProgress;
      this.isRejected = result.isRejected;
      this.statusName = result.statusName;
      this.isApprover = result.employeeRequest.approverId == this.auth.EmployeeId;
      this.isPosition = result.positionId == this.auth.PositionId;
      this.isCreater = result.employeeRequest.createById == this.auth.UserId;
      this.notes = result.notes;
      this.compareTwoDate();
      var nameOffer = result.employeeRequest.offerEmployeeCode + " - " + result.employeeRequest.offerEmployeeName;
      this.empRequestFormGroup.controls['OfferEmployeeName'].setValue(nameOffer);

      var approveUser = this.createEmpAccountList.find(item => item.employeeId == this.employeeRequestModel.ApproverId);
      if (approveUser !== undefined) {
        var nameApprove = approveUser.employeeCode + " - " + approveUser.employeeName;
        this.fullNameRequest = nameOffer;
        this.empRequestFormGroup.controls['ApproverEmployeeName'].setValue(nameApprove);
      }

      const notifyListAr = this.employeeRequestModel.NotifyList.split(', ');
      let _notifyList = this.createEmpAccountList
      notifyListAr.forEach(nt => {
        let tmp = _notifyList.find(emp => emp.employeeId == nt);
        if (tmp !== undefined) {
          this.selectedNotifier.push(tmp);
          _notifyList.splice(_notifyList.indexOf(tmp), 1);
        }

      });

      this.accountApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId).employeeCode;
      this.fullNameApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId).employeeName;

      this.filteredOptionsNotifyEmpAccount = this.empRequestFormGroup.controls['notifyListFormCtr'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, _notifyList) : _notifyList.slice())
        );
    }, error => { });
  }

  dateDiffDay(date1, date2) {
    let dt1 = new Date(date1);
    let dt2 = new Date(date2);
    return Math.floor((Date.UTC(dt2.getFullYear(), dt2.getMonth(), dt2.getDate()) - Date.UTC(dt1.getFullYear(), dt1.getMonth(), dt1.getDate())) / (1000 * 60 * 60 * 24));
  }

  getAllEmployeeAccount() {
    this.employeeService.getAllEmployee().subscribe(response => {
      var result = <any>response;
      this.createEmpAccountList = result.employeeList;
      this.currentUser = this.createEmpAccountList.find(item => item.employeeId == this.auth.EmployeeId);

      // Set value from data was gotten
      var createUser = this.createEmpAccountList.find(item => item.employeeId == this.employeeRequestModel.CreateEmployeeId);
      var offerUser = this.createEmpAccountList.find(item => item.employeeId == this.employeeRequestModel.OfferEmployeeId);
      var approveUser = this.createEmpAccountList.find(item => item.employeeId == this.employeeRequestModel.ApproverId);
      var nameValue = createUser.employeeCode + " - " + createUser.employeeName;
      this.createdName = nameValue;
      var nameOffer = offerUser.employeeCode + " - " + offerUser.employeeName;
      var nameApprove = approveUser.employeeCode + " - " + approveUser.employeeName;
      this.fullNameRequest = nameOffer;
      this.selectedNotifier = [];
      const notifyListAr = this.employeeRequestModel.NotifyList.split(', ');
      let _notifyList = this.createEmpAccountList
      notifyListAr.forEach(nt => {
        let tmp = _notifyList.find(emp => emp.employeeId == nt);
        if (tmp !== undefined) {
          this.selectedNotifier.push(tmp);
          _notifyList.splice(_notifyList.indexOf(tmp), 1);
        }

      });
      this.selectedStartShift = this.employeeRequestModel.ShiftName.split(",")[0];
      this.selectedEndShift = this.employeeRequestModel.ShiftName.split(",")[1];
      this.compareTwoDate();
      this.empRequestFormGroup.controls['OfferEmployeeName'].setValue(nameOffer);
      this.empRequestFormGroup.controls['ApproverEmployeeName'].setValue(nameApprove);
      // End

      this.offerEmpAccountList = Object.assign([], this.createEmpAccountList);
      this.managerEmpAccountList = Object.assign([], this.createEmpAccountList);
      this.approveEmpAccountList = Object.assign([], this.createEmpAccountList);
      this.notifyEmpAccountList = Object.assign([], this.createEmpAccountList);
      // Filter các autocomplete
      this.filteredOptionsOfferEmpAccount = this.empRequestFormGroup.controls['OfferEmployeeName'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.createEmpAccountList) : this.createEmpAccountList.slice())
        );

      this.filteredOptionsNotifyEmpAccount = this.empRequestFormGroup.controls['notifyListFormCtr'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, _notifyList) : _notifyList.slice())
        );
    }, error => { });

    this.employeeService.getEmployeeToApprove(this.auth.EmployeeId, "EMP_REQUEST").subscribe(response => {
      const result = <any>response;
      this.approveEmpAccountList = result.listEmployeeToApprove
      if (result.listEmployeeToApprove.length > 0) {
        this.empRequestFormGroup.controls['ApproverEmployeeName'].setValue(result.listEmployeeToApprove[0].employeeCode + " - " + result.listEmployeeToApprove[0].employeeName)
        this.employeeRequestModel.ApproverId = result.listEmployeeToApprove[0].employeeId;
      }

      this.filteredOptionsApproveEmpAccount = this.empRequestFormGroup.controls['ApproverEmployeeName'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.approveEmpAccountList) : this.approveEmpAccountList.slice())
        );
    });
  }
  getAllCategory() {
    this.catService.getAllCategoryByCategoryTypeCode('LXU').subscribe(response => {
      const result = <any>response;
      this.typeReasons = result.category;
    }, error => { });

    this.catService.getAllCategoryByCategoryTypeCode('CVI').subscribe(response => {
      const result = <any>response;
      this.shifts = result.category;
    }, error => { });;
  }
  private _filterEmployeeAccount(value: string, array): string[] {
    const filterValue = value.toString().toLowerCase();

    return array.filter(option => option.employeeName.toLowerCase().includes(filterValue) || option.employeeCode.toLowerCase().includes(filterValue));
  }

  updateEmployeeRequest(sendApprove: boolean) {
    if (this.empRequestFormGroup.invalid) {
      return;
    } else {
      this.employeeRequestModel.NotifyList = "";
      this.selectedNotifier.forEach(item => {
        if (this.selectedNotifier.indexOf(item) < this.selectedNotifier.length - 1) {
          this.employeeRequestModel.NotifyList = this.employeeRequestModel.NotifyList + item.employeeId + ',';
        } else {
          this.employeeRequestModel.NotifyList = this.employeeRequestModel.NotifyList + item.employeeId
        }
      })
      this.employeeRequestService.updateEmployeeRequest(this.employeeRequestModel).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          if (sendApprove) {
            this.sendApprove();
          } else {
            this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
            this.isViewMode = true;
            this.controlManagement();
            this.getRequestById(this.requestId);
            this.getAllEmployeeAccount();
          }
        };
      },
        error => {
          let result = <any>error;
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        });
    }
  }

  /*Quay tro lai List*/
  goBack() {
    if (this.empRequestFormGroup.dirty) {
      let _title = "XÁC NHẬN";
      let _content = "Bạn có chắc chắn hủy? Các dữ liệu sẽ không được lưu.";
      this.dialogPopup = this.dialogPop.open(PopupComponent,
        {
          width: '500px',
          height: '250px',
          autoFocus: false,
          data: { title: _title, content: _content }
        });

      this.dialogPopup.afterClosed().subscribe(result => {
        if (result) {
          this.router.navigate(['/employee/request/list']);
        }
      });
    } else {
      this.router.navigate(['/employee/request/list']);
    }
  }
  /*Ket thuc*/

  selectOfferEmployee(event: MatAutocompleteSelectedEvent) {
    this.employeeRequestModel.OfferEmployeeId = event.option.value.employeeId;
    this.empRequestFormGroup.controls['OfferEmployeeName'].setValue(event.option.viewValue);
    this.employeeRequestModel.OfferEmployeeCode = event.option.value.employeeCode;
    this.employeeService.getEmployeeToApprove(this.auth.EmployeeId, "EMP_REQUEST").subscribe(response => {
      const result = <any>response;
      this.approveEmpAccountList = result.listEmployeeToApprove
      if (result.listEmployeeToApprove.length > 0) {
        this.empRequestFormGroup.controls['ApproverEmployeeName'].setValue(result.listEmployeeToApprove[0].employeeCode + " - " + result.listEmployeeToApprove[0].employeeName)
        this.employeeRequestModel.ApproverId = result.listEmployeeToApprove[0].employeeId;
      }
      this.filteredOptionsApproveEmpAccount = this.empRequestFormGroup.controls['ApproverEmployeeName'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.approveEmpAccountList) : this.approveEmpAccountList.slice())
        );
    });
  }

  selectApproverEmployee(event: MatAutocompleteSelectedEvent) {
    this.employeeRequestModel.ApproverId = event.option.value.employeeId;
    this.empRequestFormGroup.controls['ApproverEmployeeName'].setValue(event.option.viewValue);
  }

  compareTwoDate() {
    if (this.employeeRequestModel.EnDate != null && this.employeeRequestModel.StartDate != null && this.employeeRequestModel.StartDate.setHours(0, 0, 0, 0) == this.employeeRequestModel.EnDate.setHours(0, 0, 0, 0) &&
      this.selectedStartShift == "CHI" && this.selectedEndShift == "SAN") {
      this.empRequestFormGroup.controls['amShift'].setErrors({ "invalidShift": true });
      this.empRequestFormGroup.controls['pmShift'].setErrors({ "invalidShift": true });
    }
    else {
      this.empRequestFormGroup.controls['amShift'].setErrors(null);
      this.empRequestFormGroup.controls['pmShift'].setErrors(null);
    }

    const one_day = 1000 * 60 * 60 * 24;
    if (this.employeeRequestModel.StartDate != null && this.employeeRequestModel.EnDate != null) {
      let acountAbsent = this.employeeRequestModel.EnDate.setHours(0, 0, 0, 0) - this.employeeRequestModel.StartDate.setHours(0, 0, 0, 0);
      acountAbsent = acountAbsent / one_day + 1;
      if ((this.selectedStartShift !== '' && this.selectedEndShift !== '') && (this.selectedStartShift === this.selectedEndShift)) {
        acountAbsent -= 0.5;
      } else if (this.selectedStartShift !== this.selectedEndShift && this.selectedStartShift == "CHI") {
        acountAbsent -= 1;
      }

      this.empRequestFormGroup.controls['amountAbsentFormCtr'].setValue(acountAbsent);
    }
  }

  onChangeStartDate(event: MatSelectChange) {
    const _shift = this.shifts.find(s => s.categoryId == event.value);
    if (_shift != undefined) {
      this.selectedStartShift = _shift.categoryCode;
      this.compareTwoDate();
    }
  }

  onChangeEndDate(event: MatSelectChange) {
    const _shift = this.shifts.find(s => s.categoryId == event.value);
    if (_shift != undefined) {
      this.selectedEndShift = _shift.categoryCode;
      this.compareTwoDate();
    }
  }

  // Filter Mat chip Auto Complete
  removeNotifier(item): void {
    const index = this.selectedNotifier.indexOf(item);

    if (index >= 0) {
      this.selectedNotifier.splice(index, 1);
      this.notifyEmpAccountList.push(item);
      this.filteredOptionsNotifyEmpAccount = this.empRequestFormGroup.controls['notifyListFormCtr'].valueChanges.pipe(
        startWith(null),
        map((item: string) => item ? this._filterEmployeeAccount(item, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice()));
    }
  }

  addNotifier(event: MatChipInputEvent): void {
    this.empRequestFormGroup.controls['notifyListFormCtr'].setValue(null);
  }

  @ViewChild('notifyInput', { static: true }) notifyInput: ElementRef;
  @ViewChild('notifyInput', { read: MatAutocompleteTrigger, static: true }) notifyInputTrigger: MatAutocompleteTrigger;

  selectedReasonFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedNotifier.push(event.option.value);
    this.notifyInput.nativeElement.value = '';
    this.empRequestFormGroup.controls['notifyListFormCtr'].setValue(null);
    this.notifyEmpAccountList.splice(this.notifyEmpAccountList.indexOf(event.option.value), 1);
    this.filteredOptionsNotifyEmpAccount = this.empRequestFormGroup.controls['notifyListFormCtr'].valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filterEmployeeAccount(item, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice()));
    // Van hien thi suggestion panel sau khi select
    const self = this;
    setTimeout(function () {
      self.notifyInputTrigger.openPanel();
    }, 1);
  }

  cancel() {
    if (this.empRequestFormGroup.dirty) {
      let _title = "XÁC NHẬN";
      let _content = "Bạn có chắc chắn hủy? Các dữ liệu sẽ không được lưu.";
      this.dialogPopup = this.dialogPop.open(PopupComponent,
        {
          width: '500px',
          height: '250px',
          autoFocus: false,
          data: { title: _title, content: _content }
        });

      this.dialogPopup.afterClosed().subscribe(result => {
        if (result) {
          this.isViewMode = true;
          this.controlManagement();
          this.getAllEmployeeAccount();
        }
      });
    } else {
      this.isViewMode = true;
      this.controlManagement();
    }
  }

  changeMode() {
    this.isViewMode = false;
    this.controlManagement();
  }
  getParamRequestEmail() {
    this.selectedNotifier.forEach(item => {
      if (this.selectedNotifier.indexOf(item) < this.selectedNotifier.length - 1) {
        this.listFullNameNotify = this.listFullNameNotify + item.employeeCode + "-" + item.employeeName + ', ';
      } else {
        this.listFullNameNotify = this.listFullNameNotify + item.employeeCode + "-" + item.employeeName
      }
    })
    this.requestType = this.typeReasons.find(r => r.categoryId == this.employeeRequestModel.TypeRequest).categoryName;
    this.accountCreate = this.currentUser.employeeCode;
    this.fullNameCreate = this.currentUser.employeeName;
    //this.emailAddressApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId);
    this.note = this.employeeRequestModel.Detail;
    //this.accountApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId).employeeCode;
    //this.fullNameApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId).employeeName;

    var dateNow = new Date();
    this.dateCreate = this.datePipe.transform(this.employeeRequestModel.RequestDate, 'dd-MM-yyyy');
    this.dateStart = this.datePipe.transform(this.employeeRequestModel.StartDate, 'dd-MM-yyyy');
    this.dateEnd = this.datePipe.transform(this.employeeRequestModel.EnDate, 'dd-MM-yyyy');
    //this.fullNameRequest = this.employeeRequestModel.OfferEmployeeCode + '-' + this.offerEmpAccountList.find(a => a.employeeCode == this.employeeRequestModel.OfferEmployeeCode).employeeName;
    this.caStart = this.employeeRequestModel.StartTypeTime == null ? 'Sáng' : this.shifts.find(s => s.categoryId == this.employeeRequestModel.StartTypeTime).categoryName;
    this.caEnd = this.employeeRequestModel.EndTypeTime == null ? 'Chiều' : this.shifts.find(s => s.categoryId == this.employeeRequestModel.EndTypeTime).categoryName;
    this.employeeRequestModel.StartTypeTime = this.employeeRequestModel.StartTypeTime == null ? this.shifts.find( s=> s.categoryCode == "SAN").categoryId : this.employeeRequestModel.StartTypeTime;
    this.employeeRequestModel.EndTypeTime = this.employeeRequestModel.EndTypeTime == null ? this.shifts.find(s => s.categoryCode == "CHI").categoryId : this.employeeRequestModel.EndTypeTime;
    //this.emailAddressApprove = 'ngocpt@tringhiatech.vn';
    this.listIdNotify = this.employeeRequestModel.NotifyList.split(', ');

  }
  sendApprove() {
    this.isInApprovalProgress = true;
    this.isRejected = false;
    this.isApproved = false;
    this.getParamRequestEmail();
    this.workflowService.nextWorkflowStep(this.featureCode, this.requestId, '', this.isRejected,
      '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          this.isViewMode = true;
          this.controlManagement();
          this.getRequestById(this.requestId);

          this.emailService.sendEmailPersonApprove(this.employeeRequestModel.ApproverId, this.requestIdmail, this.fullNameRequest,
            this.activeRequest, this.accountApprove, this.fullNameApprove, this.dateCreate, this.requestType, this.dateStart,
            this.caStart, this.dateEnd, this.caEnd, this.note, this.listFullNameNotify, this.requestId).subscribe(response => {
              var resultEmail = <any>response;
            });

          this.emailService.sendEmailPersonNotify(this.listIdNotify, this.accountCreate, this.fullNameCreate, this.requestIdmail,
            this.fullNameRequest, this.accountApprove, this.fullNameApprove, this.dateCreate, this.requestType, this.dateStart,
            this.caStart, this.dateEnd, this.caEnd, this.note, this.listFullNameNotify, this.requestId).subscribe(response => {
              var resultEmail = <any>response;
            });
        }
      }, error => { });
  }

  goToNextStep(step: string) {
    this.getParamRequestEmail();
    let _title = "XÁC NHẬN";
    let _content = "";
    let _mode = "";
    if (step == 'approve') {
      this.isApproved = true;
      this.activeRequest = 'Phê duyệt';
      _content = "Bạn có chắc chắn muốn phê duyệt đề xuất này.";
    } else {
      this.isRejected = true;
      _mode = "reject";
      this.activeRequest = 'Từ chối';
      _content = "Bạn có chắc chắn muốn từ chối đề xuất này.";
    }

    this.dialogConfirmPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: _title, content: _content, mode: _mode }
      });

    this.dialogConfirmPopup.afterClosed().subscribe(result => {
      if (result || result.ok) {
        let msg = result.message != null ? result.message : '';
        this.workflowService.nextWorkflowStep(this.featureCode, this.requestId, msg, this.isRejected,
          '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
            const presult = <any>response;
            if (presult.statusCode === 202 || presult.statusCode === 200) {
              this.snackBar.openFromComponent(SuccessComponent, { data: presult.messageCode, ...this.successConfig });
              this.isViewMode = true;
              this.controlManagement();
              this.getRequestById(this.requestId);

              this.emailService.sendEmailPersonCreate(this.employeeRequestModel.OfferEmployeeId, this.fullNameCreate, this.requestIdmail, 
                this.fullNameRequest, this.activeRequest, this.accountApprove, this.fullNameApprove, this.dateCreate, this.requestType, this.dateStart,
                this.caStart, this.dateEnd, this.caEnd, this.note, this.listFullNameNotify, this.requestId).subscribe(response => {
                  var resultEmail = <any>response;
                });
            }
          }, error => { });
      } else {
        this.isRejected = false;
        this.isApproved = false;
      }
    });
  }

  checkApprover() {
    return (this.auth.EmployeeId == this.employeeRequestModel.ApproverId);
  }

  checkEditButton() {
    return (this.auth.UserId == this.employeeRequestModel.CreateById);
  }
  // Filter chon từ ngay < den ngày
  filterStartedDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.employeeRequestModel.EnDate === null ? 0 : this.employeeRequestModel.EnDate.setHours(0, 0, 0, 0);

    return (n <= m || m === 0);
  }

  filterEndDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.employeeRequestModel.StartDate === null ? 0 : this.employeeRequestModel.StartDate.setHours(0, 0, 0, 0);

    return (n >= m || m === 0);
  }
  // Ket thuc
}
