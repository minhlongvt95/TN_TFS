import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { BankService } from "../../services/bank.service";
import { BankModel } from "../../models/bank.model";

//PrimeNg
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

interface BankAccount {
  bankAccountId: string,
  objectId: string,
  objectType: string,
  bankName: string, //Tên ngân hàng
  accountNumber: string,  //Số tài khoản
  accountName: string,  //Chủ tài khoản
  branchName: string, //Chi nhánh
  bankDetail: string,  //Diễn giải
  createdById: string,
  createdDate: Date
}

interface ResultDialog {
  status: boolean,  //TRUE: Lưu thành công, FALSE: Lưu thất bại
  listBankAccount: Array<BankAccount>  //list câu hỏi và câu trả lời sau khi thêm/sửa
}

@Component({
  selector: 'app-bankpopup',
  templateUrl: './bankpopup.component.html',
  styleUrls: ['./bankpopup.component.css'],
  providers: [MessageService]
})
export class BankpopupComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  bankForm: FormGroup;

  bankNameControl: FormControl;
  accountNumberControl: FormControl;
  accountNameControl: FormControl;
  branchNameControl: FormControl;
  bankDetailControl: FormControl;

  bankAccount: BankAccount = {
    bankAccountId: null,
    objectId: '',
    objectType: '',
    accountNumber: '',
    bankName: '',
    bankDetail: '',
    branchName: '',
    accountName: '',
    createdById: this.auth.UserId,
    createdDate: new Date()
  }

  constructor(
    private translate: TranslateService,
    private bankService: BankService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService) {
    translate.setDefaultLang('vi');
    this.bankAccount = this.config.data.bankAccount;
  }

  ngOnInit() {
    this.bankNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(250)]);
    this.accountNumberControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(50)]);
    this.accountNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(250)]);
    this.branchNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(250)]);
    this.bankDetailControl = new FormControl(null, [Validators.maxLength(250)]);

    this.bankForm = new FormGroup({
      bankNameControl: this.bankNameControl,
      accountNumberControl: this.accountNumberControl,
      accountNameControl: this.accountNameControl,
      branchNameControl: this.branchNameControl,
      bankDetailControl: this.bankDetailControl
    });

    if (this.bankAccount.bankAccountId != null) {
      //Cập nhật
      this.bankNameControl.setValue(this.bankAccount.bankName);
      this.accountNumberControl.setValue(this.bankAccount.accountNumber);
      this.accountNameControl.setValue(this.bankAccount.accountName);
      this.branchNameControl.setValue(this.bankAccount.branchName);
      this.bankDetailControl.setValue(this.bankAccount.bankDetail);
    }
  }

  awaitResponse: boolean = false;
  save() {
    if (!this.bankForm.valid) {
      Object.keys(this.bankForm.controls).forEach(key => {
        if (this.bankForm.controls[key].valid === false) {
          this.bankForm.controls[key].markAsTouched();
        }
      });
    } else {
      if (this.bankNameControl.value) {
        this.bankAccount.bankName = this.bankNameControl.value.trim();
      } else {
        this.bankAccount.bankName = null;
      }

      if (this.accountNumberControl.value) {
        this.bankAccount.accountNumber = this.accountNumberControl.value.trim();
      } else {
        this.bankAccount.accountNumber = null;
      }

      if (this.accountNameControl.value) {
        this.bankAccount.accountName = this.accountNameControl.value.trim();
      } else {
        this.bankAccount.accountName = null;
      }

      if (this.branchNameControl.value) {
        this.bankAccount.branchName = this.branchNameControl.value.trim();
      } else {
        this.bankAccount.branchName = null;
      }

      if (this.bankDetailControl.value) {
        this.bankAccount.bankDetail = this.bankDetailControl.value.trim();
      } else {
        this.bankAccount.bankDetail = null;
      }

      this.loading = true;
      this.awaitResponse = true;
      this.bankService.createBank(this.bankAccount).subscribe(response => {
        this.loading = false;
        let result = <any>response;

        if (result.statusCode === 202 || result.statusCode === 200) {
          let resultDialog: ResultDialog = {
            status: true,
            listBankAccount: result.listBankAccount
          };

          this.ref.close(resultDialog);
        } else {
          let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  cancel() {
    this.ref.close();
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