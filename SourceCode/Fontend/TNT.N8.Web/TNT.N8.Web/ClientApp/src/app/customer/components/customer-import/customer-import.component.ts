import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';

import { CustomerImportDetailComponent } from "../customer-import-detail/customer-import-detail.component";

import { DialogService, DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

import * as XLSX from 'xlsx';

export interface IDialogData {
  status: boolean;
  statusImport: boolean;
}

class Gender {
  public name: string;
  public code: string;
}

class companyCustomer {
  public index: number;
  public customerName: string;
  public customerCode: string;
  public customerGroupCode: string;
  public customerEmail: string;
  public customerPhone: string;
  public contactName: string;
  public contactGender: string;
  public contactRole: string;
  public contactPhone: string;
  public contactEmail: string;
  public note: string
}

class personalCustomer {
  public index: number;
  public customerGroupCode: string;
  public customerName: string;
  public customerGender: string;
  public customerPhone: string;
  public customerEmail: string;
  public note: string
}

@Component({
  selector: 'app-customer-import',
  templateUrl: './customer-import.component.html',
  styleUrls: ['./customer-import.component.css'],
  providers: [DialogService]
})
export class CustomerImportComponent implements OnInit {
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  accept = '.xls, .xlsx,.xlsm';
  customerType: number = 1;
  typeControl: FormControl;
  importCustomer: FormGroup;
  /* data list from excel file */
  listGender: Array<Gender> = [{ name: "Nam", code: "NAM" }, { name: "Nữ", code: "NU" }];
  listCompanyCustomerExcel: Array<companyCustomer> = [];
  listPersonalCustomerExcel: Array<personalCustomer> = [];
  //status import
  importSuccess: boolean = false;
  //biến phân biệt import khách hàng hoặc khách hàng tiềm năng
  isPotentialCustomer: boolean = false;
  constructor(
    private dialogService: DialogService,
    private ref1: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService
  ) {
    if (this.config.data) {
      this.isPotentialCustomer = this.config.data.isPotentialCustomer;
    }
  }

  ngOnInit() {
    this.importCustomer = new FormGroup({
      "customerType": new FormControl("DN"),
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  onCancelClick1() {
    /* let result: IDialogData = {
      status: false,
      importSuccess: this.importSuccess
    }; */
    this.ref1.close();
  }

  onFileChange(event: any) {
    const targetFiles: DataTransfer = <DataTransfer>(event.target);
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(targetFiles.files[0]);

    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
      const COMPANY_CUSTOMER_CODE = "DN"; //Mã sheet khách hàng cá nhân
      const PERSONAL_CUSTOMER_CODe = "CN" //Mã sheet khách hàng doanh nghiệp

      //kiểm tra form value và file excel có khớp mã với nhau hay không
      let customerCode = this.importCustomer.get('customerType').value;
      if (workbook.Sheets[customerCode] === undefined) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
        this.showMessage(mgs);
        return;
      }

      switch (customerCode) {
        case "DN":
          //lấy data từ file excel của khách hàng doanh nghiệp
          const worksheetCompanyCustomer: XLSX.WorkSheet = workbook.Sheets[COMPANY_CUSTOMER_CODE];
          /* save data */
          let dataCompanyCustomer: Array<any> = XLSX.utils.sheet_to_json(worksheetCompanyCustomer, { header: 1 });
          //remove header row
          dataCompanyCustomer.shift();
          dataCompanyCustomer.forEach(row => {
            let newCustomer = new companyCustomer();
            newCustomer.index = row[0];
            newCustomer.customerCode = row[1];
            newCustomer.customerName = row[2];
            newCustomer.customerEmail = row[3];
            newCustomer.customerPhone = row[4];
            newCustomer.contactName = row[5];
            newCustomer.contactRole = row[6];
            newCustomer.contactGender = row[7];
            newCustomer.contactPhone = row[8];
            newCustomer.contactEmail = row[9];
            newCustomer.customerGroupCode = row[10];
            newCustomer.note = row[11]
            this.listCompanyCustomerExcel.push(newCustomer);
          });
          //this.ref.close()
          this.openImportDetailDialog("DN", this.listCompanyCustomerExcel, []);

          break;
        case "CN":
          //lấy data từ file excel của khách hàng cá nhân
          const worksheetPersonalCustomer: XLSX.WorkSheet = workbook.Sheets[PERSONAL_CUSTOMER_CODe];
          /* save data */
          let dataPersonalCustomer: Array<any> = XLSX.utils.sheet_to_json(worksheetPersonalCustomer, { header: 1 });
          //remove header row
          dataPersonalCustomer.shift();
          dataPersonalCustomer.forEach(row => {
            let newCustomer = new personalCustomer();
            newCustomer.index = row[0];
            newCustomer.customerGroupCode = row[1];
            newCustomer.customerName = row[2];
            newCustomer.customerGender = row[3];
            newCustomer.customerPhone = row[4];
            newCustomer.customerEmail = row[5];
            newCustomer.note = row[6];
            this.listPersonalCustomerExcel.push(newCustomer);
          });

          this.openImportDetailDialog("CN", [], this.listPersonalCustomerExcel);
          break;
        default:
          break;
      }
    }
  }

  openImportDetailDialog(customerType: string, listCompanyCustomer: Array<companyCustomer>, listPersonalCustomer: Array<personalCustomer>) {
    switch (customerType) {
      case "DN":
        let ref1 = this.dialogService.open(CustomerImportDetailComponent, {
          data: {
            customerType: customerType,
            listCompanyCustomer: listCompanyCustomer,
            listPersonalCustomer: listPersonalCustomer,
            isPotentialCustomer: this.isPotentialCustomer
          },
          header: 'Import Khách hàng doanh nghiệp',
          width: '85%',
          baseZIndex: 1050,
          contentStyle: {
            //"min-height": "400px",
            "max-height": "800px",
            "over-flow": "hidden"
          }
        });

        ref1.onClose.subscribe((result: any) => {
          if (result) {
            if (result.statusImport === true) this.importSuccess = true;
          }

          let resutlImport: IDialogData = {
            status: false,
            statusImport: this.importSuccess
          };
          this.ref1.close(resutlImport);
        });
        break;
      case "CN":
        let ref2 = this.dialogService.open(CustomerImportDetailComponent, {
          data: {
            customerType: customerType,
            listCompanyCustomer: listCompanyCustomer,
            listPersonalCustomer: listPersonalCustomer,
            isPotentialCustomer: this.isPotentialCustomer
          },
          header: 'Import Khách hàng cá nhân',
          width: '85%',
          baseZIndex: 1050,
          contentStyle: {
            //"min-height": "400px",
            "max-height": "800px",
          }
        });

        ref2.onClose.subscribe((result: any) => {
          if (result) {
            if (result.statusImport === true) this.importSuccess = true;
          }
          let resutlImport: IDialogData = {
            status: false,
            statusImport: this.importSuccess
          };
          this.ref1.close(resutlImport);
        });
        break;
      default:
        break;
    }


  }
  /* end */
}
