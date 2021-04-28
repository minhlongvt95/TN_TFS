import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService, SortEvent, Table } from 'primeng';
import { GetPermission } from '../../../shared/permission/get-permission';
import { contactModel, ContactModel } from '../../../shared/models/contact.model';
import { CustomerCareService } from '../../services/customer-care.service';
import { CustomerService } from '../../services/customer.service';
import moment from "moment";
 
@Component({
  selector: 'app-list-customer-contact',
  templateUrl: './list-customer-contact.component.html',
  styleUrls: ['./list-customer-contact.component.css']
})
export class ListCustomerContactComponent implements OnInit {
  @ViewChild('table') table: Table;
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  auth: any = JSON.parse(localStorage.getItem("auth"));
  filterGlobal: string;
  innerWidth: number = 0;
  loading: boolean = false;
  isGlobalFilter: string = '';
  /*TABLE*/
  rows = 10;
  selectedColumns: any[];
  cols: any[];
  /*-*/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  lstContactCustomer: Array<contactModel> = [];
  /*-*/

  selectedContactCus: Array<any> = [];

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private customerService: CustomerService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
  ) {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    let resource = "crm/customer/list-contact-customer";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity:'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ'};
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      this.initTable();
      this.searchContactCustomer();
    }
  }

  initTable() {
    this.cols = [
      { field: 'fullName', header: 'Tên người liên hệ', textAlign: 'left', display: 'table-cell' },
      { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'right', display: 'table-cell', width: '140px' },
      { field: 'email', header: 'Email', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'role', header: 'Chức vụ', textAlign: 'left', display: 'table-cell', width: '200px' },
    ];
    this.selectedColumns = this.cols;
  }

  searchContactCustomer() {
    this.loading = true;
    this.customerService.searchContactCustomer(this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.lstContactCustomer = result.listContact;
        this.lstContactCustomer.forEach(x=> {
          x.fullName = x.firstName + ' ' + x.lastName;
        })
        if (this.lstContactCustomer.length == 0) {
          this.clear();
          let mgs = { severity: 'warn', summary: 'Thông báo:', detail: "Không tìm thấy bản ghi nào!" };
          this.showMessage(mgs);
        }
      } else {
        this.clear();
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
      }
    });

  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];
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

  refreshFilter() {
    this.lstContactCustomer = [];
    this.isGlobalFilter = '';
    this.searchContactCustomer();
  }

  goToDetail(rowData: contactModel) {
    //Khách hàng
    if (rowData.statusCustomer == "HDO") 
    {
      this.router.navigate(['/customer/detail', { customerId: rowData.objectId }]);
    }
    //Khách hàng tiềm năng
    else if (rowData.statusCustomer == "MOI") 
    {
      this.router.navigate(['/customer/potential-customer-detail', { customerId: rowData.objectId }]);
    }
  }
}
