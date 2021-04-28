import { Component, OnInit, ViewChild } from '@angular/core';
import { TreeViewComponent, NodeSelectEventArgs } from "@syncfusion/ej2-angular-navigations"
import { Router, ActivatedRoute } from '@angular/router';

/*COMPONENTS*/
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { WarehouseCreateUpdateComponent } from '../warehouse-create-update/warehouse-create-update.component';
/*END*/

/*MODELS*/
import { WarehouseModel } from '../../../models/warehouse.model';
/*END*/

/*SERVICES*/
import { GetPermission } from '../../../../shared/permission/get-permission'
import { WarehouseService } from "../../../services/warehouse.service";
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
/*END*/

class warehouseModel {
  warehouseName: string;
  warehouseAddress: string;
  canAddChild: boolean;
  canRemove: boolean;
  hasChild: boolean;
  storagekeeper: string;
  storagekeeperName: string;
  warehouseId: string;
  warehouseCode: string;
  warehouseDescription: string;
  warehouseParent: string;
}

@Component({
  selector: 'app-warehouse-list',
  templateUrl: './warehouse-list.component.html',
  styleUrls: ['./warehouse-list.component.css'],
  providers: [DialogService, MessageService, ConfirmationService]
})

export class WarehouseListComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  loading: boolean = false;
  //action variables
  actionAdd: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  //tree variables
  selectedWarehouse: Array<any> = [];
  childOfSelectedWarehouse: Array<any> = [];
  //table
  cols: any;
  selectedColumns: any;
  colsChild: any;
  selectedColumnsChild: any;
  listDetailWarehouse: Array<warehouseModel> = [];
  listChildWarehouse: Array<warehouseModel> = [];
  //master data
  listWareHouse: Array<warehouseModel> = [];
  @ViewChild('tree')
  public tree: TreeViewComponent;
  public listfields: Object;
  public loadRoutingContent(args: NodeSelectEventArgs): void {
    let data: any = this.tree.getTreeData(args.node)[0];
    this.listDetailWarehouse = [data];
    this.listChildWarehouse = this.listWareHouse.filter(e => e.warehouseParent == data.warehouseId);
  }

  constructor(
    private router: Router,
    private warehouseService: WarehouseService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    public messageService: MessageService,
    private getPermission: GetPermission,
  ) { }

  ngOnInit() {
    let resource = "war/warehouse/list";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let msg = {
        severity: 'warn',
        summary: 'Thông báo:',
        detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ'
      };
      this.router.navigate(['/home']);
    } else {
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
      this.initTable();
      this.getMasterdata();
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  initTable() {
    this.cols = [
      { field: 'warehouseName', header: 'Tên', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'warehouseCode', header: 'Mã', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'warehouseAddress', header: 'Địa chỉ', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'warehousePhone', header: 'Điện thoại', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'storagekeeperName', header: 'Thủ kho', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Chức năng', width: '50px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumns = this.cols;

    this.colsChild = [
      { field: 'warehouseName', header: 'Tên', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'warehouseCode', header: 'Mã', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'warehouseAddress', header: 'Địa chỉ', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'warehousePhone', header: 'Điện thoại', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'storagekeeperName', header: 'Thủ kho', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Chức năng', width: '50px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumnsChild = this.cols;
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.warehouseService.searchWareHouseAsync();
    this.loading = false;
    if (result.statusCode === 200) {
      this.listWareHouse = result.listWareHouse;
      this.listfields = { dataSource: this.listWareHouse, id: 'warehouseId', parentID: 'warehouseParent', text: 'warehouseName', hasChildren: 'hasChild' };
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  resetState() {
    this.listWareHouse = [];
    this.listDetailWarehouse = [];
    this.listChildWarehouse = [];
  }

  addWareHouse(rowData: warehouseModel) {
    let header: string;
    let listWarehouseNameEqualLevel: Array<string> = [];
    let parentId: string;
    if (rowData == null) {
      listWarehouseNameEqualLevel = this.listWareHouse.filter(e => e.warehouseParent == null).map(e => e.warehouseName);
      parentId = null;
      header = 'Thêm kho';
    } else {
      listWarehouseNameEqualLevel = this.listWareHouse.filter(e => e.warehouseParent == rowData.warehouseId).map(e => e.warehouseName);
      parentId = rowData.warehouseId;
      header = 'Thêm vị trí';
    }

    let ref = this.dialogService.open(WarehouseCreateUpdateComponent, {
      header: header,
      data: {
        isEdit: false,
        parentId: parentId,
        listWarehouseNameEqualLevel: listWarehouseNameEqualLevel
      },
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          //tao thanh cong
          this.resetState();
          this.getMasterdata();
          this.clearToast();
          this.showToast('success', 'Thông báo', result.message);
        } else {
          //tao that bai
          this.clearToast();
          this.showToast('error', 'Thông báo', result.message);
        }
      }
    });
  }

  editWarehouse(rowData: warehouseModel) {
    let header: string;
    if (rowData.warehouseParent == null) {
      header = 'Chỉnh sửa kho'
    } else {
      header = 'Chỉnh sửa vị trí'
    }

    let listWarehouseNameEqualLevel: Array<string> = [];
    listWarehouseNameEqualLevel = this.listWareHouse.filter(e => e.warehouseParent == rowData.warehouseParent).map(e => e.warehouseName);
    let ref = this.dialogService.open(WarehouseCreateUpdateComponent, {
      header: header,
      data: {
        isEdit: true,
        parentId: rowData.warehouseParent,
        editedWarehouseModel: rowData,
        listWarehouseNameEqualLevel: listWarehouseNameEqualLevel
      },
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          //edit thanh cong
          this.resetState();
          this.getMasterdata();
          this.clearToast();
          this.showToast('success', 'Thông báo', result.message);
        } else {
          //edit that bai
          this.clearToast();
          this.showToast('error', 'Thông báo', result.message);
        }
      }
    });
  }

  removeWarehouse(rowData: warehouseModel) {
    this.confirmationService.confirm({
      message: 'Các dữ liệu sẽ bị xoá. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn xoá?',
      accept: () => {
        this.loading = true;
        this.warehouseService.removeWareHouse(rowData.warehouseId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.resetState();
            this.getMasterdata();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        }, () => this.loading = false);
      }
    });
  }
}


