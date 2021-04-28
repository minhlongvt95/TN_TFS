import { Component, OnInit, ElementRef, ViewChild, HostListener, AfterContentChecked, ChangeDetectorRef, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from '../../../../shared/services/category.service';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { CashReceiptModel } from '../../../models/cashReceipt.model';
import { AccountingService } from '../../../services/accounting.service';
import { DatePipe } from '@angular/common';

import * as $ from 'jquery';
import { CashReceiptMappingModel } from '../../../models/cashReceiptMapping.model';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { CustomerService } from '../../../../customer/services/customer.service';
import { VendorService } from '../../../../vendor/services/vendor.service';
import { ValidatorsCommon } from '../../../../shared/CustomValidation/ValidationCommon';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';

import { MenuItem } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { DecimalPipe } from '@angular/common';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string,
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
}

interface Order {
  orderId: string,
  orderCode: string,
  amountCollected: string,  //Số tiền nhận
  amountReceivable: number, //Số phải tiền phải thu
  total: number,
  orderDate: Date,
  error: boolean
}

@Component({
  selector: 'app-cash-receipts-create',
  templateUrl: './cash-receipts-create.component.html',
  styleUrls: ['./cash-receipts-create.component.css'],
  providers: [
    DecimalPipe,
  ]
})
export class CashReceiptsCreateComponent implements OnInit, AfterContentChecked {

  awaitResult: boolean = false;
  errorEmail: boolean = false;
  typesOfReceipt: Array<any> = [];
  organizationList: Array<any> = [];
  statusOfReceipt: Array<any> = [];
  unitMoney: Array<any> = [];
  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = localStorage.getItem('Username') + '-' + localStorage.getItem('UserFullName');
  currentDate: Date = new Date();
  cashReceiptModel = new CashReceiptModel();
  cashReceiptMappingModel = new CashReceiptMappingModel();
  reasonOfReceipt: any[];
  receipter: any;
  receipterList: Array<any> = [];
  currencyCode: any;
  isShowExchangeRate = false;
  isShowPriceForeign = false;
  priceForeign: number;
  currencyLabel: string = 'VND';
  listCurrency: Array<MenuItem> = [];
  cols: any[];
  selectedColumns: any[];

  createReceiptForm: FormGroup;
  reasonControl: FormControl;
  registerTypeControl: FormControl;
  receipters: FormControl;
  organizationName: FormControl;
  status: FormControl;
  receiptDate: FormControl;
  voucherDate: FormControl;
  receiptName: FormControl;
  recipientAddressControl: FormControl;
  content: FormControl;
  unitPrice: FormControl;
  currencyUnit: FormControl;
  exchangeRate: FormControl;
  noteControl: FormControl;


  currentDatestring: string;
  voucherDateString: string;

  customerId: string = null;

  loading: boolean = false;
  actionAdd: boolean = true;
  isCheckAmountGreater: boolean = false;
  isShowAllocation: boolean = false;
  listOrder: Array<any> = [];
  listOrderHistory: Array<any> = [];
  isAllocationFollowMoney: boolean = true;

  totalAmountReceivable: number = 0;
  maxVouchersDate: Date = new Date();
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;

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

  constructor(
    private ref: ChangeDetectorRef,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router,
    private accountingService: AccountingService,
    private employeeService: EmployeeService,
    private customerService: CustomerService,
    private decimalPipe: DecimalPipe,
    private vendorService: VendorService,
    private formBuilder: FormBuilder,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private renderer: Renderer2,
    public dialogService: DialogService,
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
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target) &&
          !this.saveAndCreate.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
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

