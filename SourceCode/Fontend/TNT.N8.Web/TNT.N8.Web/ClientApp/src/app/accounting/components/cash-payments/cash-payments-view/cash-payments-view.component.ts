import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { CashPaymentModel } from '../../../models/cashPayment.model';
import { AccountingService } from "../../../services/accounting.service";
import { TooltipModule } from 'primeng/tooltip'

import * as $ from 'jquery';

import { CashPaymentMappingModel } from '../../../models/cashPaymentMapping.model';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { WarningComponent } from '../../../../shared/toast/warning/warning.component';


@Component({
  selector: 'app-cash-payments-view',
  templateUrl: './cash-payments-view.component.html',
  styleUrls: ['./cash-payments-view.component.css']
})

export class CashPaymentsViewComponent implements OnInit {
  payerName: any;
  price: number;
  createdByName: any;

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private el: ElementRef,
    private accountingService: AccountingService,
  ) { }

  actionDownload: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  // Init variables
  reasonOfPayment: Array<any> = [];
  typesOfPayment: Array<any> = [];
  organizationList: Array<any> = [];
  statusOfPayment: Array<any> = [];
  unitMoney: Array<any> = [];
  payableInvoiceModel: any;
  payableInvoiceId: any;

  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = localStorage.getItem('UserFullName');
  currentDate: Date = new Date();
  cashPaymentModel = new CashPaymentModel();
  cashPaymentMappingModel = new CashPaymentMappingModel();
  isShowExchangeRate = false;
  reasonId = null;
  categoryId = null;
  payer = '';
  payerList = <any>[];
  userId = this.auth.UserId;
  content: string;
  note: string;
  address : string;

  // @ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
  // public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  // loadingConfig: any = {
  //   'animationType': ngxLoadingAnimationTypes.circle,
  //   'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
  //   'backdropBorderRadius': '4px',
  //   'primaryColour': '#ffffff',
  //   'secondaryColour': '#999999',
  //   'tertiaryColour': '#ffffff'
  // }
  loading: boolean = false;

  async ngOnInit() {
    let resource = "acc/accounting/cash-payments-view/";
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
      this.route.params.subscribe(params => { this.payableInvoiceId = params['payableInvoiceId']; });
      await this.getPayableInvoice();
    }
  }
  // Back to list paymentInvoice
  cancel() {
    this.router.navigate(['/accounting/cash-payments-list']);
  }
  // Get payment invoice by id
  async getPayableInvoice() {
      this.accountingService.getPayableInvoiceById(this.payableInvoiceId).subscribe(response => {
      const result = <any>response;
      this.cashPaymentModel = <CashPaymentModel>({
        PayableInvoiceId: result.payableInvoice.payableInvoiceId,
        PayableInvoiceCode: result.payableInvoice.payableInvoiceCode,
        CreatedDate: result.payableInvoice.createdDate,
        PayableInvoiceReason: result.payableInvoice.payableInvoiceReason,
        OrganizationId: result.payableInvoice.organizationId,
        RegisterType: result.payableInvoice.registerType,
        PaidDate: result.payableInvoice.paidDate,
        RecipientName: result.payableInvoice.recipientName,
        StatusId: result.payableInvoice.statusId,
        RecipientAddress: result.payableInvoice.recipientAddress || '',
        PayableInvoiceDetail: result.payableInvoice.payableInvoiceDetail  || '',
        PayableInvoicePrice: result.payableInvoice.payableInvoicePrice,
        PayableInvoicePriceCurrency: result.payableInvoice.payableInvoicePriceCurrency,
        ExchangeRate: result.payableInvoice.exchangeRate,
        PayableInvoiceNote: result.payableInvoice.payableInvoiceNote || '',
        Amount: result.payableInvoice.amount,
        AmountText: result.payableInvoice.amountText || '',
        CreatedByName: result.payableInvoice.createdByName,
        PayableInvoiceReasonText: result.payableInvoice.payableInvoiceReasonText,
        ObjectName: result.payableInvoice.objectName || '',
        StatusName: result.payableInvoice.statusName,
        OrganizationName: result.payableInvoice.organizationName,
        CurrencyUnitName: result.payableInvoice.currencyUnitName,
      });
      // let temp = this.cashPaymentModel.PayableInvoiceDetail;
      // this.content = temp.length > 22 ? temp.slice(0, 22) + '...' : temp;
      // this.note = this.cashPaymentModel.PayableInvoiceNote.trim().length > 22 ? this.cashPaymentModel.PayableInvoiceNote.trim().slice(0, 22) + '...' : this.cashPaymentModel.PayableInvoiceNote;
      // this.address = this.cashPaymentModel.RecipientAddress.trim().length > 22 ? this.cashPaymentModel.RecipientAddress.trim().slice(0, 22) + '...' : this.cashPaymentModel.RecipientAddress;
      
      this.loading = false;
    }, error => { });
  }

  exportPdf() {
    this.loading = true;
    this.accountingService.exportPayableInvoice(this.payableInvoiceId, this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.payableInvoicePdf != null) {
        const binaryString = window.atob(result.payableInvoicePdf);
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
