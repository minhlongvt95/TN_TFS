import { Component, OnInit, ViewChild, ElementRef, AfterContentChecked, ChangeDetectorRef, Renderer2, HostListener } from '@angular/core';
import * as $ from 'jquery';
import { MenuItem } from 'primeng/api';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { FileUpload } from 'primeng/fileupload';
import { SaleBiddingService } from '../../services/sale-bidding.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { saveAs } from "file-saver";
import { Workbook } from 'exceljs';
import { CostsQuocte } from "../../models/costs-quocte.model";
import { SaleBiddingDetailProductAttribute } from "../../models/product-attribute-category-value.model";
import { SaleBiddingDialogComponent } from '../sale-bidding-dialog/sale-bidding-dialog.component';
import { ProductService } from '../../../product/services/product.service';
import * as XLSX from 'xlsx';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  CostsQuocteModel: any,
}

class Lead {
  fullName: string;
  leadId: string;
  personInChargeFullName: string;
  personInChargeId: string;
  contactId: string;
  customerId: string;
  leadCode: string;
}

class LeadDetail {
  taxMoney: number;
  currencyUnit: string;
  description: string;
  discountType: boolean;
  discountValue: number;
  incurredUnit: string;
  leadDetailId: string;
  leadId: string;
  leadProductDetailProductAttributeValue: Array<LeadProductDetailProductAttributeValue>;
  nameMoneyUnit: string;
  nameVendor: string;
  orderDetailType: number;
  productCode: string;
  productId: string;
  productName: string;
  productNameUnit: string;
  quantity: number;
  sumAmount: number;
  unitId: string;
  unitPrice: number;
  vat: number;
  vendorId: string;
  index: number;
  exchangeRate: number;
  amountDiscount: number;
  active: boolean;
  orderNumber: number;
}

class Product {
  price1: number;
  price2: number;
  productCategoryId: string;
  productCategoryName: string;
  productCode: string;
  productDescription: string;
  productId: string;
  productMoneyUnitId: string;
  productName: string
  productUnitId: string;
  quantity: string;
}


interface CostsQuoteExcel {
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


class discountType {
  name: string;
  code: string;
  value: boolean;
}

class LeadProductDetailProductAttributeValue {
  leadDetailId: string;
  leadProductDetailProductAttributeValue1: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
  productId: string;
}

class Employee {
  employeeId: string;
  employeeName: string;
  active: boolean;
}

class Customer {
  customerCode: string;
  customerId: string;
  customerName: string;
  displayName: string;
  customerGroup: string;
  customerPhone: string;
  fullAddress: string;
  taxCode: string;
  personInCharge: string;
}

class UnitMoney {
  categoryCode: string;
  categoryId: string;
  categoryName: string;
}

class SaleBiddingDetail {
  saleBiddingDetailId: string
  saleBiddingId: string;
  category: string;
  content: string;
  note: string;
  file: File[];
  listFile: Array<FileInFolder>;
  index: number;
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

class SaleBidding {
  saleBiddingId: string;
  saleBiddingName: string;
  saleBiddingCode: string;
  leadId: string;
  customerId: string;
  valueBid: number;
  startDate: Date;
  address: string;
  bidStartDate: Date;
  personInChargeId: string;
  effecTime: number;
  endDate: Date;
  typeContractId: string;
  formOfBid: string;
  currencyUnitId: string;
  ros: number;
  provisionalGrossProfit: number;
  typeContractName: string;
  slowDay: number;
  employeeId: string;
  statusId: string;
}

interface Category {
  categoryId: string,
  categoryCode: string,
  categoryName: string,
  isDefault: boolean;
}

@Component({
  selector: 'app-sale-bidding-create',
  templateUrl: './sale-bidding-create.component.html',
  styleUrls: ['./sale-bidding-create.component.css']
})
export class SaleBiddingCreateComponent implements OnInit, AfterContentChecked {
  fixed: boolean = false;
  withFiexd: string = "";
  tabMenu: MenuItem[];
  displayDialogImport: boolean = false;
  displayDialog: boolean = false;
  colsProduct: any[];
  colsProductAttr: any[];
  selectedProduct: any[];
  selectedProductAttr: any[];
  colsCategory: any[];
  selectedCategory: any[];
  listProduct: Array<Product> = [];
  listTypeContact: Array<Category> = [];
  selectedOrderDetailType: number = 0;
  displayCustomerDetail: boolean = false;
  loading: boolean = false;
  saleBiddingDetail: SaleBiddingDetail = new SaleBiddingDetail();
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth = JSON.parse(localStorage.getItem('auth'));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  currentEmployeeId = this.auth.EmployeeId;  //employeeId của người đang đăng nhập
  leadId: string;

  fileName: string = '';

  @ViewChild('fileUpload') fileUpload: FileUpload;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];

  file: File;
  arrayBuffer: any;
  filelist: any;
  titleText: string = '';
  isUpdateAI: boolean = false;
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;

  listCostsQuoteExcel: Array<CostsQuoteExcel> = [];
  isImportCost: boolean = false;

  // Form hồ sơ thầu
  saleBiddingForm: FormGroup;
  employeeControl: FormControl;
  personInChargeControl: FormControl;
  customerControl: FormControl;
  saleBiddingNameControl: FormControl;
  startDateControl: FormControl;
  addressControl: FormControl;
  bidStartDateControl: FormControl;
  effecTimeControl: FormControl;
  endDateControl: FormControl;
  typeContractControl: FormControl;
  formOfBidControl: FormControl;
  currencyUnitControl: FormControl;
  listEmployeeControl: FormControl;
  // End

