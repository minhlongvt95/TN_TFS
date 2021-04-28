import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AccountingService } from '../../services/accounting.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { FormControl, FormGroup } from '@angular/forms';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';

class saleReportFormModel {
  fromDate: Date;
  toDate: Date;
}

@Component({
  selector: 'app-sales-report',
  templateUrl: './sales-report.component.html',
  styleUrls: ['./sales-report.component.css'],
})
export class SalesReportComponent implements OnInit {
  loading: boolean = false;
  @ViewChild('table', { static: true }) table: Table;
  filterGlobal: string;
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  currentTime: Date = new Date();
  innerWidth: number = 0; //number window size first
  /* table */
  selectedColumns: any[];
  colsSaleReport: any[];
  saleReportForm: FormGroup;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  rows = 10;
  totalSales: any;
  totalCost: any;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  listSalesReport: Array<any> = [];

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private accountingService: AccountingService,
    private router: Router,
    private messageService: MessageService
  ) {
    this.translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "acc/accounting/sales-report/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.initTable();
      this.initForm();
      this.search();
    }
  }

  initTable() {
    this.selectedColumns = [
      { field: 'salesReportMonth', header: 'Tháng', textAlign: 'center', display: 'table-cell' },
      { field: 'totalSales', header: 'Doanh Thu (VND)', textAlign: 'right', display: 'table-cell' },
      { field: 'totalCost', header: 'Chi phí (VND)', textAlign: 'right', display: 'table-cell' },
      { field: 'benefits', header: 'Lãi trong tháng (VND)', textAlign: 'right', display: 'table-cell' },
    ];
    this.colsSaleReport = this.selectedColumns;
  }

  initForm() {
    this.saleReportForm = new FormGroup({
      'FromDate': new FormControl(new Date(`01/01/${this.currentTime.getFullYear()}`)),
      'ToDate': new FormControl(new Date())
    });
  }

  resetTable() {
    this.filterGlobal = '';
    this.table.sortOrder = 0;
    this.table.sortField = '';
    this.table.reset();
  }

  resetForm() {
    this.saleReportForm.patchValue({
      'FromDate': new Date(`01/01/${this.currentTime.getFullYear()}`),
      'ToDate': new Date()
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 768) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  search() {
    this.resetTable();
    this.loading = true;
    let saleReportForm = this.mappingFomrToModel();
    this.accountingService.searchSalesReport(this.auth.UserId, saleReportForm.fromDate, saleReportForm.toDate).subscribe(response => {
      this.loading = false;
      const result = <any>response;
      this.totalSales = result.totalSale || 0;
      this.totalCost = result.totalCost || 0;
      this.listSalesReport = result.salesReportList || [];
    }, error => { this.loading = false; });
  }

  mappingFomrToModel(): saleReportFormModel {
    let result = new saleReportFormModel();
    let fromDate = this.saleReportForm.get('FromDate').value;
    let toDate = this.saleReportForm.get('ToDate').value;
    result.fromDate = fromDate ? convertToUTCTime(fromDate) : null;
    result.toDate = toDate ? convertToUTCTime(toDate) : null;
    return result;
  }

  //apply new UI
  refreshFilter() {
    this.resetForm();
    this.search();
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
