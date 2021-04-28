import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';

import { CustomerService } from '../../services/customer.service';

import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

export interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  message: string,
  messageCode: string
}

@Component({
  selector: 'app-customer-download-template',
  templateUrl: './customer-download-template.component.html',
  styleUrls: ['./customer-download-template.component.css']
})
export class CustomerDownloadTemplateComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  downloadTemplateForm: FormGroup;

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private customerService: CustomerService,
  ) { }

  ngOnInit() {
    this.downloadTemplateForm = new FormGroup({
      "typeControl": new FormControl("1"),
    });
  }

  onSaveClick() {
    let resultDialog: ResultDialog = {
      status: true,  //Gửi
      message: '',
      messageCode: ''
    };
    let customerType = Number(this.downloadTemplateForm.get('typeControl').value);
    this.loading = true;
    this.customerService.downloadTemplateCustomer( customerType, this.auth.UserId).subscribe(response => {
      this.loading = false;
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.excelFile );
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.nameFile  + ".xlsm";
        //const fileName = result.nameFile  + ".xlsx";
        link.download = fileName;
        link.click();
       resultDialog.message = "success";
       resultDialog.messageCode = "Download thành công";
       this.ref.close(resultDialog);
      } else {
       resultDialog.message = "error";
       resultDialog.messageCode = result.messageCode;
       this.ref.close(resultDialog);
      }
    }, error => {  this.loading = false;});
  }

  onCancelClick() {
    let result: ResultDialog = {
      status: false,  //Hủy
      message: '',
      messageCode: ''
    };
    this.ref.close(result);
  }

}
