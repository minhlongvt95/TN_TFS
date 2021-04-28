import { Component, OnInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import * as $ from 'jquery';

import { TranslateService } from '@ngx-translate/core';
import { VendorService } from "../../services/vendor.service";
import { ContactService } from "../../../shared/services/contact.service";
import { BankService } from "../../../shared/services/bank.service";

import { VendorModel } from "../../models/vendor.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';

import { BankpopupComponent } from "../../../shared/components/bankpopup/bankpopup.component";
import { AddVendorContactDialogComponent } from '../add-vendor-contact-dialog/add-vendor-contact-dialog.component';

class BankAccount {
  bankAccountId: string;
  objectId: string;
  objectType: string;
  bankName: string; //Tên ngân hàng
  accountNumber: string;  //Số tài khoản
  accountName: string;  //Chủ tài khoản
  branchName: string; //Chi nhánh
  bankDetail: string;  //Diễn giải
  createdById: string;
  createdDate: Date;
}

class vendorGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class paymentMethodModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class provinceModel {
  provinceId: string;
  provinceName: string;
  provinceType: string;
  provinceCode: string;
}

class districtModel {
  districtId: string;
  districtName: string;
  districtType: string;
  districtCode: string;
  provinceId: string;
}

class wardModel {
  wardId: string;
  wardCode: string;
  wardType: string;
  wardName: string;
  districtId: string;
}

class vendorModel {
  vendorId: string;
  vendorCode: string;
  vendorName: string;
  vendorGroupId: string;
  vendorGroupName: string;
  paymentId: string;
  paymentName: string;

  constructor() {
    this.vendorName = '';
  }
}

class bankAccountModel {
  bankAccountId: string;
  objectId: string;
  objectType: string;
  accountNumber: string;
  bankDetail: string;
  branchName: string;
  accountName: string;
}

class Month {
  label: string;
  result: number;
}

class contactVendorDialogModel {
  ContactId: string;
  ObjectId: string;
  ObjectType: string;
  FullName: string;
  GenderDisplay: string;
  GenderName: string;
  Role: string;
  Phone: string;
  Email: string;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

class vendorOrderModel {
  vendorOrderId: string;
  vendorOrderCode: string;
  vendorOrderDate: Date;
  createdById: string;
  createdByName: string
  statusId: string;
  statusName: string
  statusCode: string;
  amount: string;
  backgroundColorForStatus: string;
}

class contactModel {
  public contactId: string;
  public objectId: string;
  public objectType: string;
  public firstName: string;
  public lastName: string
  public gender: string
  public dateOfBirth: Date;
  public phone: string;
  public workPhone: string;
  public otherPhone: string
  public email: string
  public workEmail: string
  public otherEmail: string
  public identityId: string
  public avatarUrl: string
  public address: string
  public maritalStatusId: string;
  public countryId: string;
  public provinceId: string
  public districtId: string;
  public wardId: string;
  public postCode: string
  public websiteUrl: string
  public socialUrl: string
  public createdById: string
  public createdDate: Date
  public updatedById: string;
  public updatedDate: Date;
  public active: boolean;
  public fullName: string;
  public personInCharge: string;
  public status: string;
  public note: string;
  public role: string;
  public taxCode: string;
  public fullAddress: string;
}

class exchangeByVendorModel {
  exchangeId: string;
  exchangeType: string;
  exchangeCode: string;
  exchangeName: string;
  exchangeValue: string;
  exchangeDetail: string;
  exchangeDate: string;
  statusCode: string;
  statusName: string;
  backgroundColorForStatus: string;
}

class vendorOrderByMonthModel {
  month: number;
  amount: number;
  constructor() {
    this.month = 1;
    this.amount = 0;
  }
}

@Component({
  selector: 'app-detail-vendor',
  templateUrl: './detail-vendor.component.html',
  styleUrls: ['./detail-vendor.component.css']
})

export class DetailVendorComponent implements OnInit {
  data: any;
  options: any;
  emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  editPermission: string = "vendor/edit";
  viewPermission: string = "vendor/view";
  orderCreatePermission: string = "vendor-order/create";
  orderListPermission: string = "vendor-order";
  orderViewPermission: string = "vendor-order/view";
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionEdit: boolean = true;
  actionDelete: boolean = true;

  vendorId: string = '';
  contactId: string = '';
  optionsLine: any = '';
  //master data
  listVendorGroup: Array<vendorGroupModel> = [];
  listPaymentMethod: Array<paymentMethodModel> = [];
  listProvince: Array<provinceModel> = [];
  listDistrict: Array<districtModel> = [];
  listCurrentDistrict: Array<districtModel> = [];
  listWard: Array<wardModel> = [];
  listCurrentWard: Array<wardModel> = [];
  listVendorCode: Array<string> = [];
  listVendorOrderByMonth: Array<vendorOrderByMonthModel> = []; //tổng đặt sp/dv
  listVendorOrderInProcessByMonth: Array<vendorOrderByMonthModel> = []; //tổng đơn hàng đang xử lý
  listReceivableByMonth: Array<vendorOrderByMonthModel> = []; //nợ phải trả
  //data by vendor id
  vendorModel: vendorModel = new vendorModel();
  contactModel: contactModel = new contactModel();
  listBankAccount: Array<bankAccountModel> = [];
  listVendorContact: Array<contactModel> = [];
  listVendorOrder: Array<vendorOrderModel> = [];
  listExchange: Array<exchangeByVendorModel> = [];
  //toggle value
  isViewVendorInfor: boolean = true;

  listMonth: Array<Month> = [
    {
      label: 'Tháng 1', result: 1
    },
    {
      label: 'Tháng 2', result: 2
    },
    {
      label: 'Tháng 3', result: 3
    },
    {
      label: 'Tháng 4', result: 4
    },
    {
      label: 'Tháng 5', result: 5
    },
    {
      label: 'Tháng 6', result: 6
    },
    {
      label: 'Tháng 7', result: 7
    },
    {
      label: 'Tháng 8', result: 8
    },
    {
      label: 'Tháng 9', result: 9
    },
    {
      label: 'Tháng 10', result: 10
    },
    {
      label: 'Tháng 11', result: 11
    },
    {
      label: 'Tháng 12', result: 12
    }
  ];

  //chart
  selectedMonth: Month = this.listMonth.find(e => e.result == new Date().getMonth() + 1);
  vendorOrderByMonth: vendorOrderByMonthModel = new vendorOrderByMonthModel(); //Tổng đặt sp/ dv
  preVendorOrderByMonth: vendorOrderByMonthModel = new vendorOrderByMonthModel();//Tổng đặt tháng trước
  receivableByMonth: vendorOrderByMonthModel = new vendorOrderByMonthModel(); //Nợ phải trả
  preReceivableByMonth: vendorOrderByMonthModel = new vendorOrderByMonthModel();//Nợ phải trả tháng trước
  vendorOrderInProcessByMonth: vendorOrderByMonthModel = new vendorOrderByMonthModel(); //Đang xử lý
  preVendorOrderInProcessByMonth: vendorOrderByMonthModel = new vendorOrderByMonthModel();//Đang xử lý tháng trước
  //table
  columns: Array<any> = [];
  selectedColumns: Array<any> = [];
  listContact: Array<contactVendorDialogModel> = [];
  rows: number = 10;
  selectedContact: ContactModel;

  bankAccountColumns: Array<any> = [];
  selectedBankColumns: Array<any> = [];
  selectedBankAccount: bankAccountModel;

  orderColumns: Array<any> = [];
  selectedOrderColumns: Array<any> = [];
  selectedOrder: vendorOrderModel;

  exchangeColumns: Array<any> = [];
  selectedExchangeColumns: Array<any> = [];
  selectedExchange: exchangeByVendorModel;

  //form
  editVendorForm: FormGroup;

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private vendorService: VendorService,
    private route: ActivatedRoute,
    private bankService: BankService,
    private contactService: ContactService,
    private confirmationService: ConfirmationService,
    private router: Router,
    public builder: FormBuilder,
    private el: ElementRef,
    private dialogService: DialogService,
    private messageService: MessageService) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    let resource = "buy/vendor/detail/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      } if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }

      this.route.params.subscribe(params => { this.vendorId = params['vendorId']; this.contactId = params['contactId']; });
      this.initForm();
      this.initTable();
      this.getMasterData();

      this.options = {
        title: {
          display: true,
          text: 'My Title',
          fontSize: 16
        },
        legend: {
          position: 'bottom'
        }
      };

      this.listMonth = this.listMonth.filter(e => e.result <= new Date().getMonth() + 1);
    }
  }

  fixed: boolean = false;
  withFiexd: string = "";
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  initForm() {
    this.editVendorForm = new FormGroup({
      'VendorGroup': new FormControl(null, [Validators.required]),
      'VendorCode': new FormControl('', [Validators.required, checkBlankString()]),
      'VendorName': new FormControl('', [Validators.required, checkBlankString()]),
      'Email': new FormControl('', [Validators.pattern(this.emailPattern)]),
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      'Location': new FormControl("1"),
      'Province': new FormControl(null),
      'District': new FormControl(null),
      'Ward': new FormControl(null),
      'Address': new FormControl(''),
      'PaymentMethod': new FormControl(null, [Validators.required]),
      'Link': new FormControl(''),
    });
  }

  initTable() {
    this.columns = [
      { field: 'FullName', header: 'Họ tên', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'GenderName', header: 'Giới tính', width: '100px', textAlign: 'center', color: '#f44336' },
      { field: 'Role', header: 'Chức vụ', width: '100px', textAlign: 'left', color: '#f44336' },
      { field: 'Phone', header: 'Số điện thoại', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'Email', header: 'Email', width: '200px', textAlign: 'left', color: '#f44336' }
    ];
    this.selectedColumns = this.columns;

    this.bankAccountColumns = [
      { field: 'bankName', header: 'Ngân hàng', textAlign: 'left', color: '#f44336' },
      { field: 'accountNumber', header: 'Số tài khoản', width: '25%', textAlign: 'right', color: '#f44336' },
      { field: 'accountName', header: 'Người thụ hưởng', width: '25%', textAlign: 'left', color: '#f44336' },
      { field: 'branchName', header: 'Chi nhánh', textAlign: 'left', color: '#f44336' },
    ];
    this.selectedBankColumns = this.bankAccountColumns;

    this.orderColumns = [
      { field: 'vendorOrderCode', header: 'Mã đơn hàng', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'createdByName', header: 'Người tạo', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'vendorOrderDate', header: 'Ngày tạo', width: '100px', textAlign: 'right', color: '#f44336' },
      { field: 'statusName', header: 'Trạng thái', width: '120px', textAlign: 'center', color: '#f44336' },
      { field: 'amount', header: 'Tổng giá trị', width: '120px', textAlign: 'right', color: '#f44336' },
    ];
    this.selectedOrderColumns = this.orderColumns;

    this.exchangeColumns = [
      { field: 'exchangeCode', header: 'Mã giao dịch', width: '160px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeDate', header: 'Ngày giao dịch', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'exchangeName', header: 'Loại giao dịch', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'statusName', header: 'Trạng thái', width: '120px', textAlign: 'center', color: '#f44336' },
      { field: 'exchangeValue', header: 'Tổng giá trị', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'exchangeDetail', header: 'Chi tiết nội dung', width: '300px', textAlign: 'left', color: '#f44336' }
    ];
    // this.selectedExchangeColumns = this.exchangeColumns;
    
    // Set mặc định số lượng column trong table để không hiên scrollbar
    this.selectedExchangeColumns = [
      { field: 'exchangeCode', header: 'Mã giao dịch', width: '160px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeDate', header: 'Ngày giao dịch', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'statusName', header: 'Trạng thái', width: '120px', textAlign: 'center', color: '#f44336' },
      { field: 'exchangeValue', header: 'Tổng giá trị', width: '150px', textAlign: 'right', color: '#f44336' },
    ];
    
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  goBackToList() {
    this.router.navigate(['/vendor/list']);
  }

  goToVendorOrder() {
    this.router.navigate(['/vendor/create-order', { vendorId: this.vendorId, contactId: this.contactId }]);
  }

  goToVenderOrderDetail(rowData: vendorOrderModel) {
    this.router.navigate(['/vendor/detail-order', { vendorOrderId: rowData.vendorOrderId }]);
  }

  goToListVendorOrder() {
    this.router.navigate(['/vendor/list-order']);
  }

  changeChooseMonth(event: any) {
    let chooseMonth: Month = event.value;
    this.calculateStatisticByMonth(chooseMonth.result);
  }

  goToListExchange() {
    // this.router.navigate(['/accounting/receivable-vendor-report']);
    // đến chi tiết công nợ nhà cung cấp
    this.router.navigate(['/accounting/receivable-vendor-detail', this.vendorId]);
  }

  goToExchangeDetail(rowData: exchangeByVendorModel) {
    switch (rowData.exchangeType) {
      case "UNC":
        //ủy nhiệm chi
        this.router.navigate(['/accounting/bank-payments-detail', rowData.exchangeId]);
        break;
      case "PC":
        //phiếu chi
        this.router.navigate(['/accounting/cash-payments-view/', rowData.exchangeId]);
        break;
      case "PT":
        //phiếu chi
        this.router.navigate(['/accounting/cash-receipts-view/', rowData.exchangeId]);
        break;
      case "BC":
        //báo có
        this.router.navigate(['/accounting/bank-receipts-detail/', rowData.exchangeId]);
        break;
      default:
        break;
    }
  }

  changeProvince(event: any): boolean {
    //reset list district and ward
    this.listCurrentDistrict = [];
    this.listCurrentWard = [];
    this.editVendorForm.get('District').setValue(null);
    this.editVendorForm.get('Ward').setValue(null);
    if (event.value === null) return false;
    let currentProvince: provinceModel = event.value;
    this.listCurrentDistrict = this.listDistrict.filter(e => e.provinceId === currentProvince.provinceId);
    return false;
  }

  changeDistrict(event: any): boolean {
    //reset ward
    this.listCurrentWard = [];
    this.editVendorForm.get('Ward').setValue(null);
    if (event.value === null) return false;
    let currentDistrict: districtModel = event.value;
    this.listCurrentWard = this.listWard.filter(e => e.districtId === currentDistrict.districtId);
    return false
  }

  addVendorContact() {
    let ref = this.dialogService.open(AddVendorContactDialogComponent, {
      header: 'Thêm người liên hệ',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          let newContact: contactVendorDialogModel = result.contactModel;
          let newVendorContact: ContactModel = new ContactModel();
          newVendorContact.ContactId = this.emptyGuid;
          newVendorContact.ObjectId = this.vendorId;
          newVendorContact.ObjectType = "VEN_CON";
          newVendorContact.FirstName = newContact.FullName;
          newVendorContact.LastName = '';
          newVendorContact.Gender = newContact.GenderDisplay;
          newVendorContact.GenderDisplay = newContact.GenderName;
          newVendorContact.Phone = newContact.Phone;
          newVendorContact.Email = newContact.Email;
          newVendorContact.Role = newContact.Role;
          newVendorContact.Active = true;
          newVendorContact.CreatedById = this.auth.UserId;
          newVendorContact.CreatedDate = new Date();

          this.loading = true;
          let createResponse: any = await this.vendorService.createVendorContact(newVendorContact, false, this.auth.UserId);
          this.loading = false;
          if (createResponse.statusCode === 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Tạo người liên hệ thành công');
            newContact.ContactId = createResponse.contactId;
            newContact.ObjectId = this.vendorId;
            newContact.ObjectType = 'VEN_CON';
            newContact.CreatedById = this.auth.UserId;
            newContact.CreatedDate = new Date();
            this.listContact = [...this.listContact, newContact];
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', createResponse.messageCode);
          }
        }
      }
    });
  }

  editContact(rowData: contactVendorDialogModel) {
    let ref = this.dialogService.open(AddVendorContactDialogComponent, {
      header: 'Chỉnh sửa người liên hệ',
      data: {
        isEdit: true,
        contact: rowData
      },
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "300px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          let newContact: contactVendorDialogModel = result.contactModel;
          let newVendorContact: ContactModel = new ContactModel();
          newVendorContact.ContactId = this.emptyGuid;
          newVendorContact.ObjectId = this.vendorId;
          newVendorContact.ObjectType = "VEN_CON";
          newVendorContact.FirstName = newContact.FullName;
          newVendorContact.LastName = '';
          newVendorContact.Gender = newContact.GenderDisplay;
          newVendorContact.GenderDisplay = newContact.GenderName;
          newVendorContact.Phone = newContact.Phone;
          newVendorContact.Email = newContact.Email;
          newVendorContact.Role = newContact.Role;
          newVendorContact.Active = true;
          newVendorContact.CreatedById = this.auth.UserId;
          newVendorContact.CreatedDate = new Date();

          this.loading = true;
          let createResponse: any = await this.vendorService.createVendorContact(newVendorContact, true, this.auth.UserId);
          this.loading = false;
          if (createResponse.statusCode === 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Chỉnh sửa người liên hệ thành công');
            newContact.ContactId = createResponse.contactId;
            newContact.ObjectId = this.vendorId;
            newContact.ObjectType = 'VEN_CON';
            newContact.CreatedById = this.auth.UserId;
            newContact.CreatedDate = new Date();
            //replace rowdata cũ
            const index = this.listContact.indexOf(rowData);
            this.listContact[index] = newContact;
            this.listContact = [...this.listContact];
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', createResponse.messageCode);
          }
        }
      }
    });
  }

  deleteContact(rowData: contactVendorDialogModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.contactService.deleteContactById(rowData.ContactId, rowData.ObjectId, "VEN_CON").subscribe(response => {
          this.loading = false;
          const result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listContact = this.listContact.filter(e => e != rowData);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa người liên hệ thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  openCreateBankDialog(mode: string, bank: BankAccount) {
    if (mode == 'create') {
      let bankAccount: BankAccount = {
        bankAccountId: null,
        objectId: this.vendorId,
        objectType: "VEN",
        bankName: "", //Tên ngân hàng
        accountNumber: "",  //Số tài khoản
        accountName: "",  //Chủ tài khoản
        branchName: "", //Chi nhánh
        bankDetail: "",  //Diễn giải
        createdById: this.emptyGuid,
        createdDate: new Date()
      };

      let ref = this.dialogService.open(BankpopupComponent, {
        data: {
          bankAccount: bankAccount,
        },
        header: 'Thêm thông tin thanh toán',
        width: '55%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "190px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result) {
          if (result.status) {
            //listBankAccount
            this.listBankAccount = result.listBankAccount;
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Tạo mới thành công');
          }
        }
      });
    } else {
      let bankAccount: BankAccount = {
        bankAccountId: bank.bankAccountId,
        objectId: this.vendorId,
        objectType: bank.objectType,
        bankName: bank.bankName, //Tên ngân hàng
        accountNumber: bank.accountNumber,  //Số tài khoản
        accountName: bank.accountName,  //Chủ tài khoản
        branchName: bank.branchName, //Chi nhánh
        bankDetail: bank.bankDetail,  //Diễn giải
        createdById: bank.createdById,
        createdDate: bank.createdDate
      };

      let ref = this.dialogService.open(BankpopupComponent, {
        data: {
          bankAccount: bankAccount,
        },
        header: 'Cập nhật thông tin thanh toán',
        width: '55%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "190px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result) {
          if (result.status) {
            this.listBankAccount = result.listBankAccount;
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Cập nhật thành công');
          }
        }
      });
    }
  }

  deleteBankById(bank: BankAccount) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.bankService.deleteBankById(bank.bankAccountId, bank.objectId, bank.objectType).subscribe(response => {
          this.loading = false;
          const result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listBankAccount = this.listBankAccount.filter(e => e != bank);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }

  toggleViewVisitCard() {
    this.isViewVendorInfor = !this.isViewVendorInfor;
  }

  async getMasterData() {
    this.loading = true;
    let [masterdataResponse, vendorDataResponse, vendorOrderResponse]: any = await Promise.all([
      this.vendorService.getDataEditVendor(this.vendorId, this.auth.UserId),
      this.vendorService.getVendorByIdAsync(this.vendorId, this.contactId, this.auth.UserId),
      this.vendorService.searchVendorOrderAsync([this.vendorId], '', null, null, [], this.auth.UserId)
    ]);
    this.loading = false;
    if (masterdataResponse.statusCode === 200 && vendorDataResponse.statusCode === 200 && vendorOrderResponse.statusCode === 200) {
      //master data
      this.listProvince = masterdataResponse.listProvince;
      this.listDistrict = masterdataResponse.listDistrict;
      this.listWard = masterdataResponse.listWard;
      this.listVendorGroup = masterdataResponse.listVendorGroup;
      this.listVendorCode = masterdataResponse.listVendorCode;
      this.listPaymentMethod = masterdataResponse.listPaymentMethod;
      this.listVendorOrderByMonth = masterdataResponse.listVendorOrderByMonth;
      this.listVendorOrderInProcessByMonth = masterdataResponse.listVendorOrderInProcessByMonth;
      this.listReceivableByMonth = masterdataResponse.listReceivableByMonth;
      this.handdleChart();
      //data by vendor id
      this.vendorModel = vendorDataResponse.vendor;
      this.contactModel = vendorDataResponse.contact;
      this.listExchange = vendorDataResponse.listExchangeByVendor;
      this.handleBackgroundColorExchange();
      this.contactModel.fullAddress = this.fullAddressBuilder(this.contactModel.address, this.contactModel.provinceId, this.contactModel.districtId, this.contactModel.wardId);
      this.mapDataToForm(this.vendorModel, this.contactModel);

      this.listContact = this.getListContact(vendorDataResponse.vendorContactList);
      this.listBankAccount = vendorDataResponse.vendorBankAccountList;
      this.listVendorOrder = vendorOrderResponse.vendorOrderList;

      this.handleBackGroundColorForStatus();
    } else if (masterdataResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', masterdataResponse.messageCode);
    } else if (vendorDataResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', vendorDataResponse.messageCode);
    } else if (vendorOrderResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', vendorOrderResponse.messageCode);
    }
  }

  handdleChart() {
    //round numbers
    this.listVendorOrderByMonth.forEach(e => {
      e.amount = this.roundNumber(e.amount, parseInt(this.defaultNumberType, 10));
    });

    this.listVendorOrderInProcessByMonth.forEach(e => {
      e.amount = this.roundNumber(e.amount, parseInt(this.defaultNumberType, 10));
    });

    this.listReceivableByMonth.forEach(e => {
      e.amount = this.roundNumber(e.amount, parseInt(this.defaultNumberType, 10));
    });

    let currentMonth = new Date().getMonth() + 1;
    let listMonth = this.listMonth.filter(e => e.result <= currentMonth).map(e => e.label);

    this.calculateStatisticByMonth(currentMonth);

    let listVendorOrderByMonth = this.listVendorOrderByMonth.filter(e => e.month <= currentMonth).map(e => e.amount);
    let listVendorOrderInProcessByMonth = this.listVendorOrderInProcessByMonth.filter(e => e.month <= currentMonth).map(e => e.amount);
    let listReceivableByMonth = this.listReceivableByMonth.filter(e => e.month <= currentMonth).map(e => e.amount);

    this.data = {
      labels: listMonth,
      datasets: [
        {
          label: 'Tổng đặt SP/DV',
          data: listVendorOrderByMonth,
          borderColor: '#007aff',
          backgroundColor: '#007aff',
          borderWidth: 2,
          fill : false,
        },
        {
          label: 'Đang xử lý',
          data: listVendorOrderInProcessByMonth,
          borderColor: '#ff3b30',
          backgroundColor: '#ff3b30',
          borderWidth: 2,
          fill : false,
        },
        {
          label: 'Nợ phải trả',
          data: listReceivableByMonth,
          borderColor: '#ff9500',
          backgroundColor: '#ff9500',
          borderWidth: 2,
          fill : false,
        }
      ]
    }

    this.optionsLine = {
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true,
            callback: function(value, index, values) {
              if((parseInt(value) >= 1000000 && parseInt(value) < 1000000000) || (parseInt(value) <= -1000000 && parseInt(value) > -1000000000)){
                let tempValue = Number(value) / 1000000;
                // return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                return tempValue.toString() + "M";
              } else if(Number(value) >= 1000000000 || Number(value) <= -1000000000) {
                let tempValue = Number(value) / 1000000000;
                return tempValue.toString() + "B";
              }else{
                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
              }
            }
          }
        }]
      },
      tooltips: {
        callbacks: {
          label: function (tooltipItems, data) {
            return data.datasets[tooltipItems.datasetIndex].label + ': ' + (<number>data.datasets[tooltipItems.datasetIndex].data[tooltipItems.index]).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
          }
        }
      }
    }
  }

  /* tính tổng đặt SP/DV, nợ phải trả, đang xử lý */
  calculateStatisticByMonth(month: number) {
    /* tháng hiện tại */
    this.vendorOrderByMonth = this.listVendorOrderByMonth.find(e => e.month == month);
    this.receivableByMonth = this.listReceivableByMonth.find(e => e.month == month);
    this.vendorOrderInProcessByMonth = this.listVendorOrderInProcessByMonth.find(e => e.month == month);
    /*  tháng trước */
    if (month == 1) return;

    this.preVendorOrderByMonth = this.listVendorOrderByMonth.find(e => e.month == month - 1);
    this.preReceivableByMonth = this.listReceivableByMonth.find(e => e.month == month - 1);
    this.preVendorOrderInProcessByMonth = this.listVendorOrderInProcessByMonth.find(e => e.month == month - 1);
  }

  mapDataToForm(vendorModel: vendorModel, contactModel: contactModel) {
    let _vendorGroup = this.listVendorGroup.find(e => e.categoryId == vendorModel.vendorGroupId);
    this.editVendorForm.get('VendorGroup').patchValue(_vendorGroup ? _vendorGroup : null);
    this.editVendorForm.get('VendorCode').patchValue(vendorModel.vendorCode ? vendorModel.vendorCode : '');
    this.editVendorForm.get('VendorName').patchValue(vendorModel.vendorName ? vendorModel.vendorName : '');
    this.editVendorForm.get('Email').patchValue(contactModel.email ? contactModel.email : '');
    this.editVendorForm.get('Phone').patchValue(contactModel.phone ? contactModel.phone : '');
    let _paymentMethod = this.listPaymentMethod.find(e => e.categoryId == vendorModel.paymentId);
    this.editVendorForm.get('PaymentMethod').patchValue(_paymentMethod ? _paymentMethod : null);
    let _province = this.listProvince.find(e => e.provinceId == contactModel.provinceId);
    if (_province) {
      this.listCurrentDistrict = this.listDistrict.filter(e => e.provinceId === _province.provinceId);
    } else {
      this.listCurrentDistrict = [];
    }
    let _district = this.listCurrentDistrict.find(e => e.districtId == contactModel.districtId);
    if (_district) {
      this.listCurrentWard = this.listWard.filter(e => e.districtId === _district.districtId);
    } else {
      this.listCurrentWard = [];
    }
    let _ward = this.listCurrentWard.find(e => e.wardId == contactModel.wardId);
    this.editVendorForm.get('Province').patchValue(_province ? _province : null);
    this.editVendorForm.get('District').patchValue(_district ? _district : null);
    this.editVendorForm.get('Ward').patchValue(_ward ? _ward : null);
    this.editVendorForm.get('Address').patchValue(contactModel.address ? contactModel.address : '');
    this.editVendorForm.get('Link').patchValue(contactModel.socialUrl ? contactModel.socialUrl : '');
  }

  editVendorInfor() {
    if (!this.editVendorForm.valid) {
      Object.keys(this.editVendorForm.controls).forEach(key => {
        if (!this.editVendorForm.controls[key].valid) {
          this.editVendorForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      let vendorModel: VendorModel = this.mapFormToVendorModel();
      let contactModel: ContactModel = this.mapFormToContactModel();
      this.loading = true;
      this.vendorService.updateVendorById(vendorModel, contactModel, this.auth.Userid).subscribe(response => {
        this.loading = false;
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.editVendorViewModel(vendorModel, contactModel);
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Chỉnh sửa nhà cung cấp thành công');
          this.isViewVendorInfor = true;
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      }, error => { this.loading = false; });
    }
  }

  mapFormToVendorModel(): VendorModel {
    let vendorModel = new VendorModel();
    vendorModel.VendorId = this.vendorId;
    vendorModel.VendorName = this.editVendorForm.get('VendorName').value;
    vendorModel.VendorCode = this.editVendorForm.get('VendorCode').value;
    vendorModel.VendorGroupId = this.editVendorForm.get('VendorGroup').value.categoryId;
    vendorModel.PaymentId = this.editVendorForm.get('PaymentMethod').value.categoryId;
    vendorModel.Active = true;
    vendorModel.CreatedById = this.auth.UserId;
    vendorModel.CreatedDate = new Date();
    vendorModel.UpdatedById = null;
    vendorModel.UpdatedDate = null;

    return vendorModel;
  }

  mapFormToContactModel(): ContactModel {
    let contactModel = new ContactModel();
    contactModel.ContactId = this.contactId;
    contactModel.ObjectId = this.vendorId;
    contactModel.ObjectType = "VEN";
    contactModel.Email = this.editVendorForm.get('Email').value;
    contactModel.Phone = this.editVendorForm.get('Phone').value;
    let _province = this.editVendorForm.get('Province').value;
    let _district = this.editVendorForm.get('District').value;
    let _ward = this.editVendorForm.get('Ward').value;
    contactModel.ProvinceId = _province !== null ? _province.provinceId : null;
    contactModel.DistrictId = _district !== null ? _district.districtId : null;
    contactModel.WardId = _ward !== null ? _ward.wardId : null;
    contactModel.Address = this.editVendorForm.get('Address').value;
    contactModel.SocialUrl = this.editVendorForm.get('Link').value;

    contactModel.Active = true;
    contactModel.CreatedById = this.auth.UserId;
    contactModel.CreatedDate = new Date();
    contactModel.UpdatedById = null;
    contactModel.UpdatedDate = null;
    return contactModel;
  }

  fullAddressBuilder(address, provinceId, districtId, wardId): string {
    let arr: Array<string> = [];
    if (address) arr = [...arr, address];
    let _ward = this.listWard.find(e => e.wardId == wardId);
    if (_ward) arr = [...arr, `${_ward.wardType} ${_ward.wardName}`];
    let _district = this.listDistrict.find(e => e.districtId == districtId);
    if (_district) arr = [...arr, `${_district.districtType} ${_district.districtName}`];
    let _province = this.listProvince.find(e => e.provinceId == provinceId);
    if (_province) arr = [...arr, `${_province.provinceType} ${_province.provinceName}`];
    return arr.join(', ');
  }

  editVendorViewModel(vendorModel: VendorModel, contactModel: ContactModel) {
    this.vendorModel.vendorName = vendorModel.VendorName;
    this.vendorModel.vendorCode = vendorModel.VendorCode;
    this.contactModel.phone = contactModel.Phone;
    this.contactModel.email = contactModel.Email;
    this.contactModel.fullAddress = this.fullAddressBuilder(contactModel.Address, contactModel.ProvinceId, contactModel.DistrictId, contactModel.WardId);
    let _payment = this.listPaymentMethod.find(e => e.categoryId == vendorModel.PaymentId);
    this.vendorModel.paymentName = _payment ? _payment.categoryName : '';
    this.contactModel.socialUrl = contactModel.SocialUrl;
  }

  getListContact(listContact: Array<contactModel>) {
    let listContactDialog: Array<contactVendorDialogModel> = [];
    listContact.forEach(contact => {
      let newContact: contactVendorDialogModel = new contactVendorDialogModel();
      newContact.ContactId = contact.contactId;
      newContact.ObjectId = contact.objectId;
      newContact.ObjectType = contact.objectType;
      newContact.FullName = contact.firstName;
      newContact.GenderDisplay = contact.gender;

      newContact.Role = contact.role;
      newContact.Email = contact.email;
      newContact.Phone = contact.phone;
      newContact.CreatedDate = contact.createdDate;
      newContact.CreatedById = contact.createdById;
      newContact.UpdatedDate = contact.updatedDate;
      newContact.UpdatedById = contact.updatedById;

      if (contact.gender == "NU") {
        newContact.GenderName = "Bà";
      } else {
        newContact.GenderName = "Ông";
      }

      listContactDialog.push(newContact)
    });
    return listContactDialog;
  }

  handleBackGroundColorForStatus() {
    this.listVendorOrder.forEach(vendorOrder => {
      switch (vendorOrder.statusCode) {
        case "ON": //hoan
          vendorOrder.backgroundColorForStatus = "#a2a9b0";
          break;
        case "IP":
          vendorOrder.backgroundColorForStatus = "#ffcc00";
          break;
        case "COMP":
          vendorOrder.backgroundColorForStatus = "#5ac8fa";
          break;
        case "RTN":
          vendorOrder.backgroundColorForStatus = "#4d5358";
          break;
        case "DXK":
          vendorOrder.backgroundColorForStatus = "#34c759";
          break;
        case "DRA":
          vendorOrder.backgroundColorForStatus = "#ff9500";
          break;
        case "DNK":
          vendorOrder.backgroundColorForStatus = "#34c759";
          break;
        case "PD":
          vendorOrder.backgroundColorForStatus = "#34c759";
          break;
        case "DLV":
          vendorOrder.backgroundColorForStatus = "#34c759";
          break;
        default:
          vendorOrder.backgroundColorForStatus = "#878d96";
          break;
      }
    });
  }

  handleBackgroundColorExchange() {
    this.listExchange.forEach(exchange => {
      switch (exchange.statusCode) {
        case "CSO"://chưa vào sổ
          exchange.backgroundColorForStatus = "#ffcc00";
          break;
        case "DSO":
          exchange.backgroundColorForStatus = "#5ac8fa";
          break;
        default:
          exchange.backgroundColorForStatus = "#5ac8fa";
          break;
      }
    });
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case 0: {
        result = result;
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }
}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateCode = array.find(e => e.trim().toLowerCase() === control.value.trim().toLowerCase());
        if (duplicateCode !== undefined) {
          return { 'duplicateCode': true };
        }
      }
    }
    return null;
  }
}

function checkBlankString(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() === "") {
        return { 'blankString': true };
      }
    }
    return null;
  }
}
