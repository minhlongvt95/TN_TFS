import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Renderer2, HostListener, NgZone } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';

//MODELS
import { Quote } from '../../models/quote.model';
import { QuoteDetail } from "../../models/quote-detail.model";
import { QuoteDocument } from "../../models/quote-document.model";
import { QuoteProductDetailProductAttributeValue } from "../../models/quote-product-detail-product-attribute-value.model";

//SERVICES
import { BankService } from '../../../shared/services/bank.service';
import { QuoteService } from '../../services/quote.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { ProductService } from '../../../product/services/product.service';
import { PromotionService } from '../../../promotion/services/promotion.service';

//DIALOG COMPONENT
import { AddEditProductDialogComponent } from '../add-edit-product-dialog/add-edit-product-dialog.component';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';
import { saveAs } from "file-saver";
import { Workbook } from 'exceljs';
import { DecimalPipe } from '@angular/common';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { QuoteCostDetail } from '../../models/quote-cost-detail.model';
import { PromotionApplyPopupComponent } from '../../../shared/components/promotion-apply-popup/promotion-apply-popup.component';
import { PromotionObjectApply } from '../../../promotion/models/promotion-object-apply.model';
import { PromotionApply } from '../../../promotion/models/promotion-apply.model';
import { PromotionObjectApplyMapping } from '../../../promotion/models/promotion-object-apply-mapping.model';

