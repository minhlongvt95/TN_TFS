import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
/* end PrimeNg API */

import { LeadService } from '../../services/lead.service';

interface ResultDialog {
  status: boolean;
}

class personalInChangeModel {
  employeeId: string;
  employeeName: string;
  employeeCode: string;
  employeeCodeName: string;
}

@Component({
  selector: 'app-set-pic-dialog',
  templateUrl: './set-pic-dialog.component.html',
  styleUrls: ['./set-pic-dialog.component.css'],
  providers: [DialogService]
})
export class SetPicDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  //routing data
  listLeadId: Array<string> = [];
  customerId: string = null;
  //master data
  listPersonalInChange: Array<personalInChangeModel> = [];
  //form
  picForm: FormGroup;
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private leadService: LeadService,
  ) { 
    this.listLeadId = this.config.data.listLeadId;
    this.customerId = this.config.data.customerId;
  }

  async ngOnInit() {
    this.initForm();
    this.loading = true;
    let result: any = await this.leadService.setPersonalInChange(this.auth.UserId, this.customerId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listPersonalInChange = result.listPersonalInChange;
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.picForm = new FormGroup({
      'Pic': new FormControl(null, [Validators.required])
    });
  }

  cancel() {
    this.ref.close();
  }

  save() {
    if (!this.picForm.valid) {
      Object.keys(this.picForm.controls).forEach(key => {
        if (!this.picForm.controls[key].valid) {
          this.picForm.controls[key].markAsTouched();
        }
      });
    } else {
      //valid
      let employeeId = this.picForm.get('Pic').value.employeeId;

      this.loading = true;
      this.leadService.editPersonInCharge(this.listLeadId, employeeId, this.auth.UserId).subscribe(response => {
        this.loading = false;
        let result: any = response;
        if (result.statusCode == 200) {
          let result: ResultDialog = {
            status: true
          }
          this.ref.close(result);
        }
        else {
          let result: ResultDialog = {
            status: false
          }
          this.ref.close(result);
        }
      });
    }
  }

}
