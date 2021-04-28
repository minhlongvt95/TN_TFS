import { Component, OnInit, ElementRef, HostListener, ViewChild, Renderer2, ChangeDetectorRef, AfterViewChecked } from '@angular/core';

import { ContactModel } from '../../../../shared/models/contact.model';
import { UserModel } from '../../../../shared/models/user.model';
import { FormGroup, FormControl, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { PositionService } from '../../../../shared/services/position.service';
import { EmployeeService } from '../../../services/employee.service';
import { EmailConfigService } from '../../../../admin/services/email-config.service';
import { ActivatedRoute, Router } from '@angular/router';

import * as $ from 'jquery';
import { EmployeeModel } from '../../../models/employee.model';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';
import { DialogService } from 'primeng/dynamicdialog';

import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html',
  styleUrls: ['./employee-create.component.css']
})
export class EmployeeCreateComponent implements OnInit, AfterViewChecked {
  /*Khai bao bien*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  password: string = this.systemParameterList.find(x => x.systemKey == "DefaultUserPassword").systemValueString;
  isManager: boolean = null;
  employeeId: string = null;
  currentOrganizationId: string = '';

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  genders = [{
    value: 'NAM', viewValue: 'Ông'
  },
  {
    value: 'NU', viewValue: 'Bà'
  }];

  roles: Array<string> = [];
  units: Array<string> = [];
  isAccessable: Boolean = true;
  toastMessage: string;
  empAccountList: Array<any> = [];
  empIdentityList: Array<any> = [];
  orgNameDisplay: string;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  employeeModel = new EmployeeModel()
  contactModel = new ContactModel()
  userModel: UserModel = {
    UserId: null, Password: this.password, UserName: '', EmployeeId: '', Disabled: false, CreatedById: 'DE2D55BF-E224-4ADA-95E8-7769ECC494EA', CreatedDate: null, UpdatedById: null, UpdatedDate: null, Active: true
  };

  createEmpForm: FormGroup;
  formGender: FormControl;
  formfirstName: FormControl;
  formlastName: FormControl;
  formEmail: FormControl;
  formPhone: FormControl;
  formIdentityId: FormControl;
  formPositionId: FormControl;
  formOrganizationId: FormControl;
  formUserName: FormControl;
  formIsAccess: FormControl;
  isInland: number = 1;
  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /*Ket thuc*/

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  fixed: boolean = false;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageXOffset;
    if (num > 100) {
      this.fixed = true;
    } else {
      this.fixed = false;
    }
  }

  /*Khai bao contructor*/
  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private organizationService: OrganizationService,
    private positionService: PositionService,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    public builder: FormBuilder,
    private emailConfigService: EmailConfigService,
    private renderer: Renderer2,
    private messageService: MessageService,
    public dialogService: DialogService,
    public changeRef: ChangeDetectorRef
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target) &&
          !this.saveAndCreate.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
    translate.setDefaultLang('vi');
  }
  /*Ket thuc*/

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  /*Function chay khi page load*/
  async ngOnInit() {
    let resource = "hrm/employee/create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      this.setForm();
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

      let result: any = await this.organizationService.getOrganizationByEmployeeId(this.employeeId);
      this.currentOrganizationId = result.organization.organizationId;

      this.contactModel.Gender = 'NAM';
      this.organizationService.getAllOrganization().subscribe(response => {
        let result = <any>response;
        this.units = result.organizationList;
      }, error => { });

      this.positionService.getAllPosition().subscribe(response => {
        let result = <any>response;
        this.roles = result.listPosition;
      }, error => { });

      await this.getAllEmpAccount();
      await this.getAllEmpIdentity();
    }
  }

  setForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    this.formfirstName = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.formlastName = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.formGender = new FormControl('NAM', [Validators.required]);
    this.formEmail = new FormControl('', [Validators.required, Validators.pattern(emailPattern)]);
    this.formPhone = new FormControl('', [Validators.pattern(this.getPhonePattern())]);
    this.formIdentityId = new FormControl('', [Validators.required, checkDuplicateIdentity(this.empIdentityList)]);
    this.formPositionId = new FormControl('');
    this.formUserName = new FormControl('', [Validators.required, checkDuplicateUsername(this.empAccountList), Validators.maxLength(250), forbiddenSpaceText]);
    this.formOrganizationId = new FormControl('', [Validators.required]);

    this.formIsAccess = new FormControl('true');
    this.createEmpForm = new FormGroup({
      formfirstName: this.formfirstName,
      formlastName: this.formlastName,
      formGender: this.formGender,
      formEmail: this.formEmail,
      formPhone: this.formPhone,
      formIdentityId: this.formIdentityId,
      formPositionId: this.formPositionId,
      formOrganizationId: this.formOrganizationId,
      formUserName: this.formUserName,
      formIsAccess: this.formIsAccess
    });
  }
  /*Ket thuc*/

  /*Quay tro lai Employee List*/
  goBack() {
    this.router.navigate(['/employee/list']);
  }
  /*Ket thuc*/

  /*Tao Employee moi*/
  @HostListener('submit', ['$event'])
  createEmployee(value: boolean) {
    if (!this.createEmpForm.valid) {
      Object.keys(this.createEmpForm.controls).forEach(key => {
        if (this.createEmpForm.controls[key].valid == false) {
          this.createEmpForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.createEmpForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      })
    } else {
      var auth = JSON.parse(localStorage.getItem("auth"));
      // Lấy giá trị cho employee model
      this.employeeModel.CreatedDate = new Date();
      this.employeeModel.CreatedById = auth.UserId;
      this.employeeModel.EmployeeFirstname = this.formfirstName.value.trim();
      this.employeeModel.EmployeeLastname = this.formlastName.value.trim();
      this.employeeModel.EmployeeCode = this.formIdentityId.value.trim();
      this.employeeModel.Username = this.formUserName.value.trim();
      if (this.formPositionId.value) {
        this.employeeModel.PositionId = this.formPositionId.value.positionId;
      }
      this.isAccessable = this.formIsAccess.value;

      // Lấy giá trị cho contact employee
      this.contactModel.CreatedDate = new Date();
      this.contactModel.CreatedById = auth.userId;
      this.contactModel.FirstName = this.formfirstName.value.trim();
      this.contactModel.LastName = this.formlastName.value.trim();
      this.contactModel.Email = this.formEmail.value.trim();
      this.contactModel.Phone = this.formPhone.value;

      this.userModel.UserName = this.formUserName.value.trim();

      this.loading = true;
      this.awaitResult = true;
      this.employeeService.createEmployee(this.employeeModel, this.contactModel, this.userModel, this.isAccessable).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode === 202 || result.statusCode === 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo nhân viên thành công' };
          this.showMessage(mgs);
          //send email
          let sendMailModel: any = result.sendEmailEntityModel;
          if (sendMailModel != null) {
            this.emailConfigService.sendEmail(7, sendMailModel).subscribe(reponse => {
            });
          }
          this.employeeService.sendEmail(this.contactModel.Email, this.contactModel.FirstName + " " + this.contactModel.LastName, this.userModel.UserName)
            .subscribe(response => { }, error => { });

          if (value) {
            this.resetFieldValue();
            this.awaitResult = false;
          } else {
            this.resetFieldValue();
            this.router.navigate(['/employee/detail', { employeeId: result.employeeId, contactId: result.contactId }]);
            this.awaitResult = false;
          }
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
          this.awaitResult = false;
        };
      },
        error => {
        });
    }
  }
  /*Ket thuc*/

  openOrgPopup() {
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: {
        chooseFinancialIndependence: false //Nếu chỉ chọn đơn vị độc lập tài chính
      },
      header: 'Chọn đơn vị',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.employeeModel.OrganizationId = result.selectedOrgId;
          this.createEmpForm.controls['formOrganizationId'].setValue(result.selectedOrgName);
        }
      }
    });
  }

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.createEmpForm.reset();
    this.employeeModel.PositionId = '';
    this.employeeModel.OrganizationId = '';
    this.contactModel.Gender = 'NAM';
    this.isAccessable = true;
    this.createEmpForm.controls['formPositionId'].setValue('');
    this.createEmpForm.controls['formOrganizationId'].setValue('');
    this.createEmpForm.controls['formGender'].setValue('NAM');
    this.createEmpForm.controls['formIsAccess'].setValue(true);
  }
  //Kết thúc

  /*Function lay ra toan bo Employee Account de check Account trung lap*/
  async getAllEmpAccount() {
    var result: any = await this.employeeService.getAllEmpAccount();
    this.empAccountList = result.empAccountList;
  }
  /*Ket thuc*/

  /*Function lay ra toan bo Employee Account de check Account trung lap*/
  async getAllEmpIdentity() {
    var result: any = await this.employeeService.getAllEmpIdentity(this.emptyGuid);
    this.empIdentityList = result.empIdentityList;
  }
  /*Ket thuc*/
  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  cancel() {
    this.router.navigate(['/employee/dashboard']);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  ngAfterViewChecked() {
    this.changeRef.detectChanges();
  }
}

function checkDuplicateUsername(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value != undefined) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'checkDuplicateUsername': true };
      }
      return null;
    }
  }
}

function checkDuplicateIdentity(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value != undefined) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'checkDuplicateIdentity': true };
      }
      return null;
    }
  }
}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}
