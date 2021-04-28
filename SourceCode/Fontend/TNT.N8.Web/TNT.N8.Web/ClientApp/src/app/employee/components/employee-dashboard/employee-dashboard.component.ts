import { Component, OnInit } from '@angular/core';

import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { animate, state, style, transition, trigger } from '@angular/animations';
import { EmployeeService } from '../../services/employee.service';
import * as $ from 'jquery';
import { Router } from '@angular/router';
import { ListEmployeeRequestService } from '../../services/employee-request/list-employee-request.service';
import { EmployeeAllowanceService } from '../../services/employee-allowance/employee-allowance.service';
import { EmployeeAllowanceModel } from '../../models/employee-allowance.model';
import { GetPermission } from '../../../shared/permission/get-permission';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';

@Component({
  selector: 'app-employee-dashboard',
  templateUrl: './employee-dashboard.component.html',
  styleUrls: ['./employee-dashboard.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', visibility: 'hidden' })),
      state('expanded', style({ height: '*', visibility: 'visible' })),
      transition('expanded <=> collapsed', animate('0ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class EmployeeDashboardComponent implements OnInit {

  dataSource: MatTableDataSource<any>;
  displayedColumns = ['expand','name','code','time','monday','tuesday','wednesday','thursday','friday','saturday','sunday'];
  expandedElement = [];
  listEmpRequestEmpId = [];
  listEmpRqInsideWeek = [];
  listEmpNearestBirthday = [];
  listEmpEndContract = [];
  isManager: boolean;
  lessthan40 = false;

  firstDayOfWeek = new Date(new Date().setDate(new Date().getDate() - new Date().getDay()+1));
  lastDayOfWeek = new Date(new Date().setDate(new Date().getDate() - new Date().getDay()+7));

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  dayOfWeek = [];
  keyname = '';
  currentKey = '';
  amountAbsentWithoutPermission: any;
  listEmployeeRequest = [];
  amountAbsentWithPermission: any;
  employeeAllowance = new EmployeeAllowanceModel();
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;

  constructor(
    private empService: EmployeeService,
    private getPermission: GetPermission,
    public snackBar: MatSnackBar,
    private employeeRequestService: ListEmployeeRequestService,
    private employeeAllowanceService: EmployeeAllowanceService,
    private router: Router
  ) { }

  async ngOnInit() {
    let resource = "hrm/employee/dashboard/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      await this.getData();
      await this.searchEmployeeRequest();
    }
  }
  // Thao tác trên bảng
  isExtendedRow = (index, item) => (item.employeeCode == "ORG" && item.employeeName == "");
  expandAll(event){
    let icon = $(event.target);
    const text = icon.text();
    if (text.trim() === 'arrow_drop_down'){
      this.expandedElement = [];
      for(let i=0;i<$('.arrowElement').length;i++){
        $($('.arrowElement')[i]).text('arrow_right');
      }
    } else {
      this.expandedElement = this.dataSource.data.filter(item => item.employeeCode == "ORG" && item.employeeName == "").map(m => m.organizationId);
      for(let i=0;i<$('.arrowElement').length;i++){
        $($('.arrowElement')[i]).text('arrow_drop_down');
      }
    }
  }
  expandEle(org){
    if (this.expandedElement.includes(org)){
      this.expandedElement.splice(this.expandedElement.indexOf(org),1);
    } else this.expandedElement.push(org);
    if (this.expandedElement.length == this.dataSource.data.filter(item => item.employeeCode == "ORG" && item.employeeName == "").length){
      $('.arrowAll').text('arrow_drop_down')
    }
    if (this.expandedElement.length == 0){
      $('.arrowAll').text('arrow_right')
    }
  }
  paddingLeft(level){
    return (parseInt(level)*15).toString()+"px";
  }
  toggleCollapseIcon(event){
    let icon = $(event.target);
    const text = icon.text();
    icon.text(text.trim() === 'arrow_drop_down' ? 'arrow_right' : 'arrow_drop_down');
  }
  onViewDetail(employeeId, contactId) {
    this.router.navigate(['/employee/detail', { employeeId: employeeId, contactId: contactId }]);
  }
  // Kết thúc
  async getData(){
    this.currentKey = this.keyname;
    await this.empService.getStatisticForEmpDashBoard(this.firstDayOfWeek,this.lastDayOfWeek,this.keyname,this.auth.UserId).subscribe(response => {
      const result = <any>response;
      this.dataSource = new MatTableDataSource(result.listEmployee);
      this.listEmpRqInsideWeek = result.listRequestInsideWeek;
      this.listEmpRequestEmpId = result.listRequestInsideWeek.map(r => r.offerEmployeeId);
      this.listEmpNearestBirthday = result.listEmpNearestBirthday.slice(0,5);
      this.listEmpEndContract = result.listEmpEndContract.slice(0,5);
      this.isManager = result.isManager;
      if (result.listEmployee.filter(item => item.employeeCode != "ORG" || item.employeeName != "").length < 40){
        this.expandedElement = this.dataSource.data.filter(item => item.employeeCode == "ORG" && item.employeeName == "").map(m => m.organizationId);
        this.lessthan40 = true;
        $('.arrowAll').text('arrow_drop_down');
      } else {
        this.expandedElement = [];
        this.lessthan40 = false;
        $('.arrowAll').text('arrow_right');
      }
    })
    await this.getDayofweek();
  }
  // Laasy thong tin tinh hinh lam viec
  async searchEmployeeRequest(){
    await this.employeeRequestService.getEmployeeRequestByEmpId(this.auth.EmployeeId,this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.amountAbsentWithoutPermission = result.amountAbsentWithoutPermission;
      this.amountAbsentWithPermission = result.amountAbsentWithPermission;
    });
    // Get thông tin phụ cấp nhân viên
    this.employeeAllowanceService.getEmpAllowanceByEmpId(this.auth.EmployeeId, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.employeeAllowance != null){
        this.employeeAllowance = <EmployeeAllowanceModel>({
          MaternityAllowance: result.employeeAllowance.maternityAllowance == null ? 0 : result.employeeAllowance.maternityAllowance,
          LunchAllowance: result.employeeAllowance.lunchAllowance == null ? 0 : result.employeeAllowance.lunchAllowance,
          FuelAllowance: result.employeeAllowance.fuelAllowance == null ? 0 : result.employeeAllowance.fuelAllowance,
          PhoneAllowance: result.employeeAllowance.phoneAllowance == null ? 0 : result.employeeAllowance.phoneAllowance,
          OtherAllownce: result.employeeAllowance.otherAllownce == null ? 0 : result.employeeAllowance.otherAllownce
      })
      } else this.employeeAllowance.MaternityAllowance = 0;
    })
  }
  
  getDayofweek(){
    this.dayOfWeek = [];
    for(let i=0; i < 7;i++){
      this.dayOfWeek.push(new Date(this.firstDayOfWeek.getTime()+86400000*i).getDate());
    }
  }
  async changeWeek(property){
    if (property == 0){
      this.firstDayOfWeek = new Date(this.firstDayOfWeek.getTime()-86400000*7);
      this.lastDayOfWeek = new Date(this.lastDayOfWeek.getTime()-86400000*7);
    } else {
      this.firstDayOfWeek = new Date(this.firstDayOfWeek.getTime()+86400000*7);
      this.lastDayOfWeek = new Date(this.lastDayOfWeek.getTime()+86400000*7);
    }
    await this.getDayofweek();
    await this.empService.getStatisticForEmpDashBoard(this.firstDayOfWeek,this.lastDayOfWeek,this.currentKey,this.auth.UserId).subscribe(response => {
      const result = <any>response;
      this.dataSource = new MatTableDataSource(result.listEmployee);
      this.listEmpRqInsideWeek = result.listRequestInsideWeek;
      this.listEmpRequestEmpId = result.listRequestInsideWeek.map(r => r.offerEmployeeId);
      this.listEmpNearestBirthday = result.listEmpNearestBirthday;
      // this.isManager = result.isManager;
    })
  }
  isHighlighted(empId,tt,shift){
    if (!this.listEmpRequestEmpId.includes(empId)) {
      return false
    } 
    else
    { 
      let result = false;
      this.listEmpRqInsideWeek.forEach(item => {
        let startDateItem = new Date(item.startDate);
        let endDateItem = new Date(item.enDate);
        if (item.offerEmployeeId == empId){
          const current = new Date(this.firstDayOfWeek.getTime()+86400000*tt);
          if (this.compareDate(current,new Date(item.startDate)) == 1 && this.compareDate(current,new Date(item.enDate)) == -1){
            result = true;
            return;
          }
          if (this.compareDate(current,new Date(item.startDate)) == 0){
            if (item.shiftName.startsWith("SAN")) {
              if (shift == "AM") {
                result = true;
                return;
              }
              if (item.shiftName.endsWith("CHI") && shift == "PM") {
                result = true;
                return;
              }
              if (item.shiftName.endsWith("SAN") && shift == "PM" && current < endDateItem) {
                result = true;
                return;
              }
            }
            if (item.shiftName.startsWith("CHI") && shift == "PM") {
              result = true;
              return;
            }
          }
          if (this.compareDate(current,new Date(item.enDate)) == 0){
            if (item.shiftName.endsWith("CHI")) {
              if (shift == "PM") {
                result = true;
                return;
              }
              if (item.shiftName.startsWith("SAN") && shift == "AM") {
                result = true;
                return;
              }
              if (item.shiftName.startsWith("CHI") && shift == "AM" && current > startDateItem) {
                result = true;
                return;
              }
            }
            if (item.shiftName.endsWith("SAN") && shift == "AM") {
              result = true;
              return;
            }
          }
        }
      });
      return result;
    }
  }
  compareDate(d1: Date, d2: Date){
    // 2 date bang nhau
    if(d1.getFullYear() === d2.getFullYear()
    && d1.getDate() === d2.getDate()
    && d1.getMonth() === d2.getMonth()) return 0;
    // d1 > d2
    if (d1.getFullYear() > d2.getFullYear()) return 1;
    if (d1.getFullYear() < d2.getFullYear()) return -1;

    if (d1.getMonth() > d2.getMonth()) return 1;
    if (d1.getMonth() < d2.getMonth()) return -1;

    if (d1.getDate() > d2.getDate()) return 1;
    if (d1.getDate() < d2.getDate()) return -1;
  }

  goToEmployeeList(type) {
    this.router.navigate(['/employee/list', { 
      type: type
    }]);
  }
}