  activeItem: MenuItem;
  leadModel: Lead = new Lead();
  listLeadDetail: Array<LeadDetail> = [];
  listQuocteDetail: Array<LeadDetail> = [];
  employee: Employee;
  listEmployee: Array<Employee> = [];
  listEmployeeJoin: Array<Employee> = [];
  listPerson: Array<Employee> = [];
  listCustomer: Array<Customer> = [];
  listMoneyUnit: Array<UnitMoney> = [];
  valueBid: number = 0;
  customerSelected: Customer = new Customer();
  sumMoneyCostBeforVAT: number = 0;
  sumCostVAT: number = 0;
  sumCostMoneyAfterVAT: number = 0;
  sumMoneyQuoteBeforVAT: number = 0;
  sumQuocteVAT: number = 0;
  sumQuocteMoneyAfterVAT: number = 0;
  ROS: number = 0;
  provisionalGrossProfit: number = 0;
  listFormOfBid: Array<any> = [
    { label: 'Online', value: "Online" },
    { label: 'Trực tiếp', value: 'Trực tiếp' }
  ];

  discountTypeList: Array<discountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  isProduct1: boolean = true;
  isProduct2: boolean = true;
  isAddCategory: boolean = true;
  isEditCategory: boolean = false;
  listCategory: Array<SaleBiddingDetail> = [];
  actionImport: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  isShowButtonSave: boolean = true;
  isPersonInCharge: boolean = false;
  leadDetail: LeadDetail = new LeadDetail();
  listLeadDetailModelOrderBy: Array<LeadDetail> = [];
  listUnitMoney: Array<Category> = [];
  listUnitProduct: Array<Category> = [];
  productCodeSystemList: string[] = [];
  listProductUnitName: string[] = [];

  amountVatProduct: number = 0;
  amountProduct: number = 0;
  amountDiscountProduct: number = 0;
  amountVatOC: number = 0;
  amountDiscountOC: number = 0;
  amountOC: number = 0;
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();

