import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';

import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatAutocompleteSelectedEvent, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatSelectChange } from '@angular/material/select';

import { EmployeeService } from "../../../services/employee.service";
import { TranslateService } from '@ngx-translate/core';
import { EmployeeRequestModel } from '../../../models/employee-request.model';
import { CreateEmployeeRequestService } from '../../../services/employee-request/create-employee-request.service';
import { PositionService } from "../../../../shared/services/position.service";
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { CategoryService } from '../../../../shared/services/category.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from "rxjs/operators";
import { Router, ActivatedRoute } from '@angular/router';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { WorkflowService } from '../../../../admin/components/workflow/services/workflow.service';
import { AppDateAdapter, APP_DATE_FORMATS } from '../../../../shared/services/date.adapter.service';
import { ListEmployeeRequestService } from '../../../services/employee-request/list-employee-request.service';
import { EmployeeAllowanceService } from '../../../services/employee-allowance/employee-allowance.service';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { WarningComponent } from '../../../../shared/toast/warning/warning.component';
import { EmailService } from '../../../../shared/services/email.service';
import { DatePipe } from '@angular/common';
import { EmailConfigService } from '../../../../admin/services/email-config.service';


@Component({
  selector: 'app-create-employee-request',
  templateUrl: './create-employee-request.component.html',
  styleUrls: ['./create-employee-request.component.css'],
  providers: [
    {
      provide: DateAdapter, useClass: AppDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS
    }
  ]
})
export class CreateEmployeeRequestComponent implements OnInit {
  // @HostListener('document:keyup', ['$event'])
  // handleDeleteKeyboardEvent(event: KeyboardEvent) {
  //   if (event.key === 'Enter') {
  //     this.createEmployeeRequest(false);
  //   }
  // }
  currentDate = new Date();
  currentUser: any;

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
  selectedStartShift: string = '';
  selectedEndShift: string = '';
  createName = '';

  // Khai báo các mảng chứa Master Data
  postionList: Array<any> = [];
  createEmpAccountList: Array<any> = [];
  offerEmpAccountList: Array<any> = [];
  managerEmpAccountList: Array<any> = [];
  approveEmpAccountList: Array<any> = [];
  notifyEmpAccountList: Array<any> = [];
  typeReasons: Array<any> = [];
  shifts: Array<any> = [];
  selectedNotifier: Array<any> = [];
  listOfferEmpAccount: Array<any> = []; // danh sách chỉ có giảng viên
  listApproveEmpAccount: Array<any> = []; // danh sách chứa quản lý của OfferEmployeeRequest
  listIdNotify: string[];
  fullNameNotify: string;
  accountCreate: string;
  fullNameCreate: string;
  requestId: string;
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
  employeeRequestId: string;
  offerEmpName: string;

  // Khai bao Observable cho autoComplete
  filteredOptionsCreateEmpAccount: Observable<any>;
  filteredOptionsOfferEmpAccount: Observable<any>;
  filteredOptionsManageEmpAccount: Observable<any>;
  filteredOptionsApproveEmpAccount: Observable<any>;
  filteredOptionsNotifyEmpAccount: Observable<any>;


  startDate: Date;
  endDate: Date;
  // Khai báo các Form Group
  createEmpRequestFormGroup: FormGroup;

  /* Khai bao FormControl nay ko con phu hop cho Angular 5 */
  // CreateEmployeeName = new FormControl();
  // OfferEmployeeName = new FormControl();
  // ManagerEmployeeName = new FormControl();
  // ApproverEmployeeName = new FormControl();

  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  // Khai báo các Model
  employeeRequestModel = new EmployeeRequestModel();
  dialogPopup: MatDialogRef<PopupComponent>;

