import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { CompanyService } from '../../../shared/services/company.service';
import { VendorService } from '../../services/vendor.service';
import { getuid } from 'process';

@Component({
  selector: 'app-send-mail-vendor-quote-dialog',
  templateUrl: './send-mail-vendor-quote-dialog.component.html',
  styleUrls: ['./send-mail-vendor-quote-dialog.component.css']
})
export class SendMailVendorQuoteDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  base64Pdf: any;
  emptyId = "00000000-0000-0000-0000-000000000000";
  
  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  vendorId: string = this.emptyId;
  vendorCode: string = '';
  vendorName: string = '';
  suggestedSupplierQuoteId: string = this.emptyId;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";
  
  /*Form send email*/
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
    private vendorService: VendorService,
    private messageService: MessageService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.vendorId = this.config.data.vendorId;
    this.vendorCode = this.config.data.vendorCode;
    this.vendorName = this.config.data.vendorName;
    this.base64Pdf = this.config.data.base64;
    this.suggestedSupplierQuoteId = this.config.data.suggestedSupplierQuoteId;
   }
  

  ngOnInit() {

    this.setFrom();
    this.emailControl.setValue(this.sendTo);
    if(this.vendorName !== null && this.vendorName.trim() !== ""){
      this.titleControl.setValue( '[' + this.vendorName + ']' + " - Đề nghị báo giá");
    }
    else{
      this.titleControl.setValue("Đề nghị báo giá");
    }
    this.companyService.getCompanyConfig().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {     
        let strContent = `<p>Kính gửi quý khách hàng,</p><p><br></p><p>Qua tìm hiểu, công ty chúng tôi biết Quý công ty đang cung cấp một số sản phẩm, dịch vụ mà 
        chúng tôi đang cần. Vì vậy, nhờ quý công ty báo giá cho chúng tôi về giá của một số sản phẩm như file đính kèm.</p><p><br></p><p>Xin trân trọng cảm ơn!</p>`
        this.contentControl.setValue(strContent);
      }
    }, error => { });
  }

  sendEmail() {
    if (!this.formQuickEmail.valid) { 
      Object.keys(this.formQuickEmail.controls).forEach(key => {
        if (!this.formQuickEmail.controls[key].valid ) {
          this.formQuickEmail.controls[key].markAsTouched();
        }
      });
    } else {
        var listEmail = [];
        var emailErr = "";
        var lstMail = this.emailControl.value.trim().split(";");
        lstMail.forEach(item => {
          if(item.trim() !== ""){
            let isPatterm = /([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+([" +"]?){2,64}/.test(item.trim());
            if(isPatterm){
              listEmail.push(item.trim());
            }
            else{
              emailErr = emailErr === "" ? item.trim() : (emailErr + ", " + item.trim());
            }
          }
        })
        if(emailErr !== ""){
          let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: "Email: " + emailErr + " sai định dạng."};
          this.showMessage(msg);
        }
        if(listEmail.length !== 0){
          this.vendorService.sendEmailVendorQuote(listEmail, this.titleControl.value.trim(), this.contentControl.value, this.suggestedSupplierQuoteId, this.base64Pdf).subscribe(response => {
            let result = <any>response;
              if (result.statusCode == 200) {
                let msg = {key: 'popup', severity:'success', summary: 'Thông báo:', detail: result.messageCode};
                this.showMessage(msg);
                if(emailErr == ""){
                  this.ref.close(result);
                }
              }
              else{
                let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: "Email sai định dạng."};
                this.showMessage(msg);
              }
          });
        }
    }
  }

  setFrom(){
    this.emailControl = new FormControl(null, [Validators.required]);
    this.titleControl = new FormControl('', [Validators.required, Validators.maxLength(200), forbiddenSpaceText]);
    this.contentControl = new FormControl('', [Validators.required]);

    this.formQuickEmail = new FormGroup({
      emailControl: this.emailControl,
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });
  }

  ngAfterViewInit() {
    this.autofocus.nativeElement.focus();
  }
  trimspace(){
    var trim = this.emailControl.value.trim();
    this.emailControl.setValue(trim);
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
    let htmlValue = event.htmlValue;
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