import { Component, OnInit, ViewChild } from '@angular/core';
import { PermissionService } from '../../../shared/services/permission.service';
import { TranslateService } from '@ngx-translate/core';
import { PermissionSetModel } from '../../../shared/models/permissionSet.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { EmployeeService } from '../../../employee/services/employee.service';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import * as $ from 'jquery';
import { GetPermission } from '../../../shared/permission/get-permission';

@Component({
  selector: 'app-permission',
  templateUrl: './permission.component.html',
  styleUrls: ['./permission.component.css'],
})
export class PermissionComponent implements OnInit {
  displayedColumns: string[] = ['roleValue', 'description', 'userNumber'];
  selectedRowIndex: string = '';
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  permissionSetModel: PermissionSetModel = {
    PermissionSetId: '',
    PermissionSetName: '',
    PermissionSetCode: '',
    PermissionSetDescription: '',
    PermissionId: [],
    PermissionList: [],
    CreatedById: '',
    CreatedDate: null,
    UpdatedById: '',
    UpdatedDate: null,
    Active: true,
    NumberOfUserHasPermission: 0
  }

  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  listRole: Array<any> = [];
  permissionList: Array<any> = [];
  dataSource: MatTableDataSource<any>;
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private employeeService: EmployeeService,
    public snackBar: MatSnackBar,
    private permissisonService: PermissionService,
  ) { }

  ngOnInit() {
    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        $(".permission_content").click((evt) => {
          if (!evt.target.classList.contains("mat-row") && !evt.target.classList.contains("mat-cell")) {
            $(".selected-row").removeClass("selected-row");
          }
        });

        this.getAllPermission();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/permission/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("add") == -1) {
            this.actionAdd = false;
          }
          $(".permission_content").click((evt) => {
            if (!evt.target.classList.contains("mat-row") && !evt.target.classList.contains("mat-cell")) {
              $(".selected-row").removeClass("selected-row");
            }
          });

          this.getAllPermission();
        }
      }
    }, error => { });
  }

  highlight(row) {
    this.selectedRowIndex = row.permissionId;
  }

  openCreate() {
    this.router.navigate(['/admin/permission-create']);
  }

  onViewPermissionDetail(id) {
    this.router.navigate(['/admin/permission-detail', { roleId: id }]);
  }

  getAllPermission() {
    this.permissisonService.getAllRole().subscribe(response => {
      var result = <any>response;
      this.listRole = result.listRole;
      this.dataSource = new MatTableDataSource<any>(this.listRole);
      this.dataSource.paginator = this.paginator;
      this.paginator._intl.itemsPerPageLabel = "Số nhóm quyền mỗi trang: ";
      this.paginator._intl.getRangeLabel = (page, pageSize, length) => {
        if (length === 0 || pageSize === 0) {
          return '0 trên ' + length;
        }
        length = Math.max(length, 0);
        const startIndex = page * pageSize;
        const endIndex = startIndex < length ?
          Math.min(startIndex + pageSize, length) :
          startIndex + pageSize;
        return startIndex + 1 + ' - ' + endIndex + ' trên ' + length;
      }
    }, error => { });
  }

  //Function sap xep du lieu
  isAscending: boolean = false;
  sort(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.listRole.sort((a: any, b: any) => {
      let x: any = "";
      let y: any = "";
      switch (property) {
        case "roleValue":
          x = a.roleValue.toLowerCase().trim();
          y = b.roleValue.toLowerCase().trim();
          break;

        case "userNumber":
          x = a.userNumber.toString();
          y = b.userNumber.toString();
          break;
        default:
          break;
      }

      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataSource = new MatTableDataSource<PermissionSetModel>(this.listRole);
    this.dataSource.paginator = this.paginator;
  }
  //Ket thuc
}

