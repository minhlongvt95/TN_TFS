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
//COMPONENTs
import { CustomerImportComponent } from '../../components/customer-import/customer-import.component';
interface ResultDialog {
  status: boolean,
  statusImport: boolean
}

class Note {
  public code: string;
  public name: string;
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
  public note: string;
  public noteArray: Array<string>;
  public status: string;
  public listNoteId: Array<string>;
}

class personalCustomer {
  public index: number;
  public customerGroupCode: string;
  public customerName: string;
  public customerGender: string;
  public customerPhone: string;
  public customerEmail: string;
  public note: string;
  public noteArray: Array<string>;
  public status: string;
  public listNoteId: Array<string>;
}

class CustomerGroup {
  public categoryId: string;
  public categoryCode: string;
  public categoryName: string;
}

class importCustomerByExcelModel {
  firstName: string;
  lastName: string;
  isEmployee: boolean;
  isVendor: boolean;
  customerCode: string;
  isCompany: boolean;
  companyName: string;
  address: string;
  taxCode: string;
  phone: string;
  email: string;

  fullNameOfContact: string;
  genderOfContact: string;
  selectedGenderOfContact: Gender;
  addressOfContact: string;
  phoneOfContact: string;
  emailOfContact: string;
  note: string;

  listStatus: Array<Note>;
  isValid: boolean;
}

