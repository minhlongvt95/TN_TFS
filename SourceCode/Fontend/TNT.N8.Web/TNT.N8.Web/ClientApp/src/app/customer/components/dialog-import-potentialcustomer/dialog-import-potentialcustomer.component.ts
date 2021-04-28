import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms'

//SERVICES
import { CustomerService } from '../../services/customer.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';

//MODELS
import { CustomerModel } from "../../models/customer.model";
import { ContactModel } from "../../../shared/models/contact.model";

class investFund {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  isDefault: boolean;
}

class Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

class ResultDialog {
  status: boolean;
}

class Status {
  public code: string;
  public name: string;
}

class ImportPotentialCustomerModel {
  customerType: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  socialUrl: string;
  picCode: string;
  investmentFundName: string;
  note: string;
  selectedCustomerType: CustomerType;
  selectedPic: Employee
  selectedInvestmentFund: investFund;

  //handler status
  valid: boolean;
  listStatus: Array<Status>;
  statusText: string;

  constructor() {
    this.customerType = 0;
    this.firstName = '';
    this.lastName = '';
    this.email = '';
    this.phone = '';
    this.socialUrl = '';
    this.picCode = '';
    this.investmentFundName = '';
    this.note = '';
    this.selectedCustomerType = null;
    this.selectedPic = null;
    this.selectedInvestmentFund = null;
    this.valid = true;
    this.listStatus = [];
  }
}

class CustomerType {
  label: string;
  value: number;
}

