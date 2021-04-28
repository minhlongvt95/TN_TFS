import { Component, OnInit, ViewChild } from '@angular/core';
import { Table } from 'primeng/table';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, SortEvent } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Router, ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { SaleBiddingService } from '../../services/sale-bidding.service';


interface SaleBidding {
  saleBiddingId: string;
  saleBiddingName: string;
  leadId: string;
  customerId: string;
  valueBid: number;
  startDate: Date;
  address: string;
  bidStartDate: Date;
  personInChargeId: string;
  effecTime: number;
  endDate: Date;
  typeContractId: string;
  formOfBid: string;
  currencyUnitId: string;
  ros: number;
  saleBiddingCode: string;
  provisionalGrossProfit: number;
  typeContractName: string;
  slowDay: number;
  employeeId: string;
  statusId: string;
  statusName: string;
  customerName: string;
  personInChargeName: string;
}

class Employee {
  employeeId: string;
  employeeName: string;
}

class Customer {
  customerCode: string;
  customerId: string;
  customerName: string;
  displayName: string;
  customerGroup: string;
  customerPhone: string;
  fullAddress: string;
  taxCode: string;
  personInCharge: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}

class StatusSaleBiddingModel {
  saleBiddingId: string;
  statusId: string;
  note: string
}

@Component({
  selector: 'app-sale-bidding-approved',
  templateUrl: './sale-bidding-approved.component.html',
  styleUrls: ['./sale-bidding-approved.component.css']
})
export class SaleBiddingApprovedComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  customerName:string;
  colsList: any[];
  selectedColumns: any[];
  listSaleBidding: Array<SaleBidding> = [];
  listEmployee: Array<Employee> = [];
  listCustomer: Array<Customer> = [];
  customerSelect: Customer;
  employeeSelect: Array<Employee>= [];
  saleBiddingName: string;
  toDate: Date = null;
  fromDate: Date = null;
  loading: boolean = false;
  isApproved:boolean = false;
  innerWidth: number = 0; //number window size first
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  //display on html  
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string;
  displayDialogStatus: boolean = false;
  displayDialogStatusAppr: boolean = false;
  reasonRefuse: string = "";
  listSaleBiddingSelected: Array<SaleBidding> = [];
  saleBiddingIdSelected: string = null;
  listStatusSaleBidding: Array<Category> = [];
  rejectAll:boolean  = false;
  apprAll:boolean  = false;
  actionAdd:boolean = true;
  actionReject:boolean = true;
  actionApprove:boolean = true;

  constructor(
    private translate: TranslateService,
    private dialogService: DialogService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private saleBiddingService: SaleBiddingService
  ) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "crm/sale-bidding/approved";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("approve") == -1) {
        this.actionApprove = false;
      }
      if (listCurrentActionResource.indexOf("reject") == -1) {
        this.actionReject = false;
      }

      this.initTable();
      this.getMasterData();
    }
    
  }

  initTable() {
    this.colsList = [
      { field: 'saleBiddingName', header: 'Tên gói thầu', textAlign: 'left', display: 'table-cell', width: "20%" },
      { field: 'customerName', header: 'Chủ đầu tư', textAlign: 'left', display: 'table-cell', width: '15%' },
      { field: 'bidStartDate', header: 'Ngày nộp thầu', textAlign: 'right', display: 'table-cell', width: '13%' },
      { field: 'personInChargeName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '15%' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '10%' },
      { field: 'valueBid', header: 'Giá trị thầu', textAlign: 'right  ', display: 'table-cell', width: '10%' },
    ];
    this.selectedColumns = this.colsList;
  }

  getMasterData() {
    this.loading = true;
    this.saleBiddingService.getMasterDataSaleBiddingApproved().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listSaleBidding = result.listSaleBidding;
        this.listEmployee = result.listEmployee;
        this.listCustomer = result.listCustomer;
        this.listStatusSaleBidding = result.listStatus;
        if(this.listSaleBidding == null || this.listSaleBidding.length == 0){
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy hồ sơ thầu nào!' };
          this.showMessage(msg);
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  approveOrReject(type: number, saleBiddingId: string,isAll:boolean) {
    let isUpdateStatus: boolean = false;
    let statusId: string = "";
    let listSaleBiddingUpdate: Array<StatusSaleBiddingModel> = [];
    
    // Đổi trạng thái hồ sơ thầu sang hủy
    if (type == 1) {
      if (this.reasonRefuse == null || this.reasonRefuse.trim().length == 0) {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: "Bạn chưa nhập lí do từ chối" };
        this.showMessage(mgs);
      } else {
        if (this.saleBiddingIdSelected != null) {
          // Xét trạng thái hồ sơ thầu
          let status = this.listStatusSaleBidding.find(x => x.categoryCode == "REFU");
          statusId = status.categoryId;
          isUpdateStatus = true;
        }
      }
    } else if (type == 2) {
      let status = this.listStatusSaleBidding.find(x => x.categoryCode == "APPR");
      statusId = status.categoryId;
      isUpdateStatus = true;
    }

    if(isAll == true || (saleBiddingId == null && isAll == null && this.rejectAll)){
      this.listSaleBiddingSelected.forEach(item => {
        let saleBiddingUpdate: StatusSaleBiddingModel = new StatusSaleBiddingModel();
        saleBiddingUpdate.saleBiddingId = item.saleBiddingId;
        saleBiddingUpdate.statusId = statusId;
        saleBiddingUpdate.note = this.reasonRefuse;
        listSaleBiddingUpdate.push(saleBiddingUpdate);
      });
    }
    else {
      let saleBiddingUpdate: StatusSaleBiddingModel = new StatusSaleBiddingModel();
      saleBiddingUpdate.saleBiddingId = saleBiddingId;
      saleBiddingUpdate.statusId = statusId;
      saleBiddingUpdate.note = this.reasonRefuse;
      listSaleBiddingUpdate.push(saleBiddingUpdate);
    }

    if (isUpdateStatus) {
      this.loading = true;
      this.saleBiddingService.updateStatusSaleBidding(listSaleBiddingUpdate).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Sửa trạng thái thành công" };
          this.displayDialogStatusAppr = false;
          this.displayDialogStatus = false;
          this.showMessage(mgs);
          this.refreshFilter();
        }
        else {
          let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(mgs);
        }
      });
    }
  }

  reject(saleBiddingId: string) {
    this.displayDialogStatus = true;
    if (saleBiddingId == null) {
      this.rejectAll = true;
    }
    else{
      this.saleBiddingIdSelected = saleBiddingId;
      this.rejectAll = false;
    }
  }

  approvalShowDiaLog(saleBiddingId: string){
    this.displayDialogStatusAppr = true;
    if (saleBiddingId == null) {
      this.apprAll = true;
    }
    else{
      this.saleBiddingIdSelected = saleBiddingId;
      this.apprAll = false;
    }
  }
  
  searchSaleBidding() {
    let employeeId: Array<string> = [];
    if (this.employeeSelect == null) {
      employeeId = null;
    }
    else {
      employeeId = this.employeeSelect.map(x=>x.employeeId);
    }
    let toDate: Date = this.toDate;
    let fromDate: Date = this.fromDate;
    if (toDate != null) {
      toDate = convertToUTCTime(toDate);
    }
    if (fromDate != null) {
      fromDate = convertToUTCTime(fromDate);
    }
    this.loading = true;
    this.saleBiddingService.searchSaleBiddingApproved(this.saleBiddingName, this.customerName, employeeId, fromDate, toDate,this.isApproved).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listSaleBidding = result.listSaleBidding;
        if(this.listSaleBidding == null || this.listSaleBidding.length == 0){
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy hồ sơ thầu nào!' };
          this.showMessage(msg);
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }
  refreshFilter() {
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.fromDate = null;
    this.toDate = null;
    this.saleBiddingName = null;
    this.employeeSelect = null;
    this.isShowFilterTop = false;
    this.isShowFilterLeft = false;
    this.customerName = null;
    this.isApproved = false;
    this.searchSaleBidding();
  }
  leftColNumber: number = 12;
  rightColNumber: number = 0;
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
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'bidStartDate') {
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

  goToDetail(rowData: any) {
    this.router.navigate(['/sale-bidding/detail', { saleBiddingId: rowData.saleBiddingId }]);
  }
}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};