import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { CustomerService } from "../../services/customer.service";

interface CustomerMeeting {
  customerMeetingId: string,
  customerId: string,
  employeeId: string,
  title: string,
  locationMeeting: string,
  startDate: Date,
  startHours: Date,
  endDate: Date,
  endHours: Date,
  content: string,
  participants: string,
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

interface ResultDialog {
  status: boolean
}

@Component({
  selector: 'app-meeting-dialog',
  templateUrl: './meeting-dialog.component.html',
  styleUrls: ['./meeting-dialog.component.css']
})
export class MeetingDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  awaitResult: boolean = false;
  today: Date = new Date();
  customerMeetingId: string = null;
  customerId: string = null;
  customerMeeting: CustomerMeeting = {
    customerMeetingId: '',
    customerId: '',
    employeeId: '',
    title: '',
    locationMeeting: '',
    startDate: new Date(),
    startHours: new Date(),
    endDate: null,
    endHours: null,
    content: '',
    participants: '',
  }
  empUser: Employee;
  isCreateBylead: boolean = false;
  listParticipants: Array<Employee> = [];
  listParticipantModel: Array<Employee> = [];
  fromDate: any;
  fromTime: any;
  toDate: any;
  toTime: any;
  /*MEETING FORM*/
  meetingForm: FormGroup;

  titleControl: FormControl;
  locationMeetingControl: FormControl;
  startDateControl: FormControl;
  startHoursControl: FormControl;
  endDateControl: FormControl;
  endHoursControl: FormControl;
  contentControl: FormControl;
  participantControl: FormControl;
  /*END*/

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerService: CustomerService,
  ) {
    this.customerId = this.config.data.customerId;
    this.customerMeetingId = this.config.data.customerMeetingId;
    if (this.config.data.isCreateBylead === true) {
      this.isCreateBylead = true;
    }
    this.listParticipants = this.config.data.listParticipants;
    this.empUser = this.listParticipants.find(c => c.employeeId == this.auth.EmployeeId);
  }

  ngOnInit() {
    this.titleControl = new FormControl(null, [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.locationMeetingControl = new FormControl(null, [Validators.required, Validators.maxLength(350), forbiddenSpaceText]);
    this.startDateControl = new FormControl(new Date(), [Validators.required]);
    this.startHoursControl = new FormControl(new Date(), [Validators.required]);
    this.endDateControl = new FormControl(null);
    this.endHoursControl = new FormControl(null);
    this.contentControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.participantControl = new FormControl([this.empUser]);

    this.meetingForm = new FormGroup({
      titleControl: this.titleControl,
      locationMeetingControl: this.locationMeetingControl,
      startDateControl: this.startDateControl,
      startHoursControl: this.startHoursControl,
      endDateControl: this.endDateControl,
      endHoursControl: this.endHoursControl,
      contentControl: this.contentControl,
      participantControl: this.participantControl
    });
    if (this.customerMeetingId) {
      //Cập nhật
      this.customerService.getDataCustomerMeetingById(this.customerMeetingId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.customerMeeting = result.customerMeeting;

          this.titleControl.setValue(this.customerMeeting.title);
          this.locationMeetingControl.setValue(this.customerMeeting.locationMeeting);
          this.startDateControl.setValue(new Date(this.customerMeeting.startDate));
          this.startHoursControl.setValue(new Date(this.customerMeeting.startDate));
          if (this.customerMeeting.endDate) {
            this.endDateControl.setValue(new Date(this.customerMeeting.endDate));
            this.endHoursControl.setValue(new Date(this.customerMeeting.endDate));
          }
          this.contentControl.setValue(this.customerMeeting.content);
          if(this.customerMeeting.participants){
            let listTemp = this.customerMeeting.participants.split(';');
            this.listParticipantModel = this.listParticipants.filter(c => listTemp.includes(c.employeeId));
          }
          this.participantControl.setValue(this.listParticipantModel);
        } else {
          let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  addMeeting() {
    if (!this.meetingForm.valid) {
      Object.keys(this.meetingForm.controls).forEach(key => {
        if (!this.meetingForm.controls[key].valid) {
          this.meetingForm.controls[key].markAsTouched();
        }
      });
    } else {
      if (!this.customerMeetingId) {
        //Tạo mới
        this.customerMeeting.customerMeetingId = null;
        this.customerMeeting.customerId = this.customerId;
        this.customerMeeting.employeeId = this.emptyGuid;
      }

      this.customerMeeting.title = this.titleControl.value.trim();
      this.customerMeeting.locationMeeting = this.locationMeetingControl.value.trim();
      let startDate = convertToUTCTime(this.startDateControl.value)
      this.customerMeeting.startDate = startDate;
      let startHours = convertToUTCTime(this.startHoursControl.value);
      this.customerMeeting.startHours = startHours;

      let endDate = null;
      if (this.endDateControl.value) {
        endDate = convertToUTCTime(this.endDateControl.value);
      }
      this.customerMeeting.endDate = endDate;
      let endHours = null;
      if (this.endHoursControl.value) {
        endHours = convertToUTCTime(this.endHoursControl.value);
      }
      this.customerMeeting.endHours = endHours;
      this.customerMeeting.content = this.contentControl.value.trim();
      let participantId = this.participantControl.value.map((c: { employeeId: any; }) => c.employeeId);

      this.customerMeeting.participants = '';
      for(let i = 0; i < participantId.length; i++){
        if(i == participantId.length - 1){
          this.customerMeeting.participants += participantId[i];
        }else{
          this.customerMeeting.participants += participantId[i] + ';';
        }
      }
      this.awaitResult = true;

      if (this.isCreateBylead === true) {
        this.customerService.createCustomerMeeting(this.customerMeeting).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            let resultDialog: ResultDialog = {
              status: true
            }

            this.ref.close(resultDialog);
          } else {
            this.awaitResult = false;
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      } else {
        this.customerService.createCustomerMeeting(this.customerMeeting).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            let resultDialog: ResultDialog = {
              status: true
            }

            this.ref.close(resultDialog);
          } else {
            this.awaitResult = false;
            let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        })
      }
    }
  }

  changeStartDate(e) {
    this.endDateControl.setValue(e.value);
    this.endDateControl.updateValueAndValidity();
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
