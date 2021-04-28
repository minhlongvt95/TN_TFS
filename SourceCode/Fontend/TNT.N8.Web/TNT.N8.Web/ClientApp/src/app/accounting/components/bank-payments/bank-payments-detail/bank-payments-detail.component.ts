import { Component, OnInit, AfterContentChecked, ChangeDetectorRef, HostListener } from '@angular/core';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CategoryService } from '../../../../shared/services/category.service';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { BankPaymentModel } from '../../../models/bankPayment.model';
import { BankPaymentMappingModel } from '../../../models/bankPaymentMapping.model';

import { AccountingService } from '../../../services/accounting.service';
import { BankService } from '../../../../shared/services/bank.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

import * as $ from 'jquery';

@Component({
  selector: 'app-bank-payments-detail',
  templateUrl: './bank-payments-detail.component.html',
  styleUrls: ['./bank-payments-detail.component.css']
})

export class BankPaymentsDetailComponent implements OnInit, AfterContentChecked {

  price: number;
  reasonOfPayment: Array<any> = [];
  typesOfPayment: Array<any> = [];
  organizationList: Array<any> = [];
  statusOfPayment: Array<any> = [];
  unitMoney: Array<any> = [];
  bankPaymentInvoiceId: string = '';

  reasonCode: string = 'LCH';
  registerTypeCode: string = 'LSO';
  receiptStatusCode: string = 'TCH';
  unitMoneyCode: string = 'DTI';

  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = '';
  currentDate: Date = new Date();

  actionDownload: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();

  bankPaymentModel = new BankPaymentModel();
  bankPaymentMappingModel = new BankPaymentMappingModel();
  isShowExchangeRate = false;
  reasonId = null;
  categoryId = null;
  payer = '';
  payerList = <any>[];
  userId = this.auth.UserId;
  currencyCode: any;
  exchangeRate: number;
  priceForeign: number;
  isShowPriceForeign = false;
  priceForeignString: string;
  createPayableForm: FormGroup;
  submitted = false;
  objectName: string = '';
  bankAccount : string = '';
  content : string;
  note : string;

  bankPayableInvoiceId: string;
  bankPayableInvoiceCode: string;
  bankPayableInvoiceDetail: string;
  bankPayableInvoicePrice: number;
  bankPayableInvoicePriceCurrency: string;
  bankPayableInvoiceExchangeRate: number;
  bankPayableInvoiceReason: string;
  bankPayableInvoiceNote: string;
  bankPayableInvoiceBankAccountId: string;
  bankPayableInvoiceAmount: number;
  bankPayableInvoiceAmountText: string;
  bankPayableInvoicePaidDate: string;
  organizationId: string;
  statusId: string;
  receiveAccountNumber: string;
  receiveAccountName: string;
  receiveBankName: string;
  receiveBranchName: string;
  active: boolean;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  bankPayableInvoiceReasonText: string;
  createdByName: string;
  avatarUrl: string;
  statusName: string;
  organizationName : string;
  unitMoneyName : string;
  fixed: boolean = false;
  withFiexd:string;
  
  loading: boolean = false;

