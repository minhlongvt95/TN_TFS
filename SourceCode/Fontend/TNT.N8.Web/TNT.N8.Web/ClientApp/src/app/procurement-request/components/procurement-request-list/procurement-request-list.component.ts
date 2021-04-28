
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CategoryService } from '../../../shared/services/category.service';

import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { OrganizationpopupComponent } from '../../../shared/components/organizationpopup/organizationpopup.component';
import { EmployeeService } from '../../../employee/services/employee.service';
import { ProcurementRequestService } from '../../services/procurement-request.service';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import * as moment from 'moment';
import { DialogService } from 'primeng/dynamicdialog';
import { OrganizationDialogComponent } from '../../../shared/components/organization-dialog/organization-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';

interface ProcurementRequest {
  procurementRequestId: string;
  procurementCode: string;
  procurementContent: string;
  requestEmployeeId: string;
  employeePhone: string;
  unit: string;
  approverId: string;
  approverPostion: string;
  explain: string;
  statusId: string;
  createdById: string;
  createdDate: string;
  updatedById: string;
  updatedDate: string;
  requestEmployeeName: string;
  approverName: string;
  statusName: string;
  organizationName: string;
  totalMoney: Number;
  backgroundColorForStatus: string;
}

@Component({
  selector: 'app-procurement-request-list',
  templateUrl: './procurement-request-list.component.html',
  styleUrls: ['./procurement-request-list.component.css'],
  providers: [
    DatePipe,
  ]
})
export class ProcurementRequestListComponent implements OnInit {
  // Mảng chứa Procurement Request
  listProcurementRequest: Array<ProcurementRequest> = [];
  filterForm: FormGroup;

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  auth: any = JSON.parse(localStorage.getItem("auth"));

  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  employeeList: Array<any> = [];

  nowDate = new Date();
  filterGlobal: string;

  prcmCode = '';
  fromDate: Date = null;
  toDate: Date = null;
  prcmOrgId: string;
  prcmOrgName: string;
  selectedRequester: Array<any> = [];
  selectedStatus: Array<any> = [];
  selectedProduct: Array<any> = [];
  selectedRequesterToSearch: Array<any> = [];

  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  frozenCols: any[];

  // Khai báo các mang/ biến chứa masterData
  listStatus: Array<any> = [];
  listEmp: Array<any> = [];
  listProduct: Array<any> = [];
  // filterEmp: Observable<any>;
  // Khai báo popup
  dialogOrg: MatDialogRef<OrganizationpopupComponent>;
  dateFieldFormat: string = 'DD/MM/YYYY';
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  messageTitle: string = '';
  message: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    public dialog: MatDialog,
    private employeeService: EmployeeService,
    public snackBar: MatSnackBar,
    private procurementRequestService: ProcurementRequestService,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private translate: TranslateService,
    private datePipe: DatePipe,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "buy/procurement-request/list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.initTable();
      this.procurementRequestService.getMasterDataSearchProcurementRequest().subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.listEmp = result.listEmployee;
          this.listStatus = result.listStatus;
          this.listProduct = result.listProduct;
          this.listEmp.forEach(item => {
            item.employeeName = item.employeeCode + ' - ' + item.employeeName;
          });
          this.searchProcurementRequestData();
        } else {
          this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
          let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }


  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  initTable() {
    this.colsList = [
      { field: 'procurementCode', header: 'Mã phiếu', width: '4%', textAlign: 'left', display: 'table-cell' },
      { field: 'requestEmployeeName', header: 'Người yêu cầu', width: '6%', textAlign: 'left', display: 'table-cell' },
      { field: 'procurementContent', header: 'Nội dung', width: '5%', textAlign: 'left', display: 'table-cell' },
      { field: 'organizationName', header: 'Bộ phận', width: '5%', textAlign: 'left', display: 'table-cell' },
      { field: 'sumQuantity', header: 'Tổng SL yêu cầu', width: '6%', textAlign: 'right', display: 'table-cell' },
      { field: 'sumQuantityApproval', header: 'Tổng SL duyệt', width: '6%', textAlign: 'right', display: 'table-cell' },
      { field: 'createdDate', header: 'Ngày tạo', width: '4%', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', width: '7%', textAlign: 'center', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsList;
  }

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  // Search ProcurementRequest
  searchProcurementRequestData() {
    if (this.prcmCode) {
      this.prcmCode = this.prcmCode.trim();
    }
    let listEmpIds: Array<string> = [];
    listEmpIds = this.selectedRequester.map(c => c.employeeId);

    let listStatusIds: Array<string> = [];
    listStatusIds = this.selectedStatus.map(c => c.categoryId);

    let listProductIds: Array<string> = [];
    listProductIds = this.selectedProduct.map(c => c.productId);

    let org: string = '';
    if (this.prcmOrgId) {
      org = this.prcmOrgId;
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
    this.loading = true;
    this.procurementRequestService.searchProcurementRequest(this.prcmCode, this.fromDate, this.toDate, listEmpIds, listStatusIds, org, this.auth.userId, listProductIds).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
      if (result.statusCode == 200) {
        this.listProcurementRequest = result.listProcurementRequest;
        if (this.listProcurementRequest.length == 0) {
          this.translate.get('procurement-request.list.no_search_data').subscribe(value => { this.message = value; });
          let msg = { severity: 'warn', summary: this.messageTitle, detail: this.message };
          this.showMessage(msg);
        } else {
          this.listProcurementRequest.forEach(item => {
            item.createdDate = this.datePipe.transform(item.createdDate, 'dd/MM/yyyy');
          });
        }
      } else {
        let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }


  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 9;
        this.rightColNumber = 3;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

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

  refreshFilter() {
    this.prcmCode = '';
    this.fromDate = null;
    this.toDate = null;
    this.selectedRequester = [];
    this.selectedStatus = [];
    this.prcmOrgName = '';
    this.prcmOrgId = '';
    this.filterGlobal = "";
    this.listProcurementRequest = [];

    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;

    this.searchProcurementRequestData();
  }

  openOrgPopup() {
    this.translate.get('procurement-request.list.unit').subscribe(value => { this.message = value; });
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: {
        chooseFinancialIndependence: false
      },
      header: this.message,
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
          this.prcmOrgId = result.selectedOrgId;
          this.prcmOrgName = result.selectedOrgName;
        }
      }
    });
  }

  gotoView(value: any) {
    this.router.navigate(['/procurement-request/view', { id: value.procurementRequestId }])
  }

  goToCreateProcurementRequest() {
    this.router.navigate(['/procurement-request/create'])
  }
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};



