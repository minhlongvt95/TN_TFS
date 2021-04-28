import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';

//ADAPTER Date: dd/mm/yyyy
import { AppDateAdapter, APP_DATE_FORMATS } from '../../../shared/adapter/date.adapter';

//SERVICES
import { CustomerCareService } from '../../services/customer-care.service';

@Component({
  selector: 'app-update-time-customer-care',
  templateUrl: './update-time-customer-care.component.html',
  styleUrls: ['./update-time-customer-care.component.css'],
  providers: [
    {
      provide: DateAdapter, useClass: AppDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS
    }
  ]
})
export class UpdateTimeCustomerCareComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;

  customerCareId: string = '';
  statusId: string = '';
  isSendNow: boolean = true;
  sendDate: Date = null;
  sendHour: Date = null;
  typeCusCareCode: string = '';

  /*Form feed back*/
  formUpdateTimeCustomerCare: FormGroup;

  sendDateControl: FormControl;
  sendHourControl: FormControl;
  /*End*/

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(
    @Inject(MAT_DIALOG_DATA) public dataDialog: any,
    public dialogPop: MatDialog,
    public dialogRef: MatDialogRef<UpdateTimeCustomerCareComponent>,
    public snackBar: MatSnackBar,
    private customerCareService: CustomerCareService
  ) {
    this.customerCareId = this.dataDialog.customerCareId;
    this.statusId = this.dataDialog.statusId;
    this.typeCusCareCode = this.dataDialog.typeCusCareCode;
  }

  ngOnInit() {
    this.sendDateControl = new FormControl('', [Validators.required]);
    this.sendHourControl = new FormControl('', [Validators.required]);

    this.formUpdateTimeCustomerCare = new FormGroup({
      sendDateControl: this.sendDateControl,
      sendHourControl: this.sendHourControl
    });
  }

  onChangeRadio(event: any) {
    if (event.value == 'true') {
      //Nếu là gửi ngay
      this.isSendNow = true;
      this.sendDate = null;
      this.sendHour = null;
    } else {
      //Nếu là gửi vào khoảng thời gian
      this.isSendNow = false;
      this.sendDate = new Date();
      this.sendHourControl.setValue('00:00');
      this.sendHour = this.sendHourControl.value;
    }
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  updateTime() {
    if (!this.isSendNow) {
      if (!this.formUpdateTimeCustomerCare.valid) {
        Object.keys(this.formUpdateTimeCustomerCare.controls).forEach(key => {
          if (!this.formUpdateTimeCustomerCare.controls[key].valid) {
            this.formUpdateTimeCustomerCare.controls[key].markAsTouched();
          }
        });
      } else {
        let today = new Date();
        if (this.sendDate != null) {
          this.sendDate.setHours(today.getHours());
          this.sendDate.setMinutes(today.getMinutes());
          this.sendDate.setSeconds(today.getSeconds());
        }
        this.customerCareService.updateStatusCustomerCare(this.customerCareId, this.statusId, this.isSendNow, this.sendDate, this.sendHour, this.typeCusCareCode, this.userId).subscribe(response => {
          let result_updateStatus = <any>response;
          if (result_updateStatus.statusCode == 200) {
            this.snackBar.openFromComponent(SuccessComponent, { data: result_updateStatus.messageCode, ... this.successConfig });
            let result = {
              statusCode: result_updateStatus.statusCode
            }
            this.dialogRef.close(result);
          } else {
            this.snackBar.openFromComponent(FailComponent, { data: result_updateStatus.messageCode, ... this.failConfig });
            let result = {
              statusCode: result_updateStatus.statusCode
            }
            this.dialogRef.close(result);
          }
        });
      }
    } else {
      this.customerCareService.updateStatusCustomerCare(this.customerCareId, this.statusId, this.isSendNow, this.sendDate, this.sendHour, this.typeCusCareCode, this.userId).subscribe(response => {
        let result_updateStatus = <any>response;
        if (result_updateStatus.statusCode == 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result_updateStatus.messageCode, ... this.successConfig });
          let result = {
            statusCode: result_updateStatus.statusCode
          }
          this.dialogRef.close(result);
        } else {
          this.snackBar.openFromComponent(FailComponent, { data: result_updateStatus.messageCode, ... this.failConfig });
          let result = {
            statusCode: result_updateStatus.statusCode
          }
          this.dialogRef.close(result);
        }
      });
    }
  }

  //Filter Validator ngày không <= ngày hiện tại
  myFilter = (d: Date): boolean => {
    let day = d;
    const now = new Date();
    now.setHours(0,0,0,0);
    return (day >= now);
  }
}
