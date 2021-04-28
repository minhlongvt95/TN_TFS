import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { ContactModel } from '../../../shared/models/contact.model';
import { LeadModel } from '../../models/lead.model';
import { LeadService } from "../../services/lead.service";
import { DialogService, DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import * as XLSX from 'xlsx';

import { LeadImportDetailComponent } from '../lead-import-detail/lead-import-detail.component';

export interface IDialogData {
  status: boolean;
}

class leadModel {
  index: string;
  leadName: string;
  leadGender: string;
  leadPhone: string;
  identity: string;
  address: string;
  provinceCode: string;
  districtCode: string;
  wardCode: string;
  email: string;
  interestedGroupCode: string;
  potentialCode: string;
  paymentMethodCode: string;
  companyName: string;
}

@Component({
  selector: 'app-lead-import',
  templateUrl: './lead-import.component.html',
  styleUrls: ['./lead-import.component.css'],
  providers: [MessageService, DialogService]

})
export class LeadImportComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  accept = '.xls, .xlsx,.xlsm';

  month: number;
  today: Date = new Date();
  maxSize: number = 11000000;
  files: File[] = [];
  dragFiles: any;
  lastInvalids: any;
  baseDropValid: any;
  lastFileAt: Date;
  importCustomer: FormGroup;
  listImportLead: Array<leadModel> = [];

  getDate() {
    return new Date();
  }

  constructor(
    private dialogService: DialogService,
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService
  ) { }

  ngOnInit() {
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  onFileChange(event: any) {
    const targetFiles: DataTransfer = <DataTransfer>(event.target);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);

    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
      const LEAD_CODE = "Lead"; //Mã sheet khách hàng cá nhân

      if (workbook.Sheets[LEAD_CODE] === undefined) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      const workSheet: XLSX.WorkSheet = workbook.Sheets[LEAD_CODE];
      /* save data */
      let data: Array<leadModel> = XLSX.utils.sheet_to_json(workSheet, { header: 1 });
      //remove header row
      data.shift();
      data.forEach(row => {
        let newLead = new leadModel();
        newLead.index = ParseToString(row[0]);
        newLead.leadName = ParseToString(row[1]);
        newLead.leadGender = ParseToString(row[2]);
        newLead.leadPhone = ParseToString(row[3]);
        newLead.identity = ParseToString(row[4]);
        newLead.address = ParseToString(row[5]);
        newLead.provinceCode = ParseToString(row[6]);
        newLead.districtCode = ParseToString(row[7]);
        newLead.wardCode = ParseToString(row[8]);
        newLead.email = ParseToString(row[9]);
        newLead.interestedGroupCode = ParseToString(row[10]);
        newLead.potentialCode = ParseToString(row[11]);
        newLead.paymentMethodCode = ParseToString(row[12]);
        newLead.companyName = ParseToString(row[13]);
        this.listImportLead.push(newLead);
      });

      //  this.ref.close(); //close pop-up chọn file
      //mở pop-up chi tiết import
      let ref = this.dialogService.open(LeadImportDetailComponent, {
        data: {
          listImportLead: this.listImportLead
        },
        header: 'Import Khách hàng tiềm năng',
        width: '85%',
        baseZIndex: 1050,
        contentStyle: {
          "max-height": "800px",
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result) {
          if (result.status === true) {
            let result: IDialogData = {
              status: true
            }
            this.ref.close(result);
          }
        }
        this.ref.close();
      });

    }
  }

  onCancel() {
    this.ref.close();
  }
}

function ParseToString(str: any) {
  if (!str) return '';
  else return String(str).trim();
}
