import { Type } from "@angular/compiler";
import { Component, OnInit, ViewChild } from "@angular/core";
import { yearsPerPage } from "@angular/material/datepicker";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import moment from "moment";

import { ConfirmationService, DialogService, MessageService, SortEvent, Table, } from "primeng";
import { GetPermission } from '../../../shared/permission/get-permission';
import { ReportLeadModel, TimeSearchModel } from "../../models/lead.model";
import { LeadService } from "../../services/lead.service";

interface TypeReport {
  Code: string;
  Name: string;
}

class Year {
  Name: string;
  Code: number
}

class EmployeeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

class CategoryModel {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

class GeographicalArea {
  geographicalAreaId: string;
  geographicalAreaCode: string;
  geographicalAreaName: string;
}

@Component({
  selector: "app-report-lead",
  templateUrl: "./report-lead.component.html",
  styleUrls: ["./report-lead.component.css"],
})
export class ReportLeadComponent implements OnInit {
  emptyGuid: string = "00000000-0000-0000-0000-000000000000";
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem(
    "ListPermissionResource"
  );
  @ViewChild("myTable") myTable: Table;
  innerWidth: number = 0; //number window size first
  minYear: number = 2010;
  currentYear: number = new Date().getFullYear();
  /*DATA*/
  listTypeReport: Array<TypeReport> = [
    { Name: "Báo cáo phân tích tuổi thọ trung bình cơ hội", Code: "AGE" },
    { Name: "Báo cáo thống kê số lượng, giá trị cơ hội theo người phụ trách", Code: "PIC" },
    { Name: "Báo cáo phân tích nguồn gốc cơ hội", Code: "SOURCE" },
    { Name: "Báo cáo phân tích theo thị trường(vùng miền)", Code: "ADDRESS" },
    { Name: "Báo cáo phân tích theo giai đoạn(trạng thái)", Code: "STATUS" },
  ];
  typeReport: TypeReport = this.listTypeReport[0];

  listType: Array<TypeReport> = [
    { Name: "Theo năm", Code: "IN" },
    { Name: "Trong khoảng", Code: "EQUAL" },
  ]
  selectedTypeStatus: TypeReport = this.listType[0];
  listYear: Array<Year> = [];
  yearNgModel = new Year();
  startYear = 1990;
  endYear = 2030;
  fromDate = new Date();
  toDate = new Date();
  /*END*/

  /*TABLE*/
  colsListReportLeadFollowAge: any[];
  selectedColListReportLeadFollowAge: any[];

  colsListReportLeadFollowPic: any[];
  selectedColListReportLeadFollowPic: any[];

  colsListReportLeadFollowSource: any[];
  selectedColListReportLeadFollowSource: any[];

  colsListReportLeadFollowProvincial: any[];
  selectedColListReportLeadFollowProvincial: any[];

  colsListReportLeadFollowMonth: any[];
  selectedColListReportLeadFollowMonth: any[];
  rows: number = 10;

  /*BIẾN ĐIỀU KIỆN*/
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  isGlobalFilter: string = '';
  /**/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listReportLeadAge: Array<ReportLeadModel> = [];
  listReportLeadPic: Array<ReportLeadModel> = [];
  listReportLeadSource: Array<ReportLeadModel> = [];
  listReportLeadProvincial: Array<ReportLeadModel> = [];
  listReportLeadStatus: Array<ReportLeadModel> = [];
  listEmployee: Array<EmployeeModel> = [];
  listSource: Array<CategoryModel> = [];
  listArea: Array<GeographicalArea> = [];
  /*END*/

  /*ĐIỀU KIỆN TÌM KIẾM*/
  lstEmployeNgModel: Array<EmployeeModel> = [];
  lstSourceNgModel: Array<CategoryModel> = [];
  lstAreaNgModel: Array<GeographicalArea> = [];
  /*END*/

  constructor(
    private translate: TranslateService,
    private dialogService: DialogService,
    private getPermission: GetPermission,
    private leadService: LeadService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {
    this.translate.setDefaultLang("vi");
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    let resource = "crm/lead/report-lead";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      this.setDataYear();
      this.initTable();
      this.getMasterData();
    }
  }

