import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener, NgZone } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';

import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';

//MODELS
import { Quote } from '../../models/quote.model';
import { QuoteDetail } from "../../models/quote-detail.model";
import { QuoteDocument } from "../../models/quote-document.model";
import { QuoteProductDetailProductAttributeValue } from "../../models/quote-product-detail-product-attribute-value.model";
import { SendEmailModel, QuoteDetailToSendEmail } from "../../../admin/models/sendEmail.model"

//SERVICES
import { EmployeeService } from '../../../employee/services/employee.service';
import { CustomerService } from '../../../customer/services/customer.service';
import { LeadService } from '../../../lead/services/lead.service';
import { BankService } from '../../../shared/services/bank.service';
import { CategoryService } from "../../../shared/services/category.service";
import { VendorService } from "../../../vendor/services/vendor.service";
import { QuoteService } from '../../services/quote.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { ProductService } from '../../../product/services/product.service';

//DIALOG COMPONENT
import { AddEditProductDialogComponent } from '../add-edit-product-dialog/add-edit-product-dialog.component';
import { GetPermission } from '../../../shared/permission/get-permission';

//
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { MenuItem } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { saveAs } from "file-saver";
import { Workbook } from 'exceljs';
import { DecimalPipe } from '@angular/common';
import * as $ from 'jquery';

import * as XLSX from 'xlsx';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { QuoteCostDetail } from '../../models/quote-cost-detail.model';

interface Customer {
  customerId: string;
  customerEmail: string;
  customerPhone: string;
  fullAddress: string;
}

interface Lead {
  leadId: string;
  email: string;
  phone: string;
  fullAddress: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
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
  bankName: string; //Ngân hàng
  accountNumber: string;  //Số tài khoản
  branchName: string; //Chi nhánh
  accountName: string; //Chủ tài khoản
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

interface QuoteDocumentResponse {
  quoteDocumentId: string;
  quoteId: string,
  documentName: string,
  documentSize: string,
  documentUrl: string,
  createdById: string,
  createdDate: Date,
  updatedById: string,
  updatedDate: Date,
  active: boolean,
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: QuoteDetail,
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: QuoteCostDetail,
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

interface InforExportExcel {
  companyName: string,
  address: string,
  phone: string,
  website: string,
  email: string,
  textTotalMoney: string
}

interface QuoteDetailExcel {
  STT: string,
  ProductCode: string,
  ProductName: string,
  Quantity: number,
  UnitPrice: number,
  CurrencyUnit: string,
  Amount: number,
  Tax: number,
  TaxAmount: number,
  DiscountType: boolean,
  DiscountValue: number,
  TotalAmount: number
}
@Component({
  selector: 'app-pontential-customer-quote',
  templateUrl: './pontential-customer-quote.component.html',
  styleUrls: ['./pontential-customer-quote.component.css'],
  providers: [
    DecimalPipe,
    DynamicDialogRef, DynamicDialogConfig
  ]
})
export class PontentialCustomerQuoteComponent implements OnInit {

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  innerWidth: number = 0; //number window size first

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;

  // @HostListener('document:scroll', [])
  // onScroll(): void {
  //   let num = window.pageYOffset;
  //   if (num > 100) {
  //     this.fixed = true;
  //     var width:number = $('#parent').width();
  //     this.withFiexd = width +'px';
  //     var colT = 0;
  //     if(this.withColCN != width){
  //       colT = this.withColCN - width;
  //       this.withColCN = width;
  //       this.withCol = $('#parentTH').width();
  //     }
  //     this.withFiexdCol = (this.withCol) +'px';
  //   } else {
  //     this.fixed = false;
  //     this.withFiexd = "";
  //     this.withCol = $('#parentTH').width();
  //     this.withColCN = $('#parent').width();
  //     this.withFiexdCol = "";
  //   }
  // }



  /*Get Global Parameter*/
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
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
  /*End*/

  /*Form Báo giá*/
  quoteForm: FormGroup;
  objectControl: FormControl; //Đối tượng báo giá
  paymentMethodControl: FormControl;  //Phương thức thanh toán
  bankAccountControl: FormControl;  //Tài khoản ngân hàng
  daysAreOwedControl: FormControl;  //Số ngày được nợ
  maxDebtControl: FormControl;  //Số nợ tối đa
  quoteStatusControl: FormControl;  //Trạng thái
  intendedDateControl: FormControl; //Ngày gửi dự kiến
  sendDateControl: FormControl; //Ngày gửi
  effectiveDateControl: FormControl; //Ngày hiệu lực
  personInChargeControl: FormControl; //Người phụ trách
  nameQuoteControl: FormControl; //Tên báo giá
  hoSoThauControl: FormControl; //Người phụ trách
  coHoiControl: FormControl; //Người phụ trách
  kenhControl: FormControl; //Người phụ trách
  descriptionControl: FormControl;  //Diễn giải
  noteControl: FormControl; //Ghi chú
  discountTypeControl: FormControl; //Loại chiết khấu (% - Số tiền)
  discountValueControl: FormControl; //Giá trị tổng chiết khấu của báo giá
  /*End*/

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

  /*Biến điều kiện*/
  objLead: any = null;
  objSaleBidding: any = null;
  quoteId: string = null;
  customerId: string = null;
  leadId: string = null;
  contactId: string = null;
  occasionId: string = null;
  saleBiddingId: string = null;
  isCreateQuote: boolean = true;  //Là tạo mới báo giá
  createQuoteFollow: number = 0;  //0: Tạo mới bình thường, 1: Tạo mới cho khách hàng, 2: Tạo mới cho khách hàng tiềm năng
  selectedObjectType: string = 'cus';
  isShowBankAccount: boolean = false;
  customerContactCode: string = 'CUS';
  path: string = "#";
  uploadedFiles: any[] = [];
  cols: any[];
  colsCost: any[];
  selectedColumns: any[];
  selectedColumnsCost: any[];
  selectedItem: any;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  colsFile: any[];
  isShowCreateOrder: boolean = false;
  colsNote: any[];
  maxOrdinal: number = 0;
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
  titleText: string = '';
  contentText: string = '';
  isUpdateAI: boolean = false;
  displayDialog: boolean = false;
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];
  /*End*/

  /*Biến lưu giá trị trả về*/
  workFollowQuote: MenuItem[];
  activeIndex: number = 0;
  isShowWorkFollowQuote: boolean = true;
  objectList = <any>[];
  objCustomer: any = null;
  listCustomerAll: Array<any> = [];
  listCustomer: Array<any> = [];
  listCustomerNew: Array<any> = [];
  listLead: Array<any> = [];
  listHoSoThau: Array<any> = []; //list hồ sơ thầu
  listSaleBidding: Array<any> = []; //list hồ sơ thầu
  listCoHoi: Array<any> = []; //list cơ hội
  listKenh: Array<any> = []; //list kênh bán hàng
  listPersonInCharge: Array<Employee> = []; //list người phụ trách
  listEmpSale: Array<Employee> = []; //list nhân viên bán hàng
  listPaymentMethod: Array<Category> = []; //list Phương thức thanh toán
  listQuoteStatus: Array<Category> = []; //list Trạng thái của báo giá
  listAdditionalInformationTemplate: Array<Category> = []; // Thông tin bổ sung mẫu được lưu trong category
  productCodeSystemList: string[] = [];
  listProductUnitName: string[] = [];
  listProduct: any[] = [];
  listUnitProduct: any[] = [];
  listUnitMoney: any[] = [];
  minDate: Date;

  colLeft: number = 8;
  isShow: boolean = true;
  email: string = '';
  phone: string = '';
  fileName: string = '';
  fullAddress: string = '';
  listBankAccount: Array<BankAccount> = [];
  quoteDate: Date = new Date();
  sellerName: string = '';
  expirationDate: Date = null; //Ngày hết hiệu lực của báo giá
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  arrayQuoteDocumentModel: Array<QuoteDocument> = [];
  listQuoteDetailModel: Array<QuoteDetail> = [];
  listQuoteCostDetailModel: Array<QuoteCostDetail> = [];
  listQuoteDetailExcelModel: Array<QuoteDetailExcel> = [];
  listQuoteDetailModelOrderBy: Array<QuoteDetail> = [];
  listQuoteCostDetailModelOrderBy: Array<QuoteCostDetail> = [];
  arrayOrderProductDetailProductAttributeValueModel: Array<QuoteProductDetailProductAttributeValue> = [];
  CustomerOrderAmountAfterDiscount: number = 0;
  AmountPriceInitial: number = 0;
  AmountPriceCost: number = 0;
  AmountPriceProfit: number = 0;
  ValuePriceProfit: number = 0;
  inforExportExcel: InforExportExcel = {
    companyName: '',
    address: '',
    phone: '',
    website: '',
    email: '',
    textTotalMoney: ''
  }
  /*End*/

  /*MODELS*/
  quoteModel: Quote = new Quote();

  quoteDocumentModel: QuoteDocument = new QuoteDocument();

  // quoteDetailModel: QuoteDetail = {
  //   QuoteDetailId: this.emptyGuid,
  //   VendorId: null,
  //   QuoteId: this.emptyGuid,
  //   ProductId: null,
  //   Quantity: null,
  //   UnitPrice: null,
  //   CurrencyUnit: this.emptyGuid,
  //   ExchangeRate: 1,
  //   Vat: 0,
  //   DiscountType: true,
  //   DiscountValue: null,
  //   Description: '',
  //   OrderDetailType: 0,
  //   CreatedById: this.auth.UserId,
  //   CreatedDate: new Date(),
  //   UpdatedById: this.emptyGuid,
  //   UpdatedDate: new Date(),
  //   Active: true,
  //   UnitId: null,
  //   IncurredUnit: 'CCCCC',
  //   QuoteProductDetailProductAttributeValue: null,
  //   ExplainStr: '',
  //   NameVendor: '',
  //   ProductNameUnit: '',
  //   NameMoneyUnit: '',
  //   SumAmount: 0,
  //   AmountDiscount: 0,
  //   PriceInitial: null,
  //   IsPriceInitial: false,
  //   ProductName: '',
  //   OrderNumber:
  // };

  quoteDetailModel = new QuoteDetail();

  // quoteCostDetailModel: QuoteCostDetail = {
  //   QuoteCostDetailId: null,
  //   CostId: '',
  //   QuoteId: '',
  //   Quantity: 0,
  //   UnitPrice: 0,
  //   CostName: '',
  //   CostCode: '',
  //   Active: true,
  //   CreatedById: null,
  //   CreatedDate: new Date(),
  //   UpdatedById: null,
  //   UpdatedDate: new Date(),
  //   SumAmount: 0
  // };
  /*END*/

  listProductVendorMapping: Array<any> = [];

