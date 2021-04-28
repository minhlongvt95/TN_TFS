import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SystemParameterService } from '../../services/system-parameter.service';
import { EmployeeService } from '../../../employee/services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';

interface SystemParameterModel {
  systemDescription: string,
  systemKey: string,
  systemValue: boolean,
  systemValueString: string,
  systemGroupCode: string,
  description: string,
  isEdit: boolean
}

@Component({
  selector: 'app-system-parameter',
  templateUrl: './system-parameter.component.html',
  styleUrls: ['./system-parameter.component.css']
})

export class SystemParameterComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  userId: string = this.auth.UserId;
  loading: boolean = false;
  actionEdit: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  systemParameterList: Array<SystemParameterModel> = [];
  listGroupSystem: Array<SystemParameterModel> = [];
  listGroupEmail: Array<SystemParameterModel> = [];
  listGroupTemplateEmail: Array<SystemParameterModel> = [];
  listGroupCurrency: Array<SystemParameterModel> = [];
  listGroupLogo: Array<SystemParameterModel> = [];
  colHeader: any[];
  base64Logo: any;
  currentBase64Logo: any;
  @ViewChild('currentLogo') currentLogo: ElementRef;
  validLogo: string = null;

  constructor(
    private router: Router,
    private getPermission: GetPermission,
    private translate: TranslateService,
    private employeeService: EmployeeService,
    private systemParameterService: SystemParameterService,
    private messageService: MessageService,
    private ref: ChangeDetectorRef
  ) {
    this.translate.setDefaultLang('vi');
  }

  ngOnInit() {
    /* #region  init table header */
    this.colHeader = [
      { field: 'loaiThamSo', header: 'Loại Tham Số', textAlign: 'left', display: 'table-cell', colWith: '15%' },
      { field: 'giaTri', header: 'Giá Trị', textAlign: 'left', display: 'table-cell', colWith: '15%' },
      { field: 'moTa', header: 'Mô tả', textAlign: 'left', display: 'table-cell', colWith: '20%' },
      { field: 'chucNang', header: 'Chức năng', textAlign: 'center', display: 'table-cell', colWith: '5%' },
    ];
    /* #endregion */

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getMasterData();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/system-parameter/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.messageService.add({ severity: 'warn', summary: 'Thông báo', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' });
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("edit") == -1) {
            this.actionEdit = false;
          }
          this.getMasterData();
        }
      }
    });
  }

  getMasterData() {
    this.loading = true;
    this.systemParameterService.GetAllSystemParameter().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.systemParameterList = result.systemParameterList;
      this.systemParameterList.forEach(item => {
        item.isEdit = false;
      });

      //Chia theo groupCode
      this.listGroupSystem = this.systemParameterList.filter(x => x.systemGroupCode == "SYSTEM");
      this.listGroupEmail = this.systemParameterList.filter(x => x.systemGroupCode == "EMAIL");
      this.listGroupTemplateEmail = this.systemParameterList.filter(x => x.systemGroupCode == "EmailTemplate");
      this.listGroupCurrency = this.systemParameterList.filter(x => x.systemGroupCode == "CURRENCY");
      this.listGroupLogo = this.systemParameterList.filter(x => x.systemGroupCode == "LOGO");

      this.base64Logo = this.systemParameterList.find(x => x.systemKey == 'Logo').systemValueString;
    });
  }

  /*Hủy chỉnh sửa*/
  cancelEdit(data: SystemParameterModel) {
    data.isEdit = false;
  }

  /*Chỉnh sửa*/
  onEdit(data: SystemParameterModel) {
    data.isEdit = true;
  }

  changeParameter(element: any) {
    this.loading = true;
    let systemKey = element.systemKey;
    let systemValue = element.systemValue;
    let systemmValueString = element.systemValueString;
    this.systemParameterService.ChangeSystemParameter(systemKey, systemValue, systemmValueString, element.description).subscribe(response => {
      let result = <any>response;
      if (result.statusCode == 200) {
        this.loading = false;
        localStorage.setItem("systemParameterList", JSON.stringify(result.systemParameterList));
        this.messageService.add({ severity: 'success', summary: 'Thông báo', detail: result.messageCode });
        this.getMasterData();
      }
      else {
        this.loading = false;
        this.messageService.add({ severity: 'error', summary: 'Thông báo', detail: result.messageCode });
      }
    });
  };
  
  handleFile(event: any) {
    this.validLogo = null;
  }

  async myUploader(event: any) {
    let blobUrl = '';
    this.validLogo = null;
    for (let file of event.files) {
      blobUrl = file.objectURL.changingThisBreaksApplicationSecurity;
    }

    if (blobUrl != '') {
      let base64 = await this.getBase64ImageFromURL(blobUrl);
      this.currentBase64Logo = base64;
      this.ref.detectChanges();
      setTimeout(() => {
        let naturalWidth = this.currentLogo.nativeElement.naturalWidth;
        let naturalHeight = this.currentLogo.nativeElement.naturalHeight;

        if ((naturalWidth < 140 || naturalWidth > 150) && (naturalHeight < 60 || naturalHeight > 70)) {
          this.validLogo = 'chiều rộng trong khoảng: 140px -> 150px, chiêu dài trong khoảng 60px -> 70px';
        } else {
          let description = this.systemParameterList.find(x => x.systemKey == 'Logo').description;
          //Update base64
          this.systemParameterService.ChangeSystemParameter('Logo', null, this.currentBase64Logo, description).subscribe(response => {
            let result = <any>response;

            if (result.statusCode == 200) {
              localStorage.setItem("systemParameterList", JSON.stringify(result.systemParameterList));
              this.getMasterData();
              this.messageService.add({ severity: 'success', summary: 'Thông báo', detail: 'Lưu thành công' });
            } else {
              this.messageService.add({ severity: 'error', summary: 'Thông báo', detail: result.messageCode });
            }
          });
        }
      }, 1);
    }
  }

  /*Event: xóa 1 file trong list file*/
  onRemove(event: any) {
    this.currentBase64Logo = null;
    this.validLogo = null;
  }

  getBase64ImageFromURL(url) {
    return new Promise((resolve, reject) => {
      var img = new Image();
      img.setAttribute("crossOrigin", "anonymous");

      img.onload = () => {
        var canvas = document.createElement("canvas");
        canvas.width = img.width;
        canvas.height = img.height;

        var ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0);

        var dataURL = canvas.toDataURL("image/png");

        resolve(dataURL);
      };

      img.onerror = error => {
        reject(error);
      };

      img.src = url;
    });
  }
};
