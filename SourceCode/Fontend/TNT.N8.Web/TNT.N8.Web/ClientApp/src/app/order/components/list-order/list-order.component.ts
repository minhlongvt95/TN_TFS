import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { CustomerOrderService } from '../../services/customer-order.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DecimalPipe } from '@angular/common';

interface OrderStatus {
  orderStatusId: string,
  orderStatusCode: string,
  description: string
}

interface Vat {
  value: number,
  name: string
}

interface Order {
  orderId: string,
  orderCode: string,
  orderDate: string,
  seller: string,
  sellerName: string,
  description: string,
  note: string,
  customerId: string,
  customerName: string,
  customerContactId: string,
  paymentMethod: string,
  daysAreOwed: number,
  maxDebt: number,
  receivedDate: Date,
  receivedHour: Date,
  recipientName: string,
  locationOfShipment: string,
  shippingNote: string,
  recipientPhone: string,
  recipientEmail: string,
  placeOfDelivery: string,
  amount: number,
  discountType: boolean,
  discountValue: number,
  statusId: string,
  statusCode: string,
  orderStatusName: string,
  sellerFirstName: string,
  sellerLastName: string,
  listOrderDetail: string,
  backgroundColorForStatus: string,
  canDelete: boolean
}

@Component({
  selector: 'app-list-order',
  templateUrl: './list-order.component.html',
  styleUrls: ['./list-order.component.css'],
  providers: [
    DecimalPipe,
    DatePipe
  ]
})
export class ListOrderComponent implements OnInit {
  // @HostListener('document:keyup', ['$event'])
  // handleDeleteKeyboardEvent(event: KeyboardEvent) {
  //   if (event.key === 'Enter') {
  //     this.getAllCustomerOrder();
  //   }
  // }

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  loading: boolean = false;
  innerWidth: number = 0; //number window size first

  paramUrl : any;
  listOrderStatus: Array<OrderStatus> = [];
  listSelectedOrderStatus: Array<OrderStatus> = [];
  orderCode: string = '';
  customerName: string = '';
  phone: string = '';
  fromDate: Date = null;
  toDate: Date = null;
  listVat: Array<Vat> = [
    {
      value: 1,
      name: 'Tất cả'
    },
    {
      value: 2,
      name: 'Có hóa đơn VAT'
    },
    {
      value: 3,
      name: 'Không có hóa đơn VAT'
    }
  ];
  listProduct: Array<any> = [];
  listQuote: Array<any> = [];
  listContract: Array<any> = [];
  selectedVat: Vat = { value: 1, name: 'Tất cả' };
  selectedProduct: any;
  selectedQuote: any;
  selectedContract: any;
  listOrder: Array<Order> = [];
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxStartDate: Date = new Date();
  maxEndDate: Date = new Date();
  contractId: any;
  isGlobalFilter: string = '';

  @ViewChild('myTable') myTable: Table;
  colsListOrder: any;
  selectedColumns: any[];

  public Seller: string = '';   //id nhân viên bán hàng nhận từ dashboard bán hàng

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private customerOrderService: CustomerOrderService,
    private messageService: MessageService,
    private decimalPipe: DecimalPipe,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "sal/order/list";
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

      this.paramUrl = this.route.params.subscribe(params =>{
          this.contractId = params['contractId'];;
      });
      //Nhận params từ dashboard bán hàng
      // let ttt = this.route.params.subscribe(params => {
      //   let isEmptyObj = this.isEmpty(params);
      //   if (!isEmptyObj) {
      //     this.OrderDateStart = new Date(params['startDate']),
      //       this.OrderDateEnd = new Date(params['endDate']),
      //       this.Seller = params['employeeId'] || ''
      //   }
      // });

      // if (this.OrderDateStart) {
      //   let item = [];
      //   let DLV = this.orderStatuss.find(x => x.orderStatusCode == "DLV").orderStatusId;
      //   let PD = this.orderStatuss.find(x => x.orderStatusCode == "PD").orderStatusId;
      //   let IP = this.orderStatuss.find(x => x.orderStatusCode == "IP").orderStatusId;
      //   let COMP = this.orderStatuss.find(x => x.orderStatusCode == "COMP").orderStatusId;

      //   item.push(DLV);
      //   item.push(PD);
      //   item.push(IP);
      //   item.push(COMP);

      //   this.selectedOrderStatus = item;
      // }

      this.initTable();