interface Customer {
  customerId: string;
  sheetName: string;
  customerName: string;
  sheetNameName: string;
  customerEmail: string;
  customerPhone: string;
  fullAddress: string;
  customerGroupId: string;
  maximumDebtDays: number;
  maximumDebtValue: number;
  personInChargeId: string;
  customerCodeName: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
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
  labelShow: string; //Text hiển thị trên giao diện
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
  selector: 'app-customer-quote-create',
  templateUrl: './customer-quote-create.component.html',
  styleUrls: ['./customer-quote-create.component.css'],
  providers: [
    DecimalPipe
  ]
})
export class CustomerQuoteCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới

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
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  /*End*/

  /*Get Current EmployeeId*/
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  /*End*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionImport: boolean = true;
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
  participantControl: FormControl; //Người tham gia
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
  customerId: string = null;
  occasionId: string = null;
  saleBiddingId: string = null;
  createObjectId: string = null;
  createObjectType: string = null;
  selectedObjectType: string = 'cus';
  isShowBankAccount: boolean = false;
  customerContactCode: string = 'CUS';
  uploadedFiles: any[] = [];
  cols: any[];
  colsCost: any[];
  selectedColumns: any[];
  selectedColumnsCost: any[];
  selectedItem: any;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  colsFile: any[];
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
  objectList = <any>[];
  objCustomer: Customer = null;

  productCodeSystemList: string[] = [];
  listProductUnitName: string[] = [];
  listProduct: any[] = [];
  listUnitProduct: any[] = [];
  listUnitMoney: Array<any> = [];
  minDate: Date = new Date();

  /* Mới */
  listInvestFund: Array<Category> = []; //list kênh bán hàng
  listAdditionalInformationTemplates: Array<Category> = []; //list thông tin bổ sung mẫu của báo giá
  listPaymentMethod: Array<Category> = []; //list Phương thức thanh toán
  listQuoteStatus: Array<Category> = []; //list Trạng thái của báo giá
  listEmployee: Array<Employee> = [];  //list nhân viên bán hàng phân quyền dữ liệu theo người đang đăng nhập
  listEmpSale: Array<Employee> = []; //list nhân viên bán hàng (có thể thay đổi)
  listCustomer: Array<Customer> = [];  //list khách hàng định danh
  listCustomerNew: Array<Customer> = []; //list Khách hàng tiềm năng
  listAllLead: Array<any> = []; //list Hồ sơ thầu phân quyền dữ liệu theo người đang đăng nhập
  listLead: Array<any> = []; //list cơ hội (có thể thay đổi)
  listAllSaleBidding: Array<any> = []; //list Hồ sơ thầu phân quyền dữ liệu theo người đang đăng nhập
  listSaleBidding: Array<any> = []; //list hồ sơ thầu (có thể thay đổi)
  listParticipant: Array<Employee> = []; //list người tham gia
  /* End */

  tooltipSelectedParticipant: string = null; //tooltip cho dropdown list người tham gia

  colLeft: number = 8;
  isShow: boolean = true;
  email: string = '';
  phone: string = '';
  fileName: string = '';
  fullAddress: string = '';
  listBankAccount: Array<BankAccount> = [];
  quoteDate: Date = new Date();
  expirationDate: Date = null; //Ngày hết hiệu lực của báo giá
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  arrayQuoteDocumentModel: Array<QuoteDocument> = [];
  listQuoteDetailModel: Array<QuoteDetail> = [];
  listQuoteCostDetailModel: Array<QuoteCostDetail> = [];
  listQuoteDetailExcelModel: Array<QuoteDetailExcel> = [];

  amount: number = 0; //Tổng giá trị hàng hóa bán ra
  amountPriceInitial: number = 0; //Tổng giá trị hàng hóa đầu vào
  amountPriceCost: number = 0; //Tổng chi phí
  totalAmountVat: number = 0; //Tổng thuế VAT
  totalAmountAfterVat: number = 0; //Tổng tiền sau thuế
  totalAmountPromotion: number = 0; //Tổng tiền khuyến mãi
  totalAmountDiscount: number = 0; //Tổng thành tiền chiết khấu
  customerOrderAmountAfterDiscount: number = 0; //Tổng thanh toán
  amountPriceProfit: number = 0; //Lợi nhuận tạm tính
  valuePriceProfit: number = 0; //% lợi nhuận tạm tính

  /*Chương trình khuyến mãi*/
  isPromotionCustomer: boolean = false;
  isPromotionAmount: boolean = false;
  colsPromotion: any[];
  listPromotionApply: Array<PromotionObjectApply> = [];
  listPromotionApplyProduct: Array<PromotionObjectApply> = [];
  /*End*/

  constructor(
    private router: Router,
    private el: ElementRef,
    private route: ActivatedRoute,
    private productService: ProductService,
    private bankService: BankService,
    private imageService: ImageUploadService,
    private quoteService: QuoteService,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private promotionService: PromotionService
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngOnInit() {
    this.setForm();

    /*Tạo báo giá*/
    this.route.params.subscribe(params => {
      /*Từ Cơ hội*/
      this.occasionId = params['occasionId'];
      /*End*/

      /*Từ Khách hàng (Khách hàng định danh hoặc Khách hàng tiềm năng*/
      this.customerId = params['customerId'];
      /*End*/

      /*Từ Hồ sơ thầu*/
      this.saleBiddingId = params['saleBiddingId'];
      /*End*/

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
        }
        if (listCurrentActionResource.indexOf("import") == -1) {
          this.actionImport = false; //import file upload
        }
      }
    });
    /*End*/
    this.setTable();
    this.getDataDefault();
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
    this.participantControl = new FormControl(null);

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
      discountValueControl: this.discountValueControl,
      participantControl: this.participantControl
    });
  }

  setTable() {
    this.cols = [
      { field: 'move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'gift', header: 'Quà KM', width: '170px', textAlign: 'center', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'nameVendor', header: 'Nhà cung cấp', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'productNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'vat', header: 'Thuế GTGT (%)', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'discountValue', header: 'Chiết khấu', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = [
      { field: 'move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'gift', header: 'Quà KM', width: '170px', textAlign: 'center', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm/dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm dịch vụ', width: '170px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'productNameUnit', header: 'Đơn vị tính', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '170px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '130px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];

    this.colsFile = [
      { field: 'DocumentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'DocumentSize', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];

    this.colsNote = [
      { field: 'title', header: 'Tiêu đề', width: '30%', textAlign: 'left' },
      { field: 'content', header: 'Nội dung', width: '70%', textAlign: 'left' },
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

    this.colsPromotion = [
      { field: 'promotionName', header: 'CTKM', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'promotionProductName', header: 'Tên sản phẩm dịch vụ', width: '160px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'productUnitName', header: 'Đơn vị tính', width: '110px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'Số lượng', width: '100px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'giaTri', header: 'Giá trị', width: '140px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'loaiGiaTri', header: 'Loại giá trị', width: '100px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền', width: '100px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];
  }

  getDataDefault() {
    if (this.occasionId) {
      this.createObjectType = "LEAD";
      this.createObjectId = this.occasionId;
    }
    else if (this.saleBiddingId) {
      this.createObjectType = "SALEBIDDING";
      this.createObjectId = this.saleBiddingId;
    }
    else if (this.customerId) {
      this.createObjectType = "CUSTOMER";
      this.createObjectId = this.customerId;
    }

    this.loading = true;
    this.quoteService.getMasterDataCreateQuote(this.createObjectId, this.createObjectType).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listInvestFund = result.listInvestFund;  //list kênh bán hàng
        this.listAdditionalInformationTemplates = result.listAdditionalInformationTemplates;  //list thông tin bổ sung mẫu của báo giá
        this.listPaymentMethod = result.listPaymentMethod; //list Phương thức thanh toán
        this.listQuoteStatus = result.listQuoteStatus; //list Trạng thái của báo giá
        this.listEmployee = result.listEmployee;  //list nhân viên bán hàng
        this.listCustomer = result.listCustomer;  //list khách hàng định danh
        this.listCustomerNew = result.listCustomerNew; //list Khách hàng tiềm năng
        this.listAllLead = result.listAllLead; //list Hồ sơ thầu
        this.listAllSaleBidding = result.listAllSaleBidding; //list Hồ sơ thầu
        this.listParticipant = result.listParticipant; //list người tham gia

        this.setDefaultValueForm();
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  goDetailLead() {
    if (this.coHoiControl.value) {
      let lead = this.coHoiControl.value;
      let url = this.router.serializeUrl(this.router.createUrlTree(['/lead/detail', { leadId: lead.leadId }]));
      window.open(url, '_blank');
    }
  }

  goDetaiSaleding() {
    if (this.hoSoThauControl.value) {
      let salebidding = this.hoSoThauControl.value;
      let url = this.router.serializeUrl(this.router.createUrlTree(['/sale-bidding/detail', { saleBiddingId: salebidding.saleBiddingId }]));
      window.open(url, '_blank');
    }
  }

  setDefaultValueForm() {
    //Trạng thái báo giá
    const toSelectQuoteStatus: Category = this.listQuoteStatus.find(x => x.categoryCode == "MTA");
    this.quoteStatusControl.setValue(toSelectQuoteStatus);

    //Phương thức thanh toán
    const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
    this.quoteForm.controls['paymentMethodControl'].setValue(toSelectPaymentMethod);

    if (toSelectPaymentMethod?.categoryCode == "CASH") {
      this.isShowBankAccount = false;
    }
    else if (toSelectPaymentMethod?.categoryCode == "BANK") {
      this.isShowBankAccount = true;
    }
    else {
      this.isShowBankAccount = false;
    }

    //Thông tin bổ sung
    this.listAdditionalInformationTemplates = this.listAdditionalInformationTemplates;
    this.listAdditionalInformationTemplates.forEach(item => {
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


    /*Nếu báo giá được tạo từ một nguồn nào đó*/
    if (this.createObjectId) {
      //Từ Cơ hội
      if (this.createObjectType == "LEAD") {
        let lead = this.listAllLead.find(x => x.leadId == this.occasionId);

        //Kiểm tra xem khách hàng của Cơ hội là Định danh hay Tiềm năng
        let customerType = this.getCustomerType(lead.customerId);

        //Là khách hàng Định danh
        if (customerType == 1) {
          this.objectList = this.listCustomer;
          let customerIdentity = this.listCustomer.find(x => x.customerId == lead.customerId);

          this.mapDataToQuote(customerIdentity);
        }
        //Là khách hàng Tiềm năng
        else if (customerType == 0) {
          let customerNew = this.listCustomerNew.find(x => x.customerId == lead.customerId);
          this.selectedObjectType = 'lea';
          this.objectList = this.listCustomerNew;

          this.mapDataToQuote(customerNew);
        }
        //khách hàng không xác định
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
          this.showMessage(mgs);
        }
      }
      //Từ Hồ sơ thầu
      else if (this.createObjectType == "SALEBIDDING") {
        let saleBidding = this.listAllSaleBidding.find(x => x.saleBiddingId == this.saleBiddingId);

        //Kiểm tra xem khách hàng của Cơ hội là Định danh hay Tiềm năng
        let customerType = this.getCustomerType(saleBidding.customerId);

        //Là khách hàng Định danh
        if (customerType == 1) {
          this.objectList = this.listCustomer;
          let customerIdentity = this.listCustomer.find(x => x.customerId == saleBidding.customerId);

          this.mapDataToQuote(customerIdentity);
        }
        //Là khách hàng Tiềm năng
        else if (customerType == 0) {
          let customerNew = this.listCustomerNew.find(x => x.customerId == saleBidding.customerId);
          this.selectedObjectType = 'lea';
          this.objectList = this.listCustomerNew;

          this.mapDataToQuote(customerNew);
        }
        //khách hàng không xác định
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
          this.showMessage(mgs);
        }
      }
      //Từ khách hàng (Định danh hoặc Tiềm năng)
      else if (this.createObjectType == "CUSTOMER") {
        //Kiểm tra xem khách hàng của Cơ hội là Định danh hay Tiềm năng
        let customerType = this.getCustomerType(this.customerId);

        //Là khách hàng Định danh
        if (customerType == 1) {
          this.objectList = this.listCustomer;
          let customerIdentity = this.listCustomer.find(x => x.customerId == this.customerId);

          this.mapDataToQuote(customerIdentity);
        }
        //Là khách hàng Tiềm năng
        else if (customerType == 0) {
          let customerNew = this.listCustomerNew.find(x => x.customerId == this.customerId);
          this.selectedObjectType = 'lea';
          this.objectList = this.listCustomerNew;

          this.mapDataToQuote(customerNew);
        }
        //khách hàng không xác định
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
          this.showMessage(mgs);
        }
      }
    }
    /*Nếu báo giá được tạo trực tiếp không qua một nguồn nào khác*/
    else {
      //Khách hàng định danh
      this.objectList = this.listCustomer;

      //Nhân viên bán hàng (người phụ trách)
      this.listEmpSale = this.listEmployee;

      //Hồ sơ thầu
      // this.listSaleBidding = this.listAllSaleBidding;
      this.listSaleBidding = [];

      //Cơ hội
      // this.listLead = this.listAllLead;
      this.listLead = [];
    }
  }

  /*Kiểm tra loại khách hàng*/
  getCustomerType(customerId: string): number {
    let object_1 = this.listCustomer.find(x => x.customerId == customerId);
    let object_2 = this.listCustomerNew.find(x => x.customerId == customerId);

    //Là khách hàng Định danh
    if (object_1 && !object_2) {
      return 1;
    }
    //Là khách hàng Tiềm năng
    else if (!object_1 && object_2) {
      return 0;
    }
    //khách hàng không xác định
    else {
      return -1;
    }
  }

  /*Mapdata vào Báo giá*/
  async mapDataToQuote(customer: Customer) {
    //Nếu khách hàng có trong tập khách hàng của người đang đăng nhập
    if (customer) {
      this.objectControl.setValue(customer);

      this.objCustomer = customer;
      this.email = customer.customerEmail;
      this.phone = customer.customerPhone;
      this.fullAddress = customer.fullAddress;

      //Số ngày được nợ
      let daysAreOwed = customer.maximumDebtDays ? customer.maximumDebtDays : 0;
      this.daysAreOwedControl.setValue(daysAreOwed);

      //Số nợ tối đa
      let maxAmount = customer.maximumDebtValue ? customer.maximumDebtValue : 0;
      this.maxDebtControl.setValue(maxAmount);

      //Nếu Phương thức thanh toán là Chuyển khoản thì lấy ra list account bank
      if (this.isShowBankAccount == true) {
        this.bankService.getAllBankAccountByObject(customer.customerId, this.customerContactCode).subscribe(response => {
          let result = <any>response;

          this.listBankAccount = result.bankAccountList;
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          }
        });
      }

      //Lấy list nhân viên bán hàng mới (phân quyền dữ liệu theo người phụ trách của khách hàng)
      this.listEmpSale = [];
      this.loading = true;
      let result: any = await this.quoteService.getEmployeeByPersonInCharge(customer.personInChargeId);
      this.loading = false;
      if (result.statusCode == 200) {
        this.listEmpSale = result.listEmployee;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }

      //Lấy list Cơ hội theo khách hàng được chọn
      this.listLead = this.listAllLead.filter(x => x.customerId == customer.customerId);

      //Lấy list Hồ sơ thầu theo khách hàng được chọn
      this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == customer.customerId);

      if (this.createObjectType == "LEAD") {
        /*Set value cho Cơ hội*/
        let defaultLead = this.listLead.find(x => x.leadId == this.occasionId);

        //Nếu cơ hội của khách hàng nằm trong tập cơ hội của người đăng nhập
        if (defaultLead) {
          this.coHoiControl.setValue(defaultLead);

          let defaultNumberType = parseInt(this.defaultNumberType, 10);
          this.listSaleBidding = this.listAllSaleBidding.filter(h => h.leadId == defaultLead.leadId);

          let personInChargeId = defaultLead.personInChargeId ?? null;
          if (personInChargeId) {
            let personInCharge: Employee = this.listEmpSale.find(x => x.employeeId == personInChargeId);

            if (personInCharge) {
              this.personInChargeControl.setValue(personInCharge);
            }
            else {
              let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Nhân viên phụ trách của Cơ hội không nằm trong tập nhân viên của người phụ trách khách hàng' };
              this.showMessage(mgs);
            }
          }

          /*Thêm list sản phẩm dịch vụ của Cơ hội vào list sản phẩm dịch vụ của Báo giá*/
          defaultLead.listLeadDetail.forEach((element, index) => {
            let obj: QuoteDetail = new QuoteDetail;
            obj.vendorId = element.vendorId;
            obj.productId = element.productId;
            obj.quantity = element.quantity;
            obj.unitPrice = element.unitPrice;
            obj.currencyUnit = element.currencyUnit;
            obj.exchangeRate = element.exchangeRate;
            obj.vat = element.vat;
            obj.discountType = element.discountType;
            obj.discountValue = element.discountValue;
            obj.description = element.description;
            obj.orderDetailType = element.orderDetailType;
            obj.unitId = element.unitId;
            obj.incurredUnit = element.incurredUnit;
            obj.productCode = element.productCode;
            obj.nameMoneyUnit = element.nameMoneyUnit;
            obj.nameVendor = element.nameVendor;
            obj.productNameUnit = element.productNameUnit;
            obj.productName = element.productName;
            obj.orderNumber = index + 1;

            let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
            if (obj.discountType) {
              obj.amountDiscount = sumprice * element.discountValue / 100;
            }
            else {
              obj.amountDiscount = sumprice - element.discountValue;
            }
            let vatAmount = (sumprice - obj.amountDiscount) * element.vat / 100;
            obj.sumAmount = this.roundNumber(sumprice - obj.amountDiscount + vatAmount, defaultNumberType);

            element.leadProductDetailProductAttributeValue.forEach(item => {
              let attributeValue: QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
              attributeValue.productId = item.productId;
              attributeValue.productAttributeCategoryId = item.productAttributeCategoryId;
              attributeValue.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
              obj.quoteProductDetailProductAttributeValue.push(attributeValue);
            });

            this.listQuoteDetailModel = [...this.listQuoteDetailModel, obj];
          });
          /*End*/

          this.calculatorAmount();
        }
        //Ngược lại
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Cơ hội của khách hàng này không thuộc quyền phụ trách của bạn' };
          this.showMessage(mgs);
        }
        /*End*/
      }
      else if (this.createObjectType == "SALEBIDDING") {
        /*Set value cho Báo giá*/
        let defaultSalebidding = this.listSaleBidding.find(x => x.saleBiddingId == this.saleBiddingId);

        //Nếu Hồ sơ thầu của khách hàng nằm trong tập cơ hội của người đăng nhập
        if (defaultSalebidding) {
          this.hoSoThauControl.setValue(defaultSalebidding);

          let defaultNumberType = parseInt(this.defaultNumberType, 10);

          //list Cơ hội sẽ bị xóa đi
          this.listLead = [];
          this.coHoiControl.reset();

          //Chọn Cơ hội gắn với Hồ sơ thầu
          let leadObj = this.listAllLead.find(l => l.leadId == defaultSalebidding.leadId);
          if (leadObj) {
            this.listLead.push(leadObj);
            this.coHoiControl.setValue(leadObj);
          }

          let personInChargeId = defaultSalebidding.personInChargeId ?? null;
          if (personInChargeId) {
            let personInCharge: Employee = this.listEmpSale.find(x => x.employeeId == personInChargeId);

            if (personInCharge) {
              this.personInChargeControl.setValue(personInCharge);
            }
            else {
              let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Nhân viên phụ trách của Hồ sơ thầu không nằm trong tập nhân viên của người phụ trách khách hàng' };
              this.showMessage(mgs);
            }
          }

          /*Thêm list sản phẩm dịch vụ của Cơ hội vào list sản phẩm dịch vụ của Báo giá*/
          defaultSalebidding.saleBiddingDetail.forEach((element, index) => {
            let obj: QuoteDetail = new QuoteDetail;
            obj.vendorId = element.vendorId;
            obj.productId = element.productId;
            obj.quantity = element.quantity;
            obj.unitPrice = element.unitPrice;
            obj.currencyUnit = element.currencyUnit;
            obj.exchangeRate = element.exchangeRate;
            obj.vat = element.vat;
            obj.discountType = element.discountType;
            obj.discountValue = element.discountValue;
            obj.description = element.description;
            obj.orderDetailType = element.orderDetailType;
            obj.unitId = element.unitId;
            obj.incurredUnit = element.incurredUnit;
            obj.productCode = element.productCode;
            obj.nameMoneyUnit = element.nameMoneyUnit;
            obj.nameVendor = element.nameVendor;
            obj.productNameUnit = element.productNameUnit;
            obj.productName = element.productName;
            obj.orderNumber = index + 1;

            let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
            if (obj.discountType) {
              obj.amountDiscount = sumprice * element.discountValue / 100;
            }
            else {
              obj.amountDiscount = sumprice - element.discountValue;
            }
            let vatAmount = (sumprice - obj.amountDiscount) * element.vat / 100;
            obj.sumAmount = this.roundNumber(sumprice - obj.amountDiscount + vatAmount, defaultNumberType);

            element.saleBiddingDetailProductAttribute.forEach(item => {
              let attributeValue: QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
              attributeValue.productId = item.productId;
              attributeValue.productAttributeCategoryId = item.productAttributeCategoryId;
              attributeValue.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
              obj.quoteProductDetailProductAttributeValue.push(attributeValue);
            });

            this.listQuoteDetailModel = [...this.listQuoteDetailModel, obj];
          });
          /*End*/

          this.calculatorAmount();
        }
        //Ngược lại
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Hồ sơ thầu của khách hàng này không thuộc quyền phụ trách của bạn' };
          this.showMessage(mgs);
        }
        /*End*/
      }
    }
    //Nếu khách hàng không có trong tập khách hàng của người đang đăng nhập
    else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Khách hàng gắn với Hồ sơ thầu này không thuộc quyền phụ trách của bạn' };
      this.showMessage(mgs);
    }
  }

  /*Event chuyển loại khách hàng (Khách hàng hoặc Khách hàng tiềm năng)*/
  changeObjectType(objecType: any) {
    if (objecType == 'cus') {
      this.objectList = this.listCustomer;
    } else if (objecType == 'lea') {
      this.objectList = this.listCustomerNew;
    }

    //Ẩn Hộp quà CTKM
    this.isPromotionCustomer = false;

    //Xóa những sản phẩm khuyến mãi theo khách hàng
    this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 1);

    if (!this.objectList.find(x => x == this.objectControl.value)) {
      //Nếu khách hàng hiện tại đang được chọn không thuộc objectList thì reset các trường
      this.objCustomer = null;
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

      //Xóa list sản phẩm dịch vụ
      this.listQuoteDetailModel = [];

      //bỏ chọn nhân viên bán hàng
      this.personInChargeControl.reset();

      //reset list nhân viên bán hàng
      this.listEmpSale = this.listEmployee;

      //bỏ chọn Cơ hội
      this.coHoiControl.reset();

      //reset list Cơ hội
      // this.listLead = this.listAllLead;
      this.listLead = [];
      
      //bỏ chọn Hồ sơ thầu
      this.hoSoThauControl.reset();

      //reset list Hồ sơ thầu
      // this.listSaleBidding = this.listAllSaleBidding;
      this.listSaleBidding = [];

      this.calculatorAmount();
    }
  }

  /*Event thay đổi khách hàng*/
  async changeCustomer(value: Customer) {
    this.objCustomer = null;
    this.email = '';
    this.phone = '';
    this.fullAddress = '';

    //Reset Phương thức thanh toán
    const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
    this.paymentMethodControl.setValue(toSelectPaymentMethod ?? null);
    //End

    this.daysAreOwedControl.setValue('0');  //Reset Số ngày được nợ
    this.maxDebtControl.setValue('0');  //Reset Số nợ tối đa

    //Reset Tài khoản ngân hàng
    this.listBankAccount = [];
    this.bankAccountControl.reset();
    if (toSelectPaymentMethod?.categoryCode == "CASH") {
      this.isShowBankAccount = false;
    }
    else if (toSelectPaymentMethod?.categoryCode == "BANK") {
      this.isShowBankAccount = true;
    }
    else {
      this.isShowBankAccount = false;
    }

    //reset nhân viên bán hàng (người phụ trách)
    this.listEmpSale = this.listEmployee;
    this.personInChargeControl.reset();

    //reset cơ hội
    // // this.listLead = this.listAllLead;
    this.listLead = [];
    this.coHoiControl.reset();

    //reset hồ sơ thầu
    // this.listSaleBidding = this.listAllSaleBidding;
    this.listSaleBidding = [];
    this.hoSoThauControl.reset();

    //Danh sách sản phẩm dịch vụ
    this.listQuoteDetailModel = [];

    //Ẩn Hộp quà CTKM
    this.isPromotionCustomer = false;

    //Xóa những sản phẩm khuyến mãi theo khách hàng
    this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 1);

    //Tính lại phần Tổng hợp báo giá
    this.calculatorAmount();

    /*Nếu chọn khách hàng*/
    if (value) {
      this.objCustomer = value;
      this.email = value.customerEmail;
      this.phone = value.customerPhone;
      this.fullAddress = value.fullAddress;

      //Số ngày được nợ
      let daysAreOwed = value.maximumDebtDays ? value.maximumDebtDays : 0;
      this.daysAreOwedControl.setValue(daysAreOwed);

      //Số nợ tối đa
      let maxAmount = value.maximumDebtValue ? value.maximumDebtValue : 0;
      this.maxDebtControl.setValue(maxAmount);

      //Nếu Phương thức thanh toán là Chuyển khoản thì lấy ra list account bank
      if (this.isShowBankAccount == true) {
        this.bankService.getAllBankAccountByObject(value.customerId, this.customerContactCode).subscribe(response => {
          let result = <any>response;

          this.listBankAccount = result.bankAccountList;
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          }
        });
      }

      //Lấy list Cơ hội theo khách hàng được chọn
      this.listLead = this.listAllLead.filter(x => x.customerId == value.customerId);

      //Lấy list Hồ sơ thầu theo khách hàng được chọn
      this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == value.customerId);

      //Lấy list nhân viên bán hàng mới (phân quyền dữ liệu theo người phụ trách của khách hàng)
      this.listEmpSale = [];
      this.loading = true;
      let result: any = await this.quoteService.getEmployeeByPersonInCharge(value.personInChargeId);
      if (result.statusCode == 200) {
        this.listEmpSale = result.listEmployee;

        //set default value là người phụ trách của khách hàng được chọn
        let emp = this.listEmpSale.find(e => e.employeeId == value.personInChargeId);
        this.personInChargeControl.setValue(emp);
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }

      let _result: any = await this.promotionService.checkPromotionByCustomer(value.customerId);
      this.loading = false;
      if (_result.statusCode == 200) {
        this.isPromotionCustomer = _result.isPromotionCustomer;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: _result.messageCode };
        this.showMessage(mgs);
      }
    }
  }

  /*Tính các giá trị của Tổng hợp báo giá*/
  calculatorAmount() {
    let defaultNumberType = parseInt(this.defaultNumberType, 10);

    this.amount = 0; //Tổng giá trị hàng hóa bán ra
    this.amountPriceInitial = 0; //Tổng giá trị hàng hóa đầu vào
    this.amountPriceCost = 0; //Tổng chi phí
    this.totalAmountVat = 0; //Tổng thuế VAT
    this.totalAmountAfterVat = 0; //Tổng tiền sau thuế
    this.totalAmountPromotion = 0; //Tổng tiền khuyến mãi
    this.totalAmountDiscount = 0; //Tổng thành tiền chiết khấu
    this.customerOrderAmountAfterDiscount = 0; //Tổng thanh toán
    this.amountPriceProfit = 0; //Lợi nhuận tạm tính
    this.valuePriceProfit = 0; //% lợi nhuận tạm tính

    //Danh sách sản phẩm dịch vụ
    this.listQuoteDetailModel.forEach(item => {
      let price = item.quantity * item.unitPrice * item.exchangeRate;

      /*Tiền chiết khấu*/
      if (item.discountType) {
        item.amountDiscount = price * item.discountValue / 100;
      }
      else {
        item.amountDiscount = item.discountValue;
      }
      /*End*/

      /*Tiền thuế VAT*/
      let amountVAT = (price - item.amountDiscount) * item.vat / 100;
      /*End*/

      /*Tổng thuế VAT*/
      this.totalAmountVat += amountVAT;
      /*End*/

      /*Thành tiền*/
      item.sumAmount = price - item.amountDiscount + amountVAT;
      /*End*/

      /*Tiền vốn*/
      let amountInitial = item.quantity * item.priceInitial * item.exchangeRate;
      /*End*/

      /*Tổng giá trị hàng hóa bán ra*/
      this.amount += this.roundNumber((price - item.amountDiscount), defaultNumberType);
      /*End*/

      /*Tổng giá trị hàng hóa đầu vào*/
      this.amountPriceInitial += this.roundNumber(amountInitial, defaultNumberType);
      /*End*/
    });

    //Danh sách chi phí
    this.listQuoteCostDetailModel.forEach(item => {
      let price = item.quantity * item.unitPrice;

      this.amountPriceCost += this.roundNumber(price, defaultNumberType);
    });

    /*Tổng tiền sau thuế*/
    this.totalAmountAfterVat = this.amount + this.totalAmountVat + this.amountPriceCost;

    //Kiểm tra có đủ điều kiện nhận quà khuyến mãi không
    this.promotionService.checkPromotionByAmount(this.totalAmountAfterVat).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.isPromotionAmount = result.isPromotionAmount;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
    /*End*/

    //Nếu không đủ điều kiện nhận khuyến mãi thì xóa các quà khuyến mãi đó
    let ignoreList = this.listPromotionApply.filter(x => x.conditionsType == 3 && x.soTienTu > this.totalAmountAfterVat);
    this.listPromotionApply = this.listPromotionApply.filter(x => !ignoreList.includes(x));

    /*Tính lại thành tiền của Tab Khuyến mãi*/
    this.listPromotionApply.forEach(item => {
      //Nếu là phiếu giảm giá
      if (!item.productId) {
        //Nếu là số tiền
        if (!item.loaiGiaTri) {
          item.amount = item.giaTri * item.soLuongTang;
        }
        //Nếu là %
        else {
          item.amount = this.roundNumber((this.totalAmountAfterVat * item.giaTri / 100) * item.soLuongTang, defaultNumberType);
        }
      }
      //Nếu là sản phẩm
      else {
        item.amount = item.giaTri * item.soLuongTang;
      }
    });
    /*End*/

    /*Tổng tiền khuyến mãi*/
    let totalAmountPhieuGiamGia = 0;  //Tổng thành tiền của tất cả phiếu giảm giá trong tab khuyến mãi
    this.listPromotionApply.forEach(item => {
      //Chỉ tính với phiếu giảm giá
      if (!item.productId) {
        totalAmountPhieuGiamGia += item.amount;
        this.totalAmountPromotion += item.amount;
      }
    }); 
    /*End*/

    /*Tổng thành tiền chiết khấu*/
    let discountType: DiscountType = this.discountTypeControl.value;
    let discountValue = ParseStringToFloat(this.discountValueControl.value);

    if (discountType.value) {
      this.totalAmountDiscount = this.totalAmountAfterVat * discountValue / 100;
    }
    else {
      this.totalAmountDiscount = discountValue;
    }
    /*End*/

    /*Tổng thanh toán*/
    this.customerOrderAmountAfterDiscount = this.roundNumber(this.totalAmountAfterVat - this.totalAmountDiscount - totalAmountPhieuGiamGia, defaultNumberType);
    /*End*/

    /*Nếu Tổng thanh toán không âm và khác 0*/
    if (this.customerOrderAmountAfterDiscount > 0) {
      /*Lợi nhuận tạm tính*/
      this.amountPriceProfit = this.roundNumber(this.customerOrderAmountAfterDiscount - this.amountPriceInitial, defaultNumberType);
      /*End*/
      
      /*% lợi nhuận tạm tính*/
      this.valuePriceProfit = this.roundNumber((this.amountPriceProfit / this.customerOrderAmountAfterDiscount) * 100, defaultNumberType);
      /*End*/
    }
    //Nếu Tổng thanh toán âm hoặc bằng 0
    else {
      this.customerOrderAmountAfterDiscount = 0;
      this.amountPriceProfit = 0;
      this.valuePriceProfit = 0;
    }
    /*End*/
  }

  /*Event thay đổi cơ hội*/
  changeLead(value: any) {
    this.hoSoThauControl.reset();
    this.personInChargeControl.reset();
    this.listQuoteDetailModel = [];

    if (value) {
      /*Khách hàng sẽ là khách hàng của Hồ sơ thầu*/
      let customerType = this.getCustomerType(value.customerId);
      //Là khách hàng Định danh
      if (customerType == 1) {
        this.objectList = this.listCustomer;
        let customer = this.listCustomer.find(x => x.customerId == value.customerId);

        if (customer) {
          this.setDataFormCustomer(customer);
        }
      }
      //Là khách hàng Tiềm năng
      else if (customerType == 0) {
        this.selectedObjectType = 'lea';
        this.objectList = this.listCustomerNew;
        let customerNew = this.listCustomerNew.find(x => x.customerId == value.customerId);

        if (customerNew) {
          this.setDataFormCustomer(customerNew);
        }
      }
      //khách hàng không xác định
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
        this.showMessage(mgs);
      }
      /*End*/

      let defaultNumberType = parseInt(this.defaultNumberType, 10);
      this.listSaleBidding = this.listAllSaleBidding.filter(h => h.leadId == value.leadId);

      let personInChargeId = value.personInChargeId ?? null;
      if (personInChargeId) {
        let personInCharge: Employee = this.listEmpSale.find(x => x.employeeId == personInChargeId);
        this.personInChargeControl.setValue(personInCharge);
      }

      /*Thêm list sản phẩm dịch vụ của Cơ hội vào list sản phẩm dịch vụ của Báo giá*/
      value.listLeadDetail.forEach((element, index) => {
        let obj: QuoteDetail = new QuoteDetail;
        obj.vendorId = element.vendorId;
        obj.productId = element.productId;
        obj.quantity = element.quantity;
        obj.unitPrice = element.unitPrice;
        obj.currencyUnit = element.currencyUnit;
        obj.exchangeRate = element.exchangeRate;
        obj.vat = element.vat;
        obj.discountType = element.discountType;
        obj.discountValue = element.discountValue;
        obj.description = element.description;
        obj.orderDetailType = element.orderDetailType;
        obj.unitId = element.unitId;
        obj.incurredUnit = element.incurredUnit;
        obj.productCode = element.productCode;
        obj.nameMoneyUnit = element.nameMoneyUnit;
        obj.nameVendor = element.nameVendor;
        obj.productNameUnit = element.productNameUnit;
        obj.productName = element.productName;
        obj.orderNumber = index + 1;

        let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
        if (obj.discountType) {
          obj.amountDiscount = sumprice * element.discountValue / 100;
        }
        else {
          obj.amountDiscount = sumprice - element.discountValue;
        }
        let vatAmount = (sumprice - obj.amountDiscount) * element.vat / 100;
        obj.sumAmount = this.roundNumber(sumprice - obj.amountDiscount + vatAmount, defaultNumberType);

        element.leadProductDetailProductAttributeValue.forEach(item => {
          let attributeValue: QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
          attributeValue.productId = item.productId;
          attributeValue.productAttributeCategoryId = item.productAttributeCategoryId;
          attributeValue.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obj.quoteProductDetailProductAttributeValue.push(attributeValue);
        });

        this.listQuoteDetailModel = [...this.listQuoteDetailModel, obj];
      });
      /*End*/
    }
    else {
      //Nếu có khách hàng đang được chọn thì list Hồ sơ thầu sẽ lấy theo khách hàng đang được chọn
      if (this.objCustomer) {
        this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == this.objCustomer.customerId);
      }
      //Nếu không có khách hàng đang được chọn thì list Hồ sơ thầu sẽ lấy theo người đang đăng nhập
      else {
        this.listSaleBidding = this.listAllSaleBidding;
      }
    }

    this.calculatorAmount();
  }

  /*Hiển thị lại Khách hàng, Email, Sđt, Địa chỉ, Số ngày được nợ, Số nợ tối đa theo CustomerId*/
  setDataFormCustomer(customer: Customer) {
    this.objectControl.setValue(customer);
    this.objCustomer = customer;
    this.email = customer.customerEmail;
    this.phone = customer.customerPhone;
    this.fullAddress = customer.fullAddress;

    //Số ngày được nợ
    let daysAreOwed = customer.maximumDebtDays ? customer.maximumDebtDays : 0;
    this.daysAreOwedControl.setValue(daysAreOwed);

    //Số nợ tối đa
    let maxAmount = customer.maximumDebtValue ? customer.maximumDebtValue : 0;
    this.maxDebtControl.setValue(maxAmount);

    //Reset Tài khoản ngân hàng
    this.isShowBankAccount = false;
    this.listBankAccount = [];
    this.bankAccountControl.setValue(null);
    //End

    //Phương thức thanh toán
    const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
    this.paymentMethodControl.setValue(toSelectPaymentMethod);

    if (toSelectPaymentMethod?.categoryCode == "CASH") {
      this.isShowBankAccount = false;
    }
    else if (toSelectPaymentMethod?.categoryCode == "BANK") {
      this.isShowBankAccount = true;
    }
    else {
      this.isShowBankAccount = false;
    }

    //Nếu Phương thức thanh toán là Chuyển khoản thì lấy ra list account bank
    if (this.isShowBankAccount == true) {
      this.bankService.getAllBankAccountByObject(customer.customerId, this.customerContactCode).subscribe(response => {
        let result = <any>response;

        this.listBankAccount = result.bankAccountList;
        if (this.listBankAccount.length > 0) {
          let toSelectBankAccount = this.listBankAccount[0];
          this.bankAccountControl.setValue(toSelectBankAccount);
        }
      });
    }
  }

  /*Event thay đổi Hồ sơ thầu*/
  changeSaleBidding(value: any) {
    //list sản phẩm dịch vụ của báo giá sẽ bị xóa đi
    this.listQuoteDetailModel = [];

    //list Cơ hội sẽ bị xóa đi
    this.listLead = [];
    this.coHoiControl.reset();

    this.personInChargeControl.reset();

    //Nếu chọn Hồ sơ thầu
    if (value) {
      /*Khách hàng sẽ là khách hàng của Hồ sơ thầu*/
      let customerType = this.getCustomerType(value.customerId);
      //Là khách hàng Định danh
      if (customerType == 1) {
        this.objectList = this.listCustomer;
        let customer = this.listCustomer.find(x => x.customerId == value.customerId);

        if (customer) {
          this.setDataFormCustomer(customer);
        }
      }
      //Là khách hàng Tiềm năng
      else if (customerType == 0) {
        this.selectedObjectType = 'lea';
        this.objectList = this.listCustomerNew;
        let customerNew = this.listCustomerNew.find(x => x.customerId == value.customerId);

        if (customerNew) {
          this.setDataFormCustomer(customerNew);
        }
      }
      //khách hàng không xác định
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Cơ hội' };
        this.showMessage(mgs);
      }
      /*End*/

      //Chọn Cơ hội gắn với Hồ sơ thầu
      let leadObj = this.listAllLead.find(l => l.leadId == value.leadId);
      if (leadObj) {
        this.listLead.push(leadObj);
        this.coHoiControl.setValue(leadObj);
      }

      //Nhân viên bán hàng (người phụ trách) của báo giá sẽ là người phụ trách của Hồ sơ thầu
      if (value.personInChargeId !== null) {
        let emp = this.listEmpSale.find(e => e.employeeId == value.personInChargeId);
        this.personInChargeControl.setValue(emp);
      }

      /*
      * Thêm các sản phẩm dịch vụ trong Hồ sơ vào list sản phẩm dịch vụ của Báo giá
      * Nhân viên bán hàng sẽ thay đổi là nhân viên phụ trách Hồ sơ thầu
      */
      value.saleBiddingDetail.forEach((element, index) => {
        let obj: QuoteDetail = new QuoteDetail;
        obj.vendorId = element.vendorId;
        obj.productId = element.productId;
        obj.quantity = element.quantity;
        obj.unitPrice = element.unitPrice;
        obj.currencyUnit = element.currencyUnit;
        obj.exchangeRate = element.exchangeRate;
        obj.vat = element.vat;
        obj.discountType = element.discountType;
        obj.discountValue = element.discountValue;
        obj.description = element.description;
        obj.orderDetailType = element.orderDetailType;
        obj.unitId = element.unitId;
        obj.incurredUnit = element.incurredUnit;
        obj.productCode = element.productCode;
        obj.productName = element.productName;
        obj.nameMoneyUnit = element.nameMoneyUnit;
        obj.nameVendor = element.nameVendor;
        obj.productNameUnit = element.productNameUnit;
        obj.orderNumber = index + 1;

        let sumprice = element.quantity * element.unitPrice * element.exchangeRate;
        if (obj.discountType) {
          obj.amountDiscount = sumprice * element.discountValue / 100;
        }
        else {
          obj.amountDiscount = sumprice - element.discountValue;
        }
        let vatAmount = (sumprice - obj.amountDiscount) * element.vat / 100;
        obj.sumAmount = sumprice - obj.amountDiscount + vatAmount;

        element.saleBiddingDetailProductAttribute.forEach(item => {
          let attributeValue: QuoteProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue;
          attributeValue.productId = item.productId;
          attributeValue.productAttributeCategoryId = item.productAttributeCategoryId;
          attributeValue.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
          obj.quoteProductDetailProductAttributeValue.push(attributeValue);
        });

        this.listQuoteDetailModel = [...this.listQuoteDetailModel, obj];
      });
    }
    //Nếu bỏ chọn Hồ sơ thầu
    else {
      //Nếu có khách hàng đang được chọn thì list Cơ hội sẽ lấy theo khách hàng đang được chọn
      if (this.objCustomer) {
        this.listLead = this.listAllLead.filter(x => x.customerId == this.objCustomer.customerId);
      }
      //Nếu không có khách hàng đang được chọn thì list Cơ hội sẽ lấy theo người đang đăng nhập
      else {
        this.listLead = this.listAllLead;
      }
    }

    this.calculatorAmount();
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
          let result = <any>response;

          this.listBankAccount = result.bankAccountList;
          if (this.listBankAccount.length > 0) {
            let toSelectBankAccount = this.listBankAccount[0];
            this.bankAccountControl.setValue(toSelectBankAccount);
          }
        });
      }
    } else {
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
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
    //Xóa ngày hết hạn của báo giá vì ngày gửi hiện tại đã null
    this.expirationDate = null;
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
    let rowData = event.data;
    this.isUpdateAI = true;
    this.AIUpdate.ordinal = rowData.ordinal;
    this.AIUpdate.title = rowData.title;

    this.titleText = rowData.title;
    this.contentText = rowData.content;
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
        cusGroupId: cusGroupId,
        orderDate: new Date()
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

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let quoteDetailModel: QuoteDetail = result.quoteDetailModel;

          //set orderNumber cho sản phẩm/dịch vụ mới thêm
          quoteDetailModel.orderNumber = this.listQuoteDetailModel.length + 1;

          //Kiểm tra sản phẩm có đủ điều kiện nhận quà KM?
          if (quoteDetailModel.productId != null && quoteDetailModel.productId != this.emptyGuid) {
            let response: any = await this.promotionService.checkPromotionByProduct(quoteDetailModel.productId, quoteDetailModel.quantity);
            quoteDetailModel.isPromotionProduct = response.isPromotionProduct;
          }

          this.listQuoteDetailModel = [...this.listQuoteDetailModel, quoteDetailModel];
          this.calculatorAmount();
        }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    var index = this.listQuoteDetailModel.indexOf(dataRow);
    var OldArray = this.listQuoteDetailModel[index];

    let titlePopup = '';
    if (OldArray.orderDetailType == 0) {
      titlePopup = 'Sửa sản phẩm dịch vụ';
    } else if (OldArray.orderDetailType == 1) {
      titlePopup = 'Sửa chi phí phát sinh';
    }

    let cusGroupId = null;
    if (this.objCustomer !== null && this.objCustomer !== undefined) cusGroupId = this.objCustomer.customerGroupId;
    let ref = this.dialogService.open(AddEditProductDialogComponent, {
      data: {
        isCreate: false,
        cusGroupId: cusGroupId,
        quoteDetailModel: OldArray,
        orderDate: new Date()
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

    ref.onClose.subscribe(async (result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.listQuoteDetailModel.splice(index, 1);
          let quoteDetailModel = result.quoteDetailModel;

          //Kiểm tra sản phẩm có đủ điều kiện nhận quà KM?
          if (quoteDetailModel.productId != null && quoteDetailModel.productId != this.emptyGuid) {
            let response: any = await this.promotionService.checkPromotionByProduct(quoteDetailModel.productId, quoteDetailModel.quantity);
            quoteDetailModel.isPromotionProduct = response.isPromotionProduct;
          }

          this.listQuoteDetailModel = [...this.listQuoteDetailModel, quoteDetailModel];
          this.reOrderListQuoteDetail();

          this.calculatorAmount();
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listQuoteDetailModel = this.listQuoteDetailModel.filter(e => e != dataRow);
        //Đánh lại số OrderNumber
        this.listQuoteDetailModel.forEach((item, index) => {
          item.orderNumber = index + 1;
        });

        this.calculatorAmount();
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
          let quoteCostDetailModel: QuoteCostDetail = result.quoteDetailModel;
          this.listQuoteCostDetailModel = [...this.listQuoteCostDetailModel, quoteCostDetailModel];

          this.calculatorAmount();
        }
      }
    });
  }

  /*Sửa một chi phí*/
  onRowCostSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
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
          let quoteCostDetailModel: QuoteCostDetail = result.quoteDetailModel;
          this.listQuoteCostDetailModel[index] = quoteCostDetailModel;

          this.calculatorAmount();
        }
      }
    });
  }

  /*Xóa một chi phí*/
  deleteCostItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listQuoteCostDetailModel.indexOf(dataRow);
        this.listQuoteCostDetailModel.splice(index, 1);
        this.listQuoteCostDetailModel = [...this.listQuoteCostDetailModel];

        this.calculatorAmount();
      }
    });
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
    this.discountValueControl.setValue('0');

    this.calculatorAmount();
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

    this.calculatorAmount();
  }

  /*Tạo báo giá*/
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
      if (this.uploadedFiles.length > 0) {
        this.uploadFiles(this.uploadedFiles);
        this.converToArrayQuoteDocument(this.uploadedFiles);
      }

      /*Binding data for quoteModel*/
      let quote = this.mapDataToModel();

      //List Người tham gia
      let listSelectedParticipant: Array<Employee> = this.participantControl.value;
      let listParticipant = listSelectedParticipant?.map(x => x.employeeId) ?? [];

      this.awaitResult = false;
      this.loading = true;
      this.quoteService.CreateQuote(quote, this.listQuoteDetailModel, 1, this.arrayQuoteDocumentModel,
        this.listAdditionalInformation, this.listQuoteCostDetailModel,
        false, this.emptyGuid, listParticipant, this.listPromotionApply).subscribe(response => {
          let result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            let messageCode = "Tạo báo giá thành công";
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
            this.showMessage(mgs);

            if (value) {
              //Lưu và thêm mới
              if (this.emitStatusChangeForm) {
                this.emitStatusChangeForm.unsubscribe();
                this.isInvalidForm = false; //Ẩn icon-warning-active
              }
              this.resetForm();
              this.awaitResult = false;
            } else {
              //Lưu
              this.router.navigate(['/customer/quote-detail', { quoteId: result.quoteID }]);
            }
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
    }
  }

  mapDataToModel(): Quote {
    let quote = new Quote();

    quote.quoteId = this.emptyGuid;
    quote.quoteCode = '';
    quote.quoteName = this.nameQuoteControl.value?.trim();
    quote.quoteDate = convertToUTCTime(new Date);
    let sendQuoteDate = (this.sendDateControl != null ? this.sendDateControl.value : null);
    if (sendQuoteDate) {
      sendQuoteDate = convertToUTCTime(sendQuoteDate);
      quote.sendQuoteDate = sendQuoteDate;
    } else {
      quote.sendQuoteDate = null;
    }

    let personInCharge: Employee = this.personInChargeControl.value != null ? this.personInChargeControl.value : null;
    if (personInCharge) {
      quote.personInChargeId = personInCharge.employeeId;
    } else {
      quote.personInChargeId = null;
    }
    quote.seller = quote.personInChargeId;

    let effectiveQuoteDate = parseFloat(this.effectiveDateControl.value.replace(/,/g, ''));
    quote.effectiveQuoteDate = effectiveQuoteDate;

    quote.expirationDate = this.expirationDate;

    let description = this.descriptionControl.value != null ? this.descriptionControl.value.trim() : '';
    quote.description = description;

    let note = this.noteControl.value != null ? this.noteControl.value.trim() : '';
    quote.note = note;

    quote.objectType = 'CUSTOMER';

    let object: Customer = this.objectControl.value;
    quote.objectTypeId = object.customerId;

    let paymentMethod: Category = this.paymentMethodControl.value != null ? this.paymentMethodControl.value : null;
    if (paymentMethod) {
      quote.paymentMethod = paymentMethod.categoryId;
    } else {
      quote.paymentMethod = null;
    }

    let discountType: DiscountType = this.discountTypeControl.value;
    quote.discountType = discountType.value;

    let bankAccount: BankAccount = this.bankAccountControl.value;
    if (bankAccount) {
      quote.bankAccountId = bankAccount.bankAccountId;
    } else {
      quote.bankAccountId = null;
    }

    let daysAreOwed = this.daysAreOwedControl.value != null ? parseFloat(this.daysAreOwedControl.value.replace(/,/g, '')) : 0;
    quote.daysAreOwed = daysAreOwed;

    let maxDebt = this.maxDebtControl.value != null ? parseFloat(this.maxDebtControl.value.replace(/,/g, '')) : 0;
    quote.maxDebt = maxDebt;

    quote.receivedDate = convertToUTCTime(new Date());
    quote.receivedHour = null;
    quote.recipientName = '';
    quote.locationOfShipment = '';
    quote.shippingNote = '';
    quote.recipientPhone = '';
    quote.recipientEmail = '';
    quote.placeOfDelivery = '';

    let intendedQuoteDate = this.intendedDateControl.value;
    quote.intendedQuoteDate = convertToUTCTime(intendedQuoteDate);

    quote.amount = this.amount;

    let discountValue = this.discountValueControl.value != null ? this.discountValueControl.value : '0';
    discountValue = parseFloat(discountValue.replace(/,/g, ''));
    quote.discountValue = discountValue;

    let statusQuote: Category = this.quoteStatusControl.value;
    quote.statusId = statusQuote.categoryId;

    let lead: any = this.coHoiControl.value != null ? this.coHoiControl.value : null;
    if (lead) {
      quote.leadId = lead.leadId;
    } else {
      quote.leadId = null;
    }

    let saleBillding: any = this.hoSoThauControl.value != null ? this.hoSoThauControl.value : null;
    if (saleBillding) {
      quote.saleBiddingId = saleBillding.saleBiddingId;
    } else {
      quote.saleBiddingId = null;
    }

    let investmentFundId: any = this.kenhControl.value != null ? this.kenhControl.value : null;
    if (investmentFundId) {
      quote.investmentFundId = investmentFundId.categoryId;
    } else {
      quote.investmentFundId = null;
    }

    quote.active = true;
    quote.createdById = this.emptyGuid;
    quote.createdDate = convertToUTCTime(new Date());
    quote.updatedById = this.emptyGuid;
    quote.updatedDate = convertToUTCTime(new Date());

    return quote;
  }

  resetForm() {
    this.objCustomer = null;
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
    this.quoteStatusControl.setValue(this.listQuoteStatus.find(x => x.isDefault == true));
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
    this.descriptionControl.setValue('');
    this.noteControl.setValue('');
    this.uploadedFiles = [];
    if (this.fileUpload) {
      this.fileUpload.clear();  //Xóa toàn bộ file trong control
    }
    this.titleText = '';
    this.contentText = '';
    this.isUpdateAI = false;
    this.listAdditionalInformation = [];
    this.arrayQuoteDocumentModel = [];
    this.listQuoteDetailModel = [];
    this.listQuoteCostDetailModel = [];
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl.setValue('0');

    this.amount = 0; //Tổng giá trị hàng hóa bán ra
    this.amountPriceInitial = 0; //Tổng giá trị hàng hóa đầu vào
    this.amountPriceCost = 0; //Tổng chi phí
    this.totalAmountVat = 0; //Tổng thuế VAT
    this.totalAmountAfterVat = 0; //Tổng tiền sau thuế
    this.totalAmountPromotion = 0; //Tổng tiền khuyến mãi
    this.totalAmountDiscount = 0; //Tổng thành tiền chiết khấu
    this.customerOrderAmountAfterDiscount = 0; //Tổng thanh toán
    this.amountPriceProfit = 0; //Lợi nhuận tạm tính
    this.valuePriceProfit = 0; //% lợi nhuận tạm tính
  }

  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  cancel() {
    this.router.navigate(['/customer/quote-list']);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  converToArrayQuoteDocument(fileList: File[]) {
    for (var x = 0; x < fileList.length; ++x) {
      let quoteDocumentModel = new QuoteDocument();
      quoteDocumentModel.documentName = fileList[x].name;
      quoteDocumentModel.documentSize = fileList[x].size + '';
      quoteDocumentModel.createdDate = convertToUTCTime(new Date());
      quoteDocumentModel.updatedDate = convertToUTCTime(new Date());
      this.arrayQuoteDocumentModel.push(quoteDocumentModel);
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  async showDialogImport() {
    await this.getInforDetailQuote();
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
        link.download = fileName;
        link.click();
      }
    }, error => { this.loading = false; });
  }

  chooseFile(event: any) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;
  }

  cancelFile() {
    $("#importFileProduct").val("")
    this.fileName = "";
  }

  /* Lấy master data để import và export sản phẩm dịch vụ */
  async getInforDetailQuote() {
    let result: any = await this.productService.getMasterdataCreateProduct();
    if (result.statusCode === 200) {
      this.productCodeSystemList = result.listProductCode;
      this.listProductUnitName = result.listProductUnitName;
    };

    let result_2: any = await this.quoteService.getDataQuoteAddEditProductDialogAsync();
    if (result_2.statusCode == 200) {
      this.listProduct = result_2.listProduct;
      this.listUnitMoney = result_2.listUnitMoney;
      this.listUnitProduct = result_2.listUnitProduct;
    }
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

          if (!isPattermSL) {
            this.messErrFile.push('Cột số lượng tại dòng ' + (i + 2) + ' sai định dạng');
          }
        }
        if (row[4] === null || row[4] === undefined || row[4] == "") {
          this.messErrFile.push('Cột đơn giá tại dòng ' + (i + 2) + ' không được để trống');
        }
        else {
          let isPattermDG = /^\d+$/.test(row[4].toString().trim());

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
        let sheetName = 'BOM Lines';
        if (workbook.Sheets[sheetName] === undefined) {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: "File không hợp lệ" };
          this.showMessage(mgs);
          return;
        }

        //lấy data từ file excel của khách hàng doanh nghiệp
        const worksheetProduct: XLSX.WorkSheet = workbook.Sheets[sheetName];
        /* save data */
        let listProductImport: Array<any> = XLSX.utils.sheet_to_json(worksheetProduct, { header: 1 });
        //remove header row
        listProductImport.shift();
        let productCodeList: string[] = [];
        let productUnitList: string[] = [];

        let isValidation = this.validateFile(listProductImport);
        if (isValidation) {
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
        }
        else {
          var messCodeErr = [];
          var messUnitErr = [];
          this.isInvalidForm = false;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = false;  //Hiển thị message lỗi

          listProductImport.forEach((row, i) => {
            // Lấy giá trị bản ghi trong excel bắt đầu từ line 6
            if (i > 4 && row.length != 0) {
              if (row[1] !== null && row[1] !== undefined && row[1].trim() != "") {
                let rowObj = productCodeList.filter(p => p.trim().toUpperCase() == row[1].trim().toUpperCase());
                if (rowObj.length === 0) {
                  productCodeList.push(row[1].toLowerCase().trim());
                }
                let check = this.productCodeSystemList.find(d => d.toLowerCase().trim() == row[1].trim().toLowerCase());
                if (check === undefined || check === null) {
                  messCodeErr.push(i + 2);
                }
              };
              if (row[5] !== null && row[5] !== undefined && row[5].trim() != "" &&
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "")) {
                let rowObj = productUnitList.filter(p => p.trim().toUpperCase() == row[5].trim().toUpperCase());
                if (rowObj.length === 0) {
                  productUnitList.push(row[5].toLowerCase().trim());
                  let check = this.listProductUnitName.find(d => d.toLowerCase().trim() == row[5].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messUnitErr.push(i + 2);
                  }
                }
              };
            }
          });

          let countCode = this.productCodeSystemList.filter(c => productCodeList.includes(c.toLowerCase().trim()));
          let countUnit = this.listProductUnitName.filter(u => productUnitList.includes(u.toLowerCase().trim()));

          //chuẩn hóa mảng
          productCodeList = [...new Set(productCodeList.map(e => Boolean(e) === true && e.trim().toLowerCase()))];
          countCode = [...new Set(countCode.map(e => Boolean(e) === true && e.trim().toLowerCase()))];
          countUnit = [...new Set(countUnit.map(e => Boolean(e) === true && e.trim().toLowerCase()))];

          if (countCode.length == productCodeList.length && countUnit.length == productUnitList.length) {
            this.listQuoteDetailExcelModel = [];

            listProductImport.forEach((row, i) => {
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
                /* dịch vụ */

                detailProduct.orderDetailType = 1;
                detailProduct.active = true;
                detailProduct.currencyUnit = moneyUnit.categoryId;
                detailProduct.discountType = item.DiscountType;
                detailProduct.exchangeRate = 1;
                detailProduct.incurredUnit = item.CurrencyUnit;
                detailProduct.nameMoneyUnit = moneyUnit.categoryCode;
                detailProduct.description = item.ProductName;
                detailProduct.productName = item.ProductName;
                detailProduct.quantity = item.Quantity;
                detailProduct.unitPrice = item.UnitPrice;
                detailProduct.vat = item.Tax;
                detailProduct.discountValue = item.DiscountValue;
                detailProduct.sumAmount = item.TotalAmount;
                detailProduct.priceInitial = 0;
                detailProduct.isPriceInitial = false;

                this.listQuoteDetailModel = [...this.listQuoteDetailModel, detailProduct];
                this.reOrderListQuoteDetail();
              }
              else {
                /* sản phẩm, hàng hóa */

                detailProduct.orderDetailType = 0;
                detailProduct.active = true;
                detailProduct.currencyUnit = moneyUnit.categoryId;
                detailProduct.discountType = item.DiscountType;
                detailProduct.exchangeRate = 1;
                detailProduct.incurredUnit = "CCCCC";
                detailProduct.nameMoneyUnit = moneyUnit.categoryCode;
                detailProduct.description = "";

                detailProduct.quantity = item.Quantity;
                detailProduct.unitPrice = item.UnitPrice;
                detailProduct.vat = item.Tax;
                detailProduct.discountValue = item.DiscountValue;
                detailProduct.sumAmount = item.TotalAmount;
                detailProduct.priceInitial = 0;
                detailProduct.isPriceInitial = false;

                let product = this.listProduct.find(p => p.productCode.trim() == item.ProductCode.trim());
                detailProduct.productId = product.productId;
                detailProduct.productCode = product.productCode;
                detailProduct.productName = item.ProductName;

                //Lấy đơn vị tính
                let productUnitId = product.productUnitId;
                let productUnitName = this.listUnitProduct.find(x => x.categoryId == productUnitId).categoryName;
                detailProduct.productNameUnit = productUnitName;
                detailProduct.unitId = productUnitId;

                this.listQuoteDetailModel = [...this.listQuoteDetailModel, detailProduct];
                this.reOrderListQuoteDetail();
              }
            });

            //set lại orderNumber
            this.listQuoteDetailModel.forEach((item, index) => {
              item.orderNumber = index + 1;
            });

            this.calculatorAmount();

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
          }
          if (countUnit.length != productUnitList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messUnitErr.forEach(item => {
              this.messErrFile.push('Đơn vị tính tại dòng ' + item + ' không tồn tại trong hệ thống')
            })
          }
        }
        this.displayDialog = false;
      }
    }
  }

  async exportExcel() {
    await this.getInforDetailQuote();
    let dateUTC = new Date();
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
      let productCode = "";
      let productName = "";
      let productQuantity = item.quantity;
      let productPriceAmount = item.unitPrice;
      let productUnit = item.productNameUnit;
      let productAmount = productQuantity * productPriceAmount;
      let productVat = item.vat;
      let productAmountVat = (productAmount * productVat) / 100;
      let productDiscountType = item.discountType ? "%" : "Tiền";
      let productDiscountValue = item.discountValue;
      let productSumAmount = item.sumAmount;
      if (item.productId !== null) {
        productCode = this.listProduct.find(p => p.productId == item.productId).productCode;
        productName = item.productName;
      }
      else {
        productName = item.description;
        productUnit = item.incurredUnit
      }

      /* Header row */
      let dataHeaderRowIndex = [index + 1, productCode, productName, productQuantity, productPriceAmount, productUnit, productAmount, productVat, productAmountVat, productDiscountType, productDiscountValue, productSumAmount];
      let headerRowIndex = worksheet.addRow(dataHeaderRowIndex);
      headerRowIndex.font = { name: 'Arial', size: 10 };
      dataHeaderRowIndex.forEach((item, index) => {
        headerRowIndex.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
        if (index == 1 || index == 2 || index == 5 || index == 9) {
          headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'left' };
        }
        if (index == 3 || index == 4 || index == 6 || index == 7 || index == 8 || index == 10 || index == 11) {
          headerRowIndex.getCell(index + 1).alignment = { vertical: 'bottom', horizontal: 'right' };
        }
      });
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

  /* Chuyển item lên một cấp */
  moveUp(data: QuoteDetail) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listQuoteDetailModel.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listQuoteDetailModel = this.listQuoteDetailModel.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listQuoteDetailModel = [...this.listQuoteDetailModel, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: QuoteDetail) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listQuoteDetailModel.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listQuoteDetailModel = this.listQuoteDetailModel.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listQuoteDetailModel = [...this.listQuoteDetailModel, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Thay đổi người phụ trách */
  changeParticipant() {
    if (this.participantControl.value?.length > 0) {
      this.tooltipSelectedParticipant = this.participantControl.value?.map(x => x.employeeCode).join(', ');
    }
    else {
      this.tooltipSelectedParticipant = null;
    }
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

  reOrderListQuoteDetail() {
    this.listQuoteDetailModel.forEach((item, index) => item.orderNumber = index + 1);
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /*Show popup các CTKM có thể áp dụng*/
  showPromotion(conditionsType: number, productId?: string, quantity?: number, productName?: string) {
    let titleHeader = '';
    let listReshowPromotionApply: Array<PromotionObjectApply> = [];
    
    if (conditionsType == 1) {
      titleHeader = 'Khuyến mãi dành cho ' + this.objCustomer.customerCodeName;
      listReshowPromotionApply = [...this.listPromotionApply];
    }
    else if (conditionsType == 2) {
      titleHeader = 'Khuyến mãi dành cho ' + productName;

      if (this.listPromotionApplyProduct.length > 0) {
        listReshowPromotionApply = [...this.listPromotionApply, ...this.listPromotionApplyProduct];
      }
    }
    else if (conditionsType == 3) {
      titleHeader = 'Khuyến mãi dành cho Tổng giá trị sản phẩm';
      listReshowPromotionApply = [...this.listPromotionApply];
    }
    
    let ref = this.dialogService.open(PromotionApplyPopupComponent, {
      data: { 
        conditionsType: conditionsType,
        customerId: this.objCustomer?.customerId,
        listReshowPromotionApply: listReshowPromotionApply,
        totalAmountAfterVat: this.totalAmountAfterVat,
        productId: productId,
        quantity: quantity
      },
      header: titleHeader,
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe(result => {
      if (result) {
        let selectedPromotionApply: Array<PromotionApply> = result;

        if (conditionsType == 1) {
          this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 1);

          selectedPromotionApply.forEach(item => {
            item.selectedPromotionProductApply.forEach(product => {
              let promotionApply = new PromotionObjectApply();
              promotionApply.promotionId = item.promotionId;
              promotionApply.promotionName = item.promotionName;
              promotionApply.conditionsType = item.conditionsType;
              promotionApply.propertyType = item.propertyType;
              promotionApply.propertyTypeName = item.propertyTypeName;
              promotionApply.notMultiplition = item.notMultiplition;
              promotionApply.promotionMappingId = product.promotionMappingId;
              promotionApply.productId = product.productId;
              promotionApply.productUnitName = product.productUnitName;
              promotionApply.promotionProductName = product.promotionProductName;
              promotionApply.promotionProductNameConvert = product.promotionProductNameConvert;
              promotionApply.soLuongTang = product.soLuongTang;
              promotionApply.loaiGiaTri = product.loaiGiaTri;
              promotionApply.giaTri = product.giaTri;
              promotionApply.soTienTu = product.soTienTu;
  
              this.listPromotionApply = [...this.listPromotionApply, promotionApply];
            });
          });
        }
        else if (conditionsType == 3) {
          this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 3);

          selectedPromotionApply.forEach(item => {
            item.selectedPromotionProductApply.forEach(product => {
              let promotionApply = new PromotionObjectApply();
              promotionApply.promotionId = item.promotionId;
              promotionApply.promotionName = item.promotionName;
              promotionApply.conditionsType = item.conditionsType;
              promotionApply.propertyType = item.propertyType;
              promotionApply.propertyTypeName = item.propertyTypeName;
              promotionApply.notMultiplition = item.notMultiplition;
              promotionApply.promotionMappingId = product.promotionMappingId;
              promotionApply.productId = product.productId;
              promotionApply.productUnitName = product.productUnitName;
              promotionApply.promotionProductName = product.promotionProductName;
              promotionApply.promotionProductNameConvert = product.promotionProductNameConvert;
              promotionApply.soLuongTang = product.soLuongTang;
              promotionApply.loaiGiaTri = product.loaiGiaTri;
              promotionApply.giaTri = product.giaTri;
              promotionApply.soTienTu = product.soTienTu;
  
              this.listPromotionApply = [...this.listPromotionApply, promotionApply];
            });
          });
        }
        else if (conditionsType == 2) {
          this.listPromotionApply = this.listPromotionApply.filter(x => x.conditionsType != 2);
          this.listPromotionApplyProduct = [];

          selectedPromotionApply.forEach(item => {
            //mua hàng tăng phiếu giảm giá
            if (item.propertyType == 3) 
            {
              item.selectedPromotionProductApply.forEach(product => {
                let promotionApply = new PromotionObjectApply();
                promotionApply.promotionId = item.promotionId;
                promotionApply.promotionName = item.promotionName;
                promotionApply.conditionsType = item.conditionsType;
                promotionApply.propertyType = item.propertyType;
                promotionApply.propertyTypeName = item.propertyTypeName;
                promotionApply.notMultiplition = item.notMultiplition;
                promotionApply.promotionMappingId = product.promotionMappingId;
                promotionApply.productId = product.productId;
                promotionApply.productUnitName = product.productUnitName;
                promotionApply.promotionProductName = product.promotionProductName;
                promotionApply.promotionProductNameConvert = product.promotionProductNameConvert;
                promotionApply.soLuongTang = product.soLuongTang;
                promotionApply.loaiGiaTri = product.loaiGiaTri;
                promotionApply.giaTri = product.giaTri;
                promotionApply.soTienTu = product.soTienTu;
    
                this.listPromotionApply = [...this.listPromotionApply, promotionApply];
              });
            }
            //mua hàng giảm giá hàng
            else if (item.propertyType == 1)
            {
              let promotionApply = new PromotionObjectApply();
              promotionApply.promotionId = item.promotionId;
              promotionApply.promotionName = item.promotionName;
              promotionApply.conditionsType = item.conditionsType;
              promotionApply.propertyType = item.propertyType;
              promotionApply.propertyTypeName = item.propertyTypeName;
              promotionApply.notMultiplition = item.notMultiplition;
              promotionApply.promotionMappingId = null;
              promotionApply.productId = null;
              promotionApply.productUnitName = null;
              promotionApply.promotionProductName = null;
              promotionApply.promotionProductNameConvert = null;
              promotionApply.soLuongTang = 0;
              promotionApply.loaiGiaTri = false;
              promotionApply.giaTri = 0;
              promotionApply.soTienTu = 0;
              promotionApply.selectedDetail = item.selectedDetail;

              promotionApply.selectedPromotionObjectApplyMapping = [];

              item.selectedPromotionProductMappingApply.forEach(product => {
                let selected = new PromotionObjectApplyMapping();

                selected.promotionMappingId = product.promotionMappingId;
                selected.productId = product.productId;
                selected.quantity = product.quantity;

                promotionApply.selectedPromotionObjectApplyMapping.push(selected);
              });

              this.listPromotionApplyProduct = [...this.listPromotionApplyProduct, promotionApply];
            }
            //mua hàng tặng hàng
            else if (item.propertyType == 2) {
              item.selectedPromotionProductMappingApply.forEach(product => {
                let promotionApply = new PromotionObjectApply();
                promotionApply.promotionId = item.promotionId;
                promotionApply.promotionName = item.promotionName;
                promotionApply.conditionsType = item.conditionsType;
                promotionApply.propertyType = item.propertyType;
                promotionApply.propertyTypeName = item.propertyTypeName;
                promotionApply.notMultiplition = item.notMultiplition;
                promotionApply.promotionMappingId = product.promotionMappingId;
                promotionApply.productId = product.productId;
                promotionApply.productUnitName = product.productUnitName;
                promotionApply.promotionProductName = product.productName;
                promotionApply.soLuongTang = product.quantity;
                promotionApply.loaiGiaTri = false;
                promotionApply.giaTri = 0;
                promotionApply.soTienTu = 0;
                promotionApply.selectedDetail = item.selectedDetail;

                this.listPromotionApply = [...this.listPromotionApply, promotionApply];
              });
            }
          });
        }

        this.calculatorAmount();
      }
    });
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
