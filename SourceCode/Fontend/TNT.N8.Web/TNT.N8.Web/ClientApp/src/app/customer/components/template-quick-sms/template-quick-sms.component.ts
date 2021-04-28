import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
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
  selector: 'app-template-quick-sms',
  templateUrl: './template-quick-sms.component.html',
  styleUrls: ['./template-quick-sms.component.css']
})
export class TemplateQuickSmsComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;

  queueModel: QueueModel = new QueueModel();
  sendTo: string = '';
  customerId: string = '';
  isCreateBylead: boolean = false;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";

  /*Form feed back*/
  formQuickSMS: FormGroup;

  phoneControl: FormControl;
  contentControl: FormControl;
  /*End*/

  // dialogSelectTemplateEmail: MatDialogRef<TemplateEmailDialogComponent>;
  // dialogSelectTemplateSMS: MatDialogRef<TemplateSmsDialogComponent>;

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerCareService: CustomerCareService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.customerId = this.config.data.customerId;
    if (this.config.data.isCreateBylead === true) {
      this.isCreateBylead = true;
    }
  }

  ngOnInit() {
    this.phoneControl = new FormControl({ value: this.sendTo, disabled: true }, [Validators.required]);
    this.contentControl = new FormControl('', [Validators.required, forbiddenSpaceText]);

    this.formQuickSMS = new FormGroup({
      phoneControl: this.phoneControl,
      contentControl: this.contentControl
    });
  }

  sendSMS() {
    if (!this.formQuickSMS.valid) {
      Object.keys(this.formQuickSMS.controls).forEach(key => {
        if (!this.formQuickSMS.controls[key].valid) {
          this.formQuickSMS.controls[key].markAsTouched();
        }
      });
    } else {
      if (!this.sendTo) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Không có số điện thoại. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else if (this.sendTo.trim() == "") {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Không có số điện thoại. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else {
        this.queueModel.FromTo = '';
        this.queueModel.SendTo = this.sendTo;
        this.queueModel.CustomerId = this.customerId;
        this.queueModel.SendContent = this.contentControl.value.trim();
        this.queueModel.Method = "SMS";
        this.queueModel.Title = "Chăm sóc khách hàng SMS";
        this.queueModel.CreateById = this.userId;

        this.awaitResult = true;

        // if (this.isCreateBylead == true) {
        //   this.customerCareService.sendQuickLeadSMS(this.queueModel).subscribe(response => {
        //     let result = <any>response;
        //     if (result.statusCode == 200) {
        //       let resultDialog: ResultDialog = {
        //         status: true
        //       };
        //       this.ref.close(resultDialog);
        //     } else {
        //       this.awaitResult = false;
        //       let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        //       this.showMessage(msg);
        //     }
        //   });
        // } else {
          this.customerCareService.sendQuickSMS(this.queueModel).subscribe(response => {
            let result = <any>response;
            if (result.statusCode == 200) {
              let resultDialog: ResultDialog = {
                status: true
              };

              this.ref.close(resultDialog);
            } else {
              this.awaitResult = false;
              let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
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
