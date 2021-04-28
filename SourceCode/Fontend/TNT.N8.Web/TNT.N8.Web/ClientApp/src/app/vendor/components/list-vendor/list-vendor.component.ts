import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { VendorService } from "../../services/vendor.service";
import { CategoryService } from "../../../shared/services/category.service";
import { VendorModel } from "../../models/vendor.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { GetPermission } from '../../../shared/permission/get-permission';

import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';

class vendorGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class vendorModel {
  vendorId: string;
  vendorName: string
  vendorCode: string;
  vendorGroupId: string;
  vendorGroupName: string;
  active: boolean;
  contactId: string;
  countVendorInformation: number;
  createdById: string;
  createdDate: string;
  paymentId: string;
  totalPayableValue: number;
  totalPurchaseValue: number;
  canDelete: boolean;
}

@Component({
  selector: 'app-list-vendor',
  templateUrl: './list-vendor.component.html',
  styleUrls: ['./list-vendor.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class ListVendorComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  //master data
  listVendorGroup: Array<vendorGroupModel> = [];
  listVendor: Array<vendorModel> = [];
  //form
  searchVendorForm: FormGroup;
  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';
  first: number = 0;
  colsListProduct: any;
  selectedColumns: any[];
  rows: number = 10;

  actionAdd: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = "vendor/create";
  viewPermission: string = "vendor/view";
  editPermission: string = 'vendor/edit';
  vendorOrderPermission: string = 'vendor-order';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  constructor(private translate: TranslateService,
    private vendorService: VendorService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private categoryService: CategoryService) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "buy/vendor/list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.initForm();
      this.initTable();
      this.getMasterData();
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.searchVendorForm = new FormGroup({
      "VendorName": new FormControl(''),
      "VendorCode": new FormControl(''),
      "VendorGroup": new FormControl([])
    });
  }

  patchValueForm() {
    this.searchVendorForm.reset();
    this.searchVendorForm.patchValue({
      "VendorName": '',
      "VendorCode": '',
      "VendorGroup": []
    });
  }

  initTable() {
    this.colsListProduct = [
      { field: 'vendorCode', header: 'Mã', textAlign: 'left', display: 'table-cell' },
      { field: 'vendorName', header: 'Nhà cung cấp', textAlign: 'left', display: 'table-cell' },
      { field: 'vendorGroupName', header: 'Nhóm', textAlign: 'left', display: 'table-cell' },
      { field: 'totalPayableValue', header: 'Số tiền phải thanh toán', textAlign: 'right', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListProduct;
  }

  refreshFilter() {
    this.searchVendorForm.reset();
    this.patchValueForm();
    this.resetTable();
    this.searchVendor();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
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

  resetTable() {
    this.listVendor = [];
    this.filterGlobal = '';
    this.first = 0;
    // this.myTable.reset();
  }

  async getMasterData() {
    this.loading = true;
    let [masterdataResponse, initSearchResponse]: any = await Promise.all([
      this.vendorService.getDataSearchVendor(this.auth.UserId),
      this.vendorService.searchVendorAsync('', '', [], this.auth.UserId)
    ]);
    this.loading = false;
    if (masterdataResponse.statusCode === 200 && initSearchResponse.statusCode === 200) {
      this.listVendorGroup = masterdataResponse.listVendorGroup;
      this.listVendor = initSearchResponse.vendorList;
    } else if (masterdataResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', masterdataResponse.messageCode);
    } else if (initSearchResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', initSearchResponse.messageCode);
    }
  }

  goToCreate() {
    this.router.navigate(['/vendor/create']);
  }

  goToDetail(rowData: vendorModel) {
    this.router.navigate(['/vendor/detail', { vendorId: rowData.vendorId, contactId: rowData.contactId }]);
  }

  deleteVendor(rowData: vendorModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.vendorService.updateActiveVendor(rowData.vendorId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listVendor = this.listVendor.filter(e => e != rowData);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa nhà cung cấp thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
          this.ngOnInit();
        }, error => { });
      }
    });
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchVendor();
    }
  }

  async searchVendor() {
    let name = this.searchVendorForm.get('VendorName').value;
    let code = this.searchVendorForm.get('VendorCode').value;
    let listVendorGroupId = this.searchVendorForm.get('VendorGroup').value.map(e => e.categoryId);
    this.loading = true;
    let result: any = await this.vendorService.searchVendorAsync(name, code, listVendorGroupId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.resetTable(); //reset state of table
      this.listVendor = result.vendorList;
      if (this.listVendor.length == 0) {
        this.showToast('warn', 'Thông báo', 'Không tìm thấy nhà cung cấp nào')
      }
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

}
