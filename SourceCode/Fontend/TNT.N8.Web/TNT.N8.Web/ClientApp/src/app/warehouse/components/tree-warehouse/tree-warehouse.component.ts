import { Component, OnInit, ElementRef, Inject } from '@angular/core';
//import { MatDialogRef, MAT_DIALOG_DATA, MatSnackBar, MatDialog } from '@angular/material';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { ProductCategoryModel } from '../../../admin/components/product-category/models/productcategory.model';
import { TranslateService } from '@ngx-translate/core';
import { ProductCategoryService } from '../../../admin/components/product-category/services/product-category.service';
import { FailComponent } from '../../../shared/toast/fail/fail.component';

import * as $ from 'jquery';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { WarehouseService } from "../../services/warehouse.service";
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';


@Component({
  selector: 'app-tree-warehouse',
  templateUrl: './tree-warehouse.component.html',
  styleUrls: ['./tree-warehouse.component.css']
})
export class TreeWarehouseComponent implements OnInit {

  productCategoryList: Array<ProductCategoryModel>;
  productCategoryListLevel0: Array<ProductCategoryModel>;
  productCategoryForm: FormGroup;
  warhouseTreeNode: TreeNode[] = [];;
  listWareHouse: Array<any>;
  lstTopInvMapping: Array<any>;
  selectWarhouseId: any;
  selectNameWarhouse: any;
  dataObject: any;
  productId: any;
  constructor(private el: ElementRef,
    private warehouseService: WarehouseService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private confirmationService: ConfirmationService,
    private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.dataObject = this.config.data.object;
    this.productId = this.config.data.productId;
    this.productCategoryForm = new FormGroup({});
    this.getMasterdata();
  }

  async getMasterdata() {
    let resultGetTop10: any = await this.warehouseService.getTop10WarehouseFromReceivingVoucherAsync();
    this.lstTopInvMapping = resultGetTop10.lstInventoryReceivingVoucherMapping;
    let result: any = await this.warehouseService.searchWareHouseAsync();
    this.listWareHouse = result.listWareHouse;
    if (this.productId == null) {
      this.convertToTreeNode(this.listWareHouse);
    } else {
      this.convertToTreeNode(this.listWareHouse);
    }
  }

  
  convertToTreeNode(listWareHouse: any[]) {
    for (var i = 0; i < listWareHouse.length; ++i) {
      if (listWareHouse[i].warehouseParent == null && listWareHouse[i].warehouseId == this.dataObject) {
        this.warhouseTreeNode.push(this.generateTreeStructure(listWareHouse[i]));
      }
    }
  }

  generateTreeStructure(parentNode: any): TreeNode {
    let parentNodeChildren: TreeNode[] = [];
    let item: TreeNode = {
      label: parentNode.warehouseName,
      data: parentNode.warehouseId,
      expandedIcon: "fa fa-folder-open",
      collapsedIcon: "fa fa-folder",
      expanded:true,
      children: []
    };
    this.listWareHouse.forEach(item => {
      if (parentNode.warehouseId === item.warehouseParent) {
        let childNode: TreeNode = {
          label: item.warehouseName,
          data: item.warehouseId,
          expanded: true,
          expandedIcon: "fa fa-folder-open",
          collapsedIcon: "fa fa-folder",
          children: []
        };
        childNode = this.generateTreeStructure(item);
        parentNodeChildren.push(childNode);
      }
    });
    item.children.push(...parentNodeChildren);
    return item;
  }

  convertToTreeNode2(listWareHouse: any[]) {
    for (var i = 0; i < listWareHouse.length; ++i) {
      if (listWareHouse[i].warehouseParent == null && listWareHouse[i].warehouseId == this.dataObject) {
        this.warhouseTreeNode.push(this.generateTreeStructure2(listWareHouse[i], this.dataObject, this.lstTopInvMapping, this.productId));
      }
    }
  }

  generateTreeStructure2(parentNode: any, warehouseRoot: any, lstTopInvMapping: Array<any>, ProductId:any): TreeNode {
    let parentNodeChildren: TreeNode[] = [];
    let item: TreeNode;
    if (parentNode.warehouseId == warehouseRoot) {
      item = {
        label: parentNode.warehouseName,
        data: parentNode.warehouseId,
        expandedIcon: "fa fa-folder-open",
        collapsedIcon: "fa fa-folder",
        expanded: true,
        children: []
      };
    }
    else {
      let checkexist = lstTopInvMapping.find(f => f.warehouseId == parentNode.warehouseId && f.productId == ProductId);
      if (checkexist != null) {
        item = {
          label: parentNode.warehouseName,
          data: parentNode.warehouseId,
          expandedIcon: "fa fa-folder-open",
          collapsedIcon: "fa fa-folder",
          expanded: true,
          children: []
        };
      }
      else {
        item = {
          label: parentNode.warehouseName,
          data: parentNode.warehouseId,
          expandedIcon: "fa fa-folder-open",
          collapsedIcon: "fa fa-folder",
          expanded: true,
          selectable: false,
          children: []
        };
      }

    }
    this.listWareHouse.forEach(item => {
      if (parentNode.warehouseId === item.warehouseParent) {
        let childNode: TreeNode;
        let checkexist = lstTopInvMapping.find(f => f.warehouseId == item.warehouseId && f.productId == ProductId);
        if (checkexist != null) {
          childNode = {
            label: item.warehouseName,
            data: item.warehouseId,
            expanded: true,
            expandedIcon: "fa fa-folder-open",
            collapsedIcon: "fa fa-folder",
            children: []
          };
        }
        else {
          childNode = {
            label:'<span style=color:#dddd>'+item.warehouseName+'</span>',
            data: item.warehouseId,
            expanded: true,
            expandedIcon: "fa fa-folder-open",
            collapsedIcon: "fa fa-folder",
            selectable: false,
            children: []
          };
        }
        childNode = this.generateTreeStructure2(item, warehouseRoot, lstTopInvMapping, ProductId);
        parentNodeChildren.push(childNode);

      }
    });
    item.children.push(...parentNodeChildren);
    return item;
  }

  closeDialog() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.ref.close();
      },
      reject: () => {
        return;
      }
    });

  }
  choseWarhouse() {
    this.ref.close({ 'warhouseId': this.selectWarhouseId, 'warhousename': this.selectNameWarhouse });

  }
  nodeSelect(event) {
    this.selectWarhouseId = event.node.data;
    this.selectNameWarhouse = event.node.label;
  }
  nodeUnselect(event) {
    this.selectWarhouseId = '';
    this.selectNameWarhouse = '';
  }
}