@Component({
  selector: 'app-customer-import-detail',
  templateUrl: './customer-import-detail.component.html',
  styleUrls: ['./customer-import-detail.component.css']
})
export class CustomerImportDetailComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  //const value
  listGender: Array<Gender> = [
    { name: "Nam", code: "NAM" },
    { name: "Nữ", code: "NU" }
  ];

  listNote: Array<Note> = [
    /* required fields */
    { code: "required_lastName", name: "Nhập tên khách hàng" },
    { code: "required_code", name: "Nhập mã khách hàng" },

    /* check exist in database */
    { code: "exist_code", name: "Đã tồn tại mã khách hàng" },
    { code: "exist_email", name: "Đã tồn tại email" },
    { code: "exist_phone", name: "Đã tồn tại số điện thoại" },

    /* duplicate in form */
    { code: "duplicate_code", name: "Trùng mã khách hàng" },
    { code: "duplicate_email", name: "Trùng email" },
    { code: "duplicate_phone", name: "Trùng số điện thoại" },
  ]

  //dialog data
  isPotentialCustomer: boolean = false;
  listCustomerImport: Array<importCustomerByExcelModel> = [];

  //table
  rows: number = 10;
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  selectedCustomerImport: Array<importCustomerByExcelModel> = [];
  customerForm: FormGroup;
  //master data
  listCustomerCompanyCode: Array<string> = [];
  listCustomerGroup: Array<CustomerGroup> = [];
  listEmail: Array<string> = [];
  listPhone: Array<string> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private customerService: CustomerService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private formBuilder: FormBuilder
  ) {
    if (this.config.data) {
      this.isPotentialCustomer = this.config.data.isPotentialCustomer;
    }
  }

  async ngOnInit() {
    this.initTable();
    this.getDataFromCustomerImportComponent();
    await this.getMasterdata();
    this.checkStatus(true);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getDataFromCustomerImportComponent() {
    this.listCustomerImport = this.config.data.listCustomerImport;
    //mapping các giá trị nếu từ excel
    this.listCustomerImport.forEach(e => {
      /* gender */
      if (e.genderOfContact === '0') {
        /* nữ */
        e.selectedGenderOfContact = this.listGender.find(e => e.code == 'NU')
      } else if (e.genderOfContact === '1') {
        /* nam */
        e.selectedGenderOfContact = this.listGender.find(e => e.code == 'NAM')
      } else {
        e.selectedGenderOfContact = null;
      }
    });
  }

  initTable() {
    this.columns = [
      { field: 'firstName', header: 'Họ đệm', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'lastName', header: 'Tên', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'isEmployee', header: 'Là nhân viên', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'isVendor', header: 'Là nhà cung cấp', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'customerCode', header: 'Mã khách hàng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'isCompany', header: 'Là doanh nghiệp', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'companyName', header: 'Thuộc Công Ty', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'address', header: 'Địa chỉ', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'taxCode', header: 'Mã số thuế', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'phone', header: 'Điện thoại', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'email', header: 'Email', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'fullNameOfContact', header: 'Họ và tên NLH', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'genderOfContact', header: 'Giới tính NLH', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'addressOfContact', header: 'Địa chỉ người liên hệ', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'phoneOfContact', header: 'ĐT di động NLH', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'emailOfContact', header: 'Email người liên hệ', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'listStatus', header: 'Trạng thái', textAlign: 'left', display: 'table-cell', width: '150px' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.customerService.getCustomerImportDetaiAsync();
    this.loading = false;
    if (result.statusCode === 200) {
      this.listCustomerCompanyCode = result.listCustomerCompanyCode;
      this.listCustomerGroup = result.listCustomerGroup;
      this.listEmail = result.listEmail;
      this.listPhone = result.listPhone;
    } else {
    }
  }

  checkStatus(autoAdd: boolean) {
    this.listCustomerImport.forEach(customer => {
      customer.listStatus = [];
      customer.isValid = true;

      /* required fields */
      if (!customer.lastName?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_lastName")];
        customer.isValid = false;
      }

      if (!customer.customerCode?.trim()) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "required_code")];
        customer.isValid = false;
      }
      /* check exist in database */
      let existCode = this.listCustomerCompanyCode.find(e => (e?.trim()?.toLowerCase() === customer.customerCode?.trim()?.toLowerCase()));
      if (existCode) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "exist_code")];
        customer.isValid = false;
      }

      let existEmail = this.listEmail.find(e => (e?.trim()?.toLowerCase() === customer.email?.trim()?.toLowerCase()));
      if (existEmail) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "exist_email")];
        customer.isValid = false;
      }

      let existPhone = this.listPhone.find(e => (e?.trim()?.toLowerCase() === customer.phone?.trim()?.toLowerCase()));
      if (existPhone) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "exist_phone")];
        customer.isValid = false;
      }

      /* duplicate in form */
      let listOtherCustomer = this.listCustomerImport.filter(e => e != customer); /* lấy danh sách khách hàng trừ bản ghi đang duyệt */
      let duplicateCode = listOtherCustomer.find(e => (Boolean(customer.customerCode?.trim()?.toLowerCase()) && (e.customerCode?.trim()?.toLowerCase() === customer.customerCode?.trim()?.toLowerCase())));
      if (duplicateCode) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "duplicate_code")];
        customer.isValid = false;
      }

      let duplicateEmail = listOtherCustomer.find(e => (Boolean(customer.email?.trim()?.toLowerCase()) && (e.email?.trim() === customer.email?.trim()?.toLowerCase())));
      if (duplicateEmail) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "duplicate_email")];
        customer.isValid = false;
      }

      let duplicatePhone = listOtherCustomer.find(e => (Boolean(customer.phone?.trim()?.toLowerCase()) && (e.phone?.trim() === customer.phone?.trim()?.toLowerCase())));
      if (duplicatePhone) {
        customer.listStatus = [...customer.listStatus, this.listNote.find(e => e.code == "duplicate_phone")];
        customer.isValid = false;
      }
    });

    /* auto add to valid list */
    if (autoAdd) this.selectedCustomerImport = this.listCustomerImport.filter(e => e.isValid);
  }

  onCancel() {
    let result: ResultDialog = {
      status: false,
      statusImport: false
    };
    this.ref.close(result);
  }

  async importCustomer() {
    let listCustomer: Array<CustomerModel> = [];
    let listContact: Array<ContactModel> = [];
    let listContactAdditional: Array<ContactModel> = [];

    /* check valid list selected */
    if (this.selectedCustomerImport.length == 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Chọn danh sách cần import' };
      this.showMessage(msg);
      return;
    }

    let inValidRecord = this.selectedCustomerImport.find(e => !e.isValid);
    if (inValidRecord) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách không hợp lệ' };
      this.showMessage(msg);
      return;
    }
    this.checkStatus(false);
    this.standardizedListCustomer();

    listCustomer = this.mappingToListCustomerModel();
    listContact = this.mappingToListContactModel();
    listContactAdditional = this.mappingToListContactAdditional();

    this.loading = true;
    let result: any = await this.customerService.importListCustomer(listCustomer, listContact, listContactAdditional, this.isPotentialCustomer, this.userId);
    this.loading = false;

    if (result.statusCode === 200) {
      let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Import thành công' };
      this.showMessage(mgs);
      let result: ResultDialog = {
        status: true,
        statusImport: true
      };
      this.ref.close(result);
    } else {
      let mgs = { severity: 'error', summary: 'Thông báo', detail: 'Import thất bại' };
      this.showMessage(mgs);
      let result: ResultDialog = {
        status: false,
        statusImport: false
      };
      this.ref.close(result);
    }
  }

  standardizedListCustomer() {
    this.listCustomerImport.forEach(customer => {
      customer.firstName = customer.firstName?.trim() ?? "";
      customer.lastName = customer.lastName?.trim() ?? "";
      customer.isEmployee = customer.isEmployee ?? false;
      customer.isVendor = customer.isVendor ?? false;
      customer.customerCode = customer.customerCode?.trim() ?? "";
      customer.companyName = customer.companyName?.trim() ?? '';
      customer.address = customer.address?.trim() ?? '';
      customer.taxCode = customer.taxCode?.trim() ?? '';
      customer.phone = customer.phone?.trim() ?? '';
      customer.email = customer.email?.trim() ?? "";
      customer.fullNameOfContact = customer.fullNameOfContact?.trim() ?? "";
      customer.addressOfContact = customer.addressOfContact?.trim() ?? "";
      customer.phoneOfContact = customer.phoneOfContact?.trim() ?? "";
      customer.emailOfContact = customer.emailOfContact?.trim() ?? "";
      customer.note = customer.note?.trim() ?? "";
    });
  }

  mappingToListCustomerModel(): Array<CustomerModel> {
    let listCustomer: Array<CustomerModel> = [];
    this.selectedCustomerImport.forEach(customer => {
      let newCustomer = new CustomerModel();
      newCustomer.CustomerId = this.emptyGuid;

      newCustomer.CustomerGroupId = this.emptyGuid;

      newCustomer.CustomerName = `${customer.firstName} ${customer.lastName}`?.trim();
      newCustomer.CustomerCode = customer.customerCode;
      newCustomer.CustomerType = customer.isCompany == true ? 1 : 2;

      newCustomer.CreatedDate = new Date();
      newCustomer.CreatedById = this.userId;
      listCustomer.push(newCustomer);
    })

    return listCustomer;
  }


  mappingToListContactModel(): Array<ContactModel> {
    let listContact: Array<ContactModel> = [];
    this.selectedCustomerImport.forEach(contact => {
      let newContact = new ContactModel();
      newContact.ContactId = this.emptyGuid;
      newContact.ObjectId = this.emptyGuid;
      newContact.ObjectType = "CUS";
      if (contact.isCompany) {
        /* là công ty -> tên công ty = first name */
        newContact.FirstName = contact.lastName;
        newContact.LastName = "";
      } else {
        /* là cá nhân */
        newContact.FirstName = contact.firstName;
        newContact.LastName = contact.lastName;
      }
      newContact.Gender = '';
      newContact.Phone = contact.phone;
      newContact.Email = contact.email;
      newContact.CompanyName = contact.companyName;
      newContact.Address = contact.address;
      newContact.TaxCode = contact.taxCode;
      newContact.Note = contact.note;

      let listPosition = ['CUS'];
      if (contact.isEmployee) listPosition = [...listPosition, "VEN"];
      if (contact.isVendor) listPosition = [...listPosition, "EMP"];
      newContact.OptionPosition = listPosition.join(',');

      newContact.CreatedDate = new Date();
      newContact.CreatedById = this.userId;
      listContact.push(newContact);
    });
    return listContact;
  }


  // thông tin người liên hệ
  mappingToListContactAdditional(): Array<ContactModel> {
    let listContactAdditional: Array<ContactModel> = [];
    this.selectedCustomerImport.forEach(contact => {
      let newContact = new ContactModel();
      newContact.ContactId = this.emptyGuid;
      newContact.ObjectId = this.emptyGuid;
      newContact.ObjectType = "CUS_CON";
      newContact.FirstName = contact.fullNameOfContact;
      newContact.LastName = '';
      newContact.Gender = contact.selectedGenderOfContact ? contact.selectedGenderOfContact.code : null;
      newContact.Address = contact.addressOfContact;
      newContact.Phone = contact.phoneOfContact;
      newContact.Email = contact.emailOfContact;

      newContact.CreatedDate = new Date();
      newContact.CreatedById = this.userId;
      listContactAdditional.push(newContact);
    });
    return listContactAdditional;
  }

  //end
}

function validateString(str: string) {
  if (str === undefined) return "";
  return str.trim();
}
