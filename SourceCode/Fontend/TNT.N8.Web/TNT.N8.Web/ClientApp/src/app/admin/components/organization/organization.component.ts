import { Component, OnInit, ElementRef } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { OrganizationService } from "../../../shared/services/organization.service";
import { OrganizationModel } from "../../../shared/models/organization.model";
import { OrganizationDialogComponent } from "../organization-dialog/organization-dialog.component";
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { TranslateService } from '@ngx-translate/core';
import { EmployeeService } from '../../../employee/services/employee.service';
import { Router } from '@angular/router';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { GetPermission } from '../../../shared/permission/get-permission';
import * as $ from 'jquery';
import { TreeNode } from 'primeng/api';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MessageService, ConfirmationService } from 'primeng/api';

class Organization {
  organizationId: string;
  organizationName: string;
  organizationCode: string;
  address: string;
  phone: string;
  parentId: string;
  parentName: string;
  level: Number;
  isFinancialIndependence: boolean;
  orgChildList: Array<Organization>;
  children: Array<any>;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: Boolean;

  constructor() { }
}

@Component({
  selector: 'app-organization',
  templateUrl: './organization.component.html',
  styleUrls: ['./organization.component.css']
})
export class OrganizationComponent implements OnInit {
  organizationModel: OrganizationModel = {
    OrganizationId: '',
    OrganizationName: '',
    OrganizationCode: '',
    Address: '',
    Phone: '',
    ParentId: '',
    ParentName: '',
    IsFinancialIndependence: false,
    Level: 1,
    CreatedById: '',
    CreatedDate: null,
    UpdatedById: '',
    UpdatedDate: null,
    Active: true
  };

  orgNameDisplay: string;
  orgCodeDisplay: string;
  orgParentDisplay: string;
  orgPhoneDisplay: string;
  orgAddressDisplay: string;
  toastMessage: string;
  orgList: Array<Organization>;
  listOrganization: Array<Organization>;
  listGeographicalArea: Array<any> = [];
  listProvince: Array<any> = [];
  listDistrict: Array<any> = [];
  listCurrentDistrict: Array<any> = [];
  listWard: Array<any> = [];
  listCurrentWard: Array<any> = [];
  listSatellite: Array<any> = [];
  selectedOrgId: string = '';
  selectedOrgName: string;
  reloadAfterCreate: boolean = false;

  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  dialogCreateEdit: MatDialogRef<OrganizationDialogComponent>;
  dialogPopup: MatDialogRef<PopupComponent>;

  loading: boolean = false;

  data: TreeNode[];
  selectedNode: TreeNode;

  organizationId: string = null;
  organizationParentName: string = null;
  isEnableForm: boolean = false;

  listCreProvince: Array<any> = [];
  listCreDistrict: Array<any> = [];
  listCreCurrentDistrict: Array<any> = [];
  listCreWard: Array<any> = [];
  listCreCurrentWard: Array<any> = [];

  organizationForm: FormGroup;
  organizationNameControl: FormControl;
  organizationCodeControl: FormControl;
  organizationPhoneControl: FormControl;
  organizationAddressControl: FormControl;
  financialIndependenceControl: FormControl;
  geographicalAreaControl: FormControl;
  provinceControl: FormControl;
  districtControl: FormControl;
  wardControl: FormControl;
  satelliteControl: FormControl;
  organizationOtherCodeControl: FormControl;

  displayModal: boolean = false;
  orgCreateForm: FormGroup;
  orgNameControl: FormControl;
  orgCodeControl: FormControl;
  orgPhoneControl: FormControl;
  orgAddressControl: FormControl;
  orgFinancialIndependenceControl: FormControl;
  orgGeographicalAreaControl: FormControl;
  orgProvinceControl: FormControl;
  orgDistrictControl: FormControl;
  orgWardControl: FormControl;
  orgSatelliteControl: FormControl;
  orgOtherCodeControl: FormControl;

