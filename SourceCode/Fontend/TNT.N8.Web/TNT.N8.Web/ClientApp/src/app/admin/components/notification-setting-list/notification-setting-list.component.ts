import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Table } from 'primeng/table';
import { MessageService, SortEvent } from 'primeng/api';
import { Screen } from '../../models/screen.model';
import { NotifiSetting } from '../../models/notifiSetting.model';
import { NotifiSettingService } from '../../services/notifi-setting.service';
import { EmployeeService } from '../../../employee/services/employee.service';

@Component({
  selector: 'app-notification-setting-list',
  templateUrl: './notification-setting-list.component.html',
  styleUrls: ['./notification-setting-list.component.css']
})
export class NotificationSettingListComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  innerWidth: number = 0; //number window size first
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  nowDate: Date = new Date();
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy

  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 0;

  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = null;
  colsList: any;
  selectedColumns: any[];

  actionAdd: boolean = true;

  listNotifiSetting: Array<NotifiSetting> = [];
  listScreen: Array<Screen> = [];
  listSelectedScreen: Array<Screen> = [];
  notifiSettingName: string = null;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    private notifiSettingService: NotifiSettingService,
    private employeeService: EmployeeService,
  ) { 
    this.translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit(): void {
    this.initTable();

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getMasterData();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/notifi-setting-list/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.router.navigate(['/home']);
        }
        else {
          this.getMasterData();
        }
      }
    });
  }

  initTable() {
    this.colsList = [
      { field: 'notifiSettingName', header: 'Tên thông báo', width: '150px', textAlign: 'left', display: 'table-cell' },
      { field: 'active', header: 'Kích hoạt', width: '60px', textAlign: 'center', display: 'table-cell' },
      { field: 'sendInternal', header: 'Gửi nội bộ', width: '60px', textAlign: 'center', display: 'table-cell' },
      { field: 'isSystem', header: 'Hệ thống', width: '60px', textAlign: 'center', display: 'table-cell' },
      { field: 'isEmail', header: 'Email', width: '60px', textAlign: 'center', display: 'table-cell' },
      { field: 'isSms', header: 'SMS', width: '60px', textAlign: 'center', display: 'table-cell' },
      { field: 'objectBackHourInternalName', header: 'Gửi trước', width: '90px', textAlign: 'left', display: 'table-cell' },
      { field: 'backHourInternal', header: 'Giờ', width: '40px', textAlign: 'center', display: 'table-cell' },
    ];

    this.selectedColumns = this.colsList;
  }

  getMasterData() {
    this.notifiSettingService.getMasterDataSearchNotifiSetting().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listScreen = result.listScreen;
        this.searchNotifiSetting();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  searchNotifiSetting() {
    this.loading = true;
    this.notifiSettingService.searchNotifiSetting(this.notifiSettingName?.trim(), this.listSelectedScreen).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listNotifiSetting = result.listNotifiSetting;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  showFilter() {
    if (this.innerWidth < 1024) {
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

  refreshFilter() {
    this.notifiSettingName = null;
    this.listSelectedScreen = [];
    this.filterGlobal = null;
    this.resetTable();

    this.searchNotifiSetting();
  }

  changeBackHourInternal(notifiSetting: NotifiSetting) {
    //Nếu không tồn tại "Gửi trước"
    if (!notifiSetting.objectBackHourInternal) {
      notifiSetting.backHourInternal = null;
    } 
    //Nếu đã có "Gửi trước"
    else {
      let backHourInternal = ParseStringToFloat(notifiSetting.backHourInternal.toString());
      //Nếu để trống thì sẽ nhận giá trị 0
      if (backHourInternal == 0) {
        notifiSetting.backHourInternal = 0;
      }

      this.loading = true;
      this.notifiSettingService.changeBackHourInternal(notifiSetting.notifiSettingId, backHourInternal).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  changeActive(notifiSetting: NotifiSetting) {
    this.loading = true;
    this.notifiSettingService.changeActive(notifiSetting.notifiSettingId, notifiSetting.active).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeSendInternal(notifiSetting: NotifiSetting) {
    this.loading = true;
    this.notifiSettingService.changeSendInternal(notifiSetting.notifiSettingId, notifiSetting.sendInternal).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeIsSystem(notifiSetting: NotifiSetting) {
    this.loading = true;
    this.notifiSettingService.changeIsSystem(notifiSetting.notifiSettingId, notifiSetting.isSystem).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeIsEmail(notifiSetting: NotifiSetting) {
    this.loading = true;
    this.notifiSettingService.changeIsEmail(notifiSetting.notifiSettingId, notifiSetting.isEmail).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  changeIsSms(notifiSetting: NotifiSetting) {
    this.loading = true;
    this.notifiSettingService.changeIsSms(notifiSetting.notifiSettingId, notifiSetting.isSms).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  onViewDetail(rowData : any) {
    this.router.navigate(['/admin/notifi-setting-detail', { notifiSettingId: rowData.notifiSettingId }]);
  }

  goToCreate() {
    this.router.navigate(['/admin/notifi-setting']);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  resetTable() {
    if (this.myTable) {
      this.myTable.reset();
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}
