import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { EmployeeService } from "../../services/employee.service";
import { EmployeeListService } from '../../services/employee-list/employee-list.service';
import { OrganizationService } from "../../../shared/services/organization.service";
import { EmployeeModel } from "../../models/employee.model";
import { ContactModel } from "../../../shared/models/contact.model";
import { UserModel } from '../../../shared/models/user.model';
import * as $ from 'jquery';
import { GetPermission } from '../../../shared/permission/get-permission';
import { OrganizationDialogComponent } from '../../../shared/components/organization-dialog/organization-dialog.component';

import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import * as moment from 'moment';
import 'moment/locale/pt-br';

interface Employee {
  index: number,
  employeeId: string,
  employeeName: string,
  organizationId: string,
  organizationName: string,
  employeeCode: string,
  positionId: string,
  createdById: string,
  createdDate: string,
  active: boolean,
  username: string,
  contactId: string,
}

@Component({
  selector: 'app-list',
  templateUrl: './employee-quit-work.component.html',
  styleUrls: ['./employee-quit-work.component.css']
})
export class EmployeeQuitWorkComponent implements OnInit {

  @HostListener('document:keyup', ['$event'])
  handleDeleteKeyboardEvent(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.searchEmployee();
    }
  }
  excel: any = {
    'STT': '',
    'Tên': '',
    'Tên tài khoản': '',
    'Mã': '',
    'Đơn vị': '',
  };
  excelExport: Array<any> = [];
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = 'employee/create';

  currentOrganizationId: string = '';
  messages: any;

  selection: Array<any>;
  empOrganizationNameDisplay = '';
  listEmp: Array<Employee> = [];
  isManager: boolean = null;
  employeeId: string = null;

  units: Array<string> = [];

  employeeModel = new EmployeeModel()
  contactModel = new ContactModel()
  userModel: UserModel = {
    UserId: null, Password: '123', UserName: '', EmployeeId: '', Disabled: false, CreatedById: 'DE2D55BF-E224-4ADA-95E8-7769ECC494EA', CreatedDate: null, UpdatedById: null, UpdatedDate: null, Active: true
  };

  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();

  leftColNumber: number = 12;
  rightColNumber: number = 0;
  nowDate: Date = new Date();
  /*Check user permission*/
  @ViewChild('myTable') myTable: Table;
  colsList: any;
  selectedColumns: any[];
  filterGlobal: string;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private organizationService: OrganizationService,
    private employeeListService: EmployeeListService,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    public dialogService: DialogService,
    private confirmationService: ConfirmationService
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  goToCreate() {
    this.router.navigate(['/employee/create']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  async ngOnInit() {
    let resource = "hrm/employee/employee-quit-work/";
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
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;
      this.initTable();
      this.contactModel.FirstName = "";
      this.contactModel.LastName = "";
      this.userModel.UserName = "";
      this.contactModel.IdentityID = "";
      this.searchEmployee();
    }
  }

  initTable() {
    this.colsList = [
      { field: 'employeeName', header: 'Tên', textAlign: 'left', display: 'table-cell' },
      { field: 'username', header: 'Tên tài khoản', textAlign: 'left', display: 'table-cell' },
      { field: 'employeeCode', header: 'Mã', textAlign: 'left', display: 'table-cell' },
      { field: 'organizationName', header: 'Đơn vị', textAlign: 'left', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
  }

  searchEmployee() {
    this.loading = true;
    this.employeeListService.searchEmployeeFromList(
      this.isManager,
      this.employeeId,
      this.contactModel.FirstName,
      this.contactModel.LastName,
      this.userModel.UserName,
      this.contactModel.IdentityID,
      [],
      this.employeeModel.OrganizationId, null, null, null, null, true).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.currentOrganizationId = result.currentOrganizationId;
          this.listEmp = result.employeeList;
          if (this.listEmp.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy nhân viên đã nghỉ việc nào!' };
            this.showMessage(msg);
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
        this.setIndex();
      }, error => { });
  };

  setIndex() {
    this.listEmp.forEach((item, index) => {
      item.index = index + 1;
    });
  }

  onViewDetail(rowData: any) {
    this.router.navigate(['/employee/detail', { employeeId: rowData.employeeId, contactId: rowData.contactId }]);
  }

  refreshFilter() {
    this.contactModel.FirstName = '';
    this.contactModel.LastName = '';
    this.userModel.UserName = '';
    this.contactModel.IdentityID = '';
    this.employeeModel.OrganizationId = this.currentOrganizationId;
    this.empOrganizationNameDisplay = '';

    this.filterGlobal = '';
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.listEmp = [];

    this.searchEmployee();
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
          this.employeeModel.OrganizationId = result.selectedOrgId;
          this.employeeModel.OrganizationName = result.selectedOrgName;
        }
      }
    });
  }

  deleteUser(employeeId : any){
    this.loading = true;
    this.employeeListService.disableEmployee(employeeId).subscribe(response =>{
      let result = <any> response;
      this.loading = false;
      if(result.statusCode === 200 || result.statusCode === 202 ){
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa nhân viên thành công!' };
        this.showMessage(msg);
        this.searchEmployee();
      }else{
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  del_user(rowData : any){
    this.confirmationService.confirm({
      message : 'Bạn có chắc muốn xóa nhân viên này?',
      accept: () =>{
        this.deleteUser(rowData.employeeId);
      }
    })
  }
  exportExcel() {
    var listCheck: any = this.selection;
    if(this.selection.length > 0){
      this.excelExport = [];
    }
    for (var i = 0; i < listCheck.length; i++) {
      this.excel = {
        'STT': i + 1,
        'Tên': listCheck[i].employeeName,
        'Tên tài khoản': listCheck[i].username,
        'Mã': listCheck[i].employeeCode,
        'Đơn vị': listCheck[i].organizationName,
      }
      this.excelExport.push(this.excel);
    }
    if (this.excelExport.length == 0) this.excelExport.push(this.excel);
    this.employeeService.exportExcel(this.excelExport, 'Excel_Danh sách nhân viên');
  }
  /*End*/
}
