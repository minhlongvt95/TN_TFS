import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { CategoryService } from "../../../shared/services/category.service";
import { ProvinceService } from "../../../shared/services/province.service";
import { TranslateService } from '@ngx-translate/core';
import { MasterdataDialogComponent } from '../masterdata-dialog/masterdata-dialog.component';

import { EmployeeService } from '../../../employee/services/employee.service';
import { Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';


interface CategoryTypeModel {
  categoryTypeId: string,
  categoryTypeName: string;
  categoryTypeCode: string;
  categoryList: Array<CategoryModel>;
  active: boolean,
}

interface CategoryModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
  categoryTypeId: CategoryTypeModel["categoryTypeId"];
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: string;
  active: Boolean;
  isEdit: Boolean;
  isDefault: Boolean;
  statusName: string;
  potentialName: string;
  categoryTypeName: string;
}

@Component({
  selector: 'app-masterdata',
  templateUrl: './masterdata.component.html',
  styleUrls: ['./masterdata.component.css']
})
export class MasterdataComponent implements OnInit {

  masterDataCodeDisplay: string; // dialog
  masterDataParentDisplay: string; // dialog
  selectedCategoryTypeId: string; // dialog
  selectedCategoryId: string; // dialog
  selectedCategoryName: string; // dialog

  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  // longhdh
  categoryTypeModellist: Array<CategoryTypeModel> = [];
  categoryTypeModel: CategoryTypeModel;
  selectedCategoryType = new FormControl('');
  colHeader: any[];
  categoryModelDefaultList: Array<CategoryModel> = [];
  categoryModelEditList: Array<CategoryModel> = [];
  checkItem: Array<CategoryModel> = [];
  selectAllDefault: boolean;
  selectAllEdit: boolean;
  displayModal: boolean = false;

