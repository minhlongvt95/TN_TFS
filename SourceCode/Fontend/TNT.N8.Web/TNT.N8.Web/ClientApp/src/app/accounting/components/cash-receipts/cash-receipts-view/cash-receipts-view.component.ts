import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CategoryService } from '../../../../shared/services/category.service';
import { CashReceiptModel } from '../../../models/cashReceipt.model';
import { AccountingService } from '../../../services/accounting.service';

import * as $ from 'jquery';
import { CashReceiptMappingModel } from '../../../models/cashReceiptMapping.model';
import { GetPermission } from '../../../../shared/permission/get-permission';

@Component({
  selector: 'app-cash-receipts-view',
  templateUrl: './cash-receipts-view.component.html',
  styleUrls: ['./cash-receipts-view.component.css']
})
export class CashReceiptsViewComponent implements OnInit {
  reasonOfPayment: Array<any> = [];
  typesOfPayment: Array<any> = [];
  organizationList: Array<any> = [];
  statusOfPayment: Array<any> = [];
  unitMoney: Array<any> = [];

  isViewMode = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  currentUserName: string = localStorage.getItem('UserFullName');
  currentDate: Date = new Date();

  cashReceiptModel = new CashReceiptModel();
  cashReceiptMappingModel = new CashReceiptMappingModel();
  reasonOfReceipt: any[];
  receipter: any;
  receiptList: any[];
  currencyCode: any;
  isShowExchangeRate = false;
  isShowPriceForeign = false;
  priceForeign: number;
  receipterName = '';
  receiptInvoiceId: any;
  price: number;
  createdByName: any;
  fixed: boolean = false;
  withFiexd: string;

  content: string;
  note: string;
  address: string;

  loading: boolean = false;

  actionDownload: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");


  //For Grid
  constructor(
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router,
    private accountingService: AccountingService,
  ) { }

  ngOnInit() {
    let resource = "acc/accounting/cash-receipts-view/";
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
      this.route.params.subscribe(params => { this.receiptInvoiceId = params['receiptInvoiceId']; });
      this.getReceiptInvoice();
    }
  }

  // Get payment invoice by id
  async getReceiptInvoice() {
    await this.accountingService.getReceiptInvoiceById(this.receiptInvoiceId).subscribe(response => {
      const result = <any>response;
      this.cashReceiptModel = <CashReceiptModel>({
        ReceiptInvoiceId: result.receiptInvoice.receiptInvoiceId,
        ReceiptInvoiceCode: result.receiptInvoice.receiptInvoiceCode,
        CreatedDate: result.receiptInvoice.createdDate,
        ReceiptInvoiceReason: result.receiptInvoice.receiptInvoiceReason,
        OrganizationId: result.receiptInvoice.organizationId,
        RegisterType: result.receiptInvoice.registerType,
        StatusId: result.receiptInvoice.statusId,
        ReceiptDate: result.receiptInvoice.receiptDate,
        RecipientName: result.receiptInvoice.recipientName,
        RecipientAddress: result.receiptInvoice.recipientAddress,
        ReceiptInvoiceDetail: result.receiptInvoice.receiptInvoiceDetail,
        UnitPrice: result.receiptInvoice.unitPrice,
        CurrencyUnit: result.receiptInvoice.currencyUnit,
        ExchangeRate: result.receiptInvoice.exchangeRate,
        ReceiptInvoiceNote: result.receiptInvoice.receiptInvoiceNote,
        NameReceiptInvoiceReason: result.receiptInvoice.nameReceiptInvoiceReason,
        NameCreateBy: result.receiptInvoice.nameCreateBy,
        NameObjectReceipt: result.receiptInvoice.nameObjectReceipt,
        StatusName: result.receiptInvoice.statusName,
        Amount: result.receiptInvoice.amount,
        AmountText: result.receiptInvoice.amountText,
        OrganizationName: result.receiptInvoice.organizationName,
        ReceiptInvoicePriceCurrency: result.receiptInvoice.currencyUnitName,

      });
      // this.content = this.cashReceiptModel.ReceiptInvoiceDetail.trim().length > 22 ? this.cashReceiptModel.ReceiptInvoiceDetail.trim().slice(0, 22) + '...' : this.cashReceiptModel.ReceiptInvoiceDetail;
      // this.note = this.cashReceiptModel.ReceiptInvoiceNote.trim().length > 22 ? this.cashReceiptModel.ReceiptInvoiceNote.trim().slice(0, 22) + '...' : this.cashReceiptModel.ReceiptInvoiceNote;
      // this.address = this.cashReceiptModel.RecipientAddress.trim().length > 22 ? this.cashReceiptModel.RecipientAddress.trim().slice(0, 22) + '...' : this.cashReceiptModel.RecipientAddress;
      this.loading = false;
    }, error => { });
  }

  // Back to receipt list
  cancel() {
    this.router.navigate(['/accounting/cash-receipts-list']);
  }

  exportPdf() {
    this.loading = true;
    this.accountingService.exportPdfReceiptInvoice(this.receiptInvoiceId).subscribe(response => {
      const result = <any>response;
      if (result.receiptInvoicePdf != null) {
        const binaryString = window.atob(result.receiptInvoicePdf);
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
    });
  }
}
