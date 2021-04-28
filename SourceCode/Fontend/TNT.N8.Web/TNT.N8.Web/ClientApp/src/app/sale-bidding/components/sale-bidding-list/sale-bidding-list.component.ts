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
  index: number;
  saleBiddingId: string,
  saleBiddingName: string,
  saleBiddingCode: string,
  statusId: string,
  statusName: string,
  effecTime: number,
  phone: string,
  email: string,
  personInChargeName: string,
  personInChargeId: string,
  valueBid: number,
  customerId: string,
  customerName: string,
  createDate: string
  bidStartDate: Date;
  startDate: Date;
  backColor: string;
}

@Component({
  selector: 'app-sale-bidding-list',
  templateUrl: './sale-bidding-list.component.html',
  styleUrls: ['./sale-bidding-list.component.css']
})
export class SaleBiddingListComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  innerWidth: number = 0; //number window size first
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  //display on html  
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  searchAll: string = "";
  saleBiddingName: string;
  cusName: string;
  phone: string;
  email: string;
  listStatusId: Array<any> = [];
  listLeadTypeId: Array<any> = [];
  listPersonalInChangeId: Array<any> = [];
  listContractId: Array<any> = [];
  listInterestedGroupId: Array<any> = [];
  fromDate: Date;
  toDate: Date;
  filterGlobal: string;

  listStatus: Array<any> = [];
  listLeadType: Array<any> = [];
  listPersonalInChange: Array<any> = [];
  listContract: Array<any> = [];
  listInterestedGroup: Array<any> = [];
  listSaleBidding: Array<SaleBidding> = [];
  colsList: any[];
  selectedColumns: any[];

  mode: string;
  effectiveDate: string;

  @ViewChild('myTable') myTable: Table;
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
    this.route.params.subscribe(params => {
      this.mode = params['mode'];
      this.effectiveDate = params['date'];
    });
    let resource = "crm/sale-bidding/list/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.initTable();
      this.saleBiddingService.getMasterDataSearchSaleBidding(this.auth.UserId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode == 200) {
          this.listStatus = result.listStatus;
          this.listLeadType = result.listLeadType;
          this.listPersonalInChange = result.listPersonalInChange;
          this.listContract = result.listContract;
          this.listInterestedGroup = result.listInterestedGroup;
          this.searchSaleBidding();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {
    this.colsList = [
      { field: 'saleBiddingName', header: 'Tên gói thầu', textAlign: 'left', display: 'table-cell', width: "150px" },
      { field: 'customerName', header: 'Chủ đầu tư', textAlign: 'left', display: 'table-cell', width: '125px' },
      { field: 'email', header: 'Email', textAlign: 'right', display: 'table-cell', width: '170px' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'right', display: 'table-cell', width: '150px' },
      { field: 'personInChargeName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '120px' },
      { field: 'valueBid', header: 'Giá trị thầu', textAlign: 'right  ', display: 'table-cell', width: '80px' },
      { field: 'bidStartDate', header: 'Ngày nộp thầu', textAlign: 'right  ', display: 'table-cell', width: '90px' },
      { field: 'effecTime', header: 'Thời gian hiệu lực', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '120px' },
    ];
    this.selectedColumns = this.colsList.filter(e => e.field == "saleBiddingName" || e.field == "customerName" || e.field == "personInChargeName"
      || e.field == "valueBid" || e.field == "bidStartDate" || e.field == "effecTime" || e.field == "statusName");
  }

  goToDetail(rowData: any) {
    this.router.navigate(['/sale-bidding/detail', { saleBiddingId: rowData.saleBiddingId }]);
  }

  setIndex() {
    this.listSaleBidding.forEach((saleBidding, index) => {
      saleBidding.index = index + 1;
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  searchSaleBidding() {
    if (this.saleBiddingName) {
      this.saleBiddingName = this.saleBiddingName.trim();
    }
    if (this.cusName) {
      this.cusName = this.cusName.trim();
    }
    if (this.phone) {
      this.phone = this.phone.trim();
    }
    if (this.email) {
      this.email = this.email.trim();
    }

    let statusWinId = this.listStatus.find(x => x.categoryCode == 'WIN').categoryId; // hồ sơ thầu trạng thái thắng thầu
    let statusLoseId = this.listStatus.find(x => x.categoryCode == 'LOSE').categoryId; // hồ sơ thầu trạng thái thua thầu
    let statusWaitApproveId = this.listStatus.find(x => x.categoryCode == 'CHO').categoryId; // hồ sơ thầu trạng thái chờ phê duyệt
    let statusNewId = this.listStatus.find(x => x.categoryCode == 'NEW').categoryId; // hồ sơ thầu trạng thái mới tạo
    let statusCancelId = this.listStatus.find(x => x.categoryCode == 'CANC').categoryId; // hồ sơ thầu trạng thái hủy
    let statusApproveId = this.listStatus.find(x => x.categoryCode == 'APPR').categoryId; // hồ sơ thầu trạng thái đã duyệt
    let statusRejectId = this.listStatus.find(x => x.categoryCode == 'REFU').categoryId; // hồ sơ thầu trạng thái từ chối

    let listStatusId: Array<string> = [];
    if (this.listStatusId.length == 0) {
      switch (this.mode) {
        case 'statusCho':
          this.listStatusId = this.listStatus.filter(c => c.categoryId == statusWaitApproveId);
          break;
        case 'statusWinLose':
          this.listStatusId = this.listStatus.filter(c => c.categoryId == statusWinId || c.categoryId == statusLoseId);
          break;
        case 'Expired':
        case 'SlowStartDate':
        case 'InWeek':
          this.listStatusId = this.listStatus.filter(c => c.categoryId == statusNewId || c.categoryId == statusWaitApproveId || c.categoryId == statusApproveId || c.categoryId == statusRejectId)
          break;
        default:
          break;
      }
    }
    listStatusId = this.listStatusId.map(x => x.categoryId);
    let listLeadTypeId: Array<string> = [];
    listLeadTypeId = this.listLeadTypeId.map(x => x.categoryId);
    let listPersonalInChangeId: Array<string> = [];
    listPersonalInChangeId = this.listPersonalInChangeId.map(x => x.employeeId);
    let listContractId: Array<string> = [];
    listContractId = this.listContractId.map(x => x.categoryId);
    let listInterestedGroupId: Array<string> = [];
    listInterestedGroupId = this.listInterestedGroupId.map(x => x.categoryId);
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
    // let statusId = '';
    // this.route.params.subscribe(params => { statusId = params['statusId'] });

    this.saleBiddingService.searchSaleBidding(this.saleBiddingName, this.cusName, this.phone, this.email,
      listLeadTypeId, listContractId, listPersonalInChangeId, listStatusId, fromDate, toDate).subscribe(response => {
        let result = <any>response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.listSaleBidding = result.listSaleBidding;
          if (this.mode == 'Expired') {
            let listSaleBiddingExpired: Array<SaleBidding> = [];
            this.listSaleBidding.forEach(x => {
              let date = new Date(x.bidStartDate);
              date.setDate(date.getDate() + x.effecTime);
              date = new Date(date.getFullYear(), date.getMonth(), date.getDate());
              let effectDate = this.effectiveDate == "undefined" ? new Date() : new Date(this.effectiveDate);
              effectDate = new Date(effectDate.getFullYear(), effectDate.getMonth(), effectDate.getDate());
              if (date.getTime() < effectDate.getTime()) {
                listSaleBiddingExpired.push(x);
              }
            });
            this.listSaleBidding = listSaleBiddingExpired;
          }
          else if (this.mode == 'SlowStartDate') {
            let listSaleBiddingSlowStartDate: Array<SaleBidding> = [];
            this.listSaleBidding.forEach(x => {
              let startDate = new Date(x.startDate);
              let bidStartDate = new Date(x.bidStartDate);
              if (bidStartDate.getTime() > startDate.getTime()) {
                listSaleBiddingSlowStartDate.push(x);
              }
            });
            this.listSaleBidding = listSaleBiddingSlowStartDate;
          }
          else if (this.mode == 'InWeek') {
            let listSaleBiddingInWeek: Array<SaleBidding> = [];
            let date = new Date;
            date.setDate(date.getDate() + 7);
            while (date.getDay() != 1) {
              date.setDate(date.getDate() - 1);
            }
            let startLastWeek = new Date(date);
            let endLastWeek = new Date(date.setDate(date.getDate() + 6));
            this.listSaleBidding.forEach(x => {
              let startDate = new Date(x.startDate);
              if (startDate.getDate() >= startLastWeek.getDate()
                && startDate.getDate() <= endLastWeek.getDate()
                && startDate.getMonth() == startLastWeek.getMonth()
                && startDate.getFullYear() == startLastWeek.getFullYear()) {
                listSaleBiddingInWeek.push(x);
              }
            })
            this.listSaleBidding = listSaleBiddingInWeek;
          }0

          this.listSaleBidding.forEach(item => {
            let status = this.listStatus.find(x => x.categoryId == item.statusId);
            if (status != null && status != undefined && status.categoryCode == "NEW") {
              item.backColor = "#FFC000";
            }
            if (status != null && status != undefined && status.categoryCode == "CANC") {
              item.backColor = "#333333";
            }
            if (status != null && status != undefined && status.categoryCode == "CHO") {
              item.backColor = "#6D98E7";
            }
            if (status != null && status != undefined && status.categoryCode == "LOSE") {
              item.backColor = "#666666";
            }
            if (status != null && status != undefined && status.categoryCode == "WIN") {
              item.backColor = "#FF0000";
            }
            if (status != null && status != undefined && status.categoryCode == "REFU") {
              item.backColor = "#46B678";
            }
            if (status != null && status != undefined && status.categoryCode == "APPR") {
              item.backColor = "#9C00FF";
            }
          });

          this.setIndex();
          if (this.listSaleBidding.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy hồ sơ thầu nào!' };
            this.showMessage(msg);
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });

  }

  clear() {
    this.messageService.clear();
  }
  refreshFilter() {
    this.mode = null;
    this.effectiveDate = null;
    this.filterGlobal = '';
    this.fromDate = null;
    this.toDate = null;
    this.saleBiddingName = '';
    this.cusName = '';
    this.phone = '';
    this.email = '';
    this.listStatusId = [];
    this.listLeadTypeId = [];
    this.listPersonalInChangeId = [];
    this.listContractId = [];
    this.listInterestedGroupId = [];
    this.listSaleBidding = [];
    this.searchAll = "";
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
      if (event.field == 'orderDate') {
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

