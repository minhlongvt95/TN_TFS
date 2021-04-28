import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, HostListener } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

//MODELS
import { QueueModel } from '../../models/queue.model';

//SERVICES
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { LeadCareService } from '../../services/lead-care.service';
import { ChipsModule } from 'primeng/chips';
import * as $ from 'jquery';
import { throwIfEmpty } from 'rxjs/operators';

//COMPONENT
// import { TemplateEmailDialogComponent } from '../template-email-dialog/template-email-dialog.component';
// import { TemplateSmsDialogComponent } from '../template-sms-dialog/template-sms-dialog.component';

interface ResultDialog {
  status: boolean
}

@Component({
  selector: 'app-lead-template-quick-sms',
  templateUrl: './lead-template-quick-sms.component.html',
  styleUrls: ['./lead-template-quick-sms.component.css']
})
export class LeadTemplateQuickSmsComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;

  queueModel: QueueModel = new QueueModel();
  queueListModel: Array<QueueModel> = [];
  sendTo: string = '';
  customerId: string = '';
  isCreateBylead: boolean = false;
  phone: string[];
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
    private leadCareService: LeadCareService,
  ) {
    this.sendTo = this.config.data.sendTo;
    this.customerId = this.config.data.customerId;
    if (this.config.data.isCreateBylead === true) {
      this.isCreateBylead = true;
    }
  }
  @HostListener('window:click', ['$event.target'])
  onClick() {
    if (this.phoneControl.invalid && (this.phoneControl.dirty || this.phoneControl.touched)) {
      $("ul.ui-inputtext").addClass("error-border");
    } else {
      $("ul.ui-inputtext").removeClass("error-border");
    }
  }

  ngOnInit() {

    // this.phoneControl = new FormControl({ value: this.sendTo, disabled: false }, [Validators.required]);
    this.phoneControl = new FormControl([], [Validators.required]);
    this.contentControl = new FormControl('', [Validators.required, forbiddenSpaceText]);

    this.formQuickSMS = new FormGroup({
      phoneControl: this.phoneControl,
      contentControl: this.contentControl
    });
    if (this.sendTo) {
      this.phone = [this.sendTo];
    }
  }
  onAddTag(event){
    if (this.phoneControl.invalid && (this.phoneControl.dirty || this.phoneControl.touched)) {
      $("ul.ui-inputtext").addClass("error-border");
    } else {
      $("ul.ui-inputtext").removeClass("error-border");
    }
  }
  onRemoveTag(event){
    if (this.phoneControl.invalid && (this.phoneControl.dirty || this.phoneControl.touched)) {
      $("ul.ui-inputtext").addClass("error-border");
    } else {
      $("ul.ui-inputtext").removeClass("error-border");
    }
  }
  sendSMS() {
    if (!this.formQuickSMS.valid) {
      Object.keys(this.formQuickSMS.controls).forEach(key => {
        if (!this.formQuickSMS.controls[key].valid) {
          this.formQuickSMS.controls[key].markAsTouched();
        }
      });
    } else {
      if (this.phone.length == 0) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Không có số điện thoại. Đề nghị bổ sung trước khi gửi" };
        this.showMessage(msg);
      } else {
        this.phone.forEach(item => {
          this.queueModel = new QueueModel();

          this.queueModel.FromTo = '';
          this.queueModel.SendTo = item;
          this.queueModel.CustomerId = this.customerId;
          this.queueModel.SendContent = this.contentControl.value.trim();
          this.queueModel.Method = "SMS";
          this.queueModel.Title = "Chăm sóc khách hàng SMS";
          this.queueModel.CreateById = this.userId;

          this.queueListModel.push(this.queueModel);
        });
        this.awaitResult = true;

        this.leadCareService.sendQuickLeadSMS(this.queueListModel).subscribe(response => {
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
