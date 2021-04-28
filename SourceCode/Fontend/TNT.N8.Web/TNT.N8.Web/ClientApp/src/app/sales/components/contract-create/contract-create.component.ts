import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { Contract, ContractCostDetail, ContractDetail, ContractProductDetailProductAttributeValue } from '../../models/contract.model';

import * as $ from 'jquery';
//
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { MenuItem } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';

//DIALOG COMPONENT
import { GetPermission } from '../../../shared/permission/get-permission';
import { ContractService } from '../../services/contract.service';
import { AddEditProductContractDialogComponent } from '../add-edit-product-contract-dialog/add-edit-product-contract-dialog.component';
import { LeadService } from '../../../lead/services/lead.service';
import { AddEditContractCostDialogComponent } from '../add-edit-contract-cost-dialog/add-edit-contract-cost-dialog.component';

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
  personInChargeId: string;
}

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface BankAccount {
  bankAccountId: string;
  objectId: string;
  bankName: string; //Ngân hàng
  accountNumber: string;  //Số tài khoản
  branchName: string; //Chi nhánh
  accountName: string; //Chủ tài khoản
  objectType: string;
}

interface Quote {
  quoteId: string;
  quoteCode: string;
  quoteDate: Date;
  sendQuoteDate: Date;
  seller: string;
  effectiveQuoteDate: number;
  expirationDate: Date;
  description: string;
  note: string;
  objectTypeId: string;
  objectType: string;
  customerContactId: string;
  paymentMethod: string;
  discountType: boolean;
  bankAccountId: string;
  daysAreOwed: number;
  maxDebt: number;
  receivedDate: Date;
  amount: number;
  discountValue: number;
  intendedQuoteDate: Date;
  statusId: string;
  createdDate: Date;
  personInChargeId: string;
  sellerName: string;
  leadId: string;
  leadCode: string;
  saleBiddingId: string;
  saleBiddingCode: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  contractDetailModel: ContractDetail,
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  contractCostModel: ContractCostDetail,
}

interface AdditionalInformation {
  ordinal: number,
  additionalInformationId: string,
  objectId: string,
  objectType: string,
  title: string,
  content: string,
  active: boolean,
  createdDate: Date,
  createdById: string,
  updatedDate: Date,
  updatedById: string,
  orderNumber: number
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
}

