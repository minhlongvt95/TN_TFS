import { Component, OnInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatRadioChange } from '@angular/material/radio';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';

import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { OrgSelectDialogComponent } from "../../org-select-dialog/org-select-dialog.component";
import { TranslateService } from '@ngx-translate/core';
import { EmployeeService } from "../../../services/employee.service";
import { OrganizationService } from "../../../../shared/services/organization.service";
import { PositionService } from "../../../../shared/services/position.service";
import { CategoryService } from "../../../../shared/services/category.service";
import { ImageUploadService } from "../../../../shared/services/imageupload.service";
import { EmployeeModel } from "../../../models/employee.model";
import { ContactModel } from "../../../../shared/models/contact.model";
import { BankService } from "../../../../shared/services/bank.service";
import { UserModel } from '../../../../shared/models/user.model';
import * as $ from 'jquery';
import { PermissionService } from '../../../../shared/services/permission.service';
import { BankModel } from '../../../../shared/models/bank.model';
import { EmployeeSalaryService } from '../../../services/employee-salary/employee-salary.service';
import { EmployeeSalaryModel } from '../../../models/employee-salary.model';
import { EmployeeCreateSalaryPopupComponent } from '../employee-create-salary-popup/employee-create-salary-popup.component';
import { EmployeeAllowanceService } from '../../../services/employee-allowance/employee-allowance.service';
import { EmployeeAllowanceModel } from '../../../models/employee-allowance.model';
import { EmployeeInsuranceService } from '../../../services/employee-insurance/employee-insurance.service';
import { EmployeeInsuranceModel } from '../../../models/employee-insurance.model';
import { EmployeeMonthySalaryService } from '../../../services/employee-salary/employee-monthy-salary.service';
import { EmployeeMonthySalaryModel } from '../../../models/employee-monthy-salary.model';
import { EmployeeAssessmentService } from '../../../services/employee-assessment/employee-assessment.service';
import { Subscription, Observable } from 'rxjs';
import { NoteModel } from '../../../../shared/models/note.model';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { NoteService } from '../../../../shared/services/note.service';
import { CustomerService } from '../../../../customer/services/customer.service';
import { AppDateAdapter, APP_DATE_FORMATS } from '../../../../shared/services/date.adapter.service';

import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { ContactService } from '../../../../shared/services/contact.service';
import { startWith, map } from 'rxjs/operators';
import { ListEmployeeRequestService } from '../../../services/employee-request/list-employee-request.service';
import { EmployeeListService } from '../../../services/employee-list/employee-list.service';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { WarningComponent } from '../../../../shared/toast/warning/warning.component';
import { ConfirmationService } from 'primeng/api';

class Assessment {
  month: Number;
  evaluate: string;
  eveluateId: string;
  monthText: string;
}

@Component({
  selector: 'app-employee-detail',
  templateUrl: './employee-detail.component.html',
  styleUrls: ['./employee-detail.component.css'],
  providers: [
    {
      provide: DateAdapter, useClass: AppDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS
    }
  ]
})
export class EmployeeDetailComponent implements OnInit {
  /*Khai báo biến*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  password: string = this.systemParameterList.find(x => x.systemKey == "DefaultUserPassword").systemValueString;

  isManager: boolean = null;
  currEmployeeId: string = '';
  currentOrganizationId: string = '';

  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  paramToSearchYear: Number;
  paramToSearchMonth: Number;
  nowYear = new Date().getFullYear();
  hasInsurance = false;
  isEditDatapermission = false;
  isEditFunctionpermission = false;
  displaySalaryCollumn = ['basedSalary', 'appliedDate'];
  displayAllowanceCollumn = ['lunchAllowance', 'maternityAllowance', 'fuelAllowance', 'phoneAllowance', 'otherAllownce'];
  displayAssessmentCollumn = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'];
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  // Declare Ghi chu nhan vien
  accept = 'image/*, video/*, audio/*, .zip, .rar, .pdf, .xls, .xlsx, .doc, .docx, .ppt, .pptx, .txt';
  files: File[] = [];
  progress: number;
  hasBaseDropZoneOver = false;
  lastFileAt: Date;
  httpEmitter: Subscription;
  sendableFormData: any;
  maxSize: number = 11000000;
  lastInvalids: any;
  baseDropValid: any;
  dragFiles: any;
  noteContent: string = '';
  noteId: string;
  searchNoteKeyword: string;
  searchNoteFromDate: Date;
  searchNoteToDate: Date;
  noteHistory: Array<string> = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '../../../../assets/images/no-avatar.png';
  activeUser: boolean;

  //Config editor
  editorConfig: any = {
    'editable': true,
    'height': '220px',
    'minHeight': '220px',
    'width': 'auto',
    'minWidth': '0',
    'translate': 'yes',
    'enableToolbar': true,
    'showToolbar': true,
    'placeholder': 'Nhập ghi chú...',
    'toolbar': [
      ['bold', 'italic', 'underline'],
      ['fontName', 'fontSize', 'color'],
      ['justifyLeft', 'justifyCenter', 'justifyRight'],
      ['link', 'unlink'],
    ]
  };
  noteModel = new NoteModel();

  // Khai báo Loading
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


  // Số buổi nghỉ phép || Không phép
  amountAbsentWithPermission: number = 0;
  amountAbsentWithoutPermission: number = 0;
  // Kiểm tra xem có tài khoản ngân hàng hay chưa
  hasBankAccount = true;
  // Ngày áp dụng
  effecticeDateToSearch = new Date();
  employeeSalaryBase: Number;
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  employeeId: string;
  contactId: string;
  valueOfNationality: string = '';
  hasNationality = true;
  nationality = '';

  // Declare Model
  employeeModel = new EmployeeModel();
  contactModel = new ContactModel();
  userModel = new UserModel();
  employeeOldModel = new EmployeeModel();
  contactOldModel = new ContactModel();
  bankOldModel = new BankModel();
  userOldModel = new UserModel();
  bankModel = new BankModel();
  employeeSalaryModel = new EmployeeSalaryModel();
  listEmployeeSalaryModel = new Array<EmployeeSalaryModel>();
  employeeAllowance = new EmployeeAllowanceModel();
  listEmployeeAllowanceModel = new Array<EmployeeAllowanceModel>();
  employeeInsuranceModel = new EmployeeInsuranceModel();
  employeeMonthySalaryModel = new EmployeeMonthySalaryModel();
  permissionList: Array<any>;
  moduleNameList: Array<any>;
  evaluateList: Array<any>;
  contractTypeList: Array<any>;
  listEmployeeAssessmentModel: Array<any> = [];
  listYearToAssessment: Array<any> = [];
  listCountry: Array<any> = []; filteredOptionsCountry: Observable<any>;
  listEmployeeRequest: Array<any>;
  dialogConfirm: MatDialogRef<PopupComponent>;

  trangthais = [
    {
      id: 1, text: 'Đang hoạt động - Được phép truy cập'
    },
    {
      id: 2, text: 'Đang hoạt động - Không được phép truy cập'
    },
    {
      id: 3, text: 'Ngừng hoạt động'
    }];

  // Declare FormGroup
  personalEmpInfoFormGroup: FormGroup;
  email: FormControl;
  identityEmpInfoFormGroup: FormGroup;
  workingEmpInfoFormGroup: FormGroup;
  assessmentEmpInforFormGroup: FormGroup;
  salaryAndAllowanceEmpInfoFormGroup: FormGroup;

  // Declare edit mode || view mode
  // Edit thông tin nhân viên
  editPersonalEmpInfo = false;
  // Edit thông tin làm việc
  editWorkingEmpInfo = false;
  // Edit thông tin lương/phụ cấp
  editSalaryEmpInfo = false;
  // Edit thoong tin danh gia
  editAssessmentEmpInfo = false;
  // Edit thong tin nhan su
  editIdentityEmpInfo = false;

  // Declare popup
  dialogOrg: MatDialogRef<OrgSelectDialogComponent>;
  dialogCreateEmpSalary: MatDialogRef<EmployeeCreateSalaryPopupComponent>;
  dialogPopup: MatDialogRef<PopupComponent>;

  // Declare MasterData
  listPosition = [];
  listPaymentMethod = [];

  // Declare Data Table
  dataTableSalary = new MatTableDataSource();
  dataTableAllowance = new MatTableDataSource();

  /*List role*/
  listRole: Array<any> = [];
  roleId: string = null;
  /*End*/

  actionEdit: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  isAllowReset: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(private imageUploadService: ImageUploadService,
    private confirmationService: ConfirmationService,
    private permissisonService: PermissionService,
    private getPermission: GetPermission,
    private customerService: CustomerService,
    private translate: TranslateService,
    private employeeRequestService: ListEmployeeRequestService,
    private contactService: ContactService,
    private bankService: BankService,
    private employeeSalaryService: EmployeeSalaryService,
    private employeeService: EmployeeService,
    private employeeAllowanceService: EmployeeAllowanceService,
    private employeeInsuranceService: EmployeeInsuranceService,
    private employeeMonthySalaryService: EmployeeMonthySalaryService,
    private employeeAssessmentService: EmployeeAssessmentService,
    private organizationService: OrganizationService,
    private categoryService: CategoryService,
    private positionService: PositionService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    public snackBar: MatSnackBar,
    private el: ElementRef,
    private employeeListService: EmployeeListService,
    public dialog: MatDialog) {
    translate.setDefaultLang('vi');
  }

  async ngOnInit() {
    let resource = "hrm/employee/detail/";
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
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("re-pass") == -1) {
        this.isAllowReset = false;
      }

      this.email = new FormControl('', [Validators.email]);