  constructor(
    private router: Router,
    private el: ElementRef,
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private customerService: CustomerService,
    private productService: ProductService,
    private bankService: BankService,
    private imageService: ImageUploadService,
    private categoryService: CategoryService,
    private vendorSevice: VendorService,
    private leadService: LeadService,
    private quoteService: QuoteService,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private decimalPipe: DecimalPipe,
    private emailConfigService: EmailConfigService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
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
      this.isShowWorkFollowQuote = false;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngOnInit() {
    this.setForm();

    /*Tạo báo giá cho Khách hàng, Khách hàng tiềm năng, hay xem lại chi tiết báo giá*/
    this.route.params.subscribe(params => {
      this.leadId = params['leadId'];
      this.contactId = params['contactId'];
      this.customerId = params['customerId'];
      this.quoteId = params['quoteId'];
      this.quoteId = params['quoteId'];
      this.occasionId = params['occasionId'];
      this.saleBiddingId = params['saleBiddingId'];
      //Nếu tạo mới báo giá
      this.isShowCreateOrder = false;
      let resource = "crm/customer/quote-create";
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
        if (listCurrentActionResource.indexOf("import") == -1) {
          this.actionImport = false; //import file upload
        }
        this.actionDelete = true; //quyền xóa sản phẩm dịch vụ là mặc định có khi tạo mới báo giá
        this.actionEdit = true; //quyền sửa báo giá là mặc định khi tạo mới
      }

      this.isCreateQuote = true;
      if (this.customerId && !this.leadId) {
        this.createQuoteFollow = 1;
        this.selectedObjectType = 'cus';
      } else if (!this.customerId && this.leadId) {
        this.createQuoteFollow = 2;
        this.selectedObjectType = 'lea';
      } else if (!this.customerId && !this.leadId) {
        this.createQuoteFollow = 0;
        this.selectedObjectType = 'cus';
      }
      this.minDate = new Date();
      // }
    });
    /*End*/

    /* nếu truyền từ mở pop up */
    if (this.config.data) {
      this.customerId = this.config.data.customerId;
    }

    this.setTable();
    this.getDataDefault(this.quoteId);
    this.getInforDetailQuote();
  }

  async getInforOccasion() {
    let result: any = await this.leadService.getDatEditLead(this.occasionId, this.auth.UserId);
  }

  setForm() {
    this.objectControl = new FormControl(null, [Validators.required]);
    this.paymentMethodControl = new FormControl(null);
    this.bankAccountControl = new FormControl(null);
    this.daysAreOwedControl = new FormControl('0');
    this.maxDebtControl = new FormControl('0');
    this.quoteStatusControl = new FormControl(null);
    this.intendedDateControl = new FormControl(new Date());
    this.sendDateControl = new FormControl(null);
    this.effectiveDateControl = new FormControl('30', [Validators.required, ageRangeValidator(1, 365)]);
    this.personInChargeControl = new FormControl(null, [Validators.required]);
    this.nameQuoteControl = new FormControl(null, [Validators.required]);
    this.hoSoThauControl = new FormControl(null);
    this.coHoiControl = new FormControl(null);
    this.kenhControl = new FormControl(null);
    this.descriptionControl = new FormControl('');
    this.noteControl = new FormControl('');
    this.discountTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl = new FormControl('0');

    this.quoteForm = new FormGroup({
      objectControl: this.objectControl,
      paymentMethodControl: this.paymentMethodControl,
      bankAccountControl: this.bankAccountControl,
      daysAreOwedControl: this.daysAreOwedControl,
      maxDebtControl: this.maxDebtControl,
      quoteStatusControl: this.quoteStatusControl,
      intendedDateControl: this.intendedDateControl,
      sendDateControl: this.sendDateControl,
      effectiveDateControl: this.effectiveDateControl,
      personInChargeControl: this.personInChargeControl,
      nameQuoteControl: this.nameQuoteControl,
      hoSoThauControl: this.hoSoThauControl,
      coHoiControl: this.coHoiControl,
      kenhControl: this.kenhControl,
      descriptionControl: this.descriptionControl,
      noteControl: this.noteControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl
    });
  }