  constructor(
    private employeeService: EmployeeService,
    private getPermission: GetPermission,
    private router: Router,
    private categoryService: CategoryService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private translate: TranslateService,
    private confirmationService: ConfirmationService,
  ) {
    this.translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.initTable();

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getMasterData();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/masterdata/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.messageService.add({ severity: 'warn', summary: 'Warning Message', detail: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ" })
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("add") == -1) {
            this.actionAdd = false;
          } if (listCurrentActionResource.indexOf("edit") == -1) {
            this.actionEdit = false;
          } if (listCurrentActionResource.indexOf("delete") == -1) {
            this.actionDelete = false;
          }
          this.getMasterData();
        }
      }
    }, error => { });
  }

  /* #region  tạo header cho table */
  initTable() {
    this.colHeader = [
      { field: 'categoryName', header: 'Tên dữ liệu', textAlign: 'left', display: 'table-cell', colWidth: 'auto' },
      { field: 'categoryCode', header: 'Mã dữ liệu', textAlign: 'left', display: 'table-cell', colWidth: '15%' },
    ];
  }
  /* #endregion */

  getMasterData() {
    this.categoryService.getAllCategory().subscribe(res => {
      let result = <any>res;
      this.categoryTypeModellist = result.categoryTypeList;
      this.selectedCategoryType.setValue(this.categoryTypeModellist[0]);
      this.categoryTypeModel = this.selectedCategoryType.value;
      this.categoryModelDefaultList = this.categoryTypeModellist[0].categoryList.filter(c => c.isEdit == false);
      this.categoryModelEditList = this.categoryTypeModellist[0].categoryList.filter(c => c.isEdit == true);
      this.checkItem = this.categoryTypeModellist[0].categoryList.filter(c => c.isDefault == true);
      this.selectAllDefault = this.categoryModelDefaultList.filter(c => c.active == true).length == this.categoryModelDefaultList.length ? true : false;
      this.selectAllEdit = this.categoryModelEditList.filter(c => c.active == true).length == this.categoryModelEditList.length && this.categoryModelEditList.length != 0 ? true : false;
    });
  }

  /* #region  is all default category selected */
  isAllDefaultSelected() {
    let numcategoryDefault = this.categoryModelDefaultList.length;
    let numcategoryDefaultSelected = this.categoryModelDefaultList.filter(c => c.active == true).length;
    return numcategoryDefaultSelected == numcategoryDefault
  }
  /* #endregion */

  /* #region  checkbox cho hoạt động của dự liệu mặc định */
  masterToggleDdefault() {
    if (this.isAllDefaultSelected()) {
      this.categoryModelDefaultList.forEach(val => {
        val.active = false;
      })
    } else {
      this.categoryModelDefaultList.forEach(val => {
        if (!val.active) {
          val.active = true;
        }
      })
    }
    this.updateActiveDefault("all");
  }
  /* #endregion */

  /* #region  is all default category selected */
  isAllEditSelected() {
    let numcategoryEdit = this.categoryModelEditList.length;
    let numcategoryEditSelected = this.categoryModelEditList.filter(c => c.active == true).length;
    return numcategoryEditSelected == numcategoryEdit
  }
  /* #endregion */

  /* #region  checkbox cho hoạt động của dự liệu tùy chỉnh */
  masterToggleEdit() {
    if (this.isAllEditSelected()) {
      this.categoryModelEditList.forEach(val => {
        val.active = false;
      })
    } else {
      this.categoryModelEditList.forEach(val => {
        if (!val.active) {
          val.active = true;
        }
      })
    }
    this.updateActiveEdit("all");
  }
  /* #endregion */

  /* #region  update active cho dữ liệu mặc định */
  updateActiveDefault(id: string) {
    if (id === "all") {
      this.categoryModelDefaultList.forEach(val => {
        if (val.categoryId != null || val.categoryId != '') {
          this.categoryService.updateStatusIsActive(val.categoryId, val.active).subscribe(res => { });
        }
      })
    } else {
      if (this.isAllDefaultSelected()) {
        this.selectAllDefault = true;
      } else {
        this.selectAllDefault = false;
      }
      let active = this.categoryModelDefaultList.find(c => c.categoryId == id).active;
      this.categoryService.updateStatusIsActive(id, active).subscribe(res => { });
    }
  }
  /* #endregion */

  /* #region  update active cho dữ liệu tùy chỉnh */
  updateActiveEdit(id: string) {
    if (id === "all") {
      this.categoryModelEditList.forEach(val => {
        if (val.categoryId != null || val.categoryId != '') {
          this.categoryService.updateStatusIsActive(val.categoryId, val.active).subscribe(res => { });
        }
      })
    } else {
      this.selectAllEdit = this.isAllEditSelected() ? true : false;
      let active = this.categoryModelEditList.find(c => c.categoryId == id).active;
      this.categoryService.updateStatusIsActive(id, active).subscribe(res => { });
    }
  }
  /* #endregion */

  /* #region  update isDefault cho categoryModel */
  updateIsDefault(id: string) {
    this.categoryModelDefaultList.forEach(val => {
      val.isDefault = false;
    });
    this.categoryModelDefaultList.find(c => c.categoryId == id).isDefault = true;
    let cagetoryTypeId = this.categoryModelDefaultList.find(c => c.categoryId == id).categoryTypeId;
    this.categoryService.updateStatusIsDefault(id, cagetoryTypeId).subscribe(res => { });
  }
  /* #endregion */

  /* #region  delete một dữ liệu tùy chỉnh */
  deleteMasterDataById(id: string, count: number) {
    if (count !== 0) {
      this.displayModal = true;
    } else {

      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn xóa?',
        accept: () => {
          this.selectedCategoryId = id;
          this.categoryService.deleteCategoryById(this.selectedCategoryId).subscribe(res => {
            let result = <any>res;
            if (result.statusCode === 202 || result.statusCode === 200) {
              this.messageService.add({ severity: "success", summary: "Success Message", detail: result.messageCode })
              this.getMasterData();
            } else {
              this.messageService.add({ severity: "error", summary: "Error Message", detail: result.messageCode })
            }
          });
        }
      });
    }
  }
  /* #endregion */

  /* #region  mở dialog thêm hoặc sửa một category tùy chỉnh */
  openMasterDataDialog(mode: string, id: string) {
    if (id != "create") {
      var getbyId = this.categoryTypeModel.categoryList.find(c => c.categoryId == id);
      this.selectedCategoryId = id;
      this.selectedCategoryName = getbyId.categoryName;
      this.masterDataCodeDisplay = getbyId.categoryCode;
    }
    this.selectedCategoryTypeId = this.categoryTypeModel.categoryTypeId;
    this.masterDataParentDisplay = this.categoryTypeModellist.find(t => t.categoryTypeId == this.selectedCategoryTypeId).categoryTypeName;
    let ref = this.dialogService.open(MasterdataDialogComponent, {
          header: 'THÔNG TIN DỮ LIỆU',
          width: '600px',
          baseZIndex: 1030,
          contentStyle: {
            "min-height": "190px",
            "max-height": "600px",
            "overflow": "auto"
          },
          data: { selectedCategoryId: this.selectedCategoryId, selectedCategoryName: this.selectedCategoryName, mode: mode, selectedCategoryCode: this.masterDataCodeDisplay, selectedCategoryType: this.masterDataParentDisplay, selectedCategoryTypeId: this.selectedCategoryTypeId }
        });

    ref.onClose.subscribe(result => {
      if (result) {
        if (result.categoryCreated || result.categoryEdited) {
          this.messageService.add({ severity: "success", summary: "Success Message", detail: result.message })
          this.getMasterData();
        }
      }
    });
  }
  /* #endregion */

  selectItemChange() {
    this.categoryTypeModel = this.selectedCategoryType.value;
    this.categoryModelDefaultList = this.categoryTypeModel.categoryList.filter(c => c.isEdit == false);
    this.categoryModelEditList = this.categoryTypeModel.categoryList.filter(c => c.isEdit == true);
    this.checkItem = this.categoryTypeModel.categoryList.filter(c => c.isDefault == true);
    this.selectAllDefault = this.categoryModelDefaultList.filter(c => c.active == true).length == this.categoryModelDefaultList.length ? true : false;
    this.selectAllEdit = this.categoryModelEditList.filter(c => c.active == true).length == this.categoryModelEditList.length && this.categoryModelEditList.length != 0 ? true : false;
  }
}
