import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
//service
import { VendorService } from "../../../../vendor/services/vendor.service";
import { WarehouseService } from "../../../services/warehouse.service";
import { CategoryService } from "../../../../shared/services/category.service";
import { EmployeeService } from '../../../../employee/services/employee.service';
import { ProductService } from '../../../../product/services/product.service';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { Observable } from 'rxjs';

import { MessageService } from 'primeng/api';
import { SortEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';

@Component({
  selector: 'app-inventory-delivery-voucher-list',
  templateUrl: './inventory-delivery-voucher-list.component.html',
  styleUrls: ['./inventory-delivery-voucher-list.component.css']
})
export class InventoryDeliveryVoucherListComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  @HostListener('window:resize', ['$event'])
  onResize(event) {
    //if (this.innerWidth < )
  }
  @ViewChild('myTable') myTable: Table;
  @ViewChild('inventoryCreateDate') private inventoryCreateDateCalendar: any;
  @ViewChild('inventoryReceivingDate') private inventoryReceivingDateCalendar: any;

  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';

  actionAdd: boolean = true;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listCategoryId: Array<string> = [];
  cols: any[];
  listInventoryReceivingVoucher: Array<any> = [];
  visibleSidebar2: Boolean = false;

  voucherCode: string = '';
  serial: string = '';

  listStatus: Array<any> = [];
  listStatusSelected: Array<any> = [];
  listStatusSelectedId: Array<any> = [];

  listWarehouse: Array<any> = [];
  listWarehouseSelected: Array<any> = [];
  listWarehouseSelectedId: Array<any> = [];

  listCreateVoucher: Array<any> = [];
  listCreateVoucherSelected: Array<any> = [];
  listCreateVoucherSelectedId: Array<any> = [];

  listStorekeeper: Array<any> = [];
  listStorekeeperSelected: Array<any> = [];
  listStorekeeperSelectedId: Array<any> = [];

  listCustomer: Array<any> = [];
  listVendorSelected: Array<any> = [];
  listVendorSelectedId: Array<any> = [];

  listProduct: Array<any> = [];
  listProductSelected: Array<any> = [];
  listProductSelectedId: Array<any> = [];

  listCreateDate: Date[];
  listInventoryReceivingDate: Date[];

  SumRow: number = 0;
  currentDate: Date;

  createParameterForm: FormGroup;
  voucherCodeControl: FormControl;
  createDateControl: FormControl;
  inventoryReceivingDate: FormControl;
  statusControl: FormControl;
  warehouseControl: FormControl;
  createVoucherControl: FormControl;
  storekeeperControl: FormControl;
  vendorControl: FormControl;
  productControl: FormControl;
  serialControl: FormControl;

  rows = 10;
  constructor(
    private warehouseService: WarehouseService,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    private vendorService: VendorService,
    private productService: ProductService,
    public messageService: MessageService,
    private employeeService: EmployeeService,
    private router: Router,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    let resource = "war/warehouse/inventory-delivery-voucher/list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.getFormControl();
      this.cols = [
        { field: 'inventoryDeliveryVoucherCode', header: 'Mã phiếu nhập', textAlign: 'left' },
        { field: 'vedorName', header: 'Đối tượng', textAlign: 'left' },
        { field: 'nameObject', header: 'Đơn hàng', textAlign: 'left' },
        { field: 'createdDate', header: 'Ngày lập phiếu', textAlign: 'right' },
        { field: 'inventoryDeliveryVoucherDate', header: 'Ngày xuất kho', textAlign: 'right' },
        { field: 'inventoryDeliveryVoucherTypeName', header: 'Loại phiếu nhập', textAlign: 'left' },
        { field: 'nameCreate', header: 'Người lập phiếu', textAlign: 'left' },
        //{ field: 'nameStorekeeper', header: 'Người nhập kho' },
        { field: 'nameStatus', header: 'Trạng thái', textAlign: 'left' },
      ];

      this.loading = true;
      await this.getMasterData();

      this.warehouseService.searchInventoryDeliveryVoucher(this.voucherCode, this.listStatusSelectedId, this.listWarehouseSelectedId,
        this.listCreateVoucherSelectedId, this.listStorekeeperSelectedId, this.listVendorSelectedId, this.listProductSelectedId,
        this.listCreateDate, this.listInventoryReceivingDate, this.serial, this.auth.UserId
      ).subscribe(response2 => {
        let result2 = <any>response2;
        this.listInventoryReceivingVoucher = result2.lstResult;
        if (result2.lstResult == null) { this.listInventoryReceivingVoucher = []; }
        this.SumRow = this.listInventoryReceivingVoucher.length;
        this.currentDate = new Date();
        this.listInventoryReceivingVoucher.forEach(function (item) {
          switch (item.inventoryDeliveryVoucherType) {
            case 1:
              item.inventoryDeliveryVoucherTypeName = 'Xuất bán hàng';
              break;
            case 2:
              item.inventoryDeliveryVoucherTypeName = 'Xuất trả lại nhà cung cấp';
              break;
            case 3:
              item.inventoryDeliveryVoucherTypeName = 'Xuất hàng biếu tặng';
              break;
            case 4:
              item.inventoryDeliveryVoucherTypeName = 'Xuất hàng kiểm định';
              break;
            default:
              item.inventoryDeliveryVoucherTypeName = 'Khác';
          }
        });
        if (this.listInventoryReceivingVoucher.length == 0) {
          this.messageService.clear();
          this.messageService.add({ key: 'info', severity: 'info', summary: ' Không tìm thấy phiếu xuất kho', detail: 'Danh sách xuất kho' });
        }

        this.loading = false;
      }, error => { });
    }
  }

  async getMasterData() {
    //get Status
    var result: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('TPHX');
    this.listStatus = result.category;
    let findNhap = this.listStatus.find(f => f.categoryCode == 'NHA');
    if (findNhap != null) {
      this.listStatusSelected.push(findNhap);
    }

    //list warhouse
    this.warehouseService.getWareHouseCha().subscribe(response => {
      let result = <any>response;
      this.listWarehouse = result.listWareHouse;
    }, error => { });
    //list Customer
    this.warehouseService.filterCustomerInInventoryDeliveryVoucher().subscribe(response => {
      let result = <any>response;
      this.listCustomer = result.lstCustomer;
    }, error => { });
    this.employeeService.getAllEmployeeAccount().subscribe(response => {
      var result = <any>response;
      this.listCreateVoucher = result.employeeAccounts;
      this.listStorekeeper = result.employeeAccounts;
      let currentUser = this.listCreateVoucher.find(item => item.employeeId == this.auth.EmployeeId);
      this.listCreateVoucherSelected.push(currentUser);
      for (var i = 0; i < this.listCreateVoucher.length; ++i) {
        this.listCreateVoucher[i].employeeName = this.listCreateVoucher[i].employeeCode + '-' + this.listCreateVoucher[i].employeeName;
      }

    }, error => { });
    //product
    let resultproduct: any = await this.productService.searchProductAsync('', '', [], []);
    this.listProduct = resultproduct.productList;

  }

  getFormControl() {
    this.voucherCodeControl = new FormControl();
    this.createDateControl = new FormControl();
    this.inventoryReceivingDate = new FormControl();
    this.statusControl = new FormControl();
    this.warehouseControl = new FormControl();
    this.createVoucherControl = new FormControl();
    this.storekeeperControl = new FormControl();
    this.vendorControl = new FormControl();
    this.productControl = new FormControl();
    this.serialControl = new FormControl();

    this.createParameterForm = new FormGroup({
      voucherCodeControl: this.voucherCodeControl,
      createDateControl: this.createDateControl,
      inventoryReceivingDate: this.inventoryReceivingDate,
      statusControl: this.statusControl,
      warehouseControl: this.warehouseControl,
      createVoucherControl: this.createVoucherControl,
      storekeeperControl: this.storekeeperControl,
      vendorControl: this.vendorControl,
      productControl: this.productControl,
      serialControl: this.serialControl,
    });

  }

  closePanelStatus() {
    this.listStatusSelectedId = [];
    let categoryList: any[] = [];
    this.listStatusSelected.forEach(function (item) {
      categoryList.push(item.categoryId);
    });
    this.listStatusSelectedId.push.apply(this.listStatusSelectedId, categoryList);
  }
  closePanelWarehouse() {
    this.listWarehouseSelectedId = [];
    let warehouseIdArray: any[] = [];
    this.listWarehouseSelected.forEach(function (item) {
      warehouseIdArray.push(item.warehouseId);
    });
    this.listWarehouseSelectedId.push.apply(this.listWarehouseSelectedId, warehouseIdArray);
  }

  closePanelCreateVoucher() {
    this.listCreateVoucherSelectedId = [];
    let employeeIdArr: any[] = [];
    this.listCreateVoucherSelected.forEach(function (item) {
      employeeIdArr.push(item.employeeId);
    });
    this.listCreateVoucherSelectedId.push.apply(this.listCreateVoucherSelectedId, employeeIdArr);
  }

  closePanelStorekeeper() {
    this.listStorekeeperSelectedId = [];
    let storekeeperIdArr: any[] = [];
    this.listStorekeeperSelected.forEach(function (item) {
      storekeeperIdArr.push(item.employeeId);
    });
    this.listStorekeeperSelectedId.push.apply(this.listStorekeeperSelectedId, storekeeperIdArr);
  }

  closePanelVendor() {
    this.listVendorSelectedId = [];
    let vendorIdArr: any[] = [];
    this.listVendorSelected.forEach(function (item) {
      vendorIdArr.push(item.customerId);
    });
    this.listVendorSelectedId.push.apply(this.listVendorSelectedId, vendorIdArr);
  }

  closePanelProduct() {
    this.listProductSelectedId = [];
    let productIdArr: any[] = [];
    this.listProductSelected.forEach(function (item) {
      productIdArr.push(item.productId);
    });
    this.listProductSelectedId.push.apply(this.listProductSelectedId, productIdArr);

  }
  filterInventoryVoucher() {
    this.loading = true;

    if (this.listCreateDate != null) {
      if (this.listCreateDate.length == 2) {
        if (this.listCreateDate[1] == null) {
          this.listCreateDate[1] = this.listCreateDate[0];
        }
      }
    }
    if (this.listInventoryReceivingDate != null) {
      if (this.listInventoryReceivingDate.length == 2) {
        if (this.listInventoryReceivingDate[1] == null) {
          this.listInventoryReceivingDate[1] = this.listInventoryReceivingDate[0];
        }
      }
    }
    this.listStatusSelectedId = [];
    let categoryList: any[] = [];
    this.listStatusSelected.forEach(function (item) {
      categoryList.push(item.categoryId);
    });
    this.listStatusSelectedId.push.apply(this.listStatusSelectedId, categoryList);

    this.listCreateVoucherSelectedId = [];
    let employeeIdArr: any[] = [];
    this.listCreateVoucherSelected.forEach(function (item) {
      employeeIdArr.push(item.employeeId);
    });
    this.listCreateVoucherSelectedId.push.apply(this.listCreateVoucherSelectedId, employeeIdArr);

    this.warehouseService.searchInventoryDeliveryVoucher(this.voucherCode, this.listStatusSelectedId, this.listWarehouseSelectedId,
      this.listCreateVoucherSelectedId, this.listStorekeeperSelectedId, this.listVendorSelectedId, this.listProductSelectedId,
      this.listCreateDate, this.listInventoryReceivingDate, this.serial, this.auth.UserId
    ).subscribe(response2 => {
      let result2 = <any>response2;
      this.listInventoryReceivingVoucher = result2.lstResult;
      if (result2.lstResult == null) { this.listInventoryReceivingVoucher = []; }
      this.SumRow = this.listInventoryReceivingVoucher.length;
      this.currentDate = new Date();
      this.listInventoryReceivingVoucher.forEach(function (item) {
        switch (item.inventoryDeliveryVoucherType) {
          case 1:
            item.inventoryDeliveryVoucherTypeName = 'Xuất bán hàng';
            break;
          case 2:
            item.inventoryDeliveryVoucherTypeName = 'Xuất trả lại nhà cung cấp';
            break;
          case 3:
            item.inventoryDeliveryVoucherTypeName = 'Xuất hàng biếu tặng';
            break;
          case 4:
            item.inventoryDeliveryVoucherTypeName = 'Xuất hàng kiểm định';
            break;
          default:
            item.inventoryDeliveryVoucherTypeName = 'Khác';
        }
      });

      if (this.listInventoryReceivingVoucher.length == 0) {
        this.messageService.clear();
        this.messageService.add({ key: 'info', severity: 'info', summary: ' Không tìm thấy phiếu xuất kho', detail: 'Danh sách xuất kho' });
      }

      this.visibleSidebar2 = true;
      this.loading = false;

    }, error => { });

  }

  cancelFilter() {
    this.voucherCode = '';
    this.listStatusSelectedId = [];
    this.listWarehouseSelectedId = [];
    this.listCreateVoucherSelectedId = [];
    this.listStorekeeperSelectedId = [];
    this.listVendorSelectedId = [];
    this.listProductSelectedId = [];
    this.listCreateDate = [];
    this.listInventoryReceivingDate = [];
    this.serial = '';
    this.warehouseService.searchInventoryDeliveryVoucher(this.voucherCode, this.listStatusSelectedId, this.listWarehouseSelectedId,
      this.listCreateVoucherSelectedId, this.listStorekeeperSelectedId, this.listVendorSelectedId, this.listProductSelectedId,
      this.listCreateDate, this.listInventoryReceivingDate, this.serial, this.auth.UserId
    ).subscribe(response2 => {
      let result2 = <any>response2;
      this.listInventoryReceivingVoucher = result2.lstResult;
      if (result2.lstResult == null) { this.listInventoryReceivingVoucher = []; }
      this.SumRow = this.listInventoryReceivingVoucher.length;
      this.currentDate = new Date();
      this.listInventoryReceivingVoucher.forEach(function (item) {
        switch (item.inventoryDeliveryVoucherType) {
          case 1:
            item.inventoryDeliveryVoucherTypeName = 'Xuất bán hàng';
            break;
          case 2:
            item.inventoryDeliveryVoucherTypeName = 'Xuất trả lại nhà cung cấp';
            break;
          case 3:
            item.inventoryDeliveryVoucherTypeName = 'Xuất hàng biếu tặng';
            break;
          case 4:
            item.inventoryDeliveryVoucherTypeName = 'Xuất hàng kiểm định';
            break;
          default:
            item.inventoryDeliveryVoucherTypeName = 'Khác';
        }
      });
      if (this.listInventoryReceivingVoucher.length == 0) {
        this.messageService.clear();
        this.messageService.add({ key: 'info', severity: 'info', summary: ' Không tìm thấy phiếu xuất kho', detail: 'Danh sách xuất kho' });
      }
      this.visibleSidebar2 = true;
    }, error => { });
  }

  createOrUpdateInventoryVoucher() {
    this.router.navigate(['/warehouse/inventory-delivery-voucher/create-update']);
  }
  goDetails(Id: any) {
    this.router.navigate(['/warehouse/inventory-delivery-voucher/details', { id: Id }]);
  }
  refreshFilter() {
    this.voucherCode = '';
    this.listStatusSelectedId = [];
    this.listWarehouseSelectedId = [];
    this.listCreateVoucherSelectedId = [];
    this.listStorekeeperSelectedId = [];
    this.listVendorSelectedId = [];
    this.listProductSelectedId = [];
    this.listCreateDate = null;
    this.listInventoryReceivingDate = null;
    this.listStatusSelected = [];
    this.listWarehouseSelected = [];
    this.listCreateVoucherSelected = [];
    this.listVendorSelected = [];
    this.listProductSelected = [];

    this.serial = '';
    let findNhap = this.listStatus.find(f => f.categoryCode == 'NHA');
    if (findNhap != null) {
      this.listStatusSelected.push(findNhap);
    }
    let currentUser = this.listCreateVoucher.find(item => item.employeeId == this.auth.EmployeeId);
    this.listCreateVoucherSelected.push(currentUser);


    this.warehouseService.searchInventoryDeliveryVoucher(this.voucherCode, this.listStatusSelectedId, this.listWarehouseSelectedId,
      this.listCreateVoucherSelectedId, this.listStorekeeperSelectedId, this.listVendorSelectedId, this.listProductSelectedId,
      this.listCreateDate, this.listInventoryReceivingDate, this.serial, this.auth.UserId
    ).subscribe(response2 => {
      let result2 = <any>response2;
      this.listInventoryReceivingVoucher = result2.lstResult;
      if (result2.lstResult == null) { this.listInventoryReceivingVoucher = []; }
      this.SumRow = this.listInventoryReceivingVoucher.length;
      this.currentDate = new Date();
      this.listInventoryReceivingVoucher.forEach(function (item) {
        switch (item.inventoryDeliveryVoucherType) {
          case 1:
            item.inventoryDeliveryVoucherTypeName = 'Xuất bán hàng';
            break;
          case 2:
            item.inventoryDeliveryVoucherTypeName = 'Xuất trả lại nhà cung cấp';
            break;
          case 3:
            item.inventoryDeliveryVoucherTypeName = 'Xuất hàng biếu tặng';
            break;
          case 4:
            item.inventoryDeliveryVoucherTypeName = 'Xuất hàng kiểm định';
            break;
          default:
            item.inventoryDeliveryVoucherTypeName = 'Khác';
        }
      });
      if (this.listInventoryReceivingVoucher.length == 0) {
        this.messageService.clear();
        this.messageService.add({ key: 'info', severity: 'info', summary: ' Không tìm thấy phiếu xuất kho', detail: 'Danh sách xuất kho' });
      }
      this.visibleSidebar2 = true;
    }, error => { });

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

      ///**Customize sort  */
      //if (event.field == 'nameVendor') {
        
      //  let value1 = data1['vedorName'];
      //  let value2 = data2['vedorName'];
      //  let result = null;
      //  if (value1 == null && value2 != null)
      //    result = -1;
      //  else if (value1 != null && value2 == null)
      //    result = 1;
      //  else if (value1 == null && value2 == null)
      //    result = 0;
      //  else if (typeof value1 === 'string' && typeof value2 === 'string')
      //    result = value1.localeCompare(value2);
      //  else
      //    result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      //  return (event.order * result);
      //}

      //if (event.field == 'productName') {

      //  let value1 = data1['nameObject'];
      //  let value2 = data2['nameObject'];
      //  let result = null;
      //  if (value1 == null && value2 != null)
      //    result = -1;
      //  else if (value1 != null && value2 == null)
      //    result = 1;
      //  else if (value1 == null && value2 == null)
      //    result = 0;
      //  else if (typeof value1 === 'string' && typeof value2 === 'string')
      //    result = value1.localeCompare(value2);
      //  else
      //    result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      //  return (event.order * result);
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
  pageChange(event: any) {
  }
  goToDetailObject(rowData: any) {
    if (rowData.inventoryDeliveryVoucherType == 2) {
      this.router.navigate(['/vendor/detail', { vendorId: rowData.vendorId, contactId: this.emptyGuid }]);
    }
    else {
      this.router.navigate(['/customer/detail', { customerId: rowData.vendorId, contactId: this.emptyGuid }]);
    }
  }

  goOrder(rowData: any) {
    if (rowData.inventoryDeliveryVoucherType == 1) {
      this.goToCustomerOrderDetail(rowData);
    }
    else if (rowData.inventoryDeliveryVoucherType == 2) {
      this.goToVendorOrderDetail(rowData);
    }
  }
  goToCustomerOrderDetail(resource: any) {
    this.router.navigate(['/order/order-detail', { customerOrderID: resource.objectId }]);
  }
  goToVendorOrderDetail(resource: any) {
    this.router.navigate(['/vendor/detail-order', { vendorOrderId: resource.objectId }]);
  }
  onDatesRangeinventoryCreateDateCalendar(selectedValue: Date) {
    if (this.listCreateDate[1]) { // If second date is selected
      this.inventoryCreateDateCalendar.hideOverlay()
    };
  }

  onDatesRangeinventoryReceivingDate(selectedValue: Date) {
    if (this.listInventoryReceivingDate[1]) { // If second date is selected
      this.inventoryReceivingDateCalendar.hideOverlay()
    };
  }

}