  setTable() {
    this.cols = [
      { field: 'ExplainStr', header: 'Mã sản phẩm/dịch vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'ProductName', header: 'Tên sản phẩm dịch vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'NameVendor', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'Quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'ProductNameUnit', header: 'Đơn vị tính', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'UnitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'NameMoneyUnit', header: 'Đơn vị tiền', width: '30px', textAlign: 'left', color: '#f44336' },
      { field: 'ExchangeRate', header: 'Tỷ giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'Vat', header: 'Thuế GTGT (%)', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'DiscountValue', header: 'Chiết khấu', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'SumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'Delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = this.cols;

    this.colsFile = [
      { field: 'DocumentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'DocumentSize', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];

    this.colsNote = [
      { field: 'title', header: 'Tiêu đề', width: '30%', textAlign: 'left' },
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

  getDataDefault(quoteId: string) {
    this.loading = true;
    this.quoteService.getDataCreateUpdateQuote(quoteId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCustomerAll = result.listCustomerAll;
        this.listProductVendorMapping = result.listProductVendorMapping;

        this.listKenh = result.listInvestFund;
        if (this.listCustomer.length == 0) {
          this.listCustomer = result.listCustomer;
          this.listCustomer.forEach(item => {
            item.customerName = item.customerCode + " - " + item.customerName;
          });
        }
        if (this.listCustomerNew.length == 0) {
          this.listCustomerNew = result.listCustomerNew;
          this.listCustomerNew.forEach(item => {
            item.customerName = item.customerCode + " - " + item.customerName;
          });
        }
        if (this.listLead.length == 0) {
          this.listLead = result.listLead;
          this.listCoHoi = result.listLead;
        }

        this.listHoSoThau = result.listSaleBidding;
        this.listSaleBidding = result.listSaleBidding;

        if (this.listPaymentMethod.length == 0) {
          this.listPaymentMethod = result.listPaymentMethod;
        }

        this.sellerName = result.employeeSeller.employeeName;

        if (this.listAdditionalInformationTemplate.length == 0) {
          this.listAdditionalInformationTemplate = result.listAdditionalInformationTemplates;
        }
        this.listAdditionalInformationTemplate.forEach(item => {
          this.maxOrdinal++;
          let additionalInformation: AdditionalInformation = {
            ordinal: this.maxOrdinal,
            additionalInformationId: this.emptyGuid,
            objectId: this.emptyGuid,
            objectType: '',
            title: item.categoryCode.trim(),
            content: item.categoryName.trim(),
            active: true,
            createdDate: new Date(),
            createdById: this.emptyGuid,
            updatedDate: null,
            updatedById: null,
            orderNumber: null
          };
          this.listAdditionalInformation.push(additionalInformation);
        });

        if (this.listQuoteStatus.length == 0) {
          this.listQuoteStatus = result.listQuoteStatus;
        }

        if (this.listPersonInCharge.length == 0) {
          this.listPersonInCharge = result.listEmployee;
          this.listEmpSale = result.listEmployee;
          this.listEmpSale.forEach(item => {
            item.employeeName = item.employeeCode + " - " + item.employeeName;
          });
        }

        if (this.occasionId !== null && this.occasionId !== undefined) {
          this.objLead = this.listLead.find(l => l.leadId == this.occasionId);

          let customerCus = this.listCustomer.find(c => c.customerId == this.objLead.customerId);
          let customerLead = this.listCustomerNew.find(c => c.customerId == this.objLead.customerId);

          if ((customerCus === null || customerCus === undefined) && (customerLead === null || customerLead === undefined)) {
            let cusObj = this.listCustomerAll.find(c => c.customerId == this.objLead.customerId);
            if (cusObj !== null && cusObj != undefined) {
              if (cusObj.statusName === 'DD') {
                this.listCustomer.push(cusObj);
              }
              else {
                this.listCustomerNew.push(cusObj);
              }
              customerCus = this.listCustomer.find(c => c.customerId == this.objLead.customerId);
              customerLead = this.listCustomerNew.find(c => c.customerId == this.objLead.customerId);
            }
          }

          if (customerCus === null || customerCus === undefined) {
            this.selectedObjectType = 'lea';
            this.objectControl.setValue(customerLead);
            this.changeCustomer(customerLead, '');
          }
          else {
            if (customerLead === null || customerLead === undefined) {
              this.selectedObjectType = 'cus';
              this.objectControl.setValue(customerCus);
              this.changeCustomer(customerCus, '');
            }
          }
          this.coHoiControl.setValue(this.objLead);
          this.changeLead(this.objLead);
        }
        if (this.customerId !== null && this.customerId !== undefined) {
          let customerCus = this.listCustomer.find(c => c.customerId == this.customerId);
          let customerLead = this.listCustomerNew.find(c => c.customerId == this.customerId);

          if ((customerCus === null || customerCus === undefined) && (customerLead === null || customerLead === undefined)) {
            let cusObj = this.listCustomerAll.find(c => c.customerId == this.customerId);
            if (cusObj !== null && cusObj != undefined) {
              if (cusObj.statusName === 'DD') {
                this.listCustomer.push(cusObj);
              }
              else {
                this.listCustomerNew.push(cusObj);
              }
              customerCus = this.listCustomer.find(c => c.customerId == this.customerId);
              customerLead = this.listCustomerNew.find(c => c.customerId == this.customerId);
            }
          }

          if (customerCus === null || customerCus === undefined) {
            this.selectedObjectType = 'lea';
            this.objectControl.setValue(customerLead);
            this.changeCustomer(customerLead, '');
          }
          if (customerLead === null || customerLead === undefined) {
            this.selectedObjectType = 'cus';
            this.objectControl.setValue(customerCus);
            this.changeCustomer(customerCus, '');
          }
        }
        if (this.saleBiddingId !== null && this.saleBiddingId !== undefined) {
          let objLead = this.listHoSoThau.find(l => l.saleBiddingId == this.saleBiddingId);
          this.objSaleBidding = objLead;
          let customerCus = this.listCustomer.find(c => c.customerId == objLead.customerId);
          let customerLead = this.listCustomerNew.find(c => c.customerId == objLead.customerId);

          if ((customerCus === null || customerCus === undefined) && (customerLead === null || customerLead === undefined)) {
            let cusObj = this.listCustomerAll.find(c => c.customerId == objLead.customerId);
            if (cusObj !== null && cusObj != undefined) {
              if (cusObj.statusName === 'DD') {
                this.listCustomer.push(cusObj);
              }
              else {
                this.listCustomerNew.push(cusObj);
              }
              customerCus = this.listCustomer.find(c => c.customerId == objLead.customerId);
              customerLead = this.listCustomerNew.find(c => c.customerId == objLead.customerId);
            }
          }

          if (customerCus === null || customerCus === undefined) {
            this.selectedObjectType = 'lea';
            this.objectControl.setValue(customerLead);
            this.changeCustomer(customerLead, objLead.personInChargeId);
          }
          if (customerLead === null || customerLead === undefined) {
            this.selectedObjectType = 'cus';
            this.objectControl.setValue(customerCus);
            this.changeCustomer(customerCus, objLead.personInChargeId);
          }
          this.occasionId = objLead.leadId;
          this.objLead = this.listLead.find(l => l.leadId == this.occasionId);
          this.coHoiControl.setValue(this.objLead);

          this.hoSoThauControl.setValue(objLead);
          this.changeSaleBidding(objLead);
        }

        if (this.selectedObjectType == 'cus') {
          this.objectList = this.listCustomer;
        } else if (this.selectedObjectType == 'lea') {
          // this.objectList = this.listLead;
          this.objectList = this.listCustomerNew;
        }

        this.setDefaultValueForm();
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }
  goDetailLead() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/lead/detail', { leadId: this.occasionId }]));
    window.open(url, '_blank');
  }
  goDetaiSaleding() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/sale-bidding/detail', { saleBiddingId: this.saleBiddingId }]));
    window.open(url, '_blank');
  }
  setDefaultValueForm() {
    if (this.isCreateQuote) {
      /*Nếu là tạo mới báo giá*/

      //Tiến trình của Báo giá
      this.workFollowQuote = [
        {
          label: this.listQuoteStatus.find(x => x.categoryCode == 'MTA').categoryName
        },
        {
          label: this.listQuoteStatus.find(x => x.categoryCode == 'DLY').categoryName
        },
        {
          label: this.listQuoteStatus.find(x => x.categoryCode == 'CHO').categoryName
        },
        {
          label: this.listQuoteStatus.find(x => x.categoryCode == 'DTH').categoryName
        }
      ];

      if (this.createQuoteFollow == 1) {
        const toSelectCustomer = this.objectList.find(x => x.customerId == this.customerId);
        this.quoteForm.controls['objectControl'].setValue(toSelectCustomer);

        this.email = toSelectCustomer.customerEmail;
        this.phone = toSelectCustomer.customerPhone;
        this.fullAddress = toSelectCustomer.fullAddress;
      } else if (this.createQuoteFollow == 2) {
        const toSelectCustomer = this.objectList.find(x => x.customerId == this.customerId);
        this.quoteForm.controls['objectControl'].setValue(toSelectCustomer);

        this.email = toSelectCustomer.customerEmail;
        this.phone = toSelectCustomer.customerPhone;
        this.fullAddress = toSelectCustomer.fullAddress;
      }

      //Phương thức thanh toán
      const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
      this.quoteForm.controls['paymentMethodControl'].setValue(toSelectPaymentMethod);
      //End

      //Trạng thái báo giá
      const toSelectQuoteStatus: Category = this.listQuoteStatus.find(x => x.isDefault == true);
      this.quoteStatusControl.setValue(toSelectQuoteStatus);
      //End
    }
  }

  /*Event chuyển loại khách hàng (Khách hàng hoặc Khách hàng tiềm năng)*/
  changeObjectType(objecType: any) {
    if (objecType == 'cus') {
      this.objectList = this.listCustomer;
    } else if (objecType == 'lea') {
      // this.objectList = this.listLead;
      this.objectList = this.listCustomerNew;
    }

    if (!this.objectList.find(x => x == this.objectControl.value)) {
      //Nếu khách hàng hiện tại đang được chọn không thuộc objectList thì reset các trường
      this.objectControl.setValue(null);
      this.email = '';
      this.phone = '';
      this.fullAddress = '';

      //Reset Phương thức thanh toán
      const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
      this.paymentMethodControl.setValue(toSelectPaymentMethod);
      //End

      this.daysAreOwedControl.setValue('0');  //Reset Số ngày được nợ
      this.maxDebtControl.setValue('0');  //Reset Số nợ tối đa

      //Reset Tài khoản ngân hàng
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
      //End
    }
  }

  /*Event thay đổi khách hàng*/
  changeCustomer(value: any, personInChargeId: string) {
    if (value) {
      this.objCustomer = value;
      this.email = value.customerEmail;
      this.phone = value.customerPhone;
      this.fullAddress = value.fullAddress;
      this.listCoHoi = this.listLead.filter(x => x.customerId == value.customerId);
      this.listHoSoThau = this.listSaleBidding.filter(x => x.customerId == value.customerId);
    } else {
      /**Reset các trường khi không có khách hàng nào được chọn*/
      this.objCustomer = null;
      this.email = '';
      this.phone = '';
      this.fullAddress = '';
    }

    //Reset Phương thức thanh toán
    const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
    this.paymentMethodControl.setValue(toSelectPaymentMethod);
    //End

    this.daysAreOwedControl.setValue('0');  //Reset Số ngày được nợ
    this.maxDebtControl.setValue('0');  //Reset Số nợ tối đa

    //Reset Tài khoản ngân hàng
    this.isShowBankAccount = false;
    this.listBankAccount = [];
    this.bankAccountControl.setValue(null);
    if (value !== null && value !== undefined && value.personInChargeId !== null) {
      let daysAreOwed = value.maximumDebtDays ? value.maximumDebtDays : 0;
      this.daysAreOwedControl.setValue(daysAreOwed);
      // let maxDebt = customer.maximumDebtValue;
      // this.maxDebtControl.setValue(maxDebt);
      let maxAmount = value.maximumDebtValue ? value.maximumDebtValue : 0;
      this.maxDebtControl.setValue(maxAmount);

      this.quoteService.getEmployeeSale(this.listPersonInCharge, value.personInChargeId).subscribe(response => {
        let result: any = response;
        this.listEmpSale = result.listEmployee;
        this.listEmpSale.forEach(item => {
          item.employeeName = item.employeeName;
        });
        if (personInChargeId === '') {
          let emp = this.listEmpSale.find(e => e.employeeId == value.personInChargeId);
          this.personInChargeControl.setValue(emp);
        }
        else {
          let emp = this.listEmpSale.find(e => e.employeeId == personInChargeId);
          this.personInChargeControl.setValue(emp);
        }
      });
    }
    else {
      this.listEmpSale = this.listPersonInCharge;
      this.listQuoteDetailModel = this.listQuoteDetailModel;
      this.personInChargeControl.setValue(null);
    }
    //End
  }

  /*Event thay đổi khách hàng tiềm năng*/
  changeLead(value: any) {
    if (value) {
      this.listHoSoThau = this.listSaleBidding.filter(h => h.leadId == value.leadId);

      this.quoteModel.leadId = value.leadId;
      this.listQuoteDetailModel = [];
      this.AmountPriceInitial = 0;
      value.listLeadDetail.forEach(element => {
        let obj: QuoteDetail = new QuoteDetail;
        // obj.VendorId = element.vendorId;
        // obj.ProductId = element.productId;
        // obj.Quantity = element.quantity;
        // obj.UnitPrice = element.unitPrice;
        // obj.CurrencyUnit = element.currencyUnit;
        // obj.ExchangeRate = element.exchangeRate;
        // obj.Vat = element.vat;
        // obj.DiscountType = element.discountType;
        // obj.DiscountValue = element.discountValue;
        // obj.Description = element.description;
        // obj.OrderDetailType = element.orderDetailType;
        // obj.UnitId = element.unitId;
        // obj.IncurredUnit = element.incurredUnit;
        // obj.ExplainStr = element.productName;
        // obj.NameMoneyUnit = element.nameMoneyUnit;
        // obj.NameVendor = element.nameVendor;
        // obj.ProductNameUnit = element.productNameUnit;
        // obj.ProductName = element.productName;
        // let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
        // if(obj.DiscountType){
        //   obj.AmountDiscount = sumprice * element.discountValue / 100;
        // }
        // else{
        //   obj.AmountDiscount = sumprice - element.discountValue;
        // }
        // let vatAmount = (sumprice - obj.AmountDiscount) * element.vat / 100;
        // obj.SumAmount = sumprice - obj.AmountDiscount + vatAmount;

        // element.leadProductDetailProductAttributeValue.forEach(item => {
        //   let attributeValue : QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
        //   attributeValue.ProductId = item.productId;
        //   attributeValue.ProductAttributeCategoryId = item.productAttributeCategoryId;
        //   attributeValue.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;
        //   obj.QuoteProductDetailProductAttributeValue.push(attributeValue);
        // });
        // this.listQuoteDetailModel.push(obj);
        // let priceIn = (obj.PriceInitial === null || obj.PriceInitial === undefined) ? 0 : obj.PriceInitial;
        // this.AmountPriceInitial = this.AmountPriceInitial + priceIn;
        // this.quoteModel.amount = this.quoteModel.amount + obj.SumAmount;
      });

      //Cộng tổng giá cho toàn bộ đơn hàng
      let discountType: DiscountType = this.discountTypeControl.value;
      let valueDis = discountType.value;
      let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
      if (valueDis) {
        this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
      } else {
        this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
      }
      this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
      if (this.CustomerOrderAmountAfterDiscount !== 0) {
        this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
      }
      else {
        this.ValuePriceProfit = 0;
      }
    }
    else {
      this.listQuoteDetailModel = this.listQuoteDetailModel;
      this.quoteModel.leadId = null;
      this.listHoSoThau = this.listSaleBidding;
    }
  }

  /*Event thay đổi khách hàng tiềm năng*/
  changeSaleBidding(value: any) {
    if (value) {
      this.listCoHoi = [];
      let leadObj = this.listLead.find(l => l.leadId == value.leadId);
      this.listCoHoi.push(leadObj);
      this.coHoiControl.setValue(leadObj);

      this.quoteModel.saleBiddingId = value.saleBiddingId;
      this.listQuoteDetailModel = [];
      this.AmountPriceInitial = 0;
      value.saleBiddingDetail.forEach(element => {
        let obj: QuoteDetail = new QuoteDetail;
        // obj.VendorId = element.vendorId;
        // obj.ProductId = element.productId;
        // obj.Quantity = element.quantity;
        // obj.UnitPrice = element.unitPrice;
        // obj.CurrencyUnit = element.currencyUnit;
        // obj.ExchangeRate = element.exchangeRate;
        // obj.Vat = element.vat;
        // obj.DiscountType = element.discountType;
        // obj.DiscountValue = element.discountValue;
        // obj.Description = element.description;
        // obj.OrderDetailType = element.orderDetailType;
        // obj.UnitId = element.unitId;
        // obj.IncurredUnit = element.incurredUnit;
        // obj.ExplainStr = element.productName;
        // obj.ProductName = element.productName;
        // obj.NameMoneyUnit = element.nameMoneyUnit;
        // obj.NameVendor = element.nameVendor;
        // obj.ProductNameUnit = element.productNameUnit;
        // let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
        // if(obj.DiscountType){
        //   obj.AmountDiscount = sumprice * element.discountValue / 100;
        // }
        // else{
        //   obj.AmountDiscount = sumprice - element.discountValue;
        // }
        // let vatAmount = (sumprice - obj.AmountDiscount) * element.vat / 100;
        // obj.SumAmount = sumprice - obj.AmountDiscount + vatAmount;

        // element.saleBiddingDetailProductAttribute.forEach(item => {
        //   let attributeValue : QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
        //   attributeValue.ProductId = item.productId;
        //   attributeValue.ProductAttributeCategoryId = item.productAttributeCategoryId;
        //   attributeValue.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;
        //   obj.QuoteProductDetailProductAttributeValue.push(attributeValue);
        // });
        // this.listQuoteDetailModel.push(obj);
        // let priceIn = (obj.PriceInitial === null || obj.PriceInitial === undefined) ? 0 : obj.PriceInitial;
        // this.AmountPriceInitial = this.AmountPriceInitial + priceIn;
        // this.quoteModel.amount = this.quoteModel.amount + obj.SumAmount;
      });

      //Cộng tổng giá cho toàn bộ đơn hàng
      // this.quoteModel.amount = this.quoteModel.amount + this.quoteDetailModel.SumAmount;
      let discountType: DiscountType = this.discountTypeControl.value;
      let valueDis = discountType.value;
      let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
      if (valueDis) {
        this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
      } else {
        this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
      }
      this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
      if (this.CustomerOrderAmountAfterDiscount !== 0) {
        this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
      }
      else {
        this.ValuePriceProfit = 0;
      }

      if (value.personInChargeId !== null) {

        let emp = this.listEmpSale.find(e => e.employeeId == value.personInChargeId);
        this.personInChargeControl.setValue(emp);
      }
    }
    else {
      this.quoteModel.saleBiddingId = null;
      this.listCoHoi = this.listLead;
    }
  }

  /*Event thay đổi phương thức thanh toán*/
  changeMethodControl(value: Category) {
    if (value.categoryCode == "BANK") {
      let customer: Customer = this.objectControl.value;

      if (customer && this.selectedObjectType == 'cus') {
        //Nếu đã chọn khách hàng
        this.isShowBankAccount = true;
        let customerId = customer.customerId;

        this.bankService.getAllBankAccountByObject(customerId, this.customerContactCode).subscribe(response => {
          var result = <any>response;

          this.listBankAccount = result.bankAccountList;
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          }
        });
      }
    } else if (value.categoryCode == "CASH") {
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
    }
  }

  /*Event trạng thái báo giá*/
  changeQuoteStatus() {
    let quoteStatus: Category = this.quoteStatusControl.value;
    if (quoteStatus.categoryCode == 'CHO' || quoteStatus.categoryCode == 'DTH' || quoteStatus.categoryCode == 'DTR') {
      //Với những trạng thái này thì trường Ngày gửi bắt buộc phải có giá trị
      this.sendDateControl.setValidators([Validators.required]);
      this.sendDateControl.updateValueAndValidity();
    } else {
      this.sendDateControl.setValidators(null);
      this.sendDateControl.updateValueAndValidity();
    }
  }

  quoteStatusBeforeClear: string = null;  //Biến lưu mã trạng thái báo giá trước khi compareDate() được thực hiện
  /*Event thay đổi Ngày gửi dự kiến và ngày hiệu lực*/
  compareDate() {
    let sendDate: Date = this.sendDateControl.value;
    let effectiveDate = this.effectiveDateControl.value;
    if (effectiveDate == null || effectiveDate == '') {
      this.effectiveDateControl.setValue('30');
      effectiveDate = 30;
    } else {
      effectiveDate = parseFloat(this.effectiveDateControl.value.replace(/,/g, ''));
    }

    let quoteStatus: Category = this.quoteStatusControl.value;
    this.quoteStatusBeforeClear = quoteStatus.categoryCode; //Lưu lại mã trạng thái
    if (sendDate) {
      if (quoteStatus.categoryCode == 'MTA' || quoteStatus.categoryCode == 'DLY' || quoteStatus.categoryCode == 'HOA') {
        let toSelectQuoteStatus = this.listQuoteStatus.find(x => x.categoryCode == 'CHO');
        this.quoteStatusControl.setValue(toSelectQuoteStatus);
      }

      //Tính: ngày hết hạn của báo giá = ngày gửi + ngày hiệu lực
      let current_miliseconds = sendDate.getTime();
      let result_miliseconds = current_miliseconds + effectiveDate * 1000 * 60 * 60 * 24;
      this.expirationDate = convertToUTCTime(new Date(result_miliseconds));
    }
  }

  /*Event Khi xóa ngày gửi (event này luôn được gọi sau event onBlur và onSelect)
  * Xử dụng event này trong trường hợp trạng thái của báo giá không thay đổi khi nó thuộc một trong các
  * trạng thái được quy định
  */
  clearSendDate() {
    //Nếu trạng thái trước đó của báo giá là: Mới tạo, Đang xử lý hoặc Hoãn thì giữ nguyên trạng thái đó
    if (this.quoteStatusBeforeClear == 'MTA' || this.quoteStatusBeforeClear == 'DLY' ||
      this.quoteStatusBeforeClear == 'HOA') {
      let toSelectQuoteStatus = this.listQuoteStatus.find(x => x.categoryCode == this.quoteStatusBeforeClear);
      this.quoteStatusControl.setValue(toSelectQuoteStatus);
    }

    //Xóa ngày hết hạn của báo giá vì ngày gửi hiện tại đã null
    this.expirationDate = null;
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
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

  /*Thêm một thông tin bổ sung*/
  addAI() {
    if (this.titleText == null || this.titleText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tiêu đề không được để trống' };
      this.showMessage(msg);
    } else if (this.contentText == null || this.contentText.trim() == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Nội dung không được để trống' };
      this.showMessage(msg);
    } else {
      this.maxOrdinal++;
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
    // let rowData: AdditionalInformation = event.data;
    // this.isUpdateAI = true;
    // this.AIUpdate.ordinal = rowData.ordinal;
    // this.AIUpdate.title = rowData.title;

    // this.titleText = rowData.title;
    // this.contentText = rowData.content;
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

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let cusGroupId = null;
    if (this.objCustomer !== null && this.objCustomer !== undefined) cusGroupId = this.objCustomer.customerGroupId;
    let ref = this.dialogService.open(AddEditProductDialogComponent, {
      data: {
        isCreate: true,
        cusGroupId: cusGroupId
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
          this.quoteDetailModel = result.quoteDetailModel;
          this.listQuoteDetailModel.push(this.quoteDetailModel);

          this.listQuoteDetailModelOrderBy = [];
          for (let i = this.listQuoteDetailModel.length - 1; i >= 0; i--) {
            this.listQuoteDetailModelOrderBy.push(this.listQuoteDetailModel[i]);
          }
          this.listQuoteDetailModel = [];
          this.listQuoteDetailModel = this.listQuoteDetailModelOrderBy;

          // //Cộng tổng giá cho toàn bộ đơn hàng
          // this.quoteModel.amount = this.quoteModel.amount + this.quoteDetailModel.SumAmount;
          // this.AmountPriceInitial = this.AmountPriceInitial + (this.quoteDetailModel.PriceInitial * this.quoteDetailModel.Quantity * this.quoteDetailModel.ExchangeRate);
          // let discountType: DiscountType = this.discountTypeControl.value;
          // let value = discountType.value;
          // let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
          // if (value) {
          //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
          // } else {
          //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
          // }

          // this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
          // if(this.CustomerOrderAmountAfterDiscount !== 0){
          //   this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
          // }
          // else{
          //   this.ValuePriceProfit = 0;
          // }
          // this.restartCustomerOrderDetailModel();
        }
      }
    });
  }

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
          // this.quoteCostDetailModel = result.quoteDetailModel;
          // this.listQuoteCostDetailModel.push(this.quoteCostDetailModel);

          // this.listQuoteCostDetailModelOrderBy = [];
          // for (let i = this.listQuoteCostDetailModel.length - 1; i >= 0; i--) {
          //   this.listQuoteCostDetailModelOrderBy.push(this.listQuoteCostDetailModel[i]);
          // }
          // this.listQuoteCostDetailModel = [];
          // this.listQuoteCostDetailModel = this.listQuoteCostDetailModelOrderBy;

          // this.AmountPriceCost = this.AmountPriceCost + (result.quoteDetailModel.Quantity * result.quoteDetailModel.UnitPrice);
          // //Cộng tổng giá cho toàn bộ đơn hàng
          // this.quoteModel.Amount = this.quoteModel.Amount + this.quoteDetailModel.SumAmount;
          // let discountType: DiscountType = this.discountTypeControl.value;
          // let value = discountType.value;
          // let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
          // if (value) {
          //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.Amount + this.AmountPriceCost - ((this.quoteModel.Amount + this.AmountPriceCost) * discountValue) / 100;
          // } else {
          //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.Amount + this.AmountPriceCost - discountValue;
          // }

          let discountType: DiscountType = this.discountTypeControl.value;
          let valueDis = discountType.value;
          let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
          if (valueDis) {
            this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
          } else {
            this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
          }

          this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
          if (this.CustomerOrderAmountAfterDiscount !== 0) {
            this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
          }
          else {
            this.ValuePriceProfit = 0;
          }
          this.restartCustomerOrderDetailModel();
        }
      }
    });
  }
  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listQuoteDetailModel.indexOf(dataRow);
        this.quoteModel.amount = this.quoteModel.amount - dataRow.SumAmount;
        let discountType: DiscountType = this.discountTypeControl.value;
        let value = discountType.value;
        let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
        if (value) {
          this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
        } else {
          this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
        }
        this.listQuoteDetailModel.splice(index, 1);

