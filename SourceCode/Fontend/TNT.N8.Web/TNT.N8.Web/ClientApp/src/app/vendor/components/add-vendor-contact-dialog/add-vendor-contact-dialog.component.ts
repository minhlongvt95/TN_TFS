import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { ContactModel } from "../../../shared/models/contact.model";

/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/* end PrimeNg API */

interface ConfigData {
  isEdit: boolean;
  contact: contactVendorDialogModel
}

interface DialogResult {
  status: boolean;
  contactModel: contactVendorDialogModel
}

class gender {
  code: string;
  name: string;
}

class contactVendorDialogModel {
  FullName: string;
  GenderDisplay: string;
  GenderName: string;
  Role: string;
  Phone: string;
  Email: string;
}

@Component({
  selector: 'app-add-vendor-contact-dialog',
  templateUrl: './add-vendor-contact-dialog.component.html',
  styleUrls: ['./add-vendor-contact-dialog.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class AddVendorContactDialogComponent implements OnInit {
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  loading: boolean = false;
  listGender: Array<gender> = [{ code: 'NAM', name: 'Nam' }, { code: 'NU', name: 'Nữ' }];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  //form
  createVendorContactForm: FormGroup;
  constructor(
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
  ) { }

  ngOnInit() {
    this.initForm();
    if (this.config.data) {
      let configData: ConfigData = this.config.data;
      if (configData.isEdit === true) this.patchDataToForm(configData.contact);
    }
  }

  initForm() {
    this.createVendorContactForm = new FormGroup({
      'FullName': new FormControl('', [Validators.required, checkBlankString()]),
      'Gender': new FormControl(null, [Validators.required]),
      'Role': new FormControl('', [Validators.required, checkBlankString()]),
      'Phone': new FormControl('', [Validators.required,Validators.pattern(this.getPhonePattern())]),
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)])
    });
    this.createVendorContactForm.get('Gender').setValue(this.listGender[0]);
  }

  cancel() {
    this.confirmationService.confirm({
      message: 'Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
      accept: () => {
        this.ref.close();
      }
    });
  }

  patchDataToForm(contact: contactVendorDialogModel) {
    this.createVendorContactForm.get('FullName').setValue(contact.FullName);
 
    let _gender = this.listGender.find(e => e.code == contact.GenderDisplay);
    this.createVendorContactForm.get('Gender').setValue(_gender ? _gender : null);

    this.createVendorContactForm.get('Role').setValue(contact.Role);
    this.createVendorContactForm.get('Phone').setValue(contact.Phone);
    this.createVendorContactForm.get('Email').setValue(contact.Email);
  }

  save() {
    if (!this.createVendorContactForm.valid) {
      Object.keys(this.createVendorContactForm.controls).forEach(key => {
        if (!this.createVendorContactForm.controls[key].valid) {
          this.createVendorContactForm.controls[key].markAsTouched();
        }
      });
    } else {
      //valid
      let contactModel: contactVendorDialogModel = this.mapFormToContactModel();
      let result: DialogResult = {
        status: true,
        contactModel: contactModel
      }
      this.ref.close(result);
    }
  }

  mapFormToContactModel(): contactVendorDialogModel {
    let contactModel: contactVendorDialogModel = new contactVendorDialogModel();
    contactModel.FullName = this.createVendorContactForm.get('FullName').value;
    contactModel.GenderDisplay = this.createVendorContactForm.get('Gender').value.code;
    contactModel.GenderName = this.createVendorContactForm.get('Gender').value.name;
    contactModel.Role = this.createVendorContactForm.get('Role').value;
    contactModel.Phone = this.createVendorContactForm.get('Phone').value;
    contactModel.Email = this.createVendorContactForm.get('Email').value;
    return contactModel;
  }
  

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
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

