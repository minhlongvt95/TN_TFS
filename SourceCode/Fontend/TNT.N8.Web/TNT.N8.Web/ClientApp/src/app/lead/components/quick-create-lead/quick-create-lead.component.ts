import { Component, OnInit, ElementRef, HostListener, Inject, ViewChild, } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';

import { LeadModel } from "../../models/lead.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { NoteModel } from '../../../shared/models/note.model';
import { SendEmailModel } from '../../../admin/models/sendEmail.model';

import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { NoteService } from '../../../shared/services/note.service';
import { TranslateService } from '@ngx-translate/core';
import { LeadService } from "../../services/lead.service";
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { GetPermission } from '../../../shared/permission/get-permission';

interface DialogResult {
  status: boolean
}

/* MODELS */
class companyModel {
  companyId: string;
  companyName: string;
}

class customerContactModel {
  customerId: string;
  customerFullName: string;
  email: string;
  phone: string;
}

class provinceModel {
  provinceId: string;
  provinceName: string;
  provinceCode: string;
  provinceType: string;
}

class districtModel {
  districtId: string;
  districtName: string;
  districtType: string;
  districtCode: string;
  provinceId: string;
}

class wardModel {
  wardId: string;
  wardName: string;
  wardCode: string;
  districtId: string;
  wardType: string;
}

class genderModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class interestedGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class leadTypeModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class paymentMethodModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class personalInChangeModel {
  employeeId: string;
  employeeName: string;
  employeeCode: string;
}

class potentialModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class leadGroup {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}