  constructor(
    private ref: ChangeDetectorRef,
    private categoryService: CategoryService,
    private getPermission: GetPermission,
    private orgService: OrganizationService,
    public bankService: BankService,
    private route: ActivatedRoute,
    private router: Router,
    private accountingService: AccountingService,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit() {
    let resource = "acc/accounting/bank-payments-detail/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      this.loading = true;
      this.route.params.subscribe(params => { this.bankPaymentInvoiceId = params['id']; });
      await this.getMasterData();
      this.getBankPayableInvoiceById();
    }
  }

  
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      this.withFiexd = $('#parent').width() +'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  ngAfterContentChecked(): void {
    this.ref.detectChanges();
  }

  async getMasterData() {
    this.loading = true;
    this.bankService.getCompanyBankAccount(this.auth.UserId).subscribe(response => {
      const result = <any>response;
      this.typesOfPayment = result.bankList;
    }, error => { });

    let resultUnitMoney: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc(this.unitMoneyCode);
    this.unitMoney = resultUnitMoney.category;

    let resultOrganizationList: any = await this.orgService.getFinancialindependenceOrgAsync();
    this.organizationList = resultOrganizationList.listOrg;
    
  }

  getBankPayableInvoiceById() {
    this.accountingService.getBankPayableInvoiceById(this.bankPaymentInvoiceId).subscribe(response => {
      var result = <any>response;
      this.bankPayableInvoiceId = result.bankPayableInvoice.bankPayableInvoiceId;
      this.bankPayableInvoiceCode = result.bankPayableInvoice.bankPayableInvoiceCode;
      this.bankPayableInvoiceDetail = result.bankPayableInvoice.bankPayableInvoiceDetail;
      this.bankPayableInvoicePrice = result.bankPayableInvoice.bankPayableInvoicePrice;
      this.bankPayableInvoicePriceCurrency = result.bankPayableInvoice.bankPayableInvoicePriceCurrency;
      this.bankPayableInvoiceExchangeRate = result.bankPayableInvoice.bankPayableInvoiceExchangeRate;
      this.bankPayableInvoiceReason = result.bankPayableInvoice.bankPayableInvoiceReason;
      this.bankPayableInvoiceNote = result.bankPayableInvoice.bankPayableInvoiceNote;
      this.bankPayableInvoiceBankAccountId = result.bankPayableInvoice.bankPayableInvoiceBankAccountId;
      this.bankPayableInvoiceAmount = result.bankPayableInvoice.bankPayableInvoiceAmount;
      this.bankPayableInvoiceAmountText = result.bankPayableInvoice.bankPayableInvoiceAmountText;
      this.bankPayableInvoicePaidDate = result.bankPayableInvoice.bankPayableInvoicePaidDate;
      this.organizationId = result.bankPayableInvoice.organizationId;
      this.statusId = result.bankPayableInvoice.statusId;
      this.receiveAccountNumber = result.bankPayableInvoice.receiveAccountNumber;
      this.receiveAccountName = result.bankPayableInvoice.receiveAccountName;
      this.receiveBankName = result.bankPayableInvoice.receiveBankName;
      this.receiveBranchName = result.bankPayableInvoice.receiveBranchName;
      this.createdById = result.bankPayableInvoice.createdById;
      this.createdDate = result.bankPayableInvoice.createdDate;
      this.bankPayableInvoiceReasonText = result.bankPayableInvoice.bankPayableInvoiceReasonText;
      this.createdByName = result.bankPayableInvoice.createdByName;
      this.objectName = result.bankPayableInvoice.objectName;
      this.statusName = result.bankPayableInvoice.statusName;
      
      this.organizationName = this.organizationList.find(x => x.organizationId === this.organizationId).organizationName;
      this.unitMoneyName = this.unitMoney.find(x => x.categoryId === this.bankPayableInvoicePriceCurrency).categoryName;
      if(this.bankPayableInvoiceBankAccountId){
        this.bankAccount = this.typesOfPayment.find(x => x.bankAccountId === this.bankPayableInvoiceBankAccountId).bankName;
      }
      
      this.content = this.bankPayableInvoiceDetail.trim().length > 22 ? this.bankPayableInvoiceDetail.trim().slice(0, 22) + '...' : this.bankPayableInvoiceDetail.trim();
      this.loading = false;

    }, error => { });
    
   
  }
  cancel() {
    this.router.navigate(['/accounting/bank-payments-list']);
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  exportPFD() {
    this.loading = true;
    this.accountingService.exportBankPayableInvoice(this.bankPaymentInvoiceId, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.bankPayableInvoicePdf != null) {
        const binaryString = window.atob(result.bankPayableInvoicePdf);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/pdf' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.code.replace('/', '-');
        link.download = fileName;
        link.click();
        this.loading = false;
      }
    }, error => { });
  }
}
