import { 
  Component, OnInit, ElementRef, 
  ViewChild, AfterContentChecked, 
  ChangeDetectorRef, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from '../../../../shared/services/category.service';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { BankReceiptModel } from '../../../models/bankReceipt.model';
import { BankReceiptMappingModel } from '../../../models/bankReceiptMapping.model';
import { BankService } from '../../../../shared/services/bank.service';

import { AccountingService } from '../../../services/accounting.service';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { CustomerService } from '../../../../customer/services/customer.service';
import { VendorService } from '../../../../vendor/services/vendor.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';
import { MenuItem } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { DecimalPipe } from '@angular/common';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface Bank {
  bankAccountId: string,
  bankName: string
}

interface Customer {
  customerId: string
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
  selector: 'app-bank-receipts-create',
  templateUrl: './bank-receipts-create.component.html',
  styleUrls: ['./bank-receipts-create.component.css'],
  providers: [
    DecimalPipe
  ]
})

export class BankReceiptsCreateComponent implements OnInit, AfterContentChecked {

  // fixed: boolean = false;
  // withFiexd:string = "";
  // @HostListener('document:scroll', [])
  // onScroll(): void {
  //   let num = window.pageYOffset;
  //   if (num > 100) {
  //     this.fixed = true;
  //     this.withFiexd = $('#parent').width() +'px';
  //   } else {
  //     this.fixed = false;
  //     this.withFiexd = "";
  //   }
  // }

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();

  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới

  //parameter of master data
  reasonCode: string = 'LTH';
  registerTypeCode: string = 'LSO';
  receiptStatusCode: string = 'TCH';
  unitMoneyCode: string = 'DTI';

  //hash infor
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = localStorage.getItem('UserFullName');
  currentDate: Date = new Date();
  
  //Declare result
  reasonOfReceipt: Array<Category> = [];
  typesOfReceipt: Array<Bank> = [];
  reperList = <any>[];
  organizationList: Array<any> = [];
  statusOfReceipt: Array<Category> = [];
  maxPaidDate: Date = new Date();
  maxVouchersDate: Date = new Date();
  listCurrency: Array<MenuItem> = [];
  currencyLabel: string = 'VND';
  unitMoney: Array<Category> = [];
  listOrder: Array<Order> = [];
  cols: any[];
  selectedColumns: any[];
  listOrderHistory: Array<any> = [];
  totalAmountReceivable: number = 0;
  bankReceiptModel = new BankReceiptModel();
  bankReceiptMappingModel = new BankReceiptMappingModel();
  reper = '';
  customerId: string = null;  //Nhận param CustomerId từ màn hình khác đến nếu có
  //End