  constructor(
    private translate: TranslateService,
    private renderer: Renderer2,
    private saleBiddingService: SaleBiddingService,
    private messageService: MessageService,
    private getPermission: GetPermission,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private productService: ProductService,
    private confirmationService: ConfirmationService,
    private ref: ChangeDetectorRef
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
        if (this.save) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
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
    let resource = "crm/sale-bidding/create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      this.route.params.subscribe(params => {
        this.leadId = params['leadId'];
      });
      this.setForm();
      this.setMenuTab();
      this.setTable();
      this.getMasterData();
    }
  }

  ngAfterContentChecked(): void {
    this.ref.detectChanges();
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  getMasterData() {
    this.loading = true;
    this.saleBiddingService.getMasterDataCreateSaleBidding(this.leadId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.employee = result.employee;
        this.leadModel = result.lead;
        if (!this.leadModel.leadCode) this.leadModel.leadCode = '';
        this.listPerson = result.listPerson;
        this.listEmployee = result.listPerson;

        this.listEmployeeJoin = result.listEmployee;
        this.listLeadDetail = result.listLeadDetail;
        this.listLeadDetail.forEach((item, index) => {
          item.orderNumber = item.orderNumber ? item.orderNumber : index + 1;
        })
        this.listCustomer = result.listCustomer;
        this.listTypeContact = result.listTypeContact;
        this.listCustomer.forEach(item => {
          item.displayName = item.customerCode + ' - ' + item.customerName;
        });
        let customer = this.listCustomer.find(x => x.customerId == this.leadModel.customerId);
        this.customerControl.setValue(customer);
        this.caculatorMoneyCost();

        this.listMoneyUnit = result.listMoneyUnit;
        this.personInChargeControl.setValue(this.employee);
        this.employeeControl.setValue(this.employee);
        let unitMoney = this.listMoneyUnit.find(x => x.categoryCode == "VND");
        this.currencyUnitControl.setValue(unitMoney);
        this.listProduct = result.listProduct;
      }
    });
  }

  setForm() {
    /* Form thông tin hồ sơ thầu */
    this.employeeControl = new FormControl(null); // Nhân viên bán hàng
    this.personInChargeControl = new FormControl(null, [Validators.required]); // Người phụ trách
    this.customerControl = new FormControl(null, [Validators.required]);  // Khách hàng bên đấu thầu
    this.saleBiddingNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]); // Tên gói thầu
    this.startDateControl = new FormControl(new Date(), [Validators.required]); // Ngày mở thầu
    this.addressControl = new FormControl(null); // Địa chỉ
    this.bidStartDateControl = new FormControl(new Date(), [Validators.required]); // Ngày nộp thầu
    this.effecTimeControl = new FormControl(0, [Validators.required]); // Thời gian có hiệu lực
    this.endDateControl = new FormControl(null); // Ngày có kết quả dự kiến
    this.typeContractControl = new FormControl(null, [Validators.required]); // Loai hợp đồng
    this.formOfBidControl = new FormControl(null, [Validators.required]); // Hình thức nhận thầu
    this.currencyUnitControl = new FormControl({ value: null, disabled: true }); // Đơn vị tiền
    this.listEmployeeControl = new FormControl(null); // Danh sach nhân viên tham gia

    this.saleBiddingForm = new FormGroup({
      employeeControl: this.employeeControl,
      personInChargeControl: this.personInChargeControl,
      saleBiddingNameControl: this.saleBiddingNameControl,
      startDateControl: this.startDateControl,
      addressControl: this.addressControl,
      bidStartDateControl: this.bidStartDateControl,
      effecTimeControl: this.effecTimeControl,
      endDateControl: this.endDateControl,
      typeContractControl: this.typeContractControl,
      formOfBidControl: this.formOfBidControl,
      currencyUnitControl: this.currencyUnitControl,
      listEmployeeControl: this.listEmployeeControl,
      customerControl: this.customerControl
    });
    /* End */
  }

  changePerson() {
    if (this.customerControl.value) {
      this.loading = true;
      this.saleBiddingService.getPersonInChargeByCustomerId(this.customerControl.value.customerId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listPerson = result.listPersonInCharge;
          this.listEmployee = this.listPerson;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    } else {

    }
  }

  setMenuTab() {
    this.tabMenu = [
      { label: 'Chi tiết CP đầu vào', icon: 'fas fa-dollar-sign' },
      { label: 'Chi tiết báo giá', icon: 'fas fa-quote-right' },
      { label: 'Chi tiết hồ sơ thầu', icon: 'fas fa-info-circle' },
    ];
    this.activeItem = this.tabMenu[0];
  }

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

  /*Sao chép danh sách sản phẩm dịch vụ ở tab chi phí đầu vào sang tab chi tiết báo giá*/
  moveQuocte() {
    this.listQuocteDetail = [];
    this.listQuocteDetail = this.listLeadDetail.filter(x => x != null);
    this.activeItem = this.tabMenu[1];
    this.caculatorMoneyQuocte();
  }

  saveSaleBidding() {
    if (!this.saleBiddingForm.valid) {
      Object.keys(this.saleBiddingForm.controls).forEach(key => {
        if (this.saleBiddingForm.controls[key].valid == false) {
          this.saleBiddingForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
    }
    else {
      let isAdd = true;
      let saleBidding: SaleBidding = new SaleBidding();

      let startDate = this.startDateControl.value;
      if (startDate) {
        startDate = convertToUTCTime(startDate);
      }
      let bidStartDate = this.bidStartDateControl.value;
      if (bidStartDate) {
        bidStartDate = convertToUTCTime(bidStartDate);
      }

      if (this.effecTimeControl.value <= 0) {
        let msg = { severity: 'error', summary: 'Thông báo', detail: "Thời gian hiệu lực phải lớn hơn 0!" };
        this.showMessage(msg);
        isAdd = false;
      }
      let endDate = this.endDateControl.value;
      if (endDate != null) {
        endDate = convertToUTCTime(endDate);
      }
      if (endDate != null) {
        if (this.calcDaysDiff(startDate, endDate) < 0) {
          let msg = { severity: 'error', summary: 'Thông báo', detail: "Ngày có kết quả dự kiến phải lớn hơn hoặc bằng ngày mở thầu!" };
          this.showMessage(msg);
          isAdd = false;
        }
      }
      if (isAdd) {
        saleBidding.saleBiddingName = this.saleBiddingNameControl.value;
        saleBidding.customerId = this.customerControl.value.customerId;
        saleBidding.startDate = this.startDateControl.value;
        saleBidding.address = this.addressControl.value;
        saleBidding.bidStartDate = this.bidStartDateControl.value;
        saleBidding.effecTime = this.effecTimeControl.value;
        saleBidding.endDate = endDate;
        saleBidding.typeContractId = this.typeContractControl.value.categoryId;
        saleBidding.formOfBid = this.formOfBidControl.value.label;
        saleBidding.currencyUnitId = this.currencyUnitControl.value.categoryId;
        saleBidding.personInChargeId = this.personInChargeControl.value.employeeId;
        saleBidding.leadId = this.leadId;
        saleBidding.ros = this.ROS;
        saleBidding.valueBid = this.sumQuocteMoneyAfterVAT;
        saleBidding.provisionalGrossProfit = this.provisionalGrossProfit;
        saleBidding.employeeId = this.employeeControl.value ? this.employeeControl.value.employeeId : null;

        this.loading = true;
        let listEmployee: Array<Employee> = this.listEmployeeControl.value;
        if (listEmployee == null) {
          listEmployee = [];
        }
        this.saleBiddingService.createSaleBidding(saleBidding, listEmployee, this.listLeadDetail, this.listQuocteDetail, this.listCategory).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.router.navigate(['/sale-bidding/detail', { saleBiddingId: result.saleBiddingId }]);
            let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm thành công!" };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    }
  }

  /*Thay đổi ngày mở thầu*/
  changeStartDate() {
    this.endDateControl.setValue(null);
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  addProduct(number: number) {
    /*Thêm sản phẩm dịch vụ*/
    let isCosts = false;
    if (number == 1) {
      isCosts = true;
    }
    let ref = this.dialogService.open(SaleBiddingDialogComponent, {
      data: {
        isCreate: true,
        isCosts: isCosts
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
          if (number == 1) {
            this.leadDetail = result.CostsQuocteModel;
            this.leadDetail.vendorId = result.CostsQuocteModel.vendorId;
            this.leadDetail.leadDetailId = result.CostsQuocteModel.leadDetailId;
            this.leadDetail.productId = result.CostsQuocteModel.productId;
            this.leadDetail.quantity = result.CostsQuocteModel.quantity;
            this.leadDetail.unitPrice = result.CostsQuocteModel.unitPrice;
            this.leadDetail.currencyUnit = result.CostsQuocteModel.currencyUnit;
            this.leadDetail.exchangeRate = result.CostsQuocteModel.exchangeRate;
            this.leadDetail.vat = result.CostsQuocteModel.vat;
            this.leadDetail.discountType = result.CostsQuocteModel.discountType;
            this.leadDetail.discountValue = result.CostsQuocteModel.discountValue;
            this.leadDetail.description = result.CostsQuocteModel.description;
            this.leadDetail.orderDetailType = result.CostsQuocteModel.orderDetailType;
            this.leadDetail.unitId = result.CostsQuocteModel.unitId;
            this.leadDetail.incurredUnit = result.CostsQuocteModel.incurredUnit;
            this.leadDetail.nameVendor = result.CostsQuocteModel.nameVendor;
            this.leadDetail.productNameUnit = result.CostsQuocteModel.productNameUnit;
            this.leadDetail.nameMoneyUnit = result.CostsQuocteModel.nameMoneyUnit;
            this.leadDetail.sumAmount = result.CostsQuocteModel.sumAmount;
            this.leadDetail.amountDiscount = result.CostsQuocteModel.amountDiscount;
            this.leadDetail.productName = result.CostsQuocteModel.productName;
            this.leadDetail.productCode = result.CostsQuocteModel.productCode;
            this.leadDetail.leadProductDetailProductAttributeValue = [];
            result.CostsQuocteModel.saleBiddingDetailProductAttribute.forEach(item => {
              let temp: LeadProductDetailProductAttributeValue = new LeadProductDetailProductAttributeValue();
              temp.productAttributeCategoryId = item.productAttributeCategoryId;
              temp.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
              temp.productId = item.productId;
              this.leadDetail.leadProductDetailProductAttributeValue.push(temp);
            });
            //set orderNumber cho sản phẩm/dịch vụ mới thêm
            this.leadDetail.orderNumber = this.listLeadDetail.length + 1;
            this.listLeadDetail.push(this.leadDetail);

            // this.listLeadDetailModelOrderBy = [];
            // for (let i = this.listLeadDetail.length - 1; i >= 0; i--) {
            //   this.listLeadDetailModelOrderBy.push(this.listLeadDetail[i]);
            // }
            // this.listLeadDetail = [];
            // this.listLeadDetail = this.listLeadDetailModelOrderBy;
            this.caculatorMoneyCost();
          } else {
            this.leadDetail = result.CostsQuocteModel;
            this.leadDetail = result.CostsQuocteModel;
            this.leadDetail.vendorId = result.CostsQuocteModel.vendorId;
            this.leadDetail.leadDetailId = result.CostsQuocteModel.leadDetailId;
            this.leadDetail.productId = result.CostsQuocteModel.productId;
            this.leadDetail.quantity = result.CostsQuocteModel.quantity;
            this.leadDetail.unitPrice = result.CostsQuocteModel.unitPrice;
            this.leadDetail.currencyUnit = result.CostsQuocteModel.currencyUnit;
            this.leadDetail.exchangeRate = result.CostsQuocteModel.exchangeRate;
            this.leadDetail.vat = result.CostsQuocteModel.vat;
            this.leadDetail.discountType = result.CostsQuocteModel.discountType;
            this.leadDetail.discountValue = result.CostsQuocteModel.discountValue;
            this.leadDetail.description = result.CostsQuocteModel.description;
            this.leadDetail.orderDetailType = result.CostsQuocteModel.orderDetailType;
            this.leadDetail.unitId = result.CostsQuocteModel.unitId;
            this.leadDetail.incurredUnit = result.CostsQuocteModel.incurredUnit;
            this.leadDetail.nameVendor = result.CostsQuocteModel.nameVendor;
            this.leadDetail.productNameUnit = result.CostsQuocteModel.productNameUnit;
            this.leadDetail.nameMoneyUnit = result.CostsQuocteModel.nameMoneyUnit;
            this.leadDetail.sumAmount = result.CostsQuocteModel.sumAmount;
            this.leadDetail.amountDiscount = result.CostsQuocteModel.amountDiscount;
            this.leadDetail.productName = result.CostsQuocteModel.productName;
            this.leadDetail.productCode = result.CostsQuocteModel.productCode;
            this.leadDetail.leadProductDetailProductAttributeValue = [];
            result.CostsQuocteModel.saleBiddingDetailProductAttribute.forEach(item => {
              let temp: LeadProductDetailProductAttributeValue = new LeadProductDetailProductAttributeValue();
              temp.productAttributeCategoryId = item.productAttributeCategoryId;
              temp.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
              temp.productId = item.productId;
              this.leadDetail.leadProductDetailProductAttributeValue.push(temp);
            });
            //set orderNumber cho sản phẩm/dịch vụ mới thêm
            this.leadDetail.orderNumber = this.listLeadDetail.length + 1;
            this.listQuocteDetail.push(this.leadDetail);

            // this.listLeadDetailModelOrderBy = [];
            // for (let i = this.listQuocteDetail.length - 1; i >= 0; i--) {
            //   this.listLeadDetailModelOrderBy.push(this.listQuocteDetail[i]);
            // }
            // this.listQuocteDetail = [];
            // this.listQuocteDetail = this.listLeadDetailModelOrderBy;
            this.caculatorMoneyQuocte();
          }

          // this.leadDetail = new LeadDetail;
        }
      }
    });
  }

  showDialog(dataRow, number) {
    //Nếu có quyền sửa thì mới cho sửa
    let isCosts = false;
    if (number == 1) {
      isCosts = true;
    }
    if (this.actionEdit) {
      let OldArray: CostsQuocte = new CostsQuocte();
      let index;
      if (number == 1) {
        index = this.listLeadDetail.indexOf(dataRow);

        OldArray.vendorId = this.listLeadDetail[index].vendorId;
        OldArray.saleBiddingId = this.emptyGuid;
        OldArray.productId = this.listLeadDetail[index].productId;
        OldArray.quantity = this.listLeadDetail[index].quantity;
        OldArray.unitPrice = this.listLeadDetail[index].unitPrice;
        OldArray.currencyUnit = this.listLeadDetail[index].currencyUnit;
        OldArray.exchangeRate = this.listLeadDetail[index].exchangeRate;
        OldArray.vat = this.listLeadDetail[index].vat;
        OldArray.discountType = this.listLeadDetail[index].discountType;
        OldArray.discountValue = this.listLeadDetail[index].discountValue;
        OldArray.description = this.listLeadDetail[index].description;
        OldArray.orderDetailType = this.listLeadDetail[index].orderDetailType;
        OldArray.unitId = this.listLeadDetail[index].unitId;
        OldArray.incurredUnit = this.listLeadDetail[index].incurredUnit;
        OldArray.nameVendor = this.listLeadDetail[index].nameVendor;
        OldArray.productNameUnit = this.listLeadDetail[index].productNameUnit;
        OldArray.nameMoneyUnit = this.listLeadDetail[index].nameMoneyUnit;
        OldArray.sumAmount = this.listLeadDetail[index].sumAmount;
        OldArray.amountDiscount = this.listLeadDetail[index].amountDiscount;
        OldArray.productName = this.listLeadDetail[index].productName;
        OldArray.productCode = this.listLeadDetail[index].productCode;
        OldArray.saleBiddingDetailProductAttribute = [];
        this.listLeadDetail[index].leadProductDetailProductAttributeValue.forEach(item => {
          let temp: SaleBiddingDetailProductAttribute = new SaleBiddingDetailProductAttribute();
          temp.productAttributeCategoryId = item.productAttributeCategoryId;
          temp.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
          temp.productId = item.productId;
          temp.saleBiddingDetailProductAttributeId = this.emptyGuid;
          temp.saleBiddingDetailId = this.listLeadDetail[index].leadDetailId;
          OldArray.saleBiddingDetailProductAttribute.push(temp);
        });

      } else {
        index = this.listQuocteDetail.indexOf(dataRow);

        OldArray.vendorId = this.listQuocteDetail[index].vendorId;
        OldArray.saleBiddingId = this.emptyGuid;
        OldArray.productId = this.listQuocteDetail[index].productId;
        OldArray.quantity = this.listQuocteDetail[index].quantity;
        OldArray.unitPrice = this.listQuocteDetail[index].unitPrice;
        OldArray.currencyUnit = this.listQuocteDetail[index].currencyUnit;
        OldArray.exchangeRate = this.listQuocteDetail[index].exchangeRate;
        OldArray.vat = this.listQuocteDetail[index].vat;
        OldArray.discountType = this.listQuocteDetail[index].discountType;
        OldArray.discountValue = this.listQuocteDetail[index].discountValue;
        OldArray.description = this.listQuocteDetail[index].description;
        OldArray.orderDetailType = this.listQuocteDetail[index].orderDetailType;
        OldArray.unitId = this.listQuocteDetail[index].unitId;
        OldArray.incurredUnit = this.listQuocteDetail[index].incurredUnit;
        OldArray.nameVendor = this.listQuocteDetail[index].nameVendor;
        OldArray.productNameUnit = this.listQuocteDetail[index].productNameUnit;
        OldArray.nameMoneyUnit = this.listQuocteDetail[index].nameMoneyUnit;
        OldArray.sumAmount = this.listQuocteDetail[index].sumAmount;
        OldArray.amountDiscount = this.listQuocteDetail[index].amountDiscount;
        OldArray.productName = this.listQuocteDetail[index].productName;
        OldArray.productCode = this.listQuocteDetail[index].productCode;
        OldArray.saleBiddingDetailProductAttribute = [];
        this.listQuocteDetail[index].leadProductDetailProductAttributeValue.forEach(item => {
          let temp: SaleBiddingDetailProductAttribute = new SaleBiddingDetailProductAttribute();
          temp.productAttributeCategoryId = item.productAttributeCategoryId;
          temp.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
          temp.productId = item.productId;
          temp.saleBiddingDetailProductAttributeId = this.emptyGuid;
          temp.saleBiddingDetailId = this.listQuocteDetail[index].leadDetailId;
          OldArray.saleBiddingDetailProductAttribute.push(temp);
        });
      }

      let titlePopup = '';
      if (OldArray.orderDetailType == 0) {
        titlePopup = 'Sửa sản phẩm dịch vụ';
      } else if (OldArray.orderDetailType == 1) {
        titlePopup = 'Sửa chi phí phát sinh';
      }

      let ref = this.dialogService.open(SaleBiddingDialogComponent, {
        data: {
          isCreate: false,
          isCosts: isCosts,
          CostsQuocteModel: OldArray
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
            if (number == 1) {
              this.leadDetail = result.CostsQuocteModel;

              this.leadDetail.leadProductDetailProductAttributeValue = [];
              result.CostsQuocteModel.saleBiddingDetailProductAttribute.forEach(item => {
                let temp: LeadProductDetailProductAttributeValue = new LeadProductDetailProductAttributeValue();
                temp.productAttributeCategoryId = item.productAttributeCategoryId;
                temp.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
                temp.productId = item.productId;
                this.leadDetail.leadProductDetailProductAttributeValue.push(temp);
              });
              this.listLeadDetail[index] = this.leadDetail;
              this.listLeadDetail = [...this.listLeadDetail];
              this.caculatorMoneyCost();
            } else {
              this.leadDetail = result.CostsQuocteModel;

              this.leadDetail.leadProductDetailProductAttributeValue = [];
              result.CostsQuocteModel.saleBiddingDetailProductAttribute.forEach(item => {
                let temp: LeadProductDetailProductAttributeValue = new LeadProductDetailProductAttributeValue();
                temp.productAttributeCategoryId = item.productAttributeCategoryId;
                temp.productAttributeCategoryValueId = item.productAttributeCategoryValueId;
                temp.productId = item.productId;
                this.leadDetail.leadProductDetailProductAttributeValue.push(temp);
              });
              this.listQuocteDetail[index] = this.leadDetail;
              this.listQuocteDetail = [...this.listQuocteDetail]
              this.caculatorMoneyQuocte();
            }
          }
          this.leadDetail = new LeadDetail;
        }
      });
    }
  }

  showCustomerDetail() {
    this.displayCustomerDetail = true;
    this.customerSelected = this.customerControl.value;
  }

  setTable() {
    this.colsProductAttr = [
      { field: 'productAttributeCategoryName', header: 'Tên thuộc tính', textAlign: 'left', color: '#f44336' },
      { field: 'productAttributeCategoryValue', header: 'Giá trị', textAlign: 'right', color: '#f44336' }
    ];
    this.selectedProductAttr = this.colsProductAttr;

    this.colsProduct = [
      { field: 'Move', header: '#', width: '95px', textAlign: 'center', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm', textAlign: 'left', color: '#f44336' },
      { field: 'productNameUnit', header: 'Đơn vị tính', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', textAlign: 'right', color: '#f44336' },
      { field: 'vat', header: 'Thuế suất', textAlign: 'right', color: '#f44336' },
      { field: 'taxMoney', header: 'Tiền thuế', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Tổng tiền', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Thao tác', textAlign: 'right', color: '#f44336' }
    ];
    this.selectedProduct = this.colsProduct;

    this.colsCategory = [
      { field: 'category', header: 'Hạng mục', textAlign: 'left', color: '#f44336' },
      { field: 'content', header: 'Nội dung', textAlign: 'left', color: '#f44336' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', color: '#f44336' }
    ];
  }

  removeProduct(data) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listLeadDetail = this.listLeadDetail.filter(x => x.index != data.index);
        //Đánh lại số OrderNumber
        this.listLeadDetail.forEach((item, index) => {
          item.orderNumber = index + 1;
        });
        this.caculatorMoneyCost();
      }
    });
  }
  removeProductQuote(data) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listQuocteDetail = this.listQuocteDetail.filter(x => x.index != data.index);
        this.caculatorMoneyQuocte();
      }
    });

  }
  calcDaysDiff(dateFrom, dateTo): number {
    let currentDate = new Date(dateTo);
    let dateSent = new Date(dateFrom);

    return Math.floor((Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()) - Date.UTC(dateSent.getFullYear(), dateSent.getMonth(), dateSent.getDate())) / (1000 * 60 * 60 * 24));
  }

  getActiveItem(activeItem) {
    this.activeItem = activeItem.activeItem;
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

  chooseFile(event) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;

    this.getInforDetailQuote();
  }

  async getInforDetailQuote() {
    let result: any = await this.productService.getMasterdataCreateProduct();
    if (result.statusCode === 200) {
      this.productCodeSystemList = result.listProductCode;
      this.listProductUnitName = result.listProductUnitName;
    };

    this.saleBiddingService.getMasterDataSaleBiddingAddEditProductDialog().subscribe(response => {
      let resultListProduct: any = response;
      if (resultListProduct.statusCode == 200) {
        this.listProduct = resultListProduct.listProduct;
        this.listUnitMoney = resultListProduct.listUnitMoney;
        this.listUnitProduct = resultListProduct.listUnitProduct;
      }
    });
  }

  showDialogImportExcel(number) {
    this.displayDialogImport = true;
    if (number == 1) {
      this.isImportCost = true;
    } else {
      this.isImportCost = false;
    }
  }

  downloadTemplateExcel() {
    this.saleBiddingService.downloadTemplateProduct().subscribe(response => {
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
          if (parseFloat(row[3]) == undefined || parseFloat(row[3]).toString() == "NaN" || parseFloat(row[3]) == null) {
            this.messErrFile.push('Cột số lượng tại dòng ' + (i + 2) + ' sai định dạng');
          }
        }
        if (row[4] === null || row[4] === undefined || row[4] == "") {
          this.messErrFile.push('Cột đơn giá tại dòng ' + (i + 2) + ' không được để trống');
        }
        else {
          if (parseFloat(row[4]) == undefined || parseFloat(row[4]).toString() == "NaN" || parseFloat(row[4]) == null) {
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
          this.messErrFile.forEach(element => {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: element };
            this.showMessage(mgs);
          });
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
                  productCodeList.push(row[1]);
                }
                let check = this.productCodeSystemList.find(d => d.toLowerCase().trim() == row[1].trim().toLowerCase());
                if (check === undefined || check === null) {
                  messCodeErr.push(i + 2);
                }
              };
              if (row[5] !== null && row[5] !== undefined && row[5].trim() != "" &&
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "")) {
                let isProduct = productUnitList.find(i => i.trim() == row[5].trim());
                if (isProduct == null || isProduct == undefined) {
                  productUnitList.push(row[5]);
                  let check = this.listProductUnitName.find(d => d.toLowerCase().trim() == row[5].trim().toLowerCase());
                  if (check === undefined || check === null) {
                    messUnitErr.push(i + 2);
                  }
                }
              };
            }
          });

          let countCode = this.productCodeSystemList.filter(c => productCodeList.includes(c));
          let countUnit = this.listProductUnitName.filter(u => productUnitList.includes(u));

          if (countCode.length == productCodeList.length && countUnit.length == productUnitList.length) {
            this.listCostsQuoteExcel = [];
            dataCompanyCustomer.forEach((row, i) => {
              // Lấy giá trị bản ghi trong excel bắt đầu từ line 6
              if (i > 4 && row.length != 0 && (
                (row[1] !== null && row[1] !== undefined && row[1].trim() != "") || (row[2] !== null && row[2] !== undefined && row[2].trim() != "") || (row[5] !== null && row[5] !== undefined && row[5].trim() != "")
              )
              ) {
                let newCustomer: CostsQuoteExcel = {
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
                this.listCostsQuoteExcel.push(newCustomer);
              }
            });
            // lấy tiền VND
            var moneyUnit = this.listUnitMoney.find(c => c.categoryCode == "VND");

            this.listCostsQuoteExcel.forEach(item => {
              let detailProduct = new LeadDetail();
              if (item.ProductCode == null || item.ProductCode.trim() == "" || item.ProductCode == undefined) {
                detailProduct.orderDetailType = 1;
                detailProduct.active = true;
                detailProduct.currencyUnit = moneyUnit.categoryId;
                detailProduct.discountType = item.DiscountType;
                detailProduct.exchangeRate = 1;
                detailProduct.incurredUnit = item.CurrencyUnit;
                detailProduct.nameMoneyUnit = moneyUnit.categoryCode;
                detailProduct.description = item.ProductName;
                detailProduct.productName = null;
                detailProduct.quantity = item.Quantity;
                detailProduct.unitPrice = item.UnitPrice;
                detailProduct.vat = item.Tax;
                detailProduct.discountValue = item.DiscountValue;
                detailProduct.sumAmount = item.TotalAmount;
                detailProduct.leadProductDetailProductAttributeValue = [];

                if (this.isImportCost) {
                  this.listLeadDetail.push(detailProduct);
                  this.caculatorMoneyCost();

                } else {
                  this.listQuocteDetail.push(detailProduct);
                  this.caculatorMoneyQuocte();
                }
              }
              else {
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

                let product = this.listProduct.find(p => p.productCode.trim() == item.ProductCode.trim());
                detailProduct.productId = product.productId;
                detailProduct.productName = product.productName;
                detailProduct.productCode = product.productCode;

                //Lấy đơn vị tính
                let productUnitId = product.productUnitId;
                let productUnitName = this.listUnitProduct.find(x => x.categoryId == productUnitId).categoryName;
                detailProduct.productNameUnit = productUnitName;
                detailProduct.unitId = productUnitId;
                detailProduct.leadProductDetailProductAttributeValue = [];


                //Lấy list nhà cung cấp
                this.saleBiddingService.getVendorByProductId(product.productId).subscribe(response => {
                  let result: any = response;
                  if (result.statusCode == 200) {
                    let listVendor = result.listVendor;
                    /*Nếu listVendor chỉ có 1 giá trị duy nhất thì lấy luôn giá trị đó làm default value*/
                    if (listVendor.length == 1) {
                      let toSelectVendor = listVendor[0];
                      detailProduct.vendorId = toSelectVendor.vendorId;
                      detailProduct.nameVendor = toSelectVendor.vendorCode + " - " + toSelectVendor.vendorName;
                    }
                    /*End*/
                  } else {
                    let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                    this.showMessage(msg);
                  }
                  if (this.isImportCost) {
                    this.listLeadDetail.push(detailProduct);
                    this.caculatorMoneyCost();
                  } else {
                    this.listQuocteDetail.push(detailProduct);
                    this.caculatorMoneyQuocte();
                  }
                });
                this.isInvalidForm = false;  //Hiển thị icon-warning-active
                this.isOpenNotifiError = false;  //Hiển thị message lỗi
              }
            })
          }
          if (countCode.length != productCodeList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messCodeErr.forEach(item => {
              this.messErrFile.push('Mã sản phẩm tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
          if (countUnit.length != productUnitList.length) {
            this.isInvalidForm = true;  //Hiển thị icon-warning-active
            this.isOpenNotifiError = true;  //Hiển thị message lỗi
            messUnitErr.forEach(item => {
              this.messErrFile.push('Đơn vị tính tại dòng ' + item + ' không tồn tại trong hệ thống')
            });
          }
        }
        this.displayDialogImport = false;
      }

      this.listLeadDetail = [...this.listLeadDetail];
      this.listQuocteDetail = [...this.listQuocteDetail];
    }
  }

  cancel() {
    this.router.navigate(['/sale-bidding/list']);
  }

  exportExcel(number) {
    let isExport = true;
    if (number == 1 && this.listLeadDetail.length == 0) {
      isExport = false;
    } else if (number == 2 && this.listQuocteDetail.length == 0) {
      isExport = false;
    }
    if (!isExport) {
      this.confirmationService.confirm({
        message: 'Không có sản phẩm nào bạn có muốn xuất excel không?',
        accept: () => {
          this.exportFileExcel(number);
        }
      });
    }
    else {
      this.exportFileExcel(number);
    }

  }

  exportFileExcel(number) {
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
    if (number == 1) {
      this.listLeadDetail.forEach((item, index) => {
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
      });
    }
    else {
      this.listQuocteDetail.forEach((item, index) => {
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
      });
    }

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
    });
  }

  addCategory() {
    let saleBiddingDetail: SaleBiddingDetail = new SaleBiddingDetail();
    let isAdd = true;
    if (this.saleBiddingDetail.category == null || this.saleBiddingDetail.category == "" || this.saleBiddingDetail.category.trim().length == 0) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Hạng mục không được để trống" };
      this.showMessage(mgs);
      isAdd = false;
    }
    if (this.saleBiddingDetail.content == null || this.saleBiddingDetail.content == "" || this.saleBiddingDetail.content.trim().length == 0) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Nội dung không được để trống" };
      this.showMessage(mgs);
      isAdd = false;
    }
    if (isAdd) {
      saleBiddingDetail.category = this.saleBiddingDetail.category;
      saleBiddingDetail.content = this.saleBiddingDetail.content;

      saleBiddingDetail.note = "";
      saleBiddingDetail.file = new Array<File>();
      this.uploadedFiles.forEach(item => {
        saleBiddingDetail.note = saleBiddingDetail.note + item.name + ";";
      });
      this.uploadedFiles.forEach(item => {
        saleBiddingDetail.file.push(item);
      })
      let index = 0;
      if (this.listCategory.length == 0) {
        index = 0;
      }
      else {
        index = this.listCategory.sort(x => x.index)[this.listCategory.length - 1].index + 1;
      }
      saleBiddingDetail.index = index;
      this.listCategory.push(saleBiddingDetail);
      this.saleBiddingDetail = new SaleBiddingDetail();
      this.uploadedFiles = [];
      this.fileUpload.files = [];
    }
  }

  cancelNoteEdit() {
    this.saleBiddingDetail = new SaleBiddingDetail();
    this.isAddCategory = true;
    this.isEditCategory = false;
  }

  cancelFile() {
    $("#importFileProduct").val("")
    this.fileName = "";
  }

  removeCategory(data: SaleBiddingDetail) {
    this.listCategory = this.listCategory.filter(x => x.index != data.index);
  }

  editCategory(data: SaleBiddingDetail) {
    this.isAddCategory = false;
    this.isEditCategory = true;
    this.saleBiddingDetail.content = data.content;
    this.saleBiddingDetail.category = data.category;
    this.saleBiddingDetail.index = data.index;
    this.saleBiddingDetail.file = new Array<File>();
    data.file.forEach(item => {
      this.fileUpload.files.push(item);
      this.uploadedFiles.push(item);
      this.saleBiddingDetail.file.push(item);
    });
  }

  saveCategory() {
    let saleBiddingDetail: SaleBiddingDetail = new SaleBiddingDetail();
    let isSave = true;
    if (this.saleBiddingDetail.category == null || this.saleBiddingDetail.category == "" || this.saleBiddingDetail.category.trim().length == 0) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Hạng mục không được để trống" };
      this.showMessage(mgs);
      isSave = false;
    }
    if (this.saleBiddingDetail.content == null || this.saleBiddingDetail.content == "" || this.saleBiddingDetail.content.trim().length == 0) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Nội dung không được để trống" };
      this.showMessage(mgs);
      isSave = false;
    }
    if (isSave) {
      saleBiddingDetail.index = this.saleBiddingDetail.index;
      saleBiddingDetail.category = this.saleBiddingDetail.category;
      saleBiddingDetail.content = this.saleBiddingDetail.content;
      saleBiddingDetail.note = "";
      this.uploadedFiles.forEach(item => {
        saleBiddingDetail.note = saleBiddingDetail.note + item.name + ";";
      });
      saleBiddingDetail.file = new Array<File>();
      this.uploadedFiles.forEach(item => {
        saleBiddingDetail.file.push(item);
      });

      let new_data = this.listCategory.find(x => x.index == saleBiddingDetail.index);
      new_data.category = saleBiddingDetail.category;
      new_data.content = saleBiddingDetail.content;
      new_data.note = saleBiddingDetail.note;
      new_data.file = saleBiddingDetail.file;

      this.listCategory = [...this.listCategory];
      this.saleBiddingDetail = new SaleBiddingDetail();
      this.uploadedFiles = [];
      this.fileUpload.files = [];

      this.isAddCategory = true;
      this.isEditCategory = false;

      let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Cập nhật thành công" };
      this.showMessage(mgs);
    }
  }

  downLoadFile(data: SaleBiddingDetail) {
    data.file.forEach(item => {

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(item);
      } else {
        var fileURL = URL.createObjectURL(item);
        if (item.type.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = item.name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  /*Tính số thống kê tab Chi tiết CP đầu vào*/
  caculatorMoneyCost() {
    let index = 0;
    //Tiền thuế
    this.sumCostVAT = 0;

    //Giá trị trước thuế
    this.sumMoneyCostBeforVAT = 0;

    //Giá trị sau thuế
    this.sumCostMoneyAfterVAT = 0;

    this.listLeadDetail.forEach(item => {
      let cacuDiscount = 0
      item.index = index + 1;
      let caculateVAT: number = 0;
      if (item.discountValue != null) {
        if (item.discountType == true) {
          cacuDiscount = ((item.quantity * item.unitPrice * item.exchangeRate * item.discountValue) / 100);
        }
        else {
          cacuDiscount = item.discountValue;
        }
      }

      if (item.vat != null) {
        caculateVAT = (((item.quantity * item.exchangeRate * item.unitPrice) - cacuDiscount) * item.vat) / 100;
      }

      this.sumMoneyCostBeforVAT = this.sumMoneyCostBeforVAT + (item.quantity * item.unitPrice * item.exchangeRate) - cacuDiscount;
      item.taxMoney = caculateVAT;
      this.sumCostVAT = this.sumCostVAT + caculateVAT;
      item.sumAmount = (item.quantity * item.unitPrice * item.exchangeRate) + item.taxMoney - cacuDiscount;
      this.sumCostMoneyAfterVAT = this.sumCostMoneyAfterVAT + item.quantity * item.unitPrice * item.exchangeRate - cacuDiscount + caculateVAT;
      index++;
    });

    if (this.listQuocteDetail.length == 0 || this.listLeadDetail.length == 0) {
      //Tỉ suất lợi nhuận tạm tính(%)
      this.ROS = 0;

      //Lợi nhuận gộp tạm tính
      this.provisionalGrossProfit = 0;
    } else {
      if (this.sumQuocteMoneyAfterVAT == 0) {
        //Tỉ suất lợi nhuận tạm tính(%)
        this.ROS = 0;
      } else {
        //Tỉ suất lợi nhuận tạm tính(%)
        this.ROS = this.roundNumber((this.sumQuocteMoneyAfterVAT - this.sumCostMoneyAfterVAT) * 100 / this.sumQuocteMoneyAfterVAT, 2);
        
        //Lợi nhuận gộp tạm tính
        this.provisionalGrossProfit = this.sumQuocteMoneyAfterVAT - this.sumCostMoneyAfterVAT;
      }
    }
  }

  /*Tính số thống kê tab Chi tiết báo giá*/
  caculatorMoneyQuocte() {
    let index = 0;
    //Tiền thuế
    this.sumQuocteVAT = 0;

    //Giá trị sau thuế
    this.sumQuocteMoneyAfterVAT = 0;

    //Giá trị trước thuế
    this.sumMoneyQuoteBeforVAT = 0;

    this.listQuocteDetail.forEach(item => {
      let cacuDiscount = 0
      item.index = index + 1;
      let caculateVAT: number = 0;
      if (item.discountValue != null) {
        if (item.discountType == true) {
          cacuDiscount = ((item.quantity * item.unitPrice * item.exchangeRate * item.discountValue) / 100);
        }
        else {
          cacuDiscount = item.discountValue;
        }
      }

      if (item.vat != null) {
        caculateVAT = ((item.quantity * item.unitPrice * item.exchangeRate - cacuDiscount) * item.vat) / 100;
      }

      this.sumMoneyQuoteBeforVAT = this.sumMoneyQuoteBeforVAT + item.quantity * item.unitPrice * item.exchangeRate - cacuDiscount;
      item.taxMoney = caculateVAT;
      this.sumQuocteVAT = this.sumQuocteVAT + caculateVAT;
      item.sumAmount = (item.quantity * item.unitPrice * item.exchangeRate) + item.taxMoney - cacuDiscount;
      this.sumQuocteMoneyAfterVAT = this.sumQuocteMoneyAfterVAT + item.quantity * item.unitPrice * item.exchangeRate - cacuDiscount + caculateVAT;
      index++;
    });

    if (this.listQuocteDetail.length == 0 || this.listLeadDetail.length == 0) {
      this.ROS = 0;
      this.provisionalGrossProfit = 0;
    } else {
      this.ROS = this.roundNumber((this.sumQuocteMoneyAfterVAT - this.sumCostMoneyAfterVAT) * 100 / this.sumQuocteMoneyAfterVAT, 2);
      this.provisionalGrossProfit = this.sumQuocteMoneyAfterVAT - this.sumCostMoneyAfterVAT;
    }
  }

  /* Chuyển item lên một cấp */
  moveUp(data: LeadDetail) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listLeadDetail.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listLeadDetail = this.listLeadDetail.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listLeadDetail = [...this.listLeadDetail, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listLeadDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: LeadDetail) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listLeadDetail.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listLeadDetail = this.listLeadDetail.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listLeadDetail = [...this.listLeadDetail, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listLeadDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item lên một cấp */
  moveUpQuocteDetail(data: LeadDetail) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listQuocteDetail.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listQuocteDetail = this.listQuocteDetail.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listQuocteDetail = [...this.listQuocteDetail, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listQuocteDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDownQuocteDetail(data: LeadDetail) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listQuocteDetail.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listQuocteDetail = this.listQuocteDetail.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listQuocteDetail = [...this.listQuocteDetail, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listQuocteDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }
}

//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidator(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) > number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
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

//So sánh giá trị nhập vào có thuộc khoảng xác định hay không?
function ageRangeValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (isNaN(control.value) || control.value < min || control.value > max)) {
      return { 'ageRange': true };
    }
    return null;
  };
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
