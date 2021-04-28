import { Component, OnInit, ElementRef, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { OrganizationService } from "../../../shared/services/organization.service";
import { OrganizationModel } from "../../../shared/models/organization.model";
import { TranslateService } from '@ngx-translate/core';
import { FailComponent } from '../../../shared/toast/fail/fail.component';

import * as $ from 'jquery';

export interface IDialogData {
  selectedOrgId: string;
  selectedOrgName: string;
  orgCreated: boolean;
  orgCreatedId: string;
  orgEdited: boolean;
  mode: string;
  message: string;
}

@Component({
  selector: 'app-organization-dialog',
  templateUrl: './organization-dialog.component.html',
  styleUrls: ['./organization-dialog.component.css']
})

export class OrganizationDialogComponent implements OnInit {
  /*Get System Parameters */
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));

  organizationModel: OrganizationModel = {
    OrganizationId: '',
    OrganizationName: '',
    OrganizationCode: '',
    Address: '',
    Phone: '',
    ParentId: '',
    ParentName: '',
    Level: 1,
    IsFinancialIndependence: false,
    CreatedById: '',
    CreatedDate: null,
    UpdatedById: '',
    UpdatedDate: null,
    Active: true
  };

  orgForm: FormGroup;
  formOrgName: FormControl;
  formOrgCode: FormControl;
  formOrgPhone: FormControl;
  formOrgAddress: FormControl;
  formOrgFinancialIndependence: FormControl;
  orgCodeList: Array<any> = [];
  parent: string;

  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(private el: ElementRef,
    private organizationService: OrganizationService,
    private translate: TranslateService,
    public dialogRef: MatDialogRef<OrganizationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public snackBar: MatSnackBar) {
    translate.setDefaultLang('vi');
  }

  async ngOnInit() {
    await this.getAllOrgCode();
    this.formOrgName = new FormControl('', [Validators.required]);
    this.formOrgCode = new FormControl('', [Validators.required, checkDuplicateCode(this.orgCodeList, this.data.mode)]);
    //this.formOrgPhone = new FormControl('', [Validators.pattern("^([+]|0)+[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$")]);
    this.formOrgPhone = new FormControl('', [Validators.pattern(this.getPhonePattern())]);
    this.formOrgAddress = new FormControl('');
    this.formOrgFinancialIndependence = new FormControl('');

    this.orgForm = new FormGroup({
      formOrgName: this.formOrgName,
      formOrgCode: this.formOrgCode,
      formOrgPhone: this.formOrgPhone,
      formOrgAddress: this.formOrgAddress,
      formOrgFinancialIndependence: this.formOrgFinancialIndependence
    });

    if (this.data.mode === 'edit') {
      this.organizationService.getOrganizationById(this.data.selectedOrgId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.organizationModel = <OrganizationModel>({
            OrganizationId: result.organization.organizationId,
            OrganizationName: result.organization.organizationName,
            OrganizationCode: result.organization.organizationCode,
            Phone: result.organization.phone,
            Address: result.organization.address,
            ParentId: result.organization.parentId,
            ParentName: result.organization.parentName,
            IsFinancialIndependence: result.organization.isFinancialIndependence,
            Level: result.organization.level,
            UpdatedById: result.organization.updatedById,
            UpdatedDate: result.organization.updatedDate,
            Active: result.organization.active,
          });
        }
      }, error => { });
    }

    this.parent = this.data.selectedOrgName;
  }

  async getAllOrgCode() {
    var result: any = await this.organizationService.getAllOrganizationCode();
    this.orgCodeList = result.orgCodeList;
  }

  createOrganization() {
    if (!this.orgForm.valid) {
      Object.keys(this.orgForm.controls).forEach(key => {
        if (this.orgForm.controls[key].valid === false) {
          this.orgForm.controls[key].markAsTouched();
        }
      });

      let target;

      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      // this.organizationService.createOrganization(this.organizationModel.OrganizationName,
      //   this.organizationModel.OrganizationCode,
      //   this.organizationModel.Phone,
      //   this.organizationModel.Address,
      //   this.organizationModel.Level,
      //   this.data.selectedOrgId,
      //   this.organizationModel.IsFinancialIndependence).subscribe(response => {
      //     let result = <any>response;
      //     this.data.message = result.messageCode;
      //     if (result.statusCode === 202 || result.statusCode === 200) {
      //       this.data.orgCreated = true;
      //       this.data.orgCreatedId = result.createdOrgId;
      //       this.dialogRef.close(this.data);
      //     } else {
      //       this.data.orgCreated = false;
      //       this.dialogRef.close(this.data);
      //     }
      //   }, error => {
      //     let result = <any>error;
      //     this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      //   });
    }
  }

  editOrganization() {
    if (!this.orgForm.valid) {
      Object.keys(this.orgForm.controls).forEach(key => {
        if (this.orgForm.controls[key].valid === false) {
          this.orgForm.controls[key].markAsTouched();
        }
      });

      let target;

      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      this.organizationService.editOrganizationById(this.data.selectedOrgId,
        this.organizationModel.OrganizationName,
        this.organizationModel.OrganizationCode,
        this.organizationModel.Phone,
        this.organizationModel.Address,
        this.organizationModel.IsFinancialIndependence).subscribe(response => {
          let result = <any>response;
          this.data.message = result.messageCode;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.data.orgEdited = true;
            this.dialogRef.close(this.data);
          } else {
            //this.data.orgEdited = false;
            //this.dialogRef.close(this.data);
            this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          }
        }, error => {
          let result = <any>error;
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        });
    }
  }

  onSaveClick() {
    if (this.data.mode === 'create') {
      this.createOrganization();
    } else {
      this.editOrganization();
    }
  }
  onCancelClick() {
    this.dialogRef.close(this.data);
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }
}

function checkDuplicateCode(array: Array<any>, mode: string): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "" && mode === "create") {
      return { 'checkDuplicateCode': true };
    }
    return null;
  }
}
