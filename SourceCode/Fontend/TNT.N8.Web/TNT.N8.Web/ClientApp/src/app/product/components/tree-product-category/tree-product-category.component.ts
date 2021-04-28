import { Component, OnInit, ElementRef, Inject, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ProductCategoryService } from '../../../admin/components/product-category/services/product-category.service';

/* Primeng API */
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
/* end PrimeNg API */

import { TreeViewComponent, NodeSelectEventArgs } from "@syncfusion/ej2-angular-navigations"

export interface ResultDialog {
  status: boolean;
  productCategory: productCategoryModel;
}

class productCategoryModel {
  public productCategoryId: string;
  public productCategoryCode: string;
  public productCategoryName: string;
  public productCategoryLevel: string;
  public parentId: string;
  public hasChildren: boolean;
  public productCategoryNameByLevel: string; //tên theo phân cấp
  public productCategoryListNameByLevel: Array<string>;
  constructor() {
    this.productCategoryListNameByLevel = [];
  }
}

@Component({
  selector: 'app-tree-product-category',
  templateUrl: './tree-product-category.component.html',
  styleUrls: ['./tree-product-category.component.css'],
  providers: [DialogService]
})
export class TreeProductCategoryComponent implements OnInit {
  loading: boolean = false;
  @ViewChild('tree', { static: true }) public tree: TreeViewComponent;
  productCategoryList: Array<productCategoryModel>;
  selectedProductCategory: productCategoryModel;

  public listfields: Object

  constructor(private el: ElementRef,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    public dialogService: DialogService,
    private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.getAllCategory();
  }

  getAllCategory() {
    this.loading = true;
    this.productCategoryService.getAllProductCategory().subscribe(response => {
      this.loading = false;
      let result = <any>response;

      this.productCategoryList = result.productCategoryList;
      this.productCategoryList.forEach((item: any) => {
        if (item.productCategoryChildList == null) {
          item.hasChildren = false;
        } else {
          if (item.productCategoryChildList.length === 0) {
            item.hasChildren = false;
          } else {
            item.hasChildren = true;
          }
        }
      });
      this.listfields = { dataSource: this.productCategoryList, id: 'productCategoryId', parentID: 'parentId', text: 'productCategoryName', hasChildren: 'hasChildren' };
    }, error => { this.loading = false; });
  }

  public loadRoutingContent(args: NodeSelectEventArgs): void {
    let data: any = this.tree.getTreeData(args.node)[0];
    this.selectedProductCategory = this.productCategoryList.find(e => e.productCategoryId == data.productCategoryId);
    let listParentId = this.getListParentId(this.selectedProductCategory, []);
    let listParentProductCategory = this.productCategoryList.filter(e => listParentId.includes(e.productCategoryId)).map(e => e.productCategoryName);
    this.selectedProductCategory.productCategoryNameByLevel = listParentProductCategory.join(' > ');
  }

  getListParentId(currentProductCategory: productCategoryModel, listParentIdReponse: Array<string>): Array<string> {
    listParentIdReponse.push(currentProductCategory.productCategoryId);
    let parent = this.productCategoryList.find(e => e.productCategoryId == currentProductCategory.parentId);
    if (parent === undefined) return listParentIdReponse;
    listParentIdReponse.push(parent.productCategoryId);
    //find parent of parent
    let parentOfParent = this.productCategoryList.find(e => e.productCategoryId == parent.parentId);
    if (parentOfParent === undefined) {
      return listParentIdReponse;
    } else {
      this.getListParentId(parentOfParent, listParentIdReponse);
    }
    return listParentIdReponse;
  }

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      productCategory: null
    };
    this.ref.close(result);
  }
  /*End*/

  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      productCategory: this.selectedProductCategory
    };
    this.ref.close(result);
  }

}