  constructor(private translate: TranslateService,
    private datePipe: DatePipe,
    private getPermission: GetPermission,
    private catService: CategoryService,
    private emailService: EmailService,
    public snackBar: MatSnackBar,
    private router: Router,
    public dialogPop: MatDialog,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private workflowService: WorkflowService,
    private positionService: PositionService,
    private empAlowanceService: EmployeeAllowanceService,
    private lstEmpRqService: ListEmployeeRequestService,
    private employeeRequestService: CreateEmployeeRequestService,
    private emailConfigService: EmailConfigService) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    let resource = "hrm/employee/request/create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.createEmpRequestFormGroup = this.formBuilder.group({
        CreateEmployeeName: [''],
        CreateEmployeeCode: [''],
        OfferEmployeeName: ['', [Validators.required]],
        typeRequestFormCtr: ['', [Validators.required]],
        startDateFormCtr: ['', [Validators.required]],
        endDateFormCtr: ['', [Validators.required]],
        amShift: [''],
        pmShift: [''],
        amountAbsentFormCtr: [{ value: "0", disabled: true }],
        detailFormCtr: [''],
        ApproverEmployeeName: ['', [Validators.required]],
        notifyListFormCtr: ['']
      });
      this.getAllEmployeeAccount();

      this.getAllCategory();

      this.createEmpRequestFormGroup.controls['CreateEmployeeName'].disable();
      this.createEmpRequestFormGroup.controls['CreateEmployeeCode'].disable();
    }
  }

  async getAllEmployeeAccount() {

    await this.employeeService.getAllEmployee().subscribe(response => {
      var result = <any>response;
      this.createEmpAccountList = result.employeeList;
      //Set default value cho nhân viên tạo và đề xuất cho
      this.currentUser = this.createEmpAccountList.find(item => item.employeeId == this.auth.EmployeeId);
      var nameValue = this.currentUser.employeeCode + " - " + this.currentUser.employeeName;
      this.createName = nameValue;
      this.createEmpRequestFormGroup.controls['OfferEmployeeName'].setValue(nameValue);
      this.employeeRequestModel.CreateEmployeeCode = this.currentUser.employeeCode;
      this.employeeRequestModel.OfferEmployeeCode = this.currentUser.employeeCode;
      this.employeeRequestModel.CreateEmployeeId = this.currentUser.employeeId;
      this.employeeRequestModel.OfferEmployeeId = this.currentUser.employeeId;

      this.managerEmpAccountList = Object.assign([], this.createEmpAccountList);
      //this.notifyEmpAccountList = Object.assign([], this.createEmpAccountList);
      //// Filter các autocomplete

      //this.filteredOptionsNotifyEmpAccount = this.createEmpRequestFormGroup.controls['notifyListFormCtr'].valueChanges
      //  .pipe(
      //    startWith(''),
      //    map(name => name ? this._filterEmployeeAccount(name, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice())
      //);

    }, error => { });


    await this.employeeService.getEmployeeToApprove(this.auth.EmployeeId, "EMP_REQUEST").subscribe(response => {
      const result = <any>response;
      this.approveEmpAccountList = result.listEmployeeToApprove
      if (result.listEmployeeToApprove.length > 0) {
        this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].setValue(result.listEmployeeToApprove[0].employeeCode + " - " + result.listEmployeeToApprove[0].employeeName)
        this.employeeRequestModel.ApproverId = result.listEmployeeToApprove[0].employeeId;

      }
      this.notifyEmpAccountList = result.listEmployeeToNotify;
      // Filter các autocomplete

      this.filteredOptionsNotifyEmpAccount = this.createEmpRequestFormGroup.controls['notifyListFormCtr'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice())
        );
      this.filteredOptionsApproveEmpAccount = this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.approveEmpAccountList) : this.approveEmpAccountList.slice())
        );
    });

    await this.employeeService.getEmployeeByPositionCode("NV").subscribe(response => {
      const result = <any>response;
      this.offerEmpAccountList = result.employeeList

      this.filteredOptionsOfferEmpAccount = this.createEmpRequestFormGroup.controls['OfferEmployeeName'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.offerEmpAccountList) : this.offerEmpAccountList.slice())
        );

    });
  }
  // Nghi phep:
  absentwithpermisstion = '';
  getAllCategory() {
    this.employeeRequestModel.StartDate = <Date>null;
    this.employeeRequestModel.EnDate = <Date>null
    this.catService.getAllCategoryByCategoryTypeCode('LXU').subscribe(response => {
      var result = <any>response;
      this.typeReasons = result.category;
      this.absentwithpermisstion = this.typeReasons.find(r => r.categoryCode == "NP").categoryId;
    }, error => { });

    this.catService.getAllCategoryByCategoryTypeCode('CVI').subscribe(response => {
      var result = <any>response;
      this.shifts = result.category;
      const _defaultShift = this.shifts.find(sh => sh.categoryCode.trim() == "AM");
      if (_defaultShift != undefined) {
        this.employeeRequestModel.StartTypeTime = _defaultShift.categoryId;
        this.selectedStartShift = _defaultShift.categoryCode;
        this.employeeRequestModel.EndTypeTime = _defaultShift.categoryId;
        this.selectedEndShift = _defaultShift.categoryCode;
      }
    }, error => { });;
  }
  private _filterEmployeeAccount(value: string, array): string[] {
    const filterValue = value.toString().toLowerCase();

    return array.filter(option => option.employeeName.toLowerCase().includes(filterValue) || option.employeeCode.toLowerCase().includes(filterValue));
  }

  createEmployeeRequest(sendApproveAfterCreate: boolean) {
    if (this.createEmpRequestFormGroup.invalid || (this.maternityAllowance - this.amountAbsentWithPermission <= 0 && this.requestWithPermission)) {
      return;
    } else {
      var dateNow = new Date();
      this.employeeRequestModel.StartDate.setHours(dateNow.getHours());
      this.employeeRequestModel.StartDate.setMinutes(dateNow.getMinutes());
      this.employeeRequestModel.StartDate.setSeconds(dateNow.getSeconds());
      //
      this.employeeRequestModel.EnDate.setHours(dateNow.getHours());
      this.employeeRequestModel.EnDate.setMinutes(dateNow.getMinutes());
      this.employeeRequestModel.EnDate.setSeconds(dateNow.getSeconds());

      this.employeeRequestModel.CreateDate = new Date();
      this.employeeRequestModel.CreateById = this.userId;
      this.employeeRequestModel.NotifyList = '';
      this.selectedNotifier.forEach(item => {
        if (this.selectedNotifier.indexOf(item) < this.selectedNotifier.length - 1) {
          this.employeeRequestModel.NotifyList = this.employeeRequestModel.NotifyList + item.employeeId + ', ';
          this.listFullNameNotify = this.listFullNameNotify + item.employeeCode + "-" + item.employeeName + ', ';
        } else {
          this.employeeRequestModel.NotifyList = this.employeeRequestModel.NotifyList + item.employeeId;
          this.listFullNameNotify = this.listFullNameNotify + item.employeeCode + "-" + item.employeeName
        }
      })
      this.requestType = this.typeReasons.find(r => r.categoryId == this.employeeRequestModel.TypeRequest).categoryName;
      this.accountCreate = this.currentUser.employeeCode;
      this.fullNameCreate = this.currentUser.employeeName;
      //this.emailAddressApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId);
      this.note = this.employeeRequestModel.Detail;
      this.accountApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId).employeeCode;
      this.fullNameApprove = this.approveEmpAccountList.find(a => a.employeeId == this.employeeRequestModel.ApproverId).employeeName;

      this.dateCreate = this.datePipe.transform(dateNow, 'dd-MM-yyyy');
      this.dateStart = this.datePipe.transform(this.employeeRequestModel.StartDate, 'dd-MM-yyyy');
      this.dateEnd = this.datePipe.transform(this.employeeRequestModel.EnDate, 'dd-MM-yyyy');
      this.fullNameRequest = this.employeeRequestModel.OfferEmployeeCode + '-' + this.offerEmpAccountList.find(a => a.employeeCode == this.employeeRequestModel.OfferEmployeeCode).employeeName;
      this.caStart = this.employeeRequestModel.StartTypeTime == null ? 'Sáng' : this.shifts.find(s => s.categoryId == this.employeeRequestModel.StartTypeTime).categoryName;
      this.caEnd = this.employeeRequestModel.EndTypeTime == null ? 'Chiều' : this.shifts.find(s => s.categoryId == this.employeeRequestModel.EndTypeTime).categoryName;
      this.employeeRequestModel.StartTypeTime = this.employeeRequestModel.StartTypeTime == null ? '2E840479-5C8F-42C2-92D4-2F1089D50DDC' : this.employeeRequestModel.StartTypeTime;
      this.employeeRequestModel.EndTypeTime = this.employeeRequestModel.EndTypeTime == null ? 'B45FDB66-6360-49A9-A233-6692805DA640' : this.employeeRequestModel.EndTypeTime;
      //this.emailAddressApprove = 'ngocpt@tringhiatech.vn';
      this.listIdNotify = this.employeeRequestModel.NotifyList.split(', ');
      this.employeeRequestService.createEmployeeRequest(this.employeeRequestModel).subscribe(response => {
        let result = <any>response;
        //gui email với button "Gửi phê duyệt"
        if ((result.statusCode === 202 || result.statusCode === 200)) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          this.requestId = result.employeeRequestCode;
          this.employeeRequestId = result.employeeRequestId;
          //Tao buoc dau tien trong FeatureWorkflowProgress
          this.workflowService.nextWorkflowStep(this.featureCode, result.employeeRequestId, '', this.isRejected,
            '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
              const presult = <any>response;
              if (presult.statusCode === 202 || presult.statusCode === 200) {
                //Neu user bam button Gui phe duyet
                if (sendApproveAfterCreate) {
                  this.emailConfigService.sendEmail(9, result.sendEmaiModel).subscribe(reponse => {
                    //let result = <any>response;
                  });
                  this.isInApprovalProgress = true;
                  this.workflowService.nextWorkflowStep(this.featureCode, result.employeeRequestId, '', this.isRejected,
                    '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
                      const resultW = <any>response;
                      if (resultW.statusCode === 202 || resultW.statusCode === 200) {
                        this.snackBar.openFromComponent(SuccessComponent, { data: resultW.messageCode, ...this.successConfig });

                        // this.emailService.sendEmailPersonApprove(this.employeeRequestModel.ApproverId, this.requestId, this.fullNameRequest,
                        //   this.activeRequest, this.accountApprove, this.fullNameApprove, this.dateCreate, this.requestType, this.dateStart,
                        //   this.caStart, this.dateEnd, this.caEnd, this.note, this.listFullNameNotify, this.employeeRequestId).subscribe(response => {
                        //     var resultEmail = <any>response;
                        //   });

                        // this.emailService.sendEmailPersonNotify(this.listIdNotify, this.accountCreate, this.fullNameCreate, this.requestId,
                        //   this.fullNameRequest, this.accountApprove, this.fullNameApprove, this.dateCreate, this.requestType, this.dateStart,
                        //   this.caStart, this.dateEnd, this.caEnd, this.note, this.listFullNameNotify, this.employeeRequestId).subscribe(response => {
                        //     var resultEmail = <any>response;
                        //   });
                      }
                    }, error => { });
                  this.router.navigate(['/employee/request/list']);
                }
                else {
                  this.router.navigate(['/employee/request/list']);
                }
              }
            }, error => { });
        } else {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        }
      },
        error => {
          let result = <any>error;
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        });

    }
  }

  /*Quay tro lai List*/
  goBack() {
    if (this.createEmpRequestFormGroup.dirty) {
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

  selectCreateEmployee(event: MatAutocompleteSelectedEvent) {
    this.employeeRequestModel.CreateEmployeeId = event.option.value.employeeId;
    this.createEmpRequestFormGroup.controls['CreateEmployeeName'].setValue(event.option.viewValue);
    this.employeeRequestModel.CreateEmployeeCode = event.option.value.employeeCode;

  }
  selectOfferEmployee(event: MatAutocompleteSelectedEvent) {
    this.employeeRequestModel.OfferEmployeeId = event.option.value.employeeId;
    this.createEmpRequestFormGroup.controls['OfferEmployeeName'].setValue(event.option.viewValue);
    this.employeeRequestModel.OfferEmployeeCode = event.option.value.employeeCode;

    this.offerEmpName = this.offerEmpAccountList.find(a => a.employeeCode == this.employeeRequestModel.OfferEmployeeCode).employeeName;

    this.employeeService.getEmployeeToApprove(event.option.value.employeeId, "EMP_REQUEST").subscribe(response => {
      const result = <any>response;
      this.filteredOptionsApproveEmpAccount = new Observable<any>();
      this.filteredOptionsNotifyEmpAccount = new Observable<any>();
      this.approveEmpAccountList = [];
      this.approveEmpAccountList = result.listEmployeeToApprove
      if (result.listEmployeeToApprove.length > 0) {
        this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].setValue(result.listEmployeeToApprove[0].employeeCode + " - " + result.listEmployeeToApprove[0].employeeName)
        this.employeeRequestModel.ApproverId = result.listEmployeeToApprove[0].employeeId;
      }

      this.notifyEmpAccountList = result.listEmployeeToNotify;
      // Filter các autocomplete

      this.filteredOptionsNotifyEmpAccount = this.createEmpRequestFormGroup.controls['notifyListFormCtr'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice())
        );

      this.filteredOptionsApproveEmpAccount = this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterEmployeeAccount(name, this.approveEmpAccountList) : this.approveEmpAccountList.slice())
        );
    });
    if (this.employeeRequestModel.TypeRequest == this.absentwithpermisstion) this.changeTypeRequest(this.absentwithpermisstion);

  }

  // selectManagerEmployee(event: MatAutocompleteSelectedEvent) {   
  //   this.employeeRequestModel.ManagerId = event.option.value.employeeId;
  //   this.createEmpRequestFormGroup.controls['ManagerEmployeeName'].setValue(event.option.viewValue);
  // }

  selectApproverEmployee(event: MatAutocompleteSelectedEvent) {
    this.employeeRequestModel.ApproverId = event.option.value;
    this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].setValue(event.option.viewValue);
  }

  compareTwoDate() {
    if (this.employeeRequestModel.EnDate != null && this.employeeRequestModel.StartDate != null && this.employeeRequestModel.StartDate.setHours(0, 0, 0, 0) == this.employeeRequestModel.EnDate.setHours(0, 0, 0, 0) &&
      this.selectedStartShift == "CHI" && this.selectedEndShift == "SAN") {
      this.createEmpRequestFormGroup.controls['amShift'].setErrors({ "invalidShift": true });
      this.createEmpRequestFormGroup.controls['pmShift'].setErrors({ "invalidShift": true });
    }
    else {
      this.createEmpRequestFormGroup.controls['amShift'].setErrors(null);
      this.createEmpRequestFormGroup.controls['pmShift'].setErrors(null);
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

      this.createEmpRequestFormGroup.controls['amountAbsentFormCtr'].setValue(acountAbsent);
    }
  }

  // Filter Mat chip Auto Complete
  removeNotifier(item): void {
    const index = this.selectedNotifier.indexOf(item);

    if (index >= 0) {
      this.selectedNotifier.splice(index, 1);
      this.notifyEmpAccountList.push(item);
      this.filteredOptionsNotifyEmpAccount = this.createEmpRequestFormGroup.controls['notifyListFormCtr'].valueChanges.pipe(
        startWith(null),
        map((item: string) => item ? this._filterEmployeeAccount(item, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice()));
    }
  }

  addNotifier(event: MatChipInputEvent): void {
    this.createEmpRequestFormGroup.controls['notifyListFormCtr'].setValue(null);
  }

  @ViewChild('notifyInput', { static: true }) notifyInput: ElementRef;
  @ViewChild('notifyInput', { read: MatAutocompleteTrigger, static: true }) notifyInputTrigger: MatAutocompleteTrigger;

  selectedReasonFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedNotifier.push(event.option.value);
    this.notifyInput.nativeElement.value = '';
    this.createEmpRequestFormGroup.controls['notifyListFormCtr'].setValue(null);
    this.notifyEmpAccountList.splice(this.notifyEmpAccountList.indexOf(event.option.value), 1);
    this.filteredOptionsNotifyEmpAccount = this.createEmpRequestFormGroup.controls['notifyListFormCtr'].valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filterEmployeeAccount(item, this.notifyEmpAccountList) : this.notifyEmpAccountList.slice()));
    // Van hien thi suggestion panel sau khi select
    const self = this;
    setTimeout(function () {
      self.notifyInputTrigger.openPanel();
    }, 1);
  }
  // Ket thuc
  //ham lay ra người quản lý của nhân viên được đề xuất xin nghỉ
  getManager(orgId: string) {
    this.listApproveEmpAccount = [];
    this.listApproveEmpAccount = this.createEmpAccountList.filter(item => item.organizationId == orgId && item.isManager == true);
    this.employeeRequestModel.ApproverId = this.listApproveEmpAccount[0].employeeId;
    this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].setValue(this.listApproveEmpAccount[0]);
    if (this.listApproveEmpAccount.length == 0) {
      this.employeeRequestModel.ApproverId = null;
      this.createEmpRequestFormGroup.controls['ApproverEmployeeName'].setValue('');
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
  requestWithPermission = false;
  maternityAllowance = 0;
  amountAbsentWithPermission = 0;
  // Kieerm tra xem la nghi phep hay ko
  changeTypeRequest(value) {
    if (value == this.absentwithpermisstion) {
      this.requestWithPermission = true;
      this.lstEmpRqService.getEmployeeRequestByEmpId(this.employeeRequestModel.OfferEmployeeId, this.auth.UserId).subscribe(response => {
        let result = <any>response;
        this.amountAbsentWithPermission = result.amountAbsentWithPermission
      });
      this.empAlowanceService.getEmpAllowanceByEmpId(this.employeeRequestModel.OfferEmployeeId, this.auth.UserId).subscribe(response => {
        let result = <any>response;
        this.maternityAllowance = result.employeeAllowance.maternityAllowance == null ? 0 : result.employeeAllowance.maternityAllowance;
        if (this.maternityAllowance - this.amountAbsentWithPermission <= 0) {
          this.createEmpRequestFormGroup.controls['startDateFormCtr'].disable();
          this.createEmpRequestFormGroup.controls['amShift'].disable();
          this.createEmpRequestFormGroup.controls['endDateFormCtr'].disable();
          this.createEmpRequestFormGroup.controls['pmShift'].disable();
        } else {
          this.createEmpRequestFormGroup.controls['startDateFormCtr'].enable();
          this.createEmpRequestFormGroup.controls['amShift'].enable();
          this.createEmpRequestFormGroup.controls['endDateFormCtr'].enable();
          this.createEmpRequestFormGroup.controls['pmShift'].enable();
        }
      });

    } else {
      this.requestWithPermission = false;
      this.createEmpRequestFormGroup.controls['startDateFormCtr'].enable();
      this.createEmpRequestFormGroup.controls['amShift'].enable();
      this.createEmpRequestFormGroup.controls['endDateFormCtr'].enable();
      this.createEmpRequestFormGroup.controls['pmShift'].enable();
    }
  }
  //
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

  get f() { return this.createEmpRequestFormGroup.controls; }
}
