import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { Contract, ContractCostDetail, ContractDetail, ContractProductDetailProductAttributeValue } from '../../models/contract.model';
import { AdditionalInformationModel } from '../../../shared/models/additional-information.model';

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
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteService } from '../../../shared/services/note.service';
import { NoteModel } from '../../../shared/models/note.model';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { LeadService } from '../../../lead/services/lead.service';
import { AddEditContractCostDialogComponent } from '../add-edit-contract-cost-dialog/add-edit-contract-cost-dialog.component';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';

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

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Component({
  selector: 'app-contract-detail',
  templateUrl: './contract-detail.component.html',
  styleUrls: ['./contract-detail.component.css']
})
export class ContractDetailComponent implements OnInit {
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
  currentEmployeeId = this.auth.EmployeeId;  //employeeId của người đang đăng nhập
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  /*End*/
  path: string = "#";
  uploadedFiles: any[] = [];
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  isShowButtonSave: boolean = true;
  isPersonInCharge: boolean = false;
  /*End*/
  selectedItem: any;
  quoteId = this.emptyGuid;
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
  contractCodeControl: FormControl; // Mã hợp đồng

  // Form Gia hạn
  extendContractForm: FormGroup;
  appendixContractCodeControl: FormControl;
  appendixContractTimeControl: FormControl;
  appendixContractTimeUnitControl: FormControl;

  contractCostDetailModel: ContractCostDetail;
  listContractCost: Array<ContractCostDetail> = [];
  cols: any[];
  selectedColumns: any[];
  colsNote: any[];
  colsCost: any[];
  colsAppendixContract: any;
  selectedColumnsCost: any[];
  listContractTimeUnit: Array<any> = [];
  isShowCreateOrder: boolean = true;
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
  createdDate = new Date();
  maxOrdinal: number = 0;
  colsFile: any[];
  /*Biến lưu giá trị trả về*/
  customerGroupId: string;
  activeIndex: number = 0;
  isShowWorkFollowContract: boolean = true;
  objectList = <any>[];
  listCustomer: Array<any> = [];
  listCustomerMaster: Array<any> = [];
  listBankAccount: Array<BankAccount> = [];
  listBankAccountCus: Array<BankAccount> = [];
  listTypeContract: Array<Category> = [];
  listQuoteMaster: Array<Quote> = [];
  listQuote: Array<Quote> = [];
  listQuoteDetail: Array<any> = [];
  listQuoteCostDetail: Array<any> = [];
  listContract: Array<any> = [];
  listAppendixContract: Array<any> = [];
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
  DiscountValue: number = 0;
  viewModeCode: string = null;
  contractCode: string = null;
  isRequiredCustomerOrder: boolean = false;
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