@Component({
  selector: 'app-contract-create',
  templateUrl: './contract-create.component.html',
  styleUrls: ['./contract-create.component.css']
})
export class ContractCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  innerWidth: number = 0; //number window size first

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }
  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString; //Số chữ số thập phân sau dấu phẩy
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  isManager: boolean = localStorage.getItem('IsManager') == "true" ? true : false;
  /*End*/

  /*Get Current EmployeeId*/
  auth = JSON.parse(localStorage.getItem('auth'));
  currentEmployeeId = this.auth.EmployeeId;  //employeeId của người đang đăng nhập
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  /*End*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  isShowButtonSave: boolean = true;
  isPersonInCharge: boolean = false;
  isShowButton: boolean = true;
  /*End*/
  selectedItem: any;
  // Form đối tượng
  contractForm: FormGroup;
  objectControl: FormControl; //Đối tượng trong hợp đồng
  paymentMethodControl: FormControl;  //Phương thức thanh toán
  bankAccountControl: FormControl;  //Tài khoản ngân hàng
  contractStatusControl: FormControl;  //Trạng thái
  quoteControl: FormControl; //Tên báo giá
  effectiveDateControl: FormControl; // Ngày hiệu lực
  expiredDateControl: FormControl; // Ngày hết hạn
  contractTimeControl: FormControl; // thời gian hợp đồng
  contractTimeUnitControl: FormControl; // đơn vị
  valueContractControl: FormControl; // Giá trị hợp đồng
  mainContractControl: FormControl; // Hợp đồng chính
  typeContractControl: FormControl; // Loại hợp đồng
  descriptionControl: FormControl;  //Diễn giải
  empSalerControl: FormControl; //Nhân viên bán hàng
  noteControl: FormControl; //Ghi chú
  discountTypeControl: FormControl; //Loại chiết khấu (% - Số tiền)
  discountValueControl: FormControl; //Giá trị tổng chiết khấu của báo giá
  unitMoneyControl: FormControl; //Giá trị tổng chiết khấu của báo giá
  exchangeRateControl: FormControl; //Giá trị tổng chiết khấu của báo giá
  listQuoteCostDetail: Array<any> = [];
  cols: any[];
  selectedColumns: any[];
  colsNote: any[];
  colsCost: any[];
  selectedColumnsCost: any[];
  listContractTimeUnit: Array<any> = [];
  AIUpdate: AdditionalInformation = {
    ordinal: null,
    additionalInformationId: this.emptyGuid,
    objectId: this.emptyGuid,
    objectType: '',
    title: '',
    content: '',
    active: true,
    createdDate: new Date(),
    createdById: this.emptyGuid,
    updatedDate: null,
    updatedById: null,
    orderNumber: null
  };
  listAdditionalInformation: Array<AdditionalInformation> = [];
  file: File[];
  listFile: Array<FileInFolder> = [];

  titleText: string = '';
  contentText: string = '';
  isUpdateAI: boolean = false;
  displayDialog: boolean = false;
  nowDate = new Date();
  maxOrdinal: number = 0;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';

  /*Biến lưu giá trị trả về*/
  activeIndex: number = 0;
  isShowWorkFollowContract: boolean = true;
  objectList = <any>[];
  listCustomer: Array<any> = [];
  dateOrder: Date = new Date();
  customerGroupId: string;
  listBankAccount: Array<BankAccount> = [];
  listBankAccountCus: Array<BankAccount> = [];
  listTypeContract: Array<Category> = [];
  listQuote: Array<Quote> = [];
  listQuoteMaster: Array<Quote> = [];
  listQuoteDetail: Array<any> = [];
  listContract: Array<any> = [];
  listEmpSale: Array<Employee> = []; //list nhân viên bán hàng
  listEmpSaleCus: Array<Employee> = [];
  listPaymentMethod: Array<Category> = []; //list Phương thức thanh toán
  listContractStatus: Array<Category> = []; //list Trạng thái của hợp đồng
  productCodeSystemList: string[] = [];
  listProductUnitName: string[] = [];
  listProduct: any[] = [];
  listUnitProduct: any[] = [];
  isRequiredMainContract: boolean = false;
  workFollowContract: MenuItem[];

  /*Valid Form*/
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  /*End*/

  // Biến
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  contractId: string = this.emptyGuid;
  quoteId: string = this.emptyGuid;
  customerId: string = this.emptyGuid;
  colLeft: number = 8;
  isShow: boolean = true;
  email: string = '';
  phone: string = '';
  fullAddress: string = '';
  taxCode: string = '';
  numberPayMentChange: string = '';
  CustomerOrderAmount: number = 0;
  DiscountValue: number = 0;
  CustomerOrderAmountAfterDiscount: number = 0;
  totalBeforeDiscount: number = 0;
  path: string = "#";
  uploadedFiles: any[] = [];
  contractCostDetailModel: ContractCostDetail;
  listContractCost: Array<ContractCostDetail> = [];
  listContractCostOrderBy: Array<ContractCostDetail> = [];

  /*Biến điều kiện*/
  minDate: Date;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  isShowBankAccount: boolean = false;
  bankName: string;
  bankAccount: string;
  bankNumber: string;
  branch: string;
  listContractDetailModel: Array<ContractDetail> = [];
  leadId: string;
  leadCode: string;
  saleBiddingId: string;
  saleBiddingCode: string;
  isRequiredCustomerOrder: boolean = false;
  isDay: boolean = true;

  contractDetailModel = new ContractDetail();



  /*MODELS*/
  contractModel: Contract = {
    ContractId: this.emptyGuid,
    ContractCode: '',
    EmployeeId: this.emptyGuid,
    EffectiveDate: new Date(),
    ExpiredDate: null,
    ContractTime: 0,
    ContractTimeUnit: '',
    MainContractId: this.emptyGuid,
    ContractTypeId: this.emptyGuid,
    ContractDescription: '',
    ContractNote: '',
    CustomerId: this.emptyGuid,
    ObjectType: '',
    QuoteId: this.emptyGuid,
    CustomerName: '',
    ValueContract: 0,
    PaymentMethodId: this.emptyGuid,
    DiscountType: true,
    BankAccountId: null,
    Amount: 0,
    DiscountValue: 0,
    StatusId: null,//Guid?
    Active: true,
    IsExtend: false,
    CreatedById: this.auth.UserId,
    CreatedDate: new Date(),
    UpdatedById: this.emptyGuid,
    UpdatedDate: new Date(),
  }

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private contractService: ContractService,
    private leadService: LeadService
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        }
      }
    });

    this.innerWidth = window.innerWidth;
    if (this.innerWidth < 768) {
      this.isShowWorkFollowContract = false;
    }
  }

  ngOnInit() {
    this.listContractTimeUnit =[
      {timeUnit: 'Ngày', code: 'DAY'},
      {timeUnit: 'Tháng', code: 'MONTH'},
      {timeUnit: 'Năm', code: 'YEAR'}
    ]
    this.setForm();
    this.route.params.subscribe(params => {
      this.quoteId = params['quoteId'];
      let resource = "sal/sales/contract-create";
      let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
      if (permission.status == false) {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        setTimeout(() => {
          this.showMessage(msg);
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 1000);
        }, 0);
      } else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("add") == -1) {
          this.actionAdd = false;
        } else {
          //Khi người dùng click vào link tạo mới báo giá thì show lại
          this.actionAdd = true;
        }
        this.actionDelete = true; //quyền xóa sản phẩm dịch vụ là mặc định có khi tạo mới báo giá
        this.actionEdit = true; //quyền sửa báo giá là mặc định khi tạo mới
      }
    });
    this.setTable();
    this.getMasterData();
  }

  setForm() {
    this.objectControl = new FormControl(null, [Validators.required]);
    this.paymentMethodControl = new FormControl(null);
    this.bankAccountControl = new FormControl(null);
    this.contractStatusControl = new FormControl(null);
    this.quoteControl = new FormControl(null);
    this.effectiveDateControl = new FormControl(new Date(), [Validators.required]);
    this.expiredDateControl = new FormControl(null);
    this.contractTimeControl = new FormControl(0);
    this.contractTimeUnitControl = new FormControl(null);
    this.valueContractControl = new FormControl('0');
    this.mainContractControl = new FormControl(null);
    this.typeContractControl = new FormControl(null, [Validators.required]);
    this.empSalerControl = new FormControl(null, [Validators.required]);
    this.descriptionControl = new FormControl('');
    this.noteControl = new FormControl('');
    this.discountTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl = new FormControl('0');
    this.unitMoneyControl = new FormControl('');
    this.exchangeRateControl = new FormControl('1');

    this.contractForm = new FormGroup({
      objectControl: this.objectControl,
      paymentMethodControl: this.paymentMethodControl,
      bankAccountControl: this.bankAccountControl,
      contractStatusControl: this.contractStatusControl,
      quoteControl: this.quoteControl,
      effectiveDateControl: this.effectiveDateControl,
      expiredDateControl: this.expiredDateControl,
      contractTimeControl: this.contractTimeControl,
      contractTimeUnitControl: this.contractTimeUnitControl,
      valueContractControl: this.valueContractControl,
      mainContractControl: this.mainContractControl,
      typeContractControl: this.typeContractControl,
      empSalerControl: this.empSalerControl,
      descriptionControl: this.descriptionControl,
      noteControl: this.noteControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl,
      unitMoneyControl: this.unitMoneyControl,
      exchangeRateControl: this.exchangeRateControl
    });
  }

  mappingForm() {
    this.contractModel.EmployeeId = this.contractForm.controls['empSalerControl'].value.employeeId;
    this.contractModel.EffectiveDate = this.contractForm.controls['effectiveDateControl'].value;
    this.contractModel.ExpiredDate = this.contractForm.controls['expiredDateControl'].value;
    this.contractModel.ContractTime = parseInt(this.contractForm.controls['contractTimeControl'].value.replace(/,/g, ''));
    this.contractModel.ContractTimeUnit = this.contractForm.controls['contractTimeUnitControl'].value.code;
    let contractDescription = this.contractForm.controls['descriptionControl'].value;
    this.contractModel.ContractDescription = contractDescription != null ? contractDescription.trim() : null;
    let contractNote = this.contractForm.controls['noteControl'].value;
    this.contractModel.ContractNote = contractNote != null ? contractNote.trim() : null;
    this.contractModel.PaymentMethodId = this.contractForm.controls['paymentMethodControl'].value.categoryId;
    this.contractModel.CustomerId = this.contractForm.controls['objectControl'].value.customerId;
    this.contractModel.ObjectType = 'CUSTOMER';
    this.contractModel.QuoteId = this.contractForm.controls['quoteControl'].value?.quoteId;
    this.contractModel.ValueContract = this.contractForm.controls['valueContractControl'].value;
    this.contractModel.ContractTypeId = this.contractForm.controls['typeContractControl'].value.categoryId;
    this.contractModel.StatusId = this.emptyGuid;
    let bankAccount = this.contractForm.controls['bankAccountControl'].value;
    this.contractModel.BankAccountId = bankAccount === null ? null : bankAccount.bankAccountId;
    this.contractModel.ValueContract = this.CustomerOrderAmountAfterDiscount;
    let mainContract = this.contractForm.controls['mainContractControl'].value;
    this.contractModel.MainContractId = mainContract === null ? null : mainContract.contractId;
    this.file = new Array<File>();
    this.uploadedFiles.forEach(item => {
      this.file.push(item);
    });
    this.uploadedFiles = [];
    this.fileUpload.files = [];
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  setTable() {
    this.cols = [
      { field: 'Move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'ExplainStr', header: 'Diễn giải', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'NameVendor', header: 'Nhà cung cấp', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'ExchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'Vat', header: 'Thuế GTGT (%)', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'DiscountValue', header: 'Chiết khấu', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = [
      { field: 'Move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'ExplainStr', header: 'Diễn giải', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'ExchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];

    this.colsNote = [
      { field: 'title', header: 'Tiêu đề', width: '20%', textAlign: 'left' },
      { field: 'content', header: 'Nội dung', width: '70%', textAlign: 'left' },
    ];

    this.colsCost = [
      { field: 'CostCode', header: 'Mã chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'CostName', header: 'Tên chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsCost = this.colsCost;
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.contractService.getMasterDataContract(this.contractId, this.quoteId);
    this.loading = false;
    this.objectList = result.listCustomer;
    this.objectList.forEach(item => {
      item.customerName = item.customerCode + ' - ' + item.customerName;
    });
    this.listPaymentMethod = result.listPaymentMethod;
    this.listTypeContract = result.listTypeContract;
    this.listQuote = result.listQuote;
    this.listQuoteMaster = result.listQuote;
    this.listEmpSale = result.listEmployeeSeller;
    this.listEmpSale.forEach(item => {
      item.employeeName = item.employeeCode + ' - ' + item.employeeName;
    });
    this.listQuoteCostDetail = result.listQuoteCostDetail;
    this.listContractStatus = result.listContractStatus;
    this.listBankAccount = result.listBankAccount;
    // this.listContract = result.listContract;
    this.listQuoteDetail = result.listQuoteDetail;
    this.setDefault();
  }

  async getListMainContractByEmp() {
    this.listContract = [];
    var emp = this.empSalerControl.value;
    if (emp != null) {
      let result: any = await this.contractService.getListMainContract(emp.employeeId);
      this.listContract = result.listContract;
      if(this.listContract.length == 0){
        let mgs = { severity: 'info', summary: 'Thông báo:', detail: 'Không có đơn hàng nào phù hợp điều kiện' };
        this.showMessage(mgs);
      }
    } else {
      this.listContract = [];
      let mgs = { severity: 'info', summary: 'Thông báo:', detail: 'Không có đơn hàng nào phù hợp điều kiện' };
      this.showMessage(mgs);
    }
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

  showTotalContract() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  mappingModelContractCostDetailFromQuoteCost(listQuoteCostDetail: Array<any>): Array<ContractCostDetail> {
    let contractCosts: Array<ContractCostDetail> = [];
    listQuoteCostDetail.forEach(item => {
      let contractCostModel = new ContractCostDetail();
      contractCostModel.CostId = item.costId;
      contractCostModel.CostCode = item.costCode;
      contractCostModel.CostName = item.costName;
      contractCostModel.Quantity = item.quantity;
      contractCostModel.UnitPrice = item.unitPrice;
      contractCostModel.CreatedById = item.createdById;
      contractCostModel.CreatedDate = item.createdDate;
      let unitPrice = parseFloat(item.unitPrice);
      let quantity = parseFloat(item.quantity);
      contractCostModel.SumAmount = unitPrice * quantity;

      contractCosts.push(contractCostModel);
    });

    return contractCosts;
  }

  setDefault() {
    this.workFollowContract = [
      {
        label: this.listContractStatus.find(x => x.categoryCode == 'MOI').categoryName
      },
      {
        label: this.listContractStatus.find(x => x.categoryCode == 'APPR').categoryName
      },
      {
        label: this.listContractStatus.find(x => x.categoryCode == 'DTH').categoryName
      },
      {
        label: this.listContractStatus.find(x => x.categoryCode == 'HTH').categoryName
      }
    ];
    this.activeIndex = 0;

    if (this.listPaymentMethod?.length > 0) {
      let paymentDefault = this.listPaymentMethod.find(x => x.isDefault == true);

      if (paymentDefault) {
        this.paymentMethodControl.setValue(paymentDefault);
      } else {
        this.paymentMethodControl.setValue(this.listPaymentMethod[0]);
      }
    }

    if (this.quoteId) {
      let quote = this.listQuote.find(c => c.quoteId == this.quoteId);
      if (quote) {
        this.quoteControl.setValue(quote);
        let customer = this.objectList.find(c => c.customerId == quote.objectTypeId);

        if (customer) {
          if (customer.personInChargeId) {
            this.leadService.getEmployeeSale(this.listEmpSale, customer.personInChargeId).subscribe(response => {
              let result: any = response;
              this.listEmpSaleCus = result.listEmployee;
              this.listEmpSaleCus.forEach(item => {
                item.employeeName = item.employeeName;
              });
              let employee = this.listEmpSaleCus.find(c => c.employeeId == quote.seller);
              if (employee) {
                this.empSalerControl.setValue(employee);
              }
            });
          } else {
            this.listEmpSaleCus = [];
          }
          this.objectControl.setValue(customer);
          this.phone = customer.customerPhone;
          this.email = customer.customerEmail;
          this.fullAddress = customer.fullAddress;
          this.taxCode = customer.taxCode;
          this.listBankAccountCus = this.listBankAccount.filter(c => c.objectId == customer.customerId);
          if (this.listBankAccountCus.length > 0) {
            this.bankAccountControl.setValue(this.listBankAccountCus[0]);
            this.bankName = this.listBankAccountCus[0].bankName;
            this.bankAccount = this.listBankAccountCus[0].accountName;
            this.bankNumber = this.listBankAccountCus[0].accountNumber;
            this.branch = this.listBankAccountCus[0].branchName;
          }

          this.customerGroupId = customer.customerGroupId;
        }

        this.leadId = quote.leadId;
        this.leadCode = quote.leadCode;
        this.saleBiddingId = quote.saleBiddingId;
        this.saleBiddingCode = quote.saleBiddingCode;
      }

      if (quote.paymentMethod) {
        let paymentMethod = this.listPaymentMethod.find(c => c.categoryId == quote.paymentMethod);
        if (paymentMethod) {
          this.contractForm.controls['paymentMethodControl'].setValue(paymentMethod);
          if (paymentMethod.categoryCode === 'BANK') {
            this.isShowBankAccount = true;
          } else {
            this.isShowBankAccount = false;
          }
        }
      }


      let listQuoteDetail = this.listQuoteDetail.filter(c => c.quoteId === this.quoteId);
      if (listQuoteDetail.length > 0) {
        this.listContractDetailModel = [];
        listQuoteDetail.forEach((item, index) => {
          let contractDetail = new ContractDetail();
          contractDetail.VendorId = item.vendorId;
          contractDetail.ProductId = item.productId;
          contractDetail.Quantity = item.quantity;
          contractDetail.UnitPrice = item.unitPrice;
          contractDetail.CurrencyUnit = item.currencyUnit;
          contractDetail.ExchangeRate = item.exchangeRate;
          contractDetail.Vat = item.vat;
          contractDetail.DiscountType = item.discountType;
          contractDetail.DiscountValue = item.discountValue;
          contractDetail.Description = item.description;
          contractDetail.OrderDetailType = item.orderDetailType;
          contractDetail.UnitId = item.unitId;
          contractDetail.IncurredUnit = item.incurredUnit;
          contractDetail.Active = item.active;
          contractDetail.OrderDetailType = item.orderDetailType;
          contractDetail.ExplainStr = item.nameProduct;
          contractDetail.NameVendor = item.nameVendor;
          contractDetail.NameProduct = item.nameProduct;
          contractDetail.ProductNameUnit = item.nameProductUnit;
          contractDetail.NameMoneyUnit = item.nameMoneyUnit;
          contractDetail.ProductName = item.productName;
          contractDetail.OrderNumber = index + 1;

          let unitPrice = parseFloat(item.unitPrice);
          let quantity = parseFloat(item.quantity);
          let exchangeRate = parseFloat(item.exchangeRate);
          let sum = unitPrice * quantity * exchangeRate;
          let discountType = item.discountType;
          let discountValue = parseFloat(item.discountValue);
          if (discountType) {
            var discount = (((quantity * unitPrice * exchangeRate) * discountValue) / 100);
            var vat = (((quantity * unitPrice * exchangeRate) - discount) * parseFloat(item.vat) / 100)
            contractDetail.SumAmount = sum - discount + vat;
          } else {
            var discount = discountValue;
            var vat = (((quantity * unitPrice * exchangeRate) - discount) * parseFloat(item.vat) / 100)
            contractDetail.SumAmount = sum - discount + vat;
          }
          item.quoteProductDetailProductAttributeValue.forEach(itemAttribute => {
            let contractAttribute = new ContractProductDetailProductAttributeValue();
            contractAttribute.ProductId = itemAttribute.productId;
            contractAttribute.ProductAttributeCategoryId = itemAttribute.productAttributeCategoryId;
            contractAttribute.ProductAttributeCategoryValueId = itemAttribute.productAttributeCategoryValueId;
            contractDetail.ContractProductDetailProductAttributeValue.push(contractAttribute);
          });

          this.listContractDetailModel = [...this.listContractDetailModel, contractDetail];
        });
      }
      let listQuoteCostDetail = this.listQuoteCostDetail.filter(c => c.quoteId == this.quoteId);
      if (listQuoteCostDetail.length > 0) {

        this.listContractCost = [];
        this.listContractCost = this.mappingModelContractCostDetailFromQuoteCost(listQuoteCostDetail);
      }

      this.CustomerOrderAmount = 0;
      this.listContractDetailModel.forEach(item => {
        this.CustomerOrderAmount = this.CustomerOrderAmount + item.SumAmount;
      });
      this.listContractCost.forEach(item => {
        this.CustomerOrderAmount = this.CustomerOrderAmount + item.SumAmount;
      });

      let value = this.discountTypeControl.value;
      let codeDiscountType = value.code;
      let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
      //Nếu loại chiết khấu là theo % thì giá trị discountValue không được lớn hơn 100%
      if (codeDiscountType == "PT") {
        if (discountValue > 100) {
          discountValue = 100;
          this.discountValueControl.setValue('100');
        }
      }
      /*Tính lại tổng thành tiền của hợp đồng*/
      let discountType = value.value;

      this.contractModel.Amount = this.CustomerOrderAmount;
      if (discountType) {
        //Nếu theo %
        this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
        this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
      } else {
        //Nếu theo Số tiền
        this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
        this.DiscountValue = discountValue;
      }
      this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
    }


    // set default cho thời gian hợp đồng
    // thời gian hợp đồng
    this.contractTimeControl.setValue('');
    // đơn vị
    this.contractTimeUnitControl.setValue(this.listContractTimeUnit.find(x => x.code == 'DAY'));
  }

  getListEmpCus(personInChargeId: string) {
    if (personInChargeId) {
      this.leadService.getEmployeeSale(this.listEmpSale, personInChargeId).subscribe(response => {
        let result: any = response;
        this.listEmpSaleCus = result.listEmployee;
        this.listEmpSaleCus.forEach(item => {
          item.employeeName = item.employeeName;
        });
        let emp = this.listEmpSaleCus.find(e => e.employeeId == personInChargeId);
        this.contractForm.controls['empSalerControl'].setValue(emp);
        this.getListMainContractByEmp();
      });
    } else {
      this.listEmpSaleCus = [];
    }
  }

  onChangeCus(event: any) {
    if (event === null) {
      this.resetForm();
      return;
    }
    this.phone = event.customerPhone;
    this.email = event.customerEmail;
    this.fullAddress = event.fullAddress;
    this.taxCode = event.taxCode;
    this.listBankAccountCus = this.listBankAccount.filter(c => c.objectId == event.customerId && c.objectType === 'CUS');
    let quotes = this.listQuote.filter(c => c.objectTypeId == event.customerId && c.objectType === 'CUSTOMER');
    this.listQuoteMaster = [];
    this.listQuoteMaster = quotes;
    if (this.listBankAccountCus.length > 0) {
      this.bankAccountControl.setValue(this.listBankAccountCus[0]);
      this.bankName = this.listBankAccountCus[0].bankName;
      this.bankAccount = this.listBankAccountCus[0].accountName;
      this.bankNumber = this.listBankAccountCus[0].accountNumber;
      this.branch = this.listBankAccountCus[0].branchName;
    } else {
      this.bankAccountControl.setValue(null);
      this.bankName = '';
      this.bankAccount = '';
      this.bankNumber = '';
      this.branch = '';
    }
    this.quoteControl.setValue(null);
    this.customerGroupId = event.customerGroupId;
    this.getListEmpCus(event.personInChargeId);
  }

  changeContractTime() {
    // if (this.contractTimeControl.value == null || this.contractTimeControl.value == '') {
    //   this.contractTimeControl.setValue(0);
    // }

    var effectiveDate = new Date(this.effectiveDateControl.value);
    var contractTime = parseFloat(this.contractTimeControl.value.replace(/,/g, ''));
    var contractTimeUnit = this.contractTimeUnitControl.value;
    let expiredDate: Date;

    if (this.contractTimeControl.value > 0){
      if (this.effectiveDateControl.value) {
        if (contractTimeUnit.code == 'DAY') {
          if (contractTime > 99981296) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 99,981,296 ngày' };
            this.showMessage(msg);
          } else {
            expiredDate = new Date(effectiveDate.setDate(effectiveDate.getDate() + contractTime));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
        else if (contractTimeUnit.code == 'MONTH') {
          if (contractTime > 3332710) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 3,332,710 tháng' };
            this.showMessage(msg);
          } else {
            expiredDate = new Date(effectiveDate.setMonth(effectiveDate.getMonth() + contractTime));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
        else if (contractTimeUnit.code == 'YEAR') {
          if (contractTime > 273921) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 273,921 năm' };
            this.showMessage(msg);
          } else {
            var days = Math.round(contractTime * 365);
            expiredDate = new Date(effectiveDate.setDate(effectiveDate.getDate() + days));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
      } else {
        this.expiredDateControl.setValue(null);
      }
    } else {
      this.expiredDateControl.setValue(null);
      this.contractTimeControl.setValue(null);
    }
  }

  changeTimeUnit() {
    // if (this.contractTimeControl.value == null || this.contractTimeControl.value == '') {
    //   this.contractTimeControl.setValue(0);
    // }
    var effectiveDate = new Date(this.effectiveDateControl.value);
    var contractTime = parseFloat(this.contractTimeControl.value.replace(/,/g, ''));
    var contractTimeUnit = this.contractTimeUnitControl.value;
    let expiredDate: Date;

    if (this.contractTimeControl.value > 0){
      if (this.effectiveDateControl.value) {
        if (contractTimeUnit.code == 'DAY') {
          this.isDay = true;
          if (contractTime > 99981296) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 99,981,296 ngày' };
            this.showMessage(msg);
          } else {
            expiredDate = new Date(effectiveDate.setDate(effectiveDate.getDate() + contractTime));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
        else if (contractTimeUnit.code == 'MONTH') {
          this.isDay = true;
          if (contractTime > 3332710) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 3,332,710 tháng' };
            this.showMessage(msg);
          } else {
            expiredDate = new Date(effectiveDate.setMonth(effectiveDate.getMonth() + contractTime));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
        else if (contractTimeUnit.code == 'YEAR') {
          this.isDay = false;
          if (contractTime > 273921) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 273,921 năm' };
            this.showMessage(msg);
          } else {
            var days = Math.round(contractTime * 12);
            expiredDate = new Date(effectiveDate.setDate(effectiveDate.getMonth() + days));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
      } else {
        this.expiredDateControl.setValue(null);
      }
    } else {
      this.expiredDateControl.setValue(null);
      this.contractTimeControl.setValue(null);
    }
  }

  changeEffectiveDate() {
    // if (this.contractTimeControl.value == null || this.contractTimeControl.value == '') {
    //   this.contractTimeControl.setValue(0);
    // }
    var effectiveDate = new Date(this.effectiveDateControl.value);
    var contractTime = parseFloat(this.contractTimeControl.value.replace(/,/g, ''));
    var contractTimeUnit = this.contractTimeUnitControl.value;
    let expiredDate: Date;

    if (this.contractTimeControl.value > 0){
      if (this.effectiveDateControl.value) {
        if (contractTimeUnit.code == 'DAY') {
          if (contractTime > 99981296) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 99,981,296 ngày' };
            this.showMessage(msg);
          } else {
            expiredDate = new Date(effectiveDate.setDate(effectiveDate.getDate() + contractTime));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
        else if (contractTimeUnit.code == 'MONTH') {
          if (contractTime > 3332710) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 3,332,710 tháng' };
            this.showMessage(msg);
          } else {
            expiredDate = new Date(effectiveDate.setMonth(effectiveDate.getMonth() + contractTime));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
        else if (contractTimeUnit.code == 'YEAR') {
          if (contractTime > 273921) {
            this.contractTimeControl.setValue(0);
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Thời gian hợp đồng không thể lớn hơn 273,921 năm' };
            this.showMessage(msg);
          } else {
            var days = Math.round(contractTime * 365);
            expiredDate = new Date(effectiveDate.setDate(effectiveDate.getDate() + days));
          }
          this.expiredDateControl.setValue(expiredDate);
        }
      }
      else {
        this.expiredDateControl.setValue(null);
      }
    } else {
      this.expiredDateControl.setValue(null);
      this.contractTimeControl.setValue(null);
    }
  }

  onClearClick() {
    this.isDay = true;
    this.contractTimeControl.setValue(0);
    this.contractTimeUnitControl.setValue(this.listContractTimeUnit.find(x => x.code = 'DAY'));
    this.expiredDateControl.setValue(null);
  }

  changeMethodControl(event: any) {
    if (event === null) return;
    if (event.categoryCode === 'BANK') {
      this.isShowBankAccount = true;
    } else {
      this.isShowBankAccount = false;
    }
  }

  changeBankAccount(event: any) {
    if (event.value === null) return;
    this.bankName = event.value.bankName;
    this.bankAccount = event.value.accountName;
    this.bankNumber = event.value.accountNumber;
    this.branch = event.value.branchName;
  }

  changeTypeContract(event: any) {
    if (event === null) return;
    if (event.value.categoryCode === 'PLHD') {
      this.mainContractControl.enable();
      this.contractForm.controls['mainContractControl'].setValidators(Validators.required);
      this.contractForm.controls['mainContractControl'].updateValueAndValidity();
      this.mainContractControl.setValue(null);
      this.isRequiredCustomerOrder = false;
      this.isRequiredMainContract = true;
    } else if (event.value.categoryCode === 'HDNT') {
      this.mainContractControl.disable();
      this.contractForm.controls['mainContractControl'].setValidators(null);
      this.contractForm.controls['mainContractControl'].updateValueAndValidity();
      this.mainContractControl.setValue(null);
      this.isRequiredCustomerOrder = false;
      this.isRequiredMainContract = false;
    } else {
      this.mainContractControl.enable();
      this.contractForm.controls['mainContractControl'].setValidators(null);
      this.contractForm.controls['mainContractControl'].updateValueAndValidity();
      this.mainContractControl.setValue(null);
      this.isRequiredCustomerOrder = true;
      this.isRequiredMainContract = false;
    }
  }

  changeQuote(event: any) {
    if (event.value === null) {
      this.resetForm();
      return;
    }
    this.leadId = event.value.leadId;
    this.leadCode = event.value.leadCode;
    this.saleBiddingId = event.value.saleBiddingId;
    this.saleBiddingCode = event.value.saleBiddingCode;
    if (event.value.objectTypeId != null) {
      let customer = this.objectList.find(c => c.customerId == event.value.objectTypeId);
      if (customer) {
        this.contractForm.controls['objectControl'].setValue(customer);
        this.phone = customer.customerPhone;
        this.email = customer.customerEmail;
        this.fullAddress = customer.fullAddress;
        this.taxCode = customer.taxCode;
        this.listBankAccountCus = this.listBankAccount.filter(c => c.objectId == customer.customerId);

        this.customerGroupId = customer.customerGroupId;
        if (customer.personInChargeId) {
          this.leadService.getEmployeeSale(this.listEmpSale, customer.personInChargeId).subscribe(response => {
            let result: any = response;
            this.listEmpSaleCus = result.listEmployee;
            this.listEmpSaleCus.forEach(item => {
              item.employeeName = item.employeeName;
            });
            let employee = this.listEmpSaleCus.find(c => c.employeeId == event.value.seller);
            if (employee) {
              this.empSalerControl.setValue(employee);
            }
          });
        }
      } else {
        this.listEmpSaleCus = [];
      }
    }
    if (event.value.paymentMethod) {
      let paymentMethod = this.listPaymentMethod.find(c => c.categoryId == event.value.paymentMethod);
      if (paymentMethod) {
        this.contractForm.controls['paymentMethodControl'].setValue(paymentMethod);
        if (paymentMethod.categoryCode === 'BANK') {
          this.isShowBankAccount = true;
        } else {
          this.isShowBankAccount = false;
        }
      }
    }

    if (event.value.quoteId) {
      let listQuoteDetail = this.listQuoteDetail.filter(c => c.quoteId === event.value.quoteId);
      if (listQuoteDetail.length > 0) {
        this.listContractDetailModel = [];
        listQuoteDetail.forEach((item, index) => {
          let contractDetail = new ContractDetail();
          contractDetail.VendorId = item.vendorId;
          contractDetail.ProductId = item.productId;
          contractDetail.Quantity = item.quantity;
          contractDetail.UnitPrice = item.unitPrice;
          contractDetail.CurrencyUnit = item.currencyUnit;
          contractDetail.ExchangeRate = item.exchangeRate;
          contractDetail.Vat = item.vat;
          contractDetail.DiscountType = item.discountType;
          contractDetail.DiscountValue = item.discountValue;
          contractDetail.Description = item.description;
          contractDetail.OrderDetailType = item.orderDetailType;
          contractDetail.UnitId = item.unitId;
          contractDetail.IncurredUnit = item.incurredUnit;
          contractDetail.Active = item.active;
          contractDetail.OrderDetailType = item.orderDetailType;
          contractDetail.ExplainStr = item.nameProduct;
          contractDetail.NameVendor = item.nameVendor;
          contractDetail.NameProduct = item.nameProduct;
          contractDetail.ProductNameUnit = item.nameProductUnit;
          contractDetail.NameMoneyUnit = item.nameMoneyUnit;
          contractDetail.ProductName = item.productName;
          contractDetail.OrderNumber = index + 1;

          let unitPrice = parseFloat(item.unitPrice);
          let quantity = parseFloat(item.quantity);
          let exchangeRate = parseFloat(item.exchangeRate);
          let total = unitPrice * quantity * exchangeRate;
          let value = item.discountType;
          let vat = parseFloat(item.vat);
          let discountValue = parseFloat(item.discountValue);
          if (value) {
            var discountResult = (total * discountValue) / 100;
            var vatResult = ((total - discountResult) * vat) / 100;
            contractDetail.SumAmount = total - discountResult + vatResult
          } else {
            var discountResult = discountValue;
            var vatResult = ((total - discountResult) * vat) / 100;
            contractDetail.SumAmount = total - discountResult + vatResult
          }
          item.quoteProductDetailProductAttributeValue.forEach(itemAttribute => {
            let contractAttribute = new ContractProductDetailProductAttributeValue();
            contractAttribute.ProductId = itemAttribute.productId;
            contractAttribute.ProductAttributeCategoryId = itemAttribute.productAttributeCategoryId;
            contractAttribute.ProductAttributeCategoryValueId = itemAttribute.productAttributeCategoryValueId;
            contractDetail.ContractProductDetailProductAttributeValue.push(contractAttribute);
          });

          this.listContractDetailModel = [...this.listContractDetailModel, contractDetail];
        });

      }
      let listQuoteCostDetail = this.listQuoteCostDetail.filter(c => c.quoteId == event.value.quoteId);
      if (listQuoteCostDetail.length > 0) {
        this.listContractCost = [];
        this.listContractCost = this.mappingModelContractCostDetailFromQuoteCost(listQuoteCostDetail);
      }
      this.CustomerOrderAmount = 0;

      this.listContractDetailModel.forEach(item => {
        this.CustomerOrderAmount = this.CustomerOrderAmount + item.SumAmount;
      });
      this.listContractCost.forEach(item => {
        this.CustomerOrderAmount = this.CustomerOrderAmount + item.SumAmount;
      });

      let value = this.discountTypeControl.value;
      let codeDiscountType = value.code;
      let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
      //Nếu loại chiết khấu là theo % thì giá trị discountValue không được lớn hơn 100%
      if (codeDiscountType == "PT") {
        if (discountValue > 100) {
          discountValue = 100;
          this.discountValueControl.setValue('100');
        }
      }
      /*Tính lại tổng thành tiền của hợp đồng*/
      let discountType = value.value;
      this.contractModel.Amount = this.CustomerOrderAmount;
      if (discountType) {
        //Nếu theo %
        this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
        this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
      } else {
        //Nếu theo Số tiền
        this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
        this.DiscountValue = discountValue;
      }
      this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
    }
  }

  createOrUpdate(value: boolean) {
    if (!this.contractForm.valid) {
      Object.keys(this.contractForm.controls).forEach(key => {
        if (this.contractForm.controls[key].valid == false) {
          this.contractForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.contractForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else if (this.listContractDetailModel.length === 0 && this.isRequiredCustomerOrder === true) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm dịch vụ được chọn' };
      this.showMessage(mgs);
    } else {
      this.mappingForm();
      this.awaitResult = false;
      this.loading = true;
      this.contractService.createOrUpdateContractFromForm(this.contractModel, this.listAdditionalInformation,
        this.listContractDetailModel, this.listContractCost,
        this.file, this.listFile, true, this.auth.UserId).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            if (value) {
              //Lưu và thêm mới
              if (this.emitStatusChangeForm) {
                this.emitStatusChangeForm.unsubscribe();
                this.isInvalidForm = false; //Ẩn icon-warning-active
              }
              this.resetForm();
              this.router.navigate(['/sales/contract-create']);
              this.awaitResult = false;
            } else {
              //Lưu
              this.router.navigate(['/sales/contract-detail', { contractId: result.contractId }]);
            }
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
    }
  }

  resetForm() {
    this.listQuoteMaster = this.listQuote;
    this.objectControl.setValue(null);
    this.listQuoteMaster = this.listQuote;
    this.phone = '';
    this.email = '';
    this.fullAddress = '';
    this.taxCode = '';
    this.paymentMethodControl.setValue(this.listPaymentMethod.find(x => x.isDefault == true));
    this.listBankAccountCus = [];
    this.bankAccountControl.setValue(null);
    this.bankName = '';
    this.bankAccount = '';
    this.bankNumber = '';
    this.branch = '';
    this.typeContractControl.setValue(null);
    this.quoteControl.setValue(null);
    this.mainContractControl.setValue(null);
    this.empSalerControl.setValue(null);
    this.isShowBankAccount = false;
    this.currentEmployeeId = this.auth.EmployeeId;
    this.effectiveDateControl.setValue(new Date);
    this.expiredDateControl.setValue(null);
    this.contractTimeControl.setValue(0);
    this.contractTimeUnitControl.setValue(null);
    this.isPersonInCharge = false;
    this.descriptionControl.setValue('');
    this.noteControl.setValue('');
    this.titleText = '';
    this.contentText = '';
    this.isUpdateAI = false;
    this.listAdditionalInformation = [];
    this.listContractDetailModel = [];
    this.selectedColumns = this.cols;
    this.resetContractModel();
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl.setValue('0');
    this.CustomerOrderAmountAfterDiscount = 0;
    this.CustomerOrderAmount = 0;
    this.DiscountValue = 0;
    this.customerGroupId = '';
  }

  resetContractModel() {
    var item: Contract = {
      ContractId: this.emptyGuid,
      ContractCode: '',
      EffectiveDate: new Date(),
      ExpiredDate: null,
      ContractTime: 0,
      ContractTimeUnit: '',
      ContractDescription: '',
      EmployeeId: this.emptyGuid,
      MainContractId: this.emptyGuid,
      ContractTypeId: this.emptyGuid,
      ValueContract: 0,
      QuoteId: this.emptyGuid,
      ContractNote: '',
      CustomerId: this.emptyGuid,
      ObjectType: '',
      CustomerName: '',
      PaymentMethodId: this.emptyGuid,
      DiscountType: true,
      BankAccountId: null,
      Amount: 0,
      DiscountValue: 0,
      StatusId: null,//Guid?
      Active: true,
      IsExtend: false,
      CreatedById: this.auth.UserId,
      CreatedDate: new Date(),
      UpdatedById: this.emptyGuid,
      UpdatedDate: new Date(),
    };
    this.contractModel = item;
  }

  cancel() {
    this.router.navigate(['/sales/contract-list']);
  }

  /*Thêm một thông tin bổ sung*/
  addAI() {
    if (this.titleText == null || this.titleText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề không được để trống' };
      this.showMessage(msg);
    } else if (this.contentText == null || this.contentText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Nội dung không được để trống' };
      this.showMessage(msg);
    } else {
      let note: AdditionalInformation = {
        ordinal: this.maxOrdinal,
        additionalInformationId: this.emptyGuid,
        objectId: this.emptyGuid,
        objectType: '',
        title: this.titleText.trim(),
        content: this.contentText.trim(),
        active: true,
        createdDate: new Date(),
        createdById: this.emptyGuid,
        updatedDate: null,
        updatedById: null,
        orderNumber: null
      };

      note.orderNumber = this.listAdditionalInformation.length + 1;

      //Kiểm tra xem title đã tồn tại chưa
      let check = this.listAdditionalInformation.find(x => x.title == note.title);
      if (check) {
        //Nếu tồn tại rồi thì không cho thêm và hiển thị cảnh báo
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề này đã tồn tại' };
        this.showMessage(msg);
      } else {
        this.listAdditionalInformation.push(note);
        this.titleText = '';
        this.contentText = '';
      }
    }
  }

  /*Hiển thị lại thông tin bổ sung*/
  reShowNote(event: any) {
    if (this.actionEdit) {
      let rowData = event.data;
      this.isUpdateAI = true;
      this.AIUpdate.ordinal = rowData.ordinal;
      this.AIUpdate.title = rowData.title;

      this.titleText = rowData.title;
      this.contentText = rowData.content;
    }
  }

  /*Hủy cập nhật thông tin bổ sung*/
  cancelAI() {
    this.isUpdateAI = false;
    this.AIUpdate = {
      ordinal: null,
      additionalInformationId: this.emptyGuid,
      objectId: this.emptyGuid,
      objectType: '',
      title: null,
      content: null,
      active: true,
      createdDate: new Date(),
      createdById: this.emptyGuid,
      updatedDate: null,
      updatedById: null,
      orderNumber: null
    };
    this.titleText = '';
    this.contentText = '';
  }

  /*Cập nhật thông tin bổ sung*/
  updateContentAI() {
    if (this.titleText == null || this.titleText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề không được để trống' };
      this.showMessage(msg);
    } else if (this.contentText == null || this.contentText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Nội dung không được để trống' };
      this.showMessage(msg);
    } else {
      var check = this.listAdditionalInformation.find(x => x.title == this.AIUpdate.title);
      if (check) {
        //Kiểm tra xem title đã tồn tại chưa
        let checkDublicate = this.listAdditionalInformation.find(x => x.title == this.titleText.trim() && x.ordinal != this.AIUpdate.ordinal);

        if (checkDublicate) {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề này đã tồn tại' };
          this.showMessage(msg);
        } else {
          this.listAdditionalInformation.forEach(item => {
            if (item.title == this.AIUpdate.title) {
              item.title = this.titleText.trim();
              item.content = this.contentText.trim();
            }
          });

          //reset form
          this.isUpdateAI = false;
          this.AIUpdate = {
            ordinal: null,
            additionalInformationId: this.emptyGuid,
            objectId: this.emptyGuid,
            objectType: '',
            title: null,
            content: null,
            active: true,
            createdDate: new Date(),
            createdById: this.emptyGuid,
            updatedDate: null,
            updatedById: null,
            orderNumber: null
          };
          this.titleText = '';
          this.contentText = '';
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Không tồn tại thông tin bổ sung này' };
        this.showMessage(msg);
      }
    }
  }

  /*Xóa thông tin bổ sung*/
  deleteAI(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listAdditionalInformation.indexOf(rowData);
        this.listAdditionalInformation.splice(index, 1);
      }
    });
  }

  /*Thêm một sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let ref = this.dialogService.open(AddEditProductContractDialogComponent, {
      data: {
        isCreate: true,
        unitMoney: this.unitMoneyControl.value,
        exchangeRate: this.exchangeRateControl.value,
        customerGroupId: this.customerGroupId,
        dateOrder: convertToUTCTime(new Date()),
      },
      header: 'Thêm sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.contractDetailModel = result.contractDetailModel;

          //set orderNumber cho sản phẩm/dịch vụ mới thêm
          this.contractDetailModel.OrderNumber = this.listContractDetailModel.length + 1;

          this.listContractDetailModel = [...this.listContractDetailModel, this.contractDetailModel];

          //Cộng tổng giá cho toàn bộ hợp đồng
          this.contractModel.Amount = this.contractModel.Amount + this.contractDetailModel.SumAmount;
          this.CustomerOrderAmount = this.CustomerOrderAmount + this.contractDetailModel.SumAmount;
          let discountType: DiscountType = this.discountTypeControl.value;
          let value = discountType.value;
          let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
          if (value) {
            this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
            this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
          } else {
            this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
            this.DiscountValue = discountValue;
          }
          this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
        }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listContractDetailModel.indexOf(dataRow);
      var OldArray = this.listContractDetailModel[index];
      let titlePopup = '';
      if (OldArray.OrderDetailType == 0) {
        titlePopup = 'Sửa sản phẩm dịch vụ';
      } else if (OldArray.OrderDetailType == 1) {
        titlePopup = 'Sửa chi phí phát sinh';
      }

      let ref = this.dialogService.open(AddEditProductContractDialogComponent, {
        data: {
          isCreate: false,
          unitMoney: this.unitMoneyControl.value,
          exchangeRate: this.exchangeRateControl.value,
          quoteDetailModel: OldArray,
          customerGroupId: this.customerGroupId,
          isEdit: true,
          dateOrder: convertToUTCTime(new Date()),
        },
        header: titlePopup,
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultDialog) => {
        if (result) {
          if (result.status) {
            this.listContractDetailModel.splice(index, 1);
            this.contractDetailModel = result.contractDetailModel;
            this.listContractDetailModel = [...this.listContractDetailModel, this.contractDetailModel];

            //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
            this.listContractDetailModel.sort((a, b) =>
              (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));

            /*Tính lại tổng tiền của đơn hàng*/
            this.contractModel.Amount -= OldArray.SumAmount;
            this.contractModel.Amount += this.contractDetailModel.SumAmount;

            this.CustomerOrderAmount -= OldArray.SumAmount;
            this.CustomerOrderAmount += this.contractDetailModel.SumAmount;

            let discountType: DiscountType = this.discountTypeControl.value;
            let value = discountType.value;
            let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
            if (value) {
              this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
              this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
            } else {
              this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
              this.DiscountValue = discountValue;
            }
            this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
            this.restartCustomerOrderDetailModel();
          }
        }
      });
    }
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.contractModel.Amount = this.contractModel.Amount - dataRow.SumAmount;
        let discountType: DiscountType = this.discountTypeControl.value;
        let value = discountType.value;
        let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));

        if (value) {
          this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
          this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
        } else {
          this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
          this.DiscountValue = discountValue;
        }
        this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
        this.CustomerOrderAmount = this.contractModel.Amount;

        this.listContractDetailModel = this.listContractDetailModel.filter(x => x != dataRow);

        //Đánh lại số OrderNumber
        this.listContractDetailModel.forEach((item, index) => {
          item.OrderNumber = index + 1;
        });
      }
    });
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Event khi thay đổi loại chiết khấu: Theo % hoặc Theo số tiền*/
  changeDiscountType(value: DiscountType) {
    this.discountValueControl.setValue('0');
  }

  /*Event khi thay đổi giá trị chiết khấu*/
  changeDiscountValue() {
    let discountValue = 0;
    if (this.discountValueControl.value.trim() == '') {
      discountValue = 0;
      this.discountValueControl.setValue('0');
    } else {
      discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
    }

    let discountType = this.discountTypeControl.value;
    let codeDiscountType = discountType.code;
    //Nếu loại chiết khấu là theo % thì giá trị discountValue không được lớn hơn 100%
    if (codeDiscountType == "PT") {
      if (discountValue > 100) {
        discountValue = 100;
        this.discountValueControl.setValue('100');
      }
    } else {
      if (discountValue > this.CustomerOrderAmount) {
        discountValue = this.CustomerOrderAmount;
        this.discountValueControl.setValue(this.CustomerOrderAmount.toString());
      }
    }

    /*Tính lại tổng thành tiền của hợp đồng*/
    var TotalSumaMount = 0;
    this.listContractDetailModel.forEach(function (item) {
      TotalSumaMount = TotalSumaMount + item.SumAmount;
    });
    this.listContractCost.forEach(function (item) {
      TotalSumaMount = TotalSumaMount + item.SumAmount;
    });

    this.contractModel.Amount = TotalSumaMount;
    this.contractModel.DiscountType = discountType.value;
    this.contractModel.DiscountValue = discountValue;
    if (discountType.value) {
      //Nếu theo %
      this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
      this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
    } else {
      //Nếu theo Số tiền
      this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
      this.DiscountValue = discountValue;
    }

    /*
    * Nếu số thành tiền âm (vì nếu loại chiết khấu là % thì giá trị chiết khấu lớn nhất là 100%
    * nên số thành tiền không thể âm, vậy nếu số thành tiền âm thì chỉ có trường hợp giá trị
    * chiết khấu lúc này là Số tiền)
    */
    if (this.CustomerOrderAmountAfterDiscount < 0) {
      this.CustomerOrderAmountAfterDiscount = 0;
      this.discountValueControl.setValue(this.contractModel.Amount.toString());
    }
    /*End*/
  }

  /*Thêm một chi phí*/
  addContractCost() {
    let ref = this.dialogService.open(AddEditContractCostDialogComponent, {
      data: {
        isCreate: true
      },
      header: 'Thêm chi phí',
      width: '30%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultCostDialog) => {
      if (result) {
        if (result.status) {
          this.contractCostDetailModel = result.contractCostModel;
          this.listContractCost.push(this.contractCostDetailModel);

          //Cộng tổng giá cho toàn bộ hợp đồng
          this.contractModel.Amount = this.contractModel.Amount + this.contractCostDetailModel.SumAmount;
          this.CustomerOrderAmount = this.CustomerOrderAmount + this.contractCostDetailModel.SumAmount;
          let discountType: DiscountType = this.discountTypeControl.value;
          let value = discountType.value;
          let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
          if (value) {
            this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
            this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
          } else {
            this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
            this.DiscountValue = discountValue;
          }
          this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
          this.restartCustomerOrderDetailModel();
        }
      }
    });
  }

  /*Sửa một thông tin phí*/
  onRowCostSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listContractCost.indexOf(dataRow);
      var OldArray = this.listContractCost[index];

      let titlePopup = 'Sửa chi phí';

      let ref = this.dialogService.open(AddEditContractCostDialogComponent, {
        data: {
          isCreate: false,
          contractCostDetailModel: OldArray
        },
        header: titlePopup,
        width: '30%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultCostDialog) => {
        if (result) {
          if (result.status) {
            this.listContractCost.splice(index, 1);
            this.contractCostDetailModel = result.contractCostModel;
            this.listContractCost.push(this.contractCostDetailModel);

            this.contractModel.Amount -= OldArray.SumAmount;
            this.contractModel.Amount += this.contractCostDetailModel.SumAmount;

            this.CustomerOrderAmount -= OldArray.SumAmount;
            this.CustomerOrderAmount += this.contractCostDetailModel.SumAmount;

            let discountType: DiscountType = this.discountTypeControl.value;
            let value = discountType.value;
            let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
            if (value) {
              this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
              this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
            } else {
              this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
              this.DiscountValue = discountValue;
            }
            this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
            this.restartCustomerOrderDetailModel();
          }
        }
      });
    }
  }

  /*Xóa thông tin phí*/
  deleteCostItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listContractCost.indexOf(dataRow);
        this.contractModel.Amount = this.contractModel.Amount - dataRow.SumAmount;
        let discountType: DiscountType = this.discountTypeControl.value;
        let value = discountType.value;
        let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
        if (value) {
          this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * discountValue) / 100;
          this.DiscountValue = (this.contractModel.Amount * discountValue) / 100;
        } else {
          this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - discountValue;
          this.DiscountValue = discountValue;
        }
        this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
        this.listContractCost.splice(index, 1);
        this.CustomerOrderAmount = this.contractModel.Amount;
      }
    });
  }

  restartCustomerOrderDetailModel() {
    this.contractDetailModel = new ContractDetail();
  }

  goToLead() {
    this.router.navigate(['/lead/detail', { 'leadId': this.leadId }]);
  }

  goToSaleBidding() {
    this.router.navigate(['/sale-bidding/detail', { saleBiddingId: this.saleBiddingId }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  /* Chuyển item lên một cấp */
  moveUp(data: ContractDetail) {
    let currentOrderNumber = data.OrderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listContractDetailModel.find(x => x.OrderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = preOrderNumber;

    //Xóa 2 item
    this.listContractDetailModel = this.listContractDetailModel.filter(x =>
      x.OrderNumber != preOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listContractDetailModel = [...this.listContractDetailModel, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listContractDetailModel.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: ContractDetail) {
    let currentOrderNumber = data.OrderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listContractDetailModel.find(x => x.OrderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listContractDetailModel = this.listContractDetailModel.filter(x =>
      x.OrderNumber != nextOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listContractDetailModel = [...this.listContractDetailModel, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listContractDetailModel.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  /* Chuyển item lên một cấp */
  moveUpAdditionalInfor(data: AdditionalInformation) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listAdditionalInformation.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listAdditionalInformation = this.listAdditionalInformation.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listAdditionalInformation = [...this.listAdditionalInformation, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listAdditionalInformation.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDownAdditionalInfor(data: AdditionalInformation) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listAdditionalInformation.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listAdditionalInformation = this.listAdditionalInformation.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listAdditionalInformation = [...this.listAdditionalInformation, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listAdditionalInformation.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

//So sánh giá trị nhập vào có thuộc khoảng xác định hay không?
function ageRangeValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (isNaN(control.value) ||
      parseFloat(control.value.replace(/,/g, '')) < min ||
      parseFloat(control.value.replace(/,/g, '')) > max)) {
      return { 'ageRange': true };
    }
    return null;
  };
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