  //Declare Form
  createReceiptForm: FormGroup;
  reasonControl: FormControl;
  bankAccountControl: FormControl;
  repper: FormControl;
  organizationName: FormControl;
  status: FormControl;
  paidDate: FormControl;
  vouchersDate: FormControl;
  content: FormControl;
  unitPrice: FormControl;
  exchangeRate: FormControl;
  noteControl: FormControl;

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
    private route: ActivatedRoute,
    private router: Router,
    private bankService: BankService,
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
    this.renderer.listen('window', 'click', (e:Event) => {
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
    let resource = "acc/accounting/bank-receipts-create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity:'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ'};
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      this.setForm();

      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      //Tạo báo có từ Customer:
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
        { field: 'total',header: 'Tổng giá trị đơn hàng', width: '25%', textAlign: 'right', color: '#f44336' },
        { field: 'orderDate', header: 'Ngày mua', width: '20%', textAlign: 'center', color: '#f44336' }
      ];

      this.selectedColumns = this.cols;
    }
  }

  setForm() {
    this.reasonControl = new FormControl('');
    this.bankAccountControl = new FormControl('');
    this.repper = new FormControl('', [Validators.required]);
    this.organizationName = new FormControl('', [Validators.required]);
    this.status = new FormControl('', [Validators.required]);
    this.paidDate = new FormControl(new Date(), [Validators.required]);
    this.vouchersDate = new FormControl(new Date(), [Validators.required]);
    this.content = new FormControl('', [Validators.required, Validators.maxLength(250), forbiddenSpaceText]);
    this.unitPrice = new FormControl('0', [Validators.required]);
    this.exchangeRate = new FormControl('1', [Validators.required]);
    this.noteControl = new FormControl('');

    this.createReceiptForm = this.formBuilder.group({
      reasonControl: this.reasonControl,
      bankAccountControl: this.bankAccountControl,
      repper: this.repper,
      organizationName: this.organizationName,
      status: this.status,
      paidDate: this.paidDate,
      vouchersDate: this.vouchersDate,
      content: this.content,
      unitPrice: this.unitPrice,
      exchangeRate: this.exchangeRate,
      noteControl: this.noteControl,
    });
  }

  async getMasterData() {
    //Đơn vị tiền
    let listUnitMoneyResult: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc(this.unitMoneyCode);
    this.unitMoney = listUnitMoneyResult.category;

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
    let listReasonResult: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc(this.reasonCode);
    this.reasonOfReceipt = listReasonResult.category;
    this.setDefaultReason();

    //Tài khoản thu
    let listBankAccountResult: any = await this.bankService.getCompanyBankAccountAsync(this.auth.UserId);
    this.typesOfReceipt = listBankAccountResult.bankList;

    //Nơi thu
    let listOrganizationResult: any = await this.orgService.getFinancialindependenceOrgAsync();
    this.organizationList = listOrganizationResult.listOrg;
    this.setDefaultOrg(); //Gia tri mac dinh

    //Trạng thái
    let listStatusResult: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc(this.receiptStatusCode);
    this.statusOfReceipt = listStatusResult.category;
    this.setDefaultStatus();  //Gia tri mac dinh
  }

  ngAfterContentChecked() : void {
    this.ref.detectChanges();
  }

  async setDefaultReason() {
    const toSelectOrderStatus = this.reasonOfReceipt.find(c => c.categoryCode === 'THA');
    this.createReceiptForm.controls['reasonControl'].setValue(toSelectOrderStatus);
    this.reper = 'THA';

    let result: any = await this.customerService.getAllCustomerAsync();
    this.reperList = result.customerList;
    this.reperList.forEach(item => {
      item.customerName = item.customerCode + ' - ' + item.customerName;
    });

    if (this.customerId != null) {
      const toSelectCustomer = this.reperList.find(c => c.customerId === this.customerId);
      this.repper.setValue(toSelectCustomer);
      this.bankReceiptMappingModel.ObjectId = toSelectCustomer.customerId;

      this.changeCustomer(toSelectCustomer);
    }
  }

  setDefaultOrg() {
    const org = this.organizationList.find(o => o.parentId === null);
    this.bankReceiptModel.OrganizationId = org.organizationId;
    this.createReceiptForm.controls['organizationName'].setValidators(null);
    this.createReceiptForm.controls['organizationName'].setValue(org.organizationName);
    this.createReceiptForm.controls['organizationName'].updateValueAndValidity();
  }

  setDefaultStatus() {
    const toSelectOrderStatus = this.statusOfReceipt.find(stt => stt.categoryCode === "DSO");
    this.createReceiptForm.controls['status'].setValue(toSelectOrderStatus);
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

  // Check Reason Pay Code
  changeReasonPay(value: Category) {
    this.listOrder = [];
    this.reperList = [];
    this.unitPrice.setValue('0');
    this.exchangeRate.setValue('1');
    this.bankReceiptModel.BankReceiptInvoiceAmount = 0;
    
    this.loading = true;
    this.categoryService.getCategoryById(value.categoryId).subscribe(response => {
      const result = <any>response;
      this.loading = false;
      this.reper = result.category.categoryCode;
      if (this.reper === 'TVI') {
        this.repper.setValue('');
        this.createReceiptForm.controls['repper'].setValidators(Validators.required);
        this.createReceiptForm.controls['repper'].updateValueAndValidity;
        this.loading = true;
        this.employeeService.searchEmployee('', '', '', '', [], '').subscribe(response1 => {
          const result1 = <any>response1;
          this.loading = false;
          this.reperList = result1.employeeList;
          this.reperList.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
        }, error => { });
      } else if (this.reper === 'TTA') {
        this.repper.setValue('');
        this.createReceiptForm.controls['repper'].setValidators(Validators.required);
        this.createReceiptForm.controls['repper'].updateValueAndValidity;
        this.loading = true;
        this.vendorService.getAllVendorToPay().subscribe(response2 => {
          const result2 = <any>response2;
          this.loading = false;
          this.reperList = result2.vendorList;
          this.reperList.forEach(item => {
            item.vendorName = item.vendorCode + ' - ' + item.vendorName;
          });
        }, error => { });
      } else if (this.reper === 'THA') {
        this.repper.setValue('');
        this.createReceiptForm.controls['repper'].setValidators(Validators.required);
        this.createReceiptForm.controls['repper'].updateValueAndValidity;
        this.loading = true;
        this.customerService.getAllCustomer().subscribe(response3 => {
          const result3 = <any>response3;
          this.loading = false;
          this.reperList = result3.customerList;
          this.reperList.forEach(item => {
            item.customerName = item.customerCode + ' - ' + item.customerName;
          });
        }, error => { });
      } else {
        this.createReceiptForm.controls['repper'].setValidators(null);
        this.createReceiptForm.controls['repper'].updateValueAndValidity();
      }
    }, error => { });
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
        let mgs = { severity:'error', summary: 'Thông báo:', detail: result.messageCode};
        this.showMessage(mgs);
      }
    }
    this.handleMoney();
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
          this.bankReceiptModel.OrganizationId = result.selectedOrgId;
          this.createReceiptForm.controls['organizationName'].setValue(result.selectedOrgName);
        }
      }
    });
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
    let unitPrice =  0;
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
      this.bankReceiptModel.BankReceiptInvoiceAmount = unitPrice;
    } else {
      this.bankReceiptModel.BankReceiptInvoiceAmount = unitPrice * exchangeRate;
    }

    this.reloadAmountCollected(this.bankReceiptModel.BankReceiptInvoiceAmount);
  }

  //Tính lại số tiền nhận của các đơn hàng
  reloadAmountCollected(totalAmountCollected: number) {
    let total = totalAmountCollected;
    if (this.reper == 'THA' && this.listOrder.length > 0) {
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
      this.bankReceiptModel.BankReceiptInvoiceAmount = totalAmountCollected;
    } else {
      let unitPrice = this.roundNumber((totalAmountCollected / exchangeRate), parseInt(this.defaultNumberType, 10));
      this.unitPrice.setValue(unitPrice.toString());
      this.bankReceiptModel.BankReceiptInvoiceAmount = totalAmountCollected;
    }
  }

  createReceipt(value: boolean) {
    if (!this.createReceiptForm.valid) {
      Object.keys(this.createReceiptForm.controls).forEach(key => {
        if (this.createReceiptForm.controls[key].valid == false) {
          this.createReceiptForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
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
      let mgs = { severity:'error', summary: 'Thông báo:', detail: 'Số thanh toán của đơn hàng không hợp lệ'};
      this.showMessage(mgs);
    } else {
      //Lý do thu
      this.bankReceiptModel.BankReceiptInvoiceReason = this.reasonControl.value.categoryId;

      //Tài khoản thu
      let bankAccount: Bank = this.bankAccountControl.value;
      this.bankReceiptModel.BankReceiptInvoiceBankAccountId = bankAccount ? bankAccount.bankAccountId : null;

      //Đối tượng thu
      let repper = this.repper.value;
      if (this.reper == 'THA') {
        this.bankReceiptMappingModel.ObjectId = repper ? repper.customerId : null;
      } else if (this.reper == 'TTA') {
        this.bankReceiptMappingModel.ObjectId = repper ? repper.vendorId : null;
      } else {
        this.bankReceiptMappingModel.ObjectId = repper ? repper.employeeId : null;
      }

      //Trạng thái
      let status = this.status.value;
      this.bankReceiptModel.StatusId = status ? status.categoryId : null;

      //Ngày hạch toán
      let paidDate = this.paidDate.value;
      paidDate = convertToUTCTime(paidDate);
      this.bankReceiptModel.BankReceiptInvoicePaidDate = paidDate;

      //Ngày chứng từ
      let vouchersDate = this.vouchersDate.value;
      vouchersDate = convertToUTCTime(vouchersDate);
      this.bankReceiptModel.VouchersDate = vouchersDate;

      //Nội dung
      this.bankReceiptModel.BankReceiptInvoiceDetail = this.content.value.trim();

      //Ghi chú
      this.bankReceiptModel.BankReceiptInvoiceNote = this.noteControl.value == null ? '' : this.noteControl.value.trim();

      //Loại tiền (VND, USD,...)
      let toSelectMoneyUnit = this.unitMoney.find(c => c.categoryCode === this.currencyLabel);
      this.bankReceiptModel.BankReceiptInvoicePriceCurrency = toSelectMoneyUnit.categoryId;

      //Tiền (chưa tính tỷ giá)
      this.bankReceiptModel.BankReceiptInvoicePrice = parseFloat(this.unitPrice.value.replace(/,/g, ''));

      //Tỷ giá
      this.bankReceiptModel.BankReceiptInvoiceExchangeRate = parseFloat(this.exchangeRate.value.replace(/,/g, ''));

      //Thành tiền (Đã tính tỷ giá)
      if (!this.bankReceiptModel.BankReceiptInvoiceAmount) {
        this.bankReceiptModel.BankReceiptInvoiceAmount = 0;
      }

      //List Order History
      this.listOrderHistory = [];
      let totalMoneyOrder = 0;
      this.listOrder.forEach(item => {
        totalMoneyOrder += item.amountReceivable;
        let listHis = {
          orderId: item.orderId,
          amountCollected: item.amountCollected,
          amount: item.amountReceivable
        }
        this.listOrderHistory.push(listHis);
      });
      
      if (this.bankReceiptModel.BankReceiptInvoiceAmount > totalMoneyOrder && this.reper == 'THA' && this.listOrder.length > 0) {
        this.confirm(value);
      } else {
        this.saveBankReceipt(value);
      }
    }
  }

  confirm(value: boolean) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn thanh toán vượt quá công nợ của khách hàng?',
      accept: () => {
        this.saveBankReceipt(value);
      }
    });
  }

  saveBankReceipt(value: boolean) {
    this.awaitResult = true;
    this.loading = true;
    this.accountingService.createBankReceiptInvoice(this.bankReceiptModel, this.bankReceiptMappingModel,
      this.auth.UserId, this.listOrderHistory).subscribe(response => {
        const result = <any>response;
        this.loading = false;
        if (result.statusCode === 202 || result.statusCode === 200) {
          let mgs = { severity:'success', summary: 'Thông báo:', detail: 'Tạo báo có thành công'};
          this.showMessage(mgs);
          if (value) {
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            }
            this.resetFieldValue();
            this.awaitResult = false;
          } else {
            this.cancel();
            this.awaitResult = false;
          }
        } else {
          let mgs = { severity:'error', summary: 'Thông báo:', detail: result.messageCode};
          this.showMessage(mgs);
          this.awaitResult = false;
        }
      }, error => { });
  }

  resetFieldValue() {
    this.setDefaultReason(); //Lý do thu
    this.repper.reset(); //Đối tượng thu
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

    this.listOrder = [];
    this.listOrderHistory = [];
    this.totalAmountReceivable = 0; //Số tiền phải thanh toán
    this.bankReceiptModel.BankReceiptInvoiceAmount = 0; //Thành tiền (Số tiền đã nhân với tỷ giá)
  }

  cancel() {
    this.router.navigate(['/accounting/bank-receipts-list']);
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch(unit) { 
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

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
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
