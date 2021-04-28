import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import * as $ from 'jquery';

//MODELS
import { Quote } from '../../models/quote.model';
import { QuoteDetail } from "../../models/quote-detail.model";
import { QuoteDocument } from "../../models/quote-document.model";
import { QuoteProductDetailProductAttributeValue } from "../../models/quote-product-detail-product-attribute-value.model";

//SERVICES
import { BankService } from '../../../shared/services/bank.service';
import { QuoteService } from '../../services/quote.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { PromotionService } from '../../../promotion/services/promotion.service';

//DIALOG COMPONENT
import { AddEditProductDialogComponent } from '../add-edit-product-dialog/add-edit-product-dialog.component';
import { GetPermission } from '../../../shared/permission/get-permission';

//
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { MenuItem } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';

import { saveAs } from "file-saver";
import { Workbook } from 'exceljs';
import { DecimalPipe } from '@angular/common';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ProductService } from '../../../product/services/product.service';

import * as XLSX from 'xlsx';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
import { SendEmailQuoteComponent } from '../send-mail-popup-quote/send-mail-popup-quote.component';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { QuoteCostDetail } from '../../models/quote-cost-detail.model';
import { AddQuoteVendorDialogComponent } from '../add-quote-vendor-dialog/add-quote-vendor-dialog.component';
import { DatePipe } from '@angular/common';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import { PromotionApplyPopupComponent } from '../../../shared/components/promotion-apply-popup/promotion-apply-popup.component';
import { PromotionObjectApply } from '../../../promotion/models/promotion-object-apply.model';
import { PromotionApply } from '../../../promotion/models/promotion-apply.model';

interface Customer {
  customerId: string;
  customerCode: string;
  customerEmail: string;
  customerEmailWork: string;
  customerEmailOther: string;
  customerPhone: string;
  customerCompany: string;
  fullAddress: string;
  maximumDebtDays: number;
  maximumDebtValue: number;
  personInChargeId: string;
  customerGroupId: string;
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
  selector: 'app-customer-quote-detail',
  templateUrl: './customer-quote-detail.component.html',
  styleUrls: ['./customer-quote-detail.component.css'],
  providers: [
    DecimalPipe,
    DatePipe
  ]
})
export class CustomerQuoteDetailComponent implements OnInit {
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
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  isManager: boolean = localStorage.getItem('IsManager') == "true" ? true : false;
  /*End*/

  /*Get Current EmployeeId*/
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  /*End*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
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
  nameQuoteControl: FormControl; //tên báo giá
  sendDateControl: FormControl; //Ngày gửi
  effectiveDateControl: FormControl; //Ngày hiệu lực
  personInChargeControl: FormControl; //Người phụ trách
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
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  /*End*/

  /*Biến điều kiện*/
  colLeft: number = 8;
  isShow: boolean = true;
  displayRejectQuote: boolean = false;
  quoteId: string = null;
  statusCode: string = '';
  createQuoteFollow: number = 0;  //0: Tạo mới bình thường, 1: Tạo mới cho khách hàng, 2: Tạo mới cho khách hàng tiềm năng
  selectedObjectType: string = 'cus';
  isShowBankAccount: boolean = false;
  customerContactCode: string = 'CUS';
  rejectReason: string = 'EMP';
  uploadedFiles: any[] = [];
  cols: any[];
  colsCost: any[];
  selectedColumns: any[];
  selectedColumnsCost: any[];
  selectedItem: any;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  colsFile: any[];
  colsNote: any[];
  colsNoteFile: any[];
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

  listAction: MenuItem[];
  /*End*/

  /*Biến lưu giá trị trả về*/
  listIdCus: Array<string> = [];
  display: boolean = false;
  descriptionReject: string = '';
  workFollowQuote: MenuItem[];
  activeIndex: number = 0;
  isShowWorkFollowQuote: boolean = true;
  objectList = <any>[];
  objCustomer: Customer = null;
  leadObj: any = null;
  listPersonInCharge: Array<Employee> = []; //list người phụ trách
  listHoSoThau: Array<any> = []; //list hồ sơ thầu
  listCoHoi: Array<any> = []; //list cơ hội
  listKenh: Array<any> = []; //list kênh bán hàng
  listAdditionalInformationTemplate: Array<Category> = []; // Thông tin bổ sung mẫu được lưu trong category
  email: string = '';
  minDate: Date = new Date();
  phone: string = '';
  fullAddress: string = '';
  personInChargeId: string = '';
  listBankAccount: Array<BankAccount> = [];


  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  arrayQuoteDocumentModel: Array<QuoteDocument> = [];
  listQuoteDetailModel: Array<QuoteDetail> = [];
  listQuoteCostDetailModelOrderBy: Array<QuoteCostDetail> = [];
  listQuoteCostDetailModel: Array<QuoteCostDetail> = [];
  listQuoteDetailExcelModel: Array<QuoteDetailExcel> = [];
  listQuoteDetailModelOrderBy: Array<QuoteDetail> = [];
  arrayOrderProductDetailProductAttributeValueModel: Array<QuoteProductDetailProductAttributeValue> = [];
  inforExportExcel: InforExportExcel = {
    companyName: '',
    address: '',
    phone: '',
    website: '',
    email: '',
    textTotalMoney: ''
  }