  constructor(private el: ElementRef,
    private getPermission: GetPermission,
    public dialog: MatDialog,
    private organizationService: OrganizationService,
    private translate: TranslateService,
    private employeeService: EmployeeService,
    private router: Router,
    public snackBar: MatSnackBar,
    private messageService: MessageService,) {
    translate.setDefaultLang('vi');
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  ngOnInit() {
    this.setForm();
    this.organizationForm.disable();

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getAllOrganization();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/organization/";
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
          if (listCurrentActionResource.indexOf("edit") == -1) {
            this.actionEdit = false;
          }
          if (listCurrentActionResource.indexOf("delete") == -1) {
            this.actionDelete = false;
          }
          this.getAllOrganization();
        }
      }
    }, error => { });
  }

  setForm() {
    this.organizationNameControl = new FormControl('', [Validators.required]);
    this.organizationCodeControl = new FormControl('', [Validators.required]);
    this.organizationPhoneControl = new FormControl('');
    this.organizationAddressControl = new FormControl('');
    this.financialIndependenceControl = new FormControl(false);
    this.geographicalAreaControl = new FormControl(null);
    this.provinceControl = new FormControl(null);
    this.districtControl = new FormControl(null);
    this.wardControl = new FormControl(null);
    this.satelliteControl = new FormControl(null);
    this.organizationOtherCodeControl = new FormControl({value: null, disabled: true});

    this.organizationForm = new FormGroup({
      organizationNameControl: this.organizationNameControl,
      organizationCodeControl: this.organizationCodeControl,
      organizationPhoneControl: this.organizationPhoneControl,
      organizationAddressControl: this.organizationAddressControl,
      financialIndependenceControl: this.financialIndependenceControl,
      geographicalAreaControl: this.geographicalAreaControl,
      provinceControl: this.provinceControl,
      districtControl: this.districtControl,
      wardControl: this.wardControl,
      satelliteControl: this.satelliteControl,
      organizationOtherCodeControl: this.organizationOtherCodeControl
    });

    this.orgNameControl = new FormControl('', [Validators.required]);
    this.orgCodeControl = new FormControl('', [Validators.required]);
    this.orgPhoneControl = new FormControl('');
    this.orgAddressControl = new FormControl('');
    this.orgFinancialIndependenceControl = new FormControl(false);
    this.orgGeographicalAreaControl = new FormControl(null);
    this.orgProvinceControl = new FormControl(null);
    this.orgDistrictControl = new FormControl(null);
    this.orgWardControl = new FormControl(null);
    this.orgSatelliteControl = new FormControl(null);
    this.orgOtherCodeControl = new FormControl({value: null, disabled: true});

    this.orgCreateForm = new FormGroup({
      orgNameControl: this.orgNameControl,
      orgCodeControl: this.orgCodeControl,
      orgPhoneControl: this.orgPhoneControl,
      orgAddressControl: this.orgAddressControl,
      orgFinancialIndependenceControl: this.orgFinancialIndependenceControl,
      orgGeographicalAreaControl: this.orgGeographicalAreaControl,
      orgProvinceControl: this.orgProvinceControl,
      orgDistrictControl: this.orgDistrictControl,
      orgWardControl: this.orgWardControl,
      orgSatelliteControl: this.orgSatelliteControl,
      orgOtherCodeControl: this.orgOtherCodeControl
    });
  }

  getAllOrganization() {
    this.loading = true;
    this.organizationService.getAllOrganization().subscribe(response => {
      let result = <any>response;
      this.loading = false;
      this.orgList = result.organizationList;
      this.listOrganization = result.listAll;
      this.listGeographicalArea = result.listGeographicalArea;
      this.listProvince = result.listProvince;
      this.listCreProvince = result.listProvince;
      this.listDistrict = result.listDistrict;
      this.listCreDistrict = result.listDistrict;
      this.listWard = result.listWard;
      this.listCreWard = result.listWard;
      this.listSatellite = result.listSatellite;

      /* Convert data to type TreeNode */
      this.data = this.list_to_tree();

      this.onNodeUnselect(1);
    });
  }

  list_to_tree() {
    let list: Array<TreeNode> = [];
    this.listOrganization.forEach(item => {
      let node: TreeNode = {
        label: item.organizationName,
        expanded: true,
        data: {
          'organizationId': item.organizationId,
          'parentId': item.parentId,
          'isFinancialIndependence': item.isFinancialIndependence,
          'level': item.level
        },
        children: []
      };

      list = [...list, node];
    });

    var map = {}, node, roots = [], i;

    for (i = 0; i < list.length; i += 1) {
      map[list[i].data.organizationId] = i; // initialize the map
      list[i].children = []; // initialize the children
    }

    for (i = 0; i < list.length; i += 1) {
      node = list[i];
      if (node.data.level !== 0) {
        // if you have dangling branches check that map[node.parentId] exists
        list[map[node.data.parentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }

  /* Enable form: Chỉnh sửa */
  updateOrg() {
    if (this.selectedNode) {
      this.organizationForm.enable();

      this.organizationOtherCodeControl.disable();
      this.isEnableForm = true;
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn phòng ban' };
      this.showMessage(msg);
    }
  }

  /* Lưu thay đổi phòng ban */
  saveOrg() {
    if (!this.organizationForm.valid) {
      Object.keys(this.organizationForm.controls).forEach(key => {
        if (this.organizationForm.controls[key].valid == false) {
          this.organizationForm.controls[key].markAsTouched();
        }
      });
    } else {
      let organizationName = this.organizationNameControl.value;
      let organizationCode = this.organizationCodeControl.value;
      let organizationPhone = this.organizationPhoneControl.value;
      let organizationAddress = this.organizationAddressControl.value;
      let isFinancialIndependence = this.financialIndependenceControl.value;
      let GeographicalAreaId = this.geographicalAreaControl.value?.geographicalAreaId;
      let ProvinceId = this.provinceControl.value?.provinceId;
      let DistrictId = this.districtControl.value?.districtId;
      let WardId = this.wardControl.value?.wardId;
      let SatelliteId = this.satelliteControl.value?.satelliteId;
      let organizationOtherCode = this.organizationOtherCodeControl.value;

      this.loading = true;
      this.organizationService.updateOrganizationById(this.organizationId, organizationName, organizationCode,
        organizationPhone, organizationAddress, isFinancialIndependence,
        GeographicalAreaId, ProvinceId, DistrictId, WardId, SatelliteId, organizationOtherCode).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            //Thay đổi trên tree
            this.selectedNode.label = organizationName;
            this.selectedNode.data.isFinancialIndependence = isFinancialIndependence;

            this.organizationForm.disable();
            this.isEnableForm = false;

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Chỉnh sửa thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
      });
    }
  }

  /* Disable form: Hủy chỉnh sửa */
  cancelUpdateOrg() {
    this.organizationForm.disable();
    this.isEnableForm = false;
  }

  getOrganizationById(orgId: string) {
    this.organizationService.getOrganizationById(orgId).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.organizationModel = <OrganizationModel>({
          OrganizationId: result.organization.organizationId,
          OrganizationName: result.organization.organizationName,
          OrganizationCode: result.organization.organizationCode,
          Phone: result.organization.phone,
          Address: result.organization.address,
          IsFinancialIndependence: result.organization.isFinancialIndependence,
          ParentId: result.organization.parentId,
          ParentName: result.organization.parentName,
          Level: result.organization.level,
          UpdatedById: result.organization.updatedById,
          UpdatedDate: result.organization.updatedDate,
          Active: result.organization.active,
        });

        this.orgParentDisplay = result.organization.parentName;

        this.setTextDisplay(this.organizationModel.OrganizationName,
          this.organizationModel.OrganizationCode,
          this.selectedOrgName,
          this.organizationModel.Phone,
          this.organizationModel.Address);
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }
    }, error => { });
  }

  deleteOrganizationById() {
    let _title = "XÁC NHẬN";
    let _content = "Bạn có muốn xóa tổ chức này không?";
    this.dialogPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      if (result) {
        this.organizationService.deleteOrganizationById(this.selectedOrgId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
            this.setTextDisplay('', '', '', '', '');
            this.getAllOrganization();
          } else {
            this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
          }
        }, error => {
          let result = <any>error;
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        });
      }
    });
  }

  setTextDisplay(orgName: string, orgCode: string, orgParent: string, orgPhone: string, orgAddress: string) {
    this.orgNameDisplay = orgName;
    this.orgCodeDisplay = orgCode;
    this.orgPhoneDisplay = orgPhone;
    this.orgAddressDisplay = orgAddress;
  }

  hasChild(org): boolean {
    return ((org.orgChildList.length > 0) ? true : false);
  }

  openDialog(mode: string) {
    if (this.selectedOrgId === null || this.selectedOrgId === '') {
      let messageString: string = 'Vui lòng chọn 1 đơn vị';
      this.snackBar.openFromComponent(FailComponent, { data: messageString, ...this.failConfig });
    } else {
      this.dialogCreateEdit = this.dialog.open(OrganizationDialogComponent,
        {
          width: '550px',
          height: '580',
          autoFocus: false,
          data: { selectedOrgId: this.selectedOrgId, selectedOrgName: this.selectedOrgName, mode: mode }
        });
      this.dialogCreateEdit.afterClosed().subscribe(result => {
        if (result.orgCreated || result.orgEdited) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.message, ...this.successConfig });

          if (result.orgCreatedId !== "" && result.orgCreatedId !== null) {
            this.selectedOrgId = result.orgCreatedId;
            this.reloadAfterCreate = true;
            this.getAllOrganization();
          }
        }
      });
    }
  }

  onNodeSelect(event) {
    this.object_code = {
      organizationCode: '',
      geographicalAreaCode : '',
      provinceCode: '',
      districtCode: '',
      wardCode: '',
      satelliteCode: ''
    }

    let organizationId = event.node.data.organizationId;

    if (organizationId) {
      this.organizationId = organizationId;
      this.loading = true;
      this.organizationService.getOrganizationById(organizationId).subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.mapDataToForm(result.organization);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  onNodeUnselect(event) {
    this.organizationId = null;
    this.organizationParentName = null;
    this.object_code = {
      organizationCode: '',
      geographicalAreaCode : '',
      provinceCode: '',
      districtCode: '',
      wardCode: '',
      satelliteCode: ''
    }
    this.selectedNode = null;
    this.organizationForm.reset();
  }

  mapDataToForm(orgInformation) {
    this.organizationParentName = orgInformation.parentName ? orgInformation.parentName : '';
    this.organizationNameControl.setValue(orgInformation.organizationName ? orgInformation.organizationName : '');
    this.organizationCodeControl.setValue(orgInformation.organizationCode ? orgInformation.organizationCode : '');
    this.organizationPhoneControl.setValue(orgInformation.phone ? orgInformation.phone : '');
    this.organizationAddressControl.setValue(orgInformation.address ? orgInformation.address : '');
    this.financialIndependenceControl.setValue(orgInformation.isFinancialIndependence ? orgInformation.isFinancialIndependence : false);
    
    let geographicalArea = this.listGeographicalArea.find(x => x.geographicalAreaId == orgInformation.geographicalAreaId);
    this.geographicalAreaControl.setValue(geographicalArea ? geographicalArea : null);

    let province = this.listProvince.find(x => x.provinceId == orgInformation.provinceId);
    if (province) {
      this.provinceControl.setValue(province ? province : null);
      this.listCurrentDistrict = this.listDistrict.filter(x => x.provinceId == province.provinceId);
    } else {
      this.listCurrentDistrict = [];
      this.listCurrentWard = [];
    }

    let district = this.listDistrict.find(x => x.districtId == orgInformation.districtId);
    if (district) {
      this.districtControl.setValue(district ? district : null);
      this.listCurrentWard = this.listWard.filter(x => x.districtId == district.districtId);
    } else {
      this.listCurrentWard = [];
    }

    let ward = this.listWard.find(x => x.wardId == orgInformation.wardId);
    this.wardControl.setValue(ward ? ward : null);

    let satellite = this.listSatellite.find(x => x.satelliteId == orgInformation.satelliteId);
    this.satelliteControl.setValue(satellite ? satellite : null);

    this.organizationOtherCodeControl.setValue(orgInformation.organizationOtherCode ? orgInformation.organizationOtherCode : '');
  }

  object_code = {
    organizationCode: '',
    geographicalAreaCode : '',
    provinceCode: '',
    districtCode: '',
    wardCode: '',
    satelliteCode: ''
  };

  changeCode(mode: number) {
    if (mode == 1) {
      //Mã đơn vị
      let organizationCode = this.organizationCodeControl.value;
      
      if (organizationCode) {
        this.object_code.organizationCode = organizationCode;
      } else {
        this.object_code.organizationCode = '';
      }

      //Khu vực
      let geographicalArea = this.geographicalAreaControl.value;

      if (geographicalArea) {
        this.object_code.geographicalAreaCode = geographicalArea.geographicalAreaCode;
      } else {
        this.object_code.geographicalAreaCode = '';
      }

      //Tỉnh
      let province = this.provinceControl.value;

      if (province) {
        this.object_code.provinceCode = province.provinceCode;
      } else {
        this.object_code.provinceCode = '';
      }

      //Huyện
      let district = this.districtControl.value;

      if (district) {
        this.object_code.districtCode = district.districtCode;
      } else {
        this.object_code.districtCode = '';
      }

      //Xã
      let ward = this.wardControl.value;

      if (ward) {
        this.object_code.wardCode = ward.wardCode;
      } else {
        this.object_code.wardCode = '';
      }

      //Vệ tinh
      let satellite = this.satelliteControl.value;

      if (satellite) {
        this.object_code.satelliteCode = satellite.satelliteCode;
      } else {
        this.object_code.satelliteCode = '';
      }

      this.reBuildCode(1);
    } else {
      //Mã đơn vị
      let organizationCode = this.orgCodeControl.value;

      if (organizationCode) {
        this.object_code.organizationCode = organizationCode;
      } else {
        this.object_code.organizationCode = '';
      }

      //Khu vực
      let geographicalArea = this.orgGeographicalAreaControl.value;

      if (geographicalArea) {
        this.object_code.geographicalAreaCode = geographicalArea.geographicalAreaCode;
      } else {
        this.object_code.geographicalAreaCode = '';
      }

      //Tỉnh
      let province = this.orgProvinceControl.value;

      if (province) {
        this.object_code.provinceCode = province.provinceCode;
      } else {
        this.object_code.provinceCode = '';
      }

      //Huyện
      let district = this.orgDistrictControl.value;

      if (district) {
        this.object_code.districtCode = district.districtCode;
      } else {
        this.object_code.districtCode = '';
      }

      //Xã
      let ward = this.orgWardControl.value;

      if (ward) {
        this.object_code.wardCode = ward.wardCode;
      } else {
        this.object_code.wardCode = '';
      }

      //Vệ tinh
      let satellite = this.orgSatelliteControl.value;

      if (satellite) {
        this.object_code.satelliteCode = satellite.satelliteCode;
      } else {
        this.object_code.satelliteCode = '';
      }

      this.reBuildCode(2);
    }
  }

  reBuildCode(mode: number) {
    let code = '';
    let organizationCode = this.object_code.organizationCode != '' ? this.object_code.organizationCode.trim() : '';
    let geographicalAreaCode = this.object_code.geographicalAreaCode != '' ? ' - ' + this.object_code.geographicalAreaCode.trim() : '';
    let provinceCode = this.object_code.provinceCode != '' ? ' - ' + this.object_code.provinceCode.trim() : '';
    let districtCode = this.object_code.districtCode != '' ? ' - ' + this.object_code.districtCode.trim() : '';
    let wardCode = this.object_code.wardCode != '' ? ' - ' + this.object_code.wardCode.trim() : '';
    let satelliteCode = this.object_code.satelliteCode != '' ? ' - ' + this.object_code.satelliteCode.trim() : '';

    if (mode == 1) {
      code = organizationCode + geographicalAreaCode + provinceCode + districtCode + wardCode + satelliteCode;

      this.organizationOtherCodeControl.setValue(code);
    } else {
      code = organizationCode + geographicalAreaCode + provinceCode + districtCode + wardCode + satelliteCode;

      this.orgOtherCodeControl.setValue(code);
    }
  }

  /* Mở popup Thêm mới Phòng ban */
  openPoupCreateOrg() {
    if (this.selectedNode) {
      this.awaitResponse = false;
      this.displayModal = true;
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn vị trí phòng ban' };
      this.showMessage(msg);
    }
  }

  /* Đóng popup Thêm mới Phòng ban */
  closePopup() {
    this.displayModal = false;
    this.object_code = {
      organizationCode: '',
      geographicalAreaCode : '',
      provinceCode: '',
      districtCode: '',
      wardCode: '',
      satelliteCode: ''
    }
    this.orgCreateForm.reset();
  }

  /* Thêm mới phòng ban */
  awaitResponse: boolean = false;
  createOrg() {
    if (!this.orgCreateForm.valid) {
      Object.keys(this.orgCreateForm.controls).forEach(key => {
        if (this.orgCreateForm.controls[key].valid == false) {
          this.orgCreateForm.controls[key].markAsTouched();
        }
      });
    } else {
      let organizationName = this.orgNameControl.value;
      let organizationCode = this.orgCodeControl.value;
      let organizationPhone = this.orgPhoneControl.value;
      let organizationAddress = this.orgAddressControl.value;
      let isFinancialIndependence = this.orgFinancialIndependenceControl.value ? this.orgFinancialIndependenceControl.value : false;
      let GeographicalAreaId = this.orgGeographicalAreaControl.value?.geographicalAreaId;
      let ProvinceId = this.orgProvinceControl.value?.provinceId;
      let DistrictId = this.orgDistrictControl.value?.districtId;
      let WardId = this.orgWardControl.value?.wardId;
      let SatelliteId = this.orgSatelliteControl.value?.satelliteId;
      let organizationOtherCode = this.orgOtherCodeControl.value;
      let level = this.selectedNode.data.level + 1;
      let parentId = this.selectedNode.data.organizationId;

      this.awaitResponse = true;
      this.organizationService.createOrganization(organizationName, organizationCode, organizationPhone, 
        organizationAddress, level, parentId, isFinancialIndependence, 
        GeographicalAreaId, ProvinceId, DistrictId, WardId, SatelliteId, 
        organizationOtherCode).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.closePopup();

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm thành công' };
            this.showMessage(msg);

            this.getAllOrganization();
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
    }
  }

  /*Event thay đổi Tỉnh/Thành phố */
  changeProvince(mode: number) {
    if (mode == 1) {
      let toSelectedProvince = this.provinceControl.value;
      this.districtControl.setValue(null);
      this.wardControl.setValue(null);
      if (toSelectedProvince) {
        this.listCurrentDistrict = this.listDistrict.filter(x => x.provinceId == toSelectedProvince.provinceId);
        this.listCurrentWard = [];
      } else {
        this.listCurrentDistrict = [];
        this.listCurrentWard = [];
      }

      this.changeCode(1);
    } else {
      let toSelectedProvince = this.orgProvinceControl.value;
      this.orgDistrictControl.setValue(null);
      this.orgWardControl.setValue(null);
      if (toSelectedProvince) {
        this.listCreCurrentDistrict = this.listDistrict.filter(x => x.provinceId == toSelectedProvince.provinceId);
        this.listCreCurrentWard = [];
      } else {
        this.listCreCurrentDistrict = [];
        this.listCreCurrentWard = [];
      }

      this.changeCode(2);
    }
  }

  /*Event thay đổi Quận/Huyện */
  changeDistrict(mode: number) {
    if (mode == 1) {
      let toSelectedDistrict = this.districtControl.value;
      this.wardControl.setValue(null);
      if (toSelectedDistrict) {
        this.listCurrentWard = this.listWard.filter(x => x.districtId == toSelectedDistrict.districtId);
      } else {
        this.listCurrentWard = [];
      }

      this.changeCode(1);
    } else {
      let toSelectedDistrict = this.orgDistrictControl.value;
      this.orgWardControl.setValue(null);
      if (toSelectedDistrict) {
        this.listCreCurrentWard = this.listWard.filter(x => x.districtId == toSelectedDistrict.districtId);
      } else {
        this.listCreCurrentWard = [];
      }

      this.changeCode(2);
    }
  }
}