@Component({
  selector: 'app-quick-create-lead',
  templateUrl: './quick-create-lead.component.html',
  styleUrls: ['./quick-create-lead.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class QuickCreateLeadComponent implements OnInit {
  //system parameter
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth = JSON.parse(localStorage.getItem("auth"));
  currentEmployeeId = this.auth.EmployeeId;
  awaitResult: boolean = false;
  //list action in page
  actionAdd: boolean = true;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  //master data
  listCompany: Array<companyModel> = [];
  listCustomerContact: Array<customerContactModel> = [];
  listDistrict: Array<districtModel> = [];
  listCurrentDistrict: Array<districtModel> = [];
  listEmailLead: Array<string> = [];
  listGender: Array<genderModel> = [];
  listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<leadTypeModel> = [];
  listLeadGroup: Array<leadGroup> = [];
  listPaymentMethod: Array<paymentMethodModel> = [];
  listPersonalInChange: Array<personalInChangeModel> = [];
  listPhoneLead: Array<string> = [];
  listPotential: Array<potentialModel> = [];
  listProvince: Array<provinceModel> = [];
  listWard: Array<wardModel> = [];
  listCurrentWard: Array<wardModel> = [];
  //form
  createLeadForm: FormGroup;
  //create company
  isCreateCompany: boolean = false;
  /* create valiable add class scroll */
  fixed: boolean = false;
  isKCL: boolean = false;
  constructor(
    private ref: DynamicDialogRef,
    private translate: TranslateService,
    private leadService: LeadService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private noteService: NoteService,
    public builder: FormBuilder,
    private el: ElementRef,
    private emailConfigService: EmailConfigService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
  ) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    //Check permission
    let resource = "crm/lead/create-lead";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.initForm();
      this.getMasterData();
    }
  }

  initForm() {
    this.createLeadForm = new FormGroup({
      'FirstName': new FormControl('', [Validators.required, checkBlankString()]),
      'LastName': new FormControl('', [Validators.required, checkBlankString()]),
      'Gender': new FormControl(null, [Validators.required]),
      'Potential': new FormControl(null, [Validators.required]),
      'Pic': new FormControl(null, []),
      'LeadType': new FormControl(null, [Validators.required]),
      'Interested': new FormControl(null, [Validators.required]),
      'DetailInterested': new FormControl(''),
      'PaymentMethod': new FormControl(null, [Validators.required]),
      'Group': new FormControl(null),

      'Email': new FormControl('', [Validators.pattern(this.emailPattern), checkDuplicateEmailLead(this.listEmailLead)]),
      'Phone': new FormControl('', [Validators.required, checkBlankString(), Validators.pattern(this.getPhonePattern()), checkDuplicatePhonelLead(this.listPhoneLead)]),
      'Company': new FormControl(null),
      'CreateNewCompany': new FormControl(''),
      'Province': new FormControl(null, [Validators.required]),
      'District': new FormControl(null, [Validators.required]),
      'Ward': new FormControl(null, [Validators.required]),
      'DetailAddress': new FormControl('', [Validators.required, checkBlankString()])
    });
  }

  resetForm() {
    this.createLeadForm.reset();
    this.createLeadForm.patchValue({
      'FirstName': '',
      'LastName': '',
      'Gender': null,
      'Potential': null,
      'Pic': null,
      'LeadType': null,
      'Interested': null,
      'DetailInterested': '',
      'PaymentMethod': null,
      'Group': null,

      'Email': '',
      'Phone': '',
      'Company': null,
      'CreateNewCompany': '',
      'Province': null,
      'District': null,
      'Ward': null,
      'DetailAddress': ''
    });
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  /*Function lấy ra các master data*/
  async getMasterData() {
    this.loading = true;
    let result: any = await this.leadService.getDataCreateLead(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listCompany = result.listCompany;
      this.listCustomerContact = result.listCustomerContact;
      this.listDistrict = result.listDistrict;
      this.listEmailLead = result.listEmailLead;
      this.listGender = result.listGender;
      this.listInterestedGroup = result.listInterestedGroup;
      this.listLeadType = result.listLeadType;
      this.listLeadGroup = result.listLeadGroup;
      this.listPaymentMethod = result.listPaymentMethod;
      this.listPersonalInChange = result.listPersonalInChange;
      this.listPhoneLead = result.listPhoneLead;
      this.listPotential = result.listPotential;
      this.listProvince = result.listProvince;
      this.listWard = result.listWard;
      //update validation
      this.createLeadForm.get('Email').setValidators([Validators.pattern(this.emailPattern), checkDuplicateEmailLead(this.listEmailLead)]);
      this.createLeadForm.get('Phone').setValidators([Validators.required, checkBlankString(), Validators.pattern(this.getPhonePattern()), checkDuplicatePhonelLead(this.listPhoneLead)]);
      this.createLeadForm.updateValueAndValidity();
      //set default value after get master data
      this.setDefaultValue();
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  setDefaultValue() {
    this.createLeadForm.get('Gender').setValue(this.listGender[0]);
  }

  checkDuplicateCustomer(type: string, formControl: FormControl | AbstractControl): boolean {
    if (formControl.invalid) return false;
    let isDuplicate: boolean = false;
    let value = formControl.value;
    switch (type) {
      case 'email':
        let duplicateEmailCustomer: customerContactModel = this.listCustomerContact.filter(e => e.email != null && e.email != "").find(e => e.email.trim() == value);
        if (duplicateEmailCustomer) {
          isDuplicate = true;
          this.confirmationService.confirm({
            message: `Đã tồn tại khách hàng có email này trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
            accept: () => {
              this.router.navigate(['/customer/detail', { customerId: duplicateEmailCustomer.customerId, contactId: this.emptyGuid }]);
            }
          });
        }
        break;
      case 'phone':
        let duplicatePhoneCustomer: customerContactModel = this.listCustomerContact.filter(e => e.phone != null).find(e => e.phone.trim() == value);
        if (duplicatePhoneCustomer) {
          isDuplicate = true;
          this.confirmationService.confirm({
            message: `Đã tồn tại khách hàng có số điện thoại này trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng này không?`,
            accept: () => {
              this.router.navigate(['/customer/detail', { customerId: duplicatePhoneCustomer.customerId, contactId: this.emptyGuid }]);
            }
          });
        }
        break;
      default:
        break;
    }
    return isDuplicate;
  }

  changeProvince(event: any): boolean {
    //reset list district and ward
    this.listCurrentDistrict = [];
    this.listCurrentWard = [];
    if (event.value === null) return false;
    let currentProvince: provinceModel = event.value;
    this.listCurrentDistrict = this.listDistrict.filter(e => e.provinceId === currentProvince.provinceId);
    return false;
  }

  changeDistrict(event: any): boolean {
    //reset ward
    this.listCurrentWard = [];
    if (event.value === null) return false;
    let currentDistrict: districtModel = event.value;
    this.listCurrentWard = this.listWard.filter(e => e.districtId === currentDistrict.districtId);
    return false
  }

  toggleCreateCompany(value) {
    //value = true: tao cong ty
    //value = false: huy tao cong ty
    switch (value) {
      case true:
        this.isCreateCompany = true;
        break;
      case false:
        this.isCreateCompany = false;
        break;
      default:
        break;
    }
  }

  mapFormToLeadModel(): LeadModel {
    let lead = new LeadModel();
    lead.LeadId = this.emptyGuid;
    lead.RequirementDetail = this.createLeadForm.get('DetailInterested').value;
    lead.PotentialId = this.createLeadForm.get('Potential').value.categoryId;
    lead.InterestedGroupId = this.createLeadForm.get('Interested').value.categoryId;
    lead.PersonInChargeId = this.createLeadForm.get('Pic').value !== null ? this.createLeadForm.get('Pic').value.employeeId : null;
    lead.CompanyId = this.createLeadForm.get('Company').value !== null ? this.createLeadForm.get('Company').value.companyId : null;
    lead.StatusId = this.emptyGuid;
    lead.PaymentMethodId = this.createLeadForm.get('PaymentMethod').value.categoryId;
    lead.LeadTypeId = this.createLeadForm.get('LeadType').value.categoryId;
    lead.LeadGroupId = this.createLeadForm.get('Group').value !== null ? this.createLeadForm.get('Group').value.categoryId : null;
    lead.CreatedById = this.auth.UserId;
    lead.CreatedDate = new Date();
    lead.UpdatedById = null;
    lead.UpdatedDate = new Date();
    lead.Active = true;
    lead.Role = null;
    lead.WaitingForApproval = false;
    return lead;
  }

  mapFormToLeadContactModel(): ContactModel {
    let contact = new ContactModel();
    contact.ContactId = this.emptyGuid;
    contact.ObjectId = this.emptyGuid;
    contact.ObjectType = "LEA";
    contact.FirstName = this.createLeadForm.get('FirstName').value;
    contact.LastName = this.isKCL ? "" : this.createLeadForm.get('LastName').value;
    contact.Gender = this.createLeadForm.get('Gender').value.categoryCode;
    contact.DateOfBirth = null;
    contact.Phone = this.createLeadForm.get('Phone').value;
    contact.WorkPhone = null;
    contact.OtherPhone = null;
    contact.Email = this.createLeadForm.get('Email').value;
    contact.WorkEmail = null;
    contact.OtherEmail = null;
    contact.IdentityID = null;
    contact.AvatarUrl = null;
    contact.Address = this.createLeadForm.get('DetailAddress').value;
    contact.ProvinceId = this.createLeadForm.get('Province').value.provinceId;
    contact.DistrictId = this.createLeadForm.get('District').value.districtId;
    contact.WardId = this.createLeadForm.get('Ward').value.wardId;
    contact.CreatedById = this.auth.UserId;
    contact.CreatedDate = new Date();
    contact.UpdatedById = null;
    contact.UpdatedDate = new Date();
    contact.Active = true;
    return contact;
  }

  cancel() {
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn hủy? Các dữ liệu sẽ không được lưu`,
      accept: () => {
        this.ref.close();
      }
    });
  }

  async CreateLead(type: boolean) {
    //type = true: luu va tao moi
    //type = false: luu
    if (!this.createLeadForm.valid) {
      Object.keys(this.createLeadForm.controls).forEach(key => {
        if (!this.createLeadForm.controls[key].valid) {
          this.createLeadForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      //check duplicate customer on save
      let _duplicateEmailCus = this.checkDuplicateCustomer('email', this.createLeadForm.get('Email'));
      let _duplicatePhoneCus = this.checkDuplicateCustomer('phone', this.createLeadForm.get('Phone'));

      if (_duplicateEmailCus) {
        //border o email
        this.createLeadForm.controls['Email'].markAsTouched();
        //
      } else if (_duplicatePhoneCus) {
        //border o so dien thoai
        this.createLeadForm.controls['Phone'].markAsTouched();
      } else {
        //valid
        let leadModel: LeadModel = this.mapFormToLeadModel();
        let leadContactModel: ContactModel = this.mapFormToLeadContactModel();
        let newCompany = this.createLeadForm.get('CreateNewCompany').value;
        this.loading = true;
        let result: any = await this.leadService.createLeadAsync(leadModel, leadContactModel, false, '' , [], [], []);
        if (result.statusCode === 200) {
          //send email
          let sendMailModel: SendEmailModel = result.sendEmailEntityModel;
          this.emailConfigService.sendEmail(1, sendMailModel).subscribe(reponse => {
            let _emailresult = <any>reponse;
            if (_emailresult.statusCode !== 200) {
              this.clearToast();
              // this.showToast('error', 'Thông báo', _emailresult.messageCode);
              this.showToast('error', 'Thông báo', "Gửi email thất bại");
            }
          });
          //create new note
          let note: NoteModel = new NoteModel();
          note.Type = 'NEW';
          note.Description = 'Mức độ tiềm năng - <b>' + result.potential + '</b>, trạng thái - <b>' + result.statusName + '</b>, chưa có người phụ trách';
          this.noteService.createNote(note, result.leadId, null, this.auth.UserId).subscribe(response => {
            var _noteresult = <any>response;
            if (_noteresult.statusCode !== 200) {
              this.clearToast();
              this.showToast('error', 'Thông báo', _noteresult.messageCode);
            }
          }, error => { });

          if (type === false) {
            let result: DialogResult  = {
              status: true
            }
            this.ref.close(result);
          } else {
            this.resetForm();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Tạo khách hàng tiềm năng thành công');
          }
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
        this.loading = false;
      }
    }
  }
  changeTypeLead(formControl: FormControl | AbstractControl){
    // if(formControl.value.categoryCode == "KCL"){
    //   this.isKCL = true;
    //   this.createLeadForm.patchValue({
    //     'LastName': 'a'
    //   });
    // }
    // else{
    //   this.isKCL = false;
    //   this.createLeadForm.patchValue({
    //     'LastName': ''
    //   });
    // }
    let lastName = this.createLeadForm.get('LastName').value
    if(formControl.value.categoryCode == "KCL"){
      this.isKCL = true;
      this.createLeadForm.patchValue({
        'LastName': (lastName == null || lastName == undefined || lastName == "") ? "-" : lastName
      });
    }
    else{
      this.isKCL = false;
      this.createLeadForm.patchValue({
        'LastName': lastName == "-" ? null : lastName
      });
    }
  }
}

function checkCreateCompanyValidation(checkValue: boolean): ValidatorFn {
  return (): { [key: string]: boolean } => {
    if (checkValue) {
      return { 'checkCreateCompanyValidation': true };
    }
    return null;
  }
}

function checkBlankString(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() === "") {
        return { 'blankString': true };
      }
    }
    return null;
  }
}

function checkDuplicateEmailLead(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateEmailLead = array.find(e => e === control.value.trim());
        if (duplicateEmailLead !== undefined) {
          return { 'duplicateEmailLead': true };
        }
      }
    }
    return null;
  }
}

function checkDuplicatePhonelLead(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicatePhoneLead = array.find(e => e === control.value.trim());
        if (duplicatePhoneLead !== undefined) {
          return { 'duplicatePhoneLead': true };
        }
      }
    }
    return null;
  }
}
