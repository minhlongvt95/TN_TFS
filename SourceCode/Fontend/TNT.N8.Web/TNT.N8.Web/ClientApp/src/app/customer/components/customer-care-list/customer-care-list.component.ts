import { Component, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe } from '@angular/common';
//MODELS
import { CustomerCareModel2 } from '../../models/customer-care.model';

//SEVICES
import { CustomerCareService } from '../../services/customer-care.service';

//DIALOG
import { GetPermission } from '../../../shared/permission/get-permission';
import { ConfirmationService, MessageService, SortEvent, Table } from 'primeng';

@Component({
  selector: 'app-customer-care-list',
  templateUrl: './customer-care-list.component.html',
  styleUrls: ['./customer-care-list.component.css'],
  providers: [
    DatePipe
  ]
})
export class CustomerCareListComponent implements OnInit {
  @ViewChild('table') table: Table;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth: any = JSON.parse(localStorage.getItem("auth"));
  filterGlobal: string;
  innerWidth: number = 0;
  loading: boolean = false;

  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  selectedCustomercareIdList: Array<string> = [];

  /*TABLE*/
  rows = 10;
  selectedColumns: any[];
  cols: any[];
  /*-*/

  startDate: Date = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  endDate: Date = new Date();
  maxEndDate: Date = new Date();