  messageSendQuote: string = null;
  fileName: string = '';
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];
  productCodeSystemList: string[] = [];
  listProductUnitName: string[] = [];
  listProduct: any[] = [];
  listUnitProduct: any[] = [];
  listUnitMoney: Array<Category> = [];
  displayDialog: boolean = false;

  /*End*/

  /*MODELS*/
  quoteModel: Quote = new Quote();

  quoteCostDetailModel: QuoteCostDetail = new QuoteCostDetail();

  quoteDetailModel = new QuoteDetail();

  /*END*/

  /*NOTE*/
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];

  isApprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  /*End : Note*/

  /* Dialog Phê duyệt */
  displayReasonApprove: boolean = false;
  descriptionApprove: string = '';
  awaitResponseApprove: boolean = false;
  /*End: Dialog Phê duyệt */

  /* Mới */
  listInvestFund: Array<Category> = []; //list kênh bán hàng
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
  tooltipSelectedParticipant: string = null; //tooltip: danh sách nhân viên bán hàng đang được chọn
  /* End */

  /* Biến lưu thông tin báo giá */
  quoteCode: string = null;
  quoteDate: Date = null;
  expirationDate: Date = null; //Ngày hết hiệu lực của báo giá
  statusId: string = null;
  isSendQuote: boolean = false;
  /* End */

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

  isParticipant: boolean = null;
  isUserSendAproval: boolean = false; // có phải người gửi phê duyệt không

  /*Chương trình khuyến mãi*/
  isPromotionCustomer: boolean = false;
  isPromotionAmount: boolean = false;
  colsPromotion: any[];
  listPromotionApply: Array<PromotionObjectApply> = [];
  /*End*/

  constructor(
    private router: Router,
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
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe,
    private noteService: NoteService,
    private ref: ChangeDetectorRef,
    private promotionService: PromotionService,
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
      this.quoteId = params['quoteId'];

      /*Check permission*/
      let resource = "crm/customer/quote-detail";
      let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
      if (permission.status == false) {
        let msg = {
          severity: 'warn',
          summary: 'Thông báo:',
          detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ'
        };
        setTimeout(() => {
          this.showMessage(msg);
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 1000);
        }, 0);
      } else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("add") == -1) {
          this.actionAdd = false; //Thêm sản phẩm dịch vụ, Tạo đơn hàng
        }
        if (listCurrentActionResource.indexOf("import") == -1) {
          this.actionImport = false;  //import file upload
        }
        if (listCurrentActionResource.indexOf("delete") == -1) {
          this.actionDelete = false;  //Xóa sản phẩm dịch vụ, xóa file upload đã lưu
        }
        if (listCurrentActionResource.indexOf("edit") == -1) {
          //Sửa báo giá, sửa sản phẩm dịch vụ
          this.actionEdit = false;
        }
        this.setTable();
        this.getDataDefault();
      }
    });
    /*End*/
  }

  setForm() {
    this.listAction = [
      {
        label: 'Xuất excel', icon: 'pi pi-file', command: () => {
          this.exportExcel("EXCEL");
        }
      },
      {
        label: 'Xuất PDF', icon: 'pi pi-file', command: () => {
          this.exportPdf('download');
        }
      },
    ];

    this.objectControl = new FormControl(null, [Validators.required]);
    this.paymentMethodControl = new FormControl(null);
    this.bankAccountControl = new FormControl(null);
    this.daysAreOwedControl = new FormControl('0');
    this.maxDebtControl = new FormControl('0');
    this.quoteStatusControl = new FormControl(null);
    this.nameQuoteControl = new FormControl(null, [Validators.required]);
    this.intendedDateControl = new FormControl(new Date());
    this.sendDateControl = new FormControl(null);
    this.effectiveDateControl = new FormControl('30', [Validators.required, ageRangeValidator(1, 365)]);
    this.personInChargeControl = new FormControl(null, [Validators.required]);
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
      nameQuoteControl: this.nameQuoteControl,
      sendDateControl: this.sendDateControl,
      effectiveDateControl: this.effectiveDateControl,
      personInChargeControl: this.personInChargeControl,
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
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left', type: 'number' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left', type: 'date' },
    ];

    this.colsNote = [
      { field: 'title', header: 'Tiêu đề', width: '30%', textAlign: 'left' },
      { field: 'content', header: 'Nội dung', width: '70%', textAlign: 'left' },
    ];

    this.colsNoteFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left' },
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

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  getDataDefault() {
    this.loading = true;
    this.quoteService.getMasterDataUpdateQuote(this.quoteId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listInvestFund = result.listInvestFund;  //list kênh bán hàng
        this.listPaymentMethod = result.listPaymentMethod; //list Phương thức thanh toán
        this.listQuoteStatus = result.listQuoteStatus; //list Trạng thái của báo giá
        this.listEmployee = result.listEmployee;  //list nhân viên bán hàng
        this.listCustomer = result.listCustomer;  //list khách hàng định danh
        this.listCustomerNew = result.listCustomerNew; //list Khách hàng tiềm năng
        this.listAllLead = result.listAllLead; //list Hồ sơ thầu
        this.listAllSaleBidding = result.listAllSaleBidding; //list Hồ sơ thầu
        this.listParticipant = result.listParticipant; //list người tham gia
        this.isApprovalQuote = result.isApproval;
        this.isParticipant = result.isParticipant;  //có phải người tham gia hay không
        this.listPromotionApply = result.listPromotionObjectApply //quà khuyến mãi

        this.setDefaultValueForm(
          result.quote, //báo giá
          result.listQuoteDetail, //list sản phẩm
          result.listQuoteCostDetail, //list chi phí
          result.listQuoteDocument, //list file đính kèm
          result.listAdditionalInformation, //list thông tin bổ sung
          result.listNote, //list ghi chú (dòng thời gian)
          result.listParticipantId //list người tham gia của báo giá hiện tại
        );
        if (this.auth.UserId == result.quote.updatedById){
          this.isUserSendAproval = true;
        }
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });
  }

  async setDefaultValueForm(quote: Quote, listQuoteDetail: Array<QuoteDetail>, listQuoteCostDetail: Array<QuoteCostDetail>,
    listQuoteDocument: Array<QuoteDocument>, listAdditionalInformation: Array<AdditionalInformation>, listNote: Array<Note>,
    listParticipantId: Array<string>) {
    //Kiểm tra xem khách hàng của Cơ hội là Định danh hay Tiềm năng
    let customerType = this.getCustomerType(quote.objectTypeId);

    //Khách hàng Định danh
    if (customerType == 1) {
      this.selectedObjectType = 'cus';
      this.objectList = this.listCustomer;
      let selectedCustomer = this.listCustomer.find(x => x.customerId == quote.objectTypeId);
      await this.mapDataToQuote(selectedCustomer);
    }
    //Khách hàng Tiềm năng
    else if (customerType == 0) {
      this.selectedObjectType = 'lea';
      this.objectList = this.listCustomerNew;
      let selectedCustomer = this.listCustomerNew.find(x => x.customerId == quote.objectTypeId);
      await this.mapDataToQuote(selectedCustomer);
    }
    else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không xác định được khách hàng của Báo giá' };
      this.showMessage(mgs);
    }

    //Nếu đã xác định được khách hàng
    if (customerType == 0 || customerType == 1) {
      //Kiểm tra khách hàng có nhận được CTKM hay không
      let _result: any = await this.promotionService.checkPromotionByCustomer(quote.objectTypeId);
      this.loading = false;
      if (_result.statusCode == 200) {
        this.isPromotionCustomer = _result.isPromotionCustomer;
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: _result.messageCode };
        this.showMessage(mgs);
      }

      //Phương thức thanh toán
      let toSelectPaymentMethod: Category = this.listPaymentMethod.find(x => x.categoryId == quote.paymentMethod);
      this.paymentMethodControl.setValue(toSelectPaymentMethod);
      if (toSelectPaymentMethod) {
        if (toSelectPaymentMethod.categoryCode == "BANK") {
          this.isShowBankAccount = true;
          this.bankService.getAllBankAccountByObject(quote.objectTypeId, this.customerContactCode).subscribe(response => {
            var result = <any>response;

            this.listBankAccount = result.bankAccountList;

            if (this.listBankAccount.length > 0) {
              let toSelectBankAccount = this.listBankAccount.find(x => x.bankAccountId == quote.bankAccountId);
              if (toSelectBankAccount) {
                this.bankAccountControl.setValue(toSelectBankAccount);
              }
            }
          });
        } else if (toSelectPaymentMethod.categoryCode == "CASH") {
          this.isShowBankAccount = false;
          this.listBankAccount = [];
          this.bankAccountControl.setValue(null);
        }
      } else {
        const toSelectPaymentMethod = this.listPaymentMethod.find(c => c.isDefault === true);
        this.paymentMethodControl.setValue(toSelectPaymentMethod);
        this.isShowBankAccount = false;
        this.listBankAccount = [];
        this.bankAccountControl.setValue(null);
      }

      //Mã báo giá
      this.quoteCode = quote.quoteCode;

      //Tên báo giá
      this.nameQuoteControl.setValue(quote.quoteName);

      //Trạng thái
      let selectedStatus = this.listQuoteStatus.find(x => x.categoryId == quote.statusId);
      if (selectedStatus) {
        this.statusCode = selectedStatus.categoryCode;
        this.quoteStatusControl.setValue(selectedStatus);

        if (this.statusCode != 'MTA' || this.isParticipant) {
          this.nameQuoteControl.disable();
        }
      }

      this.configWorkflowSteps(selectedStatus);

      //Ngày tạo
      this.quoteDate = new Date(quote.quoteDate);

      //Ngày gửi dự kiến
      let intendedDate = quote.intendedQuoteDate ? new Date(quote.intendedQuoteDate) : null;
      this.intendedDateControl.setValue(intendedDate);

      //Ngày báo giá
      let sendDate = quote.sendQuoteDate ? new Date(quote.sendQuoteDate) : null;
      this.sendDateControl.setValue(sendDate);

      //Số ngày hiệu lực
      this.effectiveDateControl.setValue(quote.effectiveQuoteDate.toString());
      if (this.isParticipant) {
        this.effectiveDateControl.disable();
      }

      //Nhân viên bán hàng
      let personInCharge = this.listEmpSale.find(x => x.employeeId == quote.personInChargeId);
      if (personInCharge) {
        this.personInChargeControl.setValue(personInCharge);
      }
      else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể hiển thị lại Nhân viên bán hàng, kiểm tra phân quyền dữ liệu' };
        this.showMessage(mgs);
      }

      //Người tham gia
      let listSelectedParticipant = this.listParticipant.filter(x => listParticipantId.includes(x.employeeId));
      this.participantControl.setValue(listSelectedParticipant);
      if (listSelectedParticipant?.length > 0) {
        this.tooltipSelectedParticipant = listSelectedParticipant.map(x => x.employeeCode).join(', ');
      }

      //Lấy list Hồ sơ thầu theo khách hàng được chọn
      this.listSaleBidding = this.listAllSaleBidding.filter(x => x.customerId == quote.objectTypeId);

      /*Set value cho Hồ sơ thầu*/
      if (quote.saleBiddingId) {
        let selectedSalebidding = this.listSaleBidding.find(x => x.saleBiddingId == quote.saleBiddingId);
        if (selectedSalebidding) {
          this.hoSoThauControl.setValue(selectedSalebidding);
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể hiển thị lại Hồ sơ thầu, kiểm tra phân quyền dữ liệu' };
          this.showMessage(mgs);
        }
      }

      //Lấy list Cơ hội theo khách hàng được chọn
      this.listLead = this.listAllLead.filter(x => x.customerId == quote.objectTypeId);

      /*Set value cho Cơ hội*/
      if (quote.leadId) {
        let selectedLead = this.listLead.find(x => x.leadId == quote.leadId);
        if (selectedLead) {
          this.coHoiControl.setValue(selectedLead);
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Không thể hiển thị lại Cơ hội, kiểm tra phân quyền dữ liệu' };
          this.showMessage(mgs);
        }
      }

      //Kênh bán hàng
      if (quote.investmentFundId) {
        let investmentFund = this.listInvestFund.find(s => s.categoryId == quote.investmentFundId);
        this.kenhControl.setValue(investmentFund);
      }

      //Diễn giải
      this.descriptionControl.setValue(quote.description);
      if (this.statusCode != 'MTA' || this.isParticipant) {
        this.descriptionControl.disable();
      }

      //Ghi chú
      this.noteControl.setValue(quote.note);
      if (this.statusCode != 'MTA' || this.isParticipant) {
        this.noteControl.disable();
      }

      /* List File đính kèm: Quote Document */
      listQuoteDocument.forEach(item => {
        let quoteDocument = new QuoteDocument();
        quoteDocument.quoteDocumentId = item.quoteDocumentId;
        quoteDocument.quoteId = item.quoteId;
        quoteDocument.documentName = item.documentName;
        quoteDocument.documentSize = item.documentSize;
        quoteDocument.documentUrl = item.documentUrl;
        quoteDocument.createdById = item.createdById;
        quoteDocument.createdDate = item.createdDate;
        quoteDocument.updatedById = item.updatedById;
        quoteDocument.updatedDate = item.updatedDate;
        quoteDocument.active = item.active;

        this.arrayQuoteDocumentModel = [...this.arrayQuoteDocumentModel, quoteDocument];
        // Sắp xếp các file upload theo descending thời gian
        this.arrayQuoteDocumentModel.sort((a, b) => new Date(b.updatedDate).getTime() - new Date(a.updatedDate).getTime());
      });
      /*End: Quote Document*/

      /*List Sản phẩm*/
      this.listQuoteDetailModel = [];
      listQuoteDetail.forEach((item, index) => {
        let quoteDetailModel = new QuoteDetail();
        quoteDetailModel.quoteDetailId = item.quoteDetailId;
        quoteDetailModel.vendorId = item.vendorId;
        quoteDetailModel.quoteId = item.quoteId;
        quoteDetailModel.productId = item.productId;
        quoteDetailModel.quantity = item.quantity;
        quoteDetailModel.unitPrice = item.unitPrice;
        quoteDetailModel.currencyUnit = item.currencyUnit;
        quoteDetailModel.exchangeRate = item.exchangeRate;
        quoteDetailModel.vat = item.vat;
        quoteDetailModel.discountType = item.discountType;
        quoteDetailModel.discountValue = item.discountValue;
        quoteDetailModel.description = item.description;
        quoteDetailModel.orderDetailType = item.orderDetailType;
        quoteDetailModel.createdById = item.createdById;
        quoteDetailModel.createdDate = item.createdDate;
        quoteDetailModel.updatedById = item.updatedById;
        quoteDetailModel.updatedDate = item.updatedDate;
        quoteDetailModel.active = item.active;
        quoteDetailModel.unitId = item.unitId;
        quoteDetailModel.incurredUnit = item.incurredUnit;
        quoteDetailModel.productCode = item.productCode;
        quoteDetailModel.nameVendor = item.nameVendor;
        quoteDetailModel.productNameUnit = item.productNameUnit;
        quoteDetailModel.nameMoneyUnit = item.nameMoneyUnit;
        quoteDetailModel.sumAmount = item.sumAmount;
        quoteDetailModel.isPriceInitial = item.isPriceInitial;
        quoteDetailModel.priceInitial = item.priceInitial;
        quoteDetailModel.productName = item.productName;
        quoteDetailModel.orderNumber = item.orderNumber ? item.orderNumber : index + 1;

        var arrayAttributeValue = item.quoteProductDetailProductAttributeValue;
        if (arrayAttributeValue !== null) {
          arrayAttributeValue.forEach(_attr => {
            let orderProductDetailProductAttributeValue = new QuoteProductDetailProductAttributeValue();

            orderProductDetailProductAttributeValue.productId = _attr.productId;
            orderProductDetailProductAttributeValue.productAttributeCategoryId = _attr.productAttributeCategoryId;
            orderProductDetailProductAttributeValue.productAttributeCategoryValueId = _attr.productAttributeCategoryValueId;

            quoteDetailModel.quoteProductDetailProductAttributeValue.push(orderProductDetailProductAttributeValue);
          });
        }

        this.listQuoteDetailModel = [...this.listQuoteDetailModel, quoteDetailModel];
      });
      /*End*/

      /*List Thông tin chi phí*/
      this.listQuoteCostDetailModel = [];
      listQuoteCostDetail.forEach(item => {
        let quoteCostDetailModel = new QuoteCostDetail();
        quoteCostDetailModel.quoteCostDetailId = item.quoteCostDetailId;
        quoteCostDetailModel.costId = item.costId;
        quoteCostDetailModel.quoteId = item.quoteId;
        quoteCostDetailModel.quantity = item.quantity;
        quoteCostDetailModel.unitPrice = item.unitPrice;
        quoteCostDetailModel.costName = item.costName;
        quoteCostDetailModel.costCode = item.costCode;
        quoteCostDetailModel.createdById = item.createdById;
        quoteCostDetailModel.createdDate = item.createdDate;
        quoteCostDetailModel.updatedById = item.updatedById;
        quoteCostDetailModel.updatedDate = item.updatedDate;
        quoteCostDetailModel.active = item.active;
        quoteCostDetailModel.sumAmount = item.quantity * item.unitPrice;

        this.listQuoteCostDetailModel = [...this.listQuoteCostDetailModel, quoteCostDetailModel];
      });
      /*End*/

      /*List thông tin bổ sung*/
      this.listAdditionalInformation = [];
      listAdditionalInformation.forEach(item => {
        this.maxOrdinal++;
        let additionalInformation: AdditionalInformation = {
          ordinal: this.maxOrdinal,
          additionalInformationId: item.additionalInformationId,
          objectId: item.objectId,
          objectType: item.objectType,
          title: item.title,
          content: item.content,
          active: true,
          createdDate: new Date(),
          createdById: this.emptyGuid,
          updatedDate: null,
          updatedById: null,
          orderNumber: null
        };
        this.listAdditionalInformation = [...this.listAdditionalInformation, additionalInformation];
      });
      /*End*/

      /*List ghi chú (dòng thời gian)*/
      this.noteHistory = listNote;
      this.handleNoteContent();
      /*End*/

      /*Loại Chiết khấu*/
      let discountType = this.discountTypeList.find(x => x.value == quote.discountType);
      this.discountTypeControl.setValue(discountType);

      this.discountValueControl.setValue(quote.discountValue.toString());
      /*End*/

      this.calculatorAmount();
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
    }
    //Nếu khách hàng không có trong tập khách hàng của người đang đăng nhập
    else {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Khách hàng này không thuộc quyền phụ trách của bạn' };
      this.showMessage(mgs);
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
    /*End*/

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
    // this.listLead = this.listAllLead;
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
          var result = <any>response;

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
      this.loading = false;
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
    } else {
      this.isShowBankAccount = false;
      this.listBankAccount = [];
      this.bankAccountControl.setValue(null);
    }
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
        var result = <any>response;

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

  /* Cập nhật báo giá */
  updateQuote() {
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

        // Xóa file trong control
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();
        }
      }

      /*Binding data for quoteModel*/
      let quote = this.mapDataToModel();

      //Lấy list người tham gia
      let listSelectedParticipant: Array<Employee> = this.participantControl.value;
      let listParticipant = listSelectedParticipant?.map(x => x.employeeId) ?? [];

      this.loading = true;
      this.quoteService.CreateQuote(
        quote, this.listQuoteDetailModel, 1,
        this.arrayQuoteDocumentModel, this.listAdditionalInformation,
        this.listQuoteCostDetailModel, false, this.emptyGuid, listParticipant, this.listPromotionApply).subscribe(response => {

          let result = <any>response;
          this.loading = false;

          if (result.statusCode == 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Lưu báo giá thành công" };
            this.showMessage(mgs);
            this.resetForm();
            this.getDataDefault();
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
    }
  }

  mapDataToModel(): Quote {
    let quote = new Quote();

    quote.quoteId = this.quoteId;
    quote.quoteCode = this.quoteCode;
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

  cloneQuote() {
    let objClone = this.mapDataToModel();
    objClone.quoteId = this.emptyGuid;

    //Lấy list người tham gia
    let listSelectedParticipant: Array<Employee> = this.participantControl.value;
    let listParticipant = listSelectedParticipant.map(x => x.employeeId);

    this.quoteService.CreateQuote(objClone, this.listQuoteDetailModel, 1, this.arrayQuoteDocumentModel,
      this.listAdditionalInformation, this.listQuoteCostDetailModel,
      true, this.quoteId, listParticipant, this.listPromotionApply).subscribe(response => {
        let result = <any>response;
        this.loading = false;

        if (result.statusCode == 200) {
          let messageCode = "Nhân bản báo giá thành công";
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
          this.showMessage(mgs);
          this.quoteId = result.quoteID;
          this.router.navigate(['/customer/quote-detail', { quoteId: result.quoteID }]);
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
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
    this.amount = 0;
    this.customerOrderAmountAfterDiscount = 0;
    this.amountPriceInitial = 0;
    this.amountPriceCost = 0;
    this.amountPriceProfit = 0;
    this.valuePriceProfit = 0;
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

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: QuoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.arrayQuoteDocumentModel.indexOf(file);
        this.arrayQuoteDocumentModel.splice(index, 1);
      }
    });
  }

  /*Event khi xóa 1 file trong comment đã lưu trên server*/
  deleteNoteFile(file: NoteDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: QuoteDocument) {
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

  /* Tải file ở ghi chú (dòng thời gian) */
  downloadNoteFile(fileInfor: NoteDocument) {
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
    let rowData: AdditionalInformation = event.data;
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
  deleteAI(rowData: AdditionalInformation) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        let index = this.listAdditionalInformation.indexOf(rowData);
        this.listAdditionalInformation.splice(index, 1);
      }
    });
  }

  /*Tạo báo giá NCC*/
  async showDialogQuoteVendor() {
    if (this.statusCode === 'MTA') {
      await this.getInforDetailQuote();

      var listQuoteVendor: Array<QuoteDetail> = [];
      this.listQuoteDetailModel.forEach(item => {
        //Hiện tại chỉ báo giá ncc đối với sản phẩm
        if (item.orderDetailType == 0) {
          let obj: QuoteDetail = new QuoteDetail();
          obj.quoteDetailId = item.quoteDetailId;
          obj.vendorId = item.vendorId;
          obj.quoteId = item.quoteId;
          obj.productId = item.productId;
          obj.quantity = item.quantity;
          obj.unitPrice = item.unitPrice;
          obj.currencyUnit = item.currencyUnit;
          obj.exchangeRate = item.exchangeRate;
          obj.vat = item.vat;
          obj.discountType = item.discountType;
          obj.discountValue = item.discountValue;
          obj.description = item.description;
          obj.orderDetailType = item.orderDetailType;
          obj.unitId = item.unitId;
          obj.active = item.active;
          obj.createdById = item.createdById;
          obj.createdDate = item.createdDate;
          obj.updatedById = item.updatedById;
          obj.updatedDate = item.updatedDate;
          obj.quoteProductDetailProductAttributeValue = item.quoteProductDetailProductAttributeValue;
          obj.productCode = item.productCode;
          obj.productName = item.productName;
          obj.nameVendor = item.nameVendor;
          obj.productNameUnit = item.productNameUnit;
          obj.nameMoneyUnit = item.nameMoneyUnit;
          obj.sumAmount = item.sumAmount;
          obj.amountDiscount = item.amountDiscount;
          obj.priceInitial = item.priceInitial;
          obj.isPriceInitial = item.isPriceInitial;

          obj.productCode = this.listProduct.find(p => p.productId == item.productId).productCode;

          listQuoteVendor.push(obj);
        }
      });

      let ref = this.dialogService.open(AddQuoteVendorDialogComponent, {
        data: {
          listQuoteDetailModel: listQuoteVendor,
          quoteId: this.quoteId
        },
        header: 'Tạo đề nghị báo giá nhà cung cấp',
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

          }
        }
      });
    }
  }

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    if (this.statusCode === 'MTA') {
      let cusGroupId = null;
      if (this.objCustomer !== null && this.objCustomer !== undefined) cusGroupId = this.objCustomer.customerGroupId;
      let ref = this.dialogService.open(AddEditProductDialogComponent, {
        data: {
          isCreate: true,
          cusGroupId: cusGroupId,
          orderDate: this.quoteDate
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
  }

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit && this.statusCode === 'MTA') {
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
          orderDate: this.quoteDate
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
    if (this.actionEdit && this.statusCode === 'MTA') {
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

    /*
    * Nếu số thành tiền âm (vì nếu loại chiết khấu là % thì giá trị chiết khấu lớn nhất là 100%
    * nên số thành tiền không thể âm, vậy nếu số thành tiền âm thì chỉ có trường hợp giá trị
    * chiết khấu lúc này là Số tiền)
    */
    if (this.customerOrderAmountAfterDiscount < 0) {
      this.customerOrderAmountAfterDiscount = 0;
      this.discountValueControl.setValue(this.amount.toString());
    }
    /*End*/

    this.calculatorAmount();
  }

  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { });
  }

  goDetailLead() {
    this.router.navigate(['/lead/detail', { leadId: this.leadObj.leadId }]);
  }

  cancel() {
    this.router.navigate(['/customer/quote-list']);
  }

  createOrder() {
    this.router.navigate(['/order/create', { quoteID: this.quoteId }]);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  gotoCreateContract() {
    this.router.navigate(['/sales/contract-create', { quoteId: this.quoteId }]);
  }

  async exportExcel(type) {
    await this.getInforDetailQuote();
    this.quoteService.getDataExportExcelQuote(this.quoteId).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        var dateNow = new Date();
        // Láy tên của khách hàng hoặc khách hàng tiềm năng
        let cusName: string = '';
        if (this.selectedObjectType == 'cus') {
          let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
          cusName = customer.customerName;
        } else if (this.selectedObjectType == 'lea') {
          let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
          cusName = customer.customerName;
        }

        let DateCHO = "";
        let status = this.listQuoteStatus.find(s => s.categoryId == result.quote.statusId);
        if (status.categoryCode == 'CHO') {
          DateCHO = this.datePipe.transform(result.quote.updatedDate, 'dd/MM/yyyy');
        }
        // convert tên khách hàng trên tên báo giá theo format : ten_khách_hàng báo giá ngày xuất file
        let cusNameExport = convertStringCusName(cusName);

        let imgBase64 = this.getBase64Logo();
        let dateUTC = new Date();

        // getMonth() trả về index trong mảng nên cần cộng thêm 1
        let title = cusNameExport + ' báo giá ' + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
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
        dataRow1[3] = result.inforExportExcel.companyName.toUpperCase();  //Tên công ty
        let row1 = worksheet.addRow(dataRow1);
        row1.font = { name: 'Times New Roman', size: 10, bold: true };
        worksheet.mergeCells(`C${row1.number}:H${row1.number}`);
        row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow2 = [];
        dataRow2[3] = 'Địa chỉ: ' + result.inforExportExcel.address;  //Địa chỉ
        let row2 = worksheet.addRow(dataRow2);
        row2.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`C${row2.number}:H${row2.number}`);
        row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow3 = [];
        dataRow3[3] = 'Điện thoại: ' + result.inforExportExcel.phone;  //Số điện thoại
        let row3 = worksheet.addRow(dataRow3);
        row3.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`C${row3.number}:H${row3.number}`);
        row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow4 = [];
        dataRow4[3] = 'Email: ' + result.inforExportExcel.email;
        let row4 = worksheet.addRow(dataRow4);
        row4.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`C${row4.number}:H${row4.number}`);
        row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow5 = [];
        dataRow5[3] = 'Website dịch vụ: ' + result.inforExportExcel.website;  //Địa chỉ website
        let row5 = worksheet.addRow(dataRow5);
        row5.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
        worksheet.mergeCells(`C${row5.number}:H${row5.number}`);
        row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        worksheet.addRow([]);

        let dataRow7 = [];
        dataRow7[2] = '';
        let row7 = worksheet.addRow(dataRow7);
        worksheet.mergeCells(`A${row7.number}:H${row7.number}`);
        row7.getCell(1).border = { top: { style: "thin" } };

        worksheet.addRow([]);

        let dataRow9 = [];
        dataRow9[3] = 'BÁO GIÁ';
        let row9 = worksheet.addRow(dataRow9);
        row9.font = { name: 'Times New Roman', size: 16, bold: true };
        row9.height = 40;
        worksheet.mergeCells(`C${row9.number}:F${row9.number}`);
        row9.alignment = { vertical: 'middle', horizontal: 'center' };

        let dataRow10 = [];
        dataRow10[3] = 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear();
        let row10 = worksheet.addRow(dataRow10);
        row10.font = { name: 'Times New Roman', size: 10, bold: false };
        worksheet.mergeCells(`C${row10.number}:F${row10.number}`);
        row10.alignment = { vertical: 'middle', horizontal: 'center' };

        let dataRow11 = [];
        dataRow11[1] = 'Số phiếu:  ' + this.quoteCode;
        let row11 = worksheet.addRow(dataRow11);
        row11.font = { name: 'Times New Roman', size: 10, bold: false };
        worksheet.mergeCells(`A${row11.number}:H${row11.number}`);
        row11.alignment = { vertical: 'middle', horizontal: 'right' };

        let dataRow12 = [];
        dataRow12[2] = 'Kính gửi: ' + cusName;
        let row12 = worksheet.addRow(dataRow12);
        row12.font = { name: 'Times New Roman', size: 10, bold: true };
        worksheet.mergeCells(`B${row12.number}:H${row12.number}`);
        row12.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow13 = [];
        dataRow13[2] = 'Địa chỉ: ' + this.fullAddress;
        let row13 = worksheet.addRow(dataRow13);
        row13.font = { name: 'Times New Roman', size: 10, bold: true };
        worksheet.mergeCells(`B${row13.number}:H${row13.number}`);
        row13.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

        let dataRow14 = [];
        dataRow14[2] = 'SĐT: ' + this.phone;
        let row14 = worksheet.addRow(dataRow14);
        row14.font = { name: 'Times New Roman', size: 10, bold: true };
        worksheet.mergeCells(`B${row14.number}:H${row14.number}`);
        row14.alignment = { vertical: 'top', horizontal: 'left' };

        worksheet.addRow([]);

        let dataRow16 = [];
        dataRow16[8] = "Đơn vị tiền: VND";
        let row16 = worksheet.addRow(dataRow16);
        row16.font = { name: 'Times New Roman', size: 10 };
        row16.alignment = { vertical: 'top', horizontal: 'right', wrapText: true };

        /* Header row */
        let dataHeaderRow = ['STT', 'Tên sản phẩm/dịch vụ', 'Đơn vị', 'Số lượng', 'Đơn giá', 'Chiết khấu', 'VAT(%)', 'Thành tiền'];
        let headerRow = worksheet.addRow(dataHeaderRow);
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
        headerRow.height = 40;

        /* Data table */
        let data: Array<any> = []; //[1, 'Dịch vụ CNTT', 'Gói', '2', '6.000.000', '12.000.000']

        let totalNotVat = 0;  //Số tiền chưa VAT
        let totalVat = 0; // Tổng VÁT của tất cả sản phẩm

        // Lấy dữ liệu trực tiếp từ data ko cần mapping
        for (var index = 0; index < result.listQuoteDetail.length; ++index) {

          let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate;

          let row: Array<any> = [];
          row[0] = index + 1;
          row[1] = (result.listQuoteDetail[index].nameGene == null || result.listQuoteDetail[index].nameGene == '') ? result.listQuoteDetail[index].description : result.listQuoteDetail[index].nameGene;
          row[2] = (result.listQuoteDetail[index].discountType == 0) ? result.listQuoteDetail[index].nameProductUnit : result.listQuoteDetail[index].nameProductUnit;
          row[3] = this.decimalPipe.transform(result.listQuoteDetail[index].quantity).toString();
          row[4] = this.decimalPipe.transform((result.listQuoteDetail[index].unitPrice).toString());
          row[5] = this.decimalPipe.transform(result.listQuoteDetail[index].discountValue).toString();
          row[6] = this.decimalPipe.transform(result.listQuoteDetail[index].vat).toString();

          // Thành tiền từng sản phẩm sau chiết khấu và VAT
          if (result.listQuoteDetail[index].discountType) {
            // Thành tiền từng sản phẩm
            row[7] = this.decimalPipe.transform(((totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) +
              (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100)).toString());

            totalVat += (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100);
            totalNotVat += totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100);

          } else if (!result.listQuoteDetail[index].discountType) {
            // Thành tiền từng sản phẩm
            row[7] = this.decimalPipe.transform(((totalNotVatTemp - result.listQuoteDetail[index].discountValue)) +
              ((totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100))).toString();

            totalVat += (totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100);
            totalNotVat += totalNotVatTemp - result.listQuoteDetail[index].discountValue;
          }

          data.push(row);
        }

        data.forEach((el, index, array) => {
          let row = worksheet.addRow(el);
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
        });

        /* VAT */
        let dataVatRow = ['Tổng số tiền chưa VAT:', '', '', '', '', '', '', this.decimalPipe.transform(totalNotVat.toString())];
        let vatRow = worksheet.addRow(dataVatRow);
        worksheet.mergeCells(`A${vatRow.number}:G${vatRow.number}`);
        dataVatRow.forEach((item, index) => {
          vatRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 8) {
            vatRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            vatRow.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        vatRow.font = { name: 'Times New Roman', size: 11, bold: true };

        /* value VAT  */
        let dataValueVatRow = ['VAT :', '', '', '', '', '', '', this.decimalPipe.transform(totalVat.toString())];
        let valueVatRow = worksheet.addRow(dataValueVatRow);
        worksheet.mergeCells(`A${valueVatRow.number}:G${valueVatRow.number}`);
        dataValueVatRow.forEach((item, index) => {
          valueVatRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 8) {
            valueVatRow.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            valueVatRow.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        valueVatRow.font = { name: 'Times New Roman', size: 11, bold: true };

        let totalNotDicount = totalNotVat + totalVat;
        /* Số tiền chết khấu*/
        let discountMoney = 0;
        if (result.quote.discountType) {
          discountMoney = totalNotDicount * result.quote.discountValue / 100;
        } else {
          discountMoney = result.quote.discountValue;
        }
        let dataDiscountQouteMoney = ['Chiết khấu theo báo giá:', '', '', '', '', '', '', this.decimalPipe.transform(discountMoney.toString())];
        let discountQouteMoney = worksheet.addRow(dataDiscountQouteMoney);
        worksheet.mergeCells(`A${discountQouteMoney.number}:G${discountQouteMoney.number}`);
        dataDiscountQouteMoney.forEach((item, index) => {
          discountQouteMoney.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 8) {
            discountQouteMoney.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            discountQouteMoney.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        discountQouteMoney.font = { name: 'Times New Roman', size: 11, bold: true };

        /* Số tiền phải thanh toán */
        let total = totalNotDicount - discountMoney;
        let dataTotalMoney = ['Tổng số tiền phải thanh toán:', '', '', '', '', '', '', this.decimalPipe.transform(total.toString())];
        let totalMoney = worksheet.addRow(dataTotalMoney);
        worksheet.mergeCells(`A${totalMoney.number}:G${totalMoney.number}`);
        dataTotalMoney.forEach((item, index) => {
          totalMoney.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          if (index + 1 < 8) {
            totalMoney.getCell(1).alignment = { vertical: 'middle', horizontal: 'right' };
          } else {
            totalMoney.getCell(8).alignment = { vertical: 'middle', horizontal: 'right' };
          }
        });
        totalMoney.font = { name: 'Times New Roman', size: 11, bold: true };

        /* Số tiền bằng chữ */
        let dataStringMoney = ['Số tiền bằng chữ: ' + result.inforExportExcel.textTotalMoney + './.', '', '', '', '', '', '', ''];
        let stringMoney = worksheet.addRow(dataStringMoney);
        worksheet.mergeCells(`A${stringMoney.number}:H${stringMoney.number}`);
        dataStringMoney.forEach((item, index) => {
          stringMoney.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
          stringMoney.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'right' };
        });
        stringMoney.font = { name: 'Times New Roman', size: 11, bold: true };

        let rowFooterTable1 = [];
        rowFooterTable1[1] = 'Thời gian có hiệu lực:  ';
        rowFooterTable1[3] = '' + this.effectiveDateControl.value?.toString();
        rowFooterTable1[4] = 'Kể từ ngày';
        rowFooterTable1[5] = '' + DateCHO;
        let rowfooterTable1 = worksheet.addRow(rowFooterTable1);
        rowfooterTable1.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`A${rowfooterTable1.number}:B${rowfooterTable1.number}`);
        rowfooterTable1.alignment = { vertical: 'top', horizontal: 'left' };

        worksheet.addRow([]);

        result.listAdditionalInformation.forEach((item, index) => {
          let dataInfo1 = [];
          dataInfo1[1] = item.title;
          let infor1 = worksheet.addRow(dataInfo1);
          infor1.font = { name: 'Times New Roman', size: 11 };
          worksheet.mergeCells(`A${infor1.number}:H${infor1.number}`);
          infor1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

          let dataInfo2 = [];
          dataInfo2[1] = item.content;
          let infor2 = worksheet.addRow(dataInfo2);
          infor2.font = { name: 'Times New Roman', size: 11 };
          worksheet.mergeCells(`A${infor2.number}:H${infor2.number}`);
          infor2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };
        });

        worksheet.addRow([]);

        let dataFooter1 = [];
        dataFooter1[6] = 'Trân trọng cảm ơn Quý khách hàng';
        let footer1 = worksheet.addRow(dataFooter1);
        footer1.font = { name: 'Times New Roman', size: 11 };
        worksheet.mergeCells(`F${footer1.number}:H${footer1.number}`);
        footer1.alignment = { vertical: 'top', horizontal: 'left' };

        let dataFooter2 = [];
        let date = (new Date()).getDate();
        let month = (new Date()).getMonth() + 1;
        let fullYear = (new Date()).getFullYear();
        dataFooter2[6] = 'Hà Nội, Ngày ' + date + ' tháng ' + month + ' năm ' + fullYear;
        let footer2 = worksheet.addRow(dataFooter2);
        footer2.font = { name: 'Times New Roman', size: 11, bold: true };
        worksheet.mergeCells(`F${footer2.number}:H${footer2.number}`);
        footer2.alignment = { vertical: 'top', horizontal: 'left' };

        let dataFooter3 = [];
        dataFooter3[6] = result.inforExportExcel.companyName.toUpperCase();
        let footer3 = worksheet.addRow(dataFooter3);
        footer3.font = { name: 'Times New Roman', size: 11, bold: true };
        worksheet.mergeCells(`F${footer3.number}:H${footer3.number}`);
        footer3.alignment = { vertical: 'top', horizontal: 'left' };


        /* fix with for column */
        worksheet.getColumn(1).width = 5;
        worksheet.getColumn(2).width = 25;
        worksheet.getColumn(3).width = 10;
        worksheet.getColumn(4).width = 10;
        worksheet.getColumn(5).width = 13;
        worksheet.getColumn(6).width = 12;
        worksheet.getColumn(7).width = 8;
        worksheet.getColumn(8).width = 15;

        if (type === "EXCEL") {
          /*Export file excel*/
          this.exportToExel(workbook, title);
        }
        else {
          /*Export file excel*/
          this.exportToPdf(workbook, title);
        }
      }
    });
  }

  exportPdf(type: string) {
    this.quoteService.getDataExportExcelQuote(this.quoteId).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        let DateCHO = "";
        let status = this.listQuoteStatus.find(s => s.categoryId == result.quote.statusId);
        if (status.categoryCode == 'CHO') {
          DateCHO = this.datePipe.transform(result.quote.updatedDate, 'dd/MM/yyyy');
        }
        let dateNow = new Date();
        let imgBase64 = this.getBase64Logo();

        let customer: any = this.objectList.find(x => x.customerId == this.objCustomer.customerId);
        let cusName = customer.customerName;

        let totalNotVat = 0;  //Số tiền chưa VAT
        let totalVat = 0; // Tổng VÁT của tất cả sản phẩm

        // Lấy dữ liệu trực tiếp từ data ko cần mapping
        for (var index = 0; index < result.listQuoteDetail.length; ++index) {
          let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate;

          // Thành tiền từng sản phẩm sau chiết khấu và VAT
          if (result.listQuoteDetail[index].discountType) {
            totalVat += (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100);
            totalNotVat += totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100);
          } else if (!result.listQuoteDetail[index].discountType) {
            totalVat += (totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100);
            totalNotVat += totalNotVatTemp - result.listQuoteDetail[index].discountValue;
          }
        }

        let totalNotDicount = totalNotVat + totalVat;
        /* Số tiền chết khấu*/
        let discountMoney = 0;
        if (result.quote.discountType) {
          discountMoney = totalNotDicount * result.quote.discountValue / 100;
        } else {
          discountMoney = result.quote.discountValue;
        }

        /* Số tiền phải thanh toán */
        let total = totalNotDicount - discountMoney;

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
                          text: "" + result.inforExportExcel.companyName.toUpperCase(),
                          style: 'timer',
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Địa chỉ: ' + result.inforExportExcel.address,
                          style: 'timer',
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Điện thoại: ' + result.inforExportExcel.phone,
                          style: 'timer',
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Email: ' + result.inforExportExcel.email,
                          style: 'timer',
                          alignment: 'left'
                        },
                        {
                          text: '',
                          margin: [0, 2, 0, 2]
                        },
                        {
                          text: 'Website dịch vụ: ' + result.inforExportExcel.website,
                          style: 'timer',
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
              text: 'BÁO GIÁ',
              style: 'header',
              alignment: 'center'
            },
            {
              text: 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
              style: 'timer',
              alignment: 'center'
            },
            {
              text: 'Số phiếu:  ' + this.quoteCode,
              style: 'timer',
              alignment: 'right',
              margin: [0, 10, 0, 10]
            },
            {
              text: 'Kính gửi: ' + cusName,
              style: 'timer',
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              text: 'Địa chỉ: ' + this.fullAddress,
              style: 'timer',
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              text: 'SĐT: ' + this.phone,
              style: 'timer',
              alignment: 'left',
              margin: [0, 2, 0, 2]
            },
            {
              text: 'Đơn vị tiền: VND',
              style: 'timer',
              alignment: 'right'
            },
            {
              style: 'table',
              table: {
                widths: [20, 168, 49, 49, 49, 49, 45, 54],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'STT', style: 'tableHeader', alignment: 'center' },
                    { text: 'Tên sản phẩm/dịch vụ', style: 'tableHeader', alignment: 'center' },
                    { text: 'Đơn vị', style: 'tableHeader', alignment: 'center' },
                    { text: 'Số lượng', style: 'tableHeader', alignment: 'center' },
                    { text: 'Đơn giá', style: 'tableHeader', alignment: 'center' },
                    { text: 'Chiết khấu', style: 'tableHeader', alignment: 'center' },
                    { text: 'VAT(%)', style: 'tableHeader', alignment: 'center' },
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
                widths: [483, 54],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: 'Tổng số tiền chưa VAT:', style: 'tableHeader', alignment: 'right' },
                    { text: this.decimalPipe.transform(totalNotVat.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                  [
                    { text: 'VAT:', style: 'tableHeader', alignment: 'right' },
                    { text: this.decimalPipe.transform(totalVat.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                  [
                    { text: 'Chiết khấu theo báo giá:', style: 'tableHeader', alignment: 'right' },
                    { text: this.decimalPipe.transform(discountMoney.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
                  ],
                  [
                    { text: 'Tổng số tiền phải thanh toán:', style: 'tableHeader', alignment: 'right' },
                    { text: this.decimalPipe.transform(total.toString()), style: { fontSize: 9, bold: true }, alignment: 'right' },
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
                widths: [546],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                  [
                    { text: "Số tiền bằng chữ: " + result.inforExportExcel.textTotalMoney, style: { fontSize: 10, bold: false, italic: true }, alignment: 'right' },
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
              text: 'Thời gian có hiệu lực:  ' + this.effectiveDateControl.value.toString() + "    " + " Kể từ ngày: " + DateCHO,
              style: 'timer',
              alignment: 'left'
            },
            {
              style: 'table',
              table: {
                widths: ["auto"],
                headerRows: 1,
                dontBreakRows: true,
                body: [
                ]
              },
              layout: {
                defaultBorder: false,
              }
            },
            {
              columns: [
                {
                  width: '55%',
                  text: '',
                  style: { fontSize: 10, bold: true },
                  alignment: 'right'
                },
                {
                  width: '45%',
                  text: 'Trân trọng cảm ơn Quý khách hàng',
                  style: { fontSize: 10, bold: false },
                  alignment: 'center'
                }
              ]
            },
            {
              columns: [
                {
                  width: '55%',
                  text: '',
                  style: { fontSize: 10, bold: true },
                  alignment: 'right'
                },
                {
                  width: '45%',
                  text: 'Hà Nội, Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
                  style: { fontSize: 10, bold: false },
                  alignment: 'center'
                }
              ]
            },
            {
              columns: [
                {
                  width: '55%',
                  text: '',
                  style: { fontSize: 10, bold: true },
                  alignment: 'right'
                },
                {
                  width: '45%',
                  text: result.inforExportExcel.companyName.toUpperCase(),
                  style: { fontSize: 10, bold: true },
                  alignment: 'center'
                }
              ]
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

        result.listQuoteDetail.forEach((item, index) => {
          let col7 = "";
          let totalNotVatTemp = result.listQuoteDetail[index].quantity * result.listQuoteDetail[index].unitPrice * result.listQuoteDetail[index].exchangeRate;
          if (result.listQuoteDetail[index].discountType) {
            col7 = this.decimalPipe.transform(((totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) +
              (totalNotVatTemp - totalNotVatTemp * (result.listQuoteDetail[index].discountValue / 100)) * (result.listQuoteDetail[index].vat / 100)).toString());
          } else if (!result.listQuoteDetail[index].discountType) {
            // Thành tiền từng sản phẩm
            col7 = this.decimalPipe.transform(((totalNotVatTemp - result.listQuoteDetail[index].discountValue)) +
              ((totalNotVatTemp - result.listQuoteDetail[index].discountValue) * (result.listQuoteDetail[index].vat / 100))).toString();
          }
          let option = [
            { text: index + 1, style: 'tableLines', alignment: 'center' },
            {
              text: (item.nameGene == null || item.nameGene == '') ? item.description : item.nameGene,
              style: 'tableLines',
              alignment: 'left'
            },
            {
              text: (item.discountType == 0) ? item.nameProductUnit : item.nameProductUnit,
              style: 'tableLines',
              alignment: 'center'
            },
            {
              text: this.decimalPipe.transform(item.quantity).toString(),
              style: 'tableLines',
              alignment: 'right'
            },
            {
              text: this.decimalPipe.transform((item.unitPrice).toString()),
              style: 'tableLines',
              alignment: 'right'
            },
            {
              text: this.decimalPipe.transform(item.discountValue).toString(),
              style: 'tableLines',
              alignment: 'right'
            },
            {
              text: this.decimalPipe.transform(item.vat).toString(),
              style: 'tableLines',
              alignment: 'right'
            },
            { text: col7, style: 'tableLines', alignment: 'right' },
          ];
          documentDefinition.content[9].table.body.push(option);
        });

        if (result.listAdditionalInformation.length > 0) {
          result.listAdditionalInformation.forEach((item, index) => {
            let option1 =
              [
                {
                  text: item.title,
                  style: { fontSize: 10, bold: true },
                  alignment: 'left'
                }
              ]
            let option2 =
              [
                {
                  text: item.content,
                  style: 'timer',
                  alignment: 'left'
                }
              ]
            documentDefinition.content[15].table.body.push(option1);
            documentDefinition.content[15].table.body.push(option2);
          });
        } else {
          documentDefinition.content.splice(15, 1);
        }

        let cusNameExport = convertStringCusName(cusName);
        let title = cusNameExport + ' báo giá ' + dateNow.getDate() + '_' + (dateNow.getMonth() + 1) + '_' + dateNow.getUTCFullYear();

        if (type === 'download') {
          pdfMake.createPdf(documentDefinition).download(title + '.pdf');
        }
        else {
          pdfMake.createPdf(documentDefinition).getBase64(async function (encodedString) {
            localStorage.setItem('base64PDFQuote', encodedString);
          });
        }
      }
    });
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }

  exportToPdf(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/pdf' });
      saveAs.saveAs(blob, fileName);
    })
  }

  converToArrayQuoteDocument(fileList: File[]) {
    for (var x = 0; x < fileList.length; ++x) {
      let quoteDocument = new QuoteDocument();
      quoteDocument.documentName = fileList[x].name;
      quoteDocument.documentSize = fileList[x].size + '';
      quoteDocument.createdDate = convertToUTCTime(new Date());
      quoteDocument.updatedDate = convertToUTCTime(new Date());

      this.arrayQuoteDocumentModel = [...this.arrayQuoteDocumentModel, quoteDocument];
    }
  }

  // Event thay đổi nội dung ghi chú
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();
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
    if (this.uploadedNoteFiles.length > 0) {
      let listFileNameExists: Array<FileNameExists> = [];
      let result: any = await this.imageService.uploadFileForOptionAsync(this.uploadedNoteFiles, 'Quote');

      listFileNameExists = result.listFileNameExists;

      for (var x = 0; x < this.uploadedNoteFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedNoteFiles[x].name;
        let fileExists = listFileNameExists.find(f => f.oldFileName == this.uploadedNoteFiles[x].name);
        if (fileExists) {
          noteDocument.DocumentName = fileExists.newFileName;
        }
        noteDocument.DocumentSize = this.uploadedNoteFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }
    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.quoteId;
      noteModel.ObjectType = 'QUOTE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.quoteId;
      noteModel.ObjectType = 'QUOTE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    if (noteModel.Description == "" && this.listNoteDocumentModel.length == 0) {

      this.loading = false;
      return;
    }
    this.noteHistory = [];
    this.noteService.createNoteForQuoteDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        this.handleNoteContent();
        let messageCode = "Thêm ghi chú thành công";
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        this.showMessage(mgs);
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
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
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

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  exportExcelProduct() {
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
        productUnit = item.incurredUnit;
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

  validateFile(data) {
    /* function kiểm tra tính hợp lệ của data từ file excel */
    this.messErrFile = [];
    this.cellErrFile = [];

    data.forEach((row, i) => {
      if (i > 4) {
        if ((row[1] === null || row[1] === undefined || row[1].toString().trim() == "") && (row[2] === null || row[2] === undefined || row[2].toString().trim() == "")) {
          this.messErrFile.push('Dòng { ' + (i + 2) + ' } chưa nhập Mã sản phẩm hoặc Tên sản phẩm!');
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

  async showDialogImport() {
    await this.getInforDetailQuote();
    this.displayDialog = true;
  }

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

  /* Xóa báo giá */
  delQuote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.quoteService.updateActiveQuote(this.quoteId).subscribe(response => {
          let result: any = response;
          if (result.statusCode == 200) {
            setTimeout(() => {
              this.router.navigate(['/customer/quote-list']);
            }, 500);
          }
        });
      }
    });
  }

  /* Thay đổi trạng thái báo giá */
  updateStatusQuoteAprroval(objectTyppe: string) {
    this.messageSendQuote = null;
    this.loading = true;
    if (objectTyppe == 'APPROVAL_QUOTE' && this.selectedObjectType == 'lea') {
      this.messageSendQuote = `Vui lòng chuyển khách hàng thành định danh!`;
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.loading = false;
    }
    else {
      this.isInvalidForm = false;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = false;  //Hiển thị message lỗi
      this.quoteService.updateStatusQuote(this.quoteId, this.auth.UserId, objectTyppe).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.resetForm();
          this.getDataDefault();
          if (objectTyppe !== 'NEW_QUOTE') {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
    }
  }

  /* Gửi báo giá cho khách hàng */
  sendQuote() {
    let base64 = localStorage.getItem('base64PDFQuote');
    let emailList = null;
    let objectType: Customer = this.objectControl.value;
    if (objectType.customerEmail !== null && objectType.customerEmail !== undefined && objectType.customerEmail.trim() != "") {
      emailList = objectType.customerEmail + "; ";
    }
    if (objectType.customerEmailOther !== null && objectType.customerEmailOther !== undefined && objectType.customerEmailOther.trim() != "") {
      emailList = emailList + objectType.customerEmailOther + "; ";
    }
    if (objectType.customerEmailWork !== null && objectType.customerEmailWork !== undefined && objectType.customerEmailWork.trim() != "") {
      emailList = emailList + objectType.customerEmailWork + "; ";
    }

    let ref = this.dialogService.open(SendEmailQuoteComponent, {
      data: {
        sendTo: emailList,
        quoteId: this.quoteId,
        customerId: this.objectControl.value.customerId,
        quoteCode: this.quoteCode,
        quoteMoney: this.customerOrderAmountAfterDiscount,
        customerCompany: objectType.customerCompany,
        base64: base64
      },
      header: 'Gửi Email',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Gửi email thành công" };
        this.showMessage(mgs);
      }
    });
  }

  /* Confirm trước khi Phê duyệt hoặc Từ chối */
  approveOrReject(isApprove) {
    this.listIdCus.push(this.quoteId);
    if (isApprove !== null) {
      if (isApprove) {
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn phê duyệt báo giá này?',
          accept: () => {
            this.displayReasonApprove = true;
            this.awaitResponseApprove = false;
          }
        });
      }
      else {
        this.rejectReason = "EMP";
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn từ chối báo giá này?',
          accept: () => {
            this.display = true;
          }
        });
      }
    }
  }
  // Hủy báo giá ở trạng thái đã duyệt
  rejectQuote() {
    this.listIdCus.push(this.quoteId);
    this.displayRejectQuote = true;
  }

  confirmRejectOrder(agree: boolean) {
    if (agree) {
      this.displayRejectQuote = false;
      this.display = true;
    } else {
      this.displayRejectQuote = false;
      this.rejectReason = 'EMP';
    }
  }

  /* Từ chối báo giá */
  rejectOrder() {
    this.loading = true;
    this.quoteService.approvalOrRejectQuote(this.listIdCus, false, this.auth.UserId, this.descriptionReject, this.rejectReason).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.listIdCus = [];
      this.descriptionReject = '';
      // this.rejectReason = 'EMP';
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.resetForm();
        this.getDataDefault();

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
      this.display = false;
    }, error => { this.loading = false; });
  }

  /* Phê duyệt báo giá */
  approveOrder() {
    this.loading = true;
    this.awaitResponseApprove = true;
    this.quoteService.approvalOrRejectQuote(this.listIdCus, true, this.auth.UserId, this.descriptionApprove, "").subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.listIdCus = [];
      this.descriptionApprove = '';

      if (result.statusCode === 202 || result.statusCode === 200) {
        this.resetForm();
        this.getDataDefault();
        this.displayReasonApprove = false;

        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Phê duyệt báo giá thành công" };
        this.showMessage(mgs);
      } else {
        this.displayReasonApprove = false;
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    }, error => { this.loading = false; });
  }

  /* Đóng dialog lý do phê duyệt báo giá */
  closeDialogReasonApprove() {
    this.descriptionApprove = '';
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

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

  reOrderListQuoteDetail() {
    this.listQuoteDetailModel.forEach((item, index) => item.orderNumber = index + 1);
    this.listQuoteDetailModel.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  configWorkflowSteps(currentStatus: Category) {
    /* workflow ứng với luồng trạng thái phê duyệt
       Mới tạo(MTA) => Chờ duyệt(DLY) => Đã duyệt(CHO) => Báo giá(DTH) => Đóng(DON)
    */
    let listStatusCodeWorkflow1 = ["MTA", "DLY", "CHO", "DTH", "DON"];
    /* workflow ứng với luồng trạng thái từ chối
       Mới tạo(MTA) => Chờ duyệt(DLY) => Từ chối(TUCHOI) => Hủy(HUY)
    */
    let listStatusCodeWorkflow2 = ["MTA", "DLY", "TUCHOI", "HUY"];

    let listStatusCodeWorkflow3 = ["MTA", "DLY", "CHO", "DTR"];

    let listStatusCodeWorkflow = [];

    if (listStatusCodeWorkflow1.includes(currentStatus.categoryCode)) {
      listStatusCodeWorkflow = listStatusCodeWorkflow1;
    } else if (listStatusCodeWorkflow2.includes(currentStatus.categoryCode)) {
      listStatusCodeWorkflow = listStatusCodeWorkflow2;
    } else if (listStatusCodeWorkflow3.includes(currentStatus.categoryCode)) {
      listStatusCodeWorkflow = listStatusCodeWorkflow3;
    }

    const listStatusWorkflow1 = this.listQuoteStatus.filter(e => listStatusCodeWorkflow.includes(e.categoryCode)).sort((x, y) => {
      //sắp xếp theo index của mảng listStatusCodeWorkflow1
      let xIndex = listStatusCodeWorkflow.findIndex(_statusCode => _statusCode == x.categoryCode);
      let yIndex = listStatusCodeWorkflow.findIndex(_statusCode => _statusCode == y.categoryCode);
      return xIndex - yIndex;
    });
    this.workFollowQuote = [];

    listStatusWorkflow1.forEach(e => {
      let newStep: any = { label: e.categoryName };
      this.workFollowQuote = [...this.workFollowQuote, newStep];
    });

    this.activeIndex = listStatusCodeWorkflow.findIndex(e => e == currentStatus.categoryCode) || 0;
  }

  /* reset workflow nếu nhấn lưu và thêm mới: ??? */
  resetWorkFollowQuote() {
    let listStatusCodeWorkflow1 = [];
    /* workflow ứng với trạng thái Mới tạo, Chờ duyệt, Đã duyệt
      Mới tạo(MTA) => Chờ duyệt(DLY) => Đã duyệt(CHO) => Báo giá(DTH) => Đóng(DON) => Hủy(HUY)
     */
    listStatusCodeWorkflow1 = ["MTA", "DLY", "CHO", "DTH", "DON", "HUY"];
    const listStatusWorkflow1 = this.listQuoteStatus.filter(e => listStatusCodeWorkflow1.includes(e.categoryCode)).sort((x, y) => {
      //sắp xếp theo index của mảng listStatusCodeWorkflow1
      let xIndex = listStatusCodeWorkflow1.findIndex(_statusCode => _statusCode == x.categoryCode);
      let yIndex = listStatusCodeWorkflow1.findIndex(_statusCode => _statusCode == y.categoryCode);
      return xIndex - yIndex;
    });
    this.workFollowQuote = [];
    listStatusWorkflow1.forEach(e => {
      let newStep: any = { label: e.categoryName };
      this.workFollowQuote = [...this.workFollowQuote, newStep];
    });
    this.activeIndex = 0;
  }

  /* Thay đổi người tham gia */
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

  /*Show popup các CTKM có thể áp dụng*/
  showPromotion(conditionsType: number, productId?: string, quantity?: number, productName?: string) {
    let titleHeader = '';
    if (conditionsType == 1) {
      titleHeader = 'Khuyến mãi dành cho ' + this.objCustomer.customerCodeName;
    }
    else if (conditionsType == 2) {
      titleHeader = 'Khuyến mãi dành cho ' + productName;
    }
    else if (conditionsType == 3) {
      titleHeader = 'Khuyến mãi dành cho Tổng giá trị sản phẩm';
    }

    let ref = this.dialogService.open(PromotionApplyPopupComponent, {
      data: {
        conditionsType: conditionsType,
        customerId: this.objCustomer?.customerId,
        listReshowPromotionApply: this.listPromotionApply,
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

          selectedPromotionApply.forEach(item => {
            if (item.propertyType == 3) {
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
            else {

            }
          });
        }

        this.calculatorAmount();
      }
    });
  }
}

function convertStringCusName(str: string) {
  if (str.includes("-", 0)) {
    str = str.replace("-", " ");
  }
  while (str.includes("  ", 0)) {
    str = str.replace("  ", " ");
  }
  while (str.includes(" ", 0)) {
    str = str.replace(" ", "_");
  }
  return str;
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
