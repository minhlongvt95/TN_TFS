import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { Observable } from 'rxjs';
import * as $ from 'jquery';
import { CustomerlevelService } from '../../../shared/services/customerlevel.service';
import { CustomerServiceLevelModel } from '../../../shared/models/customerservicelevel.model';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { EmployeeService } from '../../../employee/services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';

@Component({
  selector: 'app-config-level-customer',
  templateUrl: './config-level-customer.component.html',
  styleUrls: ['./config-level-customer.component.css']
})
export class ConfigLevelCustomerComponent implements OnInit {
  customerLevelId: string;

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private router: Router,
    private employeeService: EmployeeService,
    private customerLevelService: CustomerlevelService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog) {
    translate.setDefaultLang('vi');
  }
  public isShow = false;
  public newAttribute: any = {
    MinimumSaleValue: 0,
    CustomerServiceLevelName: '',
    CustomerServiceLevelId: null,
    CustomerServiceLevelCode: null,
    CreatedById: null,
    CreatedDate: null,
    UpdatedById: null,
    UpdatedDate: null,
    Active: true
  };

  actionAdd: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  customerServiceLevelModel = new Array<CustomerServiceLevelModel>();
  newCustomerLevelModel = new Array<CustomerServiceLevelModel>();
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  dialogPopup: MatDialogRef<PopupComponent>;

  addLevel() {
    this.customerServiceLevelModel.push(this.newAttribute);
    this.newAttribute = {
      MinimumSaleValue: 0,
      CustomerServiceLevelName: '',
      CustomerServiceLevelId: null,
      CustomerServiceLevelCode: null,
      CreatedById: null,
      CreatedDate: null,
      UpdatedById: null,
      UpdatedDate: null,
      Active: true
    };
    this.isShow = true;
  }

  removeLevel(index, id) {
    this.dialogPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: 'XÓA', content: 'Bạn có chắc chắn muốn xóa?' }
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      if (result) {
        if (id == null) {
          this.customerServiceLevelModel.splice(index, 1);
          this.newCustomerLevelModel = this.customerServiceLevelModel.filter( el => el.CustomerServiceLevelId === null );
          if (this.newCustomerLevelModel.length === 0) {
            this.isShow = false;
          }
        } else {
          this.customerLevelService.updateLevelCustomer(id).subscribe(response => {
            // tslint:disable-next-line:no-shadowed-variable
            const result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ... this.successConfig });
              this.getConfigLevelCustomer();
              this.isShow = false;
            } else {
              this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
              this.getConfigLevelCustomer();
              this.isShow = false;
            }
          }, error => { });
        }
      }
    });
  }

  getConfigLevelCustomer() {
    this.customerLevelService.getLevelCustomer().subscribe(response => {
      const result = <any>response;
      this.customerServiceLevelModel = [];
      for (let i = 0; i < result.customerServiceLevel.length; i++) {
        const item: CustomerServiceLevelModel = {
          MinimumSaleValue: result.customerServiceLevel[i].minimumSaleValue,
          CustomerServiceLevelName: result.customerServiceLevel[i].customerServiceLevelName,
          CustomerServiceLevelId: result.customerServiceLevel[i].customerServiceLevelId,
          CustomerServiceLevelCode: result.customerServiceLevel[i].customerServiceLevelCode,
          CreatedById: result.customerServiceLevel[i].createdById,
          CreatedDate: result.customerServiceLevel[i].createdDate,
          UpdatedById: result.customerServiceLevel[i].updatedById,
          UpdatedDate: result.customerServiceLevel[i].updatedDate,
          Active: result.customerServiceLevel[i].active,
        };
        this.customerServiceLevelModel.push(item);
      }
      if (result.statusCode === 202 || result.statusCode === 200) {
        if (this.customerServiceLevelModel.length === 0) {
          this.snackBar.openFromComponent(WarningComponent, { data: 'Không tìm thấy phân loại khách hàng nào!', ... this.warningConfig });
        }
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }
    }, error => { });
  }

  cancel() {
    this.isShow = false;
  }

  save() {
    this.newCustomerLevelModel = this.customerServiceLevelModel.filter( el => el.CustomerServiceLevelId === null );
    this.customerLevelService.addLevelCustomer(this.newCustomerLevelModel).subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
        this.getConfigLevelCustomer();
        this.isShow = false;
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }
    }, error => { });
  }

  ngOnInit() {
    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getConfigLevelCustomer();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/config-level-customer/";
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
          if (listCurrentActionResource.indexOf("delete") == -1) {
            this.actionDelete = false;
          }
          this.getConfigLevelCustomer();
        }
      }
    }, error => { });
  }

}
