import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MenuItem, MessageService } from 'primeng/api';
import { SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { BillSaleService } from '../../services/bill-sale.service';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface BillSale {
  billOfSaLeId: string,
  billOfSaLeCode: string,
  orderId: string,
  orderName: string,
  customerId: string,
  customerName: string,
  employeeId: string,
  seller: string,
  statusId: string,
  statusName: string,
  amount: string,
  billDate: string,
}

interface Customer {
  customerId: string;
  customerName: string;
  customerCode: string;
  customerEmail: string;
  customerPhone: string;
  fullAddress: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  isManager: boolean;
  positionId: string;
  organizationId: string;
}

@Component({
  selector: 'app-bill-sale-list',
  templateUrl: './bill-sale-list.component.html',
  styleUrls: ['./bill-sale-list.component.css']
})
export class BillSaleListComponent implements OnInit {

  innerWidth: number = 0; //number window size first
  emitParamUrl: any;
  @ViewChild('myTable') myTable: Table;

  loading: boolean = false;
  cols: any;
  selectedColumns: any[];
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  today = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  isGlobalFilter: string = '';
  startDate: Date = null;
  maxEndDate: Date = new Date();
  endDate: Date = null;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  userPermission: any = localStorage.getItem("UserPermission").split(',');

  //Biến lưu giá trị trả về
  listStatus: Array<Category> = [];
  listProduct: Array<any> = [];
  listBillOfSale: Array<BillSale> = [];

  /*Action*/
  actionAdd: boolean = true;
  actionDelete: boolean = true;
  /*End*/

  // Value search
  billSaleCode: string = '';
  orderCode: string = '';
  customerName: string;
  listProductId: Array<any> = [];
  fromDate: Date = null;
  toDate: Date = null;
  listStatusId: Array<Category> = [];
  statusNEWId: string;
  displayDialog: boolean = false;
  billSaleDelete:BillSale= null;

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private datePipe: DatePipe,
    private confirmationService: ConfirmationService,
    private billSaleService: BillSaleService
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "sal/bill-sale/create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.setTable();
      this.getMasterData();
      this.searchBillOfSale();
    }
  }

  setTable() {
    this.cols = [
      { field: 'billOfSaLeCode', header: 'Mã hóa đơn', textAlign: 'left', display: 'table-cell', width: '10%' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell', width: '20%' },
      { field: 'billDate', header: 'Ngày hóa đơn', textAlign: 'right', display: 'table-cell', width: '10%' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '10%' },
      { field: 'amount', header: 'Giá trị hóa đơn', textAlign: 'right', display: 'table-cell', width: '10%' },
      { field: 'paid', header: 'Đã thanh toán', textAlign: 'left', display: 'table-cell', width: '10%' },
      { field: 'seller', header: 'Nhân viên bán hàng', textAlign: 'left', display: 'table-cell', width: '20%'  },
    ];
    this.selectedColumns = this.cols.filter(e => e.field == "billOfSaLeCode" || e.field == "customerName" || e.field == "billDate"
      || e.field == "statusName" || e.field == "amount" || e.field == "paid" );
  }

  getMasterData() {
    this.loading = true;
    this.billSaleService.getMasterDataSearchBillSale().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listProduct = result.listProduct;
        this.listStatus = result.listStatus;
        let status = this.listStatus.find(x => x.categoryCode == "NEW");
        if (status != null && status != undefined) {
          this.statusNEWId = status.categoryId;
        }
        this.listProduct.forEach(item => {
          item.productName = item.productCode + ' - ' + item.productName;
        });
      } else {

      }
    });
  }
  deleteItem() {
    this.displayDialog = false;
    if(this.billSaleDelete != null){
      this.loading = true;
      this.billSaleService.deleteBillSale(this.billSaleDelete.billOfSaLeId).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listBillOfSale = this.listBillOfSale.filter(x => x.billOfSaLeId != this.billSaleDelete.billOfSaLeId);
          let msg = { severity: 'success', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  showDialog(data:BillSale){
    this.displayDialog = true;
    this.billSaleDelete = data;
  }
  showMessage(msg: any) {
    this.messageService.add(msg);
  }
  searchBillOfSale() {
    if (this.billSaleCode) {
      this.billSaleCode = this.billSaleCode.trim();
    }
    if (this.customerName) {
      this.customerName = this.customerName.trim();
    }
    if (this.orderCode) {
      this.orderCode = this.orderCode.trim();
    }
    let fromDate = null;
    if (this.fromDate) {
      fromDate = this.fromDate;
      fromDate.setHours(0, 0, 0, 0);
      fromDate = convertToUTCTime(fromDate);
    }
    let toDate = null;
    if (this.toDate) {
      toDate = this.toDate;
      toDate.setHours(23, 59, 59, 999);
      toDate = convertToUTCTime(toDate);
    }

    let listProductId: Array<string> = [];
    listProductId = this.listProductId.map(c => c.productId);
    let listStatusId: Array<string> = [];
    listStatusId = this.listStatusId.map(c => c.categoryId);
    this.loading = true;
    this.billSaleService.searchBillSale(this.billSaleCode, this.orderCode, this.customerName, listProductId, listStatusId, fromDate, toDate).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.listBillOfSale = result.listBillOfSale;
    });

  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
        // this.selectedColumns = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
        //   || e.field == "valueContract" || e.field == "nameStatus");
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        // this.selectedColumns = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
        //   || e.field == "valueContract" || e.field == "nameEmployee" || e.field == "nameStatus");
      }
    }
  }

  // Refresh parameter search
  refreshFilter() {
    this.billSaleCode = '';
    this.orderCode = '';
    this.customerName = '';
    this.fromDate = null;
    this.toDate = null;
    this.listProductId = [];
    this.listStatusId = [];
    this.isGlobalFilter = '';
    this.listBillOfSale = [];
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.isShowFilterLeft = false;

    this.searchBillOfSale();
  }

  goToCreate() {
    this.router.navigate(['/bill-sale/create']);
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/bill-sale/detail', { billSaleId: rowData }]);
  }

  dateFieldFormat: string = 'DD/MM/YYYY';

  sortColumnInList(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'createdDate') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }
      /**End */

      let result = null;

      if (value1 == null && value2 != null)
        result = -1;
      else if (value1 != null && value2 == null)
        result = 1;
      else if (value1 == null && value2 == null)
        result = 0;
      else if (typeof value1 === 'string' && typeof value2 === 'string')
        result = value1.localeCompare(value2);
      else
        result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      return (event.order * result);
    });
  }

}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

