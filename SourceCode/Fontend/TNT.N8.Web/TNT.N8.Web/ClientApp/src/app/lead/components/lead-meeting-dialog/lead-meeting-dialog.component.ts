import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { LeadCareService } from '../../services/lead-care.service';
import { LeadService } from '../../services/lead.service';

interface LeadMeeting {
  leadMeetingId: string,
  leadId: string,
  employeeId: string,
  title: string,
  locationMeeting: string,
  startDate: Date,
  startHours: Date,
  endDate: Date,
  endHours: Date,
  content: string,
  participant: string,
}

class Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

interface ResultDialog {
  status: boolean
}

@Component({
  selector: 'app-lead-meeting-dialog',
  templateUrl: './lead-meeting-dialog.component.html',
  styleUrls: ['./lead-meeting-dialog.component.css']
})
export class LeadMeetingDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  awaitResult: boolean = false;
  today: Date = new Date();
  leadMeetingId: string = null;
  leadId: string = null;
  leadMeeting: LeadMeeting = {
    leadMeetingId: '',
    leadId: '',
    employeeId: '',
    title: '',
    locationMeeting: '',
    startDate: new Date(),
    startHours: new Date(),
    endDate: null,
    endHours: null,
    content: '',
    participant: '',
  }

  listParticipant: Array<Employee> = [];
  listParticipantModel: Array<Employee> = [];
  empUser: Employee;
  // listParticipantId: Array<Employee> = [];
  isCreateBylead: boolean = false;

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
    private leadService: LeadService,
    private leadCateService : LeadCareService
  ) {
    this.leadId = this.config.data.leadId;
    this.leadMeetingId = this.config.data.leadMeetingId;
    if (this.config.data.isCreateBylead === true) {
      this.isCreateBylead = true;
    }
    this.listParticipant = this.config.data.listEmployee;
    this.empUser = this.listParticipant.find(c => c.employeeId == this.auth.EmployeeId);
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

    if (this.leadMeetingId) {
      //Cập nhật
      this.leadService.getDataLeadMeetingById(this.leadMeetingId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.leadMeeting = result.leadMeeting;

          this.titleControl.setValue(this.leadMeeting.title);
          this.locationMeetingControl.setValue(this.leadMeeting.locationMeeting);
          this.startDateControl.setValue(new Date(this.leadMeeting.startDate));
          this.startHoursControl.setValue(new Date(this.leadMeeting.startDate));
          if (this.leadMeeting.endDate) {
            this.endDateControl.setValue(new Date(this.leadMeeting.endDate));
            this.endHoursControl.setValue(new Date(this.leadMeeting.endDate));
          }
          this.contentControl.setValue(this.leadMeeting.content);
          if(this.leadMeeting.participant){
            let listTemp = this.leadMeeting.participant.split(';');
            this.listParticipantModel = this.listParticipant.filter(c => listTemp.includes(c.employeeId));
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
      if (!this.leadMeetingId) {
        //Tạo mới
        this.leadMeeting.leadMeetingId = null;
        this.leadMeeting.leadId = this.leadId;
        this.leadMeeting.employeeId = this.emptyGuid;
      }
      
      this.leadMeeting.title = this.titleControl.value.trim();
      this.leadMeeting.locationMeeting = this.locationMeetingControl.value.trim();
      let startDate = convertToUTCTime(this.startDateControl.value)
      this.leadMeeting.startDate = startDate;
      let startHours = convertToUTCTime(this.startHoursControl.value);
      this.leadMeeting.startHours = startHours;

      let endDate = null;
      if (this.endDateControl.value) {
        endDate = convertToUTCTime(this.endDateControl.value);
      }
      this.leadMeeting.endDate = endDate;
      let endHours = null;
      if (this.endHoursControl.value) {
        endHours = convertToUTCTime(this.endHoursControl.value);
      }
      this.leadMeeting.endHours = endHours;
      this.leadMeeting.content = this.contentControl.value.trim();
      let participantId = this.participantControl.value.map((c: { employeeId: any; }) => c.employeeId);

      this.leadMeeting.participant = '';
      for(let i = 0; i < participantId.length; i++){
        if(i == participantId.length - 1){
          this.leadMeeting.participant += participantId[i];
        }else{
          this.leadMeeting.participant += participantId[i] + ';';
        }
      }
      this.awaitResult = true;

      if (this.isCreateBylead === true) {
        this.leadCateService.createLeadMeeting(this.leadMeeting).subscribe(response => {
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
        this.leadCateService.createLeadMeeting(this.leadMeeting).subscribe(response => {
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