  async ngOnInit() {
    let resource = "acc/accounting/cash-receipts-create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      //$("body").addClass("sidebar-collapse"); //Thu gọn left menu
      this.setForm();

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.route.params.subscribe(params => {
        this.customerId = params['customerId'];
      });

      this.loading = true;
      await this.getMasterData();
      this.loading = false;

      this.cols = [
        { field: 'orderCode', header: 'Mã đơn hàng', width: '15%', textAlign: 'left', color: '#f44336' },
        { field: 'amountCollected', header: 'Thanh toán', width: '20%', textAlign: 'right', color: '#f44336' },
        { field: 'amountReceivable', header: 'Số phải thu', width: '20%', textAlign: 'right', color: '#f44336' },
        { field: 'total', header: 'Tổng giá trị đơn hàng', width: '25%', textAlign: 'right', color: '#f44336' },
        { field: 'orderDate', header: 'Ngày mua', width: '20%', textAlign: 'center', color: '#f44336' }
      ];

      this.selectedColumns = this.cols;

    }
  }

  setForm() {
    this.reasonControl = new FormControl('');
    this.registerTypeControl = new FormControl('');
    this.receipters = new FormControl('', [Validators.required]);
    this.organizationName = new FormControl('', [Validators.required]);
    this.status = new FormControl('', [Validators.required]);
    this.receiptDate = new FormControl(new Date(), [Validators.required, ValidatorsCommon.formatDate]);
    this.voucherDate = new FormControl(new Date(), [Validators.required, ValidatorsCommon.formatDate]);
    this.receiptName = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.recipientAddressControl = new FormControl('', [Validators.maxLength(250), forbiddenSpaceText]);
    this.content = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.unitPrice = new FormControl('0', [Validators.required]);
    // this.currencyUnit = new FormControl('', [Validators.required]);
    this.exchangeRate = new FormControl('1', [Validators.required]);
    this.noteControl = new FormControl('');

    this.createReceiptForm = this.formBuilder.group({
      reasonControl: this.reasonControl,
      registerTypeControl: this.registerTypeControl,
      receipters: this.receipters,
      organizationName: this.organizationName,
      status: this.status,
      receiptDate: this.receiptDate,
      voucherDate: this.voucherDate,
      receiptName: this.receiptName,
      recipientAddressControl: this.recipientAddressControl,
      content: this.content,
      unitPrice: this.unitPrice,
      // currencyUnit: this.currencyUnit,
      exchangeRate: this.exchangeRate,
      noteControl: this.noteControl
    });
  }

  ngAfterContentChecked(): void {
    this.ref.detectChanges();
  }

  async getMasterData() {
    this.loading = true;
    var result: any = await this.accountingService.getMasterDataReceiptInvoice();
    this.reasonOfReceipt = result.reasonOfReceiptList;
    this.reasonOfReceipt.sort((a, b) => a.categoryName.localeCompare(b.categoryName));

    this.typesOfReceipt = result.typesOfReceiptList;
    this.typesOfReceipt.sort((a, b) => a.categoryName.localeCompare(b.categoryName));

    this.statusOfReceipt = result.statusOfReceiptList;

    this.unitMoney = result.unitMoneyList;
    this.organizationList = result.organizationList;

    this.receipterList = result.customerList;

    this.receipterList.forEach(item => {
      item.customerName = item.customerCode + ' - ' + item.customerName;
    });

    this.unitMoney.forEach(item => {
      let optionMenu: MenuItem = {
        label: item.categoryCode,
        command: () => {
          this.currencyLabel = item.categoryCode;
          this.setExchangeRate();
        }
      }
      this.listCurrency.push(optionMenu);
    });


    this.setDefaultPaidDate();
    //Set giá trị mặc định cho Đơn vị tiền:
    this.setDefaultCurrencyUnit();
    //Set giá trị mặc định cho Tỷ giá:
    this.setDefaultExchangeRate();
    //Lý do thu và Đối tượng thu
    this.setDefaultReason();
    //Nơi chi
    this.setDefaultOrg(); //Gia tri mac dinh
    //Trạng thái
    this.setDefaultStatus();  //Gia tri mac dinh
    this.setDefaultObject();
    this.loading = false;
  }

  async setDefaultReason() {
    const reasonCode = this.reasonOfReceipt.find(r => r.categoryCode === 'THA');
    this.createReceiptForm.controls['reasonControl'].setValue(reasonCode);

    this.receipter = 'THA';
  }

  setDefaultObject(){
    if(this.customerId){
      let customer : Customer = this.receipterList.find(c => c.customerId == this.customerId);
      this.createReceiptForm.controls['receipters'].setValue(customer);
      this.createReceiptForm.controls['receipters'].updateValueAndValidity();
      this.changeCustomer(customer);
    }
  }

  setDefaultRegisterType() {
    this.createReceiptForm.controls['registerTypeControl'].setValue("");
    this.createReceiptForm.controls['registerTypeControl'].updateValueAndValidity();
  }

  setDefaultOrg() {
    const org = this.organizationList.find(o => o.parentId === null);
    this.cashReceiptModel.OrganizationId = org.organizationId;
    this.createReceiptForm.controls['organizationName'].setValidators(null);
    this.createReceiptForm.controls['organizationName'].setValue(org.organizationName);
    this.createReceiptForm.controls['organizationName'].updateValueAndValidity();
  }

  setDefaultStatus() {
    const toSelectOrderStatus = this.statusOfReceipt.find(stt => stt.categoryCode === "DSO");
    this.createReceiptForm.controls['status'].setValue(toSelectOrderStatus);
  }

  setDefaultPaidDate() {
    var datePipe = new DatePipe("en-US");
    var today = new Date();
    let formatedyear = datePipe.transform(today, 'yyyy-MM-dd');
    this.currentDatestring = formatedyear;
    this.voucherDateString = formatedyear;
  }

  setDefaultCurrencyUnit() {
    const toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === 'VND');
    this.currencyLabel = toSelectMoneyUnit.categoryCode;
  }

  setDefaultExchangeRate() {
    this.createReceiptForm.controls['exchangeRate'].setValue('1');
    this.createReceiptForm.controls['exchangeRate'].setValidators(null);
    this.createReceiptForm.controls['exchangeRate'].updateValueAndValidity();
  }

  // tslint:disable-next-line:member-ordering
  price: number;
  // convenience getter for easy access to form fields
  get f() { return this.createReceiptForm.controls; }

  createReceipt(value: boolean) {
    if (!this.createReceiptForm.valid) {
      Object.keys(this.createReceiptForm.controls).forEach(key => {
        if (this.createReceiptForm.controls[key].valid == false) {
          this.createReceiptForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.createReceiptForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else if (!this.validationListOrder()) {
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: 'Số thanh toán của đơn hàng không hợp lệ' };
      this.showMessage(mgs);
    }
    else {
      this.cashReceiptModel.ReceiptInvoiceReason = this.reasonControl.value.categoryId;

      this.cashReceiptModel.RegisterType = this.registerTypeControl.value.categoryId;

      let receipter = this.receipters.value;
      if (this.receipter === 'THA') {
        this.cashReceiptMappingModel.ObjectId = receipter ? receipter.customerId : null;
      } else if (this.receipter === 'TTA') {
        this.cashReceiptMappingModel.ObjectId = receipter ? receipter.vendorId : null;
      } else {
        this.cashReceiptMappingModel.ObjectId = receipter ? receipter.employeeId : null;
      }

      //Trạng thái
      let status = this.status.value;
      this.cashReceiptModel.StatusId = status ? status.categoryId : null;

      //Ngày hạch toán
      let paidDate = this.receiptDate.value;
      paidDate = convertToUTCTime(paidDate);
      this.cashReceiptModel.ReceiptDate = paidDate;

      // Ngày chứng từ
      let vouchersDate = this.voucherDate.value;
      vouchersDate = convertToUTCTime(vouchersDate);
      this.cashReceiptModel.VouchersDate = vouchersDate;

      //Nội dung
      this.cashReceiptModel.ReceiptInvoiceDetail = this.content.value.trim();

      this.cashReceiptModel.RecipientName = this.receiptName.value.trim();

      this.cashReceiptModel.RecipientAddress = this.recipientAddressControl.value ? this.recipientAddressControl.value.trim() : '';
      
      this.cashReceiptModel.ReceiptInvoiceNote = this.noteControl.value ? this.noteControl.value.trim() : '';
      //Loại tiền (VND, USD,...)
      let toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === this.currencyLabel);
      this.cashReceiptModel.CurrencyUnit = toSelectMoneyUnit.categoryId;

      //Tiền (chưa tính tỷ giá)
      this.cashReceiptModel.UnitPrice = parseFloat(this.unitPrice.value.replace(/,/g, ''));

      //Tỷ giá
      this.cashReceiptModel.ExchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));

      //Thành tiền (Đã tính tỷ giá)
      if (!this.cashReceiptModel.Amount) {
        this.cashReceiptModel.Amount = 0;
      }

      for (let i = 0; i < this.listOrder.length; i++) {
        let listHis = {
          orderId: this.listOrder[i].orderId,
          amountCollected: this.listOrder[i].amountCollected,
          amount: this.listOrder[i].amountReceivable
        }
        this.listOrderHistory.push(listHis);
      }

      this.saveReceipt(value);
    }
  }

  saveReceipt(value: boolean) {
    this.awaitResult = true;
    this.loading = true;
    this.accountingService.createReceiptInvoice(this.cashReceiptModel, this.cashReceiptMappingModel,
      this.auth.UserId, this.listOrderHistory).subscribe(response => {
        const result = <any>response;
        this.loading = false;
        if (result.statusCode == 202 || result.statusCode == 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo phiếu thu tiền mặt thành công' };
          this.showMessage(mgs);

          if (value) {
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            }
            this.resetFormAndModel();
            this.awaitResult = false;
          } else {
            this.cancel();
            this.awaitResult = false;
          }
        } else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
          this.awaitResult = false;
        }
      }, error => { });
  }

  selectPaidDate() {
    let receiptDate: Date = this.receiptDate.value;
    this.maxVouchersDate = receiptDate;
    if (receiptDate < this.voucherDate.value) {
      this.voucherDate.setValue(receiptDate);
    }
  }

  resetFormAndModel() {
    /*Reset form and set default value*/
    this.setDefaultReason();  //Lý do chi
    this.receipters.reset();  //Đối tượng chi
    this.setDefaultRegisterType();  //Loại sổ
    this.setDefaultOrg(); //Nơi chi
    this.setDefaultStatus(); //Trạng thái
    this.setDefaultPaidDate();  //Ngày hạch toán
    this.receiptName.reset(); //Người nhận
    this.createReceiptForm.controls['recipientAddressControl'].reset(); //Địa chỉ
    this.createReceiptForm.controls['content'].reset(); //Nội dung chi
    this.createReceiptForm.controls['unitPrice'].reset(); //Số tiền
    this.setDefaultCurrencyUnit();  //set default value: Đơn vị tiền
    this.setDefaultExchangeRate();  //Tỷ giá
    this.isShowExchangeRate = false;
    this.isShowPriceForeign = false;
    this.createReceiptForm.controls['noteControl'].reset();
    this.unitPrice.setValue('0');
    /*End*/

    /*Reset model*/
    this.cashReceiptModel.RegisterType = null;  //Loại sổ
    this.cashReceiptMappingModel.ObjectId = null; //Đối tượng chi
    this.cashReceiptModel.RecipientName = null; //Người nhận
    this.cashReceiptModel.RecipientAddress = null; //Địa chỉ
    this.cashReceiptModel.ReceiptInvoiceDetail = null;  //Nội dung chi
    this.cashReceiptModel.UnitPrice = null; //Số tiền
    const toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === 'VND');
    this.cashReceiptModel.CurrencyUnit = toSelectMoneyUnit.categoryId; //Đơn vị tiền
    this.priceForeign = null; //Số tiền quy ra VND nếu đơn vị tiền không phải VND
    this.cashReceiptModel.ReceiptInvoiceNote = '';
    this.cashReceiptMappingModel.ObjectId = null;
    this.cashReceiptModel.Amount = null;
    this.listOrder = [];
    /*End*/
  }

  // Check Reason Pay Code
  changeReasonPay(value: Category) {
    this.receipterList = [];
    this.unitPrice.setValue('0');
    this.exchangeRate.setValue('1');
    this.cashReceiptModel.Amount = 0;
    // this.bankPaymentModel.OrganizationId = null;
    this.loading = true;

    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.receipter = result.category.categoryCode;

      if (this.receipter === 'TVI') {
        this.receipters.setValue('');
        this.createReceiptForm.controls['receipters'].setValidators(Validators.required);
        this.createReceiptForm.controls['receipters'].updateValueAndValidity();
        this.loading = true;
        this.employeeService.searchEmployee('', '', '', '', [], '').subscribe(response1 => {
          const result1 = <any>response1;
          this.loading = false;
          this.receipterList = result1.employeeList;
          this.receipterList.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
        }, error => { });
      } else if (this.receipter === 'TTA') {
        this.receipters.setValue('');
        this.createReceiptForm.controls['receipters'].setValidators(Validators.required);
        this.createReceiptForm.controls['receipters'].updateValueAndValidity();

        this.loading = true;
        this.vendorService.getAllVendorToPay().subscribe(response2 => {
          const result2 = <any>response2;
          this.loading = false;
          this.receipterList = result2.vendorList;
          this.receipterList.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
        }, error => { });
      } else if (this.receipter === 'THA') {
        this.receipters.setValue('');
        this.createReceiptForm.controls['receipters'].setValidators(Validators.required);
        this.createReceiptForm.controls['receipters'].updateValueAndValidity();

        this.loading = true;
        this.customerService.getAllCustomer().subscribe(response3 => {
          const result3 = <any>response3;
          this.loading = false;
          this.receipterList = result3.customerList;
          this.receipterList.forEach(item => {
            item.customerName = item.customerCode + ' - ' + item.customerName;
          });
        }, error => { });
      } else {
        this.createReceiptForm.controls['receipters'].setValidators(null);
        this.createReceiptForm.controls['receipters'].updateValueAndValidity();
      }
    }, error => { });

  }

  displayCustomer(value: any) {
    return typeof value === 'string' ? value : (value == null ? '' : value.customerName);
  }

  private _filterCustomer(value: string, array: any) {
    return array.filter(state =>
      (state.customerName != null && state.customerName.toLowerCase().indexOf(value.toLowerCase()) >= 0) ||
      (state.customerCode != null && state.customerCode.toLowerCase().indexOf(value.toLowerCase()) >= 0)
    );
  }

  // Choose first item when press enter key
  chooseFirstOption(): void {
    //this.matAutocomplete.options.first.select();
  }

  async changeCustomer(value: Customer) {
    this.listOrder = [];
    if (value) {
      this.loading = true;
      let result: any = await this.accountingService.getOrderByCustomerIdAsync(this.auth.UserId, value.customerId);
      this.loading = false;
      if (result.statusCode == 200) {
        this.listOrder = result.listOrder;
        this.totalAmountReceivable = result.totalAmountReceivable;

        if (this.listOrder.length > 0) {
          this.listOrder.forEach(item => {
            item.amountCollected = this.decimalPipe.transform(item.amountCollected.toString());
            item.error = false;
          });
        }

      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    }
    this.handleMoney();
  }

  //Tính lại ô Số tiền
  handleMoney() {
    let exchangeRate = 1;
    if (this.exchangeRate.value.trim() == '') {
      exchangeRate = 1;
    } else {
      exchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));
    }

    //Tính tổng tiền nhận của tất cả đơn hàng
    let totalAmountCollected = 0;
    this.listOrder.forEach(item => {
      if (item.amountCollected == '') {
        item.amountCollected = '0';
      } else {
        let amountCollected = parseFloat(item.amountCollected.replace(/,/g, ''));
        if (amountCollected > item.amountReceivable) {
          item.error = true;
        } else {
          item.error = false;
        }
      }
      let amountCollected = parseFloat(item.amountCollected.replace(/,/g, ''));
      totalAmountCollected += amountCollected;
    });

    //Gán lại cho ô Số tiền
    if (this.currencyLabel == 'VND') {
      this.unitPrice.setValue(totalAmountCollected.toString());
      this.cashReceiptModel.Amount = totalAmountCollected;
    } else {
      let unitPrice = this.roundNumber((totalAmountCollected / exchangeRate), parseInt(this.defaultNumberType, 10));
      this.unitPrice.setValue(unitPrice.toString());
      this.cashReceiptModel.Amount = totalAmountCollected;
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


  // Back to receipt list
  cancel() {
    this.router.navigate(['/accounting/cash-receipts-list']);
  }

  openOrgPopup() {
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: { 
        chooseFinancialIndependence: true //Nếu chỉ chọn đơn vị độc lập tài chính
      },
      header: 'Chọn đơn vị',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.cashReceiptModel.OrganizationId = result.selectedOrgId;
          this.createReceiptForm.controls['organizationName'].setValue(result.selectedOrgName);
        }
      }
    });
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  clearDataReceipter() {
    this.receipters.reset();
    this.cashReceiptMappingModel.ObjectId = null;
    this.listOrder = [];
    this.unitPrice.reset();
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  ngOnDestroy() {
    $("body").removeClass("sidebar-collapse");
  }

  setExchangeRate() {
    //Set value
    this.exchangeRate.setValue('1');
    if (this.currencyLabel == 'VND') {
      //Remove validators for FormControl
      this.exchangeRate.setValidators(null);
      this.exchangeRate.updateValueAndValidity();
    } else {
      //Add validators for FormControl
      this.exchangeRate.setValidators([Validators.required]);
      this.exchangeRate.updateValueAndValidity();
    }

    //Tính lại tiền khi thay đổi loại tiền tệ
    this.calculatorMoney();
  }

  calculatorMoney() {
    let unitPrice = 0;
    if (this.unitPrice.value.trim() == '') {
      unitPrice = 0;
      this.unitPrice.setValue('0');
    } else {
      unitPrice = parseFloat(this.unitPrice.value.replace(/,/g, ''));
    }

    let exchangeRate = 1;
    if (this.exchangeRate.value.trim() == '') {
      exchangeRate = 1;
      this.exchangeRate.setValue('1');
    } else {
      exchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));
    }

    if (this.currencyLabel == 'VND') {
      this.cashReceiptModel.Amount = unitPrice;
    } else {
      this.cashReceiptModel.Amount = unitPrice * exchangeRate;
    }
    this.reloadAmountCollected(this.cashReceiptModel.Amount);
  }

  //Tính lại số tiền nhận của các đơn hàng
  reloadAmountCollected(totalAmountCollected: number) {
    let total = totalAmountCollected;
    if (this.receipter == 'THA' && this.listOrder.length > 0) {
      this.listOrder.forEach(item => {
        item.error = false;
        item.amountCollected = '0';
        if (total >= item.amountReceivable) {
          item.amountCollected = this.decimalPipe.transform((item.amountReceivable).toString());
          total = total - item.amountReceivable;
        } else if (total != 0 && total < item.amountReceivable) {
          item.amountCollected = this.decimalPipe.transform(total.toString());
          total = 0;
        }
      });
    }
  }

  confirm(value: boolean) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn thanh toán vượt quá công nợ của khách hàng?',
      accept: () => {
        this.saveReceipt(value);
      }
    });
  }

  //Kiêm tra lỗi ở Bảng danh sách đơn hàng
  validationListOrder(): boolean {
    let result = true;

    if (this.listOrder.length > 0) {
      let errorItem = this.listOrder.find(x => x.error == true);
      if (errorItem) {
        result = false;
        return result;
      }
    }
    return result;
  }

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

