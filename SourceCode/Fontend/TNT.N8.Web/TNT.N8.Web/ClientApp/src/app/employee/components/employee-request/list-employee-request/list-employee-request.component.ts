import { Component, OnInit, ViewChild } from '@angular/core';

import { OrganizationService } from '../../../../shared/services/organization.service';

import { TranslateService } from '@ngx-translate/core';
import { ListEmployeeRequestService } from '../../../services/employee-request/list-employee-request.service';
import {  FormBuilder} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';

import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';

import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import * as moment from 'moment';
import 'moment/locale/pt-br';


interface EmployeeRequest {
  index: number,
  approverName: string,
  createById: string,
  createDate: string,
  createEmployeeCode: string,
  createEmployeeId: string,
  detail: string,
  employeeRequestCode: string,
  employeeRequestId: string,
  enDate: string,
  managerId: string,
  notifyList: string,
  offerEmployeeCode: string,
  offerEmployeeId: string,
  offerEmployeeName: string,
  organization: string,
  organizationCode: string,
  organizationId: string,
  statusId: string,
  statusName: string,
  parentId: string,
  parentName: string,
  requestDate: string,
  startDate: string,
  typeReason: string,
  typeRequest: string,
  typeRequestName: string,
  contactId: string,
  backgroupStatusColor : string,
}

@Component({
  selector: 'app-list-employee-request',
  templateUrl: './list-employee-request.component.html',
  styleUrls: ['./list-employee-request.component.css'],
  providers: [
  ]
})
export class ListEmployeeRequestComponent implements OnInit {

  isManager: boolean = null;
  employeeId: string = null;
  currentOrganizationId: string = '';

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  employeeRequestCreatePermission: string = 'employeerequest/create-empRequest'
  employeeRequestEditPermission: string = 'employeerequest/update-empRequest'

  //khai báo các biến cho danh sách options TypeRequest
  listTypeRequest: Array<any> = [];
  //khai báo các biến cho danh sách options Status
  listStatus: Array<any> = [];

  //khai báo các danh sách lưu dữ liệu đầu vào
  listAllEmployeeRequest: Array<EmployeeRequest> = [];

  auth: any = JSON.parse(localStorage.getItem('auth'));

  //khai báo các thuộc tính để tìm kiếm 
  employeeRequestCode = "";
  offerEmployeeCode = "";
  offerEmployeeName = "";// search theo tên của nhân viên do có trường hợp nhiều tên giống nhau mà Id thì chỉ có một
  offerOrganizationId = "";
  startDate: Date = null;
  endDate: Date = null;
  selectedGroupTypeRequest: Array<any> = [];
  selectedGroupStatus: Array<any> = [];

  offerOrganizationName: string = "";
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  filterGlobal: string;

  leftColNumber: number = 12;
  rightColNumber: number = 0;

