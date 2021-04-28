import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { QuoteService } from '../../services/quote.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { SortEvent } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { CategoryService } from '../../../shared/services/category.service';

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string
}

@Component({
  selector: 'app-customer-quote-list',
  templateUrl: './customer-quote-list.component.html',
  styleUrls: ['./customer-quote-list.component.css'],
  providers: [
    DatePipe
  ]
})
export class CustomerQuoteListComponent implements OnInit {
  innerWidth: number = 0; //number window size first
  
  @HostListener('window:resize', ['$event'])
  onResize(event){
    //if (this.innerWidth < )
  }

  emitParamUrl: any;
  @ViewChild('myTable') myTable: Table;

  loading: boolean = false;
  colsListQuote: any;
  first = 0;
  rows = 10;
  selectedColumns: any[];
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  today = new Date();
  minYear: number = 2015;
  currentYear: number = (new Date()).getFullYear();
  
  listStatus: Array<Category> = [];
  selectedStatus: Array<Category> = [];
  listQuote: Array<any> = [];
  quoteCode: string = '';
  quoteName: string = '';  
  startDate: Date = null;
  maxEndDate: Date = new Date();
  endDate: Date = null;
  listStatusQuote: Array<any> = [];
  isOutOfDate: boolean = false; //Báo giá quá hạn
  isCompleteInWeek: boolean = false; //Báo giá phải hoàn thành trong tuần
  isGlobalFilter: string = '';

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = "order/create";
  viewPermission: string = "order/view";
  /*End*/

  /*Action*/
  actionAdd: boolean = true;
  actionDownload: boolean = true;
  actionDelete: boolean = true;
  /*End*/
  
  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private quoteService: QuoteService,
    private messageService: MessageService,
    private datePipe: DatePipe,
    private confirmationService: ConfirmationService,
    private categoryService: CategoryService) { 
    this.innerWidth = window.innerWidth;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngOnInit() {
    //Check permission
    let resource = "crm/customer/quote-list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity:'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ'};
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
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
      this.declaireTable();

      /**Nếu Link từ Dashboard báo giá sang */
      this.emitParamUrl = this.route.params.subscribe(params => {
        if (params['mode']) {
          if (params['mode'] == 'OutOfDate') {
            this.isOutOfDate = true;
            this.changeCompleteInWeek();
            this.searchQuote();
          } else if (params['mode'] == 'InWeek') {
            this.isCompleteInWeek = true;
            this.changeOutOfDate();
            this.searchQuote();
          }
        } else {
          this.isOutOfDate = false;
          this.isCompleteInWeek = false;
          this.searchQuote();
        }
      });
      /**End */
    }
  }

  declaireTable() {
    this.colsListQuote = [
      { field: 'quoteCode', header: 'Mã báo giá', textAlign: 'left', display: 'table-cell' },
      { field: 'quoteName', header: 'Tên báo giá', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Đối tượng', textAlign: 'left', display: 'table-cell' },
      { field: 'quoteDate', header: 'Ngày báo giá', textAlign: 'right', display: 'table-cell' },
      { field: 'quoteStatusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'amount', header: 'Tổng giá trị', textAlign: 'right', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListQuote;
  }

  searchQuote() {
    this.loading = true;
    let startDate = this.startDate;
    if (startDate) {
      startDate.setHours(0, 0, 0, 0);
      startDate = convertToUTCTime(startDate);
    }
    
    let endDate = this.endDate;
    if (endDate) {
      endDate.setHours(23, 59, 59, 999);
      endDate = convertToUTCTime(endDate);
    }
    
    this.listStatusQuote = this.selectedStatus.map(x => x.categoryId);
    this.quoteService.searchQuote(this.quoteCode, startDate, endDate, 
                                  this.listStatusQuote, this.isOutOfDate, this.isCompleteInWeek, this.quoteName).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        if (this.listStatus.length == 0) {
          this.listStatus = result.listStatus;
        }
        this.listQuote = result.listQuote;
        this.isShowFilterLeft = false;
        this.leftColNumber = 12;
        this.rightColNumber = 0;
        if (this.listQuote.length == 0) {
          let mgs = { severity:'warn', summary: 'Thông báo:', detail: 'Không tìm thấy báo giá nào!'};
          this.showMessage(mgs);
        } else {
          this.listQuote.forEach(item => {
            item.quoteDate = this.datePipe.transform(item.quoteDate, 'dd/MM/yyyy');
          });
        }
      } else {
        let mgs = { severity:'error', summary: 'Thông báo:', detail: result.messageCode};
        this.showMessage(mgs);
      }
    });
  }

  resetTable() {
    if (this.myTable) {
      this.myTable.reset();
    }
  }

  // Refresh parameter search
  refreshFilter() {
    this.quoteCode = '';
    this.quoteName = '';
    this.startDate = null;
    this.endDate = null;
    this.selectedStatus = [];
    this.isOutOfDate = false;
    this.isCompleteInWeek = false;    
    this.isGlobalFilter = '';
    this.resetTable();

    this.searchQuote();
  }

  pageChange(event: any) {
  }

  leftColNumber: number = 12;
  rightColNumber: number = 2;
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

  checkExpirationDate(expirationDate: Date) {
    if (expirationDate != null) {
      var formatedExpirationDate = new Date(expirationDate);
      formatedExpirationDate.setHours(this.today.getHours(), this.today.getMinutes(), this.today.getMilliseconds());
      if (formatedExpirationDate < this.today) {
        return true;
      }
      else {
        return false;
      }
    } else return false;
  }

  /**Xóa báo giá */
  del_quote(quoteId: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.quoteService.updateActiveQuote(quoteId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            let mgs = { severity:'success', summary: 'Thông báo:', detail: result.messageCode};
            this.showMessage(mgs);
            this.searchQuote();
          } else {
            let mgs = { severity:'error', summary: 'Thông báo:', detail: result.messageCode};
            this.showMessage(mgs);
          }
        }, error => { });
      }
    });
  }

  dateFieldFormat:string = 'DD/MM/YYYY';
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

  changeOutOfDate() {
    if (this.isOutOfDate) {
      this.selectedStatus = this.listStatus.filter(item => item.categoryCode == 'MTA' || item.categoryCode == 'CHO' || item.categoryCode == 'DLY');
      this.isCompleteInWeek = false;
    }
  }

  changeCompleteInWeek() {
    if (this.isCompleteInWeek) {
      this.selectedStatus = this.listStatus.filter(item => item.categoryCode == 'MTA' || item.categoryCode == 'CHO' || item.categoryCode == 'DLY');
      this.isOutOfDate = false;
    }
  }

  setDafaultStartDate(): Date {
    let date = new Date();
    date.setDate(1);
    
    return date;
  }

  goToCreateQuote() {
    this.router.navigate(['/customer/quote-create']);
  }

  goToDetail(id: string) {
    this.router.navigate(['/customer/quote-detail', { quoteId: id }]);
  }

  ngOnDestroy() {
    if (this.emitParamUrl) {
      this.emitParamUrl.unsubscribe();
    }
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};