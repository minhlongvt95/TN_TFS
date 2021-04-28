import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

import { ContactModel, contactModel } from "../../../shared/models/contact.model";

class leadContactConfigDataModel {
  isEdit: boolean; //tạo mới: false; chỉnh sửa: true
  contact: contactModel
}

class genderModel {
  code: string;
  name: string;
}

class resultDialog {
  status: boolean;
  contact: contactModel;
}

@Component({
  selector: 'app-creat-contact-lead-dialog',
  templateUrl: './creat-contact-lead-dialog.component.html',
  styleUrls: ['./creat-contact-lead-dialog.component.css']
})
export class CreatContactLeadDialogComponent implements OnInit {
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  listGenders: Array<genderModel> = [{ code: 'NAM', name: 'Nam' }, { code: 'NU', name: 'Nữ' }];
  maxEndDate: Date = new Date();
  //form
  contactForm: FormGroup;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.initForm();
    if (this.config.data) {
      let configData: leadContactConfigDataModel = this.config.data.leadContactConfigData;
      if (configData.isEdit === true) {
        this.patchDataToForm(configData.contact);
      }
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  initForm() {
    this.contactForm = new FormGroup({
      'ContactName': new FormControl(null, [Validators.required, forbiddenSpaceText]),
      'Gender': new FormControl(null),
      'DateOfBirth': new FormControl(null),
      'Position': new FormControl(null),
      'RelationShip': new FormControl(null),
      'Phone': new FormControl(null, [Validators.required, Validators.pattern(this.getPhonePattern())]),
      'Email': new FormControl(null, [Validators.pattern(this.emailPattern)]),
      'Note': new FormControl(null),
    });
  }

  cancel() {
    this.ref.close();
  }

  patchDataToForm(contact: contactModel) {
    let dateOfBirth = contact.dateOfBirth ? new Date(contact.dateOfBirth) : null;
    let _gender = this.listGenders.find(e => e.code == contact.gender);
    this.contactForm.patchValue({
      'ContactName': contact.firstName,
      'Gender': _gender ? _gender : null,
      'DateOfBirth': dateOfBirth,
      'Position': contact.role,
      'RelationShip': contact.relationShip,
      'Phone': contact.phone,
      'Email': contact.email,
      'Note': contact.note
    });
  }

  save() {
    if (!this.contactForm.valid) {
      Object.keys(this.contactForm.controls).forEach(key => {
        if (!this.contactForm.controls[key].valid) {
          this.contactForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    let contact: contactModel = this.mapFormToModel();
    let result: resultDialog = new resultDialog();
    result.status = true;
    result.contact = contact;
    this.ref.close(result);
  }

  mapFormToModel(): contactModel {
    let _contactName = this.contactForm.get('ContactName').value;
    let _gender: genderModel = this.contactForm.get('Gender').value;
    let _dateOfBirth: Date = this.contactForm.get('DateOfBirth').value;
    let _position =  this.contactForm.get('Position').value;
    let _relationShip =  this.contactForm.get('RelationShip').value;
    let _phone = this.contactForm.get('Phone').value;
    let _email = this.contactForm.get('Email').value;
    let _note = this.contactForm.get('Note').value;

    let contact = new contactModel();
    contact.contactId = this.emptyGuid;
    contact.objectId = this.emptyGuid;
    contact.objectType = "LEA_CON";
    contact.firstName = _contactName ? _contactName.trim() : '';
    contact.lastName = '';
    contact.gender = _gender ? _gender.code : null;
    contact.genderDisplay = _gender ? _gender.name : '';
    contact.dateOfBirth = _dateOfBirth ? convertToUTCTime(_dateOfBirth) : null;
    contact.phone = _phone ? _phone.trim() : '';
    contact.email = _email ? _email.trim() : '';
    contact.role = _position ? _position.trim() : '';
    contact.relationShip = _relationShip ? _relationShip.trim() : '';
    contact.note = _note ? _note.trim() : '';

    contact.workPhone = null;
    contact.otherPhone = null;
    contact.workEmail = null;
    contact.otherEmail = null;

    contact.createdById = this.auth.UserId;
    contact.createdDate = new Date();
    contact.updatedById = null;
    contact.updatedDate = null;
    contact.active = true;

    return contact;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

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
