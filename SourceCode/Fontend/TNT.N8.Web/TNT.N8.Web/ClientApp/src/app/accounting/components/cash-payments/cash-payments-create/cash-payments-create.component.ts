import { Component, OnInit, ElementRef, ViewChild, HostListener, ChangeDetectorRef, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from '../../../../shared/services/category.service';
import { OrganizationService } from '../../../../shared/services/organization.service';

import { AccountingService } from '../../../services/accounting.service';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { CustomerService } from '../../../../customer/services/customer.service';
import { VendorService } from '../../../../vendor/services/vendor.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { MenuItem } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { DecimalPipe, DatePipe } from '@angular/common';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';
import { CashPaymentModel } from '../../..//models/cashPayment.model';
import { CashPaymentMappingModel } from '../../..//models/cashPaymentMapping.model';
import * as $ from 'jquery';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

@Component({
  selector: 'app-cash-payments-create',
  templateUrl: './cash-payments-create.component.html',
  styleUrls: ['./cash-payments-create.component.css'],
  providers: [
    DecimalPipe
  ]
})
export class CashPaymentsCreateComponent implements OnInit {

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      this.withFiexd = $('#parent').width() + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;

  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();


  loading: boolean = false;
  awaitResult: boolean = false;// khóa nút lưu
  
  //hash infor
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = localStorage.getItem('UserFullName');
  currentDate: Date = new Date();

  maxVouchersDate: Date = new Date();
  price: number;
  reasonOfPayment: Array<any> = [];
  typesOfPayment: Array<any> = [];
  organizationList: Array<any> = [];
  statusOfPayment: Array<any> = [];
  unitMoney: Array<any> = [];

  currentDatestring: string;
  voucherDateString: string;

  
  messageError: Array<string> = [];

  cashPaymentModel = new CashPaymentModel();
  cashPaymentMappingModel = new CashPaymentMappingModel();
  isShowExchangeRate = false;
  reasonId = null;
  categoryId = null;
  payer = '';
  payerList: any[];
  userId = this.auth.UserId;
  currencyCode: any;
  //exchangeRate: number;
  priceForeign: number;
  isShowPriceForeign = false;
  priceForeignString: string;

  /*FORM TẠO PHIẾU CHI TIỀN MẶT*/
  createPayableForm: FormGroup;
  reasonControl: FormControl;
  registerTypeControl: FormControl;
  payers: FormControl;
  organizationName: FormControl;
  status: FormControl;
  paidDate: FormControl;
  vouchersDate : FormControl;
  receiptName: FormControl;
  recipientAddressControl: FormControl;
  content: FormControl;
  unitPrice: FormControl;
  currencyUnit: FormControl;
  exchangeRate: FormControl;
  noteControl: FormControl;

  submitted = false;
  withFiexd: string;

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;


  fixed: boolean = false;
  listCurrency: Array<MenuItem> = [];
  currencyLabel: string = 'VND';
  /*Check user permission*/


  constructor(
    private ref: ChangeDetectorRef,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    private orgService: OrganizationService,
    private route: ActivatedRoute,
    private router: Router,
    private el: ElementRef,
    private accountingService: AccountingService,
    private employeeService: EmployeeService,
    private customerService: CustomerService,
    private vendorService: VendorService,
    private formBuilder: FormBuilder,
    private decimalPipe: DecimalPipe,
    private renderer: Renderer2,
    private messageService: MessageService,
    public dialogService: DialogService,
    private confirmationService: ConfirmationService
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

  async ngOnInit() {
    let resource = "acc/accounting/cash-payments-create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      this.setForm();

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.loading = true;
      await this.getMasterData();
      this.loading = false;
    }
  }

  setForm() {
    this.reasonControl = new FormControl('');
    this.registerTypeControl = new FormControl('');
    this.payers = new FormControl('', [Validators.required]);
    this.organizationName = new FormControl('', [Validators.required]);
    this.status = new FormControl('', [Validators.required]);
    this.paidDate = new FormControl(new Date(), [Validators.required]);
    this.vouchersDate = new FormControl(new Date(), [Validators.required]);
    this.receiptName = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.recipientAddressControl = new FormControl('', [Validators.maxLength(250), forbiddenSpaceText]);
    this.content = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.unitPrice = new FormControl('0', [Validators.required]);
    this.exchangeRate = new FormControl('', [Validators.required]);
    this.noteControl = new FormControl('');

    this.createPayableForm = new FormGroup({
      reasonControl: this.reasonControl,
      registerTypeControl: this.registerTypeControl,
      payers: this.payers,
      organizationName: this.organizationName,
      status: this.status,
      paidDate: this.paidDate,    //ValidatorsCommon.formatDate
      vouchersDate : this.vouchersDate,
      receiptName: this.receiptName,
      recipientAddressControl: this.recipientAddressControl,
      content: this.content,
      unitPrice: this.unitPrice,
      exchangeRate: this.exchangeRate,
      noteControl: this.noteControl
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngAfterContentChecked(): void {
    this.ref.detectChanges();
  }

  async getMasterData() {
    this.loading = true;
    var result: any = await this.accountingService.getMasterDataPayableInvoice();

    this.reasonOfPayment = result.reasonOfPaymentList;
    this.reasonOfPayment.sort((a, b) => a.categoryName.localeCompare(b.categoryName));

    this.typesOfPayment = result.typesOfPaymentList;
    this.typesOfPayment.sort((a, b) => a.categoryName.localeCompare(b.categoryName));

    this.statusOfPayment = result.statusOfPaymentList;
    this.unitMoney = result.unitMoneyList;
    this.organizationList = result.organizationList;
    this.payerList = result.customerList;
    this.payerList.forEach(item => {
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
    this.loading = false;
  }

async setDefaultReason() {
    const reasonCode = this.reasonOfPayment.find(r => r.categoryCode === 'CHA');
    this.createPayableForm.controls['reasonControl'].setValue(reasonCode);

    this.payer = 'CHA';
  }

  setDefaultRegisterType() {
    this.createPayableForm.controls['registerTypeControl'].setValue("");
    this.createPayableForm.controls['registerTypeControl'].updateValueAndValidity();
  }

  setDefaultOrg() {
    const org = this.organizationList.find(o => o.parentId === null);
    this.cashPaymentModel.OrganizationId = org.organizationId;
    this.createPayableForm.controls['organizationName'].setValidators(null);
    this.createPayableForm.controls['organizationName'].setValue(org.organizationName);
    this.createPayableForm.controls['organizationName'].updateValueAndValidity();
  }

  setDefaultStatus() {
    const toSelectOrderStatus = this.statusOfPayment.find(stt => stt.categoryCode === "DSO");
    this.createPayableForm.controls['status'].setValue(toSelectOrderStatus);
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
    this.createPayableForm.controls['exchangeRate'].setValue('1');
    this.createPayableForm.controls['exchangeRate'].setValidators(null);
    this.createPayableForm.controls['exchangeRate'].updateValueAndValidity();
  }

  selectPaidDate() {
    let paidDate: Date = this.paidDate.value;
    this.maxVouchersDate = paidDate;
    if (paidDate < this.vouchersDate.value) {
      this.vouchersDate.setValue(paidDate);
    }
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

    if (this.currencyLabel == 'VND') {
      this.cashPaymentModel.Amount = unitPrice;
    } else {
      let exchangeRate = 1;
      if (this.exchangeRate.value.trim() == '') {
        exchangeRate = 1;
        this.exchangeRate.setValue('1');
      } else {
        exchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));
      }
      this.cashPaymentModel.Amount = unitPrice * exchangeRate;
    }
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
          this.cashPaymentModel.OrganizationId = result.selectedOrgId;
          this.createPayableForm.controls['organizationName'].setValue(result.selectedOrgName);
        }
      }
    });
  }

  // convenience getter for easy access to form field
  get f() { return this.createPayableForm.controls; }

  createPayable(value: boolean) {
    if (!this.createPayableForm.valid) {
      Object.keys(this.createPayableForm.controls).forEach(key => {
        if (this.createPayableForm.controls[key].valid == false) {
          this.createPayableForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.createPayableForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else {
      // Lý do chi
      this.cashPaymentModel.PayableInvoiceReason = this.reasonControl.value.categoryId;

      // Sổ chi
      this.cashPaymentModel.RegisterType = this.registerTypeControl.value.categoryId;

      // Đối tượng chi
      let payer = this.payers.value;
      if (this.payer === 'CHA') {
        this.cashPaymentMappingModel.ObjectId = payer ? payer.customerId : null;
      } else if (this.payer === 'CTA') {
        this.cashPaymentMappingModel.ObjectId = payer ? payer.vendorId : null;
      } else {
        this.cashPaymentMappingModel.ObjectId = payer ? payer.employeeId : null;
      }

      //Trạng thái
      let status = this.status.value;
      this.cashPaymentModel.StatusId = status ? status.categoryId : null;

      //Ngày hạch toán
      let paidDate = this.paidDate.value;
      paidDate = convertToUTCTime(paidDate);
      this.cashPaymentModel.PaidDate = paidDate;

      // Ngày chứng từ
      let vouchersDate = this.vouchersDate.value;
      vouchersDate = convertToUTCTime(vouchersDate);
      this.cashPaymentModel.VouchersDate = vouchersDate;
    

      this.cashPaymentModel.RecipientName = this.receiptName.value ? this.receiptName.value.trim() : "";
      this.cashPaymentModel.RecipientAddress = this.recipientAddressControl.value ? this.recipientAddressControl.value.trim() : "";
      this.cashPaymentModel.PayableInvoiceDetail = this.content.value ? this.content.value.trim() : "";
      this.cashPaymentModel.PayableInvoiceNote = this.noteControl.value ? this.noteControl.value.trim() : "";

      //Nội dung
      this.cashPaymentModel.PayableInvoiceDetail = this.content.value.trim();

      //Loại tiền (VND, USD,...)
      let toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === this.currencyLabel);
      this.cashPaymentModel.PayableInvoicePriceCurrency = toSelectMoneyUnit.categoryId;

      //Tiền (chưa tính tỷ giá)
      this.cashPaymentModel.PayableInvoicePrice = parseFloat(this.unitPrice.value.replace(/,/g, ''));

      //Tỷ giá
      this.cashPaymentModel.ExchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));

      //Thành tiền (Đã tính tỷ giá)
      if (!this.cashPaymentModel.Amount) {
        this.cashPaymentModel.Amount = 0;
      }

      this.saveCashPayment(value);
    }
  }

  saveCashPayment(value: boolean) {
    this.awaitResult = true;
    this.loading = true;
    this.accountingService.createPayableInvoice(this.cashPaymentModel, this.cashPaymentMappingModel,
      this.auth.UserId).subscribe(response => {
        const result = <any>response;
        this.loading = false;
        if (result.statusCode === 202 || result.statusCode === 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo phiếu chi thành công' };
          this.showMessage(mgs);
          if (value) {
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            } else {
              this.resetFormAndModel();
              this.awaitResult = false;
            }
          } else {
            this.cancel();
            this.awaitResult = false;
          }
        }else{
          let mgs = { severity:'error', summary: 'Thông báo:', detail: result.messageCode};
            this.showMessage(mgs);
            this.awaitResult = false;
        }
      }, error => {});
  }

  cancel() {
    this.router.navigate(['/accounting/cash-payments-list']);
  }

  showErrorMessage(msg: Array<string>) {
    msg.forEach(item => {
      this.messageService.add({ severity: 'error', summary: 'Error Message', detail: item });
    });
  }
  resetFormAndModel() {
    /*Reset form and set default value*/
    this.setDefaultReason();  //Lý do chi
    this.payers.reset();  //Đối tượng chi
    this.setDefaultRegisterType();  //Loại sổ
    this.setDefaultOrg(); //Nơi chi
    this.setDefaultStatus(); //Trạng thái
    this.setDefaultPaidDate();  //Ngày hạch toán
    this.receiptName.reset(); //Người nhận
    this.createPayableForm.controls['recipientAddressControl'].reset(); //Địa chỉ
    this.createPayableForm.controls['content'].reset(); //Nội dung chi
    this.createPayableForm.controls['unitPrice'].reset(); //Số tiền
    this.setDefaultCurrencyUnit();  //set default value: Đơn vị tiền
    this.setDefaultExchangeRate();  //Tỷ giá
    this.isShowExchangeRate = false;
    this.isShowPriceForeign = false;
    this.createPayableForm.controls['noteControl'].reset();
    this.unitPrice.setValue('0');
    /*End*/


    /*Reset model*/
    this.cashPaymentModel.RegisterType = null;  //Loại sổ
    this.cashPaymentMappingModel.ObjectId = null; //Đối tượng chi
    this.cashPaymentModel.RecipientName = null; //Người nhận
    this.cashPaymentModel.RecipientAddress = null; //Địa chỉ
    this.cashPaymentModel.PayableInvoiceDetail = null;  //Nội dung chi
    this.cashPaymentModel.PayableInvoicePrice = null; //Số tiền
    const toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === 'VND');
    this.cashPaymentModel.PayableInvoicePriceCurrency = toSelectMoneyUnit.categoryId; //Đơn vị tiền
    this.priceForeign = null; //Số tiền quy ra VND nếu đơn vị tiền không phải VND
    this.cashPaymentModel.PayableInvoiceNote = '';
    this.cashPaymentMappingModel.ObjectId = null;
    /*End*/
  }

  // Check Reason Pay Code
  changeReasonPay(value: any) {
    this.loading = true;
    this.payerList = [];
    this.unitPrice.setValue('0');
    this.exchangeRate.setValue('1');

    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.payer = result.category.categoryCode;
      if (this.payer === 'CVI') {
        this.payers.setValue('');
        this.createPayableForm.controls['payers'].setValidators(Validators.required);
        this.createPayableForm.controls['payers'].updateValueAndValidity();
        this.loading = true;
        this.employeeService.searchEmployee('', '', '', '', [], '').subscribe(response1 => {
          const result1 = <any>response1;
          this.loading = false;
          this.payerList = result1.employeeList;
          this.payerList.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
        }, error => { });
      } else if (this.payer === 'CTA') {
        this.payers.setValue('');
        this.createPayableForm.controls['payers'].setValidators(Validators.required);
        this.createPayableForm.controls['payers'].updateValueAndValidity();
        this.loading = true;
        this.vendorService.getAllVendorToPay().subscribe(response2 => {
          const result2 = <any>response2;
          this.loading = false;
          this.payerList = result2.vendorList;
          this.payerList.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
        }, error => { });
      } else if (this.payer === 'CHA') {
        this.payers.setValue('');
        this.createPayableForm.controls['payers'].setValidators(Validators.required);
        this.createPayableForm.controls['payers'].updateValueAndValidity();
        this.customerService.getAllCustomer().subscribe(response3 => {
          const result3 = <any>response3;
          this.loading = false;
          this.payerList = result3.customerList;
          this.payerList.forEach(item => {
            item.customerName = item.customerCode + ' - ' + item.customerName;
          });
        }, error => { });
      } else {
        this.createPayableForm.controls['payers'].setValidators(null);
        this.createPayableForm.controls['payers'].updateValueAndValidity();
      }
    }, error => { });
    this.loading = false;
  }

  // Check currency code
  changeCurrencyCode(value: any) {
    this.categoryService.getCategoryById(value).subscribe(response => {
      const result = <any>response;
      this.currencyCode = result.category.categoryCode;
      if (this.currencyCode !== 'VND') {
        this.createPayableForm.controls['exchangeRate'].setValidators([Validators.required]);
        this.createPayableForm.controls['exchangeRate'].updateValueAndValidity();
        this.isShowExchangeRate = true;
        this.isShowPriceForeign = true;
      } else if (this.currencyCode === 'VND') {
        this.createPayableForm.controls['exchangeRate'].setValidators(null);
        this.createPayableForm.controls['exchangeRate'].updateValueAndValidity();
        this.isShowExchangeRate = false;
        this.isShowPriceForeign = false;
        this.cashPaymentModel.ExchangeRate = 1;
      }
    }, error => { });
  }

  // Convert Currency
  convertCurrency() {
    this.priceForeign = this.cashPaymentModel.PayableInvoicePrice *
      (this.cashPaymentModel.ExchangeRate == null ? 1 : this.cashPaymentModel.ExchangeRate);
    return this.priceForeign;
  }

  //Filter Validator ngày hạch toán không < ngày hiện tại
  myFilter = (d: Date): boolean => {
    let day = d;
    var n = day.setHours(23, 59, 59);
    const now = new Date();
    return (day >= now);
  }

  onKeyPress(event: any) {
    const pattern = /^[0-9\.]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  clearDataPayer() {
    this.payers.reset();
    this.cashPaymentMappingModel.ObjectId = null;
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
