import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { CustomerOrder } from '../../models/customer-order.model';
import { CustomerOrderDetail } from '../../models/customer-order-detail.model';
import { OrderProductDetailProductAttributeValue } from '../../models/order-product-detail-product-attribute-value.model';
import { ContactModel } from '../../../shared/models/contact.model';
import { CustomerModel } from '../../../customer/models/customer.model';
import { CustomerService } from '../../../customer/services/customer.service';
import { CustomerOrderService } from '../../services/customer-order.service';
import { ContactService } from '../../../shared/services/contact.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import * as $ from 'jquery';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { OrderDetailDialogComponent } from '../order-detail-dialog/order-detail-dialog.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { OrderCostDetail } from '../../models/customer-order-cost-detail.model';
import { QuoteService } from '../../../customer/services/quote.service';
import { MenuItem } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { Workbook } from 'exceljs';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import { saveAs } from "file-saver";
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';

interface ResultDialog {
  status: boolean,
  customerOrderDetailModel: CustomerOrderDetail,
}

class Folder {
  folderId: string;
  parentId: string;
  name: string;
  url: string;
  isDelete: boolean;
  active: boolean;
  hasChild: boolean;
  listFile: Array<FileInFolder>;
  folderType: string;
  numberFile: number;
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
}
class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: OrderCostDetail,
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface OrderStatus {
  orderStatusId: string;
  orderStatusCode: string;
  orderStatusName: string;
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

interface NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}

