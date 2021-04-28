import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

//SERVICES
import { EmailService } from '../../services/email.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

//COMPONENT
import { TemplateEmailLeadCusDialogComponent } from '../template-email-dialog/template-email-dialog.component';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  message: string,
  messageCode: string
}

@Component({
  selector: 'app-send-email-dialog',
  templateUrl: './send-email-dialog.component.html',
  styleUrls: ['./send-email-dialog.component.css']
})
export class SendEmailDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  selectedTypeTime: number = 0;

  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";

  leadIdList: Array<any> = [];
  isSendEmailNow: boolean = true;

  /*Form send email*/
  formSendEmail: FormGroup;

  titleControl: FormControl;
  contentControl: FormControl;
  sendEmailDateControl: FormControl;
  sendEmailHourControl: FormControl;
  /*End*/

  today:Date = new Date();
  // dialogSelectTemplateEmail: MatDialogRef<TemplateEmailLeadCusDialogComponent>;
  
  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private leadService: EmailService,
  ) { 
    this.leadIdList = this.config.data.leadIdList;
  }

  ngOnInit() {
    this.titleControl = new FormControl(null, [Validators.required, Validators.maxLength(300)]);
    this.contentControl = new FormControl(null, [Validators.required]);
    this.sendEmailDateControl = new FormControl(new Date(), [Validators.required]);
    this.sendEmailHourControl = new FormControl(new Date(), [Validators.required]);

    this.formSendEmail = new FormGroup({
      titleControl: this.titleControl,
      contentControl: this.contentControl,
      sendEmailDateControl: this.sendEmailDateControl,
      sendEmailHourControl: this.sendEmailHourControl
    });
  }

  sendEmail() {
    let resultDialog: ResultDialog = {
      status: true,  //Gửi
      message: '',
      messageCode: ''
    };

    Object.keys(this.formSendEmail.controls).forEach(key => {
      if (!this.formSendEmail.controls[key].valid ) {
        this.formSendEmail.controls[key].markAsTouched();
      }
    });

    let check = this.validateForm();
    if (check) { 
      this.loading = true;
      
      let sendEmailDate = convertToUTCTime(this.sendEmailDateControl.value);
      let sendEmailHour = convertToUTCTime(this.sendEmailHourControl.value);

      this.leadService.sendEmailLead(this.titleControl.value, this.contentControl.value, this.isSendEmailNow, 
        sendEmailDate, sendEmailHour, this.leadIdList, this.userId).subscribe(response => {
          let result = <any>response;
          this.loading = false;          
          if (result.statusCode == 200) {
            let listCustomerEmailIgnored:Array<any> = result.listCustomerEmailIgnored;
            let listCustomerNameIgnored: Array<string> = [];
            listCustomerEmailIgnored.forEach(customer => {
              let fullName = customer.firstName + ' ' + customer.lastName;
              listCustomerNameIgnored.push(fullName);
            });

            if (this.leadIdList.length === 1) {
              if (listCustomerEmailIgnored.length == 0){
                resultDialog.message = "success";
                resultDialog.messageCode = "Gửi email thành công";
                this.ref.close(resultDialog);
              } else {
                resultDialog.message = "warning";
                resultDialog.messageCode = `Khách hàng ${listCustomerNameIgnored.join(', ')} chưa có email nhận. Đề nghị bổ sung trước khi gửi`;
                this.ref.close(resultDialog);
              }
            } else {
              //TODO: gửi message khi multi send email
            }               
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
    if (this.titleControl.valid && this.contentControl.valid) {
      if (this.isSendEmailNow) {
        result = true;
      } else {
        if (this.sendEmailDateControl.valid && this.sendEmailHourControl.valid) {
          result = true;
        }
      }
    }
    return result;
  }

  //Chọn email mẫu
  // selectEmailTemplate() {
  //   this.dialogSelectTemplateEmail = this.dialogPop.open(TemplateEmailLeadCusDialogComponent,
  //     {
  //       width: '1200px',
  //       height: 'auto',
  //       autoFocus: false,
  //       data: {}
  //     });

  //   this.dialogSelectTemplateEmail.afterClosed().subscribe(result => {
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
      this.isSendEmailNow = true;
    } else {
      this.isSendEmailNow = false;
    }
  }
  /*End*/
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