      this.personalEmpInfoFormGroup = this.formBuilder.group({
        firstName: ['', [Validators.required]],
        code: ['', [Validators.required]],
        lastName: ['', [Validators.required]],
        gender: ['', [Validators.required]],
        birthDay: ['', []],
        phone: ['', []],
        email: this.email,
        address: ['']
      });
      this.personalEmpInfoFormGroup.disable();
      this.identityEmpInfoFormGroup = this.formBuilder.group({
        nationality: [''],
        identityId: [''],
        identityIddateOfIssue: [''],
        identityIddateOfParticipation: [''],
        identityIdplaceOfIssue: [''],
        workPermitNumber: [''],
        visaNumber: [''],
        visaDateOfIssue: [''],
        visaExpirationDate: [''],
        socialInsuranceNumber: [''],
        socialInsuranceDateOfIssue: [''],
        socialInsuranceDateOfParticipation: [''],
        healthInsuranceNumber: [''],
        healthInsuranceDateOfIssue: [''],
        healthInsuranceDateOfParticipation: [''],
        accountNumber: [''],
        bankName: [''],
        branchName: [''],
        paymentMethod: ['']
      })
      this.identityEmpInfoFormGroup.disable();
      this.workingEmpInfoFormGroup = this.formBuilder.group({
        position: ['', [Validators.required]],
        organization: ['', [Validators.required]],
        workstarthour: [''],
        workendhour: [''],
        trainingStartDate: [''],
        probationStartDate: [''],
        probationEndDate: [''],
        startedDate: [''],
        username: [''],
        formStatus: [''],
        contractEndDate: [''],
        contract: [''],
        isTakeCareControl: [false]
      });
      this.workingEmpInfoFormGroup.disable();
      this.assessmentEmpInforFormGroup = this.formBuilder.group({
        yearToAssessment: [''],
        assessment1: [''],
        assessment2: [''],
        assessment3: [''],
        assessment4: [''],
        assessment5: [''],
        assessment6: [''],
        assessment7: [''],
        assessment8: [''],
        assessment9: [''],
        assessment10: [''],
        assessment11: [''],
        assessment12: [''],
      })
      this.salaryAndAllowanceEmpInfoFormGroup = this.formBuilder.group({
        basedSalary: ['', [Validators.required]],
        effecticeDateToSearch: ['', [Validators.required]],
        LunchAllowance: [''],
        MaternityAllowance: [''],
        FuelAllowance: [''],
        PhoneAllowance: [''],
        OtherAllownce: [''],
        SocialInsuranceSalary: [''],
        SocialInsuranceSupportPercent: [''],
        UnemploymentinsuranceSupportPercent: [''],
        HealthInsuranceSupportPercent: [''],
        UnemploymentinsurancePercent: [''],
        SocialInsurancePercent: [''],
        HealthInsurancePercent: [''],
      })
      this.salaryAndAllowanceEmpInfoFormGroup.disable();

      this.loading = true;
      this.route.params.subscribe(params => { this.employeeId = params['employeeId']; this.contactId = params['contactId']; });
      await this.getAllEmpIdentity();
      await this.getAllEmpAccIdentity();
      this.personalEmpInfoFormGroup.controls['code'].setValidators(checkDuplicateIdentity(this.empIdentityList));
      this.personalEmpInfoFormGroup.controls['phone'].setValidators([Validators.pattern(this.getPhonePattern())]);
      this.workingEmpInfoFormGroup.controls['username'].setValidators([Validators.required, checkDuplicateIdentity(this.empAccIdentityList)]);
      await this.getMasterData();
      this.loading = false;

      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.currEmployeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