  initTable() {
    this.colsListReportLeadFollowAge = [
      { field: "leadName", header: "Tên cơ hội", textAlign: "left", display: "table-cell" },
      { field: "picName", header: "Nhân viên phụ trách", textAlign: "left", display: "table-cell", width: "200px", },
      { field: "probabilityName", header: "Xác suất thắng", textAlign: "left", display: "table-cell", width: "200px", },
      { field: "statusName", header: "Trạng thái", textAlign: "center", display: "table-cell", width: "200px", },
      { field: "dayCount", header: "Tuổi thọ(ngày)", textAlign: "right", display: "table-cell", width: "140px", },
    ];
    this.selectedColListReportLeadFollowAge = this.colsListReportLeadFollowAge.filter((e) => e.field == "leadName" || e.field == "picName" || e.field == "probabilityName" || e.field == "statusName" || e.field == "dayCount");

    this.colsListReportLeadFollowPic = [
      { field: "picCode", header: "Mã nhân viên", textAlign: "left", display: "table-cell", width: "200px", },
      { field: "picName", header: "Tên nhân viên", textAlign: "left", display: "table-cell" },
      { field: "winCount", header: "Số lượng cơ hội thắng", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "loseCount", header: "Số lượng cơ hội thua", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "undefinedCount", header: "Số lượng cơ hội kỳ sau", textAlign: "right", display: "table-cell", width: "200px", },
    ];
    this.selectedColListReportLeadFollowPic = this.colsListReportLeadFollowPic.filter((e) => e.field == "picCode" || e.field == "picName" || e.field == "winCount" || e.field == "loseCount" || e.field == "undefinedCount");

    this.colsListReportLeadFollowSource = [
      { field: "potentialSource", header: "Nguồn tiềm năng", textAlign: "left", display: "table-cell" },
      { field: "winCount", header: "Số lượng cơ hội thắng", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "loseCount", header: "Số lượng cơ hội thua", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "undefinedCount", header: "Số lượng cơ hội kỳ sau", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "sumAmount", header: "Doanh thu", textAlign: "right", display: "table-cell", width: "140px", },
    ];
    this.selectedColListReportLeadFollowSource = this.colsListReportLeadFollowSource.filter((e) => e.field == "potentialSource" || e.field == "winCount" || e.field == "loseCount" || e.field == "undefinedCount" || e.field == "sumAmount");

    this.colsListReportLeadFollowProvincial = [
      { field: "provincial", header: "Địa điểm", textAlign: "left", display: "table-cell" },
      { field: "winCount", header: "Số lượng cơ hội thắng", textAlign: "right", display: "table-cell", width: "180px", },
      { field: "loseCount", header: "Số lượng cơ hội thua", textAlign: "right", display: "table-cell", width: "180px", },
      { field: "undefinedCount", header: "Số lượng cơ hội kỳ sau", textAlign: "right", display: "table-cell", width: "200px", },
    ];
    this.selectedColListReportLeadFollowProvincial = this.colsListReportLeadFollowProvincial.filter((e) => e.field == "provincial" || e.field == "winCount" || e.field == "loseCount" || e.field == "undefinedCount");

    this.colsListReportLeadFollowMonth = [
      { field: "monthTime", header: "Tháng năm", textAlign: "left", display: "table-cell", width: "200px", },
      { field: "winCount", header: "Số lượng cơ hội thắng", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "loseCount", header: "Số lượng cơ hội thua", textAlign: "right", display: "table-cell", width: "200px", },
      { field: "undefinedCount", header: "Số lượng cơ hội kỳ sau", textAlign: "right", display: "table-cell", width: "200px", },
    ];
    this.selectedColListReportLeadFollowMonth = this.colsListReportLeadFollowMonth.filter((e) => e.field == "monthTime" || e.field == "winCount" || e.field == "loseCount" || e.field == "undefinedCount");
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  getMasterData() {
    this.loading = true;
    this.leadService.getMasterDataReportLead(this.auth.UserId).subscribe((response) => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listEmployee = result.listEmployee;
        this.listSource = result.listSource;
        this.listArea = result.listArea;
        this.listEmployee.forEach(item => {
          item.employeeName = item.employeeCode + '-' + item.employeeName;
        });
        this.searchReport(this.typeReport.Code);
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  searchReport(code: string) {
    let listEmployeeId: Array<string> = [];
    let listSourceId: Array<string> = [];
    let listGeographicalAreaId: Array<string> = [];
    if (this.lstEmployeNgModel != null && this.lstEmployeNgModel != undefined) {
      listEmployeeId = this.lstEmployeNgModel.map(c => c.employeeId);
    }
    if (this.lstSourceNgModel != null && this.lstSourceNgModel != undefined) {
      listSourceId = this.lstSourceNgModel.map(c => c.categoryId);
    }
    if (this.lstAreaNgModel != null && this.lstAreaNgModel != undefined) {
      listGeographicalAreaId = this.lstAreaNgModel.map(c => c.geographicalAreaId);
    }
    let timeParameter = new TimeSearchModel();
    timeParameter.code = this.selectedTypeStatus.Code;
    if (this.yearNgModel) {
      timeParameter.year = this.yearNgModel.Code;
    }
    if (this.fromDate) {
      this.fromDate.setHours(0, 0, 0, 0);
      this.fromDate.setDate(1);
      timeParameter.fromDate = convertToUTCTime(this.fromDate);
    }
    if (this.toDate) {
      this.toDate.setHours(23, 59, 59, 999);
      var dayInMonth = new Date(this.toDate.getFullYear(), this.fromDate.getMonth(), 0).getDate();
      this.toDate.setDate(dayInMonth);
      timeParameter.toDate = convertToUTCTime(this.toDate);
    }

    this.loading = true;
    this.leadService.reportLead(this.auth.UserId, code, listEmployeeId, listSourceId, listGeographicalAreaId, timeParameter).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        switch (code) {
          case 'AGE':
            this.listReportLeadAge = result.listReportLeadFollowAge;
            if (this.listReportLeadAge.length == 0) {
              this.notFoundMessage();
            }
            break;
          case 'PIC':
            this.listReportLeadPic = result.listReportLeadFollowPic;
            if (this.listReportLeadPic.length == 0) {
              this.notFoundMessage();
            }
            break;
          case 'SOURCE':
            this.listReportLeadSource = result.listReportLeadFollowSource;
            if (this.listReportLeadSource.length == 0) {
              this.notFoundMessage();
            }
            break;
          case 'ADDRESS':
            this.listReportLeadProvincial = result.listReportLeadFollowProvincial;
            if (this.listReportLeadProvincial.length == 0) {
              this.notFoundMessage();
            }
            break;
          case 'STATUS':
            debugger
            this.listReportLeadStatus = result.listReportLeadFollowMonth;
            if (this.listReportLeadStatus.length == 0) {
              this.notFoundMessage();
            }
            break;
        }
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  refreshFilter() {
    this.listReportLeadAge = [];
    this.listReportLeadPic = [];
    this.listReportLeadSource = [];
    this.listReportLeadProvincial = [];
    this.listReportLeadStatus = [];
    this.isGlobalFilter = '';

    this.yearNgModel = this.listYear.find(c => c.Code == new Date().getFullYear());
    this.fromDate = new Date();
    this.toDate = new Date();

    this.lstEmployeNgModel = [];
    this.lstAreaNgModel = [];
    this.lstSourceNgModel = [];
    // this.myTable.reset();

    this.searchReport(this.typeReport.Code);
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'quoteDate') {
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

  onChangeTypeReport(value: TypeReport) {
    if (value == null) return;
    this.searchReport(value.Code);
  }

  onChangeTypeStatus(value) {
  }

  setDataYear() {
    for (let i = this.startYear; i <= this.endYear; i++) {
      let year = new Year();
      year.Code = i;
      year.Name = i.toString();
      this.listYear.push(year);
    }
    this.yearNgModel = this.listYear.find(c => c.Code == new Date().getFullYear());
  }
  notFoundMessage() {
    this.clearToast();
    this.showToast('warn', 'Thông báo', "Không tìm thấy bản ghi nào");
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
