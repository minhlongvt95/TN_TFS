import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { PromotionService } from "../../services/promotion.service";
import { GetPermission } from '../../../shared/permission/get-permission';
import { SortEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-promotion-list',
  templateUrl: './promotion-list.component.html',
  styleUrls: ['./promotion-list.component.css']
})
export class PromotionListComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('myTable') myTable: Table;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  loading: boolean = false;
  SumRow: number = 0;
  filterGlobal: string = null;

  cols: any[];
  listPromotion: Array<any> = [];

  promotionCode: string = '';
  promotionName: string = '';
  expirationDateFrom: Date = null;
  expirationDateTo: Date = null;
  quantityOrder: number = null;
  quantityQuote: number = null;
  quantityContract: number = null;
  
  constructor(
    private getPermission: GetPermission,
    private promotionService: PromotionService,
    public messageService: MessageService,
    private router: Router,
  ) 
  {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    this.setTable();

    let resource = "crm/promotion/list/view";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.getMasterData();
    }
  }

  setTable() {
    this.cols = [
      { field: 'promotionCode', header: 'Mã', textAlign: 'left' },
      { field: 'promotionName', header: 'Tên CTKM', textAlign: 'left' },
      { field: 'expirationDate', header: 'Ngày hết hạn', textAlign: 'right' },
      { field: 'active', header: 'Hoạt động', textAlign: 'center' },
      // { field: 'quantityOrder', header: 'Số lượng đơn hàng', textAlign: 'right' },
      { field: 'quantityQuote', header: 'Số lượng báo giá', textAlign: 'right' },
      // { field: 'quantityContract', header: 'Số lượng hợp đồng', textAlign: 'right' },
    ];
  }

  getMasterData() {
    this.promotionService.getMasterDataListPromotion().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.searchListPromotion();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  searchListPromotion() {
    this.filterGlobal = null;
    if (this.myTable) {
      this.myTable.reset();
    }

    let expirationDateFrom = null;
    if (this.expirationDateFrom) {
      expirationDateFrom = convertToUTCTime(this.expirationDateFrom);
    }

    let expirationDateTo = null;
    if (this.expirationDateTo) {
      expirationDateTo = convertToUTCTime(this.expirationDateTo);
    }
    
    let quantityOrder = null;
    if (this.quantityOrder != null && this.quantityOrder?.toString() != '') {
      quantityOrder = ParseStringToFloat(this.quantityOrder.toString());
    }
    let quantityQuote = null;
    if (this.quantityQuote != null && this.quantityQuote?.toString() != '') {
      quantityQuote = ParseStringToFloat(this.quantityQuote.toString());
    }
    let quantityContract = null;
    if (this.quantityContract != null && this.quantityContract?.toString() != '') {
      quantityContract = ParseStringToFloat(this.quantityContract.toString());
    }

    this.loading = true;
    this.promotionService.searchListPromotion(this.promotionCode, this.promotionName, expirationDateFrom, 
      expirationDateTo, quantityOrder, quantityQuote, quantityContract).subscribe(response => 
    {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listPromotion = result.listPromotion;
        this.SumRow = this.listPromotion.length;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  refreshFilter() {
    this.promotionCode = '';
    this.promotionName = '';
    this.expirationDateFrom = null;
    this.expirationDateTo = null;
    this.quantityOrder = null;
    this.quantityQuote = null;
    this.quantityContract = null;
    this.searchListPromotion();
  }

  leftColNumber: number = 12;
  rightColNumber: number = 2;

  showFilter() {
    if (this.innerWidth < 1423) {
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

  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      ///**Customize sort date */
      //if (event.field == 'quoteDate') {
      //  const date1 = moment(value1, this.dateFieldFormat);
      //  const date2 = moment(value2, this.dateFieldFormat);

      //  let result: number = -1;
      //  if (moment(date2).isBefore(date1, 'day')) { result = 1; }

      //  return result * event.order;
      //}
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

  createPromotion() {
    this.router.navigate(['/promotion/create']);
  }

  goDetail(Id: string) {
    this.router.navigate(['/promotion/detail', { promotionId: Id }]);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = String(str).replace(/,/g, '');
  return parseFloat(str);
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
}