      let result: any = await this.organizationService.getOrganizationByEmployeeId(this.currEmployeeId);
      this.currentOrganizationId = result.organization.organizationId;
    }
  }

  /*Lay ra toan bo Master Data*/
  getMasterData() {
    // Get country
    this.contactService.getAllCountry(this.userId).subscribe(response => {
      let result = <any>response;
      this.listCountry = result.listCountry;
      // let vn = this.listCountry.find(ct => ct.countryCode.trim() === "VN");
      // if (vn !== null && vn != undefined) {
      //   this.listCountry.splice(this.listCountry.indexOf(vn), 1);
      // }
      // this.listCountry.sort((a: any, b: any) => {
      //   let x = a.countryName.toLowerCase().trim();
      //   let y = b.countryName.toLowerCase().trim();
      //   return (x.localeCompare(y) === -1 ? 1 : -1)
      // })
      // this.listCountry.push(vn);
      // this.listCountry.sort((a: any, b: any) => {
      //   return (-1);
      // })
      
      this.filteredOptionsCountry = this.identityEmpInfoFormGroup.controls['nationality'].valueChanges
        .pipe(
          startWith(''),
          map(name => name ? this._filterCountry(name, this.listCountry) : this.listCountry.slice())
        );
    })

    // Get chức vụ
    this.positionService.getAllPosition().subscribe(response => {
      let result = <any>response;
      this.listPosition = result.listPosition;
    },
      error => { });

    // Get hình thức chi
    this.categoryService.getAllCategoryByCategoryTypeCode("PM").subscribe(response => {
      let result = <any>response;
      this.listPaymentMethod = result.categoryPMList;// Get hình thức chi
      this.evaluateList = result.categoryDVIList;// GetLoại đánh giá
      this.contractTypeList = result.categoryLabourContractList;// Get Loại hợp đồng
    })

    // Get quyền chức năng
    this.permissisonService.getAllPermission().subscribe(response => {
      let result = <any>response;
      this.permissionList = result.permissionList;
    }, error => { });

    this.permissisonService.getAllRole().subscribe(response => {
      let result: any = response;
      this.listRole = result.listRole;
    });

    // GetLoại đánh giá
    //this.categoryService.getAllCategoryByCategoryTypeCode("DVI").subscribe(response => {
    //  let result = <any>response;
    //  this.evaluateList = result.category;
    //})
    // Get các năm co thể tìm để tìm đánh giá
    this.employeeAssessmentService.getAllYearToAssessment(this.employeeId, this.userId).subscribe(response => {
      let result = <any>response;
      this.listYearToAssessment = result.listYear;
    })
    // Get Loại hợp đồng
    //this.categoryService.getAllCategoryByCategoryTypeCode("LabourContract").subscribe(response => {
    //  let result = <any>response;
    //  this.contractTypeList = result.category;
    //})
    this.getNoteHistory();
    this.getEmployeeInfo();
  }

  empIdentityList: Array<any>;
  /*Function lay ra toan bo Employee Account de check Account trung lap*/
  async getAllEmpIdentity() {
    var result: any = await this.employeeService.getAllEmpIdentity(this.employeeId);
    this.empIdentityList = result.empIdentityList;
  }
  /*Ket thuc*/

  empAccIdentityList: Array<any>;
  async getAllEmpAccIdentity() {
    let result: any = await this.employeeService.getAllEmpAccIdentity(this.employeeId);
    this.empAccIdentityList = result.listAccEmployee;
  }

  private _filterCountry(value: string, array: any) {
    // let val = array.find(el => el.countryName.toLowerCase() === value.toString().toLowerCase());
    // if (val != null && val != undefined) {
    //   this.contactModel.CountryId = val.countryId;
    // } else {
    //   this.contactModel.CountryId = null
    // }
    // return array.filter(state =>
    //   state.countryName.toLowerCase().includes(value.toString().toLowerCase()));
    
    let filterValue = value.toLowerCase();
    return array.filter(item => item.countryName.toLowerCase().indexOf(filterValue) >= 0);
  }
  // Auto complete select employee
  selectCountry(event: MatAutocompleteSelectedEvent) {
    this.identityEmpInfoFormGroup.controls['nationality'].setValue(event.option.viewValue);
    this.contactModel.CountryId = event.option.value;
    this.hasNationality = true;
  }
  /*Ket thuc*/

  // Laasy thong tin tinh hinh lam viec
  async searchEmployeeRequest() {
    var result: any = await this.employeeRequestService.searchEmployeeRequest
      (
        "",
        null,
        this.employeeId,
        null,
        null,
        null,
        null,
        null,
        this.auth.UserId
      );

    this.listEmployeeRequest = result.employeeRequestList;
    //this.amountAbsentWithoutPermission = result.amountAbsentWithoutPermission;
    //this.amountAbsentWithPermission = result.amountAbsentWithPermission;

    await this.employeeRequestService.getEmployeeRequestByEmpId(this.employeeId, this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.amountAbsentWithoutPermission = result.amountAbsentWithoutPermission;
      this.amountAbsentWithPermission = result.amountAbsentWithPermission;
    });
  }
  /*Lay thong tin cua Employee*/
  empStatusDisplay = '';
  empStatus: any;
  empStatusOldDisplay = '';
  empStatusOld: any;
  getEmployeeInfo() {
    this.searchEmployeeRequest();
    this.employeeService.getEmployeeById(this.employeeId, this.contactId).subscribe(response => {
      let result = <any>response;
      console.log(result.employee.isTakeCare)
      this.activeUser = result.employee.activeUser;
      this.roleId = result.employee.roleId;
      this.employeeModel = <EmployeeModel>({
        EmployeeCode: result.employee.employeeCode,
        EmployeeId: result.employee.employeeId,
        EmployeeName: result.employee.employeeName,
        StartedDate: result.employee.startedDate == null ? null : new Date(result.employee.startedDate.toString()),
        OrganizationId: result.employee.organizationId,
        PositionId: result.employee.positionId,
        CreatedById: result.employee.createdById,
        CreatedDate: result.employee.createdDate,
        UpdatedById: result.employee.updatedById,
        UpdatedDate: result.employee.updatedDate,
        Active: result.employee.active,
        Username: result.employee.username,
        Identity: result.employee.identity,
        OrganizationName: result.employee.organizationName,
        PositionName: result.employee.positionName,
        IsManager: result.employee.isManager,
        PermissionSetId: result.employee.permissionSetId,
        AvatarUrl: result.employee.avatarUrl,
        ProbationEndDate: result.employee.probationEndDate == null ? null : new Date(result.employee.probationEndDate.toString()),
        ProbationStartDate: result.employee.probationStartDate == null ? null : new Date(result.employee.probationStartDate.toString()),
        TrainingStartDate: result.employee.trainingStartDate == null ? null : new Date(result.employee.trainingStartDate.toString()),
        ContractEndDate: result.employee.contractEndDate == null ? null : new Date(result.employee.contractEndDate.toString()),
        ContractType: result.employee.contractType,
        IsTakeCare: result.employee.isTakeCare,
      });
      this.employeeOldModel = <EmployeeModel>({
        EmployeeCode: result.employee.employeeCode,
        EmployeeId: result.employee.employeeId,
        EmployeeName: result.employee.employeeName,
        StartedDate: result.employee.startedDate == null ? null : new Date(result.employee.startedDate.toString()),
        OrganizationId: result.employee.organizationId,
        PositionId: result.employee.positionId,
        CreatedById: result.employee.createdById,
        CreatedDate: result.employee.createdDate,
        UpdatedById: result.employee.updatedById,
        UpdatedDate: result.employee.updatedDate,
        Active: result.employee.active,
        Username: result.employee.username,
        Identity: result.employee.identity,
        OrganizationName: result.employee.organizationName,
        PositionName: result.employee.positionName,
        IsManager: result.employee.isManager,
        PermissionSetId: result.employee.permissionSetId,
        AvatarUrl: result.employee.avatarUrl,
        ProbationEndDate: result.employee.probationEndDate == null ? null : new Date(result.employee.probationEndDate.toString()),
        ProbationStartDate: result.employee.probationStartDate == null ? null : new Date(result.employee.probationStartDate.toString()),
        TrainingStartDate: result.employee.trainingStartDate == null ? null : new Date(result.employee.trainingStartDate.toString()),
        ContractEndDate: result.employee.contractEndDate == null ? null : new Date(result.employee.contractEndDate.toString()),
        ContractType: result.employee.contractType,
        IsTakeCare: result.employee.isTakeCare,
      });

      this.contactModel = <ContactModel>({
        ContactId: result.contact.contactId,
        ObjectId: result.contact.objectId,
        CountryId: result.contact.countryId,
        ObjectType: result.contact.objectType,
        FirstName: result.contact.firstName,
        LastName: result.contact.lastName,
        Gender: result.contact.gender,
        DateOfBirth: result.contact.dateOfBirth == null ? null : new Date(result.contact.dateOfBirth.toString()),
        Phone: result.contact.phone,
        Email: result.contact.email,
        IdentityID: result.contact.identityId,
        AvatarUrl: result.contact.avatarUrl,
        Address: result.contact.address,
        ProvinceId: result.contact.provinceId,
        DistrictId: result.contact.districtId,
        WardId: result.contact.wardId,
        PostCode: result.contact.postCode,
        WebsiteUrl: result.contact.websiteUrl,
        SocialUrl: result.contact.socialUrl,
        CreatedById: result.contact.createdById,
        CreatedDate: result.contact.createdDate,
        UpdatedById: result.contact.updatedById,
        UpdatedDate: result.contact.updatedDate,
        Active: result.contact.active,
        Note: result.contact.note,
        TaxCode: result.contact.taxCode,
        Job: result.contact.job,
        Agency: result.contact.agency,
        Birthplace: result.contact.birthplace,
        HealthInsuranceDateOfIssue: result.contact.healthInsuranceDateOfIssue,
        HealthInsuranceDateOfParticipation: result.contact.healthInsuranceDateOfParticipation,
        HealthInsuranceNumber: result.contact.healthInsuranceNumber,
        IdentityIDDateOfIssue: result.contact.identityIddateOfIssue,
        IdentityIDDateOfParticipation: result.contact.identityIddateOfParticipation,
        IdentityIDPlaceOfIssue: result.contact.identityIdplaceOfIssue,
        WorkPermitNumber: result.contact.workPermitNumber,
        SocialInsuranceDateOfIssue: result.contact.socialInsuranceDateOfIssue,
        SocialInsuranceDateOfParticipation: result.contact.socialInsuranceDateOfParticipation,
        SocialInsuranceNumber: result.contact.socialInsuranceNumber,
        VisaDateOfIssue: result.contact.visaDateOfIssue,
        VisaExpirationDate: result.contact.visaExpirationDate,
        VisaNumber: result.contact.visaNumber,
        WorkHourOfEnd: result.contact.workHourOfEnd,
        WorkHourOfStart: result.contact.workHourOfStart,
        TypePaid: result.contact.typePaid
      });
      this.contactOldModel = <ContactModel>({
        ContactId: result.contact.contactId,
        ObjectId: result.contact.objectId,
        CountryId: result.contact.countryId,
        ObjectType: result.contact.objectType,
        FirstName: result.contact.firstName,
        LastName: result.contact.lastName,
        Gender: result.contact.gender,
        DateOfBirth: result.contact.dateOfBirth == null ? null : new Date(result.contact.dateOfBirth.toString()),
        Phone: result.contact.phone,
        Email: result.contact.email,
        IdentityID: result.contact.identityId,
        AvatarUrl: result.contact.avatarUrl,
        Address: result.contact.address,
        ProvinceId: result.contact.provinceId,
        DistrictId: result.contact.districtId,
        WardId: result.contact.wardId,
        PostCode: result.contact.postCode,
        WebsiteUrl: result.contact.websiteUrl,
        SocialUrl: result.contact.socialUrl,
        CreatedById: result.contact.createdById,
        CreatedDate: result.contact.createdDate,
        UpdatedById: result.contact.updatedById,
        UpdatedDate: result.contact.updatedDate,
        Active: result.contact.active,
        Note: result.contact.note,
        TaxCode: result.contact.taxCode,
        Job: result.contact.job,
        Agency: result.contact.agency,
        Birthplace: result.contact.birthplace,
        HealthInsuranceDateOfIssue: result.contact.healthInsuranceDateOfIssue,
        HealthInsuranceDateOfParticipation: result.contact.healthInsuranceDateOfParticipation,
        HealthInsuranceNumber: result.contact.healthInsuranceNumber,
        IdentityIDDateOfIssue: result.contact.identityIddateOfIssue,
        IdentityIDDateOfParticipation: result.contact.identityIddateOfParticipation,
        IdentityIDPlaceOfIssue: result.contact.identityIdplaceOfIssue,
        WorkPermitNumber: result.contact.workPermitNumber,
        SocialInsuranceDateOfIssue: result.contact.socialInsuranceDateOfIssue,
        SocialInsuranceDateOfParticipation: result.contact.socialInsuranceDateOfParticipation,
        SocialInsuranceNumber: result.contact.socialInsuranceNumber,
        VisaDateOfIssue: result.contact.visaDateOfIssue,
        VisaExpirationDate: result.contact.visaExpirationDate,
        VisaNumber: result.contact.visaNumber,
        WorkHourOfEnd: result.contact.workHourOfEnd,
        WorkHourOfStart: result.contact.workHourOfStart,
        TypePaid: result.contact.typePaid
      });

      let national = this.listCountry.find(lc => lc.countryId === this.contactModel.CountryId);
      if (national !== null && national !== undefined) {
        this.hasNationality = true;
        this.identityEmpInfoFormGroup.controls["nationality"].setValue(national.countryName);
        this.nationality = national.countryName;
      } else {
        this.hasNationality = false;
      }
      this.userModel = <UserModel>({
        UserId: result.user.userId,
        Active: result.user.active,
        EmployeeId: result.user.employeeId,
        UserName: result.user.userName,
        Password: result.user.password,
        CreatedDate: result.user.createdDate,
        CreatedById: result.user.createdById
      });
      this.userOldModel = <UserModel>({
        UserId: result.user.userId,
        Active: result.user.active,
        EmployeeId: result.user.employeeId,
        UserName: result.user.userName,
        Password: result.user.password,
        CreatedDate: result.user.createdDate,
        CreatedById: result.user.createdById
      });

      if (this.employeeModel.Active && this.userModel.Active) {
        this.empStatusDisplay = this.trangthais[0].text;
        this.empStatus = this.trangthais[0].id;
      } else if (this.employeeModel.Active && !this.userModel.Active) {
        this.empStatusDisplay = this.trangthais[1].text;
        this.empStatus = this.trangthais[1].id;
      } else {
        this.empStatusDisplay = this.trangthais[2].text;
        this.empStatus = this.trangthais[2].id;
      }

      // this.nationality = national.countryName;
      this.getModuleByPermissionSetId(this.employeeModel.PermissionSetId);

    }, error => { });

    // Get tài khoản ngân hàng của nhân viên
    this.bankService.getAllBankAccountByObject(this.employeeId, 'EMP').subscribe(response => {
      let result = <any>response;
      if (result.bankAccountList.length > 0) {
        this.hasBankAccount = true;
        this.bankModel = <BankModel>({
          ObjectType: result.bankAccountList[0].objectType,
          CreatedDate: result.bankAccountList[0].createdDate,
          CreatedById: result.bankAccountList[0].createdById,
          Active: result.bankAccountList[0].active,
          ObjectId: result.bankAccountList[0].objectId,
          BankDetail: result.bankAccountList[0].bankDetail,
          BankAccountId: result.bankAccountList[0].bankAccountId,
          AccountName: result.bankAccountList[0].accountName,
          AccountNumber: result.bankAccountList[0].accountNumber,
          BankName: result.bankAccountList[0].bankName,
          BranchName: result.bankAccountList[0].branchName
        });
        this.bankOldModel = <BankModel>({
          ObjectType: result.bankAccountList[0].objectType,
          CreatedDate: result.bankAccountList[0].createdDate,
          CreatedById: result.bankAccountList[0].createdById,
          Active: result.bankAccountList[0].active,
          ObjectId: result.bankAccountList[0].objectId,
          BankDetail: result.bankAccountList[0].bankDetail,
          BankAccountId: result.bankAccountList[0].bankAccountId,
          AccountName: result.bankAccountList[0].accountName,
          AccountNumber: result.bankAccountList[0].accountNumber,
          BankName: result.bankAccountList[0].bankName,
          BranchName: result.bankAccountList[0].branchName
        });
      } else {
        this.hasBankAccount = false;
      }
    })

    // Get thông tin lương nhân viên
    this.listEmployeeSalaryModel = [];
    this.employeeSalaryService.getEmpSalaryByEmpId(this.employeeId, this.effecticeDateToSearch, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.listEmployeeSalary.length > 0) {
        this.employeeSalaryBase = result.listEmployeeSalary[0].employeeSalaryBase;
        this.effecticeDateToSearch = result.listEmployeeSalary[0].effectiveDate;
      }
      if (result.listEmployeeSalary === null || result.listEmployeeSalary.length === 0) {
        this.employeeSalaryModel.EmployeeSalaryBase = 0;
        this.listEmployeeSalaryModel.push(this.employeeSalaryModel);
      } else {
        result.listEmployeeSalary.forEach(emps => {
          this.employeeSalaryModel = <EmployeeSalaryModel>({
            EmployeeSalaryId: emps.employeeSalaryId,
            EmployeeSalaryBase: emps.employeeSalaryBase,
            EffectiveDate: emps.effectiveDate,
            EmployeeId: this.employeeId,
            ResponsibilitySalary: emps.responsibilitySalary
          })
          this.listEmployeeSalaryModel.push(this.employeeSalaryModel);
        })
        this.dataTableSalary = new MatTableDataSource<EmployeeSalaryModel>(this.listEmployeeSalaryModel);
        this.paginationFunction();
        this.dataTableSalary.paginator = this.paginator;
      }
    })

    // Get thông tin phụ cấp nhân viên
    this.employeeAllowanceService.getEmpAllowanceByEmpId(this.employeeId, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.employeeAllowance != null) {
        this.employeeAllowance = <EmployeeAllowanceModel>({
          MaternityAllowance: result.employeeAllowance.maternityAllowance == null ? 0 : result.employeeAllowance.maternityAllowance,
          LunchAllowance: result.employeeAllowance.lunchAllowance,
          FuelAllowance: result.employeeAllowance.fuelAllowance,
          PhoneAllowance: result.employeeAllowance.phoneAllowance,
          OtherAllownce: result.employeeAllowance.otherAllownce
        })
      } else this.employeeAllowance.MaternityAllowance = 0;
      this.listEmployeeAllowanceModel = [];
      this.listEmployeeAllowanceModel.push(this.employeeAllowance);
      this.dataTableAllowance = new MatTableDataSource<EmployeeAllowanceModel>(this.listEmployeeAllowanceModel);
    })

    // Get thông tin Bảo hiểm
    this.employeeInsuranceService.searchEmployeeInsurance(this.employeeId, null, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.listEmployeeInsurance.length !== 0) {
        this.hasInsurance = true;
        this.employeeInsuranceModel = <EmployeeInsuranceModel>({
          EffectiveDate: result.listEmployeeInsurance[0].effectiveDate,
          HealthInsurancePercent: result.listEmployeeInsurance[0].healthInsurancePercent === null ? 1 : result.listEmployeeInsurance[0].healthInsurancePercent,
          HealthInsuranceSupportPercent: result.listEmployeeInsurance[0].healthInsuranceSupportPercent === null ? 3 : result.listEmployeeInsurance[0].healthInsuranceSupportPercent,
          SocialInsurancePercent: result.listEmployeeInsurance[0].socialInsurancePercent === null ? 1 : result.listEmployeeInsurance[0].socialInsurancePercent,
          SocialInsuranceSalary: result.listEmployeeInsurance[0].socialInsuranceSalary === null ? 0 : result.listEmployeeInsurance[0].socialInsuranceSalary,
          SocialInsuranceSupportPercent: result.listEmployeeInsurance[0].socialInsuranceSupportPercent === null ? 17.5 : result.listEmployeeInsurance[0].socialInsuranceSupportPercent,
          UnemploymentinsurancePercent: result.listEmployeeInsurance[0].unemploymentinsurancePercent === null ? 1 : result.listEmployeeInsurance[0].unemploymentinsurancePercent,
          UnemploymentinsuranceSupportPercent: result.listEmployeeInsurance[0].unemploymentinsuranceSupportPercent === null ? 8.5 : result.listEmployeeInsurance[0].unemploymentinsuranceSupportPercent
        })
      } else {
        this.employeeInsuranceModel.HealthInsurancePercent = 1;
        this.employeeInsuranceModel.HealthInsuranceSupportPercent = 3;
        this.employeeInsuranceModel.SocialInsurancePercent = 1;
        this.employeeInsuranceModel.SocialInsuranceSalary = 0;
        this.employeeInsuranceModel.SocialInsuranceSupportPercent = 17.5;
        this.employeeInsuranceModel.UnemploymentinsurancePercent = 1;
        this.employeeInsuranceModel.UnemploymentinsuranceSupportPercent = 8.5;
      }
    })

    // Get thông tin tình hình công việc
    this.paramToSearchMonth = new Date().getMonth() + 1;
    this.paramToSearchYear = new Date().getFullYear();
    this.employeeMonthySalaryService.getEmpMonthySalaryByEmpId(this.employeeId, this.paramToSearchYear, this.paramToSearchMonth).subscribe(response => {
      let result = <any>response;
      this.employeeMonthySalaryModel = <EmployeeMonthySalaryModel>({
        ActualOfSalary: result.employeeMonthySalary.actualOfSalary,
        ActualWorkingDay: result.employeeMonthySalary.actualWorkingDay,
        ActualPaid: result.employeeMonthySalary.actualPaid,
        AdditionalAmount: result.employeeMonthySalary.additionalAmount,
        DesciplineAmount: result.employeeMonthySalary.desciplineAmount,
        MonthlyWorkingDay: result.employeeMonthySalary.monthlyWorkingDay,
        Overtime: result.employeeMonthySalary.overtime,
        OvertimeOfSalary: result.employeeMonthySalary.overtimeOfSalary,
        ReductionAmount: result.employeeMonthySalary.reductionAmount,
      });
    })

    // Get thông tin thông tin đánh giá
    this.assessmentEmpInforFormGroup.controls['yearToAssessment'].setValue(new Date().getFullYear());
    this.getAssessmentInfo(new Date().getFullYear());
  }
  /*Ket thuc*/

  // Edit thông tin tài khoản ngân hàng
  // onEditBankAccountByEmpId() {
  //   if (!this.hasBankAccount) {
  //     this.bankModel.ObjectId = this.employeeId;
  //     this.bankModel.ObjectType = 'EMP';
  //     this.bankModel.CreatedById = this.userId;
  //     this.bankModel.CreatedDate = new Date();
  //     this.bankModel.AccountName = this.employeeModel.EmployeeName;
  //     this.bankService.createBank(this.bankModel).subscribe(response => {
  //       let result = <any>response;
  //       this.getPersonalInfo();
  //     })
  //   } else {
  //     this.bankService.editBankById(this.bankModel, this.userId).subscribe(response => {
  //       let result = <any>response;
  //       this.getPersonalInfo();
  //     })
  //   }
  // }

  // Edit thông tin nhân viên
  onEditPersonalEmpInfo(mode) {
    this.editPersonalEmpInfo = mode;
    if (mode) {
      this.personalEmpInfoFormGroup.enable();
    } else {
      this.personalEmpInfoFormGroup.disable();
      this.contactModel.AvatarUrl = this.contactOldModel.AvatarUrl;
      this.employeeModel.EmployeeCode = this.employeeOldModel.EmployeeCode;
      this.contactModel.Gender = this.contactOldModel.Gender;
      this.contactModel.FirstName = this.contactOldModel.FirstName;
      this.contactModel.LastName = this.contactOldModel.LastName;
      this.contactModel.DateOfBirth = this.contactOldModel.DateOfBirth;
      this.contactModel.Phone = this.contactOldModel.Phone;
      this.contactModel.Email = this.contactOldModel.Email;
      this.contactModel.Address = this.contactOldModel.Address;
    }
  }

  // Edit thông tin lương & phụ cấp
  onEditSalaryAndAllowance(mode) {
    this.editSalaryEmpInfo = mode;
    if (mode) {
      this.salaryAndAllowanceEmpInfoFormGroup.enable();
    } else {
      this.salaryAndAllowanceEmpInfoFormGroup.disable();
    }
  }

  // Edit thông tin làm việc của nhân viên
  onEditWorkingEmpInfo(tab, mode) {
    if (mode) {
      if (tab === 'work') {
        this.workingEmpInfoFormGroup.enable();
        this.editWorkingEmpInfo = mode;
      } else {
        this.identityEmpInfoFormGroup.enable();
        this.editIdentityEmpInfo = mode;
      }
    } else {
      if (tab === 'work') {
        this.workingEmpInfoFormGroup.disable();
        this.editWorkingEmpInfo = mode;
        this.employeeModel.PositionId = this.employeeOldModel.PositionId;
        this.employeeModel.OrganizationName = this.employeeOldModel.OrganizationName;
        this.contactModel.WorkHourOfStart = this.contactOldModel.WorkHourOfStart;
        this.contactModel.WorkHourOfEnd = this.contactOldModel.WorkHourOfEnd;
        this.employeeModel.ContractType = this.employeeOldModel.ContractType;
        this.employeeModel.ContractEndDate = this.employeeOldModel.ContractEndDate;
        this.employeeModel.TrainingStartDate = this.employeeOldModel.TrainingStartDate;
        this.employeeModel.ProbationStartDate = this.employeeOldModel.ProbationStartDate;
        this.employeeModel.ProbationEndDate = this.employeeOldModel.ProbationEndDate;
        this.employeeModel.StartedDate = this.employeeOldModel.StartedDate;
        this.employeeModel.IsTakeCare = this.employeeOldModel.IsTakeCare;
        this.userModel.UserName = this.userOldModel.UserName;
        this.empStatusDisplay = this.empStatusOldDisplay;
        this.empStatus = this.empStatusOld;
      } else {
        this.identityEmpInfoFormGroup.disable();
        this.editIdentityEmpInfo = mode;
        this.identityEmpInfoFormGroup.controls["nationality"].setValue(this.nationality);
        this.contactModel.IdentityID = this.contactOldModel.IdentityID;
        this.contactModel.CountryId = this.contactOldModel.CountryId;
        this.contactModel.IdentityIDDateOfIssue = this.contactOldModel.IdentityIDDateOfIssue;
        this.contactModel.IdentityIDPlaceOfIssue = this.contactOldModel.IdentityIDPlaceOfIssue;
        this.contactModel.WorkPermitNumber = this.contactOldModel.WorkPermitNumber;
        this.contactModel.VisaNumber = this.contactOldModel.VisaNumber;
        this.contactModel.VisaDateOfIssue = this.contactOldModel.VisaDateOfIssue;
        this.contactModel.VisaExpirationDate = this.contactOldModel.VisaExpirationDate;
        this.contactModel.SocialInsuranceNumber = this.contactOldModel.SocialInsuranceNumber;
        this.contactModel.SocialInsuranceDateOfIssue = this.contactOldModel.SocialInsuranceDateOfIssue;
        this.contactModel.SocialInsuranceDateOfParticipation = this.contactOldModel.SocialInsuranceDateOfParticipation;
        this.contactModel.HealthInsuranceNumber = this.contactOldModel.HealthInsuranceNumber;
        this.contactModel.HealthInsuranceDateOfIssue = this.contactOldModel.HealthInsuranceDateOfIssue;
        this.contactModel.HealthInsuranceDateOfParticipation = this.contactOldModel.HealthInsuranceDateOfParticipation;
        this.contactModel.TypePaid = this.contactOldModel.TypePaid;
        this.bankModel.AccountNumber = this.bankOldModel.AccountNumber;
        this.bankModel.BankName = this.bankOldModel.BankName;
        this.bankModel.BranchName = this.bankOldModel.BranchName;
      }
    }
  }

  // Edit thong tin danh gia
  onEditAssessmentEmpInfo(mode) {
    this.editAssessmentEmpInfo = mode;
  }
  goToEmployee() {
    this.router.navigate(['/employee/list']);
  }
  // Tạo mới Employee Salary
  onCreateEmployeeSalary() {
    this.dialogCreateEmpSalary = this.dialog.open(EmployeeCreateSalaryPopupComponent,
      {
        width: '500px',
        autoFocus: false,
        data: { empId: this.employeeId, empName: this.employeeModel.EmployeeName }
      });

    this.dialogCreateEmpSalary.afterClosed().subscribe(result => {
      this.getEmployeeInfo();
    })
  }

  // Save mọi thông tin nhân viên
  onSaveUpdatePersonalEmpInfo(value) {
    if (value === 'savePersonalEmpInfo') {
      if (!this.personalEmpInfoFormGroup.valid) {
        Object.keys(this.personalEmpInfoFormGroup.controls).forEach(key => {
          if (this.personalEmpInfoFormGroup.controls[key].valid === false) {
            this.personalEmpInfoFormGroup.controls[key].markAsTouched();
          }
        });

        let target;

        target = this.el.nativeElement.querySelector('.emp-edit-control.ng-invalid');

        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
      } else {
        this.employeeService.editEmployeeById(this.employeeModel, this.contactModel, this.userModel, false).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.getPersonalInfo();
            this.editPersonalEmpInfo = false
            // Thông báo thay đổi thành công || thay đổi thất bại
            this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          } else {
            this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          }
        }, error => { });
      }
    } else {
      if (value == 'work') {
        switch (this.empStatus) {
          case 1:
            this.empStatusDisplay = this.trangthais[0].text;
            this.employeeModel.Active = true;
            this.userModel.Active = true;
            break;
          case 2:
            this.empStatusDisplay = this.trangthais[1].text;
            this.employeeModel.Active = true;
            this.userModel.Active = false;
            break;
          case 3:
            this.empStatusDisplay = this.trangthais[2].text;
            this.employeeModel.Active = false;
            this.userModel.Active = false;
            break;
          default:
            break;
        }

        if (!this.workingEmpInfoFormGroup.valid) {
          Object.keys(this.workingEmpInfoFormGroup.controls).forEach(key => {
            if (this.workingEmpInfoFormGroup.controls[key].valid === false) {
              this.workingEmpInfoFormGroup.controls[key].markAsTouched();
            }
          });
        } else {
          this.employeeService.editEmployeeById(this.employeeModel, this.contactModel, this.userModel, false).subscribe(response => {
            let result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              //this.onEditBankAccountByEmpId();
              this.getPersonalInfo();
              this.editWorkingEmpInfo = false;
              this.editIdentityEmpInfo = false;
              this.workingEmpInfoFormGroup.disable();
              // Thông báo thay đổi thành công || thay đổi thất bại
              this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
            } else {
              this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
            }
          }, error => { });
        }
      }
      else {
        if (!this.identityEmpInfoFormGroup.valid) {
          Object.keys(this.identityEmpInfoFormGroup.controls).forEach(key => {
            if (this.identityEmpInfoFormGroup.controls[key].valid === false) {
              this.identityEmpInfoFormGroup.controls[key].markAsTouched();
            }
          });
        } else {
          if (this.employeeModel.TrainingStartDate != null) this.employeeModel.TrainingStartDate = convertToUTCTime(new Date(this.employeeModel.TrainingStartDate));
          if (this.employeeModel.ProbationEndDate != null) this.employeeModel.ProbationEndDate = convertToUTCTime(new Date(this.employeeModel.ProbationEndDate));
          if (this.employeeModel.ProbationStartDate != null) this.employeeModel.ProbationStartDate = convertToUTCTime(new Date(this.employeeModel.ProbationStartDate));
          if (this.employeeModel.StartedDate != null) this.employeeModel.StartedDate = convertToUTCTime(new Date(this.employeeModel.StartedDate));
          if (this.employeeModel.ContractEndDate != null) this.employeeModel.ContractEndDate = convertToUTCTime(new Date(this.employeeModel.ContractEndDate));
          if (this.contactModel.DateOfBirth != null) this.contactModel.DateOfBirth = convertToUTCTime(new Date(this.contactModel.DateOfBirth));
          if (this.contactModel.IdentityIDDateOfIssue !=null) this.contactModel.IdentityIDDateOfIssue = convertToUTCTime(new Date(this.contactModel.IdentityIDDateOfIssue));
          if (this.contactModel.SocialInsuranceDateOfIssue != null) this.contactModel.SocialInsuranceDateOfIssue = convertToUTCTime(new Date(this.contactModel.SocialInsuranceDateOfIssue));
          if (this.contactModel.SocialInsuranceDateOfParticipation != null) this.contactModel.SocialInsuranceDateOfParticipation = convertToUTCTime(new Date(this.contactModel.SocialInsuranceDateOfParticipation));
          if (this.contactModel.VisaDateOfIssue != null) this.contactModel.VisaDateOfIssue = convertToUTCTime(setDateTime(new Date(this.contactModel.VisaDateOfIssue)));
          if (this.contactModel.VisaExpirationDate != null) this.contactModel.VisaExpirationDate = convertToUTCTime(setDateTime(new Date(this.contactModel.VisaExpirationDate)));
          if (this.contactModel.HealthInsuranceDateOfIssue != null) this.contactModel.HealthInsuranceDateOfIssue = convertToUTCTime(new Date(this.contactModel.HealthInsuranceDateOfIssue));
          if (this.contactModel.HealthInsuranceDateOfParticipation != null) this.contactModel.HealthInsuranceDateOfParticipation = convertToUTCTime(new Date(this.contactModel.HealthInsuranceDateOfParticipation));
          this.employeeService.editEmployeeById(this.employeeModel, this.contactModel, this.userModel, false).subscribe(response => {
            let result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              //this.onEditBankAccountByEmpId();
              this.getPersonalInfo();
              this.editWorkingEmpInfo = false;
              this.editIdentityEmpInfo = false;
              this.identityEmpInfoFormGroup.disable();
              // Thông báo thay đổi thành công || thay đổi thất bại
              this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
            } else {
              this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
            }
          }, error => { });
        }
      }
    }
  }

  // Kiểm tra validator thông tin lương và phụ cấp nhân viên
  onSaveSalary() {
    if (!this.salaryAndAllowanceEmpInfoFormGroup.valid) {
      Object.keys(this.salaryAndAllowanceEmpInfoFormGroup.controls).forEach(key => {
        if (this.salaryAndAllowanceEmpInfoFormGroup.controls[key].valid === false) {
          this.salaryAndAllowanceEmpInfoFormGroup.controls[key].markAsTouched();
        }
      });

      let target;

      target = this.el.nativeElement.querySelector('.emp-edit-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      this.onSaveSalaryAndAllowance();
    }
  }

  // Save thông tin lương và phụ cấp nhân viên
  onSaveSalaryAndAllowance() {
    this.loading = true;
    this.employeeSalaryModel = new EmployeeSalaryModel();
    this.employeeSalaryModel.EmployeeSalaryBase = this.employeeSalaryBase;
    this.employeeSalaryModel.EmployeeId = this.employeeId;
    this.employeeSalaryModel.EffectiveDate = this.effecticeDateToSearch;
    this.employeeSalaryService.creatEmpSalary(this.employeeSalaryModel, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.statusCode !== 200 && result.statusCode !== 202) {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        return;
      }
    });
    this.employeeAllowance.EmployeeId = this.employeeId;
    this.employeeAllowanceService.editEmpAllowance(this.employeeAllowance, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.statusCode !== 200 && result.statusCode !== 202) {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        return;
      }
    });
    this.employeeInsuranceModel.EmployeeId = this.employeeId;
    if (this.hasInsurance) {
      this.employeeInsuranceService.editEmployeeInsurance(this.employeeInsuranceModel, this.userId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode !== 200 && result.statusCode !== 202) {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          return;
        }
        this.getSalaryEmp()
      })
    } else {
      this.employeeInsuranceService.createEmployeeInsurance(this.employeeInsuranceModel, this.userId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode !== 200 && result.statusCode !== 202) {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          return;
        }
        this.getSalaryEmp()
      })
    }
    this.editSalaryEmpInfo = false;
    this.salaryAndAllowanceEmpInfoFormGroup.disable();
    this.snackBar.openFromComponent(SuccessComponent, { data: "Chỉnh sửa lương nhân viên thành công", ...this.successConfig });
  }
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  // Mở Dialog chọn đơn vị
  openOrgDialog(mode: string) {
    this.dialogOrg = this.dialog.open(OrgSelectDialogComponent,
      {
        width: '500px',
        autoFocus: false,
        data: { selectedId: this.currentOrganizationId, mode: mode }
      });

    this.dialogOrg.afterClosed().subscribe(result => {
      if (result === undefined) return;
      if (result.chosenItem) {
        this.employeeModel.OrganizationId = result.selectedId;
        this.employeeModel.OrganizationName = result.selectedName;
      }
    });
  }

  /*Mo dialog chinh sua anh dai dien*/
  openimageDialog(): void {
    
  }

  DeleteAvatarUrl() {
    this.contactModel.AvatarUrl = '';
  }
  // Get thông tin lương nhân viên
  async getSalaryEmp() {
    this.loading = true;
    // Get thông tin lương nhân viên
    this.listEmployeeSalaryModel = [];
    this.employeeSalaryService.getEmpSalaryByEmpId(this.employeeId, this.effecticeDateToSearch, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.listEmployeeSalary === null || result.listEmployeeSalary.length === 0) {
        this.employeeSalaryModel.EmployeeSalaryBase = 0;
        this.listEmployeeSalaryModel.push(this.employeeSalaryModel);
      } else {
        result.listEmployeeSalary.forEach(emps => {
          this.employeeSalaryModel = <EmployeeSalaryModel>({
            EmployeeSalaryId: emps.employeeSalaryId,
            EmployeeSalaryBase: emps.employeeSalaryBase,
            EffectiveDate: emps.effectiveDate,
            EmployeeId: this.employeeId,
            ResponsibilitySalary: emps.responsibilitySalary
          })
          this.listEmployeeSalaryModel.push(this.employeeSalaryModel);
        })
        this.dataTableSalary.data = this.listEmployeeSalaryModel;
        this.dataTableSalary.paginator = this.paginator;
      }
    })

    // Get thông tin phụ cấp nhân viên
    // this.employeeAllowanceService.getEmpAllowanceByEmpId(this.employeeId, this.userId).subscribe(response => {
    //   let result = <any>response;
    //   this.employeeAllowance = <EmployeeAllowanceModel>({
    //     MaternityAllowance: result.employeeAllowance.maternityAllowance,
    //     LunchAllowance: result.employeeAllowance.lunchAllowance,
    //     FuelAllowance: result.employeeAllowance.fuelAllowance,
    //     PhoneAllowance: result.employeeAllowance.phoneAllowance,
    //     OtherAllownce: result.employeeAllowance.otherAllownce
    //   })
    //   this.listEmployeeAllowanceModel = [];
    //   this.listEmployeeAllowanceModel.push(this.employeeAllowance);
    //   this.dataTableAllowance = new MatTableDataSource<EmployeeAllowanceModel>(this.listEmployeeAllowanceModel);
    // })

    let dataEmployeeAllowance: any = await this.employeeAllowanceService.getEmpAllowanceByEmpIdAsync(this.employeeId, this.userId);
    this.employeeAllowance = <EmployeeAllowanceModel>({
      MaternityAllowance: dataEmployeeAllowance.employeeAllowance.maternityAllowance,
      LunchAllowance: dataEmployeeAllowance.employeeAllowance.lunchAllowance,
      FuelAllowance: dataEmployeeAllowance.employeeAllowance.fuelAllowance,
      PhoneAllowance: dataEmployeeAllowance.employeeAllowance.phoneAllowance,
      OtherAllownce: dataEmployeeAllowance.employeeAllowance.otherAllownce
    })
    this.listEmployeeAllowanceModel = [];
    this.listEmployeeAllowanceModel.push(this.employeeAllowance);
    this.dataTableAllowance = new MatTableDataSource<EmployeeAllowanceModel>(this.listEmployeeAllowanceModel);

    // Get thông tin Bảo hiểm
    this.employeeInsuranceService.searchEmployeeInsurance(this.employeeId, null, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.listEmployeeInsurance.length !== 0) {
        this.hasInsurance = true;
        this.employeeInsuranceModel = <EmployeeInsuranceModel>({
          EffectiveDate: result.listEmployeeInsurance[0].effectiveDate,
          HealthInsurancePercent: result.listEmployeeInsurance[0].healthInsurancePercent,
          HealthInsuranceSupportPercent: result.listEmployeeInsurance[0].healthInsuranceSupportPercent,
          SocialInsurancePercent: result.listEmployeeInsurance[0].socialInsurancePercent,
          SocialInsuranceSalary: result.listEmployeeInsurance[0].socialInsuranceSalary,
          SocialInsuranceSupportPercent: result.listEmployeeInsurance[0].socialInsuranceSupportPercent,
          UnemploymentinsurancePercent: result.listEmployeeInsurance[0].unemploymentinsurancePercent,
          UnemploymentinsuranceSupportPercent: result.listEmployeeInsurance[0].unemploymentinsuranceSupportPercent
        })
      }
    })
    this.loading = false;
  }
  // Get thông tin đánh giá theo năm
  getAssessmentInfo(value: any) {
    this.loading = true;
    this.listEmployeeAssessmentModel = [];
    // Get thông tin thông tin đánh giá
    this.employeeAssessmentService.getEmpAssessmentByEmpId(this.employeeId, value, this.userId).subscribe(response => {
      let result = <any>response;
      let tmp: any;
      let listmonth = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'];
      result.listEmployeeAssessment.forEach(ea => {
        tmp = <Assessment>({
          month: ea.month,
          evaluate: '',
          monthText: ea.month.toString(),
          eveluateId: ea.type
        })
        listmonth.splice(listmonth.indexOf(ea.month.toString()), 1);
        tmp.evaluate = this.evaluateList.find(ev => ev.categoryId === ea.type) === undefined ? '' : this.evaluateList.find(ev => ev.categoryId === ea.type).categoryName;
        this.listEmployeeAssessmentModel.push(tmp);
      })
      listmonth.forEach(ac => {
        tmp = <Assessment>({
          month: parseInt(ac),
          evaluate: '',
          monthText: ac,
          eveluateId: ''
        })
        this.listEmployeeAssessmentModel.push(tmp);
      })
      this.listEmployeeAssessmentModel.sort((a: any, b: any) => {
        let x = a.month;
        let y = b.month;
        return x - y;
      })
    })
    this.loading = false;
  }

  // Get thông tin cá nhân + công việc
  getPersonalInfo() {
    this.loading = true;
    this.employeeService.getEmployeeById(this.employeeId, this.contactId).subscribe(response => {
      let result = <any>response;
      this.employeeModel = <EmployeeModel>({
        EmployeeCode: result.employee.employeeCode,
        EmployeeId: result.employee.employeeId,
        EmployeeName: result.employee.employeeName,
        StartedDate: result.employee.startedDate == null ? null : new Date(result.employee.startedDate.toString()),
        OrganizationId: result.employee.organizationId,
        PositionId: result.employee.positionId == null ? "" : result.employee.positionId,
        CreatedById: result.employee.createdById,
        CreatedDate: result.employee.createdDate,
        UpdatedById: result.employee.updatedById,
        UpdatedDate: result.employee.updatedDate,
        Active: result.employee.active,
        Username: result.employee.username,
        Identity: result.employee.identity,
        OrganizationName: result.employee.organizationName,
        PositionName: result.employee.positionName,
        IsManager: result.employee.isManager,
        PermissionSetId: result.employee.permissionSetId,
        AvatarUrl: result.employee.avatarUrl,
        ProbationEndDate: result.employee.probationEndDate == null ? null : new Date(result.employee.probationEndDate.toString()),
        ProbationStartDate: result.employee.probationStartDate == null ? null : new Date(result.employee.probationStartDate.toString()),
        TrainingStartDate: result.employee.trainingStartDate == null ? null : new Date(result.employee.trainingStartDate.toString()),
        ContractEndDate: result.employee.contractEndDate == null ? null : new Date(result.employee.contractEndDate.toString()),
        ContractType: result.employee.contractType,
        IsTakeCare: result.employee.isTakeCare,
      });
      this.employeeOldModel = <EmployeeModel>({
        EmployeeCode: result.employee.employeeCode,
        EmployeeId: result.employee.employeeId,
        EmployeeName: result.employee.employeeName,
        StartedDate: result.employee.startedDate == null ? null : new Date(result.employee.startedDate.toString()),
        OrganizationId: result.employee.organizationId,
        PositionId: result.employee.positionId == null ? "" : result.employee.positionId,
        CreatedById: result.employee.createdById,
        CreatedDate: result.employee.createdDate,
        UpdatedById: result.employee.updatedById,
        UpdatedDate: result.employee.updatedDate,
        Active: result.employee.active,
        Username: result.employee.username,
        Identity: result.employee.identity,
        OrganizationName: result.employee.organizationName,
        PositionName: result.employee.positionName,
        IsManager: result.employee.isManager,
        PermissionSetId: result.employee.permissionSetId,
        AvatarUrl: result.employee.avatarUrl,
        ProbationEndDate: result.employee.probationEndDate == null ? null : new Date(result.employee.probationEndDate.toString()),
        ProbationStartDate: result.employee.probationStartDate == null ? null : new Date(result.employee.probationStartDate.toString()),
        TrainingStartDate: result.employee.trainingStartDate == null ? null : new Date(result.employee.trainingStartDate.toString()),
        ContractEndDate: result.employee.contractEndDate == null ? null : new Date(result.employee.contractEndDate.toString()),
        ContractType: result.employee.contractType,
        IsTakeCare: result.employee.isTakeCare,
      });

      this.contactModel = <ContactModel>({
        ContactId: result.contact.contactId,
        ObjectId: result.contact.objectId,
        CountryId: result.contact.countryId,
        ObjectType: result.contact.objectType,
        FirstName: result.contact.firstName,
        LastName: result.contact.lastName,
        Gender: result.contact.gender,
        DateOfBirth: result.contact.dateOfBirth == null ? null : new Date(result.contact.dateOfBirth.toString()),
        Phone: result.contact.phone,
        Email: result.contact.email,
        IdentityID: result.contact.identityId,
        AvatarUrl: result.contact.avatarUrl,
        Address: result.contact.address,
        ProvinceId: result.contact.provinceId,
        DistrictId: result.contact.districtId,
        WardId: result.contact.wardId,
        PostCode: result.contact.postCode,
        WebsiteUrl: result.contact.websiteUrl,
        SocialUrl: result.contact.socialUrl,
        CreatedById: result.contact.createdById,
        CreatedDate: result.contact.createdDate,
        UpdatedById: result.contact.updatedById,
        UpdatedDate: result.contact.updatedDate,
        Active: result.contact.active,
        Note: result.contact.note,
        TaxCode: result.contact.taxCode,
        Job: result.contact.job,
        Agency: result.contact.agency,
        Birthplace: result.contact.birthplace,
        HealthInsuranceDateOfIssue: result.contact.healthInsuranceDateOfIssue,
        HealthInsuranceDateOfParticipation: result.contact.healthInsuranceDateOfParticipation,
        HealthInsuranceNumber: result.contact.healthInsuranceNumber,
        IdentityIDDateOfIssue: result.contact.identityIddateOfIssue,
        IdentityIDDateOfParticipation: result.contact.identityIddateOfParticipation,
        IdentityIDPlaceOfIssue: result.contact.identityIdplaceOfIssue,
        WorkPermitNumber: result.contact.workPermitNumber,
        SocialInsuranceDateOfIssue: result.contact.socialInsuranceDateOfIssue,
        SocialInsuranceDateOfParticipation: result.contact.socialInsuranceDateOfParticipation,
        SocialInsuranceNumber: result.contact.socialInsuranceNumber,
        VisaDateOfIssue: result.contact.visaDateOfIssue,
        VisaExpirationDate: result.contact.visaExpirationDate,
        VisaNumber: result.contact.visaNumber,
        WorkHourOfEnd: result.contact.workHourOfEnd,
        WorkHourOfStart: result.contact.workHourOfStart,
        TypePaid: result.contact.typePaid
      });
      this.contactOldModel = <ContactModel>({
        ContactId: result.contact.contactId,
        ObjectId: result.contact.objectId,
        CountryId: result.contact.countryId,
        ObjectType: result.contact.objectType,
        FirstName: result.contact.firstName,
        LastName: result.contact.lastName,
        Gender: result.contact.gender,
        DateOfBirth: result.contact.dateOfBirth == null ? null : new Date(result.contact.dateOfBirth.toString()),
        Phone: result.contact.phone,
        Email: result.contact.email,
        IdentityID: result.contact.identityId,
        AvatarUrl: result.contact.avatarUrl,
        Address: result.contact.address,
        ProvinceId: result.contact.provinceId,
        DistrictId: result.contact.districtId,
        WardId: result.contact.wardId,
        PostCode: result.contact.postCode,
        WebsiteUrl: result.contact.websiteUrl,
        SocialUrl: result.contact.socialUrl,
        CreatedById: result.contact.createdById,
        CreatedDate: result.contact.createdDate,
        UpdatedById: result.contact.updatedById,
        UpdatedDate: result.contact.updatedDate,
        Active: result.contact.active,
        Note: result.contact.note,
        TaxCode: result.contact.taxCode,
        Job: result.contact.job,
        Agency: result.contact.agency,
        Birthplace: result.contact.birthplace,
        HealthInsuranceDateOfIssue: result.contact.healthInsuranceDateOfIssue,
        HealthInsuranceDateOfParticipation: result.contact.healthInsuranceDateOfParticipation,
        HealthInsuranceNumber: result.contact.healthInsuranceNumber,
        IdentityIDDateOfIssue: result.contact.identityIddateOfIssue,
        IdentityIDDateOfParticipation: result.contact.identityIddateOfParticipation,
        IdentityIDPlaceOfIssue: result.contact.identityIdplaceOfIssue,
        WorkPermitNumber: result.contact.workPermitNumber,
        SocialInsuranceDateOfIssue: result.contact.socialInsuranceDateOfIssue,
        SocialInsuranceDateOfParticipation: result.contact.socialInsuranceDateOfParticipation,
        SocialInsuranceNumber: result.contact.socialInsuranceNumber,
        VisaDateOfIssue: result.contact.visaDateOfIssue,
        VisaExpirationDate: result.contact.visaExpirationDate,
        VisaNumber: result.contact.visaNumber,
        WorkHourOfEnd: result.contact.workHourOfEnd,
        WorkHourOfStart: result.contact.workHourOfStart,
        TypePaid: result.contact.typePaid
      });

      this.userModel = <UserModel>({
        UserId: result.user.userId,
        Active: result.user.active,
        EmployeeId: result.user.employeeId,
        UserName: result.user.userName,
        Password: result.user.password,
        CreatedDate: result.user.createdDate,
        CreatedById: result.user.createdById
      });
      this.userOldModel = <UserModel>({
        UserId: result.user.userId,
        Active: result.user.active,
        EmployeeId: result.user.employeeId,
        UserName: result.user.userName,
        Password: result.user.password,
        CreatedDate: result.user.createdDate,
        CreatedById: result.user.createdById
      });
      let national = this.listCountry.find(lc => lc.countryId === this.contactModel.CountryId);
      if (national !== null && national !== undefined) {
        this.hasNationality = true;
        this.identityEmpInfoFormGroup.controls["nationality"].setValue(national.countryName);
        this.nationality = national.countryName;
      } else {
        this.hasNationality = false;
      }
      
      if (this.employeeModel.Active && this.userModel.Active) {
        this.empStatusDisplay = this.trangthais[0].text;
        this.empStatus = this.trangthais[0].id;
      } else if (this.employeeModel.Active && !this.userModel.Active) {
        this.empStatusDisplay = this.trangthais[1].text;
        this.empStatus = this.trangthais[1].id;
      } else {
        this.empStatusDisplay = this.trangthais[2].text;
        this.empStatus = this.trangthais[2].id;
      }

      this.empStatusOldDisplay = this.empStatusDisplay;
      this.empStatusOld = this.empStatus;
      this.getModuleByPermissionSetId(this.employeeModel.PermissionSetId);

    }, error => { });

    // Get tài khoản ngân hàng của nhân viên
    this.bankService.getAllBankAccountByObject(this.employeeId, 'EMP').subscribe(response => {
      let result = <any>response;
      if (result.bankAccountList.length > 0) {
        this.hasBankAccount = true;
        this.bankModel = <BankModel>({
          ObjectType: result.bankAccountList[0].objectType,
          CreatedDate: result.bankAccountList[0].createdDate,
          CreatedById: result.bankAccountList[0].createdById,
          Active: result.bankAccountList[0].active,
          ObjectId: result.bankAccountList[0].objectId,
          BankDetail: result.bankAccountList[0].bankDetail,
          BankAccountId: result.bankAccountList[0].bankAccountId,
          AccountName: result.bankAccountList[0].accountName,
          AccountNumber: result.bankAccountList[0].accountNumber,
          BankName: result.bankAccountList[0].bankName,
          BranchName: result.bankAccountList[0].branchName
        });
        this.bankOldModel = <BankModel>({
          ObjectType: result.bankAccountList[0].objectType,
          CreatedDate: result.bankAccountList[0].createdDate,
          CreatedById: result.bankAccountList[0].createdById,
          Active: result.bankAccountList[0].active,
          ObjectId: result.bankAccountList[0].objectId,
          BankDetail: result.bankAccountList[0].bankDetail,
          BankAccountId: result.bankAccountList[0].bankAccountId,
          AccountName: result.bankAccountList[0].accountName,
          AccountNumber: result.bankAccountList[0].accountNumber,
          BankName: result.bankAccountList[0].bankName,
          BranchName: result.bankAccountList[0].branchName
        });
      } else {
        this.hasBankAccount = false;
      }
    })
    this.loading = false;
  }

  //
  paginationFunction() {
    this.paginator._intl.itemsPerPageLabel = 'Số mục mỗi trang: ';
    this.paginator._intl.getRangeLabel = (page, pageSize, length) => {
      if (length === 0 || pageSize === 0) {
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
  /*Mo edit data_permission*/
  editDatapermission(mode) {
    this.isEditDatapermission = this.isEditDatapermission ? false : true;
  }
  /*Ket thuc*/

  saveDataPermission() {
    this.loading = true;
    this.employeeService.editEmployeeDataPermission(this.employeeId, this.employeeModel.IsManager).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.isEditDatapermission = false;
        this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }
    }, error => { });
    this.loading = false;
  }
  radioSelected: boolean[] = [false, false, false];
  /* Xảy ra khi giá trị phân quyền dữ liệu thay đổi */
  changeradiobutton(event: MatRadioChange) {
    var index = event.value;
    this.radioSelected[index] = true;
    if (index === '1') {
      //Quản lý
      this.radioSelected[2] = false;
      this.employeeModel.IsManager = true;
    } else {
      //Nhân viên
      this.radioSelected[1] = false;
      this.employeeModel.IsManager = false;
    }
  }
  /*Ket thuc */
  toggleCollapseIcon(event) {
    let icon = $(event.target);
    const text = icon.text();
    if (text === 'Chi tiết >>' || text === 'Thu gọn <<') {
      icon.text(text.trim() === 'Chi tiết >>' ? 'Thu gọn <<' : 'Chi tiết >>');
      return;
    }
    icon.text(text.trim() === 'expand_less' ? 'expand_more' : 'expand_less');
  }
  /*Mo edit function_permission*/
  editFunctionpermission(mode) {
    this.isEditFunctionpermission = this.isEditFunctionpermission ? false : true;
  }

  saveFunctionPermission() {
    if (this.roleId != null) {
      this.loading = true;
      this.permissisonService.addUserRole(this.employeeId, this.roleId, this.userId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.isEditFunctionpermission = false;
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
        }
        else {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        }
      });
    }
    else {
      this.isEditFunctionpermission = false;
    }
  }
  /*Ket thuc */

  getModuleByPermissionSetId(id: string) {
    this.permissisonService.getModuleByPermissionSetId(id).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.moduleNameList = result.PermissionListAsModule;
      }
    }, error => { });
  }

  // Huy du lieu da duoc ghi
  onCancelSaveNote(): void {
    let _title = "XÁC NHẬN";
    let _content = "Bạn có chắc chắn huỷ? Các dữ liệu sẽ không được lưu";
    this.dialogPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      if (result) {
        this.noteContent = '';
        this.files = [];
      }
    });
  }

  //Function save note when click button "Lưu"
  btnSaveNoteClick() {
    this.loading = true;
    this.onSaveNote('ADD', true);
    //this.sendEmailAfterCreateNote();
    this.noteContent = '';
    this.files = [];
    this.loading = false;
  }
  // End

  // Save note to server
  onSaveNote(type: string, showMessage: boolean) {
    this.noteModel.Description = this.noteContent.trim();
    this.noteModel.Type = type;

    if (this.files.length > 0) {
      this.uploadFiles(this.files);
    }

    this.noteService.createNote(this.noteModel, this.employeeId, this.files, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if ((result.statusCode === 202 || result.statusCode === 200) && showMessage) {
        this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
        this.getNoteHistory();
      }
    });
  }


  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { }, error => { });
  }
  // Remove file
  onRemoveFile(index: number) {
    this.files.splice(index, 1);
  }
  // End

  // Get history note
  getNoteHistory() {
    this.loading = true;
    this.customerService.getNoteHistory(this.employeeId).subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.noteHistory = result.listNote;
        result.listNote.forEach(element => {
          this.noteModel.Description = element.description;
          this.noteModel.NoteId = element.noteId;
          this.noteModel.NoteTitle = element.noteTitle;
          setTimeout(() => {
            $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
            $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
          }, 1000);

        });
      } else {

      }
    });
    this.loading = false;

  }
  // End

  // Edit a Note
  onEditNote() {
    if (this.files.length > 0) {
      this.uploadFiles(this.files);
    }
    this.noteService.editNoteById(this.noteId, this.noteContent, this.files, this.employeeId).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.snackBar.openFromComponent(SuccessComponent, {
          data: result.messageCode, ...this.successConfig
        });
        this.noteContent = '';
        this.isEditNote = false;
        this.getNoteHistory();
        this.files = [];
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }

    }, error => { });
  }
  // End

  // Disable/Delete Note
  onDisableNote(nodeId) {
    var _confirm;
    let _title = "XÁC NHẬN";
    let _content = "Bạn có chắc chắn muốn xóa ghi chú ?";
    this.dialogPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      _confirm = result;
      if (_confirm) {
        this.loading = true;
        this.noteService.disableNote(nodeId).subscribe(response => {
          const result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
            //this.getLeadInfo();
            this.getNoteHistory();
          } else {
            this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          }

        }, error => {
          const result = <any>error;
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        });
      }
    });
  }
  // End

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  //Filter Validator
  myFilter = (d: Date): boolean => {
    let day = d;
    let n = day.setHours(0, 0, 0, 0);
    const now = new Date();

    return (day <= now);
  }

  filterTrainingStartDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.employeeModel.ProbationStartDate === null || this.employeeModel.ProbationStartDate === undefined ? 0 : this.employeeModel.ProbationStartDate.setHours(0, 0, 0, 0);
    let p = this.employeeModel.ProbationEndDate === null || this.employeeModel.ProbationEndDate === undefined ? 0 : this.employeeModel.ProbationEndDate.setHours(0, 0, 0, 0);
    let q = this.employeeModel.StartedDate === null || this.employeeModel.StartedDate === undefined ? 0 : this.employeeModel.StartedDate.setHours(0, 0, 0, 0);
    return ((n <= m || m === 0) && (n <= p || p === 0) && (n <= q || q === 0));
  }

  filterProbationStartDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.employeeModel.TrainingStartDate === null || this.employeeModel.TrainingStartDate === undefined ? 0 : this.employeeModel.TrainingStartDate.setHours(0, 0, 0, 0);
    let p = this.employeeModel.ProbationEndDate === null || this.employeeModel.ProbationEndDate === undefined ? 0 : this.employeeModel.ProbationEndDate.setHours(0, 0, 0, 0);
    let q = this.employeeModel.StartedDate === null || this.employeeModel.StartedDate === undefined ? 0 : this.employeeModel.StartedDate.setHours(0, 0, 0, 0);

    return ((n >= m || m === 0) && (n <= p || p === 0) && (n <= q || q === 0));
  }

  filterProbationEndDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.employeeModel.TrainingStartDate === null || this.employeeModel.TrainingStartDate === undefined ? 0 : this.employeeModel.TrainingStartDate.setHours(0, 0, 0, 0);
    let p = this.employeeModel.ProbationStartDate === null || this.employeeModel.ProbationStartDate === undefined ? 0 : this.employeeModel.ProbationStartDate.setHours(0, 0, 0, 0);
    let q = this.employeeModel.StartedDate === null || this.employeeModel.StartedDate === undefined ? 0 : this.employeeModel.StartedDate.setHours(0, 0, 0, 0);
    return ((n >= m || m === 0) && (n >= p || p === 0) && (n <= q || q === 0));
  }

  filterStartedDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.employeeModel.TrainingStartDate === null || this.employeeModel.TrainingStartDate === undefined ? 0 : this.employeeModel.TrainingStartDate.setHours(0, 0, 0, 0);
    let p = this.employeeModel.ProbationEndDate === null || this.employeeModel.ProbationEndDate === undefined ? 0 : this.employeeModel.ProbationEndDate.setHours(0, 0, 0, 0);
    let q = this.employeeModel.ProbationStartDate === null || this.employeeModel.ProbationStartDate === undefined ? 0 : this.employeeModel.ProbationStartDate.setHours(0, 0, 0, 0);

    return ((n >= m || m === 0) && (n >= p || p === 0) && (n >= q || q === 0));
  }
  scrollWrapper() {
    if ($(".wrapper").scrollTop() < 100) {
      $('.wrapper').animate({ scrollTop: 100 }, 'slow');
    }
  }
  resetAllInfoEmp(event: MatTabChangeEvent) {
    if (event.index === 0 || event.index === 1) {
      //this.getPersonalInfo();
    }
  }

  del_user() {
    let _title = "XÁC NHẬN";
    let _content = "Bạn có chắc chắn muốn xóa?";
    this.dialogConfirm = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '300px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });

    this.dialogConfirm.afterClosed().subscribe(resultPopup => {
      if (resultPopup) {
        this.employeeListService.disableEmployee(this.employeeId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.snackBar.openFromComponent(SuccessComponent,
              {
                duration: 5000,
                data: result.messageCode,
                panelClass: 'success-dialog',
                horizontalPosition: 'end'
              });
            this.router.navigate(['/employee/list']);
          } else {
            this.snackBar.openFromComponent(FailComponent,
              {
                duration: 5000,
                data: result.messageCode,
                panelClass: 'fail-dialog',
                horizontalPosition: 'end'
              });
          }
          this.ngOnInit();
        }, error => { });
        this.dialogConfirm.close();
      }
    });
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  onKeyPress(event: any) {
    const pattern = /^[0-9\.]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  /*Xóa dữ liệu ô quoc tich*/
  clearDataCountry() {
    this.identityEmpInfoFormGroup.controls['nationality'].reset();
    this.contactModel.CountryId = null;
  }
  /*End*/

  confirm() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn reset mật khẩu cho người dùng ' + this.contactModel.FirstName + ' ' + this.contactModel.LastName + '?',
      accept: () => {
        this.employeeService.editEmployeeById(this.employeeModel, this.contactModel, this.userModel, true).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.getPersonalInfo();
            this.editPersonalEmpInfo = false
            // Thông báo thay đổi thành công || thay đổi thất bại
            this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          } else {
            this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          }
        }, error => { });
      }
    });
  }

}
function checkNationality(array: Array<any>, hasNationality: boolean): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      let val = array.find(el => el.countryName.toLowerCase() === control.value.toLowerCase());
      if ((val !== null && val !== undefined) || control.value === '' || hasNationality) {
        return { 'checkNationality': true };
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function setDateTime(time:Date){
  if(time.getFullYear() == new Date().getFullYear() && time.getMonth() == new Date().getMonth() && time.getDay() == new Date().getDay()){
    return new Date();
  }

  return time;
}

