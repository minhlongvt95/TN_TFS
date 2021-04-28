import { Component, OnInit, ElementRef, HostListener, Inject, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NoteService } from '../../../shared/services/note.service';
import { TranslateService } from '@ngx-translate/core';
import { LeadService } from "../../services/lead.service";
import { CompanyService } from "../../../shared/services/company.service";
import { ContactService } from "../../../shared/services/contact.service";
import { CategoryService } from "../../../shared/services/category.service";
import { EmployeeService } from "../../../employee/services/employee.service";
import { WardService } from '../../../shared/services/ward.service';
import { ProvinceService } from '../../../shared/services/province.service';
import { DistrictService } from '../../../shared/services/district.service';
import { EmailService } from '../../../shared/services/email.service';
import { LeadModel } from "../../models/lead.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { NoteModel } from '../../../shared/models/note.model';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import * as $ from 'jquery';
import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";

export interface IDialogData {
  lead: LeadModel;
  contact: ContactModel;
  isLoadData: boolean;
}

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})

export class CreateComponent implements OnInit {
/*Khai báo biến*/

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
  @ViewChild('email', { static: true }) emailElement: ElementRef;
  @ViewChild('phone', { static: true }) phoneElement: ElementRef;

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth = JSON.parse(localStorage.getItem("auth"));
  currentEmployeeId = this.auth.EmployeeId;
  isManager: boolean = null;
  employeeId: string = '00000000-0000-0000-0000-000000000000';

  /*Khai báo các form control*/
  createForm: FormGroup;
  formGender: FormControl;
  formfirstName: FormControl;
  formlastName: FormControl;
  formEmail: FormControl;
  formPhone: FormControl;
  formCity: FormControl;
  formDistrict: FormControl;
  formGuild: FormControl;
  formAddress: FormControl;
  formRequirement: FormControl;
  formRequirementDetail: FormControl;
  formPayment: FormControl;
  formPotential: FormControl;
  formCompany: FormControl;
  formCompanyName: FormControl;
  picCtrl = new FormControl();

  isCreateCompany: boolean;
  isCreateLeadAndResetField: Boolean;
  companyName: string;
  controls = new FormControl();
  messages: any = {};
  toastMessage: string;
  companies: Array<string> = [];
  selectedPic: Array<any> = [];
  filteredCompanies: Observable<string[]>;
  filteredProvinces: Observable<string[]>;
  filteredDistricts: Observable<string[]>;
  filteredWards: Observable<string[]>;
  filteredPic: Observable<string[]>;
  requirements: Array<string> = [];
  payments: Array<string> = [];
  employees: Array<any> = [];
  potentials: Array<string> = [];
  provinces: Array<string> = [];
  districts: Array<string> = [];
  wards: Array<string> = [];
  genders = [{ code: 'NAM', name: 'Ông' }, { code: 'NU', name: 'Bà' }];
  returnUrl: string;
  private productService: string = 'NHU';
  private potentialCode: string = 'MTN';
  private paymentCode: string = 'PTO';
  private genderCode: string = 'GTI';
  dialogPopup: MatDialogRef<PopupComponent>;

  leadModel = new LeadModel();
  contactModel = new ContactModel();
  noteModel = new NoteModel();
  errEmail: boolean = false;
  objectType: string;
  errPhone: boolean = false;
  objectId: string;
  contactId: string;

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  /*Kết thúc khai báo biến*/

