import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { Router } from '@angular/router';
//service
import { WarehouseService } from "../../services/warehouse.service";
import { GetPermission } from '../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import 'moment/locale/pt-br';
import { HubConnection, HubConnectionBuilder, HttpTransportType } from '@aspnet/signalr';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-in-stock-report',
  templateUrl: './in-stock-report.component.html',
  styleUrls: ['./in-stock-report.component.css'],
  providers: [
    {
      provide: HubConnection, useClass: HubConnectionBuilder
    }
  ]
})
export class InStockReportComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  @HostListener('window:resize', ['$event'])
  onResize(event) {
    //if (this.innerWidth < )
  }
  @ViewChild('myTable') myTable: Table;
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  cols: any[];
  rows = 10;
  leftColNumber: number = 12;
  rightColNumber: number = 2;

  listProductCategory: Array<any> = [];
  selectedProductCategory: any = null;

  listWareHouse: Array<any> = [];
  selectedWareHouse: any = null;

  listResult: Array<any> = [];
  SumRow: number = 0;
  currentDate: Date = new Date();

  currentYear = this.currentDate.getFullYear();
  currentMonth = this.currentDate.getMonth() + 1;
  listYear = this.setListYear();
  selectedYear = this.listYear.find(x => x.value == this.currentYear);
  listMonth = [
    {
      value: 1
    },
    {
      value: 2
    },
    {
      value: 3
    },
    {
      value: 4
    },
    {
      value: 5
    },
    {
      value: 6
    },
    {
      value: 7
    },
    {
      value: 8
    },
    {
      value: 9
    },
    {
      value: 10
    },
    {
      value: 11
    },
    {
      value: 12
    },
  ];
  selectedMonth = this.listMonth.find(x => x.value == this.currentMonth);
  selectedDay: Date = new Date();
  productNameCode: string = null;

  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  messages: string[] = [];
  message: string;
  notifications: any[] = [];

  constructor(
    private warehouseService: WarehouseService,
    private getPermission: GetPermission,
    public dialogService: DialogService,
    public messageService: MessageService,
    private router: Router,
    private hubConnection: HubConnection,
    private notificationService: NotificationService
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "war/warehouse/inventory-delivery-voucher/list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    } else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      this.initTable();
      this.getDay();
      this.getMasterData();

      // let url = localStorage.getItem('ApiEndPoint') + '/notification';
      // this.hubConnection = new HubConnectionBuilder()
      // .withUrl(url, {}) 
      // .build();

      // this.hubConnection.on("ReceiveNotifications", (listNotifications) => {
      //   this.notifications = listNotifications;
      // });

      // this.hubConnection
      //   .start()
      //   .then(() => console.log('Connection started'))
      //   .catch(err => console.log('Error while starting connection: ' + err));
    }
  }

  /* Test connect real time */
  // send() {
  //   // // message sent from the client to the server
  //   // this.hubConnection.invoke("Echo", this.message);
  //   // this.message = "";
  //   this.notificationService.getNotification("EA944DC1-DD48-49FC-9A24-4B88B9A8D33A", 
  //   "00000000-0000-0000-0000-000000000000").subscribe(response => {
  //     let result: any = response;
  //   });
  // }

  initTable() {
    this.cols = [
      { field: 'productCode', header: 'Mã vật tư', width: '90px', textAlign: 'left' },
      { field: 'productName', header: 'Tên vật tư', width: '150px', textAlign: 'left' },
      { field: 'productUnitName', header: 'Đơn vị tính', width: '50px', textAlign: 'left' },
      { field: 'quantityInStock', header: 'Số lượng', width: '40px', textAlign: 'right' },
      { field: 'productPrice', header: 'Giá bán', width: '60px', textAlign: 'right' },
    ];
  }

  getMasterData() {
    this.loading = true;
    this.warehouseService.getMasterDataSearchInStockReport().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listProductCategory = result.listProductCategory;
        this.listWareHouse = result.listWareHouse;

        if (this.listWareHouse?.length > 0) {
          this.selectedWareHouse = this.listWareHouse[0];
        }

        this.findInstock(true);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  findInstock(first: boolean) {
    this.isShowFilterLeft = false;
    this.leftColNumber = 12;
    this.rightColNumber = 0;

    let selectedDay = convertToUTCTime(this.selectedDay);
    let productNameCode = this.productNameCode?.trim() ?? "";
    let productCategoryId = this.selectedProductCategory?.productCategoryId;
    let warehouseId = this.selectedWareHouse?.warehouseId;

    if (first == false) {
      this.loading = true;
    }

    this.warehouseService.searchInStockReport(selectedDay, productNameCode, productCategoryId, warehouseId)
    .subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listResult = result.listResult;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showFilter() {
    if (this.innerWidth < 1000) {
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

  refreshFilter() {
    this.selectedYear = this.listYear.find(x => x.value == this.currentYear);
    this.selectedMonth = this.listMonth.find(x => x.value == this.currentMonth);
    this.getDay();
    this.productNameCode = null;
    this.selectedProductCategory = null;
    if (this.listWareHouse.length > 0)  this.selectedWareHouse = this.listWareHouse[0];

    this.findInstock(false);
  }

  changeYear(event: any) {
    this.getDay();
  }

  changeMonth(event: any) {
    this.getDay();
  }

  // onKeydown($event: KeyboardEvent) {
  //   if ($event.key === 'Enter') {
  //     this.findInstock();
  //   }
  // }

  /*Tính ngày khi thay đổi Năm hoặc Tháng*/
  getDay() {
    let year = this.selectedYear.value;
    let month = this.selectedMonth.value;

    //Lấy ra ngày cuối cùng của tháng đang được chọn
    if (month == 12) {
      //Ngày đầu tiên của tháng tiếp theo
      let firstDay = new Date(year + 1, 0, 1, 0, 0, 0, 0);
      let time_first_day = firstDay.getTime();
      let time_last_day = time_first_day - 24 * 60 * 60 * 1000;
      let lastDay = new Date(time_last_day);
      //Vậy ngày cuối của tháng 
      this.selectedDay = lastDay;
    } else {
      //Ngày đầu tiên của tháng tiếp theo
      let firstDay = new Date(year, month, 1, 0, 0, 0, 0);
      let time_first_day = firstDay.getTime();
      let time_last_day = time_first_day - 24 * 60 * 60 * 1000;
      let lastDay = new Date(time_last_day);
      //Vậy ngày cuối của tháng 
      this.selectedDay = lastDay;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  setListYear() {
    let result = [];
    let length = this.currentYear - 2000;
    for (let i = 0; i <= length; i++) {
      let year = {
        value: 2000 + i
      };

      result = [...result, year];
    }

    return result;
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};