@Component({
  selector: 'app-dialog-import-potentialcustomer',
  templateUrl: './dialog-import-potentialcustomer.component.html',
  styleUrls: ['./dialog-import-potentialcustomer.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class DialogImportPotentialcustomerComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  listStatus: Array<Status> = [
    { code: "required_firstname", name: "Nhập Họ và tên" },
    { code: "required_lastname", name: "Nhập tên" },
    { code: "required_phone", name: "Nhập số điện thoại" },
    { code: "duplicate_phone_client", name: "Trùng số điện thoại" },
    { code: "duplicate_phone_server", name: "Đã tồn tại số điện thoại" },
    { code: "duplicate_email_client", name: "Trùng Email" },
    { code: "duplicate_email_server", name: "Đã tồn tại Email" },
  ]

  //master data
  listPersonalInChange: Array<Employee> = [];
  listInvestFund: Array<investFund> = [];
  listPhone: Array<string> = [];
  listEmail: Array<string> = [];

  //table
  rows: number = 10;
  cols: Array<any> = [];
  selectedCols: Array<any> = [];

  //const value
  listCustomerType: Array<CustomerType> = [
    { label: "Doanh nghiệp", value: 1 },
    { label: "Doanh nghiệp", value: 2 }
  ];

  //dialog data
  listImportPotentialCustomerImport: Array<ImportPotentialCustomerModel> = [];
  listValidPotentialCustomer: Array<ImportPotentialCustomerModel> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private customerService: CustomerService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    if (this.config.data) {
      this.listImportPotentialCustomerImport = this.config.data.listImportPotentialCustomerImport;
    }
    this.initTable();
    this.getMasterData();
  }

  initTable() {
    //table khách hàng doanh nghiệp
    this.cols = [
      { field: 'check', header: '', textAlign: 'center', width: "1rem" },
      { field: 'customerType', header: 'Loại tiềm năng', textAlign: 'left', display: 'table-cell' },
      { field: 'firstName', header: 'Họ và tên đệm', textAlign: 'left', display: 'table-cell' },
      { field: 'lastName', header: 'Tên', textAlign: 'left', display: 'table-cell' },
      { field: 'email', header: 'Email', textAlign: 'left', display: 'table-cell' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'left', display: 'table-cell' },
      { field: 'socialUrl', header: 'Facebook', textAlign: 'left', display: 'table-cell' },
      { field: 'picCode', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell' },
      { field: 'investmentFundName', header: 'Nguồn gốc', textAlign: 'left', display: 'table-cell' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', display: 'table-cell' },
      { field: 'statusText', header: 'Trạng thái', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedCols = this.cols;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.customerService.getDataImportPotentialCustomer(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listPersonalInChange = result.listPersonalInChange;
      this.listInvestFund = result.listInvestFund;
      this.listPhone = result.listPhone;
      this.listEmail = result.listEmail;

      this.validationExcelData();
      this.checkStatus();
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  validationExcelData() {
    this.listImportPotentialCustomerImport.forEach((row, index) => {
      row.firstName = validateString(row.firstName);
      row.lastName = validateString(row.lastName);
      row.email = validateString(row.email);
      row.phone = validateString(row.phone);
      row.socialUrl = validateString(row.socialUrl);
      row.picCode = validateString(row.picCode);
      row.investmentFundName = validateString(row.investmentFundName);
      row.note = validateString(row.note);

      /* loại khách hàng */
      let _customerType = this.listCustomerType.find(e => e.value == row.customerType);
      row.selectedCustomerType = _customerType ? _customerType : null;
      let _pic = this.listPersonalInChange.find(e => e.employeeCode == row.picCode)
      row.selectedPic = _pic ? _pic : null;
      let _investFund = this.listInvestFund.find(e => e.categoryName == row.investmentFundName);
      row.selectedInvestmentFund = _investFund ? _investFund : null;
    });
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
    };
    this.ref.close(result);
  }

  async importCustomer() {

    if (this.listValidPotentialCustomer.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }

    let inValidRecord = this.listValidPotentialCustomer.find(e => e.valid == false);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }

    let listCustomer = this.mappingToListCustomer();
    let listContact = this.mappingToListContactModel();

    this.loading = true;
    let result: any = await this.customerService.importListCustomer(listCustomer, listContact, [], true, this.userId);
    
    this.loading = false;

    if (result.statusCode === 200) {
      let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Import thành công' };
      this.showMessage(mgs);
      let resultPersonalCustomer: ResultDialog = {
        status: true,
      };
      this.ref.close(resultPersonalCustomer);
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Import thất bại' };
      this.showMessage(mgs);
      let resultPersonalCustomer: ResultDialog = {
        status: false,
      };
      this.ref.close(resultPersonalCustomer);
    }

  }

  checkStatus() {
    /* check status company customer */
    this.listImportPotentialCustomerImport.forEach(customer => {
      customer.valid = true;
      customer.listStatus = [];
      customer.statusText = '';

      /* 1. bắt buộc họ và tên (chỉ với khách hàng cá nhân) */
      if (customer.selectedCustomerType) {
        if (customer.selectedCustomerType.value == 2) {
          if (!customer.firstName) {
            customer.valid = false;
            customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "required_firstname")]
          }
        }
      }

      /* 2. bắt buộc tên */
      if (!customer.lastName) {
        customer.valid = false;
        customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "required_lastname")]
      }

      /*  3.bắt buộc số điện thoại */
      if (!customer.phone) {
        customer.valid = false;
        customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "required_phone")]
      }

      /*  4.Nhập trùng số điện thoại(ở client) */
      let othersPhoneCustomer = this.listImportPotentialCustomerImport.filter(e => (e !== customer && (customer.phone.trim() !== ""))).map(e => e.phone);
      if (customer.phone.trim() && othersPhoneCustomer.includes(customer.phone)) {
        customer.valid = false;
        customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "duplicate_phone_client")]
      }

      /*  5.Nhập trùng số điện thoại(ở server) */
      if (customer.phone.trim() && this.listPhone.includes(customer.phone)) {
        customer.valid = false;
        customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "duplicate_phone_server")]
      }

      /*  6.Nhập trùng email(ở client) */
      let othersEmailCustomer = this.listImportPotentialCustomerImport.filter(e => (e !== customer && (customer.email.trim() !== ""))).map(e => e.email);
      if (customer.email.trim() && othersEmailCustomer.includes(customer.email)) {
        customer.valid = false;
        customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "duplicate_email_client")]
      }

      /*  7.Nhập trùng email(ở server) */
      if (customer.email.trim() && this.listEmail.includes(customer.email)) {
        customer.valid = false;
        customer.listStatus = [...customer.listStatus, this.listStatus.find(e => e.code == "duplicate_email_server")]
      }

      customer.statusText = customer.listStatus.map(e => e.name).join('. ');
    });
    /* end */

    //tự động thêm vào danh sách cần import
    this.autoImportCustomerValid();
  }

  autoImportCustomerValid() {
    this.listValidPotentialCustomer = this.listImportPotentialCustomerImport.filter(e => e.valid == true);
  }

  mappingToListCustomer(): Array<CustomerModel> {
    let listCustomer: Array<CustomerModel> = [];
    this.listValidPotentialCustomer.forEach(customer => {
      let newCustomer = new CustomerModel();
      newCustomer.CustomerId = this.emptyGuid;
      let customerType = customer.selectedCustomerType;
      let customerName = `${customer.firstName} ${customer.lastName}`;
      newCustomer.CustomerType = customerType ? customerType.value : 1;
      newCustomer.CustomerName = customerName;
      newCustomer.CustomerCode = '';
      newCustomer.CreatedDate = new Date()
      listCustomer.push(newCustomer);
    });
    return listCustomer;
  }

  mappingToListContactModel(): Array<ContactModel> {
    let listContact: Array<ContactModel> = [];
    this.listValidPotentialCustomer.forEach(contact => {
      let newContact = new ContactModel();
      newContact.ContactId = this.emptyGuid;
      newContact.ObjectId = this.emptyGuid;
      newContact.ObjectType = "CUS";
      newContact.FirstName = contact.firstName;
      newContact.LastName = contact.lastName;
      newContact.Gender = null;
      newContact.Phone = contact.phone;
      newContact.Email = contact.email;
      newContact.CreatedDate = new Date();
      listContact.push(newContact);
    });
    return listContact;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function validateString(str: string) {
  if (str === undefined || str == null) return "";
  return str.trim();
}
