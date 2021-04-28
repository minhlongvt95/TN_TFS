import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { CustomerCareService } from '../../services/customer-care.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

//COMPONENT
// import { TemplateEmailDialogComponent } from '../template-email-dialog/template-email-dialog.component';
// import { TemplateSmsDialogComponent } from '../template-sms-dialog/template-sms-dialog.component';

interface ResultDialog {
  status: boolean
}

@Component({
  selector: 'app-template-quick-email',
  templateUrl: './template-quick-email.component.html',
  styleUrls: ['./template-quick-email.component.css']
})
export class TemplateQuickEmailComponent implements AfterViewInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  fromTo: string = localStorage.getItem('UserEmail');
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  disableEmail: boolean = true;

  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  customerId: string = '';
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

  //dialogSelectTemplateEmail: MatDialogRef<TemplateEmailDialogComponent>;
  //dialogSelectTemplateSMS: MatDialogRef<TemplateSmsDialogComponent>;

  constructor(
    private ref: DynamicDialogRef, 
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerCareService: CustomerCareService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.disableEmail = this.config.data.disableEmail;
    this.customerId = this.config.data.customerId;
    if (this.config.data.isCreateBylead) {
      this.isCreateBylead = true;
    }
  }

  ngOnInit() {
    this.emailControl = new FormControl({value: this.sendTo, disabled: this.disableEmail}, [Validators.required, Validators.email]);
    this.titleControl = new FormControl('', [Validators.required, Validators.maxLength(200), forbiddenSpaceText]);
    this.contentControl = new FormControl('', [Validators.required]);

    this.formQuickEmail = new FormGroup({
      emailControl: this.emailControl,
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });
  }

  ngAfterViewInit() {
    //this.autofocus.nativeElement.focus();
  }

  sendEmail() {
    if (!this.formQuickEmail.valid) { 
      Object.keys(this.formQuickEmail.controls).forEach(key => {
        if (!this.formQuickEmail.controls[key].valid ) {
          this.formQuickEmail.controls[key].markAsTouched();
        }
      });
    } else if (this.currentTextChange.trim() == "") {
      let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: "Nội dung email không được để trống"};
      this.showMessage(msg);
    } else {
      let sendTo = this.emailControl.value == null ? "" : this.emailControl.value.trim();
      if (!sendTo) {
        let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: "Chưa có email nhận. Đề nghị bổ sung trước khi gửi"};
        this.showMessage(msg);
      } else if (sendTo.trim() == "") {
        let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: "Chưa có email nhận. Đề nghị bổ sung trước khi gửi"};
        this.showMessage(msg);
      } else {
        this.queueModel.FromTo = this.fromTo != null ? this.fromTo.trim() : "";
        this.queueModel.SendTo = sendTo;
        this.queueModel.CustomerId = this.customerId;
        this.queueModel.Title = this.titleControl.value.trim();
        this.queueModel.SendContent = this.contentControl.value;
        this.queueModel.Method = "Email";
        this.queueModel.CreateById = this.userId;

        this.awaitResult = true;

        // if (this.isCreateBylead == true) {
        //   this.customerCareService.sendQuickLeadEmail(this.queueModel).subscribe(response => {
        //     let result = <any>response;
        //     if (result.statusCode == 200) {
        //       let resultDialog: ResultDialog = {
        //         status: true
        //       };
  
        //       this.ref.close(resultDialog);
        //     } else {
        //       this.awaitResult = false;
        //       let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
        //       this.showMessage(msg);
        //     }
        //   });
        // } else {
          this.customerCareService.sendQuickEmail(this.queueModel).subscribe(response => {
            let result = <any>response;
            if (result.statusCode == 200) {
              let resultDialog: ResultDialog = {
                status: true
              };
  
              this.ref.close(resultDialog);
            } else {
              this.awaitResult = false;
              let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
              this.showMessage(msg);
            }
          });
        // }

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