  /*ĐIỀU KIỆN SEARCH*/
  customerCareCode: string = '';
  fromDate: Date = null;
  toDate: Date = null;
  title: string;
  lstPic: Array<any> = [];
  lstStatus: Array<any> = [];
  content: string;
  lstProgramType: Array<number> = [];
  lstTypeCusCare: Array<any> = [];
  /*END*/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listEmployeeCharge: Array<any> = [];
  lstCustomerCare: Array<CustomerCareModel2> = [];
  listStatus: Array<any> = [];
  listTypeCusCare: Array<any> = [];
  /*-*/
  selectedCustomerCare: Array<any> = [];
  /* ACTION*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionEdit: boolean = true;
  /*END*/

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private customerCareService: CustomerCareService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
  ) {
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    //Check permission
    let resource = "crm/customer/care-list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
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
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }

      this.initTable();
      await this.getMasterData();
      this.searchCustomerCare();
    }
  }

  initTable() {
    this.cols = [
      { field: 'customerCareCode', header: 'Mã CTKH', textAlign: 'left', display: 'table-cell' },
      { field: 'customerCareTitle', header: 'Tiêu đề', textAlign: 'left', display: 'table-cell', width: '270px' },
      { field: 'customerCareContactTypeName', header: 'Hình thức', textAlign: 'left', display: 'table-cell', width: '140px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'employeeChargeName', header: 'Người phụ trách', textAlign: 'center', display: 'table-cell' },
      { field: 'effecttiveFromDate', header: 'Ngày bắt đầu', textAlign: 'right', display: 'table-cell' },
      { field: 'effecttiveToDate', header: 'Ngày kết thúc', textAlign: 'right', display: 'table-cell' },
    ];
    this.selectedColumns = this.cols;
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.customerCareService.getMasterDataCustomerCareListAsync(this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listEmployeeCharge = result.listEmployee;
      this.listStatus = result.listStatus;
      this.listTypeCusCare = result.listFormCusCare;
      if (this.listStatus) {
        this.lstStatus = this.listStatus.filter(c => c.categoryCode == "Active" || c.categoryCode == "New");
      }
      if (this.listEmployeeCharge) {
        this.listEmployeeCharge.forEach(item => {
          item.employeeName = item.employeeCode + '-' + item.employeeName;
        });
      }
    } else {
      this.clear();
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(mgs);
    }
  }

  refreshFilter() {
    this.title = '';
    this.content = '';
    this.lstPic = [];
    this.lstStatus = [];
    this.fromDate = null;
    this.toDate = null;
    this.customerCareCode = '';
    this.lstTypeCusCare = [];
    this.filterGlobal = '';
    this.table.reset();

    if (this.listStatus) {
      this.lstStatus = this.listStatus.filter(c => c.categoryCode == "Active" || c.categoryCode == "New");
    }
    this.searchCustomerCare();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
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

  searchCustomerCare() {
    let startDate = this.fromDate;
    if (startDate) {
      startDate.setHours(0, 0, 0, 0);
      startDate = convertToUTCTime(startDate);
    }

    let endDate = this.toDate;
    if (endDate) {
      endDate.setHours(23, 59, 59, 999);
      endDate = convertToUTCTime(endDate);
    }
    let lstStatusId: Array<string> = [];
    let lstPicId: Array<string> = [];
    let lstTypeCusCareId: Array<string> = [];
    if (this.lstStatus) {
      lstStatusId = this.lstStatus.map(c => c.categoryId);
    }
    if (this.lstPic) {
      lstPicId = this.lstPic.map(c => c.employeeId);
    }
    if(this.lstTypeCusCare){
      lstTypeCusCareId = this.lstTypeCusCare.map(c => c.categoryId);
    }
    this.title = this.title ? this.title.trim() : this.title;
    this.content = this.content ? this.content.trim() : this.content;
    this.customerCareCode = this.customerCareCode ? this.customerCareCode.trim() : this.customerCareCode;

    this.loading = true;
    this.customerCareService.searchCustomerCare(startDate, endDate, this.title, lstPicId, lstStatusId, this.content, this.lstProgramType, this.auth.UserId, this.customerCareCode, lstTypeCusCareId).subscribe(response => {
      this.loading = false;
      let result = <any>response;
      if (result.statusCode == 200) {
        this.lstCustomerCare = result.lstCustomerCare;
        if (this.lstCustomerCare.length === 0) {
          this.clear();
          let mgs = { severity: 'warn', summary: 'Thông báo:', detail: "Không tìm thấy bản ghi nào!" };
          this.showMessage(mgs);
        }
        else {
          //Khởi tạo isEditStatus dùng để check trạng thái chương trình khi thay đổi
          this.lstCustomerCare.forEach((item, index) => {
            let i = index + 1;
            item.index = i++;
            item.status = this.listStatus.find(c => c.categoryId == item.statusId);
            switch (item.statusCode) {
              case 'New':
                item.listStatus = this.listStatus.filter(c => c.categoryCode != "New");
                break;
              case 'Active':
                item.listStatus = this.listStatus.filter(c => c.categoryCode != "New" && c.categoryCode != "Active");
                break;
              case 'Stoped':
                item.listStatus = this.listStatus.filter(c => c.categoryCode != "New" && c.categoryCode != "Stoped");
              case 'Đóng':
                item.listStatus = this.listStatus.filter(c => c.categoryCode == "Closed");
            }
            if (item.statusCode == 'Closed') {
              item.isEditStatus = false;
            } else {
              item.isEditStatus = true;
            }
          })
        }
      } else {
        this.clear();
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    }, error => { });
  }


  leftColNumber: number = 12;
  rightColNumber: number = 0;
  showFilter() {
    if (this.innerWidth < 768) {
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

  confirmChangeCustomerCareStatus(value: any, rowData: any) {
    if (value == null) {
      return;
    }
    let contentMess = "Bạn có chắc chắn muốn " + value.categoryName + " chương trình CSKH?";
    this.confirmationService.confirm({
      message: contentMess,
      accept: () => {
        this.changedCustomerCareStatus(rowData.customerCareId, value.categoryId);
      }
    });
  }

  async changedCustomerCareStatus(customerCareId: string, statusId: string) {
    this.loading = true;
    let result: any = await this.customerCareService.updateStatusCusCare(this.auth.UserId, customerCareId, statusId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.clear();
      let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Thay đổi trạng thái thành công" };
      this.showMessage(mgs);
    } else {
      this.clear();
      let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(mgs);
    }
    this.searchCustomerCare();
  }

  changeProgramType(event) {
    if (event.isUserInput) {
      if (event.source.selected) {
        this.lstProgramType = [];
        this.lstProgramType.push(event.source.value);
      }
    }
  }

  onViewDetail(customerCareId) {
    this.router.navigate(['/customer/care-detail', { CustomerCareId: customerCareId }]);
  }
  selectedRowIndex: string = '';

  highlight(row) {
    this.selectedRowIndex = row.CustomerCareId;
  }

  goToCreate() {
    this.router.navigate(['/customer/care-create']);
  }

  exportExcel(){
    let cskh = this.selectedCustomerCare;
    let dateUTC = new Date();
    // getMonth() trả về index trong mảng nên cần cộng thêm 1
    let title = "Danh sách CSKH_" + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(title);
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    /* Header row */
    let dataHeaderRow = ['Mã CSKH', '', '', 'Tiêu đề', '', '', '', 'Hình thức', '', 'Trạng thái', '', 'Người phụ trách', '', 'Ngày bắt đầu', '', 'Ngày kết thúc', ''];
    let headerRow = worksheet.addRow(dataHeaderRow);
    worksheet.mergeCells(`A${headerRow.number}:C${headerRow.number}`);
    worksheet.mergeCells(`D${headerRow.number}:G${headerRow.number}`);
    worksheet.mergeCells(`H${headerRow.number}:I${headerRow.number}`);
    worksheet.mergeCells(`J${headerRow.number}:K${headerRow.number}`);
    worksheet.mergeCells(`L${headerRow.number}:M${headerRow.number}`);
    worksheet.mergeCells(`N${headerRow.number}:O${headerRow.number}`);
    worksheet.mergeCells(`P${headerRow.number}:Q${headerRow.number}`);
    headerRow.font = { name: 'Times New Roman', size: 10, bold: true };
    dataHeaderRow.forEach((item, index) => {
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '8DB4E2' }
      };
    });
    headerRow.height = 35;

    /* Data table */
    let data: Array<any> = []; //[1, 'Dịch vụ CNTT', 'Gói', '2', '6.000.000', '12.000.000']

    cskh.forEach(item => {
      let row: Array<any> = [];
      row[0] = item.customerCareCode;
      row[3] = item.customerCareTitle;
      row[7] = item.customerCareContactTypeName;
      row[9] = item.statusName;
      row[11] = item.employeeChargeName;
      row[13] = this.datePipe.transform(item.effecttiveFromDate, 'dd/MM/yyyy');
      row[15] = this.datePipe.transform(item.effecttiveToDate, 'dd/MM/yyyy');
      data.push(row);
    });

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      worksheet.mergeCells(`A${row.number}:C${row.number}`);
      worksheet.mergeCells(`D${row.number}:G${row.number}`);
      worksheet.mergeCells(`H${row.number}:I${row.number}`);
      worksheet.mergeCells(`J${row.number}:K${row.number}`);
      worksheet.mergeCells(`L${row.number}:M${row.number}`);
      worksheet.mergeCells(`N${row.number}:O${row.number}`);
      worksheet.mergeCells(`P${row.number}:Q${row.number}`);

      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };


      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(7).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(7).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(8).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(8).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(9).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(9).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(10).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(10).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      row.getCell(11).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(11).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };

      row.getCell(12).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(12).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };
      row.getCell(13).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(13).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(14).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(14).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(15).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(15).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(16).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(16).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      row.getCell(17).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(17).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
    });

    this.exportToExel(workbook, title);
  }

  exportToExel(workbook: Workbook, fileName: string) {
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, fileName);
    })
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
