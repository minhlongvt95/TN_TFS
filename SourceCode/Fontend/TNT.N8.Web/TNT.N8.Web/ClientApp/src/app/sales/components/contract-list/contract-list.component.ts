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
import { CategoryService } from '../../../shared/services/category.service';
import { ContractService } from '../../services/contract.service';
import { runInThisContext } from 'vm';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

interface Contract {
  contractId: string,
  contractCode: string,
  quoteCode: string,
  customerId: string,
  effectiveDate: string,
  valueContract: string,
  employeeId: string,
  contractDescription: string,
  statusId: string,
  createdDate: string,
  createdById: string,
  nameCustomer: string,
  nameEmployee: string,
  nameStatus: string,
  statusCode: string,
  nameCreateBy: string,
  canDelete: boolean,
  backgroupStatus: string;
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
  selector: 'app-contract-list',
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.css']
})
export class ContractListComponent implements OnInit {

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
  auth = JSON.parse(localStorage.getItem('auth'));

  //Biến lưu giá trị trả về
  listStatusMaster: Array<Category> = [];
  listTypeContractMaster: Array<Category> = [];
  listCustomerMaster: Array<Customer> = [];
  listEmployeeMaster: Array<Employee> = [];
  listProductMaster: Array<any> = [];
  listContract: Array<Contract> = [];
  /*Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  /*End*/
  actions: MenuItem[] = [];
  // Value search
  contractCode: string = '';
  quoteCode: string = '';
  listCustomer: Array<Customer> = [];
  listEmployee: Array<Employee> = [];
  listProduct: Array<any> = [];
  fromDate: Date = null;
  toDate: Date = null;


  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private contractService: ContractService
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "sal/sales/contract-list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.setTable();
      this.getMasterData();
      this.searchContract();
    }
  }

  setTable() {
    this.cols = [
      { field: 'contractCode', header: 'Mã hợp đồng', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'nameCustomer', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'effectiveDate', header: 'Ngày hợp đồng', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'valueContract', header: 'Giá trị hợp đồng', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'nameEmployee', header: 'Nhân viên bán hàng', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'contractDescription', header: 'Diễn giải', textAlign: 'right', display: 'table-cell' },
      { field: 'nameStatus', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'createdDate', header: 'Ngày tạo', textAlign: 'right', display: 'table-cell' },
      { field: 'nameCreateBy', header: 'Người tạo', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumns = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
      || e.field == "valueContract" || e.field == "nameEmployee" || e.field == "nameStatus");
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.contractService.getMasterDataSearchContract();
    this.loading = false;
    this.listStatusMaster = result.listStatus;
    this.listTypeContractMaster = result.listTypeContract;
    this.listCustomerMaster = result.listCustomer;
    this.listEmployeeMaster = result.listEmployee;
    this.listProductMaster = result.listProduct;
    this.listCustomerMaster.forEach(item => {
      item.customerName = item.customerCode + ' - ' + item.customerName;
    });
    this.listProductMaster.forEach(item => {
      item.productName = item.productCode + ' - ' + item.productName;
    });
    this.listEmployeeMaster.forEach(item => {
      item.employeeName = item.employeeCode + ' - ' + item.employeeName;
    });
  }

  searchContract() {
    if (this.contractCode) {
      this.contractCode = this.contractCode.trim();
    }
    if (this.quoteCode) {
      this.quoteCode = this.quoteCode.trim();
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
    let listEmployeeId: Array<string> = [];
    listEmployeeId = this.listEmployee.map(c => c.employeeId);
    let listCustomerId: Array<string> = [];
    listCustomerId = this.listCustomer.map(c => c.customerId);
    let listProductId: Array<string> = [];
    listProductId = this.listProduct.map(c => c.productId);

    this.loading = true;
    this.contractService.searhcContract(this.contractCode, this.quoteCode, listCustomerId, listEmployeeId, listProductId, fromDate, toDate, this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listContract = result.listContract;
        this.handleBackGroundColorForStatus();
      } else {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy hợp đồng nào!' };
        this.showMessage(msg);
      }
    })
  }

  handleBackGroundColorForStatus() {
    this.listContract.forEach(item => {
      switch (item.statusCode) {
        case "APPR"://Xac nhan
          item.backgroupStatus = "#34c759";
          break;
        case "HTH"://dong
          item.backgroupStatus = "#5ac8fa";
          break;
        case "MOI"://nhap
          item.backgroupStatus = "#78a9ff";
          break;
        case "HUY"://huy
          item.backgroupStatus = "#ff2d55";
          break;
        case "DTH"://huy
          item.backgroupStatus = "#2c96c7";
          break;
        default:
          break;
      }
    });
  }

  // Refresh parameter search
  refreshFilter() {
    this.contractCode = '';
    this.quoteCode = '';
    this.fromDate = null;
    this.toDate = null;
    this.listCustomer = [];
    this.listEmployee = [];
    this.listProduct = [];
    this.isGlobalFilter = '';
    this.listContract = [];
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.isShowFilterLeft = false;

    this.searchContract();
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
        this.selectedColumns = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
          || e.field == "valueContract" || e.field == "nameStatus");
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        this.selectedColumns = this.cols.filter(e => e.field == "contractCode" || e.field == "nameCustomer" || e.field == "effectiveDate"
          || e.field == "valueContract" || e.field == "nameEmployee" || e.field == "nameStatus");
      }
    }
  }

  goToCreateQuote() {
    this.router.navigate(['/sales/contract-create']);
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/sales/contract-detail', { contractId: rowData }]);
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


  del_contract(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.contractService.deleteContract(rowData.contractId).subscribe(repsonse => {
          let result = <any>repsonse;
          if (result.statusCode == 200) {
            this.listContract = this.listContract.filter(c => c.contractId !== rowData.contractId);
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
      }
    });
  }
  showMessage(msg: any) {
    this.messageService.add(msg);
  }
  clear() {
    this.messageService.clear();
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