      this.customerOrderService.getMasterDataOrderSearch().subscribe(response => {
        let result: any = response;
        this.loading = true;
        if (result.statusCode == 200) {
          this.listOrderStatus = result.listOrderStatus;
          this.listProduct = result.listProduct;
          this.listQuote = result.listQuote;
          this.listContract = result.listContract;
          this.searchOrder();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  initTable() {
    this.colsListOrder = [
      { field: 'orderCode', header: 'Mã', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'orderDate', header: 'Ngày đặt hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'orderStatusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'amount', header: 'Tổng giá trị', textAlign: 'right', display: 'table-cell' },
      { field: 'sellerName', header: 'Nhân viên bán hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'listOrderDetail', header: 'Chi tiết', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListOrder;
  }

  searchOrder() {
    let listStatusId: Array<string> = [];
    listStatusId = this.listSelectedOrderStatus.map(x => x.orderStatusId);

    let vat: number = 1;
    if (this.selectedVat) {
      vat = this.selectedVat.value;
    }
    let productId: string = null;
    if(this.selectedProduct !== null && this.selectedProduct !== undefined){
      productId = this.selectedProduct.productId;
    }
    let quoteId: string = null;
    if(this.selectedQuote !== null && this.selectedQuote !== undefined){
      quoteId = this.selectedQuote.quoteId;
    }
  
    let contractId: string = null;
    if(this.selectedContract !== null && this.selectedContract !== undefined){
      contractId = this.selectedContract.contractId;
      this.contractId = null;
    }else{
      contractId = this.contractId;
    }

    let fromDate = null;
    if (this.fromDate) {
      this.fromDate.setHours(0, 0, 0, 0);
      fromDate = convertToUTCTime(this.fromDate);
    }

    let toDate = null;
    if (this.toDate) {
      this.toDate.setHours(23, 59, 59, 999);
      toDate = convertToUTCTime(this.toDate);
    }

    this.loading = true;
    this.customerOrderService.searchOrder(this.orderCode, this.customerName, listStatusId, this.phone,
      fromDate, toDate, vat, productId, quoteId, contractId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        
        if (result.statusCode == 200) {
          this.listOrder = result.listOrder;

          // this.isShowFilterLeft = false;
          // this.leftColNumber = 12;
          // this.rightColNumber = 0;
          if (this.listOrder.length == 0) {
            let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không tìm thấy đơn hàng nào!' };
            this.showMessage(msg);
          } else {
            
            this.handleBackGroundColorForStatus();
            this.listOrder.forEach(item => {
              item.orderDate = this.datePipe.transform(item.orderDate, 'dd/MM/yyyy');
            });
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }
  exportExcel(){
    let pdfOrder = this.listOrder;
    let dateUTC = new Date();
    // getMonth() trả về index trong mảng nên cần cộng thêm 1
    let title = "Danh sách đơn hàng" + dateUTC.getDate() + '_' + (dateUTC.getMonth() + 1) + '_' + dateUTC.getUTCFullYear();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(title);
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    /* Header row */
    let dataHeaderRow = ['Mã', '', '', 'Tên khách hàng', '', '', '', 'Ngày đặt hàng', '', 'Trạng thái', '', 'Tổng giá trị', '', 'Nhân viên bán hàng', '', 'Chi tiết', ''];
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

    pdfOrder.forEach(item => {          
      let row: Array<any> = [];
      row[0] = item.orderCode;
      row[3] = item.customerName;
      row[7] = item.orderDate;
      row[9] = item.orderStatusName;
      row[11] = this.decimalPipe.transform(item.amount).toString(); // item.amount;
      row[13] = item.sellerName;
      row[15] = item.listOrderDetail;

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

  handleBackGroundColorForStatus() {
    this.listOrder.forEach(lead => {
      switch (lead.statusCode) {
        case "RTN"://bi tra lai
          lead.backgroundColorForStatus = "#BB0000";
          break;
        case "COMP"://da dong
          lead.backgroundColorForStatus = "#6D98E7";
          break;
        case "DLV"://da giao hang
          lead.backgroundColorForStatus = "#66CC00";
          break;
        case "PD"://da thanh toan
          lead.backgroundColorForStatus = "#9C00FF";
          break;
        case "IP"://dang xu ly
          lead.backgroundColorForStatus = "#34c759";
          break;
        case "ON"://hoan
          lead.backgroundColorForStatus = "#666666";
          break;
        default:
          lead.backgroundColorForStatus = "#ffcc00";
          break;
      }
    });
  }

  refreshFilter() {
    this.orderCode = '';
    this.customerName = '';
    this.phone = '';
    this.fromDate = null;
    this.toDate = null;
    this.listSelectedOrderStatus = [];
    this.selectedVat = { value: 1, name: 'Tất cả' };
    this.selectedProduct = null;
    this.selectedQuote = null;
    this.selectedContract = null;
    this.isGlobalFilter = '';
    this.resetTable();

    this.searchOrder();
  }

  resetTable() {
    if (this.myTable) {
      this.myTable.reset();
    }
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  goToCreateOrder() {
    this.router.navigate(['/order/create']);
  }

  goToOrderDetail(orderId: string) {
    this.router.navigate(['/order/order-detail', { customerOrderID: orderId }]);
  }

  goToCustomerDetail(customerId: string) {
    this.router.navigate(['/customer/detail', { customerId: customerId }]);
  }

  //Kiểm tra xem 1 object có null hay không
  isEmpty(obj) {
    for (var key in obj) {
      if (obj.hasOwnProperty(key))
        return false;
    }
    return true;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  deleteOrder(rowData: Order) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.customerOrderService.deleteOrder(rowData.orderId).subscribe(response => {
          this.loading = false;
          const result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.listOrder = this.listOrder.filter(e => e != rowData);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa đơn hàng thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