  /*Khởi tại constructor*/
  constructor(private translate: TranslateService,
    private leadService: LeadService,
    private companyService: CompanyService,
    private contactService: ContactService,
    private categoryService: CategoryService,
    private employeeService: EmployeeService,
    private wardService: WardService,
    private districtService: DistrictService,
    private provinceService: ProvinceService,
    private route: ActivatedRoute,
    private router: Router,
    private noteService: NoteService,
    private emailService: EmailService,
    public dialog: MatDialogRef<CreateComponent>,
    public dialogPop: MatDialog,
    public snackBar: MatSnackBar,
    public builder: FormBuilder,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData) {
    translate.setDefaultLang('vi');

    /*Khởi tạo các form control*/
    this.formfirstName = new FormControl('', [Validators.required]);
    this.formlastName = new FormControl('', [Validators.required]);
    this.formGender = new FormControl('', [Validators.required]);
    this.formEmail = new FormControl('', [Validators.pattern('^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}')]);
    this.formPhone = new FormControl('', [Validators.required, Validators.pattern(this.getPhonePattern())]);
    this.formCity = new FormControl('', [Validators.required]);
    this.formDistrict = new FormControl('', [Validators.required]);
    this.formGuild = new FormControl('', [Validators.required]);
    this.formAddress = new FormControl('', [Validators.required]);
    this.formRequirement = new FormControl('', [Validators.required]);
    this.formRequirementDetail = new FormControl();
    this.formPayment = new FormControl('', [Validators.required]);
    this.formPotential = new FormControl('', [Validators.required]);
    this.formCompany = new FormControl();
    this.formCompanyName = new FormControl('');
    this.picCtrl = new FormControl('');

    this.createForm = builder.group({
      formGender: this.formGender,
      formfirstName: this.formfirstName,
      formlastName: this.formlastName,
      formEmail: this.formEmail,
      formPhone: this.formPhone,
      formCity: this.formCity,
      formDistrict: this.formDistrict,
      formGuild: this.formGuild,
      formAddress: this.formAddress,
      formRequirement: this.formRequirement,
      formRequirementDetail: this.formRequirementDetail,
      formPayment: this.formPayment,
      formPotential: this.formPotential,
      formCompany: this.formCompany,
      picCtrl: this.picCtrl,
      formCompanyName: this.formCompanyName
    });
  }

  ngOnInit() {
    this.getMasterData();
    this.leadModel.PersonInChargeId = this.auth.EmployeeId;
    this.dialog.disableClose = true;
  }
  
