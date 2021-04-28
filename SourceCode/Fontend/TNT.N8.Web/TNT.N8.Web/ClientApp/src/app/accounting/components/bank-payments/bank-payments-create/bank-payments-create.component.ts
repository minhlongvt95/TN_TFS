import { Component, OnInit, ElementRef, ViewChild, HostListener, ChangeDetectorRef, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from '../../../../shared/services/category.service';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { BankPaymentModel } from '../../../models/bankPayment.model';
import { BankPaymentMappingModel } from '../../../models/bankPaymentMapping.model';

import { AccountingService } from '../../../services/accounting.service';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { CustomerService } from '../../../../customer/services/customer.service';
import { VendorService } from '../../../../vendor/services/vendor.service';
import { BankService } from '../../../../shared/services/bank.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { MenuItem } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { DecimalPipe } from '@angular/common';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface Bank {
  bankAccountId: string,
  bankName: string
}

@Component({
  selector: 'app-bank-payments-create',
  templateUrl: './bank-payments-create.component.html',
  styleUrls: ['./bank-payments-create.component.css'],
  providers: [
    DecimalPipe,
  ]
})
export class BankPaymentsCreateComponent implements OnInit {
  fixed: boolean = false;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageXOffset;
    if (num > 100) {
      this.fixed = true;
    } else {
      this.fixed = false;
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

  //parameter of master data
  reasonCode: string = 'LCH';
  registerTypeCode: string = 'LSO';
  receiptStatusCode: string = 'TCH';
  unitMoneyCode: string = 'DTI';

  //hash infor
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = localStorage.getItem('UserFullName');
  currentDate: Date = new Date();


  // Biến lưu giá trị trả về
  price: number;
  maxVouchersDate: Date = new Date();
  reasonOfPayment: Array<Category> = [];
  typesOfPayment: Array<Bank> = [];
  payerList = <any>[];
  organizationList: Array<any> = [];
  statusOfPayment: Array<Category> = [];
  maxPaidDate: Date = new Date();
  bankList: Array<any> = [];
  listCurrency: Array<MenuItem> = [];
  currencyLabel: string = 'VND';
  unitMoney: Array<Category> = [];
  bankAccountNumber : string;
  bankAccountName : string;
  bankName : string;
  bankBranchName : string;
  selectedColumns: any[];
  totalAmountReceivable: number = 0;
  bankPaymentModel = new BankPaymentModel();
  bankPaymentMappingModel = new BankPaymentMappingModel();
  payer = '';
  isEmp : boolean = false;
  //End

  /*FORM ỦY NHIỆM CHI */
  createPayableForm: FormGroup;
  reasonControl: FormControl;
  bankAccountControl: FormControl;
  payers: FormControl;
  organization: FormControl;
  status: FormControl;
  paidDate: FormControl;
  vouchersDate: FormControl;
  content: FormControl;
  unitPrice: FormControl;
  exchangeRate: FormControl;
  noteControl: FormControl;
  receiveBankControl: FormControl;

  ;
  /*END*/

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;

  constructor(
    private ref: ChangeDetectorRef,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    private orgService: OrganizationService,
    public bankService: BankService,
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
  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  async ngOnInit() {
    let resource = "acc/accounting/bank-payments-create/";
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
    this.bankAccountControl = new FormControl('');
    this.organization = new FormControl('', [Validators.required]);
    this.status = new FormControl('', [Validators.required]);
    this.paidDate = new FormControl(new Date(), [Validators.required]);
    this.vouchersDate = new FormControl(new Date(), [Validators.required]);
    this.content = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.unitPrice = new FormControl('0', [Validators.required]);
    this.exchangeRate = new FormControl('1', [Validators.required]);
    this.noteControl = new FormControl('');
    this.payers = new FormControl('', [Validators.required]);
    this.receiveBankControl = new FormControl ('', [Validators.required]);

    this.createPayableForm = this.formBuilder.group({
      reasonControl: this.reasonControl,
      bankAccountControl: this.bankAccountControl,
      payers: this.payers,
      organization: this.organization,
      status: this.status,
      paidDate: this.paidDate,
      vouchersDate: this.vouchersDate,
      content: this.content,
      unitPrice: this.unitPrice,
      exchangeRate: this.exchangeRate,
      receiveBankControl : this.receiveBankControl,
    })
  }
  async getMasterData() {
    this.loading = true;
    var result : any = await this.accountingService.getMasterDataBankPayableInvoice();
    
    this.reasonOfPayment = result.reasonOfPaymentList;
    this.reasonOfPayment.sort((a, b) => a.categoryName.localeCompare(b.categoryName));

    this.typesOfPayment = result.typesOfPaymentList;
    this.typesOfPayment.sort((a, b) => a.bankName.localeCompare(b.bankName));

    this.statusOfPayment = result.statusOfPaymentList;
    this.unitMoney = result.unitMoneyList;
    this.organizationList = result.organizationList;
    this.payerList = result.vendorList;
    this.payerList.forEach(item => {
      item.vendorName = item.vendorCode + ' - ' + item.vendorName;
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

  ngAfterContentChecked(): void {
    this.ref.detectChanges();
  }

  setDefaultOrg() {
    const org = this.organizationList.find(o => o.parentId === null);
    this.bankPaymentModel.OrganizationId = org.organizationId;
    this.createPayableForm.controls['organization'].setValidators(null);
    this.createPayableForm.controls['organization'].setValue(org.organizationName);
    this.createPayableForm.controls['organization'].updateValueAndValidity();
  }

  async setDefaultReason() {
    const toSelectOrderStatus = this.reasonOfPayment.find(c => c.categoryCode === 'CTA');
    
    this.createPayableForm.controls['reasonControl'].setValue(toSelectOrderStatus);
    this.payer = 'CTA';
    let result: any = await this.vendorService.getAllVendorToPayAsync();
    this.payerList = result.vendorList;
    this.payerList.forEach((item: { vendorName: string; vendorCode: string; }) => {
      item.vendorName = item.vendorCode + ' - ' + item.vendorName;
    });
  }

  setDefaultStatus() {
    const toSelectOrderStatus = this.statusOfPayment.find(stt => stt.categoryCode === "DSO");
    this.createPayableForm.controls['status'].setValue(toSelectOrderStatus);
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
      //Lý do chi
      this.bankPaymentModel.BankPayableInvoiceReason = this.reasonControl.value.categoryId;

      // Tài khoản chi
      let bankAccount: Bank = this.bankAccountControl.value;
      this.bankPaymentModel.BankPayableInvoiceBankAccountId = bankAccount ? bankAccount.bankAccountId : null;

      //Đối tượng chi
      let payer = this.payers.value;
      if (this.payer === 'CHA') {
        this.bankPaymentMappingModel.ObjectId = payer ? payer.customerId : null;
      } else if (this.payer === 'CTA') {
        this.bankPaymentMappingModel.ObjectId = payer ? payer.vendorId : null;
      } else {
        this.bankPaymentMappingModel.ObjectId = payer ? payer.employeeId : null;
      }

      //Trạng thái
      let status = this.status.value;
      this.bankPaymentModel.StatusId = status ? status.categoryId : null;

      //Ngày hạch toán
      let paidDate = this.paidDate.value;
      paidDate = convertToUTCTime(paidDate);
      this.bankPaymentModel.BankPayableInvoicePaidDate = paidDate;

      // Ngày chứng từ
      let vouchersDate = this.vouchersDate.value;
      vouchersDate = convertToUTCTime(vouchersDate);
      this.bankPaymentModel.VouchersDate = vouchersDate;
    
      //Nội dung
      this.bankPaymentModel.BankPayableInvoiceDetail = this.content.value.trim();

      //Loại tiền (VND, USD,...)
      let toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === this.currencyLabel);
      this.bankPaymentModel.BankPayableInvoicePriceCurrency = toSelectMoneyUnit.categoryId;

       //Tiền (chưa tính tỷ giá)
       this.bankPaymentModel.BankPayableInvoicePrice = parseFloat(this.unitPrice.value.replace(/,/g, ''));

       //Tỷ giá
      this.bankPaymentModel.BankPayableInvoiceExchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));

      //Thành tiền (Đã tính tỷ giá)
      if (!this.bankPaymentModel.BankPayableInvoiceAmount) {
        this.bankPaymentModel.BankPayableInvoiceAmount = 0;
      }
      this.bankPaymentModel.ReceiveAccountName = this.receiveBankControl.value.accountName;
      this.bankPaymentModel.ReceiveAccountNumber = this.receiveBankControl.value.accountNumber;
      this.bankPaymentModel.ReceiveBankName = this.receiveBankControl.value.bankName;
      this.bankPaymentModel.ReceiveBranchName = this.receiveBankControl.value.branchName;
      this.saveBankPayment(value);
    }
  }

  saveBankPayment(value : boolean){
    this.awaitResult = true;
    this.loading = true;
    this.accountingService.createBankPayableInvoice(this.bankPaymentModel, this.bankPaymentMappingModel, this.auth.UserId).subscribe(response =>{
      const result = <any> response;
      this.loading = false;
      if(result.statusCode === 202 || result.statusCode === 200){
        let mgs = { severity:'success', summary: 'Thông báo:', detail: 'Tạo ủy nhiệm chi thành công'};
        this.showMessage(mgs);
          
        if(value){
          if (this.emitStatusChangeForm) {
            this.emitStatusChangeForm.unsubscribe();
            this.isInvalidForm = false; //Ẩn icon-warning-active
          }
          this.resetFieldValue();
          this.awaitResult = false;
        }else{
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

  showBankControl(value) {
    if (value) {
        this.isEmp = false;
        this.createPayableForm.controls['receiveBankControl'].setValidators(Validators.required);
        this.createPayableForm.controls['receiveBankControl'].updateValueAndValidity();
    } else {
        this.isEmp = true;
        this.createPayableForm.controls['receiveBankControl'].setValidators(null)
        this.createPayableForm.controls['receiveBankControl'].updateValueAndValidity();
    }
}

  selectPaidDate() {
    let paidDate: Date = this.paidDate.value;
    this.maxVouchersDate = paidDate;
    if (paidDate < this.vouchersDate.value) {
      this.vouchersDate.setValue(paidDate);
    }
  }

  resetFieldValue() {
    this.setDefaultReason(); //Lý do thu
    this.payers.reset(); //Đối tượng thu
    this.bankAccountControl.reset(); //Tài khoản thu
    this.setDefaultOrg(); //Nơi thu
    this.setDefaultStatus(); //Trạng thái
    this.paidDate.setValue(new Date()); //Ngày hạch toán
    this.vouchersDate.setValue(new Date()); //Ngày chứng từ
    this.content.reset(); //Nội dung thu
    this.unitPrice.setValue('0'); //Số tiền
    this.setDefaultCurrencyUnit(); //Loại tiền (VND, USD,...)
    this.setDefaultExchangeRate(); //Tỷ giá
    this.noteControl.reset(); //Ghi chú
    this.receiveBankControl.setValue('');
    this.receiveBankControl.updateValueAndValidity();
    this.bankAccountNumber = '';
    this.bankAccountName = '';
    this.bankName ='';
    this.bankBranchName = '';
    this.bankList = [];

    this.bankPaymentModel.BankPayableInvoiceAmount = 0; //Thành tiền (Số tiền đã nhân với tỷ giá)
  }

  cancel() {
    this.router.navigate(['/accounting/bank-payments-list']);
  }

  getBankAccount(id: string, type: string) {
    this.bankService.getAllBankAccountByObject(id, type).subscribe(response => {
      var result = <any>response;
      this.bankList = result.bankAccountList;
    }, error => { });
  }

  // Check Reason Pay Code
  changeReasonPay(value: Category) {
    this.payerList = [];
    this.unitPrice.setValue('0');
    this.exchangeRate.setValue('1');
    this.bankPaymentModel.BankPayableInvoiceAmount = 0;
    // this.bankPaymentModel.OrganizationId = null;
    this.loading = true;
    
    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.payer = result.category.categoryCode;
      
      if (this.payer === 'CVI') {
        this.payers.setValue('');
        this.createPayableForm.controls['payers'].setValidators(Validators.required);
        this.createPayableForm.controls['payers'].updateValueAndValidity();
        this.showBankControl(false);
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
        this.showBankControl(true);
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
        this.showBankControl(true);
        this.loading = true;
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
        this.showBankControl(false);
      }
    }, error => { });
  }

  changeObjectPay(value : any){
    if(value){
      if(this.payer === 'CTA'){
        this.bankService.getAllBankAccountByObject(value.vendorId, 'VEN').subscribe(response =>{
          const result = <any> response;
          this.bankList = result.bankAccountList;
        });
      }else if(this.payer === 'CHA'){
        this.bankService.getAllBankAccountByObject(value.customerId, 'CUS').subscribe(response1 =>{
            const result1 = <any> response1;
            this.bankList = result1.bankAccountList;
        });
      }
    }
    this.clearDataPayer();
  }

  changBankAccount(value : any){
    if(value){
      this.bankAccountNumber = value.accountNumber;
      this.bankAccountName = value.accountName;
      this.bankName = value.bankName;
      this.bankBranchName = value.branchName;
    }else{
      this.bankAccountNumber = '';
      this.bankAccountName = '';
      this.bankName ='';
      this.bankBranchName = '';
    }
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
      this.bankPaymentModel.BankPayableInvoiceAmount = unitPrice;
    } else {
      this.bankPaymentModel.BankPayableInvoiceAmount = unitPrice * exchangeRate;
    }
    // this.reloadAmountCollected(this.bankPaymentModel.BankPayableInvoiceAmount);
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
          this.bankPaymentModel.OrganizationId = result.selectedOrgId;
          this.createPayableForm.controls['organization'].setValue(result.selectedOrgName);
        }
      }
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  clearDataPayer() {
    this.bankPaymentModel.ReceiveBankName = null;
    this.bankPaymentModel.ReceiveAccountName = null;
    this.bankPaymentModel.ReceiveAccountNumber = null;
    this.bankPaymentModel.ReceiveBranchName = null;
    this.bankAccountNumber = '';
    this.bankAccountName = '';
    this.bankName ='';
    this.bankBranchName = '';
    this.receiveBankControl.setValue('');
    this.receiveBankControl.updateValueAndValidity();
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
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