interface Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css']
})
export class OrderDetailComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actionDownload: boolean = true;
  actionImport: boolean = true;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  loading: boolean = false;
  awaitResult: boolean = false;
  descriptionReject: string = '';
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

  orderId: string = null;
  workFollowOrder: MenuItem[];
  activeIndex: number = 0;

  /* Form */
  createOrderForm: FormGroup;
  cusNameControl: FormControl;
  orderContractControl: FormControl;
  quoteControl: FormControl;
  wareHouseControl: FormControl;
  customerMSTControl: FormControl;
  customerPhoneControl: FormControl;
  customerAddressControl: FormControl;
  orderDateControl: FormControl;
  orderStatusControl: FormControl;
  sellerControl: FormControl;
  descriptionControl: FormControl;
  noteControl: FormControl;
  customerControl: FormControl;
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
  unitMoneyControl: FormControl;
  exchangeRateControl: FormControl;
  /* End */

  /* Valid Form */
  isInvalidForm: boolean = false;
  display: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  /* End */

  colLeft: number = 8;
  isShow: boolean = true;
  licensed: boolean = true; //true: Được sửa, false: Không được sửa
  isManager: boolean = false; //true: Được sửa, false: Không được sửa
  isStatusNew: boolean = false; //true: Được sửa, false: Không được sửa
  isStatusIP: boolean = false; //true: Được sửa, false: Không được sửa
  isStatusDLV: boolean = false; //true: Được sửa, false: Không được sửa
  isStatusCAN: boolean = false; //true: Được sửa, false: Không được sửa
  isStatusRTN: boolean = false; //true: Được sửa, false: Không được sửa
  isDraft: boolean = false; //true: Là đơn hàng nháp, false: Không phải đơn hàng nháp
  isUserSendAproval: boolean = false; // có phải người gửi phê duyệt không
  listQuote: Array<any> = [];
  listQuoteObj: Array<any> = [];
  listWare: Array<any> = [];
  listProduct: Array<any> = [];
  listInventoryDeliveryVoucher: Array<any> = [];
  listBill: Array<any> = [];
  listOrderContract: Array<any> = []; // List hợp đồng
  listContract: Array<any> = []; // List hợp đồng
  listOrderCostDetailModel: Array<OrderCostDetail> = [];
  listAllOrderStatus: Array<OrderStatus> = [];
  listOrderStatus: Array<OrderStatus> = [];
  listEmployee: Array<Employee> = [];
  listUnitMoney: Array<Category> = [];
  optionCustomer: string = '2';
  warehouseName: string = '';
  listCustomer: Array<Customer> = [];
  customerEmail: string = '';
  customerPhone: string = '';
  fullAddress: string = '';
  listPaymentMethod: Array<Category> = [];
  listCustomerBankAccount: Array<BankAccount> = [];
  isShowBankAccount: boolean = false;
  listBankAccount: Array<BankAccount> = [];
  customerContactCode: string = 'CUS';
  listCustomerGroup: Array<Category> = [];
  listCustomerCode: Array<string> = [];
  listPersonInCharge: Array<Employee> = [];
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

  cols: any[];
  selectedColumns: any[];
  colsDelivery: any[];
  selectedColumnsDelivery: any[];
  colsBill: any[];
  selectedColumnsBill: any[];
  colsCost: any[];
  selectedColumnsCost: any[];
  selectedItem: any;
  listCustomerOrderDetailModel: Array<CustomerOrderDetail> = [];
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];

  autoGenValue: boolean = false;

  quoteId: string = null;
  isShowOptionCustomer: boolean = true;

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
    UpdatedById: null,
    UpdatedDate: null,
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
  customerOrderDetailModel = new CustomerOrderDetail();

  CustomerOrderAmountAfterDiscount: number = 0;
  CustomerOrderTotalDiscount: number = 0;
  TotalSumAmountProduct: number = 0;
  TotalSumAmountCost: number = 0;
  TotalSumVatProduct: number = 0;
  TotalPriceInitial: number = 0;
  AmountPriceProfit: number = 0;
  ValuePriceProfit: number = 0;

  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];

  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  colsFile: any;
  dialogConfirm: MatDialogRef<PopupComponent>;
  statusOld: any;
  messageError: string = '';
  messageConfirm: string = '';
  messageTitle: string = '';
  listAction: MenuItem[];
  cusObj: any = null;

  constructor(private translate: TranslateService,
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private customerOrderService: CustomerOrderService,
    private contactService: ContactService,
    public cdRef: ChangeDetectorRef,
    private dialogService: DialogService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
    private noteService: NoteService,
    private imageService: ImageUploadService,
    private quoteService: QuoteService,
    private folderService: ForderConfigurationService
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
    /*Kiểm tra nhân viên hoặc quản lý */
    let resource = "sal/order/order-detail";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }

      this.route.params.subscribe(params => { this.orderId = params['customerOrderID'] });

      this.setForm();
      this.setTable();

      this.loading = true;
      this.getMasterData();
    }
  }
  getMasterData() {
    this.listCustomerOrderDetailModel = [];
    this.listOrderCostDetailModel = [];
    this.awaitResult = true;
    this.customerOrderService.getMasterDataOrderDetail(this.orderId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listAllOrderStatus = result.listOrderStatus;
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
        this.isManager = result.isManager;
        this.listInventoryDeliveryVoucher = result.listInventoryDeliveryVoucher;
        this.listBill = result.listBillSale;
        this.listOrderContract = result.listContract;
        // this.listContract = result.listContract;

        /* Map data Order */
        let customerOrderObject = result.customerOrderObject;

        this.typeAccount = result.customerOrderObject.typeAccount;

        this.customerOrderModel.OrderId = customerOrderObject.orderId;
        this.customerOrderModel.OrderCode = customerOrderObject.orderCode;
        this.customerOrderModel.OrderDate = new Date(customerOrderObject.orderDate);
        this.customerOrderModel.Seller = customerOrderObject.seller;
        this.customerOrderModel.Description = customerOrderObject.description;
        this.customerOrderModel.Note = customerOrderObject.note;
        this.customerOrderModel.CustomerId = customerOrderObject.customerId;
        this.customerOrderModel.CustomerContactId = customerOrderObject.customerContactId;
        this.customerOrderModel.PaymentMethod = customerOrderObject.paymentMethod;
        this.customerOrderModel.DiscountType = customerOrderObject.discountType;
        this.customerOrderModel.BankAccountId = customerOrderObject.bankAccountId;
        this.customerOrderModel.DaysAreOwed = customerOrderObject.daysAreOwed;
        this.customerOrderModel.MaxDebt = customerOrderObject.maxDebt;
        this.customerOrderModel.ReceivedDate = customerOrderObject.receivedDate == null ? null : new Date(customerOrderObject.receivedDate);
        this.customerOrderModel.ReceivedHour = customerOrderObject.receivedHour;
        this.customerOrderModel.RecipientName = customerOrderObject.recipientName;
        this.customerOrderModel.LocationOfShipment = customerOrderObject.locationOfShipment;
        this.customerOrderModel.ShippingNote = customerOrderObject.shippingNote;
        this.customerOrderModel.RecipientPhone = customerOrderObject.recipientPhone;
        this.customerOrderModel.RecipientEmail = customerOrderObject.recipientEmail;
        this.customerOrderModel.PlaceOfDelivery = customerOrderObject.placeOfDelivery;
        this.customerOrderModel.Amount = customerOrderObject.amount;
        this.customerOrderModel.DiscountValue = customerOrderObject.discountValue;
        this.customerOrderModel.ReceiptInvoiceAmount = customerOrderObject.receiptInvoiceAmount;
        this.customerOrderModel.StatusId = customerOrderObject.statusId;
        this.customerOrderModel.CreatedById = customerOrderObject.createdById;
        this.customerOrderModel.CreatedDate = customerOrderObject.createdDate == null ? new Date() : new Date(customerOrderObject.createdDate);
        this.customerOrderModel.CustomerName = customerOrderObject.customerName;
        this.customerOrderModel.CustomerAddress = customerOrderObject.customerAddress;
        this.customerOrderModel.WarehouseId = customerOrderObject.warehouseId;
        this.customerOrderModel.QuoteId = customerOrderObject.quoteId;
        this.customerOrderModel.OrderContractId = customerOrderObject.orderContractId;
        this.customerOrderModel.UpdatedById = customerOrderObject.updatedById;
        this.customerOrderModel.UpdatedDate = customerOrderObject.updatedDate;
        this.statusOld = customerOrderObject.statusId;
        this.autoGenValue = customerOrderObject.isAutoGenReceiveInfor;
        /* End */

        if (this.auth.UserId == this.customerOrderModel.UpdatedById) {
          this.isUserSendAproval = true;
        }

        /* Map data list Order Detail */
        let listCustomerOrderDetail: Array<any> = result.listCustomerOrderDetail;
        let count_start = result.listCustomerOrderDetail.length;

        listCustomerOrderDetail.forEach((item, index) => {
          this.customerOrderDetailModel = new CustomerOrderDetail();

          this.customerOrderDetailModel.VendorId = item.vendorId;
          this.customerOrderDetailModel.ProductId = item.productId;
          this.customerOrderDetailModel.ProductName = item.productName;
          this.customerOrderDetailModel.Quantity = item.quantity;
          this.customerOrderDetailModel.UnitPrice = item.unitPrice;
          this.customerOrderDetailModel.CurrencyUnit = item.currencyUnit;
          this.customerOrderDetailModel.ExchangeRate = item.exchangeRate;
          this.customerOrderDetailModel.Vat = item.vat;
          this.customerOrderDetailModel.DiscountType = item.discountType;
          this.customerOrderDetailModel.DiscountValue = item.discountValue;
          this.customerOrderDetailModel.Description = item.description;
          this.customerOrderDetailModel.OrderDetailType = item.orderDetailType;
          this.customerOrderDetailModel.CreatedById = item.createdById;
          this.customerOrderDetailModel.CreatedDate = item.createdDate;
          this.customerOrderDetailModel.UpdatedById = item.updatedById;
          this.customerOrderDetailModel.UpdatedDate = item.updatedDate;
          this.customerOrderDetailModel.UnitId = item.unitId;
          this.customerOrderDetailModel.IncurredUnit = item.incurredUnit;
          this.customerOrderDetailModel.ExplainStr = item.nameGene;
          this.customerOrderDetailModel.NameVendor = item.nameVendor;
          this.customerOrderDetailModel.ProductNameUnit = item.nameProductUnit;
          this.customerOrderDetailModel.NameMoneyUnit = item.nameMoneyUnit;
          this.customerOrderDetailModel.GuaranteeTime = item.guaranteeTime;
          this.customerOrderDetailModel.GuaranteeDatetime = item.guaranteeDatetime == null ? null : new Date(item.guaranteeDatetime);
          this.customerOrderDetailModel.ExpirationDate = item.expirationDate == null ? null : new Date(item.expirationDate);
          this.customerOrderDetailModel.SumAmount = item.sumAmount;
          this.customerOrderDetailModel.WarehouseId = item.warehouseId;
          this.customerOrderDetailModel.PriceInitial = item.priceInitial;
          this.customerOrderDetailModel.IsPriceInitial = item.isPriceInitial;
          this.customerOrderDetailModel.WarrantyPeriod = item.warrantyPeriod;
          this.customerOrderDetailModel.ActualInventory = item.actualInventory;
          this.customerOrderDetailModel.BusinessInventory = item.businessInventory;
          this.customerOrderDetailModel.ProductCode = item.productId ? this.listProduct.find(p => p.productId == item.productId).productCode : "";
          this.customerOrderDetailModel.WareCode = item.warehouseId ? this.listWare.find(w => w.warehouseId == item.warehouseId)?.warehouseCode : "";
          this.customerOrderDetailModel.OrderNumber = item.orderNumber ? item.orderNumber : (index + 1);

          let arrayAttributeValue: Array<any> = item.orderProductDetailProductAttributeValue;
          if (arrayAttributeValue !== null) {
            arrayAttributeValue.forEach(option => {
              let orderProductDetailProductAttributeValue: OrderProductDetailProductAttributeValue = {
                OrderDetailId: this.emptyGuid,
                OrderProductDetailProductAttributeValueId: this.emptyGuid,
                ProductAttributeCategoryId: option.productAttributeCategoryId,
                ProductAttributeCategoryValueId: option.productAttributeCategoryValueId,
                ProductId: option.productId,
              };
              this.arrayOrderProductDetailProductAttributeValueModel.push(orderProductDetailProductAttributeValue);
            });
          }

          this.customerOrderDetailModel.OrderProductDetailProductAttributeValue = this.arrayOrderProductDetailProductAttributeValueModel;
          this.listCustomerOrderDetailModel.push(this.customerOrderDetailModel);
          this.arrayOrderProductDetailProductAttributeValueModel = [];
        });
        /* End */

        let listCostOrder: Array<any> = result.listCustomerOrderCostDetail;
        listCostOrder.forEach(item => {
          let objCost = new OrderCostDetail();
          objCost.orderCostDetailId = item.orderCostDetailId;
          objCost.costId = item.costId;
          objCost.orderId = item.orderId;
          objCost.quantity = item.quantity;
          objCost.unitPrice = item.unitPrice;
          objCost.costName = item.costName;
          objCost.costCode = item.costCode;
          objCost.active = item.active;
          objCost.createdById = item.createdById;
          objCost.createdDate = item.createdDate;
          objCost.updatedById = item.updatedById;
          objCost.updatedDate = item.updatedDate;
          objCost.sumAmount = item.quantity * item.unitPrice;

          this.listOrderCostDetailModel.push(objCost);
        });
        /*Reshow Time Line */
        this.noteHistory = result.listNote;
        this.handleNoteContent();

        this.checkIsDraft();

        this.checkLicensed();

        this.filterOrderStatus(this.customerOrderModel.StatusId);

        this.mapDataToForm();

        if (count_start == this.listCustomerOrderDetailModel.length) {
          setTimeout(() => {
            this.awaitResult = false;
          }, 5000);
        }
      } else {
        this.loading = false;
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
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

  setForm() {
    this.listAction = [
      {
        label: 'Xuất Excel', icon: 'pi pi-file', command: () => {
          this.exportExcel();
        }
      },
      {
        label: 'Xuất PDF', icon: 'pi pi-file', command: () => {
          this.exportPFD();
        }
      },
    ];

    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    this.orderDateControl = new FormControl(new Date(), [Validators.required]);
    this.orderStatusControl = new FormControl(null, [Validators.required]);
    this.orderContractControl = new FormControl(null);
    this.sellerControl = new FormControl(null, [Validators.required]);
    this.descriptionControl = new FormControl(null);
    this.quoteControl = new FormControl(null);
    this.wareHouseControl = new FormControl(null);
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
    this.customerGroupControl = new FormControl(null, [Validators.required]);
    this.customerCodeControl = new FormControl(null);
    this.customerNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.customerTypeControl = new FormControl(this.listCustomerType[0]);
    this.newCusPaymentMethodControl = new FormControl(null);
    this.newCusDaysAreOwedControl = new FormControl(null);
    this.newCusMaxDebtControl = new FormControl(null);
    this.receivedDateControl = new FormControl(new Date(), [Validators.required]);
    this.receivedHourControl = new FormControl(null);
    this.recipientNameControl = new FormControl(null); //, [Validators.required, forbiddenSpaceText]);
    this.locationOfShipmentControl = new FormControl(null);
    this.placeOfDeliveryControl = new FormControl(null);
    this.recipientPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern())]);
    this.recipientEmailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.shippingNoteControl = new FormControl(null);
    this.discountTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl = new FormControl('0');
    this.unitMoneyControl = new FormControl('');
    this.exchangeRateControl = new FormControl('1');

    this.createOrderForm = new FormGroup({
      orderDateControl: this.orderDateControl,
      orderStatusControl: this.orderStatusControl,
      orderContractControl: this.orderContractControl,
      quoteControl: this.quoteControl,
      wareHouseControl: this.wareHouseControl,
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
      receivedDateControl: this.receivedDateControl,
      receivedHourControl: this.receivedHourControl,
      recipientNameControl: this.recipientNameControl,
      locationOfShipmentControl: this.locationOfShipmentControl,
      placeOfDeliveryControl: this.placeOfDeliveryControl,
      recipientPhoneControl: this.recipientPhoneControl,
      recipientEmailControl: this.recipientEmailControl,
      shippingNoteControl: this.shippingNoteControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl,
      unitMoneyControl: this.unitMoneyControl,
      exchangeRateControl: this.exchangeRateControl
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

    this.colsFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left' },
    ];

    this.colsDelivery = [
      { field: 'inventoryDeliveryVoucherCode', header: 'Mã phiếu nhập', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'inventoryDeliveryVoucherType', header: 'Loại phiếu nhập', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày lập phiếu', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'inventoryDeliveryVoucherDate', header: 'Ngày xuất kho', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'nameCreate', header: 'Người lập phiếu', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'nameStatus', header: 'Trạng thái', width: '50px', textAlign: 'left', color: '#f44336' },
    ];
    this.selectedColumnsDelivery = this.colsDelivery;

    this.colsBill = [
      { field: 'billOfSaLeCode', header: 'Mã hóa đơn', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'billDate', header: 'Ngày hóa đơn', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'customerName', header: 'Tên khách hàng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'amount', header: 'Số tiền trên hóa đơn', width: '50px', textAlign: 'right', color: '#f44336' },
    ];
    this.selectedColumnsBill = this.colsBill;
  }

  /*Kiểm tra đơn hàng có phải đơn hàng Nháp không*/
  checkIsDraft() {
    let status = this.listAllOrderStatus.find(x => x.orderStatusId == this.customerOrderModel.StatusId);
    if (status) {
      let listValidStatus = ['DRA']; //Nháp
      if (listValidStatus.includes(status.orderStatusCode)) {
        this.isDraft = true;
      } else {
        this.isDraft = false;
      }
    } else {
      this.isDraft = false;
    }
  }

  /*Kiểm tra đơn hàng có được phép sửa hay không*/
  checkLicensed() {
    let status = this.listAllOrderStatus.find(x => x.orderStatusId == this.customerOrderModel.StatusId);
    if (status) {
      let listValidStatus = ['COMP', 'CAN', 'RTN']; //Đã đóng, Hủy, Bị trả lại
      if (listValidStatus.includes(status.orderStatusCode)) {
        this.licensed = false;
      } else {
        this.licensed = true;
      }
    } else {
      this.licensed = false;
    }
  }

  /*Lọc các trạng thái đơn hàng theo luật*/
  filterOrderStatus(orderStatusId: string) {
    let status = this.listAllOrderStatus.find(x => x.orderStatusId == orderStatusId);

    switch (status.orderStatusCode) {
      case "DRA":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "DRA" || x.orderStatusCode == "IP");
        break;
      case "IP":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "IP" || x.orderStatusCode == "PD"
          || x.orderStatusCode == "DLV" || x.orderStatusCode == "COMP"
          || x.orderStatusCode == "ON" || x.orderStatusCode == "RTN"
          || x.orderStatusCode == "CAN");
        break;
      case "ON":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "ON" || x.orderStatusCode == "IP"
          || x.orderStatusCode == "PD" || x.orderStatusCode == "DLV"
          || x.orderStatusCode == "COMP" || x.orderStatusCode == "RTN"
          || x.orderStatusCode == "CAN");
        break;
      case "DLV":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "DLV" || x.orderStatusCode == "RTN"
          || x.orderStatusCode == "PD" || x.orderStatusCode == "CAN"
          || x.orderStatusCode == "COMP");
        break;
      case "PD":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "PD" || x.orderStatusCode == "DLV"
          || x.orderStatusCode == "COMP" || x.orderStatusCode == "CAN");
        break;
      case "COMP":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "COMP");
        break;
      case "CAN":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "CAN");
        break;
      case "RTN":
        this.listOrderStatus = this.listAllOrderStatus.filter(x => x.orderStatusCode == "RTN");
        break;
    }
  }

  mapDataToForm() {
    //Ngày đặt hàng
    this.orderDateControl.setValue(this.customerOrderModel.OrderDate);

    // Báo giá
    if (this.customerOrderModel.QuoteId) {
      let quoteObj = this.listQuote.find(q => q.quoteId === this.customerOrderModel.QuoteId);
      this.quoteControl.setValue(quoteObj);
    }
    else {
      this.quoteControl.setValue(null);
    }

    // Kho
    if (this.customerOrderModel.WarehouseId) {
      let wareObj = this.listWare.find(q => q.warehouseId === this.customerOrderModel.WarehouseId);
      this.wareHouseControl.setValue(wareObj);
      this.warehouseName = wareObj?.warehouseName;
    }
    else {
      this.wareHouseControl.setValue(null);
    }

    // Hợp đồng
    if (this.customerOrderModel.OrderContractId) {
      let contractObj = this.listOrderContract.find(q => q.contractId === this.customerOrderModel.OrderContractId);
      this.orderContractControl.setValue(contractObj);
    }
    else {
      this.orderContractControl.setValue(null);
    }

    //Nhân viên bán hàng
    let seller: Employee = this.listEmployee.find(x => x.employeeId == this.customerOrderModel.Seller);
    if (seller) {
      this.sellerControl.setValue(seller);
    }

    //Mô tả
    this.descriptionControl.setValue(this.customerOrderModel.Description);

    //Ghi chú
    this.noteControl.setValue(this.customerOrderModel.Note);

    if (this.typeAccount == 1) {
      //Nếu là khách lẻ
      this.optionCustomer = '1';
    } else if (this.typeAccount == 2) {
      //Nếu là khách hàng
      this.optionCustomer = '2';

      let customer: Customer = this.listCustomer.find(x => x.customerId == this.customerOrderModel.CustomerId);
      if (customer) {
        this.customerControl.setValue(customer);
        this.customerMSTControl.setValue(customer.taxCode);
        this.changeCustomer(customer);
      }

      this.customerEmail = customer.customerEmail;
      this.customerPhone = customer.customerPhone;

      //Lấy địa chỉ của khách hàng
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

      //Phương thức thanh toán
      let paymentMethod: Category = this.listPaymentMethod.find(x => x.categoryId == this.customerOrderModel.PaymentMethod);
      if (paymentMethod) {
        this.paymentMethodControl.setValue(paymentMethod);

        if (paymentMethod.categoryCode == "BANK") {
          this.isShowBankAccount = true;
          let customerId = customer.customerId;

          this.listBankAccount = this.listCustomerBankAccount.filter(x => x.objectId == customerId);
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount.find(x => x.bankAccountId == this.customerOrderModel.BankAccountId);
            if (toSelectBankAccount) this.bankAccountControl.setValue(toSelectBankAccount);
          } else {
            this.listBankAccount = [];
          }
        } else {
          this.isShowBankAccount = false;
          this.listBankAccount = [];
          this.bankAccountControl.setValue(null);
        }
      }

      //Số ngày được nợ
      let daysAreOwed = this.customerOrderModel.DaysAreOwed;
      this.daysAreOwedControl.setValue(daysAreOwed);

      //Số nợ tối đa
      let maxDebt = this.customerOrderModel.MaxDebt;
      this.maxDebtControl.setValue(maxDebt);
    }

    //Ngày nhận
    let receivedDate = this.customerOrderModel.ReceivedDate;
    if (receivedDate) this.receivedDateControl.setValue(receivedDate);

    //Giờ nhận
    let receivedHour: any = this.customerOrderModel.ReceivedHour;
    if (receivedHour) {
      receivedHour = this.convertTimeSpanToDate(receivedHour);
      this.receivedHourControl.setValue(receivedHour);
    }

    //Tên người nhận
    this.recipientNameControl.setValue(this.customerOrderModel.RecipientName);

    //Địa điểm xuất hàng
    this.locationOfShipmentControl.setValue(this.customerOrderModel.LocationOfShipment);

    //Địa điểm giao hàng
    this.placeOfDeliveryControl.setValue(this.customerOrderModel.PlaceOfDelivery);

    //Số điện thoại người nhận
    this.recipientPhoneControl.setValue(this.customerOrderModel.RecipientPhone);

    //Email người nhận
    this.recipientEmailControl.setValue(this.customerOrderModel.RecipientEmail);

    //Ghi chú
    this.shippingNoteControl.setValue(this.customerOrderModel.ShippingNote);

    //Loại chiết khấu
    let toSelectDiscountType: DiscountType = this.discountTypeList.find(x => x.value == this.customerOrderModel.DiscountType);
    this.discountTypeControl.setValue(toSelectDiscountType);

    //Giá trị chiết khấu
    let discountValue = this.customerOrderModel.DiscountValue == null ? 0 : this.customerOrderModel.DiscountValue;
    this.discountValueControl.setValue(discountValue.toString());

    //Tình trạng
    let status: OrderStatus = this.listOrderStatus.find(x => x.orderStatusId == this.customerOrderModel.StatusId);
    this.isStatusNew = status.orderStatusCode == 'DRA' ? true : false;
    this.isStatusIP = status.orderStatusCode == 'IP' ? true : false;
    this.isStatusDLV = status.orderStatusCode == 'DLV' ? true : false;
    this.isStatusCAN = status.orderStatusCode == 'CAN' ? true : false;
    this.isStatusRTN = status.orderStatusCode == 'RTN' ? true : false;

    if (status) {
      this.orderStatusControl.setValue(status);
    }

    //Tiến trình của Báo giá
    if (status.orderStatusCode == 'DRA' || status.orderStatusCode == 'IP' || status.orderStatusCode == 'DLV') {
      this.workFollowOrder = [
        {
          label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'DRA').description
        },
        {
          label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'IP').description
        },
        {
          label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'DLV').description
        },
        {
          label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'COMP').description
        },
        {
          label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'CAN').description
        }
      ];

      switch (status.orderStatusCode) {
        case 'DRA': {
          this.activeIndex = 0;
          break;
        }
        case 'IP': {
          this.activeIndex = 1;
          break;
        }
        case 'DLV': {
          this.activeIndex = 2;
          break;
        }
        default: {
          break;
        }
      }
    } else {
      if (status.orderStatusCode == 'COMP') {
        this.workFollowOrder = [
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'DRA').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'IP').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'DLV').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'COMP').description
          }
        ];

        this.activeIndex = 3;
      }
      else if (status.orderStatusCode == 'RTN') {
        this.workFollowOrder = [
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'DRA').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'IP').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'RTN').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'CAN').description
          },
          // {
          //   label: this.listAllOrderStatus.find(x => x.orderStatusCode == status.orderStatusCode).description
          // }
        ];

        this.activeIndex = 2;
      }
      else {
        this.workFollowOrder = [
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'DRA').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'IP').description
          },
          {
            label: this.listAllOrderStatus.find(x => x.orderStatusCode == 'CAN').description
          },
          // {
          //   label: this.listAllOrderStatus.find(x => x.orderStatusCode == status.orderStatusCode).description
          // }
        ];
        this.activeIndex = 2;
      }
    }

    // Tên khách hàng
    this.cusNameControl.setValue(this.customerOrderModel.CustomerName);

    // Địa chỉ khách hàng
    this.customerAddressControl.setValue(this.customerOrderModel.CustomerAddress);

    // Điện thoại
    this.customerPhoneControl.setValue(this.customerOrderModel.RecipientPhone);

    this.calculatorAll();
  }

  /*Event chọn radio button: Khách lẻ, Chọn khách hàng, Khách hàng mới*/
  chooseOptionCustomer() {
    this.autoGen(); //Lấy thông tin giao hàng
  }

  changeWare(event) {
    if (event) {
      this.warehouseName = event.warehouseName;
    }
    else {
      this.warehouseName = '';
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
        obj.ProductCode = element.productId ? this.listProduct.find(p => p.productId == element.productId).productCode : "";
        // obj.BusinessInventory = element.businessInventory;
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
      this.cusObj = customer;
      this.customerEmail = customer.customerEmail;
      this.customerPhone = customer.customerPhone;

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

            let emp = this.listEmployee.find(e => e.employeeId == this.customerOrderModel.Seller);
            this.sellerControl.setValue(emp);
          }
        });
      }
      else {
        this.listEmployee = [];
        let emp = this.listPersonInCharge.find(e => e.employeeId == this.customerOrderModel.Seller);
        this.listEmployee.push(emp)
        this.sellerControl.setValue(emp);
      }
      //Lấy địa chỉ của khách hàng
      this.contactService.getAddressByObject(customer.customerId, "CUS").subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.fullAddress = result.address;
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
      let maxDebt = customer.maximumDebtValue ? customer.maximumDebtValue : 0;;
      this.maxDebtControl.setValue(maxDebt);

      this.autoGen(); //Lấy thông tin giao hàng
    }
    else {
      this.cusObj = null;
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

  changeContract(value: any) {
    this.quoteControl.reset();
    this.listQuoteObj = [];
    this.listCustomerOrderDetailModel = [];
    this.listOrderCostDetailModel = [];
    if (value) {
      this.listQuoteObj = this.listQuote.filter(c => c.quoteId == value.quoteId);
    }
  }


  /*Lấy thông tin giao hàng theo Thông tin khách hàng*/
  autoGen() {
    if (this.autoGenValue) {
      switch (this.optionCustomer) {
        case "1":
          this.recipientNameControl.setValue('Khách lẻ');
          this.recipientPhoneControl.setValue(null);
          this.recipientEmailControl.setValue(null);
          break;
        case "2":
          let selectCustomer: Customer = this.customerControl.value;
          if (selectCustomer) {
            this.recipientNameControl.setValue(selectCustomer.customerName);
            this.recipientPhoneControl.setValue(selectCustomer.customerPhone);
            this.recipientEmailControl.setValue(selectCustomer.customerEmail);
          } else {
            this.recipientNameControl.setValue(null);
            this.recipientPhoneControl.setValue(null);
            this.recipientEmailControl.setValue(null);
          }
          break;
        case "3":
          let newCustomerName = this.customerNameControl.value;
          this.recipientNameControl.setValue(newCustomerName);
          this.recipientPhoneControl.setValue(null);
          this.recipientEmailControl.setValue(null);
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
        unitMoney: this.unitMoneyControl.value,
        exchangeRate: this.exchangeRateControl.value,
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
          this.customerOrderDetailModel = result.customerOrderDetailModel;

          //set orderNumber cho sản phẩm/dịch vụ mới thêm
          this.customerOrderDetailModel.OrderNumber = this.listCustomerOrderDetailModel.length + 1;

          this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, this.customerOrderDetailModel];

          this.calculatorAll();
        }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    if (this.actionEdit) {
      let readOnly = false;
      if (!this.isStatusNew) {
        readOnly = true;
      }

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
          unitMoney: this.unitMoneyControl.value,
          exchangeRate: this.exchangeRateControl.value,
          cusGroupId: cusGroupId,
          dateOrder: date,
          // warehouse: wareHouse,
          customerOrderDetailModel: OldArray,
          readOnly: readOnly
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
            this.customerOrderDetailModel = result.customerOrderDetailModel;
            this.listCustomerOrderDetailModel = [...this.listCustomerOrderDetailModel, this.customerOrderDetailModel];

            //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
            this.listCustomerOrderDetailModel.sort((a, b) =>
              (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));

            this.calculatorAll();
          }
        }
      });
    }
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    //this.translate.get('order.messages_confirm.confirm_check').subscribe(value => { this.messageConfirm = value; });
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

  /*Thêm một chi phí*/
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
    if (this.actionEdit && this.isStatusNew) {
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
  }

  /*Xóa một chi phí*/
  deleteCostItem(dataRow) {
    //this.translate.get('order.messages_confirm.confirm_check').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listOrderCostDetailModel.indexOf(dataRow);
        this.listOrderCostDetailModel.splice(index, 1);

        this.calculatorAll();
      }
    });
  }

  gotoCreateBill() {
    this.router.navigate(['/bill-sale/create', { orderId: this.orderId }])
  }

  restartCustomerOrderDetailModel() {
    var item = new CustomerOrderDetail();
    this.customerOrderDetailModel = item;
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

  deleteOrder() {
    this.translate.get('order.messages_confirm.delete_confirm_order').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        this.customerOrderService.deleteOrder(this.orderId).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.router.navigate(['order/list']);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
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

  updateOrder() {
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
        this.typeAccount = 1;

        this.customerOrderModel.OrderDate = convertToUTCTime(this.orderDateControl.value);
        let seller: Employee = this.sellerControl.value;
        if (seller) this.customerOrderModel.Seller = seller.employeeId;
        this.customerOrderModel.Description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
        this.customerOrderModel.Note = this.noteControl.value == null ? null : this.noteControl.value.trim();
        this.customerOrderModel.CustomerId = this.emptyGuid;
        let discountType: DiscountType = this.discountTypeControl.value;
        this.customerOrderModel.DiscountType = discountType.value;
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

        this.customerOrderModel.RecipientName = this.recipientNameControl.value.trim();
        this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
        this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
        this.customerOrderModel.RecipientPhone = this.recipientPhoneControl.value;
        this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
        this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();
        this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
        let status: OrderStatus = this.orderStatusControl.value;
        this.customerOrderModel.StatusId = status.orderStatusId;

        this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;

        this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.filterOrderStatus(this.customerOrderModel.StatusId);
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
          this.getMasterData();
        });
      } else if (this.optionCustomer == '2') {
        this.mappingData();
        let statusCode = this.listAllOrderStatus.find(s => s.orderStatusId == this.statusOld).orderStatusCode;

        this.translate.get('order.messages_confirm.confirm_check').subscribe(value => { this.messageConfirm = value; });
        this.customerOrderService.checkBeforCreateOrUpdateOrder(this.customerOrderModel.CustomerId, this.customerOrderModel.MaxDebt, this.CustomerOrderAmountAfterDiscount).subscribe(responseCheck => {
          let resultCheck: any = responseCheck;
          if (resultCheck.statusCode == 200) {
            if (resultCheck.isCheckMaxDebt == true) {
              this.confirmationService.confirm({
                message: this.messageConfirm,
                accept: () => {
                  if (statusCode == 'DLV' || statusCode == 'IP') {
                    this.customerOrderService.checkReceiptOrderHistory(this.orderId, this.customerOrderModel.Amount).subscribe(responseCheck => {
                      let resultCheck: any = responseCheck;
                      if (resultCheck.statusCode == 200) {
                        if (!resultCheck.checkReceiptOrderHistory) {
                          this.translate.get('order.messages_title.title_warning').subscribe(value => { this.messageTitle = value; });
                          this.translate.get('order.messages_confirm.confirm_pay_change_status').subscribe(value => { this.messageConfirm = value; });
                          let _title = this.messageTitle;
                          let _content = this.messageConfirm;
                          this.dialogConfirm = this.dialog.open(PopupComponent,
                            {
                              width: '500px',
                              height: '300px',
                              autoFocus: false,
                              data: { title: _title, content: _content }
                            });

                          this.dialogConfirm.afterClosed().subscribe(resultPopup => {
                            if (resultPopup) {
                              this.customerOrderModel.StatusId = this.listAllOrderStatus.find(s => s.orderStatusCode == 'PD').orderStatusId;
                              this.awaitResult = true;
                              this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                                this.loading = true;
                                let result: any = response;

                                this.translate.get('order.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
                                if (result.statusCode == 200) {
                                  this.filterOrderStatus(this.customerOrderModel.StatusId);
                                  let msg = { severity: 'success', summary: this.messageTitle, detail: result.messageCode };
                                  this.showMessage(msg);
                                } else {
                                  let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
                                  this.showMessage(msg);
                                }
                                this.getMasterData();
                              });
                            }
                          });
                        }
                        else {
                          this.awaitResult = true;
                          this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                            let result: any = response;

                            this.translate.get('order.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
                            if (result.statusCode == 200) {
                              this.filterOrderStatus(this.customerOrderModel.StatusId);
                              let msg = { severity: 'success', summary: this.messageTitle, detail: result.messageCode };
                              this.showMessage(msg);
                            } else {
                              let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
                              this.showMessage(msg);
                            }
                            this.getMasterData();
                          });
                        }
                      }
                    });
                  } else {
                    this.awaitResult = true;
                    this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                      let result: any = response;

                      this.translate.get('order.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
                      if (result.statusCode == 200) {
                        this.filterOrderStatus(this.customerOrderModel.StatusId);
                        let msg = { severity: 'success', summary: this.messageTitle, detail: result.messageCode };
                        this.showMessage(msg);
                      } else {
                        let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
                        this.showMessage(msg);
                      }
                      this.getMasterData();
                    });
                  }
                }
              });
            }
            else {
              if (statusCode == 'DLV' || statusCode == 'IP') {
                this.customerOrderService.checkReceiptOrderHistory(this.orderId, this.customerOrderModel.Amount).subscribe(responseCheck => {
                  let resultCheck: any = responseCheck;
                  if (resultCheck.statusCode == 200) {
                    if (!resultCheck.checkReceiptOrderHistory) {
                      this.translate.get('order.messages_title.title_warning').subscribe(value => { this.messageTitle = value; });
                      this.translate.get('order.messages_confirm.confirm_pay_change_status').subscribe(value => { this.messageConfirm = value; });
                      let _title = this.messageTitle;
                      let _content = this.messageConfirm;
                      this.dialogConfirm = this.dialog.open(PopupComponent,
                        {
                          width: '500px',
                          height: '300px',
                          autoFocus: false,
                          data: { title: _title, content: _content }
                        });

                      this.dialogConfirm.afterClosed().subscribe(resultPopup => {
                        if (resultPopup) {
                          this.customerOrderModel.StatusId = this.listAllOrderStatus.find(s => s.orderStatusCode == 'PD').orderStatusId;
                          this.awaitResult = true;
                          this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                            let result: any = response;

                            this.translate.get('order.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
                            if (result.statusCode == 200) {
                              this.filterOrderStatus(this.customerOrderModel.StatusId);
                              let msg = { severity: 'success', summary: this.messageTitle, detail: result.messageCode };
                              this.showMessage(msg);
                            } else {
                              let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
                              this.showMessage(msg);
                            }
                            this.getMasterData();
                          });
                        }
                      });
                    }
                    else {
                      this.awaitResult = true;
                      this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                        let result: any = response;

                        this.translate.get('order.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
                        if (result.statusCode == 200) {
                          this.filterOrderStatus(this.customerOrderModel.StatusId);
                          let msg = { severity: 'success', summary: this.messageTitle, detail: result.messageCode };
                          this.showMessage(msg);
                        } else {
                          let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
                          this.showMessage(msg);
                        }
                        this.getMasterData();
                      });
                    }
                  }
                });
              } else {
                this.awaitResult = true;
                this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                  let result: any = response;

                  if (result.statusCode == 200) {
                    this.filterOrderStatus(this.customerOrderModel.StatusId);
                    let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
                    this.showMessage(msg);
                  } else {
                    let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                    this.showMessage(msg);
                  }
                  this.getMasterData();
                });
              }
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
        this.typeAccount = 3;

        let customerType: CustomerType = this.customerTypeControl.value;
        this.customerModel.CustomerType = customerType.typeValue;
        this.customerModel.CustomerCode = this.customerCodeControl.value.trim();
        let customerGroup: Category = this.customerGroupControl.value;
        this.customerModel.CustomerGroupId = customerGroup.categoryId;
        let paymentMethod: Category = this.newCusPaymentMethodControl.value;
        if (paymentMethod) {
          this.customerOrderModel.PaymentMethod = paymentMethod.categoryId;
        } else {
          this.customerOrderModel.PaymentMethod = null;
        }

        this.customerModel.CustomerName = this.customerNameControl.value.trim();

        let newCusDaysAreOwed = this.newCusDaysAreOwedControl.value;
        if (newCusDaysAreOwed) {
          this.customerModel.MaximumDebtDays = ParseStringToFloat(newCusDaysAreOwed);
        } else {
          this.customerModel.MaximumDebtDays = null;
        }

        let newCusMaxDebt = this.newCusMaxDebtControl.value;
        if (newCusMaxDebt) {
          this.customerModel.MaximumDebtValue = ParseStringToFloat(newCusMaxDebt);
        } else {
          this.customerModel.MaximumDebtValue = null;
        }

        this.customerService.quickCreateCustomer(
          this.auth.UserId,
          this.customerModel.CustomerType,
          this.customerModel.CustomerCode,
          this.customerModel.CustomerGroupId,
          this.customerOrderModel.PaymentMethod,
          this.customerModel.CustomerName,
          this.customerModel.MaximumDebtValue,
          this.customerModel.MaximumDebtDays,
          new Date()).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.customerOrderModel.OrderDate = convertToUTCTime(this.orderDateControl.value);
              let seller: Employee = this.sellerControl.value;
              if (seller) this.customerOrderModel.Seller = seller.employeeId;
              this.customerOrderModel.Description = this.descriptionControl.value == null ? null : this.descriptionControl.value.trim();
              this.customerOrderModel.Note = this.noteControl.value == null ? null : this.noteControl.value.trim();
              this.customerOrderModel.CustomerId = result.customerID;

              let paymentMethod: Category = this.newCusPaymentMethodControl.value;
              if (paymentMethod) this.customerOrderModel.PaymentMethod = paymentMethod.categoryId;

              let discountType: DiscountType = this.discountTypeControl.value;
              this.customerOrderModel.DiscountType = discountType.value;

              let daysAreOwed = this.newCusDaysAreOwedControl.value;
              if (daysAreOwed) {
                this.customerOrderModel.DaysAreOwed = ParseStringToFloat(daysAreOwed);
              }

              let maxDebt = this.newCusMaxDebtControl.value;
              if (maxDebt) {
                this.customerOrderModel.MaxDebt = ParseStringToFloat(maxDebt);
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
              this.customerOrderModel.RecipientName = this.recipientNameControl.value.trim();
              this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
              this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
              this.customerOrderModel.RecipientPhone = this.recipientPhoneControl.value;
              this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
              this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();
              this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
              let status: OrderStatus = this.orderStatusControl.value;
              this.customerOrderModel.StatusId = status.orderStatusId;

              this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;

              this.customerOrderService.UpdateCustomerOrder(this.customerOrderModel, this.listCustomerOrderDetailModel, this.listOrderCostDetailModel, this.typeAccount).subscribe(response => {
                let result: any = response;

                if (result.statusCode == 200) {
                  this.filterOrderStatus(this.customerOrderModel.StatusId);
                  let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Lưu thành công' };
                  this.showMessage(msg);
                } else {
                  let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                  this.showMessage(msg);
                }
                this.getMasterData();
              });
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
      }
    }
  }

  mappingData() {
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
    }
    let maxDebt = this.maxDebtControl.value;
    if (maxDebt) {
      this.customerOrderModel.MaxDebt = ParseStringToFloat(maxDebt);
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
    this.customerOrderModel.RecipientName = this.recipientNameControl.value.trim();
    this.customerOrderModel.LocationOfShipment = this.locationOfShipmentControl.value == null ? null : this.locationOfShipmentControl.value.trim();
    this.customerOrderModel.ShippingNote = this.shippingNoteControl.value == null ? null : this.shippingNoteControl.value.trim();
    this.customerOrderModel.RecipientPhone = this.customerPhoneControl.value;
    this.customerOrderModel.RecipientEmail = this.recipientEmailControl.value == null ? null : this.recipientEmailControl.value.trim();
    this.customerOrderModel.PlaceOfDelivery = this.placeOfDeliveryControl.value == null ? null : this.placeOfDeliveryControl.value.trim();

    //Amount = Tổng tiền hàng trước thuế + Tổng tiền thuế VAT
    this.customerOrderModel.Amount = this.TotalSumAmountProduct + this.TotalSumVatProduct;

    this.customerOrderModel.DiscountValue = ParseStringToFloat(this.discountValueControl.value);
    this.customerOrderModel.WarehouseId = this.wareHouseControl.value ? this.wareHouseControl.value.warehouseId : null;
    this.customerOrderModel.CustomerName = this.cusNameControl.value.trim();
    this.customerOrderModel.CustomerAddress = this.customerAddressControl.value;
    let status: OrderStatus = this.orderStatusControl.value;
    this.customerOrderModel.StatusId = status.orderStatusId;

    this.customerOrderModel.IsAutoGenReceiveInfor = this.autoGenValue;
  }

  updateStatusOrder(type: string) {
    if (type === "SEND_APPROVAL") {
      this.translate.get('order.messages_confirm.confirm_send_approval').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "APPROVAL") {
      this.translate.get('order.messages_confirm.confirm_approval').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "REJECT") {
      this.translate.get('order.messages_confirm.confirm_reject').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "EDIT_NEW") {
      this.translate.get('order.messages_confirm.confirm_edit_new').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "CANCEL_APPROVAL") {
      this.translate.get('order.messages_confirm.confirm_cancel_approval').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "CANCEL") {
      this.translate.get('order.messages_confirm.confirm_cancel').subscribe(value => { this.messageConfirm = value; });
    }
    this.mappingData();
    if (type !== "PAY_ORDER") {
      this.confirmationService.confirm({
        message: this.messageConfirm,
        accept: () => {
          this.checkUpdateStatus(type);
        }
      });
    }
    else {
      this.checkUpdateStatus(type);
    }
  }

  // cloneOrder(){
  //   this.loading = true;
  //   this.awaitResult = true;
  //   this.customerOrderService.updateStatusOrder(this.customerOrderModel.OrderId, "CLONE", this.auth.UserId, '').subscribe(response => {
  //     let result: any = response;
  //     if(result.statusCode == 200){
  //       let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
  //       this.showMessage(msg);
  //       this.router.navigate(['/procurement-request/view', { id: result.procurementRequestId }])
  //     }
  //     else{
  //       let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
  //       this.showMessage(msg);
  //     }
  //   });
  // }

  checkUpdateStatus(type) {
    this.translate.get('order.messages_confirm.confirm_check').subscribe(value => { this.messageConfirm = value; });
    this.customerOrderService.checkBeforCreateOrUpdateOrder(this.customerOrderModel.CustomerId, this.customerOrderModel.MaxDebt, this.CustomerOrderAmountAfterDiscount).subscribe(responseCheck => {
      let resultCheck: any = responseCheck;
      if (resultCheck.statusCode == 200) {
        if (resultCheck.isCheckMaxDebt == true) {
          this.confirmationService.confirm({
            key: 'checkMaxDebt',
            message: this.messageConfirm,
            accept: () => {
              this.updateOrderStatus(type);
            }
          })
        }
        else {
          if (type === "REJECT") {
            this.display = true;
          }
          else {
            this.updateOrderStatus(type);
          }
        }
      }
      else {
        this.translate.get('order.messages_error.send_approval_error').subscribe(value => { this.messageError = value; });
        let msg = { severity: 'error', summary: 'Thông báo:', detail: this.messageError };
        this.showMessage(msg);
        this.awaitResult = false;
        this.loading = false;
      }
    });
  }

  updateOrderStatus(type) {
    if (type === "REJECT") {
      this.display = true;
    }
    else {
      this.loading = true;
      this.awaitResult = true;
      this.customerOrderService.updateStatusOrder(this.customerOrderModel.OrderId, type, this.auth.UserId, '').subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
          if (type === 'PAY_ORDER') {
            this.router.navigate(['/procurement-request/view', { id: result.procurementRequestId }])
          }
          this.getMasterData();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  rejectOrder() {
    this.loading = true;
    this.awaitResult = true;
    this.customerOrderService.updateStatusOrder(this.customerOrderModel.OrderId, "REJECT", this.auth.UserId, this.descriptionReject).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);

        this.display = false;
        this.getMasterData();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Event Thêm các file được chọn vào list file*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 1000000) {
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

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.translate.get('order.messages_confirm.cancel_confirm_note').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    this.loading = true;
    this.listNoteDocumentModel = [];

    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      let listFileNameExists: Array<FileNameExists> = [];
      let result: any = await this.imageService.uploadFileForOptionAsync(this.uploadedFiles, 'Order');

      listFileNameExists = result.listFileNameExists;

      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        let fileExists = listFileNameExists.find(f => f.oldFileName == this.uploadedFiles[x].name);
        if (fileExists) {
          noteDocument.DocumentName = fileExists.newFileName;
        }
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }

    let noteModel = new NoteModel();

    if (!this.noteId) {
      /*Tạo mới ghi chú*/

      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.orderId;
      noteModel.ObjectType = 'ORDER';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/

      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.orderId;
      noteModel.ObjectType = 'ORDER';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();

      //Thêm file cũ đã lưu nếu có
      this.listUpdateNoteDocument.forEach(item => {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = item.documentName;
        noteDocument.DocumentSize = item.documentSize;
        noteDocument.UpdatedById = item.updatedById;
        noteDocument.UpdatedDate = item.updatedDate;

        this.listNoteDocumentModel.push(noteDocument);
      });
    }

    this.noteService.createNoteForOrderDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        /*Reshow Time Line*/
        this.noteHistory = result.listNote;

        // this.noteHistory.forEach(file => {
        let file = result.noteId;
        let listFileUploadModel: Array<FileUploadModel> = [];
        this.uploadedFiles.forEach(item => {
          let fileUpload: FileUploadModel = new FileUploadModel();
          fileUpload.FileInFolder = new FileInFolder();
          fileUpload.FileInFolder.active = true;
          let index = item.name.lastIndexOf(".");
          let name = item.name.substring(0, index);
          fileUpload.FileInFolder.fileName = name;
          fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
          fileUpload.FileInFolder.size = item.size;
          fileUpload.FileInFolder.objectId = file;
          fileUpload.FileInFolder.objectType = 'NOTE';
          fileUpload.FileSave = item;
          listFileUploadModel.push(fileUpload)
          // });

          this.folderService.uploadFileByFolderType("QLDH", listFileUploadModel, file).subscribe(response => {

            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              // this.listDetailFolder[0].listFile;
              this.uploadedFiles = [];
              this.fileUpload.files = [];
              let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm file thành công" };
              this.showMessage(msg);
            } else {
              let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
              this.showMessage(msg);
            }
            // this.showDialogAddFile = false;
          });
        });

        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        this.handleNoteContent();

        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu ghi chú thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {
    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        // $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.translate.get('order.messages_confirm.delete_confirm_note').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa ghi chú thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }
  /*End*/

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: NoteDocument) {
    this.translate.get('order.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: NoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.documentName;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  exportPFD() {
    this.loading = true;
    this.customerOrderService.ExportPdfCustomerOrder(this.customerOrderModel.OrderId, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      this.loading = false;

      if (result.statusCode == 200) {
        let pdfOrder = result.pdfOrder;
        let orderCode = result.code == null ? "" : result.code.trim();
        let listPDFOrderAttribute: Array<any> = pdfOrder.listPDFOrderAttribute;
        let listPDFOrderAttributeOther: Array<any> = pdfOrder.listPDFOrderAttributeOther;

        let note = (this.noteControl.value === null || this.noteControl.value.trim() === '' || this.noteControl.value === undefined) ? "" : "Ghi chú:  " + this.noteControl.value.trim();

        let dateNow = new Date();
        let imgBase64 = this.getBase64Logo();

        let documentDefinition: any = {
          pageSize: 'A4',
          pageMargins: [20, 20, 20, 20],
          content: [
            {
              table: {
                widths: ['*', '400'],
                body: [
                  [
                    {
                      stack: [
                        {
                          image: imgBase64, width: 100, height: 60
                        }
                      ],
                    },
                    {
                      stack: [
                        {
                          text: "" + pdfOrder.companyName.toUpperCase(),
                          style: { fontSize: 9 },
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Địa chỉ: ' + pdfOrder.companyAddress,
                          style: { fontSize: 9 },
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Điện thoại: ' + pdfOrder.companyPhone,
                          style: { fontSize: 9 },
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Email: ' + pdfOrder.companyEmail,
                          style: { fontSize: 9 },
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Website dịch vụ: ' + pdfOrder.website,
                          style: { fontSize: 9 },
                          alignment: 'left'
                        },
                      ],
                    }
                  ],
                ]
              },
              layout: {
                defaultBorder: false
              },
              lineHeight: 0.75
            },
            {
              text: '',
              margin: [0, 10, 0, 10]
            },
            {
              text: 'ĐƠN HÀNG',
              style: 'header',
              alignment: 'center'
            },
            {
              text: pdfOrder.orderDate,
              style: { fontSize: 9 },
              alignment: 'center'
            },
            {
              text: 'Số đơn hàng:  ' + pdfOrder.orderCode,
              style: { fontSize: 9 },
              alignment: 'right',
              margin: [0, 10, 0, 10]
            },
            {
              text: 'Khách hàng: ' + pdfOrder.customerCode + "-" + pdfOrder.customerName,
              style: { fontSize: 9 },
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              text: 'Địa chỉ: ' + pdfOrder.customerAddress,
              style: { fontSize: 9 },
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              text: 'Người nhận hàng: ' + pdfOrder.recipientName + "      Số điện thoại: " + pdfOrder.customerPhone,
              style: { fontSize: 9 },
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              style: 'table',
              table: {
                widths: [40, 154, 40, 40, 70, 70, 70],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'Mã hàng', style: 'tableHeader', alignment: 'center' },
                    { text: 'Tên mặt hàng', style: 'tableHeader', alignment: 'left' },
                    { text: 'Đvt', style: 'tableHeader', alignment: 'center' },
                    { text: 'Số lượng', style: 'tableHeader', alignment: 'center' },
                    { text: 'Đơn giá', style: 'tableHeader', alignment: 'center' },
                    { text: 'Chiết khấu', style: 'tableHeader', alignment: 'center' },
                    { text: 'Thành tiền', style: 'tableHeader', alignment: 'center' },
                  ],
                ]
              },
              layout: {
                defaultBorder: true,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            },
            {
              text: '',
              margin: [0, -15.5, 0, -15.5]
            },
            {
              style: 'table',
              table: {
                widths: [380, 70, 70],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'Tổng chiết khấu:', style: 'tableHeader', alignment: 'right' },
                    { text: pdfOrder.totalDiscountValue, style: { fontSize: 9, bold: false }, alignment: 'right' },
                    { text: '', style: { fontSize: 9, bold: false }, alignment: 'right' },
                  ],
                ]
              },
              layout: {
                defaultBorder: true,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            },
            {
              text: '',
              margin: [0, -15.5, 0, -15.5]
            },
            {
              style: 'table',
              table: {
                widths: [459, 70],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'Tổng tiền trước thuế:', style: 'tableHeader', alignment: 'right' },
                    { text: pdfOrder.totalBeforVat, style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                  [
                    { text: 'Thuế GTGT:', style: 'tableHeader', alignment: 'right' },
                    { text: pdfOrder.totalVat, style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                  [
                    { text: 'Chiết khấu tổng đơn hàng:', style: 'tableHeader', alignment: 'right' },
                    { text: pdfOrder.discountValue, style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                  [
                    { text: 'Tổng tiền hàng:', style: 'tableHeader', alignment: 'right' },
                    { text: pdfOrder.totalAmountAfter, style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                ]
              },
              layout: {
                defaultBorder: true,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            },
            {
              text: note,
              style: { fontSize: 9 },
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              style: 'table',
              table: {
                widths: [265, 264],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: '', style: 'tableHeader', alignment: 'right' },
                    { text: 'Ngày.....tháng.....năm.....', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: 'NGƯỜI LẬP', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: 'GIÁM ĐỐC', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                    { text: '', style: { fontSize: 9, bold: true }, alignment: 'center' },
                  ],
                  [
                    { text: '(Ký, họ tên)', style: 'timer', alignment: 'center' },
                    { text: '(Ký, họ tên, đóng dấu)', style: 'timer', alignment: 'center' },
                  ],
                ]
              },
              layout: {
                defaultBorder: false,
                paddingTop: function (i, node) { return 2; },
                paddingBottom: function (i, node) { return 2; }
              }
            },
            {
              text: 'Cảm ơn quý khách hàng',
              style: { fontSize: 9, italic: true, bold: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            },
          ],
          styles: {
            header: {
              fontSize: 18.5,
              bold: true
            },
            timer: {
              fontSize: 10,
              italics: true
            },
            table: {
              margin: [0, 15, 0, 15]
            },
            tableHeader: {
              fontSize: 10,
              bold: true
            },
            tableLine: {
              fontSize: 10,
            },
            tableLines: {
              fontSize: 9,
            },
            tableLiness: {
              fontSize: 7,
            },
            StyleItalics: {
              italics: true
            }
          }
        };

        listPDFOrderAttribute.forEach(item => {
          let option = [
            { text: item.productCode, style: 'tableLines', alignment: 'center' },
            { text: item.productName, style: 'tableLines', alignment: 'left' },
            { text: item.unitName, style: 'tableLines', alignment: 'center' },
            { text: item.quantity, style: 'tableLines', alignment: 'right' },
            { text: item.unitPrice, style: 'tableLines', alignment: 'right' },
            { text: item.discountValue, style: 'tableLines', alignment: 'right' },
            { text: item.amount, style: 'tableLines', alignment: 'right' },
          ];
          documentDefinition.content[8].table.body.push(option);
        });

        // let optionOther = [
        //   { colSpan: 9, text: 'Chi phí khác', style: 'tableHeader', alignment: 'left' }
        // ];

        // documentDefinition.content[8].table.body.push(optionOther);

        if (listPDFOrderAttributeOther.length > 0) {
          listPDFOrderAttributeOther.forEach(item => {
            let option = [
              { text: item.productCode, style: '', alignment: 'center' },
              { text: item.productName, style: '', alignment: 'left' },
              { text: item.unitName, style: '', alignment: 'center' },
              { text: item.quantity, style: '', alignment: 'right' },
              { text: item.unitPrice, style: '', alignment: 'right' },
              // { text: item.exchangeRate, style: '', alignment: 'left' },
              // { text: item.vat, style: '', alignment: 'left' },
              { text: item.discountValue, style: '', alignment: 'right' },
              { text: item.amount, style: '', alignment: 'right' },
            ];
            documentDefinition.content[8].table.body.push(option);
          });
        }

        pdfMake.createPdf(documentDefinition).download(orderCode + '.pdf');
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  exportExcel() {
    this.loading = true;
    this.customerOrderService.ExportPdfCustomerOrder(this.customerOrderModel.OrderId, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      this.loading = false;

      if (result.statusCode == 200) {
        let pdfOrder = result.pdfOrder;
        let orderCode = result.code == null ? "" : result.code.trim();
        let listPDFOrderAttribute: Array<any> = pdfOrder.listPDFOrderAttribute;
        let listPDFOrderAttributeOther: Array<any> = pdfOrder.listPDFOrderAttributeOther;
        let note = (this.noteControl.value === null || this.noteControl.value.trim() === '' || this.noteControl.value === undefined) ? "" : "Ghi chú:  " + this.noteControl.value.trim();

        let imgBase64 = this.getBase64Logo();
        let dateUTC = new Date();

        // getMonth() trả về index trong mảng nên cần cộng thêm 1
        let title = orderCode + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
        let workbook = new Workbook();
        let worksheet = workbook.addWorksheet(title);
        worksheet.pageSetup.margins = {
          left: 0.25, right: 0.25,
          top: 0.75, bottom: 0.75,
          header: 0.3, footer: 0.3
        };
        worksheet.pageSetup.paperSize = 9;  //A4 : 9

        /* Image */
        var imgLogo = workbook.addImage({
          base64: imgBase64,
          extension: 'png',
        });

        worksheet.addImage(imgLogo, {
          tl: { col: 0, row: 0 },
          ext: { width: 155, height: 95 }
        });

        let dataRow1 = [];
        dataRow1[4] = pdfOrder.companyName;  //Tên công ty
        let row1 = worksheet.addRow(dataRow1);
        row1.font = { name: 'Times New Roman', size: 10, bold: true };
        worksheet.mergeCells(`D${row1.number}:L${row1.number}`);
        row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow2 = [];
        dataRow2[4] = 'Địa chỉ: ' + pdfOrder.companyAddress;  //Địa chỉ
        let row2 = worksheet.addRow(dataRow2);
        row2.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`D${row2.number}:L${row2.number}`);
        row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow3 = [];
        dataRow3[4] = 'Điện thoại: ' + pdfOrder.companyPhone;  //Số điện thoại
        let row3 = worksheet.addRow(dataRow3);
        row3.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`D${row3.number}:L{row3.number}`);
        row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow4 = [];
        dataRow4[4] = 'Email: ' + pdfOrder.companyEmail;
        let row4 = worksheet.addRow(dataRow4);
        row4.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`D${row4.number}:L${row4.number}`);
        row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow5 = [];
        dataRow5[4] = 'Website dịch vụ: ' + pdfOrder.website;  //Địa chỉ website
        let row5 = worksheet.addRow(dataRow5);
        row5.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`D${row5.number}:L${row5.number}`);
        row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        worksheet.mergeCells(`A${row1.number}:C${row5.number}`);

        let dataRow6 = [];
        dataRow6[1] = 'ĐƠN HÀNG';
        let row6 = worksheet.addRow(dataRow6);
        row6.font = { name: 'Times New Roman', size: 16, bold: true };
        worksheet.mergeCells(`A${row6.number}:L${row6.number}`);
        row6.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        row6.height = 40;

        let dataRow7 = [];
        dataRow7[1] = pdfOrder.orderDate;
        let row7 = worksheet.addRow(dataRow7);
        row7.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${row7.number}:L${row7.number}`);
        row7.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        let dataRow8 = [];
        dataRow8[1] = 'Số đơn hàng: ' + pdfOrder.orderCode;
        let row8 = worksheet.addRow(dataRow8);
        row8.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${row8.number}:L${row8.number}`);
        row8.alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

        let dataRow9 = [];
        dataRow9[1] = 'Khách hàng:';
        dataRow9[4] = pdfOrder.customerCode + "-" + pdfOrder.customerName;
        let row9 = worksheet.addRow(dataRow9);
        row9.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${row9.number}:C${row9.number}`);
        worksheet.mergeCells(`D${row9.number}:L${row9.number}`);
        row9.alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        let dataRow10 = [];
        dataRow10[1] = 'Địa chỉ:';
        dataRow10[4] = pdfOrder.customerAddress;
        let row10 = worksheet.addRow(dataRow10);
        row10.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${row10.number}:C${row10.number}`);
        worksheet.mergeCells(`D${row10.number}:L${row10.number}`);
        row10.alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        let dataRow13 = [];
        dataRow13[1] = 'Người nhận hàng:';
        dataRow13[4] = pdfOrder.recipientName;
        dataRow13[8] = 'Số điện thoại:';
        dataRow13[10] = pdfOrder.customerPhone;
        let row13 = worksheet.addRow(dataRow13);
        row13.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${row13.number}:C${row13.number}`);
        worksheet.mergeCells(`D${row13.number}:G${row13.number}`);
        worksheet.mergeCells(`H${row13.number}:I${row13.number}`);
        worksheet.mergeCells(`J${row13.number}:L${row13.number}`);
        row13.alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

        worksheet.addRow([]);

        /* Header row */
        let dataHeaderRow = ['Mã hàng', 'Tên mặt hàng', '', '', 'Đvt', 'Số lượng', 'Đơn giá', '', 'Chiết khấu', '', 'Thành tiền', ''];
        let headerRow = worksheet.addRow(dataHeaderRow);
        worksheet.mergeCells(`B${headerRow.number}:D${headerRow.number}`);
        worksheet.mergeCells(`G${headerRow.number}:H${headerRow.number}`);
        worksheet.mergeCells(`I${headerRow.number}:J${headerRow.number}`);
        worksheet.mergeCells(`K${headerRow.number}:L${headerRow.number}`);
        headerRow.font = { name: 'Times New Roman', size: 10, bold: true };
        dataHeaderRow.forEach((item, index) => {
          headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
          headerRow.getCell(index + 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '8DB4E2' }
          };
        });
        headerRow.height = 35;

        /* Data table */
        let data: Array<any> = []; //[1, 'Dịch vụ CNTT', 'Gói', '2', '6.000.000', '12.000.000']

        listPDFOrderAttribute.forEach(item => {
          let row: Array<any> = [];
          row[0] = item.productCode;
          row[1] = item.productName;
          row[4] = item.unitName;
          row[5] = item.quantity;
          row[6] = item.unitPrice;
          row[8] = item.discountValue;
          row[10] = item.amount;

          data.push(row);
        });

        if (listPDFOrderAttributeOther.length > 0) {
          listPDFOrderAttributeOther.forEach(item => {
            let row: Array<any> = [];
            row[0] = item.productCode;
            row[1] = item.productName;
            row[4] = item.unitName;
            row[5] = item.quantity;
            row[6] = item.unitPrice;
            row[8] = item.discountValue;
            row[10] = item.amount;

            data.push(row);
          });
        }

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
          worksheet.mergeCells(`B${row.number}:D${row.number}`);
          worksheet.mergeCells(`G${row.number}:H${row.number}`);
          worksheet.mergeCells(`I${row.number}:J${row.number}`);
          worksheet.mergeCells(`K${row.number}:L${row.number}`);

          row.font = { name: 'Times New Roman', size: 11 };

          row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

          row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(2).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

          row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(3).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

          row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(4).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

          row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(5).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

          row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(6).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

          row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(7).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

          row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

          row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

          row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(10).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

          row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          row.getCell(11).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
        });

        let dataDiscountRow = ['Tổng chiết khấu:', '', '', '', '', '', '', '', pdfOrder.totalDiscountValue, '', '', ''];
        let discountRow = worksheet.addRow(dataDiscountRow);
        worksheet.mergeCells(`A${discountRow.number}:H${discountRow.number}`);
        worksheet.mergeCells(`I${discountRow.number}:J${discountRow.number}`);
        worksheet.mergeCells(`K${discountRow.number}:L${discountRow.number}`);
        dataDiscountRow.forEach((item, index) => {
          discountRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 9) {
            discountRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            discountRow.getCell(9).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        discountRow.font = { name: 'Times New Roman', size: 11, bold: true };

        let dataBeforVatRow = ['Tổng tiền trước thuế:', '', '', '', '', '', '', '', '', '', pdfOrder.totalBeforVat, ''];
        let beforVatRow = worksheet.addRow(dataBeforVatRow);
        worksheet.mergeCells(`A${beforVatRow.number}:J${beforVatRow.number}`);
        worksheet.mergeCells(`K${beforVatRow.number}:L${beforVatRow.number}`);
        dataBeforVatRow.forEach((item, index) => {
          beforVatRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 11) {
            beforVatRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            beforVatRow.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        beforVatRow.font = { name: 'Times New Roman', size: 11, bold: true };

        let dataVatRow = ['Thuế GTGT:', '', '', '', '', '', '', '', '', '', pdfOrder.totalVat, ''];
        let vatRow = worksheet.addRow(dataVatRow);
        worksheet.mergeCells(`A${vatRow.number}:J${vatRow.number}`);
        worksheet.mergeCells(`K${vatRow.number}:L${vatRow.number}`);
        dataVatRow.forEach((item, index) => {
          vatRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 11) {
            vatRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            vatRow.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        vatRow.font = { name: 'Times New Roman', size: 11, bold: true };

        let dataDiscountOrderRow = ['Chiết khấu tổng đơn hàng:', '', '', '', '', '', '', '', '', '', pdfOrder.discountValue, ''];
        let discountOrderRow = worksheet.addRow(dataDiscountOrderRow);
        worksheet.mergeCells(`A${discountOrderRow.number}:J${discountOrderRow.number}`);
        worksheet.mergeCells(`K${discountOrderRow.number}:L${discountOrderRow.number}`);
        dataDiscountOrderRow.forEach((item, index) => {
          discountOrderRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 11) {
            discountOrderRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            discountOrderRow.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        discountOrderRow.font = { name: 'Times New Roman', size: 11, bold: true };

        let dataTotalOrderRow = ['Tổng tiền hàng:', '', '', '', '', '', '', '', '', '', pdfOrder.totalAmountAfter, ''];
        let totalOrderRow = worksheet.addRow(dataTotalOrderRow);
        worksheet.mergeCells(`A${totalOrderRow.number}:J${totalOrderRow.number}`);
        worksheet.mergeCells(`K${totalOrderRow.number}:L${totalOrderRow.number}`);
        dataTotalOrderRow.forEach((item, index) => {
          totalOrderRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 11) {
            totalOrderRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            totalOrderRow.getCell(11).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        totalOrderRow.font = { name: 'Times New Roman', size: 11, bold: true };

        worksheet.addRow([]);
        if (note !== "") {
          let noteRow = [];
          noteRow[1] = note;
          let rowNote = worksheet.addRow(noteRow);
          rowNote.font = { name: 'Times New Roman', size: 11 };
          worksheet.mergeCells(`A${rowNote.number}:L${rowNote.number}`);;
          rowNote.alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

          worksheet.addRow([]);
        }

        let footer1 = [];
        footer1[9] = 'Ngày.....tháng.....năm..........';
        let rowFooter1 = worksheet.addRow(footer1);
        rowFooter1.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${rowFooter1.number}:H${rowFooter1.number}`);
        worksheet.mergeCells(`I${rowFooter1.number}:L${rowFooter1.number}`);
        rowFooter1.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        let footer2 = [];
        footer2[2] = 'NGƯỜI LẬP';
        footer2[9] = 'GIÁM ĐỐC';
        let rowFooter2 = worksheet.addRow(footer2);
        rowFooter2.font = { name: 'Times New Roman', size: 11, bold: true };
        worksheet.mergeCells(`B${rowFooter2.number}:E${rowFooter2.number}`);
        worksheet.mergeCells(`I${rowFooter2.number}:L${rowFooter2.number}`);
        rowFooter2.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        let footer3 = [];
        footer3[2] = '(Ký, họ tên)';
        footer3[9] = '(Ký, họ tên, đóng dấu)';
        let rowFooter3 = worksheet.addRow(footer3);
        rowFooter3.font = { name: 'Times New Roman', size: 11, italic: true };
        worksheet.mergeCells(`B${rowFooter3.number}:E${rowFooter3.number}`);
        worksheet.mergeCells(`I${rowFooter3.number}:L${rowFooter3.number}`);
        rowFooter3.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };
        rowFooter3.height = 70;

        worksheet.addRow([]);
        worksheet.addRow([]);

        let footer4 = [];
        footer4[1] = 'Cảm ơn quý khách hàng';
        let rowFooter4 = worksheet.addRow(footer4);
        rowFooter4.font = { name: 'Times New Roman', size: 11, italic: true, bold: true };
        worksheet.mergeCells(`A${rowFooter4.number}:L${rowFooter4.number}`);
        rowFooter4.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

        this.exportToExel(workbook, title);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  showTotalOrder() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  convertDateToTimeSpan(Time: Date): string {
    let result = '';
    let Hour = Time.getHours().toString();
    let Minute = Time.getMinutes().toString();
    result = Hour + ":" + Minute;

    return result;
  }

  convertTimeSpanToDate(TimeSpan: string): Date {
    let result = new Date();
    let index = TimeSpan.indexOf(':');
    let hour = parseInt(TimeSpan.slice(0, index), 10);
    let minute = parseInt(TimeSpan.slice(index + 1, TimeSpan.length), 10);
    result.setHours(hour);
    result.setMinutes(minute);

    return result;
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
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

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

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
