import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';
import { CustomerOrder } from '../../models/customer-order.model';
import { OrderProductDetailProductAttributeValue } from '../../models/order-product-detail-product-attribute-value.model';
import { ContactModel } from '../../../shared/models/contact.model';
import { CustomerModel } from '../../../customer/models/customer.model';
import { CustomerService } from '../../../customer/services/customer.service';
import { CustomerOrderService } from '../../services/customer-order.service';
import { BankService } from '../../../shared/services/bank.service';
import { ContactService } from '../../../shared/services/contact.service';
import { QuoteService } from '../../../customer/services/quote.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { SendEmailModel, OrderDetailToSendEmailModel } from "../../../admin/models/sendEmail.model"
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { OrderDetailDialogComponent } from '../order-detail-dialog/order-detail-dialog.component';
import { CustomerOrderDetail } from '../../models/customer-order-detail.model';
import * as $ from 'jquery';
import { Quote } from '../../../customer/models/quote.model';
// import { AddEditCostQuoteDialogComponent } from '../../../customer/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { OrderCostDetail } from '../../models/customer-order-cost-detail.model';
import { TranslateService } from '@ngx-translate/core';

interface ResultDialog {
  status: boolean,
  customerOrderDetailModel: CustomerOrderDetail,
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface OrderStatus {
  orderStatusId: string;
  orderStatusCode: string;
  description: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

interface Customer {
  customerId: string;
  customerCode: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  fullAddress: string;
  paymentId: string;
  maximumDebtDays: number;
  maximumDebtValue: number;
  taxCode: string;
  personInChargeId: string;
}

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

interface BankAccount {
  bankAccountId: string;
  bankName: string; //Ngân hàng
  accountNumber: string;  //Số tài khoản
  branchName: string; //Chi nhánh
  accountName: string; //Chủ tài khoản
  objectId: string;
}

interface CustomerType {
  typeValue: number;
  typeName: string;
}

interface QuoteResponse {
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
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: OrderCostDetail,
}

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;

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

  // fixed: boolean = false;
  // withFiexd: string = "";
  // @HostListener('document:scroll', [])
  // onScroll(): void {
  //   let num = window.pageYOffset;
  //   if (num > 100) {
  //     this.fixed = true;
  //     var width: number = $('#parent').width();
  //     this.withFiexd = width + 'px';
  //   } else {
  //     this.fixed = false;
  //     this.withFiexd = "";
  //   }
  // }

  /* Form */
  colLeft: number = 8;
  isShow: boolean = true;
  createOrderForm: FormGroup;
  orderDateControl: FormControl;
  quoteControl: FormControl;
  orderContractControl: FormControl;
  orderStatusControl: FormControl;
  sellerControl: FormControl;
  descriptionControl: FormControl;
  noteControl: FormControl;
  customerControl: FormControl;
  cusNameControl: FormControl;
  customerMSTControl: FormControl;
  customerPhoneControl: FormControl;
  customerAddressControl: FormControl;
  paymentMethodControl: FormControl;
  daysAreOwedControl: FormControl;
  maxDebtControl: FormControl;
  bankAccountControl: FormControl;
  customerGroupControl: FormControl;
  customerCodeControl: FormControl;
  customerNameControl: FormControl;
  customerTypeControl: FormControl;
  newCusPaymentMethodControl: FormControl;
  newCusDaysAreOwedControl: FormControl;
  newCusMaxDebtControl: FormControl;
  paymentMethodQuoteControl: FormControl;
  daysAreOwedQuoteControl: FormControl;
  maxDebtQuoteControl: FormControl;
  receivedDateControl: FormControl;
  receivedHourControl: FormControl;
  recipientNameControl: FormControl;
  locationOfShipmentControl: FormControl;
  placeOfDeliveryControl: FormControl;
  recipientPhoneControl: FormControl;
  recipientEmailControl: FormControl;
  shippingNoteControl: FormControl;
  discountTypeControl: FormControl;
  discountValueControl: FormControl;
  wareHouseControl: FormControl;
  /* End */

  /* Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  /* End */

  listOrderCostDetailModel: Array<OrderCostDetail> = [];
  listOrderStatus: Array<OrderStatus> = [];
  listProduct: Array<any> = [];
  listQuote: Array<any> = [];
  listQuoteObj: Array<any> = [];
  listWare: Array<any> = [];
  listOrderContract: Array<any> = []; // List hợp đồng
  listContract: Array<any> = []; // List hợp đồng
  listEmployee: Array<Employee> = [];
  listPersonInCharge: Array<Employee> = [];
  optionCustomer: string = '2';
  listCustomer: Array<any> = [];
  cusObj: any = null;
  customerEmail: string = '';
  customerPhone: string = '';
  fullAddress: string = '';
  leadName: string = '';
  leadEmail: string = '';
  leadPhone: string = '';
  leadFullAddress: string = '';
  listPaymentMethod: Array<Category> = [];
  listCustomerBankAccount: Array<BankAccount> = [];
  isShowBankAccount: boolean = false;
  listBankAccount: Array<BankAccount> = [];
  customerContactCode: string = 'CUS';
  listUnitMoney: Array<Category> = [];
  unitMoneyLabel: string = 'VND';
  listCustomerGroup: Array<Category> = [];
  listCustomerCode: Array<string> = [];
  listCustomerType: Array<CustomerType> = [
    {
      typeValue: 2,
      typeName: 'Khách hàng cá nhân'
    },
    {
      typeValue: 1,
      typeName: 'Khách hàng doanh nghiệp'
    }
  ];
  quoteObj: any;
  contractObj: any;
  warehouseName: string = '';
  cols: any[];
  selectedColumns: any[];
  colsCost: any[];
  selectedColumnsCost: any[];
  selectedItem: any;
  listCustomerOrderDetailModel: Array<CustomerOrderDetail> = [];
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];

  autoGenValue: boolean = true;

  customerId: string = null;
  quoteId: string = null;
  contractId: string = null;

  typeAccount: number = 2;

  customerModel: CustomerModel = {
    CustomerId: this.emptyGuid,
    CustomerCode: '',
    CustomerGroupId: '',
    CustomerName: '',
    LeadId: '',
    StatusId: this.emptyGuid,
    StatusCareId: this.emptyGuid,
    CustomerServiceLevelId: '',
    CustomerServiceLevelName: '',
    PersonInChargeId: '',
    CustomerCareStaff: '',
    CustomerType: 2,
    PaymentId: '',
    FieldId: '',
    ScaleId: '',
    TotalSaleValue: 0,
    TotalReceivable: 0,
    NearestDateTransaction: null,
    BusinessRegistrationDate: null,
    BusinessScale: this.emptyGuid,
    BusinessType: this.emptyGuid,
    EnterpriseType: this.emptyGuid,
    TotalCapital: 0,
    TotalEmployeeParticipateSocialInsurance: 0,
    TotalRevenueLastYear: 0,
    MaximumDebtValue: 0,
    MaximumDebtDays: 0,
    MainBusinessSector: null,
    IsGraduated: false,
    CreatedById: this.emptyGuid,
    CreatedDate: new Date(),
    UpdatedById: '',
    UpdatedDate: '',
    Active: true,
    IsApproval: null,
    ApprovalStep: null,
    InvestmentFundId: null,
    ContactDate : null,
    EmployeeTakeCareId : null,
    SalesUpdate: '',
    SalesUpdateAfterMeeting: '',
    EvaluateCompany: ''
  };

  customerOrderModel: CustomerOrder = {
    OrderId: this.emptyGuid,
    OrderCode: '',
    OrderDate: new Date(),
    Seller: this.auth.UserId,
    Description: '',
    Note: '',
    CustomerId: this.emptyGuid,
    CustomerContactId: null,
    PaymentMethod: this.emptyGuid,
    DaysAreOwed: null,
    MaxDebt: 0,
    ReceivedDate: new Date(),
    ReceivedHour: null,
    RecipientName: '',
    LocationOfShipment: '',
    ShippingNote: '',
    RecipientPhone: '',
    RecipientEmail: '',
    PlaceOfDelivery: '',
    Amount: 0,
    DiscountValue: 0,
    ReceiptInvoiceAmount: null,
    StatusId: null,
    CreatedById: this.auth.UserId,
    CreatedDate: new Date(),
    UpdatedById: this.emptyGuid,
    UpdatedDate: new Date(),
    Active: true,
    DiscountType: true,
    BankAccountId: null,
    QuoteId: null,
    ReasonCancel: null,
    IsAutoGenReceiveInfor: true,
    CustomerName: null,
    CustomerAddress: null,
    OrderContractId: null,
    WarehouseId: null,
  };

  contactModel = new ContactModel();

  arrayCustomerOrderDetailModel: Array<CustomerOrderDetail> = [];
  arrayOrderProductDetailProductAttributeValueModel: Array<OrderProductDetailProductAttributeValue> = [];

  CustomerOrderAmountAfterDiscount: number = 0;
  TotalSumAmountProduct: number = 0;
  TotalSumAmountCost: number = 0;
  TotalSumVatProduct: number = 0;
  TotalPriceInitial: number = 0;
  AmountPriceProfit: number = 0;
  ValuePriceProfit: number = 0;
  CustomerOrderTotalDiscount: number = 0;
  messageConfirm: string = '';

  constructor(private translate: TranslateService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private bankService: BankService,
    private customerOrderService: CustomerOrderService,
    private quoteService: QuoteService,
    private contactService: ContactService,
    public cdRef: ChangeDetectorRef,
    private emailConfigService: EmailConfigService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
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
  }