  cancelCreate() {
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
        this.dialog.close(this.data);
      }
    });
  }

  getPhonePattern(){
   let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType"); 
   return phonePatternObj.systemValueString;
  }


  /*Function lấy ra các master data*/
 async getMasterData() {
    /*Company*/
    this.companyService.getAllCompany().subscribe(response => {
      let result = <any>response;
      this.companies = result.company;
      this.filteredCompanies = this.formCompany.valueChanges.pipe(
        startWith(''),
        map((item: string) => item ? this._filterCompany(item, this.companies) : this.companies.slice()));
    }, error => { });
    /*Requirement*/
    this.categoryService.getAllCategoryByCategoryTypeCode(this.productService).subscribe(response => {
      let result = <any>response;
      this.requirements = result.category;
    }, error => { });
    /*Potential*/
    this.categoryService.getAllCategoryByCategoryTypeCode(this.potentialCode).subscribe(response => {
      let result = <any>response;
      this.potentials = result.category;
    }, error => { });
    /*Payment*/
    this.categoryService.getAllCategoryByCategoryTypeCode(this.paymentCode).subscribe(response => {
      let result = <any>response;
      this.payments = result.category;
    }, error => { });
    /*Employee*/
    //this.employeeService.getAllEmployee().subscribe(response => {
    //  let result = <any>response;
    //  this.employees = result.employeeList;

    //  //Set default value for Pic
    //  let defaultValue = this.employees.find(pic => pic.employeeId == this.currentEmployeeId);
    //  if (defaultValue) {
    //    this.picCtrl.setValue(defaultValue.employeeName);
    //  }

    //  this.filteredPic = this.picCtrl.valueChanges.pipe(
    //    startWith(null),
    //    map((item: string) => item ? this._filterEmployee(item, this.employees) : this.employees.slice()));
    //}, error => { });

    //this.employeeService.getEmployeeByPositionCode("GetIt").subscribe(response => {
    //  const result = <any>response;
    //  this.employees = result.employeeList
    //  let defaultValue = this.employees.find(pic => pic.employeeId == this.currentEmployeeId);
    //  if (defaultValue) {
    //    this.picCtrl.setValue(defaultValue.employeeName);
    //  }

    //  this.filteredPic = this.picCtrl.valueChanges.pipe(
    //      startWith(''),
    //    map(name => name ? this._filterEmployee(name, this.employees) : this.employees.slice())
    //    );
    //});

   this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
   this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;
   let resultEmployeeCharge: any = await this.employeeService.getEmployeeCareStaffAsyc(this.isManager, this.employeeId);
   this.employees = resultEmployeeCharge.employeeList.filter(e => e.active == true);
   let defaultValue = this.employees.find(pic => pic.employeeId == this.currentEmployeeId);
   if (defaultValue) {
     this.picCtrl.setValue(defaultValue.employeeName);
   }
   this.filteredPic = this.picCtrl.valueChanges.pipe(
     startWith(''),
     map(name => name ? this._filterEmployee(name, this.employees) : this.employees.slice())
   );

    /*Provinces*/
    this.provinceService.getAllProvince().subscribe(response => {
      let result = <any>response;
      this.provinces = result.listProvince;
      this.filteredProvinces = this.formCity.valueChanges.pipe(
        startWith(''),
        map((item: string) => item ? this._filterProvince(item, this.provinces) : this.provinces.slice()));
    }, error => { });
  }
  /*Kết thúc*/

  changePersonInCharge() {
    if (this.picCtrl.value.trim() == "") {
      this.selectedPic = [];
    } else {
      let employeesList: any = this.employees;
      let count = employeesList.filter(item => item.employeeName == this.picCtrl.value.trim()).length;
      if (count != 1) {
        this.selectedPic = [];
      } else {
        this.selectedPic = [];
        this.selectedPic.push(employeesList.find(item => item.employeeName == this.picCtrl.value.trim()).employeeId);
      }
    }
  }

  private _filterEmployee(value: any, array: any): string[] {
    const filterValue = value;
    let pesonId = array.find(c => c.employeeId == value);
    if (pesonId != undefined) {
      this.leadModel.PersonInChargeId = value;
    }
    return array.filter(item => item.employeeName.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1 || item.employeeCode.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1);
    //return array.filter(item => {if (typeof item != 'string') item.employeeId.indexOf(filterValue) === 0});
  }

  addPic(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.selectedPic.push(value.trim());
    }

    if (input) {
      input.value = '';
    }

    this.picCtrl.setValue(null);
  }

  selectedPicFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedPic = [];
    this.picCtrl.setValue(event.option.viewValue);
    this.selectedPic.push(event.option.value);
    //this.selectedPic.push(event.option.value);
    //this.picInput.nativeElement.value = '';
    //this.picCtrl.setValue(null);
    //this.employees.splice(this.employees.indexOf(event.option.value), 1);
  }

  /*Hien thi danh sach cac Quan/Huyen*/
  getAllDistrictByProvinceId(provinceId: string) {
    this.districtService.getAllDistrictByProvinceId(provinceId).subscribe(response => {
      let result = <any>response;
      this.districts = result.listDistrict;
      //let currentDistrictId: string = result.listDistrict[0].districtId;
      //this.getAllWardByDistrictId(currentDistrictId);
      this.filteredDistricts = this.formDistrict.valueChanges.pipe(
        startWith(''),
        map((item: string) => item ? this._filterDistrict(item, this.districts) : this.districts.slice()));
    }, error => { });
  }
  /*Ket thuc*/

  /*Lay ra danh sach cac Phuong/Xa*/
  getAllWardByDistrictId(value: string) {
    this.contactModel.DistrictId = value;
    this.wardService.getAllWardByDistrictId(value).subscribe(response => {
      let result = <any>response;
      this.wards = result.listWard;
      this.filteredWards = this.formGuild.valueChanges.pipe(
        startWith(''),
        map((item: string) => item ? this._filterWard(item, this.wards) : this.wards.slice()));
    }, error => { });
  }
  /*Ket thuc*/

  /*Kiem tra xem co tao Company khi tao Lead hay khong*/
  setIsCreateLeadAndResetField(value: boolean) {
    this.isCreateLeadAndResetField = value;
  }
  /*Ket thuc*/

  //Function tạo Lead
  @HostListener('submit', ['$event'])
  createLeadFunction(value) {
    if (!this.errEmail && !this.errPhone) {
      if (!this.createForm.valid) {
        Object.keys(this.createForm.controls).forEach(key => {
          if (this.createForm.controls[key].valid === false) {
            this.createForm.controls[key].markAsTouched();
          }
        });

        let target;
        target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
      }
      else {
        this.leadModel.CreatedDate = new Date();
        this.leadModel.CreatedById = this.auth.UserId;
        this.contactModel.CreatedDate = new Date();
        this.contactModel.CreatedById = this.auth.UserId;
        this.leadService.createLead(this.leadModel, this.contactModel, this.isCreateCompany, this.leadModel.CompanyName)
          .subscribe(response => {
            let result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              this.translate.get('toast.lead.create_success').subscribe(value => { this.toastMessage = value; });
              if (value) {
                this.resetFieldValue();
              } else {
                this.dialog.close();
                this.noteModel.Type = 'NEW';
                this.noteModel.Description = 'Mức độ tiềm năng - <b>' + result.potential + '</b>, trạng thái - <b>' + result.statusName + '</b>, chưa có người phụ trách';
                this.noteService.createNote(this.noteModel, result.leadId, null, this.auth.UserId).subscribe(response => {
                  var cResult = <any>response;
                  if (cResult.statusCode === 202 || cResult.statusCode === 200) {
                    var currentUrl = window.location.host + '/lead/detail?leadId=' + result.leadId + '&contactId=' + result.contactId;
                    this.emailService.sendEmailAfterCreatedLead(localStorage.getItem('UserFullName'), localStorage.getItem('UserEmail'), currentUrl, this.auth.UserId, result.leadId)
                      .subscribe(response => {
                      
                      }, error => { });
                  }
                });
              }
              this.snackBar.openFromComponent(SuccessComponent, { data: this.toastMessage, ...this.successConfig });
            } else {
              this.translate.get('toast.lead.create_fail').subscribe(value => { this.toastMessage = value; });
              this.snackBar.openFromComponent(FailComponent, { data: this.toastMessage, ...this.failConfig });
            }
          }, error => {
            this.translate.get('toast.lead.create_fail').subscribe(value => { this.toastMessage = value; });
            this.snackBar.openFromComponent(FailComponent, { data: this.toastMessage, ...this.failConfig });
          });
      }
    }
  }
  //Kết thúc

  //Function hủy tạo công ty
  btnCancelClick() {
    this.isCreateCompany = false;
  }
  //Kết thúc

  //Function tạo công ty
  btnCreateCompanyClick() {
    this.isCreateCompany = true;
  }
  //Kết thúc

  //Function reset toàn bộ các value đã nhập trên form
  resetFieldValue() {
    this.createForm.reset();
    this.contactModel.ProvinceId = '';
    this.contactModel.DistrictId = '';
    this.contactModel.WardId = '';
    this.leadModel.InterestedGroupId = '';
    this.leadModel.CompanyId = '';
    this.leadModel.PotentialId = '';
    this.leadModel.PaymentMethodId = '';
    this.contactModel.Gender = 'NAM';
  }
  //Kết thúc

  /*Autocomplete*/
  selectedCompany(event: MatAutocompleteSelectedEvent): void {
    this.formCompany.setValue(event.option.viewValue);
    this.leadModel.CompanyId = event.option.value;
  }
  private _filterCompany(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.companyName.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1);
  }

  selectedProvince(event: MatAutocompleteSelectedEvent): void {
    this.formCity.setValue(event.option.viewValue);
    this.formDistrict.setValue('');
    this.formGuild.setValue('');
    this.contactModel.ProvinceId = event.option.value;
    this.getAllDistrictByProvinceId(event.option.value);

    $('#formDistrict').focus();
  }
  private _filterProvince(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.provinceName.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1);
  }

  selectedDistrict(event: MatAutocompleteSelectedEvent): void {
    this.formDistrict.setValue(event.option.viewValue);
    this.formGuild.setValue('');
    $('#formGuild').focus();
    this.contactModel.DistrictId = event.option.value;
    this.getAllWardByDistrictId(event.option.value);
  }
  private _filterDistrict(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.districtName.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1);
  }

  selectedWard(event: MatAutocompleteSelectedEvent): void {
    this.formGuild.setValue(event.option.viewValue);
    this.contactModel.WardId = event.option.value;
  }
  private _filterWard(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.wardName.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1);
  }

  popupConfirm() {
    if (this.objectType == 'CUS') {
      let _title = "XÁC NHẬN";
      let _content = "Đã tồn tại khách hàng trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?";
      this.dialogPopup = this.dialogPop.open(PopupComponent,
        {
          width: '500px',
          height: '300px',
          autoFocus: false,
          data: { title: _title, content: _content }
        });

      this.dialogPopup.afterClosed().subscribe(result => {
        if (result) {
          this.loading = true;    
          this.dialog.close();
          this.router.navigate(['/customer/detail', { customerId: this.objectId, contactId: this.contactId }]);
          this.loading = false;
        }
      });
    }
  }

  async checkEmail() {
    if (this.contactModel.Email !== undefined && this.contactModel.Email !== "") {
      var result: any = await this.leadService.checkEmailLead(this.contactModel.Email);
      this.errEmail = result.checkEmail;
      this.objectType = result.objectType;
      this.contactId = result.contactId;
      this.objectId = result.objectId;
    }
    else {
      this.errEmail = false;
      this.objectType = "";
      this.contactId = "";
      this.objectId = ""
    }
  }

  async checkPhone() {
    if (this.contactModel.Phone !== undefined && this.contactModel.Phone !== "") {
      var result: any = await this.leadService.checkPhoneLead(this.contactModel.Phone);
      this.errPhone = result.checkPhone;
      this.objectType = result.objectType;
      this.contactId = result.contactId;
      this.objectId = result.objectId;
    }
    else {
      this.errPhone = false;
      this.objectType = "";
      this.contactId = "";
      this.objectId = "";
    }
  }
  /*Ket thuc*/

  /*Xóa dữ liệu ô Tỉnh*/
  clearDataProvince() {
    this.formCity.reset();
    this.clearDataDistrict();
    this.filteredDistricts = null;
    this.clearDataWard();
    this.contactModel.ProvinceId = null;
  }
  /*End*/

  /*Xóa dữ liệu ô Quận*/
  clearDataDistrict() {
    this.formDistrict.reset();
    this.filteredWards = null; 
    this.clearDataWard();
    this.contactModel.DistrictId = null;
  }
  /*End*/

  /*Xóa dữ liệu ô Xã*/
  clearDataWard() {
    this.formGuild.reset();  
    this.contactModel.WardId = null;   
  }
  /*End*/
  /*Xóa dữ liệu ô ng phụ trách*/
  clearEmployee() {
    this.picCtrl.reset();
    this.leadModel.PersonInChargeId = null;   
  }
  /*End*/

  /*Xóa dữ liệu ô công ty*/
  clearCompanyData() {
    this.formCompany.reset();
    this.leadModel.CompanyId = null;    
  }
  /*End*/
}

function checkCreateCompanyValidation(checkValue: boolean): ValidatorFn {
  return (): { [key: string]: boolean } => {
    if (checkValue) {
        return { 'checkCreateCompanyValidation': true };
    }
    return null;
  }
}
