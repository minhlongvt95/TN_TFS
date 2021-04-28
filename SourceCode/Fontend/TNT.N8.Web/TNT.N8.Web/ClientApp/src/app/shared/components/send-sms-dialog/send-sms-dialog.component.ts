import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

//SERVICES
import { EmailService } from '../../services/email.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

//COMPONENT
import { TemplateSmsLeadCusDialogComponent } from '../template-sms-dialog/template-sms-dialog.component';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  message: string,
  messageCode: string
}

@Component({
  selector: 'app-send-sms-dialog',
  templateUrl: './send-sms-dialog.component.html',
  styleUrls: ['./send-sms-dialog.component.css']
})
export class SendSmsDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  selectedTypeTime: number = 0;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";

  leadIdList: Array<any> = [];
  isSendSMSNow: boolean = true;
  today: Date = new Date();
  /*Form send SMS*/
  formSendSMS: FormGroup;

  contentControl: FormControl;
  sendSMSDateControl: FormControl;
  sendSMSHourControl: FormControl;
  /*End*/
  
  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private leadService: EmailService,
  ) { 
    this.leadIdList = this.config.data.leadIdList;
  }

  ngOnInit() {
    this.contentControl = new FormControl(null, [Validators.required]);
    this.sendSMSDateControl = new FormControl(new Date(), [Validators.required]);
    this.sendSMSHourControl = new FormControl(new Date(), [Validators.required]);

    this.formSendSMS = new FormGroup({
      contentControl: this.contentControl,
      sendSMSDateControl: this.sendSMSDateControl,
      sendSMSHourControl: this.sendSMSHourControl
    });
  }

  sendSMS() {
    let resultDialog: ResultDialog = {
      status: true,  //Gửi
      message: '',
      messageCode: ''
    };

    Object.keys(this.formSendSMS.controls).forEach(key => {
      if (!this.formSendSMS.controls[key].valid ) {
        this.formSendSMS.controls[key].markAsTouched();
      }
    });

    let check = this.validateForm();
    if (check) { 
      this.loading = true;

      let sendSmsDate = convertToUTCTime(this.sendSMSDateControl.value);
      let sendSmsHour = convertToUTCTime(this.sendSMSHourControl.value);

      this.leadService.sendSMSLead(this.contentControl.value, this.isSendSMSNow, 
        sendSmsDate, sendSmsHour, this.leadIdList, this.userId).subscribe(response => {
          let result = <any>response;
          this.loading = false;

          if (result.statusCode == 200) {
            resultDialog.message = "success";
            resultDialog.messageCode = "Gửi SMS thành công";
            this.ref.close(resultDialog);
          } else {
            resultDialog.message = "error";
            resultDialog.messageCode = result.messageCode;
            this.ref.close(resultDialog);
          }
        });
    }
  }

  validateForm () {
    let result = false;
    if (this.contentControl.valid) {
      if (this.isSendSMSNow) {
        result = true;
      } else {
        if (this.sendSMSDateControl.valid && this.sendSMSHourControl.valid) {
          result = true;
        }
      }
    }
    return result;
  }

  //Chọn SMS mẫu
  // selectSMSTemplate() {
  //   this.dialogSelectTemplateSMS = this.dialogPop.open(TemplateSmsLeadCusDialogComponent,
  //     {
  //       width: '1200px',
  //       height: 'auto',
  //       autoFocus: false,
  //       data: {}
  //     });

  //   this.dialogSelectTemplateSMS.afterClosed().subscribe(result => {
  //     if (result) {
  //       this.sendContent = this.sendContent + result.content;
  //     }
  //   });
  // }

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      message: '',
      messageCode: ''
    };
    this.ref.close(result);
  }
  /*End*/

  replate_token(token: string) {
    let newContent = (this.contentControl.value != null ? this.contentControl.value : "") + token;
    this.contentControl.setValue(newContent);
  }

  /*Event khi thay đổi thời gian gửi (gửi ngay/gửi vào lúc)*/
  changeTypeTime(selectedTypeTime: number) {
    this.selectedTypeTime = selectedTypeTime; //thay đổi kiểu dữ liệu từ text => number
    if (this.selectedTypeTime == 0) {
      this.isSendSMSNow = true;
    } else {
      this.isSendSMSNow = false;
    }
  }
  /*End*/

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};