        this.listQuoteDetailModelOrderBy = [];
        this.AmountPriceInitial = 0;
        // for (let i = this.listQuoteDetailModel.length - 1; i >= 0; i--) {
        //   this.listQuoteDetailModelOrderBy.push(this.listQuoteDetailModel[i]);
        //   let price = (this.listQuoteDetailModel[i].PriceInitial === undefined || this.listQuoteDetailModel[i].PriceInitial === null) ? 0 : this.listQuoteDetailModel[i].PriceInitial;
        //   this.AmountPriceInitial = this.AmountPriceInitial + (this.listQuoteDetailModel[i].Quantity * price * this.listQuoteDetailModel[i].ExchangeRate);
        // }
        // this.listQuoteDetailModel = [];
        // this.listQuoteDetailModel = this.listQuoteDetailModelOrderBy;

        // this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
        // if(this.CustomerOrderAmountAfterDiscount !== 0){
        //   this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
        // }
        // else{
        //   this.ValuePriceProfit = 0;
        // }
      }
    });
  }
  /*Xóa một sản phẩm dịch vụ*/
  deleteCostItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listQuoteCostDetailModel.indexOf(dataRow);
        // this.quoteCostDetailModel.SumAmount = this.quoteCostDetailModel.SumAmount - dataRow.SumAmount;
        // let discountType: DiscountType = this.discountTypeControl.value;
        // let value = discountType.value;
        // let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
        // if (value) {
        //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.Amount + this.AmountPriceCost - ((this.quoteModel.Amount + this.AmountPriceCost) * discountValue) / 100;
        // } else {
        //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.Amount + this.AmountPriceCost - discountValue;
        // }

        // this.listQuoteCostDetailModel.splice(index, 1);

        // this.listQuoteCostDetailModelOrderBy = [];
        // for (let i = this.listQuoteCostDetailModel.length - 1; i >= 0; i--) {
        //   this.listQuoteCostDetailModelOrderBy.push(this.listQuoteCostDetailModel[i]);
        // }
        // this.listQuoteCostDetailModel = [];
        // this.listQuoteCostDetailModel = this.listQuoteCostDetailModelOrderBy;

        // var AmountCost = 0;
        // this.listQuoteCostDetailModel.forEach(item => {
        //   AmountCost = AmountCost + (item.UnitPrice * item.Quantity);
        // });
        // this.AmountPriceCost = AmountCost;
        // let discountType: DiscountType = this.discountTypeControl.value;
        // let valueDis = discountType.value;
        // let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
        // if (valueDis) {
        //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
        // } else {
        //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
        // }

        // this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
        // if(this.CustomerOrderAmountAfterDiscount !== 0){
        //   this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
        // }
        // else{
        //   this.ValuePriceProfit = 0;
        // }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listQuoteDetailModel.indexOf(dataRow);
      var OldArray = this.listQuoteDetailModel[index];

      // let titlePopup = '';
      // if (OldArray.OrderDetailType == 0) {
      //   titlePopup = 'Sửa sản phẩm dịch vụ';
      // } else if (OldArray.OrderDetailType == 1) {
      //   titlePopup = 'Sửa chi phí phát sinh';
      // }

      // let cusGroupId = null;
      // if(this.objCustomer !== null && this.objCustomer !== undefined) cusGroupId = this.objCustomer.customerGroupId;
      // let ref = this.dialogService.open(AddEditProductDialogComponent, {
      //   data: {
      //     isCreate: false,
      //     cusGroupId: cusGroupId,
      //     quoteDetailModel: OldArray
      //   },
      //   header: titlePopup,
      //   width: '70%',
      //   baseZIndex: 1030,
      //   contentStyle: {
      //     "min-height": "280px",
      //     "max-height": "600px",
      //     "overflow": "auto"
      //   }
      // });

      // ref.onClose.subscribe((result: ResultDialog) => {
      //   if (result) {
      //     if (result.status) {
      //       this.listQuoteDetailModel.splice(index, 1);
      //       this.quoteDetailModel = result.quoteDetailModel;
      //       this.listQuoteDetailModel.push(this.quoteDetailModel);

      //       /*Tính lại tổng tiền của đơn hàng*/
      //       var TotalSumaMount = 0;
      //       var TotalPriceInitial = 0;
      //       this.listQuoteDetailModel.forEach(function (item) {
      //         TotalSumaMount = TotalSumaMount + item.SumAmount;

      //         let price = (item.PriceInitial === null || item.PriceInitial === undefined) ? 0 : item.PriceInitial;
      //         TotalPriceInitial = TotalPriceInitial + (price * item.Quantity * item.ExchangeRate);
      //       });
      //       this.AmountPriceInitial = TotalPriceInitial;
      //       this.quoteModel.amount = TotalSumaMount;
      //       let discountType: DiscountType = this.discountTypeControl.value;
      //       let value = discountType.value;
      //       let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
      //       if (value) {
      //         this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
      //       } else {
      //         this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
      //       }

      //       this.listQuoteDetailModelOrderBy = [];
      //       for (let i = this.listQuoteDetailModel.length - 1; i >= 0; i--) {
      //         this.listQuoteDetailModelOrderBy.push(this.listQuoteDetailModel[i]);
      //       }
      //       this.listQuoteDetailModel = [];
      //       this.listQuoteDetailModel = this.listQuoteDetailModelOrderBy;

      //       this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
      //       if(this.CustomerOrderAmountAfterDiscount !== 0){
      //         this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
      //       }
      //       else{
      //         this.ValuePriceProfit = 0;
      //       }
      //       this.restartCustomerOrderDetailModel();
      //     }
      //   }
      // });
    }
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowCostSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listQuoteCostDetailModel.indexOf(dataRow);
      var OldArray = this.listQuoteCostDetailModel[index];

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
            // this.listQuoteCostDetailModel.splice(index, 1);
            // this.quoteCostDetailModel = result.quoteDetailModel;
            // this.listQuoteCostDetailModel.push(this.quoteCostDetailModel);

            // var AmountCost = 0;
            // this.listQuoteCostDetailModel.forEach(item => {
            //   AmountCost = AmountCost + (item.UnitPrice * item.Quantity);
            // });
            // this.AmountPriceCost = AmountCost;
            // /*Tính lại tổng tiền của đơn hàng*/
            // var TotalSumaMount = 0;
            // this.listQuoteDetailModel.forEach(function (item) {
            //   TotalSumaMount = TotalSumaMount + item.SumAmount;
            // });
            // this.quoteModel.Amount = TotalSumaMount;
            // let discountType: DiscountType = this.discountTypeControl.value;
            // let value = discountType.value;
            // let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
            // if (value) {
            //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.Amount + this.AmountPriceCost - ((this.quoteModel.Amount + this.AmountPriceCost) * discountValue) / 100;
            // } else {
            //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.Amount + this.AmountPriceCost - discountValue;
            // }
            let listQuoteCostDetailModelOrderBy = [];
            for (let i = this.listQuoteCostDetailModel.length - 1; i >= 0; i--) {
              listQuoteCostDetailModelOrderBy.push(this.listQuoteCostDetailModel[i]);
            }
            this.listQuoteCostDetailModel = [];
            this.listQuoteCostDetailModel = listQuoteCostDetailModelOrderBy;

            let discountType: DiscountType = this.discountTypeControl.value;
            let valueDis = discountType.value;
            let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
            if (valueDis) {
              this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
            } else {
              this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
            }

            this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
            if (this.CustomerOrderAmountAfterDiscount !== 0) {
              this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
            }
            else {
              this.ValuePriceProfit = 0;
            }
            this.restartCustomerOrderDetailModel();
          }
        }
      });
    }
  }
  showTotalQuote() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }
  /*Event khi thay đổi loại chiết khấu: Theo % hoặc Theo số tiền*/
  changeDiscountType(value: DiscountType) {
    let codeDiscountType = value.code;
    let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
    //Nếu loại chiết khấu là theo % thì giá trị discountValue không được lớn hơn 100%
    if (codeDiscountType == "PT") {
      if (discountValue > 100) {
        discountValue = 100;
        this.discountValueControl.setValue('100');
      }
    }

    /*Tính lại tổng thành tiền của báo giá*/
    let discountType = value.value;

    // var TotalSumaMount = 0;
    // this.listQuoteDetailModel.forEach(function (item) {
    //   TotalSumaMount = TotalSumaMount + item.SumAmount;
    // });
    // this.quoteModel.amount = TotalSumaMount;

    // if (discountType) {
    //   //Nếu theo %
    //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
    // } else {
    //   //Nếu theo Số tiền
    //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
    // }

    /*
    * Nếu số thành tiền âm (vì nếu loại chiết khấu là % thì giá trị chiết khấu lớn nhất là 100%
    * nên số thành tiền không thể âm, vậy nếu số thành tiền âm thì chỉ có trường hợp giá trị
    * chiết khấu lúc này là Số tiền)
    */
    // if (this.CustomerOrderAmountAfterDiscount < 0) {
    //   this.CustomerOrderAmountAfterDiscount = 0;
    //   this.discountValueControl.setValue(this.quoteModel.amount.toString());
    // }
    // this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
    // if(this.CustomerOrderAmountAfterDiscount !== 0){
    //   this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
    // }
    // else{
    //   this.ValuePriceProfit = 0;
    // }
    /*End*/
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

    /*Tính lại tổng thành tiền của báo giá*/
    // var TotalSumaMount = 0;
    // this.listQuoteDetailModel.forEach(function (item) {
    //   TotalSumaMount = TotalSumaMount + item.SumAmount;
    // });
    // this.quoteModel.amount = TotalSumaMount;

    // if (discountType.value) {
    //   //Nếu theo %
    //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
    // } else {
    //   //Nếu theo Số tiền
    //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
    // }

    /*
    * Nếu số thành tiền âm (vì nếu loại chiết khấu là % thì giá trị chiết khấu lớn nhất là 100%
    * nên số thành tiền không thể âm, vậy nếu số thành tiền âm thì chỉ có trường hợp giá trị
    * chiết khấu lúc này là Số tiền)
    */
    if (this.CustomerOrderAmountAfterDiscount < 0) {
      this.CustomerOrderAmountAfterDiscount = 0;
      this.discountValueControl.setValue(this.quoteModel.amount.toString());
    }
    this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
    if (this.CustomerOrderAmountAfterDiscount !== 0) {
      this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
    }
    else {
      this.ValuePriceProfit = 0;
    }
    /*End*/
  }

  createQuote(value: boolean) {
    if (!this.quoteForm.valid) {
      Object.keys(this.quoteForm.controls).forEach(key => {
        if (this.quoteForm.controls[key].valid == false) {
          this.quoteForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.quoteForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else if (this.listQuoteDetailModel.length == 0) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm dịch vụ được chọn' };
      this.showMessage(mgs);
    } else {
      let validFile = true; //this.validFile();

      if (validFile) {
        if (this.uploadedFiles.length > 0) {
          this.uploadFiles(this.uploadedFiles);
          this.converToArrayQuoteDocument(this.uploadedFiles);
        }

        /*Binding data for quoteModel*/
        let quoteDate = new Date();
        quoteDate = convertToUTCTime(quoteDate);
        this.quoteModel.quoteDate = quoteDate;

        let sendQuoteDate = (this.sendDateControl != null ? this.sendDateControl.value : null);
        if (sendQuoteDate) {
          sendQuoteDate = convertToUTCTime(sendQuoteDate);
          this.quoteModel.sendQuoteDate = sendQuoteDate;
        } else {
          this.quoteModel.sendQuoteDate = null;
        }

        let effectiveQuoteDate = parseFloat(this.effectiveDateControl.value.replace(/,/g, ''));
        this.quoteModel.effectiveQuoteDate = effectiveQuoteDate;

        let expirationDate = this.expirationDate;
        this.quoteModel.expirationDate = expirationDate;

        let description = this.descriptionControl.value != null ? this.descriptionControl.value.trim() : '';
        this.quoteModel.description = description;

        let note = this.noteControl.value != null ? this.noteControl.value.trim() : '';
        this.quoteModel.note = note;

        let objectType = 'CUSTOMER'; // this.selectedObjectType == 'cus' ? 'CUSTOMER' : 'LEAD';
        this.quoteModel.objectType = objectType;

        if (this.selectedObjectType == 'cus') {
          let objectType: Customer = this.objectControl.value;
          this.quoteModel.objectTypeId = objectType.customerId;
        } else {
          // let objectType: Lead = this.objectControl.value;
          // this.quoteModel.ObjectTypeId = objectType.leadId;
          let objectType: Customer = this.objectControl.value;
          this.quoteModel.objectTypeId = objectType.customerId;
        }

        this.quoteModel.customerContactId = null;

        let paymentMethod: Category = this.paymentMethodControl.value != null ? this.paymentMethodControl.value : null;
        if (paymentMethod) {
          this.quoteModel.paymentMethod = paymentMethod.categoryId;
        } else {
          this.quoteModel.paymentMethod = null;
        }

        let discountType: DiscountType = this.discountTypeControl.value;
        this.quoteModel.discountType = discountType.value;

        let bankAccount: BankAccount = this.bankAccountControl.value;
        if (bankAccount) {
          this.quoteModel.bankAccountId = bankAccount.bankAccountId;
        } else {
          this.quoteModel.bankAccountId = null;
        }

        let daysAreOwed = this.daysAreOwedControl.value != null ? parseFloat(this.daysAreOwedControl.value.replace(/,/g, '')) : 0;
        this.quoteModel.daysAreOwed = daysAreOwed;

        let maxDebt = this.maxDebtControl.value != null ? parseFloat(this.maxDebtControl.value.replace(/,/g, '')) : 0;
        this.quoteModel.maxDebt = maxDebt;

        this.quoteModel.receivedDate = convertToUTCTime(new Date());

        this.quoteModel.receivedHour = null;
        this.quoteModel.recipientName = '';
        this.quoteModel.locationOfShipment = '';
        this.quoteModel.shippingNote = '';
        this.quoteModel.recipientPhone = '';
        this.quoteModel.recipientEmail = '';
        this.quoteModel.placeOfDelivery = '';

        this.quoteModel.amount = this.quoteModel.amount; //Tổng tiền của báo giá (Chưa tính chiết khấu)

        let discountValue = this.discountValueControl.value != null ? this.discountValueControl.value : '0';
        discountValue = parseFloat(discountValue.replace(/,/g, ''));
        this.quoteModel.discountValue = discountValue;

        let intendedQuoteDate = this.intendedDateControl.value;
        intendedQuoteDate = convertToUTCTime(intendedQuoteDate);
        this.quoteModel.intendedQuoteDate = intendedQuoteDate;

        let statusQuote: Category = this.quoteStatusControl.value;
        this.quoteModel.statusId = statusQuote.categoryId;

        this.quoteModel.active = true;
        this.quoteModel.createdById = this.emptyGuid;
        this.quoteModel.createdDate = convertToUTCTime(new Date());
        this.quoteModel.updatedById = this.emptyGuid;
        this.quoteModel.updatedDate = convertToUTCTime(new Date());

        let personInCharge: Employee = this.personInChargeControl.value != null ? this.personInChargeControl.value : null;
        if (personInCharge) {
          this.quoteModel.personInChargeId = personInCharge.employeeId;
        } else {
          this.quoteModel.personInChargeId = null;
        }
        this.quoteModel.seller = this.quoteModel.personInChargeId;

        let leadCH: any = this.coHoiControl.value != null ? this.coHoiControl.value : null;
        if (leadCH) {
          this.quoteModel.leadId = leadCH.leadId;
        } else {
          this.quoteModel.leadId = null;
        }

        let saleBillding: any = this.hoSoThauControl.value != null ? this.hoSoThauControl.value : null;
        if (saleBillding) {
          this.quoteModel.saleBiddingId = saleBillding.saleBiddingId;
        } else {
          this.quoteModel.saleBiddingId = null;
        }

        let investmentFundId: any = this.kenhControl.value != null ? this.kenhControl.value : null;
        if (investmentFundId) {
          this.quoteModel.investmentFundId = investmentFundId.categoryId;
        } else {
          this.quoteModel.investmentFundId = null;
        }

        this.awaitResult = false;
        this.loading = true;
        // this.quoteService.CreateQuote(this.quoteModel, this.listQuoteDetailModel, 1,
        //   this.arrayQuoteDocumentModel, this.listAdditionalInformation,
        //   this.listQuoteCostDetailModel, false, this.emptyGuid, []).subscribe(response => {
        //     let result = <any>response;
        //     this.loading = false;
        //     if (result.statusCode == 200) {
        //       let messageCode = this.isCreateQuote ? "Tạo báo giá thành công" : "Lưu báo giá thành công";
        //       let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        //       this.showMessage(mgs);
        //       this.sendEmailAfterQuote(result);

        //       if (value) {
        //         //Lưu và thêm mới
        //         if (this.emitStatusChangeForm) {
        //           this.emitStatusChangeForm.unsubscribe();
        //           this.isInvalidForm = false; //Ẩn icon-warning-active
        //         }
        //         this.resetForm();
        //         this.router.navigate(['/customer/quote-create']);
        //         this.awaitResult = false;
        //       } else {
        //         //Lưu
        //         if (this.isCreateQuote) {
        //           this.router.navigate(['/customer/quote-detail', { quoteId: result.quoteID }]);
        //         } else {
        //           window.location.reload();
        //         }
        //       }
        //     } else {
        //       let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        //       this.showMessage(mgs);
        //     }
        //   });
      }
    }
  }

  sendEmailAfterQuote(result: any) {
    const moneyPipe = new NumberToStringPipe();
    let sendEmailModel: SendEmailModel = result.sendEmailModel;
    let sumAmountDiscount_temp = 0;
    let sumAmount_temp = 0;
    sendEmailModel.ListQuoteDetailToSendEmail = [];//reset
    this.listQuoteDetailModel.forEach(quoteDetail => {
      //tính tổng tiền chiết khấu
      // let newQuoteDetail = new QuoteDetailToSendEmail();
      // newQuoteDetail.ProductName = quoteDetail.ExplainStr;
      // newQuoteDetail.ProductNameUnit = quoteDetail.ProductNameUnit;
      // newQuoteDetail.UnitPrice = quoteDetail.UnitPrice.toLocaleString('en');
      // newQuoteDetail.Quantity = quoteDetail.Quantity.toLocaleString('en');
      // newQuoteDetail.Vat = quoteDetail.Vat.toLocaleString('en');
      // newQuoteDetail.DiscountValue = quoteDetail.DiscountValue.toLocaleString('en');
      // if (quoteDetail.DiscountType == true) {
      //   newQuoteDetail.DiscountValue += "%";
      // }
      // newQuoteDetail.SumAmount = quoteDetail.SumAmount.toLocaleString('en');
      // newQuoteDetail.AmountDiscountPerProduct = quoteDetail.AmountDiscount.toLocaleString('en');
      // sumAmountDiscount_temp += ParseStringToFloat(newQuoteDetail.AmountDiscountPerProduct);
      // sumAmount_temp += ParseStringToFloat(newQuoteDetail.SumAmount);
      // sendEmailModel.ListQuoteDetailToSendEmail.push(newQuoteDetail);
    });
    sendEmailModel.SumAmountDiscount = sumAmountDiscount_temp.toLocaleString('en') + " VND"; //tổng tiền theo sản phẩm
    sendEmailModel.SumAmount = sumAmount_temp.toLocaleString('en') + " VND"; //tổng tiền theo sản phẩm
    sendEmailModel.AmountDiscountByQuote = this.discountValueControl.value;
    let discountType = this.discountTypeControl.value;
    let codeDiscountType = discountType.code;
    if (codeDiscountType == "PT") {
      sendEmailModel.AmountDiscountByQuote += "%";
    }
    sendEmailModel.SumAmountByQuote = this.CustomerOrderAmountAfterDiscount.toLocaleString('en');
    sendEmailModel.SumAmountTransform = moneyPipe.transform(this.CustomerOrderAmountAfterDiscount, this.defaultNumberType);
    sendEmailModel.SendDetailProduct = true;
    if (this.isCreateQuote == true) {
      //gui email sau khi tao bao gia
      this.emailConfigService.sendEmail(2, sendEmailModel).subscribe(reponse => {
        //let result = <any>response;
      });
    } else {
      //gui email sau khi sua bao gia
      this.emailConfigService.sendEmail(3, sendEmailModel).subscribe(reponse => {
        //let result = <any>response;
      });
    }
  }

  /*Khi tạo hoặc sửa báo giá phải có ít nhất một file đính kèm người dùng thêm vào hoặc tồn tại trên server*/
  validFile(): boolean {
    let result = true;
    if (this.isCreateQuote) {
      if (this.uploadedFiles.length == 0) {
        result = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một File tài liệu đính kèm' };
        this.showMessage(mgs);
      } else {
        result = true;
      }
    } else {
      if (this.uploadedFiles.length == 0 && this.arrayQuoteDocumentModel.length == 0) {
        result = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một File tài liệu đính kèm' };
        this.showMessage(mgs);
      } else {
        result = true;
      }
    }
    return result;
  }

  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  cancel() {
    this.router.navigate(['/customer/quote-list']);
  }

  createOrder() {
    this.router.navigate(['/order/create', { quoteID: this.quoteModel.quoteId }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  restartCustomerOrderDetailModel() {
    this.quoteDetailModel = new QuoteDetail();
  }

  converToArrayQuoteDocument(fileList: File[]) {
    for (var x = 0; x < fileList.length; ++x) {
      // this.quoteDocumentModel.DocumentName = fileList[x].name;
      // this.quoteDocumentModel.DocumentSize = fileList[x].size + '';
      // this.quoteDocumentModel.CreatedDate = convertToUTCTime(new Date());
      // this.quoteDocumentModel.UpdatedDate = convertToUTCTime(new Date());
      // this.arrayQuoteDocumentModel.push(this.quoteDocumentModel);
      // this.restartQuoteDocumentModel();
    }
  }

  restartQuoteDocumentModel() {
    var item: QuoteDocument = new QuoteDocument();
  }

  resetForm() {
    this.selectedObjectType = 'cus';  //Loại khách hàng (Khách hàng tiềm năng hoặc Khách hàng)
    this.objectList = this.listCustomer;
    this.objectControl.reset();
    this.phone = '';
    this.email = '';
    this.fullAddress = '';
    this.paymentMethodControl.setValue(this.listPaymentMethod.find(x => x.isDefault == true));
    this.daysAreOwedControl.setValue('0');
    this.maxDebtControl.setValue('0');
    this.listBankAccount = [];
    this.bankAccountControl.setValue(null);
    this.isShowBankAccount = false;
    this.currentEmployeeId = this.auth.EmployeeId;
    this.quoteStatusControl.setValue(this.listQuoteStatus.find(x => x.isDefault == true));
    this.resetWorkFollowQuote();
    this.intendedDateControl.setValue(new Date());
    this.sendDateControl.setValue(null);
    this.sendDateControl.setValidators(null);
    this.sendDateControl.updateValueAndValidity();
    this.effectiveDateControl.setValue('30');
    this.expirationDate = null;
    this.personInChargeControl.reset();
    this.hoSoThauControl.reset();
    this.coHoiControl.reset();
    this.kenhControl.reset();
    this.nameQuoteControl.reset();
    this.isPersonInCharge = false;
    this.descriptionControl.setValue('');
    this.noteControl.setValue('');
    this.uploadedFiles = [];
    this.listQuoteCostDetailModel = [];
    if (this.fileUpload) {
      this.fileUpload.clear();  //Xóa toàn bộ file trong control
    }
    this.titleText = '';
    this.contentText = '';
    this.isUpdateAI = false;
    this.listAdditionalInformation = [];
    this.arrayQuoteDocumentModel = [];
    this.listQuoteDetailModel = [];
    this.selectedColumns = this.cols;
    this.resetQuoteModel();
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl.setValue('0');
    this.CustomerOrderAmountAfterDiscount = 0;
    this.AmountPriceInitial = 0;
    this.AmountPriceCost = 0;
    this.AmountPriceProfit = 0;
    this.ValuePriceProfit = 0;
  }

  resetQuoteModel() {
    this.quoteModel = new Quote();
  }

  resetWorkFollowQuote() {
    this.workFollowQuote = [
      {
        label: this.listQuoteStatus.find(x => x.categoryCode == 'MTA').categoryName
      },
      {
        label: this.listQuoteStatus.find(x => x.categoryCode == 'DLY').categoryName
      },
      {
        label: this.listQuoteStatus.find(x => x.categoryCode == 'CHO').categoryName
      },
      {
        label: this.listQuoteStatus.find(x => x.categoryCode == 'DTH').categoryName
      }
    ];
    this.activeIndex = 0;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }
  showDialogImport() {
    this.displayDialog = true;

  }
  downloadTemplateExcel() {
    this.quoteService.downloadTemplateProduct().subscribe(response => {
      this.loading = false;
      const result = <any>response;
      if (result.templateExcel != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.templateExcel);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.fileName + ".xls";
        //const fileName = result.nameFile  + ".xlsx";
        link.download = fileName;
        link.click();
      }
    }, error => { this.loading = false; });
  }
  chooseFile(event: any) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;

    this.getInforDetailQuote();
  }

  cancelFile() {
    $("#importFileProduct").val("")
    this.fileName = "";
  }

  async getInforDetailQuote() {
    let result: any = await this.productService.getMasterdataCreateProduct();
    if (result.statusCode === 200) {
      this.productCodeSystemList = result.listProductCode;
      this.listProductUnitName = result.listProductUnitName;
    };

    this.quoteService.getDataQuoteAddEditProductDialog().subscribe(response => {
      let resultListProduct: any = response;
      if (resultListProduct.statusCode == 200) {
        this.listProduct = resultListProduct.listProduct;
        this.listUnitMoney = resultListProduct.listUnitMoney;
        this.listUnitProduct = resultListProduct.listUnitProduct;
      }
    });
  }

  validateFile(data) {
    this.messErrFile = [];
    this.cellErrFile = [];

    data.forEach((row, i) => {
      if (i > 4) {
        if ((row[1] === null || row[1] === undefined || row[1].toString().trim() == "") && (row[2] === null || row[2] === undefined || row[2].toString().trim() == "")) {
          this.messErrFile.push('Dòng { ' + (i + 2) + ' } chưa nhập Mã sp hoặc Tên sản phẩm!');
        }
        if (row[3] === null || row[3] === undefined || row[3] == "") {
          this.messErrFile.push('Cột số lượng tại dòng ' + (i + 2) + ' không được để trống');
        }
        else {
          let isPattermSL = /^\d+$/.test(row[3].toString().trim());

          // if(parseFloat(row[3]) == undefined || parseFloat(row[3]).toString() == "NaN" || parseFloat(row[3]) == null){
          if (!isPattermSL) {
            this.messErrFile.push('Cột số lượng tại dòng ' + (i + 2) + ' sai định dạng');
          }
        }
        if (row[4] === null || row[4] === undefined || row[4] == "") {
          this.messErrFile.push('Cột đơn giá tại dòng ' + (i + 2) + ' không được để trống');
        }
        else {
          let isPattermDG = /^\d+$/.test(row[4].toString().trim());

          // if(parseFloat(row[4]) == undefined || parseFloat(row[4]).toString() == "NaN" || parseFloat(row[4]) == null){
          if (!isPattermDG) {
            this.messErrFile.push('Cột đơn giá tại dòng ' + (i + 2) + ' sai định dạng');
          }
        }
      }
    });
    if (this.messErrFile.length != 0) return true;
    else return false;
  }

  importExcel() {
    if (this.fileName == "") {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Chưa chọn file cần nhập" };
      this.showMessage(mgs);
    }
    else {
      const targetFiles: DataTransfer = <DataTransfer>(this.importFileExcel);
      const reader: FileReader = new FileReader();
      reader.readAsBinaryString(targetFiles.files[0]);

      reader.onload = (e: any) => {
        /* read workbook */
        const bstr: string = e.target.result;

        const workbook: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

        // kiểm tra form value và file excel có khớp mã với nhau hay không
        let customerCode = 'BOM Lines';
        if (workbook.Sheets[customerCode] === undefined) {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
          this.showMessage(mgs);
          return;
        }

        //lấy data từ file excel của khách hàng doanh nghiệp
        const worksheetCompanyCustomer: XLSX.WorkSheet = workbook.Sheets[customerCode];
        /* save data */
        let dataCompanyCustomer: Array<any> = XLSX.utils.sheet_to_json(worksheetCompanyCustomer, { header: 1 });
        //remove header row
        dataCompanyCustomer.shift();
        let productCodeList: string[] = [];
        let productUnitList: string[] = [];

        let isValidation = this.validateFile(dataCompanyCustomer);
        if (isValidation) {
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
          // this.messErrFile.forEach(element => {
          //   let mgs = { severity:'error', summary: 'Thông báo:', detail: element };
          //   this.showMessage(mgs);
          // });
        }
        else {
          var messCodeErr = [];
          var messUnitErr = [];
          this.isInvalidForm = false;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = false;  //Hiển thị message lỗi
          dataCompanyCustomer.forEach((row, i) => {
            // Lấy giá trị bản ghi trong excel bắt đầu từ line 6
            if (i > 4 && row.length != 0) {
              if (row[1] !== null && row[1] !== undefined && row[1].trim() != "") {
                let rowObj = productCodeList.filter(p => p.trim().toUpperCase() == row[1].trim().toUpperCase());
                if (rowObj.length === 0) {
                  productCodeList.push(row[1].toLowerCase());
                }
                // productCodeList.push(row[1])
                let check = this.productCodeSystemList.find(d => d.toLowerCase().trim() == row[1].trim().toLowerCase());
                if (check === undefined || check === null) {
                  messCodeErr.push(i + 2);
                }
              };
              if (row[5] !== null && row[5] !== undefined && row[5].trim() != "" &&
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "")) {
                // let isProduct = productUnitList.find(i=>i.trim() == row[5].trim());
                let rowObj = productUnitList.filter(p => p.trim().toUpperCase() == row[5].trim().toUpperCase());
                if (rowObj.length === 0) {
                  productUnitList.push(row[5].toLowerCase());
                  let check = this.listProductUnitName.find(d => d.toLowerCase().trim() == row[5].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messUnitErr.push(i + 2);
                  }
                }
              };
            }
          });

          let countCode = this.productCodeSystemList.filter(c => productCodeList.includes(c.toLowerCase()));
          let countUnit = this.listProductUnitName.filter(u => productUnitList.includes(u.toLowerCase()));

          if (countCode.length == productCodeList.length && countUnit.length == productUnitList.length) {
            this.listQuoteDetailExcelModel = [];
            dataCompanyCustomer.forEach((row, i) => {
              // Lấy giá trị bản ghi trong excel bắt đầu từ line 6
              if (i > 4 && row.length != 0 && (
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "") || (row[2] !== null && row[2] !== undefined && row[2].trim() != "") || (row[5] !== null && row[5] !== undefined && row[5].trim() != "")
              )
              ) {
                let newCustomer: QuoteDetailExcel = {
                  STT: row[0],
                  ProductCode: row[1],
                  ProductName: row[2],
                  Quantity: (row[3] === null || row[3] === undefined || row[3] == "") ? 0 : row[3],
                  UnitPrice: (row[4] === null || row[4] === undefined || row[4] == "") ? 0 : row[4],
                  CurrencyUnit: row[5],
                  Amount: (row[6] === null || row[6] === undefined || row[6] == "") ? 0 : row[6],
                  Tax: (row[7] === null || row[7] === undefined || row[7] == "") ? 0 : row[7],
                  TaxAmount: (row[8] === null || row[8] === undefined || row[8] == "") ? 0 : row[8],
                  DiscountType: row[9] == "%" ? true : false,
                  DiscountValue: (row[10] === null || row[10] === undefined || row[10] == "" || row[9] === null || row[9] === undefined || row[9].trim() == "") ? 0 : row[10],
                  TotalAmount: (row[11] === null || row[11] === undefined || row[11] == "") ? 0 : row[11]
                }
                this.listQuoteDetailExcelModel.push(newCustomer);
              }
            });
            // lấy tiền VND
            var moneyUnit = this.listUnitMoney.find(c => c.categoryCode == "VND");

            this.listQuoteDetailExcelModel.forEach(item => {
              let detailProduct = new QuoteDetail();
              if (item.ProductCode == null || item.ProductCode.trim() == "" || item.ProductCode == undefined) {
                // detailProduct.OrderDetailType = 1;
                // detailProduct.Active = true;
                // detailProduct.CurrencyUnit = moneyUnit.categoryId;
                // detailProduct.DiscountType = item.DiscountType;
                // detailProduct.ExchangeRate = 1;
                // detailProduct.IncurredUnit = item.CurrencyUnit;
                // detailProduct.NameMoneyUnit = moneyUnit.categoryCode;
                // detailProduct.Description = item.ProductName;
                // detailProduct.ProductName = item.ProductName;
                // detailProduct.Quantity = item.Quantity;
                // detailProduct.UnitPrice = item.UnitPrice;
                // detailProduct.Vat = item.Tax;
                // detailProduct.DiscountValue = item.DiscountValue;
                // detailProduct.SumAmount = item.TotalAmount;

                // this.listQuoteDetailModel = [...this.listQuoteDetailModel, detailProduct];
              }
              else {
                // detailProduct.OrderDetailType = 0;
                // detailProduct.Active = true;
                // detailProduct.CurrencyUnit = moneyUnit.categoryId;
                // detailProduct.DiscountType = item.DiscountType;
                // detailProduct.ExchangeRate = 1;
                // detailProduct.IncurredUnit = "CCCCC";
                // detailProduct.NameMoneyUnit = moneyUnit.categoryCode;
                // detailProduct.Description = "";

                // detailProduct.Quantity = item.Quantity;
                // detailProduct.UnitPrice = item.UnitPrice;
                // detailProduct.Vat = item.Tax;
                // detailProduct.DiscountValue = item.DiscountValue;
                // detailProduct.SumAmount = item.TotalAmount;

                // let product = this.listProduct.find(p=>p.productCode.trim() == item.ProductCode.trim());
                // detailProduct.ProductId = product.productId;
                // detailProduct.ExplainStr = product.productCode;
                // detailProduct.ProductName = product.productName;

                // //Lấy đơn vị tính
                // let productUnitId = product.productUnitId;
                // let productUnitName = this.listUnitProduct.find(x => x.categoryId == productUnitId).categoryName;
                // detailProduct.ProductNameUnit = productUnitName;
                // detailProduct.UnitId = productUnitId;

                // //Lấy list nhà cung cấp
                // let listVendor = this.listProductVendorMapping.filter(x => x.productId == product.productId);

                // //Nếu listVendor chỉ có 1 giá trị duy nhất thì lấy luôn giá trị đó làm default value
                // if (listVendor.length == 1) {
                //   let toSelectVendor = listVendor[0];
                //   detailProduct.VendorId = toSelectVendor.vendorId;
                //   detailProduct.NameVendor = toSelectVendor.vendorCode + " - " + toSelectVendor.vendorName;
                // }

                // this.listQuoteDetailModel = [...this.listQuoteDetailModel, detailProduct];

                // this.quoteModel.amount = 0;
                // this.listQuoteDetailModel.forEach(itemDetail => {
                //   this.quoteModel.amount = this.quoteModel.amount + itemDetail.SumAmount;
                // })
                // this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
                // if(this.CustomerOrderAmountAfterDiscount !== 0){
                //   this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
                // }
                // else{
                //   this.ValuePriceProfit = 0;
                // }

                // let discountType: DiscountType = this.discountTypeControl.value;
                // let valueDis = discountType.value;
                // let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));

                // //Số thành tiền
                // if (valueDis) {
                //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
                // } else {
                //   this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
                // }

                // this.cancelFile();
              }
            });

            this.quoteModel.amount = 0;
            this.AmountPriceInitial = 0;
            // this.listQuoteDetailModel.forEach(itemDetail => {
            //   this.quoteModel.amount = this.quoteModel.amount + itemDetail.SumAmount;
            //   let price = (itemDetail.PriceInitial === null || itemDetail.PriceInitial === undefined) ? 0 : parseFloat(itemDetail.PriceInitial.toString().replace(/,/g, ''));
            //   let quantity = (itemDetail.Quantity === null || itemDetail.Quantity === undefined) ? 0 : parseFloat(itemDetail.Quantity.toString().replace(/,/g, ''));
            //   let exchange = (itemDetail.ExchangeRate === null || itemDetail.ExchangeRate === undefined) ? 0 : parseFloat(itemDetail.ExchangeRate.toString().replace(/,/g, ''));
            //   this.AmountPriceInitial = this.AmountPriceInitial + (price * quantity * exchange);
            // })
            this.AmountPriceProfit = this.CustomerOrderAmountAfterDiscount - this.AmountPriceInitial;
            if (this.CustomerOrderAmountAfterDiscount !== 0) {
              this.ValuePriceProfit = parseFloat((this.AmountPriceProfit / this.CustomerOrderAmountAfterDiscount * 100).toFixed(this.defaultNumberType));
            }
            else {
              this.ValuePriceProfit = 0;
            }

            let discountType: DiscountType = this.discountTypeControl.value;
            let valueDis = discountType.value;
            let discountValue = parseFloat(this.discountValueControl.value.replace(/,/g, ''));
            //Số thành tiền
            if (valueDis) {
              this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - ((this.quoteModel.amount + this.AmountPriceCost) * discountValue) / 100;
            } else {
              this.CustomerOrderAmountAfterDiscount = this.quoteModel.amount + this.AmountPriceCost - discountValue;
            }
            this.cancelFile();
            this.isInvalidForm = false;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = false;  //Hiển thị message lỗi
          }
          if (countCode.length != productCodeList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messCodeErr.forEach(item => {
              this.messErrFile.push('Mã sản phẩm tại dòng ' + item + ' không tồn tại trong hệ thống')
            })
            // let mgs = { severity:'error', summary: 'Thông báo:', detail: "Mã sản phẩm không có tồn tại trong hệ thống" };
            // this.showMessage(mgs);
          }
          if (countUnit.length != productUnitList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messUnitErr.forEach(item => {
              this.messErrFile.push('Đơn vị tính tại dòng ' + item + ' không tồn tại trong hệ thống')
            })
            // let mgs = { severity:'error', summary: 'Thông báo:', detail: "Đơn vị tính không có tồn tại trong hệ thống" };
            // this.showMessage(mgs);
          }
        }
        this.displayDialog = false;
      }
    }
  }

  exportExcel() {
    let dateUTC = new Date();
    // getMonth() trả về index trong mảng nên cần cộng thêm 1
    let title = "Danh sách sản phẩm dịch vụ " + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('BOM Lines');
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    let dataRow1 = [];
    dataRow1[1] = `    `
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Arial', size: 18, bold: true };
    row1.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    let dataRow2 = [];
    dataRow2[1] = `    `
    dataRow2[5] = `Danh sách BOM hàng hóa
    (BOM Line)`
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Arial', size: 18, bold: true };
    worksheet.mergeCells(`E${row2.number}:H${row2.number}`);
    row2.alignment = { vertical: 'bottom', horizontal: 'center', wrapText: true };

    worksheet.addRow([]);

    let dataRow4 = [];
    dataRow4[2] = `- Các cột màu đỏ là các cột bắt buộc nhập
    - Các cột có ký hiệu (*) là các cột bắt buộc nhập theo điều kiện`
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Arial', size: 11, color: { argb: 'ff0000' } };
    row4.alignment = { vertical: 'bottom', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);

    /* Header row */
    let dataHeaderRow = ['STT', 'Mã sản phẩm', 'Tên sản phẩm/Mô tả', 'Số lượng', 'Đơn giá', 'Đơn vị tính', 'Thành tiền (VND)', `Thuế suất`, `Tiền thuế`, 'Loại chiết khấu', 'Chiết khấu', 'Tổng tiền'];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Arial', size: 10, bold: true };
    dataHeaderRow.forEach((item, index) => {
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      if (index + 1 == 4 || index + 1 == 5) {
        headerRow.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'ff0000' }
        };
      }
      else {
        headerRow.getCell(index + 1).fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: '8DB4E2' }
        };
      }
    });
    headerRow.height = 40;

    this.listQuoteDetailModel.forEach((item, index) => {
      // let productCode = "";
      // let productName = "";
      // let productQuantity = item.Quantity;
      // let productPriceAmount = item.UnitPrice;
      // let productUnit = item.ProductNameUnit;
      // let productAmount = productQuantity * productPriceAmount;
      // let productVat = item.Vat;
      // let productAmountVat = (productAmount * productVat)/100;
      // let productDiscountType = item.DiscountType ? "%" : "Tiền";
      // let productDiscountValue = item.DiscountValue;
      // let productSumAmount = item.SumAmount;
      // if(item.ProductId !== null){
      //   productCode = this.listProduct.find(p=>p.productId == item.ProductId).productCode;
      //   productName = item.ExplainStr;
      // }
      // else {
      //   productName = item.Description;
      //   productUnit = item.IncurredUnit
      // }

      /* Header row */
      // let dataHeaderRowIndex = [index + 1, productCode, productName, productQuantity, productPriceAmount, productUnit, productAmount, productVat, productAmountVat, productDiscountType, productDiscountValue, productSumAmount];
      // let headerRowIndex = worksheet.addRow(dataHeaderRowIndex);
      // headerRowIndex.font = { name: 'Arial', size: 10 };
      // dataHeaderRowIndex.forEach((item, index) => {
      //   // headerRowIndex.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      //   headerRowIndex.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      //   if(index == 1 || index == 2 || index == 5 || index == 9){
      //     headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'left' };
      //   }
      //   if(index == 3 || index == 4 || index == 6 || index == 7 || index == 8 || index == 10 || index == 11){
      //     headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'right' };
      //   }
      // });
      // headerRowIndex.height = 40;
    })

    worksheet.addRow([]);
    worksheet.getRow(2).height = 47;
    worksheet.getRow(4).height = 70;
    worksheet.getColumn(1).width = 5;
    worksheet.getColumn(2).width = 25;
    worksheet.getColumn(3).width = 25;
    worksheet.getColumn(4).width = 25;
    worksheet.getColumn(5).width = 25;
    worksheet.getColumn(6).width = 25;
    worksheet.getColumn(7).width = 25;
    worksheet.getColumn(8).width = 25;
    worksheet.getColumn(9).width = 25;
    worksheet.getColumn(10).width = 25;
    worksheet.getColumn(11).width = 25;
    worksheet.getColumn(12).width = 25;

    worksheet.getColumn(5).numFmt = '#,##0.00';
    worksheet.getColumn(7).numFmt = '#,##0.00';
    worksheet.getColumn(9).numFmt = '#,##0.00';
    worksheet.getColumn(11).numFmt = '#,##0.00';
    worksheet.getColumn(12).numFmt = '#,##0.00';

    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
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
