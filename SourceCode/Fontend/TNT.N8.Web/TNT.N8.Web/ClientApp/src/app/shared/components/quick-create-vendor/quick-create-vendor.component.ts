import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { VendorService } from '../../services/vendor.service';
import { MessageService, ConfirmationService } from 'primeng/api';

class VendorModel {
  VendorId: string;
  VendorName: string;
  VendorCode: string;
  VendorGroupId: string;
  PaymentId: string;
  TotalPurchaseValue: number;
  TotalPayableValue: number;
  NearestDateTransaction: Date;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: boolean;

  constructor() {
    this.VendorId = '00000000-0000-0000-0000-000000000000';
    this.VendorName = '';
    this.VendorCode = '';
    this.VendorGroupId = null;
    this.PaymentId = '00000000-0000-0000-0000-000000000000';
    this.TotalPurchaseValue = 0;
    this.TotalPayableValue = 0;
    this.NearestDateTransaction = null;
    this.CreatedById = '00000000-0000-0000-0000-000000000000';
    this.CreatedDate = new Date();
    this.Active = true;
  }
}

@Component({
  selector: 'app-quick-create-vendor',
  templateUrl: './quick-create-vendor.component.html',
  styleUrls: ['./quick-create-vendor.component.css']
})
export class QuickCreateVendorComponent implements OnInit {
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listVendorGroup: Array<any> = [];

  quickCreVendorForm: FormGroup;
  vendorGroupControl: FormControl;
  vendorCodeControl: FormControl;
  vendorNameControl: FormControl;
  vendorPhoneControl: FormControl;
  vendorEmailControl: FormControl;
  vendorAddressControl: FormControl;

  awaitResponse: boolean = false;

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private vendorService: VendorService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.setForm();
    this.vendorService.quickCreateVendorMasterdata().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listVendorGroup = result.listVendorCategory;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    let vendorCodePattern = '[a-zA-Z0-9]+$';

    this.vendorGroupControl = new FormControl(null);
    this.vendorCodeControl = new FormControl(null, [Validators.required, Validators.pattern(vendorCodePattern), forbiddenSpaceText]);
    this.vendorNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.vendorEmailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.vendorAddressControl = new FormControl(null);
    this.vendorPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern())]);

    this.quickCreVendorForm = new FormGroup({
      vendorGroupControl: this.vendorGroupControl,
      vendorCodeControl: this.vendorCodeControl,
      vendorNameControl: this.vendorNameControl,
      vendorEmailControl: this.vendorEmailControl,
      vendorAddressControl: this.vendorAddressControl,
      vendorPhoneControl: this.vendorPhoneControl
    });
  }

  /* Tạo nhanh Nhà cung cấp */
  quickCreaVendor() {
    if (!this.quickCreVendorForm.valid) {
      Object.keys(this.quickCreVendorForm.controls).forEach(key => {
        if (this.quickCreVendorForm.controls[key].valid == false) {
          this.quickCreVendorForm.controls[key].markAsTouched();
        }
      });
    } else {
      let vendor = new VendorModel();
      vendor.VendorGroupId = this.vendorGroupControl.value ? this.vendorGroupControl.value?.categoryId : this.emptyGuid;
      vendor.VendorCode = this.vendorCodeControl.value.trim();
      vendor.VendorName = this.vendorNameControl.value.trim();

      let email = this.vendorEmailControl.value?.trim();
      let phone = this.vendorPhoneControl.value?.trim();
      let address = this.vendorAddressControl.value?.trim();

      this.awaitResponse = true;
      this.vendorService.quickCreateVendor(vendor, email, phone, address).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          let resultPopup = {
            newVendorId: result.vendorId,
            listVendor: result.listVendor
          }

          this.ref.close(resultPopup);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  cancelQuickCre() {
    this.ref.close();
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
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