  ngOnInit() {
    /*Kiểm tra nhân viên hoặc quản lý*/

    let resource = "sal/order/create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      this.route.params.subscribe(params => {
        this.quoteId = params['quoteID'];
        this.customerId = params['customerId'];
        this.contractId = params['contractId'];
      });

      this.setForm();
      this.setTable();

      this.loading = true;

      let createType = 1;
      let createObjectId = null;
      if (this.customerId) {
        createType = 2;
        createObjectId = this.customerId;
      }
      else if (this.quoteId) {
        createType = 3;
        createObjectId = this.quoteId;
      }
      else if (this.contractId) {
        createType = 4;
        createObjectId = this.contractId;
      }

      this.customerOrderService.getMasterDataOrderCreate(createType, createObjectId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listOrderStatus = result.listOrderStatus;
          this.listEmployee = result.listEmployee;
          this.listPersonInCharge = result.listEmployee;
          this.listCustomer = result.listCustomer;
          this.listPaymentMethod = result.listPaymentMethod;
          this.listCustomerBankAccount = result.listCustomerBankAccount;
          this.listCustomerGroup = result.listCustomerGroup;
          this.listCustomerCode = result.listCustomerCode;
          this.listQuote = result.listQuote;
          // this.listQuoteObj = result.listQuote;
          this.listWare = result.listWare;
          this.listProduct = result.listProduct;
          this.listOrderContract = result.listContract;
          // this.listContract = result.listContract;

          this.setDefaultValue();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }
  changeWare(event) {
    if (event) {
      this.warehouseName = event.warehouseName;
    }
    else {
      this.warehouseName = '';
    }
  }
  setForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    this.orderDateControl = new FormControl(new Date(), [Validators.required]);
    this.orderStatusControl = new FormControl(null, [Validators.required]);
    this.sellerControl = new FormControl(null, [Validators.required]);
    this.descriptionControl = new FormControl(null);
    this.quoteControl = new FormControl(null);
    this.wareHouseControl = new FormControl(null);
    this.orderContractControl = new FormControl(null);
    this.noteControl = new FormControl(null);
    this.customerControl = new FormControl(null, [Validators.required]);
    this.cusNameControl = new FormControl(null, [Validators.required]);
    this.customerMSTControl = new FormControl(null);
    this.customerPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern())]);
    this.customerAddressControl = new FormControl(null);
    this.paymentMethodControl = new FormControl(null);
    this.daysAreOwedControl = new FormControl(null);
    this.maxDebtControl = new FormControl(null);
    this.bankAccountControl = new FormControl(null);
    this.customerGroupControl = new FormControl(null);
    this.customerCodeControl = new FormControl(null);
    this.customerNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.customerTypeControl = new FormControl(this.listCustomerType[0]);
    this.newCusPaymentMethodControl = new FormControl(null);
    this.newCusDaysAreOwedControl = new FormControl(null);
    this.newCusMaxDebtControl = new FormControl(null);
    this.paymentMethodQuoteControl = new FormControl(null);
    this.daysAreOwedQuoteControl = new FormControl(null);
    this.maxDebtQuoteControl = new FormControl(null);
    this.receivedDateControl = new FormControl(new Date(), [Validators.required]);
    this.receivedHourControl = new FormControl(null);
    this.recipientNameControl = new FormControl(null, [forbiddenSpaceText]);
    this.locationOfShipmentControl = new FormControl(null);
    this.placeOfDeliveryControl = new FormControl(null);
    this.recipientPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern())]);
    this.recipientEmailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.shippingNoteControl = new FormControl(null);
    this.discountTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl = new FormControl('0');

    this.createOrderForm = new FormGroup({
      orderDateControl: this.orderDateControl,
      wareHouseControl: this.wareHouseControl,
      orderStatusControl: this.orderStatusControl,
      quoteControl: this.quoteControl,
      orderContractControl: this.orderContractControl,
      sellerControl: this.sellerControl,
      descriptionControl: this.descriptionControl,
      noteControl: this.noteControl,
      customerControl: this.customerControl,
      cusNameControl: this.cusNameControl,
      customerMSTControl: this.customerMSTControl,
      customerPhoneControl: this.customerPhoneControl,
      customerAddressControl: this.customerAddressControl,
      paymentMethodControl: this.paymentMethodControl,
      daysAreOwedControl: this.daysAreOwedControl,
      maxDebtControl: this.maxDebtControl,
      bankAccountControl: this.bankAccountControl,
      customerGroupControl: this.customerGroupControl,
      customerCodeControl: this.customerCodeControl,
      customerNameControl: this.customerNameControl,
      customerTypeControl: this.customerTypeControl,
      newCusPaymentMethodControl: this.newCusPaymentMethodControl,
      newCusDaysAreOwedControl: this.newCusDaysAreOwedControl,
      newCusMaxDebtControl: this.newCusMaxDebtControl,
      paymentMethodQuoteControl: this.paymentMethodQuoteControl,
      daysAreOwedQuoteControl: this.daysAreOwedQuoteControl,
      maxDebtQuoteControl: this.maxDebtQuoteControl,
      receivedDateControl: this.receivedDateControl,
      receivedHourControl: this.receivedHourControl,
      recipientNameControl: this.recipientNameControl,
      locationOfShipmentControl: this.locationOfShipmentControl,
      placeOfDeliveryControl: this.placeOfDeliveryControl,
      recipientPhoneControl: this.recipientPhoneControl,
      recipientEmailControl: this.recipientEmailControl,
      shippingNoteControl: this.shippingNoteControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl
    });
  }

  setTable() {
    this.cols = [
      { field: 'Move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'ProductCode', header: 'Mã sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'ProductName', header: 'Tên sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'NameVendor', header: 'Nhà cung cấp', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'WareCode', header: 'Mã kho', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'ActualInventory', header: 'Tồn kho thực tế', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'BusinessInventory', header: 'Tồn kho kinh doanh', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'ExchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'DiscountValue', header: 'Chiết khấu', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'Vat', header: 'Thuế GTGT (%)', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'PriceInitial', header: 'Giá vốn', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'GuaranteeTime', header: 'Thời hạn bảo hành', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = [
      { field: 'Move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'ProductCode', header: 'Mã sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'ProductName', header: 'Tên sản phẩm dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];

    this.colsCost = [
      { field: 'costCode', header: 'Mã chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'costName', header: 'Tên chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsCost = this.colsCost;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
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

  setDefaultValue() {
    let defaultOrderStatus = this.listOrderStatus.find(x => x.orderStatusCode == "DRA");
    if (defaultOrderStatus) this.orderStatusControl.setValue(defaultOrderStatus);

    let defaultSeller = this.listEmployee.find(x => x.employeeId == this.employeeId);
    if (defaultSeller) this.sellerControl.setValue(defaultSeller);

    let defaultCustomerGroup = this.listCustomerGroup.find(x => x.categoryCode == 'MPH');
    if (defaultCustomerGroup) this.customerGroupControl.setValue(defaultCustomerGroup);

    this.customerCodeControl.setValidators([Validators.required, checkDuplicateCode(this.listCustomerCode), forbiddenSpaceText]);
    this.customerCodeControl.updateValueAndValidity();

    //Tạo đơn hàng từ Khách hàng
    if (this.customerId) {
      this.loading = false;
      //Nếu tạo đơn hàng từ một khách hàng
      let customer: Customer = this.listCustomer.find(x => x.customerId == this.customerId);
      if (customer) {
        this.customerControl.setValue(customer);
      }
    }
    //Tạo đơn hàng từ Báo giá
    else if (this.quoteId) {
      let quoteModel = this.listQuote.find(q => q.quoteId == this.quoteId);
      this.quoteObj = quoteModel;
      this.quoteControl.setValue(quoteModel);

      this.listContract = this.listOrderContract.filter(oc => oc.quoteId == this.quoteId);

      this.customerOrderModel.Seller = quoteModel.seller;
      if (quoteModel.objectType == 'CUSTOMER') {
        this.customerOrderModel.CustomerId = quoteModel.objectTypeId;
      }

      let customer: Customer = this.listCustomer.find(x => x.customerId == this.customerOrderModel.CustomerId);
      if (customer) {
        this.customerControl.setValue(customer);
      }

      this.customerOrderModel.CustomerContactId = quoteModel.customerContactId;
      this.customerOrderModel.Description = quoteModel.description;
      this.customerOrderModel.Note = quoteModel.note;
      this.customerOrderModel.PaymentMethod = quoteModel.paymentMethod;
      this.customerOrderModel.DaysAreOwed = quoteModel.daysAreOwed;
      this.customerOrderModel.MaxDebt = quoteModel.maxDebt;
      this.customerOrderModel.ReceivedDate = new Date(quoteModel.receivedDate);
      this.customerOrderModel.ReceivedHour = null;
      this.customerOrderModel.RecipientName = null;
      this.customerOrderModel.LocationOfShipment = null;
      this.customerOrderModel.ShippingNote = null;
      this.customerOrderModel.RecipientPhone = null;
      this.customerOrderModel.RecipientEmail = null;
      this.customerOrderModel.PlaceOfDelivery = null;
      this.customerOrderModel.Amount = quoteModel.amount;
      this.customerOrderModel.DiscountValue = quoteModel.discountValue;
      this.customerOrderModel.CreatedById = this.emptyGuid;
      this.customerOrderModel.CreatedDate = new Date();
      this.customerOrderModel.UpdatedById = null;
      this.customerOrderModel.UpdatedDate = null;
      this.customerOrderModel.DiscountType = quoteModel.discountType;
      this.customerOrderModel.BankAccountId = quoteModel.bankAccountId;
      this.customerOrderModel.QuoteId = this.quoteId;
      /* End */

      quoteModel.listDetail.forEach((element, index) => {
        let obj = new CustomerOrderDetail();
        // obj.OrderDetailId = element.orderDetailId;
        // obj.OrderId = element.orderId;
        obj.VendorId = element.vendorId;
        obj.ProductId = element.productId;
        obj.Quantity = element.quantity;
        obj.UnitPrice = element.unitPrice;
        obj.CurrencyUnit = element.currencyUnit;
        obj.ExchangeRate = element.exchangeRate;
        obj.Vat = element.vat;
        obj.DiscountType = element.discountType;
        obj.DiscountValue = element.discountValue;
        obj.Description = element.description;
        obj.OrderDetailType = element.orderDetailType;
        obj.UnitId = element.unitId;
        obj.IncurredUnit = element.incurredUnit;
        obj.Active = element.active;
        obj.ExplainStr = element.nameProduct;
        obj.ProductName = element.productName;
        obj.NameVendor = element.nameVendor;
        obj.ProductNameUnit = element.nameProductUnit;
        obj.NameMoneyUnit = element.nameMoneyUnit;
        obj.AmountDiscount = obj.DiscountType == true ? (element.quantity * element.unitPrice * element.exchangeRate * element.discountValue / 100) : (element.discountValue);
        obj.SumAmount = (element.quantity * element.unitPrice * element.exchangeRate) - obj.AmountDiscount + ((element.quantity * element.unitPrice * element.exchangeRate) - obj.AmountDiscount) * element.vat / 100;
        obj.PriceInitial = element.priceInitial;
        obj.IsPriceInitial = element.isPriceInitial;
        obj.WarrantyPeriod = element.warrantyPeriod;
        obj.ProductCode = element.productId ? this.listProduct.find(p => p.productId == element.productId).productCode : "";
        obj.OrderNumber = index + 1;

        element.quoteProductDetailProductAttributeValue.forEach(item => {
          let obtAttribute = new OrderProductDetailProductAttributeValue();
          obtAttribute.ProductId = item.productId;
          obtAttribute.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obtAttribute.ProductAttributeCategoryId = item.productAttributeCategoryId;
          obj.OrderProductDetailProductAttributeValue.push(obtAttribute);
        });

        this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, obj];
      });

      quoteModel.listCostDetail.forEach(element => {
        let costObj = new OrderCostDetail();
        // costObj.OrderCostDetailId = element.orderCostDetailId;
        costObj.costId = element.costId;
        // costObj.OrderId = element.orderId;
        costObj.quantity = element.quantity;
        costObj.unitPrice = element.unitPrice;
        costObj.costName = element.costName;
        costObj.costCode = element.costCode;
        costObj.active = element.active;
        costObj.sumAmount = element.quantity * element.unitPrice;

        this.listOrderCostDetailModel = [...this.listOrderCostDetailModel, costObj];
      });

      //Nhân viên bán hàng
      let seller: Employee = this.listEmployee.find(x => x.employeeId == this.customerOrderModel.Seller);
      if (seller) this.sellerControl.setValue(seller);

      //Mô tả
      this.descriptionControl.setValue(this.customerOrderModel.Description);

      //Ghi chú
      this.noteControl.setValue(this.customerOrderModel.Note);

      //Chiết khấu
      let discountType: DiscountType = this.discountTypeList.find(x => x.value == this.customerOrderModel.DiscountType);
      if (discountType) this.discountTypeControl.setValue(discountType);

      //Giá tri chiết khấu
      let discountValue = this.customerOrderModel.DiscountValue;
      if (discountValue) {
        this.discountValueControl.setValue(discountValue);
      } else {
        this.discountValueControl.setValue(0);
      }

      //Nếu báo giá cho khách hàng
      this.optionCustomer = '2';

      this.customerEmail = customer.customerEmail;
      this.customerPhone = customer.customerPhone;
      this.cusNameControl.setValue(this.customerEmail);
      this.customerPhoneControl.setValue( this.customerPhone);
      this.cusNameControl.setValue(customer.customerName);
      this.customerMSTControl.setValue(customer.taxCode);
      //Lấy địa chỉ của khách hàng
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
          this.customerAddressControl.setValue(this.fullAddress);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

      //Phương thức thanh toán
      let paymentMethod = this.listPaymentMethod.find(x => x.categoryId == this.customerOrderModel.PaymentMethod);
      if (paymentMethod) {
        this.paymentMethodControl.setValue(paymentMethod);
        if (paymentMethod.categoryCode == "BANK") {
          this.isShowBankAccount = true;
          this.listBankAccount = this.listCustomerBankAccount.filter(x => x.objectId == customer.customerId);
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount.find(x => x.bankAccountId == this.customerOrderModel.BankAccountId);
            this.bankAccountControl.setValue(toSelectBankAccount);
          } else {
            this.listBankAccount = [];
          }
        }
      }
      if (this.quoteId === null || this.quoteId === undefined || this.quoteId === '') {
        //Số ngày được nợ
        let daysAreOwed = this.customerOrderModel.DaysAreOwed;
        this.daysAreOwedControl.setValue(daysAreOwed);

        //Số nợ tối đa
        let maxDebt = this.customerOrderModel.MaxDebt;
        this.maxDebtControl.setValue(maxDebt);
      }
      this.autoGen(); //Lấy thông tin giao hàng

      this.calculatorAll();

      this.loading = false;

    }
    //Tạo đơn hàng từ Hợp đồng
    else if (this.contractId) {
      let objContract = this.listOrderContract.find(c => c.contractId == this.contractId);

      this.contractObj = objContract;
      this.orderContractControl.setValue(objContract);

      this.customerOrderModel.Seller = objContract.employeeId;

      this.customerOrderModel.CustomerId = objContract.customerId;

      let customer: Customer = this.listCustomer.find(x => x.customerId == objContract.customerId);

      if (customer) {
        this.customerControl.setValue(customer);
      }

      this.customerOrderModel.OrderContractId = objContract.contractId;
      this.customerOrderModel.Description = objContract.contractDescription;
      this.customerOrderModel.Note = objContract.contractNote;
      this.customerOrderModel.PaymentMethod = objContract.paymentMethodId;
      this.customerOrderModel.DaysAreOwed = null;
      this.customerOrderModel.MaxDebt = null;
      this.customerOrderModel.ReceivedDate = null;
      this.customerOrderModel.ReceivedHour = null;
      this.customerOrderModel.RecipientName = null;
      this.customerOrderModel.LocationOfShipment = null;
      this.customerOrderModel.ShippingNote = null;
      this.customerOrderModel.RecipientPhone = null;
      this.customerOrderModel.RecipientEmail = null;
      this.customerOrderModel.PlaceOfDelivery = null;
      this.customerOrderModel.Amount = objContract.amount;
      this.customerOrderModel.DiscountValue = objContract.discountValue;
      this.customerOrderModel.CreatedById = this.emptyGuid;
      this.customerOrderModel.CreatedDate = new Date();
      this.customerOrderModel.UpdatedById = null;
      this.customerOrderModel.UpdatedDate = null;
      this.customerOrderModel.DiscountType = objContract.discountType;
      this.customerOrderModel.BankAccountId = objContract.bankAccountId;
      this.customerOrderModel.QuoteId = objContract.quoteId;
      /* End */

      let quote = this.listQuote.find(q => q.quoteId === this.customerOrderModel.QuoteId);
      this.quoteControl.setValue(quote);
      this.quoteObj = quote;

      objContract.listDetail.forEach((element, index) => {
        let obj = new CustomerOrderDetail();
        // obj.OrderDetailId = element.orderDetailId;
        // obj.OrderId = element.orderId;
        obj.VendorId = element.vendorId;
        obj.ProductId = element.productId;
        obj.Quantity = element.quantity;
        obj.UnitPrice = element.unitPrice;
        obj.CurrencyUnit = element.currencyUnit;
        obj.ExchangeRate = element.exchangeRate;
        obj.Vat = element.vat;
        obj.DiscountType = element.discountType;
        obj.DiscountValue = element.discountValue;
        obj.Description = element.description;
        obj.OrderDetailType = element.orderDetailType;
        obj.UnitId = element.unitId;
        obj.IncurredUnit = element.incurredUnit;
        obj.Active = element.active;
        obj.ExplainStr = element.nameProduct;
        obj.NameVendor = element.nameVendor;
        obj.ProductNameUnit = element.nameProductUnit;
        obj.NameMoneyUnit = element.nameMoneyUnit;
        obj.AmountDiscount = obj.DiscountType == true ? (element.quantity * element.unitPrice * element.exchangeRate * element.discountValue / 100) : (element.discountValue);
        obj.SumAmount = (element.quantity * element.unitPrice * element.exchangeRate) - obj.AmountDiscount + ((element.quantity * element.unitPrice * element.exchangeRate) - obj.AmountDiscount) * element.vat / 100;
        obj.PriceInitial = element.priceInitial;
        obj.IsPriceInitial = element.isPriceInitial;
        obj.ProductName = element.nameProduct;
        obj.WarrantyPeriod = element.warrantyPeriod;
        obj.ProductCode = element.productId ? this.listProduct.find(p => p.productId == element.productId).productCode : "";
        obj.OrderNumber = index + 1;

        element.contractProductDetailProductAttributeValue.forEach(item => {
          let obtAttribute = new OrderProductDetailProductAttributeValue();
          obtAttribute.ProductId = item.productId;
          obtAttribute.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obtAttribute.ProductAttributeCategoryId = item.productAttributeCategoryId;
          obj.OrderProductDetailProductAttributeValue.push(obtAttribute);
        });

        this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, obj];
      });

      //Nhân viên bán hàng
      let seller: Employee = this.listEmployee.find(x => x.employeeId == this.customerOrderModel.Seller);
      if (seller) this.sellerControl.setValue(seller);

      //Mô tả
      this.descriptionControl.setValue(this.customerOrderModel.Description);

      //Ghi chú
      this.noteControl.setValue(this.customerOrderModel.Note);

      //Chiết khấu
      let discountType: DiscountType = this.discountTypeList.find(x => x.value == this.customerOrderModel.DiscountType);
      if (discountType) this.discountTypeControl.setValue(discountType);

      //Giá tri chiết khấu
      let discountValue = this.customerOrderModel.DiscountValue;
      if (discountValue) {
        this.discountValueControl.setValue(discountValue);
      } else {
        this.discountValueControl.setValue(0);
      }

      this.optionCustomer = '2';

      this.customerEmail = customer.customerEmail;
      this.customerPhone = customer.customerPhone;
      this.customerEmail = customer.customerEmail;
      this.customerPhone = customer.customerPhone;
      this.cusNameControl.setValue(this.customerEmail);
      this.customerPhoneControl.setValue( this.customerPhone);
      this.cusNameControl.setValue(customer.customerName);
      this.customerMSTControl.setValue(customer.taxCode);

      //Lấy địa chỉ của khách hàng
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
          this.customerAddressControl.setValue(this.fullAddress);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

      //Phương thức thanh toán
      let paymentMethod = this.listPaymentMethod.find(x => x.categoryId == this.customerOrderModel.PaymentMethod);
      if (paymentMethod) {
        this.paymentMethodControl.setValue(paymentMethod);
        if (paymentMethod.categoryCode == "BANK") {
          this.isShowBankAccount = true;
          this.listBankAccount = this.listCustomerBankAccount.filter(x => x.objectId == customer.customerId);
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount.find(x => x.bankAccountId == this.customerOrderModel.BankAccountId);
            this.bankAccountControl.setValue(toSelectBankAccount);
          } else {
            this.listBankAccount = [];
          }
        }
      }
      if (this.quoteId === null || this.quoteId === undefined || this.quoteId === '') {
        //Số ngày được nợ
        let daysAreOwed = this.customerOrderModel.DaysAreOwed;
        this.daysAreOwedControl.setValue(daysAreOwed);

        //Số nợ tối đa
        let maxDebt = this.customerOrderModel.MaxDebt;
        this.maxDebtControl.setValue(maxDebt);
      }

      this.autoGen();

      this.calculatorAll();

      this.loading = false;
    }
    else {
      this.loading = false;
    }
  }

  goDetailQuote() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/customer/quote-detail', { quoteId: this.quoteObj.quoteId }]));
    window.open(url, '_blank');
  }

  goDetaiContract() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/sales/contract-detail', { contractId: this.contractId }]));
    window.open(url, '_blank');
  }

  /*Event chọn radio button: Khách lẻ, Chọn khách hàng, Khách hàng mới*/
  chooseOptionCustomer() {
    this.autoGen(); //Lấy thông tin giao hàng
  }

  changeCustomer(customer: Customer, clear?: string) {
    //Nếu thay đổi khách hàng trên giao diện thì clear hết sản phẩm dịch vụ
    if (clear) {
      this.listCustomerOrderDetailModel = [];
      this.calculatorAll();
    }

    this.paymentMethodControl.setValue(null);
    this.daysAreOwedControl.setValue(null);
    this.maxDebtControl.setValue(null);
    this.isShowBankAccount = false;
    this.listBankAccount = [];
    this.bankAccountControl.setValue(null);
    this.customerEmail = '';
    this.customerPhone = '';
    this.fullAddress = '';

    if (customer) {
      this.customerEmail = customer.customerEmail;
      this.customerPhone = customer.customerPhone;
      this.cusObj = customer;
      this.customerPhoneControl.setValue(this.customerPhone);
      this.cusNameControl.setValue(customer.customerName);
      this.customerMSTControl.setValue(customer.taxCode);
      this.listQuoteObj = this.listQuote.filter(q => q.objectTypeId == customer.customerId);
      this.listContract = this.listOrderContract.filter(c => c.customerId == customer.customerId);

      if (customer.personInChargeId !== null) {
        this.quoteService.getEmployeeSale(this.listPersonInCharge, customer.personInChargeId).subscribe(response => {
          let result: any = response;
          if (result.listEmployee !== null && result.listEmployee !== undefined && result.listEmployee !== []) {
            this.listEmployee = result.listEmployee;

            let emp = this.listEmployee.find(e => e.employeeId == customer.personInChargeId);
            this.sellerControl.setValue(emp);
          }
        });
      }
      else {
        this.cusObj = null;
        this.listEmployee = [];
        this.sellerControl.setValue(null);
      }

      //Lấy địa chỉ của khách hàng
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
          this.customerAddressControl.setValue(this.fullAddress);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

      let paymentMethod = this.listPaymentMethod.find(x => x.categoryId == customer.paymentId);
      if (paymentMethod) {
        this.paymentMethodControl.setValue(paymentMethod);
        if (paymentMethod.categoryCode == "BANK") {
          this.changeMethodControl(paymentMethod);
        }
      }
      let daysAreOwed = customer.maximumDebtDays ? customer.maximumDebtDays : 0;
      this.daysAreOwedControl.setValue(daysAreOwed);
      let maxAmount = customer.maximumDebtValue ? customer.maximumDebtValue : 0;
      this.maxDebtControl.setValue(maxAmount);

      this.autoGen(); //Lấy thông tin giao hàng
    }
    else {
      this.cusNameControl.setValue('');
      this.customerAddressControl.setValue('');
      this.customerMSTControl.setValue('');
      this.customerPhoneControl.setValue('');
      this.recipientNameControl.setValue('');
      this.daysAreOwedControl.setValue('');
      this.maxDebtControl.setValue('');
      this.paymentMethodControl.setValue(null);
    }
  }

  changeQuote(event) {
    this.listCustomerOrderDetailModel = [];
    this.listOrderCostDetailModel = [];

    if (event) {
      event.listDetail.forEach((element, index) => {
        let obj = new CustomerOrderDetail();
        // obj.OrderDetailId = element.orderDetailId;
        // obj.OrderId = element.orderId;
        obj.VendorId = element.vendorId;
        obj.ProductId = element.productId;
        obj.Quantity = element.quantity;
        obj.UnitPrice = element.unitPrice;
        obj.CurrencyUnit = element.currencyUnit;
        obj.ExchangeRate = element.exchangeRate;
        obj.Vat = element.vat;
        obj.DiscountType = element.discountType;
        obj.DiscountValue = element.discountValue;
        obj.Description = element.description;
        obj.OrderDetailType = element.orderDetailType;
        obj.UnitId = element.unitId;
        obj.IncurredUnit = element.incurredUnit;
        obj.Active = element.active;
        obj.ExplainStr = element.nameProduct;
        obj.NameVendor = element.nameVendor;
        obj.ProductNameUnit = element.nameProductUnit;
        obj.ProductName = element.productName;
        obj.NameMoneyUnit = element.nameMoneyUnit;
        // obj.GuaranteeTime = element.guaranteeTime;
        // obj.GuaranteeDatetime = element.guaranteeDatetime;
        obj.AmountDiscount = obj.DiscountType == true ? (element.quantity * element.unitPrice * element.exchangeRate * element.discountValue / 100) : (element.discountValue);
        obj.SumAmount = (element.quantity * element.unitPrice * element.exchangeRate) - obj.AmountDiscount + ((element.quantity * element.unitPrice * element.exchangeRate) - obj.AmountDiscount) * element.vat / 100;
        // obj.ExpirationDate = element.expirationDate;
        // obj.WarehouseId = element.warehouseId;
        obj.PriceInitial = element.priceInitial;
        obj.IsPriceInitial = element.isPriceInitial;
        obj.WarrantyPeriod = element.warrantyPeriod;
        // obj.ActualInventory = element.actualInventory;
        // obj.BusinessInventory = element.businessInventory;
        obj.ProductCode = element.productId ? this.listProduct.find(p => p.productId == element.productId).productCode : "";
        obj.OrderNumber = index + 1;

        element.quoteProductDetailProductAttributeValue.forEach(item => {
          let obtAttribute = new OrderProductDetailProductAttributeValue();
          obtAttribute.ProductId = item.productId;
          obtAttribute.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obtAttribute.ProductAttributeCategoryId = item.productAttributeCategoryId;
          obj.OrderProductDetailProductAttributeValue.push(obtAttribute);
        });

        this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, obj];
      });

      event.listCostDetail.forEach(element => {
        let costObj = new OrderCostDetail();
        // costObj.OrderCostDetailId = element.orderCostDetailId;
        costObj.costId = element.costId;
        // costObj.OrderId = element.orderId;
        costObj.quantity = element.quantity;
        costObj.unitPrice = element.unitPrice;
        costObj.costName = element.costName;
        costObj.costCode = element.costCode;
        costObj.active = element.active;
        costObj.sumAmount = element.quantity * element.unitPrice;

        this.listOrderCostDetailModel = [...this.listOrderCostDetailModel, costObj];
      });
      this.listContract = this.listOrderContract.filter(oc => oc.quoteId == event.quoteId);
    }

    this.calculatorAll();
  }

  changeContract(value: any) {
    this.quoteControl.reset();
    this.listQuoteObj = [];
    this.listCustomerOrderDetailModel = [];
    this.listOrderCostDetailModel = [];
    if (value) {
      this.listQuoteObj = this.listQuote.filter(c => c.quoteId == value.quoteId);
    }
  }

  /*Event thay đổi Phương thức thanh toán*/
  changeMethodControl(value: Category) {
    if (value) {
      if (value.categoryCode == "BANK") {
        let customer: Customer = this.customerControl.value;

        if (customer) {
          //Nếu đã chọn khách hàng
          this.isShowBankAccount = true;
          let customerId = customer.customerId;

          this.listBankAccount = this.listCustomerBankAccount.filter(x => x.objectId == customerId);
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          } else {
            this.listBankAccount = [];
          }
        }
      } else if (value.categoryCode == "CASH") {
        this.isShowBankAccount = false;
        this.listBankAccount = [];
        this.bankAccountControl.setValue(null);
      }
    } else {
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
    }
  }

  /*Event khi nhập tên khách hàng*/
  changeCustomerName() {
    if (this.autoGenValue) {
      let customerName = this.customerNameControl.value;
      this.recipientNameControl.setValue(customerName);
    }
  }

  /*Lấy tên người nhận hàng*/
  autoGen() {
    if (this.autoGenValue) {
      switch (this.optionCustomer) {
        case "1":
          // this.recipientNameControl.setValue('Khách lẻ');
          // this.recipientPhoneControl.setValue(null);
          // this.recipientEmailControl.setValue(null);
          break;
        case "2":
          let selectCustomer: Customer = this.customerControl.value;
          if (selectCustomer) {
            this.recipientNameControl.setValue(selectCustomer.customerName);
            // this.recipientPhoneControl.setValue(selectCustomer.customerPhone);
            // this.recipientEmailControl.setValue(selectCustomer.customerEmail);
          } else {
            this.recipientNameControl.setValue(null);
            // this.recipientPhoneControl.setValue(null);
            // this.recipientEmailControl.setValue(null);
          }
          break;
        case "3":
          // let newCustomerName = this.customerNameControl.value;
          // this.recipientNameControl.setValue(newCustomerName);
          // this.recipientPhoneControl.setValue(null);
          // this.recipientEmailControl.setValue(null);
          break;
        case "4":
          // this.recipientNameControl.setValue(this.leadName);
          // this.recipientPhoneControl.setValue(this.leadPhone);
          // this.recipientEmailControl.setValue(this.leadEmail);
          break;
      }
    }
  }

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let cusGroupId = null;
    if (this.cusObj !== null && this.cusObj !== undefined) cusGroupId = this.cusObj.customerGroupId;
    let date = convertToUTCTime(this.orderDateControl.value);
    let wareHouse = this.wareHouseControl.value ? this.wareHouseControl.value.warehouseId : null;
    let ref = this.dialogService.open(OrderDetailDialogComponent, {
      data: {
        isCreate: true,
        cusGroupId: cusGroupId,
        dateOrder: date,
        warehouse: wareHouse
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
          let customerOrderDetailModel: CustomerOrderDetail = result.customerOrderDetailModel;

          //set orderNumber cho sản phẩm/dịch vụ mới thêm
          customerOrderDetailModel.OrderNumber = this.listCustomerOrderDetailModel.length + 1;

          this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, customerOrderDetailModel];

          this.calculatorAll();
        }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    var index = this.listCustomerOrderDetailModel.indexOf(dataRow);
    var OldArray = this.listCustomerOrderDetailModel[index];

    let titlePopup = '';
    if (OldArray.OrderDetailType == 0) {
      titlePopup = 'Sửa sản phẩm dịch vụ';
    } else if (OldArray.OrderDetailType == 1) {
      titlePopup = 'Sửa chi phí phát sinh';
    }
    let cusGroupId = null;
    if (this.cusObj !== null && this.cusObj !== undefined) cusGroupId = this.cusObj.customerGroupId;
    let date = convertToUTCTime(this.orderDateControl.value);
    let wareHouse = this.wareHouseControl.value ? this.wareHouseControl.value.warehouseId : null;

    let ref = this.dialogService.open(OrderDetailDialogComponent, {
      data: {
        isCreate: false,
        customerOrderDetailModel: OldArray,
        dateOrder: date,
        cusGroupId: cusGroupId,
        // warehouse: wareHouse
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
          this.listCustomerOrderDetailModel.splice(index, 1);
          let customerOrderDetailModel: CustomerOrderDetail = result.customerOrderDetailModel;
          this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, customerOrderDetailModel];

          //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
          this.listCustomerOrderDetailModel.sort((a, b) =>
            (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));

          this.calculatorAll();
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow, event: Event) {
    //this.translate.get('order.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listCustomerOrderDetailModel = this.listCustomerOrderDetailModel.filter(x => x != dataRow);

        //Đánh lại số OrderNumber
        this.listCustomerOrderDetailModel.forEach((item, index) => {
          item.OrderNumber = index + 1;
        });

        this.calculatorAll();
      }
    });
  }

  /*Thêm chi phí*/
  addCostQuote() {
    let ref = this.dialogService.open(PopupAddEditCostQuoteDialogComponent, {
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
          this.listOrderCostDetailModel.push(result.quoteDetailModel);

          this.calculatorAll();
        }
      }
    });
  }

  /*Sửa một chi phí*/
  onRowCostSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    var index = this.listOrderCostDetailModel.indexOf(dataRow);
    var OldArray = this.listOrderCostDetailModel[index];

    let titlePopup = 'Sửa chi phí';

    let ref = this.dialogService.open(PopupAddEditCostQuoteDialogComponent, {
      data: {
        isCreate: false,
        quoteDetailModel: OldArray
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
          this.listOrderCostDetailModel.splice(index, 1);
          this.listOrderCostDetailModel.push(result.quoteDetailModel);

          this.calculatorAll();
        }
      }
    });
  }

  /*Xóa một chi phí*/
  deleteCostItem(dataRow) {
    //this.translate.get('order.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listOrderCostDetailModel.indexOf(dataRow);
        this.listOrderCostDetailModel.splice(index, 1);

        this.calculatorAll();
      }
    });
  }

  /*Event khi thay đổi loại chiết khấu: Theo % hoặc Theo số tiền*/
  changeDiscountType(value: DiscountType) {
    this.discountValueControl.setValue('0');

    this.calculatorAll();
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
    }

    this.calculatorAll();
  }

  /*Tính lại tất cả các số phần tổng hợp đơn hàng*/
  calculatorAll() {
    //Tổng tiền hàng trước thuế: TotalSumAmountProduct = SUM(đơn giá sp * số lượng sp * tỷ giá sp - chiết khấu)
    //Tổng chi phí: TotalSumAmountCost = SUM(số lượng cp * đơn giá cp)
    //Tổng thuế VAT: TotalSumVatProduct = SUM(tiền thuế của sp/dịch vụ)
    //Tổng tiền chiết khấu: CustomerOrderTotalDiscount
    //  + Nếu chiết khấu là % = (Tổng tiền hàng trước thuế + Tổng chi phí + Tổng thuế VAT) * (% chiết khấu) / 100
    //  + Nếu chiết khấu là số tiền = Số tiền chiết khấu
    //Tổng thanh toán: CustomerOrderAmountAfterDiscount = Tổng tiền hàng trước thuế + Tổng thuế VAT + Tổng chi phí- Tổng tiền chiết khấu
    //Tổng tiền vốn: TotalPriceInitial = SUM(số lượng sp * giá vốn)
    //Lợi nhuận tạm tính: AmountPriceProfit = Tổng thanh toán - Tổng tiền vốn
    //% Lợi nhuận tạm tính: ValuePriceProfit = Lợi nhuận tạm tính / Tổng thanh toán * 100

    let defaultNumberType = ParseStringToFloat(this.defaultNumberType);

    this.TotalSumAmountProduct = 0;
    this.TotalSumAmountCost = 0;
    this.TotalSumVatProduct = 0;
    this.CustomerOrderTotalDiscount = 0;
    this.CustomerOrderAmountAfterDiscount = 0;
    this.TotalPriceInitial = 0;
    this.AmountPriceProfit = 0;
    this.ValuePriceProfit = 0;

    //Tổng tiền hàng trước thuế: TotalSumAmountProduct = SUM(đơn giá sp * số lượng sp * tỷ giá sp - chiết khấu)
    //Tổng thuế VAT: TotalSumVatProduct = SUM(tiền thuế của sp/dịch vụ)
    this.listCustomerOrderDetailModel.forEach(item => {
      let price = item.UnitPrice * item.Quantity * item.ExchangeRate;
      let discount = 0;
      if (item.DiscountType == true) {
        discount = price * item.DiscountValue / 100;
      }
      else {
        discount = item.DiscountValue;
      }
      this.TotalSumAmountProduct += this.roundNumber(price - discount, defaultNumberType);
      this.TotalSumVatProduct += this.roundNumber(((price - discount) * item.Vat / 100), defaultNumberType);
    });

    //Tổng chi phí: TotalSumAmountCost = SUM(số lượng cp * đơn giá cp)
    this.listOrderCostDetailModel.forEach(item => {
      this.TotalSumAmountCost += this.roundNumber(item.quantity * item.unitPrice, defaultNumberType);
    });

    //Tổng tiền chiết khấu: CustomerOrderTotalDiscount
    //  + Nếu chiết khấu là % = (Tổng tiền hàng trước thuế + Tổng chi phí + Tổng thuế VAT) * (% chiết khấu) / 100
    let discountType: DiscountType = this.discountTypeControl.value;
    if (discountType.value == true) {
      this.CustomerOrderTotalDiscount = (this.TotalSumAmountProduct + this.TotalSumAmountCost + this.TotalSumVatProduct) *
        ParseStringToFloat(this.discountValueControl.value.toString()) / 100;
      this.CustomerOrderTotalDiscount = this.roundNumber(this.CustomerOrderTotalDiscount, defaultNumberType);
    }
    //  + Nếu chiết khấu là số tiền = Số tiền chiết khấu
    else {
      this.CustomerOrderTotalDiscount = ParseStringToFloat(this.discountValueControl.value.toString());
      this.CustomerOrderTotalDiscount = this.roundNumber(this.CustomerOrderTotalDiscount, defaultNumberType);
    }

    //Tổng thanh toán: CustomerOrderAmountAfterDiscount = Tổng tiền hàng trước thuế + Tổng thuế VAT + Tổng chi phí- Tổng tiền chiết khấu
    this.CustomerOrderAmountAfterDiscount = this.roundNumber(this.TotalSumAmountProduct + this.TotalSumVatProduct + this.TotalSumAmountCost - this.CustomerOrderTotalDiscount, defaultNumberType);

    //Tổng tiền vốn: TotalPriceInitial = SUM(số lượng sp * giá vốn)
    this.listCustomerOrderDetailModel.forEach(item => {
      //Chỉ tính giá vốn của sản phẩm
      if (item.OrderDetailType == 0) {
        this.TotalPriceInitial += this.roundNumber(item.Quantity * item.PriceInitial ?? 0, defaultNumberType);
      }
    });

    //Lợi nhuận tạm tính: AmountPriceProfit = Tổng thanh toán - Tổng tiền vốn
    this.AmountPriceProfit = this.roundNumber(this.CustomerOrderAmountAfterDiscount - this.TotalPriceInitial, defaultNumberType);

    //% Lợi nhuận tạm tính: ValuePriceProfit = Lợi nhuận tạm tính / Tổng thanh toán * 100
    this.ValuePriceProfit = this.roundNumber(this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100, defaultNumberType);
  }

  createOrder(value: boolean) {
    /* Tắt/Bật validate Form theo loại Thông tin khách hàng */
    switch (this.optionCustomer) {
      case '1':
        //Khách lẻ

        this.customerControl.setValidators(null);
        this.customerControl.updateValueAndValidity();

        this.customerGroupControl.setValidators(null);
        this.customerGroupControl.updateValueAndValidity();

        this.customerCodeControl.setValidators(null);
        this.customerCodeControl.updateValueAndValidity();

        this.customerNameControl.setValidators(null);
        this.customerNameControl.updateValueAndValidity();
        break;
      case '2':
        //Chọn khách hàng

        //Tắt validate form Thêm khách hàng mới
        this.customerGroupControl.setValidators(null);
        this.customerGroupControl.updateValueAndValidity();

        this.customerCodeControl.setValidators(null);
        this.customerCodeControl.updateValueAndValidity();

        this.customerNameControl.setValidators(null);
        this.customerNameControl.updateValueAndValidity();

        //Bật validate form Chọn khách hàng
        this.customerControl.setValidators([Validators.required]);
        this.customerControl.updateValueAndValidity();
        break;
      case '3':
        //Thêm khách hàng mới

        //Tắt validate form Chọn khách hàng
        this.customerControl.setValidators(null);
        this.customerControl.updateValueAndValidity();

        //Bật validate form Thêm khách hàng mới
        this.customerGroupControl.setValidators([Validators.required]);
        this.customerGroupControl.updateValueAndValidity();

        this.customerCodeControl.setValidators([Validators.required, checkDuplicateCode(this.listCustomerCode), forbiddenSpaceText]);
        this.customerCodeControl.updateValueAndValidity();

        this.customerNameControl.setValidators([Validators.required, forbiddenSpaceText]);
        this.customerNameControl.updateValueAndValidity();
        break;
      case '4':
        //Bào giá Khách hàng tiềm năng

        this.customerControl.setValidators(null);
        this.customerControl.updateValueAndValidity();

        this.customerCodeControl.setValidators(null);
        this.customerCodeControl.updateValueAndValidity();

        this.customerNameControl.setValidators(null);
        this.customerNameControl.updateValueAndValidity();
        break;
    }
    /*End*/

    if (!this.createOrderForm.valid) {
      Object.keys(this.createOrderForm.controls).forEach(key => {
        if (this.createOrderForm.controls[key].valid == false) {
          this.createOrderForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.createOrderForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else if (this.listCustomerOrderDetailModel.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm dịch vụ được chọn' };
      this.showMessage(msg);
    } else {
      /* Convert Time */
      this.listCustomerOrderDetailModel.forEach(item => {
        let guaranteeDatetime = item.GuaranteeDatetime;
        if (guaranteeDatetime) item.GuaranteeDatetime = convertToUTCTime(guaranteeDatetime);

        let expirationDate = item.ExpirationDate;
        if (expirationDate) item.ExpirationDate = convertToUTCTime(expirationDate);
      });
      /* End */

      if (this.optionCustomer == '1') {
        // this.typeAccount = 1;

        // this.customerOrderModel.OrderDate = convertToUTCTime(this.orderDateControl.value);
        // let seller: Employee = this.sellerControl.value;
        // if (seller) this.customerOrderModel.Seller = seller.employeeId;
        // this.customerOrderModel.Description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
        // this.customerOrderModel.Note = this.noteControl.value == null ? null : this.noteControl.value.trim();
        // this.customerOrderModel.CustomerId = this.emptyGuid;
        // let discountType: DiscountType = this.discountTypeControl.value;
        // this.customerOrderModel.DiscountType = discountType.value;
        // let receivedDate = this.receivedDateControl.value;
        // if (receivedDate) {
        //   receivedDate = convertToUTCTime(receivedDate);
        //   this.customerOrderModel.ReceivedDate = receivedDate;
        // }
        // let receivedHour = this.receivedHourControl.value;
        // if (receivedHour) {
        //   receivedHour = this.convertDateToTimeSpan(receivedHour);
        //   this.customerOrderModel.ReceivedHour = receivedHour;
        // }
        // this.customerOrderModel.RecipientName = this.recipientNameControl.value.trim();
        // this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
        // this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
        // this.customerOrderModel.RecipientPhone = this.recipientPhoneControl.value;
        // this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
        // this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();
        // this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
        // let status: OrderStatus = this.orderStatusControl.value;
        // this.customerOrderModel.StatusId = status.orderStatusId;

        // //thêm autoGenValue vào model
        // this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;

        // this.loading = true;
        // this.awaitResult = true;
        // this.customerOrderService.CreateCustomerOrder(
        //   this.customerOrderModel,
        //   this.listCustomerOrderDetailModel,
        //   this.listOrderCostDetailModel,
        //   this.typeAccount,
        //   this.contactModel,
        //   this.quoteId).subscribe(response => {
        //     let result: any = response;
        //     if (result.statusCode == 200) {
        //       // this.sendEmail(result);
        //       if (value) {
        //         //Lưu và thêm mới
        //         this.resetForm();
        //         let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng thành công' };
        //         this.showMessage(msg);
        //       } else {
        //         //Lưu
        //         this.router.navigate(['/order/order-detail', { customerOrderID: result.customerOrderID }]);
        //       }
        //     } else {
        //       let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        //       this.showMessage(msg);
        //       this.awaitResult = false;
        //       this.loading = false;
        //     }
        //   });
      } else if (this.optionCustomer == '2') {
        this.typeAccount = 2;

        this.customerOrderModel.OrderDate = convertToUTCTime(this.orderDateControl.value);
        let seller: Employee = this.sellerControl.value;
        if (seller) this.customerOrderModel.Seller = seller.employeeId;
        this.customerOrderModel.Description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
        this.customerOrderModel.Note = this.noteControl.value == null ? null : this.noteControl.value.trim();
        let selectCustomer: Customer = this.customerControl.value;
        if (selectCustomer) this.customerOrderModel.CustomerId = selectCustomer.customerId;
        let selectPaymentMethod: Category = this.paymentMethodControl.value;
        if (selectPaymentMethod) this.customerOrderModel.PaymentMethod = selectPaymentMethod.categoryId;
        let discountType: DiscountType = this.discountTypeControl.value;
        this.customerOrderModel.DiscountType = discountType.value;
        let selectBankAccount: BankAccount = this.bankAccountControl.value;
        if (selectBankAccount) this.customerOrderModel.BankAccountId = selectBankAccount.bankAccountId;
        let daysAreOwed = this.daysAreOwedControl.value;
        if (daysAreOwed) {
          this.customerOrderModel.DaysAreOwed = ParseStringToFloat(daysAreOwed);
        } else {
          this.customerOrderModel.DaysAreOwed = 0;
        }
        let maxDebt = this.maxDebtControl.value;
        if (maxDebt) {
          this.customerOrderModel.MaxDebt = ParseStringToFloat(maxDebt);
        }
        else {
          this.customerOrderModel.MaxDebt = 0;
        }
        let receivedDate = this.receivedDateControl.value;
        if (receivedDate) {
          receivedDate = convertToUTCTime(receivedDate);
          this.customerOrderModel.ReceivedDate = receivedDate;
        }
        let receivedHour = this.receivedHourControl.value;
        if (receivedHour) {
          receivedHour = this.convertDateToTimeSpan(receivedHour);
          this.customerOrderModel.ReceivedHour = receivedHour;
        }

        let contractObj = this.orderContractControl.value;
        if (contractObj) {
          this.customerOrderModel.OrderContractId = contractObj.contractId;
        }

        this.customerOrderModel.RecipientName = this.recipientNameControl.value ? this.recipientNameControl.value.trim() : "";
        this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
        this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
        this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
        this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();

        //Amount = Tổng tiền hàng trước thuế + Tổng tiền thuế VAT
        this.customerOrderModel.Amount = this.TotalSumAmountProduct + this.TotalSumVatProduct;

        this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
        let status: OrderStatus = this.orderStatusControl.value;
        this.customerOrderModel.StatusId = status.orderStatusId;
        this.customerOrderModel.CustomerName = this.cusNameControl.value ? this.cusNameControl.value.trim() : "";
        this.customerOrderModel.CustomerAddress = this.customerAddressControl.value ? this.customerAddressControl.value.trim() : "";
        this.customerOrderModel.RecipientPhone = this.customerPhoneControl.value ? this.customerPhoneControl.value.trim() : ""; // Lưu số điện thoại thông tin khách hàng
        this.customerOrderModel.QuoteId = this.quoteControl.value ? this.quoteControl.value.quoteId : null;
        this.customerOrderModel.WarehouseId = this.wareHouseControl.value ? this.wareHouseControl.value.warehouseId : null;

        //thêm autoGenValue vào model
        this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;
        this.translate.get('order.messages_confirm.confirm_check').subscribe(value => {
          //Khách hàng này đã vượt quá số tiền nợ tối đa! Bạn có chắc chắn tiếp tục?
          this.messageConfirm = value;
        });

        //Kiểm tra số nợ tối đa của khách hàng
        this.customerOrderService.checkBeforCreateOrUpdateOrder(this.customerOrderModel.CustomerId, this.customerOrderModel.MaxDebt, this.CustomerOrderAmountAfterDiscount).subscribe(responseCheck => {
          let resultCheck: any = responseCheck;
          if (resultCheck.statusCode == 200) {

            //Nếu số nợ tối đa còn lại của khách hàng nhỏ hơn tổng thanh toán của đơn hàng thì hiển thị cảnh báo
            if (resultCheck.isCheckMaxDebt == true) {
              this.confirmationService.confirm({
                message: this.messageConfirm,
                accept: () => {
                  this.loading = true;
                  this.awaitResult = true;
                  this.customerOrderService.CreateCustomerOrder(
                    this.customerOrderModel,
                    this.listCustomerOrderDetailModel,
                    this.listOrderCostDetailModel,
                    this.typeAccount,
                    this.contactModel,
                    this.quoteId
                  ).subscribe(response => {
                    let result: any = response;
                    if (result.statusCode == 200) {
                      // this.sendEmail(result);
                      if (value) {
                        //Lưu và thêm mới
                        this.resetForm();
                        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
                        this.showMessage(msg);
                      } else {
                        //Lưu
                        this.router.navigate(['/order/order-detail', { customerOrderID: result.customerOrderID }]);
                      }
                    } else {
                      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                      this.showMessage(msg);
                      this.awaitResult = false;
                      this.loading = false;
                    }
                  });
                }
              });
            }
            else {
              this.awaitResult = true;
              this.customerOrderService.CreateCustomerOrder(
                this.customerOrderModel,
                this.listCustomerOrderDetailModel,
                this.listOrderCostDetailModel,
                this.typeAccount,
                this.contactModel,
                this.quoteId
              ).subscribe(response => {
                let result: any = response;
                if (result.statusCode == 200) {
                  // this.sendEmail(result);
                  if (value) {
                    //Lưu và thêm mới
                    this.resetForm();
                    let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng thành công' };
                    this.showMessage(msg);
                  } else {
                    //Lưu
                    this.router.navigate(['/order/order-detail', { customerOrderID: result.customerOrderID }]);
                  }
                } else {
                  let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                  this.showMessage(msg);
                  this.awaitResult = false;
                  this.loading = false;
                }
              });
            }
          }
          else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: resultCheck.messageCode };
            this.showMessage(msg);
            this.awaitResult = false;
            this.loading = false;
          }
        });
      } else if (this.optionCustomer == '3') {
        // this.typeAccount = 3;

        // let customerType: CustomerType = this.customerTypeControl.value;
        // this.customerModel.CustomerType = customerType.typeValue;
        // this.customerModel.CustomerCode = this.customerCodeControl.value.trim();
        // let customerGroup: Category = this.customerGroupControl.value;
        // this.customerModel.CustomerGroupId = customerGroup.categoryId;
        // let paymentMethod: Category = this.newCusPaymentMethodControl.value;
        // if (paymentMethod) {
        //   this.customerOrderModel.PaymentMethod = paymentMethod.categoryId;
        // } else {
        //   this.customerOrderModel.PaymentMethod = null;
        // }

        // this.customerModel.CustomerName = this.customerNameControl.value.trim();

        // let newCusDaysAreOwed = this.newCusDaysAreOwedControl.value;
        // if (newCusDaysAreOwed) {
        //   this.customerModel.MaximumDebtDays = ParseStringToFloat(newCusDaysAreOwed);
        // } else {
        //   this.customerModel.MaximumDebtDays = null;
        // }

        // let newCusMaxDebt = this.newCusMaxDebtControl.value;
        // if (newCusMaxDebt) {
        //   this.customerModel.MaximumDebtValue = ParseStringToFloat(newCusMaxDebt);
        // } else {
        //   this.customerModel.MaximumDebtValue = null;
        // }

        // this.customerService.quickCreateCustomer(
        //   this.auth.UserId,
        //   this.customerModel.CustomerType,
        //   this.customerModel.CustomerCode,
        //   this.customerModel.CustomerGroupId,
        //   this.customerOrderModel.PaymentMethod,
        //   this.customerModel.CustomerName,
        //   this.customerModel.MaximumDebtValue,
        //   this.customerModel.MaximumDebtDays,
        //   new Date()).subscribe(response => {
        //     let result: any = response;
        //     if (result.statusCode == 200) {
        //       this.customerOrderModel.OrderDate = convertToUTCTime(this.orderDateControl.value);
        //       let seller: Employee = this.sellerControl.value;
        //       if (seller) this.customerOrderModel.Seller = seller.employeeId;
        //       this.customerOrderModel.Description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
        //       this.customerOrderModel.Note = this.noteControl.value == null ? null : this.noteControl.value.trim();
        //       this.customerOrderModel.CustomerId = result.customerID;

        //       let paymentMethod: Category = this.newCusPaymentMethodControl.value;
        //       if (paymentMethod) this.customerOrderModel.PaymentMethod = paymentMethod.categoryId;

        //       let discountType: DiscountType = this.discountTypeControl.value;
        //       this.customerOrderModel.DiscountType = discountType.value;

        //       let daysAreOwed = this.newCusDaysAreOwedControl.value;
        //       if (daysAreOwed) {
        //         this.customerOrderModel.DaysAreOwed = ParseStringToFloat(daysAreOwed);
        //       }

        //       let maxDebt = this.newCusMaxDebtControl.value;
        //       if (maxDebt) {
        //         this.customerOrderModel.MaxDebt = ParseStringToFloat(maxDebt);
        //       }

        //       let receivedDate = this.receivedDateControl.value;
        //       if (receivedDate) {
        //         receivedDate = convertToUTCTime(receivedDate);
        //         this.customerOrderModel.ReceivedDate = receivedDate;
        //       }
        //       let receivedHour = this.receivedHourControl.value;
        //       if (receivedHour) {
        //         receivedHour = this.convertDateToTimeSpan(receivedHour);
        //         this.customerOrderModel.ReceivedHour = receivedHour;
        //       }
        //       this.customerOrderModel.RecipientName = this.recipientNameControl.value.trim();
        //       this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
        //       this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
        //       this.customerOrderModel.RecipientPhone = this.recipientPhoneControl.value;
        //       this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
        //       this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();
        //       this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
        //       let status: OrderStatus = this.orderStatusControl.value;
        //       this.customerOrderModel.StatusId = status.orderStatusId;
        //       //thêm autoGenValue vào model
        //       this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;

        //       this.loading = true;
        //       this.awaitResult = true;
        //       this.customerOrderService.CreateCustomerOrder(this.customerOrderModel,
        //         this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount, this.contactModel, this.quoteId).subscribe(response => {
        //           let result: any = response;
        //           if (result.statusCode == 200) {
        //             // this.sendEmail(result);
        //             if (value) {
        //               //Lưu và thêm mới
        //               this.resetForm();
        //               let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng thành công' };
        //               this.showMessage(msg);
        //             } else {
        //               //Lưu
        //               this.router.navigate(['/order/order-detail', { customerOrderID: result.customerOrderID }]);
        //             }
        //           } else {
        //             let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        //             this.showMessage(msg);
        //             this.awaitResult = false;
        //             this.loading = false;
        //           }
        //         });
        //     } else {
        //       let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        //       this.showMessage(msg);
        //     }
        //   });
      } else if (this.optionCustomer == '4') {
        // this.typeAccount = 4;

        // this.customerOrderModel.OrderDate = convertToUTCTime(this.orderDateControl.value);
        // let seller: Employee = this.sellerControl.value;
        // if (seller) this.customerOrderModel.Seller = seller.employeeId;
        // this.customerOrderModel.Description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
        // this.customerOrderModel.Note = this.noteControl.value == null ? null : this.noteControl.value.trim();
        // this.customerOrderModel.CustomerId = this.emptyGuid;
        // let discountType: DiscountType = this.discountTypeControl.value;
        // this.customerOrderModel.DiscountType = discountType.value;
        // let receivedDate = this.receivedDateControl.value;
        // if (receivedDate) {
        //   receivedDate = convertToUTCTime(receivedDate);
        //   this.customerOrderModel.ReceivedDate = receivedDate;
        // }
        // let receivedHour = this.receivedHourControl.value;
        // if (receivedHour) {
        //   receivedHour = this.convertDateToTimeSpan(receivedHour);
        //   this.customerOrderModel.ReceivedHour = receivedHour;
        // }
        // this.customerOrderModel.RecipientName = this.recipientNameControl.value.trim();
        // this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
        // this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
        // this.customerOrderModel.RecipientPhone = this.recipientPhoneControl.value;
        // this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
        // this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();
        // this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
        // let status: OrderStatus = this.orderStatusControl.value;
        // this.customerOrderModel.StatusId = status.orderStatusId;

        // //thêm autoGenValue vào model
        // this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;

        // this.loading = true;
        // this.awaitResult = true;
        // this.customerOrderService.CreateCustomerOrder(
        //   this.customerOrderModel,
        //   this.listCustomerOrderDetailModel,
        //   this.listOrderCostDetailModel,
        //   this.typeAccount,
        //   this.contactModel,
        //   this.quoteId).subscribe(response => {
        //     let result: any = response;
        //     if (result.statusCode == 200) {
        //       // this.sendEmail(result);
        //       if (value) {
        //         //Lưu và thêm mới
        //         this.resetForm();
        //         let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng thành công' };
        //         this.showMessage(msg);
        //       } else {
        //         //Lưu
        //         this.router.navigate(['/order/order-detail', { customerOrderID: result.customerOrderID }]);
        //       }
        //     } else {
        //       let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        //       this.showMessage(msg);
        //       this.awaitResult = false;
        //       this.loading = false;
        //     }
        //   });
      }
    }
  }

  /* Reset form tạo đơn hàng */
  resetForm() {
    if (this.quoteId || this.customerId)
      this.router.navigate(['/order/create']);
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
      this.isInvalidForm = false; //Ẩn icon-warning-active
    }

    //Reset các control trong Section Thông tin khách hàng
    this.resetSectionCusInforControl();

    //Ngày đặt hàng
    this.orderDateControl.setValue(new Date());

    //Trạng thái đơn hàng
    let defaultOrderStatus = this.listOrderStatus.find(x => x.orderStatusCode == "DRA");
    if (defaultOrderStatus) this.orderStatusControl.setValue(defaultOrderStatus);

    //Nhân viên bán hàng
    let defaultSeller = this.listEmployee.find(x => x.employeeId == this.employeeId);
    if (defaultSeller) this.sellerControl.setValue(defaultSeller);

    //Số báo giá
    this.quoteControl.reset();

    //Số hợp đồng
    this.orderContractControl.reset();

    //Mã kho
    this.wareHouseControl.reset();

    //Tên kho
    this.warehouseName = '';

    //Địa chỉ giao hàng
    this.placeOfDeliveryControl.reset();

    //Diễn giải
    this.descriptionControl.setValue(null);

    //Ghi chú
    this.noteControl.setValue(null);

    this.optionCustomer = '2';

    this.autoGenValue = true;

    //Loại chiết khấu theo tổng đơn hàng
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));

    //Gía trị chiết khấu theo tổng đơn hàng
    this.discountValueControl.setValue('0');

    //Danh sách sản phẩm dịch vụ
    this.listCustomerOrderDetailModel = [];

    //Danh sách chi phí
    this.listOrderCostDetailModel = [];

    //Tính lại các giá trị phần tổng hợp
    this.calculatorAll();

    this.customerOrderModel = new CustomerOrder();
    this.contactModel = new ContactModel();

    this.awaitResult = false;
    this.loading = false;
  }

  /* Reset các control trong Section Thông tin khách hàng */
  resetSectionCusInforControl() {
    switch (this.optionCustomer) {
      case '1':
        //Khách lẻ
        // this.customerControl.setValidators(null);
        // this.customerControl.updateValueAndValidity();

        // this.customerGroupControl.setValidators(null);
        // this.customerGroupControl.updateValueAndValidity();

        // this.customerCodeControl.setValidators(null);
        // this.customerCodeControl.updateValueAndValidity();

        // this.customerNameControl.setValidators(null);
        // this.customerNameControl.updateValueAndValidity();

        // this.customerTypeControl.setValue(this.listCustomerType[0]);

        // this.leadName = '';
        // this.leadEmail = '';
        // this.leadPhone = '';
        // this.leadFullAddress = '';
        // this.paymentMethodQuoteControl.setValue(null);
        // this.daysAreOwedQuoteControl.setValue(null);
        // this.maxDebtQuoteControl.setValue(null);
        break;
      case '2':
        //Chọn khách hàng

        //Bật validate form Chọn khách hàng
        this.customerControl.reset();
        this.customerControl.setValidators([Validators.required]);
        this.customerControl.updateValueAndValidity();

        //Tên khách hàng
        this.cusNameControl.reset();

        //Địa chỉ
        this.customerAddressControl.reset();

        //Mã số thuế
        this.customerMSTControl.reset();

        //Điện thoại
        this.customerPhoneControl.reset();

        //Người nhận hàng
        this.recipientNameControl.reset();

        //Số ngày được nợ
        this.daysAreOwedControl.setValue('0');

        //Số nợ tối đa
        this.maxDebtControl.setValue('0');

        //Phương thức thanh toán
        this.paymentMethodControl.setValue(null);

        //Tài khoản ngân hàng
        this.isShowBankAccount = false;
        this.listBankAccount = [];
        this.bankAccountControl.setValue(null);

        break;
      case '3':
        //Thêm khách hàng mới

        // //Tắt validate form Chọn khách hàng
        // this.customerControl.setValidators(null);
        // this.customerControl.updateValueAndValidity();

        // //Bật validate form Thêm khách hàng mới
        // this.customerGroupControl.setValidators([Validators.required]);
        // this.customerGroupControl.updateValueAndValidity();

        // this.customerCodeControl.setValidators([Validators.required, checkDuplicateCode(this.listCustomerCode), forbiddenSpaceText]);
        // this.customerCodeControl.updateValueAndValidity();

        // this.customerNameControl.setValidators([Validators.required, forbiddenSpaceText]);
        // this.customerNameControl.updateValueAndValidity();

        // this.leadName = '';
        // this.leadEmail = '';
        // this.leadPhone = '';
        // this.leadFullAddress = '';
        // this.paymentMethodQuoteControl.setValue(null);
        // this.daysAreOwedQuoteControl.setValue(null);
        // this.maxDebtQuoteControl.setValue(null);
        break;
      case '4':
        //Bào giá Khách hàng tiềm năng

        // this.customerControl.setValidators(null);
        // this.customerControl.updateValueAndValidity();

        // this.customerCodeControl.setValidators(null);
        // this.customerCodeControl.updateValueAndValidity();

        // this.customerNameControl.setValidators(null);
        // this.customerNameControl.updateValueAndValidity();
        break;
    }
  }

  convertDateToTimeSpan(Time: Date): string {
    let result = '';
    let Hour = Time.getHours().toString();
    let Minute = Time.getMinutes().toString();
    result = Hour + ":" + Minute;

    return result;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  cancel() {
    this.router.navigate(['order/list']);
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  showTotalOrder() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  // sendEmail(result: any) {
  //   const moneyPipe = new NumberToStringPipe();
  //   let sendEmailModel: SendEmailModel = result.sendEmailEntityModel;
  //   let sumAmountDiscount_temp = 0;
  //   let sumAmount_temp = 0;
  //   sendEmailModel.ListDetailToSendEmailInOrder = [];//reset
  //   this.listCustomerOrderDetailModel.forEach(orderDetail => {
  //     let newOrderDetail = new OrderDetailToSendEmailModel();
  //     newOrderDetail.ProductName = orderDetail.ExplainStr ? orderDetail.ExplainStr : "";
  //     newOrderDetail.ProductNameUnit = orderDetail.ProductNameUnit ? orderDetail.ProductNameUnit : "";
  //     newOrderDetail.UnitPrice = orderDetail.UnitPrice ? orderDetail.UnitPrice.toLocaleString('en') : "0";
  //     newOrderDetail.Quantity = orderDetail.Quantity ? orderDetail.Quantity.toLocaleString('en') : "0";
  //     newOrderDetail.Vat = orderDetail.Quantity ? orderDetail.Vat.toLocaleString('en') : "0";
  //     newOrderDetail.DiscountValue = orderDetail.DiscountValue ? orderDetail.DiscountValue.toLocaleString('en') : "0";
  //     if (orderDetail.DiscountType == true) {
  //       newOrderDetail.DiscountValue += "%";
  //     }
  //     newOrderDetail.SumAmount = orderDetail.SumAmount ? orderDetail.SumAmount.toLocaleString('en') : "0";
  //     newOrderDetail.AmountDiscountPerProduct = orderDetail.AmountDiscount ? orderDetail.AmountDiscount.toLocaleString('en') : "0";;
  //     sumAmountDiscount_temp += ParseStringToFloat(newOrderDetail.AmountDiscountPerProduct);
  //     sumAmount_temp += ParseStringToFloat(newOrderDetail.SumAmount);
  //     sendEmailModel.ListDetailToSendEmailInOrder.push(newOrderDetail);
  //   });
  //   sendEmailModel.SumAmountDiscountByProductInOder = sumAmountDiscount_temp.toLocaleString('en');  //tổng tiền chiết khấu theo sản phẩm
  //   sendEmailModel.SumAmountBeforeDiscount = sumAmount_temp.toLocaleString('en') + " VND";
  //   //lấy chiết khấu tổng và tổng tiền sau khi trừ chiết khấu tổng
  //   sendEmailModel.SumAmountBeforeDiscount = this.customerOrderModel.Amount.toLocaleString('en') + " VND";
  //   sendEmailModel.SumAmountDiscountByOrder = this.customerOrderModel.DiscountValue.toLocaleString('en');
  //   if (this.customerOrderModel.DiscountType == true) {
  //     sendEmailModel.SumAmountDiscountByOrder += "%";
  //   }
  //   sendEmailModel.SumAmountAfterDiscount = this.CustomerOrderAmountAfterDiscount.toLocaleString('en');
  //   sendEmailModel.SumAmountTransformInOrder = moneyPipe.transform(this.CustomerOrderAmountAfterDiscount, this.defaultNumberType);
  //   sendEmailModel.SendDetailProductInOrder = true;
  //   this.emailConfigService.sendEmail(8, sendEmailModel).subscribe(reponse => {
  //     //let result = <any>response;
  //   });
  // }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  /* Chuyển item lên một cấp */
  moveUp(data: CustomerOrderDetail) {
    let currentOrderNumber = data.OrderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listCustomerOrderDetailModel.find(x => x.OrderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = preOrderNumber;

    //Xóa 2 item
    this.listCustomerOrderDetailModel = this.listCustomerOrderDetailModel.filter(x =>
      x.OrderNumber != preOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listCustomerOrderDetailModel.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: CustomerOrderDetail) {
    let currentOrderNumber = data.OrderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listCustomerOrderDetailModel.find(x => x.OrderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listCustomerOrderDetailModel = this.listCustomerOrderDetailModel.filter(x =>
      x.OrderNumber != nextOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listCustomerOrderDetailModel.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }
}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'duplicateCode': true };
      }
      return null;
    }
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
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


