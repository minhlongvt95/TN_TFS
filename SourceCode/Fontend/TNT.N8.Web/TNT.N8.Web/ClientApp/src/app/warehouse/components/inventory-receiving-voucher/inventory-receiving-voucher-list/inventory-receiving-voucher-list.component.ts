import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router } from '@angular/router';
//service
import { WarehouseService } from "../../../services/warehouse.service";
import { GetPermission } from '../../../../shared/permission/get-permission';

import { SortEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';

@Component({
  selector: 'app-inventory-receiving-voucher-list',
  templateUrl: './inventory-receiving-voucher-list.component.html',
  styleUrls: ['./inventory-receiving-voucher-list.component.css']
})
export class InventoryReceivingVoucherListComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('myTable') myTable: Table;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  
  cols: any[];
  listInventoryReceivingVoucher: Array<any> = [];

  voucherCode: string = '';
  serialCode: string = '';

  listStatus: Array<any> = [];
  listStatusSelected: Array<any> = [];

  listWarehouse: Array<any> = [];
  listWarehouseSelected: Array<any> = [];

  listCreateVoucher: Array<any> = [];
  listCreateVoucherSelected: Array<any> = [];

  listStorekeeper: Array<any> = [];
  listStorekeeperSelected: Array<any> = [];

  listVendor: Array<any> = [];
  listVendorSelected: Array<any> = [];

  listProduct: Array<any> = [];
  listProductSelected: Array<any> = [];

  listCreateDate: Date[];
  listInventoryReceivingDate: Date[];
  SumRow: number = 0;
  currentDate: Date;
  rows = 10;
  filterGlobal: string = null;

  constructor(
    private getPermission: GetPermission,
    private warehouseService: WarehouseService,
    public dialogService: DialogService,
    public messageService: MessageService,
    private router: Router,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    //Check permission
    let resource = "war/warehouse/inventory-receiving-voucher/list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.setTable();
      this.getMasterData();
    }
  }

  setTable() {
    this.cols = [
      { field: 'inventoryReceivingVoucherCode', header: 'Mã phiếu', textAlign: 'center' },
      { field: 'partnersName', header: 'Đối tác', textAlign: 'center' },
      // { field: 'vendorOrderCode', header: 'Đơn hàng', textAlign: 'center' },
      { field: 'createdDate', header: 'Ngày lập phiếu', textAlign: 'right' },
      { field: 'inventoryReceivingVoucherDate', header: 'Ngày nhập kho', textAlign: 'right' },
      { field: 'inventoryReceivingVoucherTypeName', header: 'Loại phiếu', textAlign: 'center' },
      { field: 'createdName', header: 'Người lập phiếu', textAlign: 'center' },
      // { field: 'nameStorekeeper', header: 'Người nhập kho', textAlign: 'center' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center' },
    ];
  }

  getMasterData() {
    this.warehouseService.getMasterDataListPhieuNhapKho().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listStatus = result.listStatus;
        this.listWarehouse = result.listWarehouse;
        this.listCreateVoucher = result.listEmployee;
        this.listProduct = result.listProduct;

        this.setDefaultFilter();

        this.searchListPhieuNhapKho();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Set các giá trị mặc định ban đầu cho bộ lọc*/
  setDefaultFilter() {
    this.voucherCode = '';
    this.listCreateDate = null;
    this.listInventoryReceivingDate = null;
    this.listStatusSelected = [];
    this.listWarehouseSelected = [];
    this.listCreateVoucherSelected = [];
    this.listProductSelected = [];
    this.serialCode = '';

    //Mặc định lấy trạng thái nháp
    let status = this.listStatus.find(x => x.categoryCode == "NHA");
    this.listStatusSelected.push(status);
  }

  searchListPhieuNhapKho() {
    this.filterGlobal = null;
    if (this.myTable) {
      this.myTable.reset();
    }

    //Ngày lập phiếu
    let fromNgayLapPhieu: Date = null;
    let toNgayLapPhieu: Date = null;
    if (this.listCreateDate) {
      //Nếu chỉ có 1 ngày được chọn thì ngày đó là toNgayLapPhieu
      if (!this.listCreateDate[1]) {
        toNgayLapPhieu = convertToUTCTime(this.listCreateDate[0]);
      } 
      //Nếu chỉ có 2 ngày được chọn thì
      else {
        fromNgayLapPhieu = convertToUTCTime(this.listCreateDate[0]);
        toNgayLapPhieu = convertToUTCTime(this.listCreateDate[1]);
      }
    }

    //Ngày nhập kho
    let fromNgayNhapKho: Date = null;
    let toNgayNhapKho: Date = null;
    if (this.listInventoryReceivingDate) {
      //Nếu chỉ có 1 ngày được chọn thì ngày đó là toNgayNhapKho
      if (!this.listInventoryReceivingDate[1]) {
        toNgayNhapKho = convertToUTCTime(this.listInventoryReceivingDate[0]);
      } 
      //Nếu chỉ có 2 ngày được chọn thì
      else {
        fromNgayNhapKho = convertToUTCTime(this.listInventoryReceivingDate[0]);
        toNgayNhapKho = convertToUTCTime(this.listInventoryReceivingDate[1]);
      }
    }

    //Trạng thái
    let listStatusId = this.listStatusSelected.map(x => x.categoryId);

    //Kho nhập
    let listWarehouseId = this.listWarehouseSelected.map(x => x.warehouseId);

    //Người lập phiếu
    let listEmployeeId = this.listCreateVoucherSelected.map(x => x.employeeId);

    //Sản phẩm
    let listProductId = this.listProductSelected.map(x => x.productId);

    let serialCode = this.serialCode == null ? "" : this.serialCode.trim();

    this.loading = true;
    this.warehouseService.searchListPhieuNhapKho(this.voucherCode, fromNgayLapPhieu,
      toNgayLapPhieu, fromNgayNhapKho, toNgayNhapKho, listStatusId, listWarehouseId,
      listEmployeeId, listProductId, serialCode).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.listInventoryReceivingVoucher = result.listPhieuNhapKho;
          this.SumRow = this.listInventoryReceivingVoucher.length;
          this.currentDate = new Date();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  refreshFilter() {
    this.setDefaultFilter();
    this.searchListPhieuNhapKho();
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

  /*Event nhấn phím Enter khi đang forcus vào ô tìm kiếm theo tên khách hàng*/
  onKeydown($event: KeyboardEvent) {
    if ($event.key === 'Enter') {
      this.searchListPhieuNhapKho();
    }
  }

  createInventoryVoucher() {
    this.router.navigate(['/warehouse/inventory-receiving-voucher/create']);
  }

  goDetails(Id: any) {
    this.router.navigate(['/warehouse/inventory-receiving-voucher/detail', { inventoryReceivingVoucherId: Id }]);
  }

  goToDetailObject(rowData: any) {
    if (rowData.inventoryReceivingVoucherType == 2) {
      this.router.navigate(['/customer/detail', { customerId: rowData.partnersId, contactId: this.emptyGuid }]);
    }
    else {
      this.router.navigate(['/vendor/detail', { vendorId: rowData.partnersId, contactId: this.emptyGuid }]);
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
}