  /*NOTE*/
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];

  isAprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  /*End : Note*/
  file: File[];
  listFile: Array<FileInFolder> = [];
  // Biến
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  colLeft: number = 8;
  isShow: boolean = true;
  contractId: string = this.emptyGuid;
  customerId: string = this.emptyGuid;
  email: string = '';
  phone: string = '';
  fullAddress: string = '';
  taxCode: string = '';
  numberPayMentChange: string = '';
  CustomerOrderAmount: number = 0;
  DiscountValueVnd: number = 0;
  CustomerOrderAmountAfterDiscount: number = 0;


  /*Biến điều kiện*/
  minDate: Date;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  isShowBankAccount: boolean = false;
  isEdit: boolean = true;
  bankName: string;
  bankAccount: string;
  bankNumber: string;
  branch: string;
  listContractDetailModel: Array<ContractDetail> = [];
  listContractDetailModelOrderBy: Array<ContractDetail> = [];
  leadId: string;
  leadCode: string;
  saleBiddingId: string;
  saleBiddingCode: string;

  contractDetailModel = new ContractDetail();
  isDay: boolean = true;
  isOutOfDate: boolean = false;
  isShowDialog: boolean = false;
  customerName: string;
  isExtend: boolean = false;

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
    StatusId: this.emptyGuid,//Guid?
    Active: true,
    CreatedById: this.auth.UserId,
    CreatedDate: new Date(),
    UpdatedById: this.emptyGuid,
    UpdatedDate: new Date(),
    IsExtend: false,
  }

  constructor(
    private router: Router,
    private el: ElementRef,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private contractService: ContractService,
    private noteService: NoteService,
    private imageService: ImageUploadService,
    private leadService: LeadService,
    private folderService: ForderConfigurationService,
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

  async ngOnInit() {
    this.listContractTimeUnit = [
      { timeUnit: 'Ngày', code: 'DAY' },
      { timeUnit: 'Tháng', code: 'MONTH' },
      { timeUnit: 'Năm', code: 'YEAR' }
    ]
    this.setForm();
    this.route.params.subscribe(params => {
      // //Nếu tạo mới báo giá
      // this.isShowCreateOrder = false;
      this.contractId = params['contractId'];
      let resource = "sal/sales/contract-detail";
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
        this.setTable();
        this.getMasterData();
        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("edit") == -1) {
          this.actionEdit = false;
        }
        if (listCurrentActionResource.indexOf("add") == -1) {
          this.actionAdd = false;
        } else {
          //Khi người dùng click vào link tạo mới báo giá thì show lại
          this.actionAdd = true;
        }
      }
    });
  }

  setForm() {
    this.contractCodeControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
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

    this.appendixContractCodeControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.appendixContractTimeControl = new FormControl('');
    this.appendixContractTimeUnitControl = new FormControl('');

    this.contractForm = new FormGroup({
      contractCodeControl: this.contractCodeControl,
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
    });

    this.extendContractForm = new FormGroup({
      appendixContractCodeControl: this.appendixContractCodeControl,
      appendixContractTimeControl: this.appendixContractTimeControl,
      appendixContractTimeUnitControl: this.appendixContractTimeUnitControl,
    });
  }

  isShowButton: boolean = true;

  setDefault() {
    this.expiredDateControl.disable();

    if (this.isManager) {
      this.isShowButton = true;
    } else {
      this.isShowButton = false;
    }
    let quote = this.listQuote.find(c => c.quoteId == this.contractModel.QuoteId);
    this.quoteControl.setValue(quote);

    if (quote) {
      this.leadId = quote.leadId;
      this.leadCode = quote.leadCode;
      this.saleBiddingId = quote.saleBiddingId;
      this.saleBiddingCode = quote.saleBiddingCode;
    }

    this.contractCodeControl.setValue(this.contractModel.ContractCode);
    let customer = this.objectList.find(c => c.customerId == this.contractModel.CustomerId);
    this.objectControl.setValue(customer);
    this.customerGroupId = customer?.customerGroupId;
    this.phone = customer.customerPhone;
    this.email = customer.customerEmail;
    this.fullAddress = customer.fullAddress;
    this.taxCode = customer.taxCode;
    if (customer.personInChargeId) {
      this.leadService.getEmployeeSale(this.listEmpSale, customer.personInChargeId).subscribe(response => {
        let result: any = response;
        this.listEmpSaleCus = result.listEmployee;
        this.listEmpSaleCus.forEach(item => {
          item.employeeName = item.employeeName;
        });
        let employee = this.listEmpSaleCus.find(c => c.employeeId == this.contractModel.EmployeeId);
        if (employee) {
          this.empSalerControl.setValue(employee);
        }
      });

      //Hợp đồng chính
      let mainContract = this.listContract.find(x => x.contractId == this.contractModel.ContractId);
      this.mainContractControl.setValue(mainContract);
    } else {
      this.listEmpSaleCus = [];
    }

    let paymentMethod = this.listPaymentMethod.find(c => c.categoryId == this.contractModel.PaymentMethodId);
    this.paymentMethodControl.setValue(paymentMethod);
    if (paymentMethod) {
      this.contractForm.controls['paymentMethodControl'].setValue(paymentMethod);
      if (paymentMethod.categoryCode === 'BANK') {
        this.isShowBankAccount = true;
      } else {
        this.isShowBankAccount = false;
      }
    }
    this.listBankAccountCus = this.listBankAccount.filter(c => c.objectId == customer.customerId);
    let bankAccount = this.listBankAccount.find(c => c.bankAccountId == this.contractModel.BankAccountId);
    this.bankAccountControl.setValue(bankAccount);
    if (bankAccount) {
      this.isShowBankAccount = true;
      this.bankName = bankAccount.bankName;
      this.bankAccount = bankAccount.accountName;
      this.bankNumber = bankAccount.accountNumber;
      this.branch = bankAccount.branchName;
    }

    this.effectiveDateControl.setValue(new Date(this.contractModel.EffectiveDate));

    if (this.contractModel.ExpiredDate) {
      this.expiredDateControl.setValue(new Date(this.contractModel.ExpiredDate));
    } else {
      this.expiredDateControl.setValue(null);
    }

    this.contractTimeControl.setValue(this.contractModel.ContractTime)

    if (this.contractModel.ContractTimeUnit) {
      this.contractTimeUnitControl.setValue(this.listContractTimeUnit.find(x => x.code == this.contractModel.ContractTimeUnit));
    } else {
      this.contractTimeUnitControl.setValue(this.listContractTimeUnit.find(x => x.code == 'DAY'));
    }

    this.valueContractControl.setValue(this.contractModel.ValueContract);
    let mainContract = this.listContract.find(c => c.contractId == this.contractModel.MainContractId);
    this.mainContractControl.setValue(mainContract);
    let typeContract = this.listTypeContract.find(c => c.categoryId == this.contractModel.ContractTypeId);
    this.typeContractControl.setValue(typeContract);

    if (typeContract.categoryCode === 'HDNT') {
      this.mainContractControl.disable();
    }


    this.descriptionControl.setValue(this.contractModel.ContractDescription);
    this.noteControl.setValue(this.contractModel.ContractNote);
    let discountType = this.discountTypeList.find(x => x.value == this.contractModel.DiscountType);
    this.discountTypeControl.setValue(discountType);
    this.discountValueControl.setValue(this.contractModel.DiscountValue);
    this.CustomerOrderAmount = this.contractModel.Amount;
    if (discountType.value) {
      //Nếu theo %
      this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - (this.contractModel.Amount * this.contractModel.DiscountValue) / 100;
      this.DiscountValue = (this.contractModel.Amount * this.contractModel.DiscountValue) / 100;
    } else {
      //Nếu theo Số tiền
      this.CustomerOrderAmountAfterDiscount = this.contractModel.Amount - this.contractModel.DiscountValue;
      this.DiscountValue = this.contractModel.DiscountValue;
    }
    this.valueContractControl.setValue(this.CustomerOrderAmountAfterDiscount);
    let toSelectContractStatus: Category = this.listContractStatus.find(x => x.categoryId == this.contractModel.StatusId);
    if (toSelectContractStatus.categoryCode == 'MOI' || toSelectContractStatus.categoryCode == 'APPR' || toSelectContractStatus.categoryCode == 'DTH') {
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
        },
        {
          label: this.listContractStatus.find(x => x.categoryCode == 'HUY').categoryName
        }
      ];

      switch (toSelectContractStatus.categoryCode) {
        case 'MOI': {
          this.activeIndex = 0;
          break;
        }
        case 'APPR': {
          this.activeIndex = 1;
          break;
        }
        case 'DTH': {
          this.activeIndex = 2;
          break;
        }
        default: {
          break;
        }
      }
    } else {
      this.workFollowContract = [
        {
          label: this.listContractStatus.find(x => x.categoryCode == 'MOI').categoryName
        },
        {
          label: this.listContractStatus.find(x => x.categoryCode == 'APPR').categoryName
        },
        {
          label: this.listContractStatus.find(x => x.categoryCode == toSelectContractStatus.categoryCode).categoryName
        }
      ];
      this.activeIndex = 2;
    }
  }

  showTotalContract() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  mappingForm() {
    this.contractModel.ContractCode = this.contractForm.controls['contractCodeControl'].value.trim();
    this.contractModel.EmployeeId = this.contractForm.controls['empSalerControl'].value.employeeId;
    this.contractModel.EffectiveDate = convertToUTCTime(this.contractForm.controls['effectiveDateControl'].value);
    if(this.expiredDateControl.value != null){
      this.contractModel.ExpiredDate = convertToUTCTime(this.contractForm.controls['expiredDateControl'].value);
    }
    this.contractModel.ContractTime = parseInt(this.contractForm.controls['contractTimeControl'].value.replace(/,/g, ''));
    this.contractModel.ContractTimeUnit = this.contractForm.controls['contractTimeUnitControl'].value.code;
    let contractDescription = this.contractForm.controls['descriptionControl'].value;
    this.contractModel.ContractDescription = contractDescription != null ? contractDescription.trim() : '';
    let contractNote = this.contractForm.controls['noteControl'].value;
    this.contractModel.ContractNote = contractNote != null ? contractNote.trim() : '';
    this.contractModel.PaymentMethodId = this.contractForm.controls['paymentMethodControl'].value.categoryId;
    this.contractModel.CustomerId = this.contractForm.controls['objectControl'].value.customerId;
    this.contractModel.ObjectType = 'CUSTOMER';
    this.contractModel.QuoteId = this.contractForm.controls['quoteControl'].value?.quoteId;
    this.contractModel.ValueContract = this.contractForm.controls['valueContractControl'].value;
    this.contractModel.ContractTypeId = this.contractForm.controls['typeContractControl'].value.categoryId;
    let bankAccount = this.contractForm.controls['bankAccountControl'].value === undefined ? null : this.contractForm.controls['bankAccountControl'].value;
    this.contractModel.BankAccountId = bankAccount === null ? null : bankAccount.bankAccountId;
    this.contractModel.ValueContract = this.CustomerOrderAmountAfterDiscount;
    let mainContract = this.contractForm.controls['mainContractControl'].value === undefined ? null : this.contractForm.controls['mainContractControl'].value;
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

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
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

    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
    ];

    this.colsAppendixContract = [
      { field: 'contractOrder', header: '#', textAlign: 'center', display: 'table-cell', width: '50px' },
      { field: 'contractCode', header: 'Mã hợp đồng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'effectiveDate', header: 'Ngày hiệu lực', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'expiredDate', header: 'Ngày hết hạn', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'valueContract', header: 'Giá trị hợp đồng', textAlign: 'right', display: 'table-cell', width: '100px' },
    ];
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

  async getMasterData() {
    this.loading = true;
    let result: any = await this.contractService.getMasterDataContract(this.contractId, this.quoteId);
    this.loading = false;
    this.contractModel = this.mappingContractModel(result.contract);
    this.objectList = result.listCustomer;
    this.noteHistory = result.listNote;
    this.listContractCost = this.mappingModelContractCostDetail(result.listContractCost);
    this.handleNoteContent();
    this.objectList.forEach(item => {
      item.customerName = item.customerCode + ' - ' + item.customerName;
    });
    this.listPaymentMethod = result.listPaymentMethod;
    this.listTypeContract = result.listTypeContract;
    this.listQuote = result.listQuote;

    // Get contract type and required customer order
    var contractType = this.contractModel.ContractCode.trim().slice(0, 4);
    if (contractType == 'HĐKT') {
      this.isRequiredCustomerOrder = true;
    } else if (contractType == 'HĐNT') {
      this.isRequiredCustomerOrder = false;
    } else {
      this.isRequiredCustomerOrder = false;
    }

    if (contractType != 'PLHĐ' && result.isOutOfDate ){
      this.isOutOfDate = true;
    }
    else {
      this.isOutOfDate = false;
    }


    // data quote when customer change
    this.listQuoteMaster = result.listQuote;
    this.listQuoteCostDetail = result.listQuoteCostDetail;

    this.listEmpSale = result.listEmployeeSeller;

    this.listEmpSale.forEach(item => {
      item.employeeName = item.employeeCode + ' - ' + item.employeeName;
    });

    this.listContractStatus = result.listContractStatus;
    this.listBankAccount = result.listBankAccount;
    this.listQuoteDetail = result.listQuoteDetail;

    this.listAdditionalInformation = [];
    result.listAdditionalInformation.forEach((item, index) => {
      let additionalInformation = this.mappingAdditionalInformationModel(item, index);
      this.listAdditionalInformation = [...this.listAdditionalInformation, additionalInformation];
    });

    this.listContractDetailModel = [];
    result.listContractDetail.forEach((item, index) => {
      let contractDetailModel = this.mappingContractDetailModel(item, index);
      this.listContractDetailModel = [...this.listContractDetailModel, contractDetailModel];
    });

    this.listFile = result.listFile;
    this.listContract = result.listContract;
    this.listAppendixContract = this.listContract.filter(x => x.mainContractId == this.contractModel.ContractId);

    let status = this.listContractStatus.find(c => c.categoryId == this.contractModel.StatusId);
    if (status) {
      this.viewModeCode = status.categoryCode;
    } else {
      //status không xác định
      this.viewModeCode = "MOI";
    }
    this.setAuthorization();
    this.setDefault();
    if (this.contractModel.IsExtend) {
      this.mainContractControl.disable();
      this.typeContractControl.disable();
      this.isExtend = false;
    }
  }

  showDialog() {
    this.appendixContractTimeControl.setValue(this.contractTimeControl.value)
    this.appendixContractTimeUnitControl.setValue(this.contractTimeUnitControl.value)
    this.customerName = this.objectControl.value.customerName
    this.isShowDialog = true;
  }

  closeDialog() {
    this.isShowDialog = false;
  }

  confirm() {
    if (!this.extendContractForm.valid) {
      Object.keys(this.extendContractForm.controls).forEach(key => {
        if (this.extendContractForm.controls[key].valid == false) {
          this.extendContractForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      this.isShowDialog = false;
      this.confirmationService.confirm({
        message: 'Bạn có muốn gia hạn hợp đồng: ' + this.contractModel.ContractCode + ' thêm ' + this.appendixContractTimeControl.value + ' ' + this.appendixContractTimeUnitControl.value.timeUnit + ' không?',
        accept: () => {
          this.contractExtension();
        }
      });
    }
  }

  contractExtension() {
    let objClone = this.mapDataToModel();
    this.contractId = objClone.ContractId
    objClone.ContractId = this.emptyGuid;

    // Add note when extension contract
    let note = new NoteModel();
    note.Type = 'ADD';
    note.ObjectId = this.contractId;
    note.ObjectType = 'CONTRACT';
    note.NoteTitle = 'đã thêm ghi chú';
    note.Active = true;
    note.CreatedById = this.emptyGuid;
    note.CreatedDate = new Date();

    this.contractService.createCloneContract(objClone, this.listAdditionalInformation,
      this.listContractDetailModel, this.listContractCost,
      this.contractId, this.auth.UserId).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.contractId = result.contractId;
          note.Description = '<b> Đã gia hạn hợp đồng thêm  <b>' + this.appendixContractTimeControl.value + '<b>  <b> ' + this.appendixContractTimeUnitControl.value.timeUnit;
          this.noteService.createNoteForContract(note, []).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.isEditNote = false;
              /*Reshow Time Line*/
              this.noteHistory = result.listNote;
              this.handleNoteContent();
            }
          });
          let mgs = { severity: 'info', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
          this.router.navigate(['/sales/contract-detail', { contractId: this.contractId }]);
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
  }

  mapDataToModel(): Contract {
    let contract = new Contract();

    contract.ContractId = this.contractId;
    contract.ContractCode = this.contractCode + '/' + this.appendixContractCodeControl.value;

    let empSaler: Employee = this.empSalerControl.value != null ? this.empSalerControl.value : null;
    if (empSaler) {
      contract.EmployeeId = empSaler.employeeId;
    } else {
      contract.EmployeeId = null;
    }

    contract.MainContractId = this.contractId;

    contract.EffectiveDate = convertToUTCTime(new Date(this.contractModel.EffectiveDate));
    contract.ExpiredDate = convertToUTCTime(new Date(this.contractModel.ExpiredDate));

    contract.ContractTime = this.appendixContractTimeControl.value;
    contract.ContractTimeUnit = this.appendixContractTimeUnitControl.value.code;

    let description = this.descriptionControl.value != null ? this.descriptionControl.value.trim() : '';
    contract.ContractDescription = description;

    let note = this.noteControl.value != null ? this.noteControl.value.trim() : '';
    contract.ContractNote = note;

    contract.CustomerId = this.objectControl.value != null ? this.objectControl.value.customerId : null;
    contract.CustomerName = this.objectControl.value != null ? this.objectControl.value.customerName : null;
    contract.PaymentMethodId = this.contractModel.PaymentMethodId != null ? this.contractModel.PaymentMethodId : null;
    contract.BankAccountId = this.contractModel.BankAccountId != null ? this.contractModel.BankAccountId : null;

    contract.QuoteId = this.contractModel.QuoteId != null ? this.contractModel.QuoteId : null;

    contract.ValueContract = this.contractModel.ValueContract;
    contract.DiscountType = this.contractModel.DiscountType;
    contract.DiscountValue = this.contractModel.DiscountValue;
    contract.Amount = this.contractModel.Amount;

    contract.Active = true;
    contract.IsExtend = true;
    contract.CreatedById = this.emptyGuid;
    contract.CreatedDate = convertToUTCTime(new Date());
    contract.UpdatedById = this.emptyGuid;
    contract.UpdatedDate = convertToUTCTime(new Date());

    return contract;
  }

  setAuthorization() {
    switch (this.viewModeCode) {
      case "MOI":
        this.contractForm.enable();
        // nếu hợp đồng là hợp đồng nguyên tắc thì khi đặt về nháp cũng không đc chọn hợp đồng chính
        var typteContract = this.listTypeContract.find(c => c.categoryId == this.contractModel.ContractTypeId);
        if (typteContract) {
          switch (typteContract.categoryCode) {
            case 'HDNT':
              this.mainContractControl.disable();
              break;
            // default:
            //   this.mainContractControl.enable();
            //   break;
          }
        }
        break;
      case "APPR":
        this.contractForm.disable();
        break;
      case "DTH":
        //Trạng thái đang thực hiện
        this.contractForm.disable();
        break;
      case "HUY":
        //Trạng thái hủy
        this.contractForm.disable();
        break;
      default:
        break;
    }
  }

  async setViewMode(code: string) {
    let viewModeCode = code;
    let status: Category = this.listContractStatus.find(e => e.categoryCode == viewModeCode);
    let statusId = status ? status.categoryId : null;

    // Add note when change status
    let note = new NoteModel();
    note.Type = 'ADD';
    note.ObjectId = this.contractId;
    note.ObjectType = 'CONTRACT';
    note.NoteTitle = 'đã thêm ghi chú';
    note.Active = true;
    note.CreatedById = this.emptyGuid;
    note.CreatedDate = new Date();

    switch (viewModeCode) {
      case "MOI":
        //Trạng thái mới
        this.loading = true;
        let draftResult: any = await this.contractService.changeContractStatus(this.contractModel.ContractId, statusId);
        if (draftResult.statusCode == 200) {
          note.Description = '<b> Đã thay đổi trạng thái thành - <b>' + status.categoryName;
          this.noteService.createNoteForContract(note, []).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.isEditNote = false;
              /*Reshow Time Line*/
              this.noteHistory = result.listNote;
              this.handleNoteContent();
            }
          });
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Sửa trạng thái thành công' };
          this.viewModeCode = viewModeCode;
          this.setAuthorization();
          this.updateNewStatus(status);
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: draftResult.messageCode };
          this.showMessage(msg);
        }
        this.loading = false;
        break;
      case "APPR":
        //Trạng thái xác nhận
        //chuyen trang thai lead thanh xac nhan
        this.loading = true;
        let result: any = await this.contractService.changeContractStatus(this.contractModel.ContractId, statusId);
        this.loading = false;
        if (result.statusCode == 200) {
          note.Description = '<b> Đã thay đổi trạng thái thành - <b>' + status.categoryName;
          this.noteService.createNoteForContract(note, []).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.isEditNote = false;
              /*Reshow Time Line*/
              this.noteHistory = result.listNote;
              this.handleNoteContent();
            }
          });
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Sửa trạng thái thành công' };
          this.viewModeCode = viewModeCode;
          this.setAuthorization();
          this.updateNewStatus(status);
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
        break;
      case "HUY":
        //Trạng thái hủy
        // if(this.statusSaleBiddingAndQuote == 1 || this.statusSaleBiddingAndQuote == 2){
        //   let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Cơ hội đã gắn với hồ sơ thầu hoặc báo giá! Cần hủy hồ sơ thầu hoặc báo giá trước' };
        //   this.showMessage(msg);
        //   return;
        // }
        this.loading = true;
        let cancelResult: any = await this.contractService.changeContractStatus(this.contractModel.ContractId, statusId);
        this.loading = false;
        if (cancelResult.statusCode == 200) {
          note.Description = '<b> Đã thay đổi trạng thái thành - <b>' + status.categoryName;
          this.noteService.createNoteForContract(note, []).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.isEditNote = false;
              /*Reshow Time Line*/
              this.noteHistory = result.listNote;
              this.handleNoteContent();
            }
          });
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Sửa trạng thái thành công' };
          this.viewModeCode = viewModeCode;
          this.setAuthorization();
          this.updateNewStatus(status);
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: cancelResult.messageCode };
          this.showMessage(msg);
        }
        break;
      case "HTH":
        this.loading = true;
        let completeResult: any = await this.contractService.changeContractStatus(this.contractModel.ContractId, statusId);
        this.loading = false;
        if (completeResult.statusCode == 200) {
          note.Description = '<b> Đã thay đổi trạng thái thành - <b>' + status.categoryName;
          this.noteService.createNoteForContract(note, []).subscribe(response => {
            let result: any = response;
            if (result.statusCode == 200) {
              this.isEditNote = false;
              /*Reshow Time Line*/
              this.noteHistory = result.listNote;
              this.handleNoteContent();
            }
          });
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Bạn đóng hợp đồng thành công!' };
          this.viewModeCode = viewModeCode;
          this.setAuthorization();
          this.updateNewStatus(status);
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: "Bạn chưa thể đóng hợi đồng này!" };
          this.showMessage(msg);
        }
        break;
      default:
        break;
    }
  }

  changeContractTime() {
    if (this.contractTimeControl.value == null || this.contractTimeControl.value == '') {
      this.contractTimeControl.setValue(0);
    }

    var effectiveDate = new Date(this.effectiveDateControl.value);
    var contractTime = parseFloat(this.contractTimeControl.value.replace(/,/g, ''));
    var contractTimeUnit = this.contractTimeUnitControl.value;
    let expiredDate: Date;

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
  }

  changeTimeUnit() {
    if (this.contractTimeControl.value == null || this.contractTimeControl.value == '') {
      this.contractTimeControl.setValue(0);
    }
    var effectiveDate = new Date(this.effectiveDateControl.value);
    var contractTime = parseFloat(this.contractTimeControl.value.replace(/,/g, ''));
    var contractTimeUnit = this.contractTimeUnitControl.value;
    let expiredDate: Date;

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
  }

  changeEffectiveDate() {
    if (this.contractTimeControl.value == null || this.contractTimeControl.value == '') {
      this.contractTimeControl.setValue(0);
    }
    var effectiveDate = new Date(this.effectiveDateControl.value);
    var contractTime = parseFloat(this.contractTimeControl.value.replace(/,/g, ''));
    var contractTimeUnit = this.contractTimeUnitControl.value;
    let expiredDate: Date;

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
  }

  onClearClick() {
    this.isDay = true;
    this.contractTimeControl.setValue(0);
    this.contractTimeUnitControl.setValue(this.listContractTimeUnit.find(x => x.code = 'DAY'));
    this.expiredDateControl.setValue(null);
  }

  updateNewStatus(status: Category) {
    if (this.viewModeCode == 'MOI' || this.viewModeCode == 'APPR' || this.viewModeCode == 'DTH') {
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
        },
        {
          label: this.listContractStatus.find(x => x.categoryCode == 'HUY').categoryName
        }
      ];

      switch (this.viewModeCode) {
        case 'MOI': {
          this.activeIndex = 0;
          break;
        }
        case 'APPR': {
          this.activeIndex = 1;
          break;
        }
        case 'DTH': {
          this.activeIndex = 2;
          break;
        }
        default: {
          break;
        }
      }
    } else {
      this.workFollowContract = [
        {
          label: this.listContractStatus.find(x => x.categoryCode == 'MOI').categoryName
        },
        {
          label: this.listContractStatus.find(x => x.categoryCode == 'APPR').categoryName
        },
        {
          label: this.listContractStatus.find(x => x.categoryCode == this.viewModeCode).categoryName
        }
      ];
      this.activeIndex = 2;
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

  mappingModelContractCostDetail(listContractCost: Array<any>): Array<ContractCostDetail> {
    let contractCosts: Array<ContractCostDetail> = [];
    listContractCost.forEach(item => {
      let contractCostModel = new ContractCostDetail();
      contractCostModel.ContractId = item.contractId;
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

  mappingContractDetailModel(contractDetail: any, index: number): ContractDetail {
    let newContractDetail = new ContractDetail;
    newContractDetail.ContractDetailId = contractDetail.contractDetailId;
    newContractDetail.VendorId = contractDetail.vendorId;
    newContractDetail.ContractId = contractDetail.contractId;
    newContractDetail.ProductId = contractDetail.productId;
    newContractDetail.Quantity = contractDetail.quantity;
    newContractDetail.UnitPrice = contractDetail.unitPrice;
    newContractDetail.CurrencyUnit = contractDetail.currencyUnit;
    newContractDetail.ExchangeRate = contractDetail.exchangeRate;
    newContractDetail.Vat = contractDetail.tax;
    newContractDetail.DiscountType = contractDetail.discountType;
    newContractDetail.DiscountValue = contractDetail.discountValue;
    newContractDetail.Description = contractDetail.description;
    newContractDetail.OrderDetailType = contractDetail.orderDetailType;
    newContractDetail.UnitId = contractDetail.unitId;
    newContractDetail.IncurredUnit = contractDetail.incurredUnit;
    contractDetail.contractProductDetailProductAttributeValue.forEach(item => {
      let contractDetailAttribute = new ContractProductDetailProductAttributeValue();
      contractDetailAttribute.ContractDetailId = item.contractDetailId;
      contractDetailAttribute.ProductId = item.productId;
      contractDetailAttribute.ProductAttributeCategoryId = item.productAttributeCategoryId;
      contractDetailAttribute.ProductAttributeCategoryValueId = item.productAttributeCategoryValueId;
      contractDetailAttribute.ContractProductDetailProductAttributeValueId = item.contractProductDetailProductAttributeValueId;
      contractDetailAttribute.NameProductAttributeCategoryValue = item.nameProductAttributeCategoryValue;
      contractDetailAttribute.NameProductAttributeCategory = item.nameProductAttributeCategory;

      newContractDetail.ContractProductDetailProductAttributeValue.push(contractDetailAttribute);
    });
    newContractDetail.ExplainStr = contractDetail.nameProduct;
    newContractDetail.NameVendor = contractDetail.nameVendor;
    newContractDetail.NameProduct = contractDetail.nameProduct;
    newContractDetail.ProductNameUnit = contractDetail.nameProductUnit;
    newContractDetail.NameMoneyUnit = contractDetail.nameMoneyUnit;
    newContractDetail.GuaranteeTime = contractDetail.guaranteeTime;
    newContractDetail.SumAmount = contractDetail.sumAmount;
    newContractDetail.ProductName = contractDetail.productName;
    newContractDetail.OrderNumber = contractDetail.orderNumber ? contractDetail.orderNumber : index + 1;

    return newContractDetail;
  }

  mappingAdditionalInformationModel(additionalInformation: any, index: number): AdditionalInformation {
    let newAdditionalInformation = new AdditionalInformationModel;
    newAdditionalInformation.active = additionalInformation.active;
    newAdditionalInformation.additionalInformationId = additionalInformation.additionalInformationId;
    newAdditionalInformation.content = additionalInformation.content;
    newAdditionalInformation.createdById = additionalInformation.createdById;
    newAdditionalInformation.createdDate = additionalInformation.createdDate;
    newAdditionalInformation.objectId = additionalInformation.objectId;
    newAdditionalInformation.objectType = additionalInformation.objectType;
    newAdditionalInformation.orderNumber = additionalInformation.orderNumber ? additionalInformation.orderNumber : index + 1;
    newAdditionalInformation.ordinal = additionalInformation.ordinal;
    newAdditionalInformation.title = additionalInformation.title;

    return newAdditionalInformation;
  }

  mappingContractModel(contract: any): Contract {
    let newContract = new Contract();
    newContract.ContractId = contract.contractId;  //done
    newContract.ContractCode = contract.contractCode;  //done
    newContract.EmployeeId = contract.employeeId;
    newContract.MainContractId = contract.mainContractId;
    newContract.ContractTypeId = contract.contractTypeId;
    newContract.EffectiveDate = contract.effectiveDate; //done
    newContract.ExpiredDate = contract.expiredDate;
    newContract.ContractTime = contract.contractTime;
    newContract.ContractTimeUnit = contract.contractTimeUnit;
    newContract.ContractDescription = contract.contractDescription;  //done
    newContract.ContractNote = contract.contractNote; //done
    newContract.CustomerId = contract.customerId; //done
    newContract.ObjectType = contract.objectType; //done
    newContract.QuoteId = contract.quoteId;
    newContract.CustomerName = contract.customerName; //done
    newContract.ValueContract = contract.valueContract;
    newContract.PaymentMethodId = contract.paymentMethodId;  //done
    newContract.DiscountType = contract.discountType;  //done
    newContract.BankAccountId = contract.bankAccountId; //done
    newContract.Amount = contract.amount; //done
    newContract.DiscountValue = contract.discountValue;  //done
    newContract.StatusId = contract.statusId; //done
    newContract.Active = contract.active;  //done
    newContract.IsExtend = contract.isExtend;  //done
    newContract.CreatedById = contract.createdById; //done
    newContract.CreatedDate = contract.createdDate;
    this.contractCode = contract.contractCode;

    this.createdDate = newContract.CreatedDate;
    return newContract;
  }

  async getListMainContractByEmp() {
    this.listContract = [];
    var emp = this.empSalerControl.value;
    if (emp != null) {
      let result: any = await this.contractService.getListMainContract(emp.employeeId);
      this.listContract = result.listContract;
      if (this.listContract.length == 0) {
        let mgs = { severity: 'info', summary: 'Thông báo:', detail: 'Không có hợp đồng nào phù hợp điều kiện' };
        this.showMessage(mgs);
      }
    } else {
      this.listContract = [];
      let mgs = { severity: 'info', summary: 'Thông báo:', detail: 'Không có hợp đồng nào phù hợp điều kiện' };
      this.showMessage(mgs);
    }
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

  deleteContract() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.contractService.deleteContract(this.contractModel.ContractId).subscribe(repsonse => {
          let result = <any>repsonse;
          if (result.statusCode == 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            this.router.navigate(['/sales/contract-list']);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
      }
    });
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
        } else {
          this.listEmpSaleCus = [];
        }
        this.customerGroupId = customer.customerGroupId;
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
    if (event.value.seller) {
      let seller = this.listEmpSale.find(c => c.employeeId == event.value.seller);
      this.empSalerControl.setValue(seller);
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
            contractDetail.SumAmount = total - (total * discountValue) / 100 + ((total - (total * discountValue) / 100) * vat) / 100;
          } else {
            contractDetail.SumAmount = total - discountValue + ((total - (total * discountValue) / 100) * vat) / 100;
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
    } else if (this.listContractDetailModel.length == 0 && this.isRequiredCustomerOrder == true) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm dịch vụ được chọn' };
      this.showMessage(mgs);
    } else {
      this.mappingForm();
      this.awaitResult = false;
      this.loading = true;
      this.contractService.createOrUpdateContractFromForm(this.contractModel, this.listAdditionalInformation, this.listContractDetailModel, this.listContractCost, this.file, this.listFile, false, this.auth.UserId).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.getMasterData();
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Cập nhật hợp đồng thành công" };
          this.showMessage(mgs);
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
    }
  }

  resetForm() {
    // this.objectList = this.listCustomer;
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
    this.DiscountValueVnd = 0;
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
      StatusId: this.emptyGuid,//Guid?
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

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let dateOrder = this.effectiveDateControl.value;
    if (dateOrder) {
      dateOrder = convertToUTCTime(dateOrder);
    } else {
      dateOrder = new Date();
    }
    let ref = this.dialogService.open(AddEditProductContractDialogComponent, {
      data: {
        isCreate: true,
        customerGroupId: this.customerGroupId,
        dateOrder: dateOrder,
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

          //Cộng tổng giá cho toàn bộ đơn hàng
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
    var index = this.listContractDetailModel.indexOf(dataRow);
    var OldArray = this.listContractDetailModel[index];
    this.isEdit = this.actionEdit && this.viewModeCode == "MOI";
    let titlePopup = '';
    if (OldArray.OrderDetailType == 0) {
      titlePopup = 'Sửa sản phẩm dịch vụ';
    } else if (OldArray.OrderDetailType == 1) {
      titlePopup = 'Sửa chi phí phát sinh';
    }
    let dateOrder = this.effectiveDateControl.value;
    if (dateOrder) {
      dateOrder = convertToUTCTime(dateOrder);
    } else {
      dateOrder = new Date();
    }
    let ref = this.dialogService.open(AddEditProductContractDialogComponent, {
      data: {
        isCreate: false,
        quoteDetailModel: OldArray,
        customerGroupId: this.customerGroupId,
        isEdit: this.isEdit,
        dateOrder: dateOrder,
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
          var TotalSumaMount = 0;
          this.listContractDetailModel.forEach(function (item) {
            TotalSumaMount = TotalSumaMount + item.SumAmount;
          });

          this.CustomerOrderAmount = TotalSumaMount;
          this.contractModel.Amount = TotalSumaMount;
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

        this.listContractDetailModel = this.listContractDetailModel.filter(x => x != dataRow);

        this.CustomerOrderAmount = 0;
        for (let i = 0; i < this.listContractDetailModel.length; i++) {
          this.CustomerOrderAmount = this.CustomerOrderAmount + (this.listContractDetailModel[i].SumAmount);
        }

        //Đánh lại số OrderNumber
        this.listContractDetailModel.forEach((item, index) => {
          item.OrderNumber = index + 1;
        });
      }
    });
  }

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
      this.isEdit = this.actionEdit && this.viewModeCode == "MOI";
      let titlePopup = 'Sửa chi phí';

      let ref = this.dialogService.open(AddEditContractCostDialogComponent, {
        data: {
          isCreate: false,
          contractCostDetailModel: OldArray,
          isEdit: this.isEdit
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

  // Xóa thông tin phí
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

  /*Event upload list file*/
  myUploader(event: any) {
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
      fileUpload.FileInFolder.objectId = this.contractId;
      fileUpload.FileInFolder.objectType = 'QLHD';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.contractService.uploadFile("QLHD", listFileUploadModel, this.contractId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.listFile = result.listFileInFolder;

        let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm file thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
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
      let result: any = await this.imageService.uploadFileForOptionAsync(this.uploadedNoteFiles, 'CONTRACT');

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
      noteModel.ObjectId = this.contractId;
      noteModel.ObjectType = 'CONTRACT';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.contractId;
      noteModel.ObjectType = 'CONTRACT';
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
    this.noteService.createNoteForContract(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.noteId = null;
        this.isEditNote = false;

        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        this.handleNoteContent();
        // this.createOrUpdate(false);
        let messageCode = "Thêm ghi chú thành công";
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: messageCode };
        this.showMessage(mgs);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
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

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
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

  /*End*/

  restartCustomerOrderDetailModel() {
    this.contractDetailModel = new ContractDetail();
  }

  createOrder() {
    this.router.navigate(['/order/create', { contractId: this.contractModel.ContractId }]);
  }
  goToLead() {
    this.router.navigate(['/lead/detail', { 'leadId': this.leadId }]);
  }

  goToSaleBidding() {
    this.router.navigate(['/sale-bidding/detail', { saleBiddingId: this.saleBiddingId }]);
  }

  goToOrder() {
    this.router.navigate(['/order/list', { contractId: this.contractModel.ContractId }]);
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: FileInFolder) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.contractService.deleteFile(file.fileInFolderId).subscribe(res => {
          let result: any = res;

          if (result.statusCode == 200) {
            this.listFile = this.listFile.filter(x => x.fileInFolderId != file.fileInFolderId);

            let msg = { severity: 'success', summary: 'Thông báo', detail: 'Xóa file thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(msg);
          }
        })
        // let index = this.listFile.indexOf(file);
        // this.listFile.splice(index, 1);
      }
    });
  }

  downloadFile(data: FileInFolder) {
    this.folderService.downloadFile(data.fileInFolderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
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
            anchor.download = data.fileName.substring(0, data.fileName.lastIndexOf('_')) + "." + data.fileExtension;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView(true);
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/sales/contract-detail', { contractId: rowData }]);
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

//Không được nhập chỉ có khoảng trắng
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