  nowDate: Date = new Date();
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];

  empPerAddRequest: boolean = false;

  /*Check user permission*/

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private orgService: OrganizationService,
    private employeeRequestService: ListEmployeeRequestService,
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private messageService: MessageService,
    public dialogService: DialogService,
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  goToCreate() {
    this.router.navigate(['/employee/request/create']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }
  async ngOnInit() {
    let resource = "hrm/employee/request/list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

      this.initTable();
      let result: any = await this.orgService.getOrganizationByEmployeeId(this.employeeId);
      this.currentOrganizationId = result.organization.organizationId;
      this.offerOrganizationId = this.currentOrganizationId;

      this.employeeRequestService.getDataSearchEmployeeRequest(this.auth.UserId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode == 200) {
          this.listStatus = result.listStatus;
          this.listTypeRequest = result.listTypeRequest;
          this.searchEmployeeRequest();

          this.employeeRequestService.checkEmployeeCreateRequest().subscribe(response => {
            var result = <any>response;
            this.empPerAddRequest = result.isEmpCreateRequest;
          });
        }
      });
    }
  }

  initTable() {
    this.colsList = [
      { field: 'employeeRequestCode', header: 'Mã đề xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'offerEmployeeName', header: 'Tên NV được đề xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'organization', header: 'Đơn vị', textAlign: 'left', display: 'table-cell' },
      { field: 'parentName', header: 'Chi nhánh', textAlign: 'left', display: 'table-cell' },
      { field: 'startDate', header: 'Từ ngày', textAlign: 'right', display: 'table-cell' },
      { field: 'enDate', header: 'Đến ngày', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'createDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell' },
      { field: 'approverName', header: 'Người phê duyệt', textAlign: 'left', display: 'table-cell' },
      { field: 'typeRequestName', header: 'Loại đề xuất', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = [
      { field: 'employeeRequestCode', header: 'Mã đề xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'offerEmployeeName', header: 'Tên NV được đề xuất', textAlign: 'left', display: 'table-cell' },
      { field: 'organization', header: 'Đơn vị', textAlign: 'left', display: 'table-cell' },
      { field: 'startDate', header: 'Từ ngày', textAlign: 'right', display: 'table-cell' },
      { field: 'enDate', header: 'Đến ngày', textAlign: 'right', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'typeRequestName', header: 'Loại đề xuất', textAlign: 'left', display: 'table-cell' },
    ];
  }

  onViewDetail(rowData: any) {
    this.router.navigate(['/employee/detail', { employeeId: rowData.offerEmployeeId, contactId: rowData.contactId }]);
  }

  searchEmployeeRequest() {
    if (this.employeeRequestCode) {
      this.employeeRequestCode = this.employeeRequestCode.trim();
    }
    if (this.offerEmployeeCode) {
      this.offerEmployeeCode = this.offerEmployeeCode.trim();
    }
    if (this.offerEmployeeName) {
      this.offerEmployeeName = this.offerEmployeeName.trim();
    }
    let startDate = null;
    if (this.startDate) {
      startDate = this.startDate;
      startDate.setHours(0, 0, 0, 0);
      startDate = convertToUTCTime(startDate);
    }

    let endDate = null;
    if (this.endDate) {
      endDate = this.endDate;
      endDate.setHours(23, 59, 59, 999);
      endDate = convertToUTCTime(endDate);
    }

    let listStatusId: Array<string> = [];
    listStatusId = this.selectedGroupStatus.map(x => x.categoryId);

    let listTypeRequestId: Array<string> = [];
    listTypeRequestId = this.selectedGroupTypeRequest.map(x => x.categoryId);

    this.loading = true;
    this.employeeRequestService.searchEmployeeRequest(this.employeeRequestCode, this.offerEmployeeCode, this.offerEmployeeName,
      this.offerOrganizationId, listTypeRequestId, listStatusId, startDate, endDate,
      this.auth.UserId).subscribe(response => {
        var result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listAllEmployeeRequest = result.employeeRequestList;
          if (this.listAllEmployeeRequest.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy đề xuất xin nghỉ nào!' };
            this.showMessage(msg);
          }
        }else{
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }

        this.setIndex();
      }, error => { });
  }
  errors: any = { isError: false, errorMessage: '' };

  setIndex() {
    this.listAllEmployeeRequest.forEach((item, index) => {
      item.index = index + 1;
    });
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

  //hàm refesh các input trong hàm search
  refreshFilter() {
    this.employeeRequestCode = null;
    this.offerEmployeeCode = null;
    this.offerEmployeeName = null;
    this.offerOrganizationId = this.currentOrganizationId;
    this.startDate = null;
    this.endDate = null;
    this.selectedGroupTypeRequest = [];
    this.selectedGroupStatus = [];

    this.filterGlobal = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.listAllEmployeeRequest = [];
    this.searchEmployeeRequest();
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

  openOrgPopup() {
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: {
        chooseFinancialIndependence: false
      },
      header: 'Chọn đơn vị',
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
          this.offerOrganizationId = result.selectedOrgId;
          this.offerOrganizationName = result.selectedOrgName;
        }
      }
    });
  }

  //hàm xử lý khi chọn một option trong chip list
  // selectedFn(event: MatSelectChange, type: string) {
  //   if (type == 'TypeRequest') {
  //     this.selectedGroupTypeRequest = event.value;
  //   }
  //   if (type == 'Status') {
  //     this.selectedGroupStatus = event.value;
  //   }
  // }
  // Filter chon từ ngay < den ngày
  filterStartedDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.endDate === null ? 0 : this.endDate.setHours(0, 0, 0, 0);

    return (n <= m || m === 0);
  }

  filterEndDate = (d: Date): boolean => {
    let n = d.setHours(0, 0, 0, 0);
    let m = this.startDate === null ? 0 : this.startDate.setHours(0, 0, 0, 0);

    return (n >= m || m === 0);
  }
  // Ket thuc

  goToRequest(rowData: any) {
    this.router.navigate(['/employee/request/detail', { requestId: rowData.employeeRequestId }]);
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};







