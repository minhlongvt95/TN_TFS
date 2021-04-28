import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { CompanyService } from '../../../shared/services/company.service';
import { QuoteService } from '../../services/quote.service';

@Component({
  selector: 'app-send-mail-popup-quote',
  templateUrl: './send-mail-popup-quote.component.html',
  styleUrls: ['./send-mail-popup-quote.component.css']
})
export class SendEmailQuoteComponent implements AfterViewInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  base64Pdf: any;

  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  quoteId: string = '';
  customerCompany: string = '';
  customerId: string = '';
  quoteCode: string = '';
  quoteMoney: number = 0;
  isCreateBylead: boolean = false;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";

  /*Form send quick email*/
  formQuickEmail: FormGroup;

  emailControl: FormControl;
  titleControl: FormControl;
  contentControl: FormControl;
  /*End*/

  @ViewChild('autofocus', { static: true }) private autofocus: ElementRef;

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private companyService: CompanyService,
    private quoteService: QuoteService,
    private messageService: MessageService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.customerId = this.config.data.customerId;
    this.quoteId = this.config.data.quoteId;
    this.quoteMoney = this.config.data.quoteMoney;
    this.quoteCode = this.config.data.quoteCode;
    this.base64Pdf = this.config.data.base64;
    this.customerCompany = this.config.data.customerCompany;
    if (this.config.data.isCreateBylead) {
      this.isCreateBylead = true;
    }
  }

  ngOnInit() {
    this.emailControl = new FormControl(null, [Validators.required]);
    this.titleControl = new FormControl('', [Validators.required, Validators.maxLength(200), forbiddenSpaceText]);
    this.contentControl = new FormControl('', [Validators.required]);

    this.formQuickEmail = new FormGroup({
      emailControl: this.emailControl,
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });
    this.emailControl.setValue(this.sendTo);
    if (this.customerCompany !== null && this.customerCompany.trim() !== "") {
      this.titleControl.setValue(this.customerCompany + " - Bảng báo giá sản phẩm/dịch vụ");
    }
    else {
      this.titleControl.setValue("Bảng báo giá sản phẩm/dịch vụ");
    }
    this.companyService.getCompanyConfig().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        let strContent = `<p>Kính gửi quý khách hàng,</p><p><br></p><p>Đây là bản báo giá sản phẩm dịch vụ của chúng tôi ${this.quoteCode}, với số tiền ${formatNumber(this.quoteMoney)} VND, chi tiết xem file đính kèm.</p><p>Nếu quý khách hàng có bất kỳ thắc mắc nào xin hãy liên hệ lại với chúng tôi theo số điện thoại ${result.companyConfig.phone} .</p><p><br></p><p>Xin trân trọng cảm ơn!</p>`
        this.contentControl.setValue(strContent);
      }
    }, error => { });
  }

  ngAfterViewInit() {
    this.autofocus.nativeElement.focus();
  }

  trimspace() {
    var trim = this.emailControl.value.trim();
    this.emailControl.setValue(trim);
  }
  
  sendEmail() {
    if (!this.formQuickEmail.valid) {
      Object.keys(this.formQuickEmail.controls).forEach(key => {
        if (!this.formQuickEmail.controls[key].valid) {
          this.formQuickEmail.controls[key].markAsTouched();
        }
      });
    } else {
      var listEmail = [];
      var emailErr = "";
      var lstMail = this.emailControl.value.trim().split(";");
      lstMail.forEach(item => {
        if (item.trim() !== "") {
          let isPatterm = /([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+([" +"]?){2,64}/.test(item.trim());
          if (isPatterm) {
            listEmail.push(item.trim());
          }
          else {
            emailErr = emailErr === "" ? item.trim() : (emailErr + ", " + item.trim());
          }
        }
      })
      if (emailErr !== "") {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Email: " + emailErr + " sai định dạng." };
        this.showMessage(msg);
      }
      if (listEmail.length !== 0) {
        this.quoteService.sendEmailCustomerQuote(listEmail, this.titleControl.value.trim(), this.contentControl.value, this.quoteId, this.base64Pdf).subscribe(response => {
          let result = <any>response;
          if (result.statusCode == 200) {
            let msg = { key: 'popup', severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
            if (emailErr == "") {
              this.ref.close(result);
            }
          }
          else {
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Email sai định dạng." };
            this.showMessage(msg);
          }
        });
      }
    }
  }

  cancel() {
    this.ref.close();
  }

  replate_token(token: string) {
    let newContent = (this.contentControl.value != null ? this.contentControl.value : "") + token;
    this.contentControl.setValue(newContent);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    this.currentTextChange = event.textValue;
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
function formatNumber(num) {
  return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}