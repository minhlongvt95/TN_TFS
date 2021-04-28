import { Component, OnInit, ElementRef } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { CreateComponent } from '../create/create.component';
import { SuccessComponent } from '../../../../../shared/toast/success/success.component'; 
import { FailComponent } from '../../../../../shared/toast/fail/fail.component';
import { ProductCategoryService } from '../../services/product-category.service'
import { ProductCategoryModel } from '../../models/productcategory.model'
import { PopupComponent } from '../../../../../shared/components/popup/popup.component';
import * as $ from 'jquery'
import { GetPermission } from '../../../../../shared/permission/get-permission';
import { WarningComponent } from '../../../../../shared/toast/warning/warning.component';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-list-product-category',
  templateUrl: './list-product-category.component.html',
  styleUrls: ['./list-product-category.component.css']
})
export class ListProductCategoryComponent implements OnInit {
  displayedColumns: string[] = ['name','id','description','status'];   
  productCategoryList: Array<ProductCategoryModel>=[];
  productCategoryCodeList: Array<any>;
  Keyword_name: string = "";
  Keyword_code: string = "";

  /* Action*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /*END*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(private el: ElementRef, private router: Router,
    private getPermission: GetPermission,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private productCategoryService: ProductCategoryService
  ) { }
  dialogCreateEdit: MatDialogRef<CreateComponent>;
  dialogConfirm: MatDialogRef<PopupComponent>;

  ngOnInit() {
    //Check permission
    let resource = "sal/admin/list-product-category";
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
      this.getAllCategory();
    }
  }
  changeicon(event) {
    let curText = $(event.target).text().trim();
    if (curText === 'arrow_right' || curText === 'arrow_drop_down') $(event.target).text(curText === 'arrow_right' ? 'arrow_drop_down' : 'arrow_right');
    if (curText === 'add' || curText === 'remove') $(event.target).text(curText === 'add' ? 'remove' : 'add'); 
    if (this.expandedAll()) $("#headerCell_icontrigger .icon_triger").text('remove');
    if (this.collapsedAll()) $("#headerCell_icontrigger .icon_triger").text('add');
  }

  getAllCategory() {
    this.productCategoryService.getAllProductCategory().subscribe(response => {
      let result = <any>response;
      this.productCategoryList = result.productCategoryList.filter(item => item.productCategoryLevel == 0);//result.productCategoryList;

      //for (var x = 0; x < result.productCategoryList.length; ++x) {
      //  if (result.productCategoryList[x].parentId === null || result.productCategoryList[x].parentId === '') {
      //    this.productCategoryList.push(result.productCategoryList[x]);
      //  }
      //};
      //this.productCategoryList = result.productCategoryList;
    }, error => {});
  }

  searchProductCategory() {
    this.productCategoryService.searchProductCategory(this.Keyword_name,this.Keyword_code).subscribe(response => {
      let result = <any>response;
      this.productCategoryList = result.productCategoryList;
    })
  }
  
  expandedAll():boolean{
    var result = true;
    for (var i=0 ; i<$(".body_table .icon_triger").length ; i++){
      if ($($(".body_table .icon_triger")[i]).text().trim() === "add"){
        result = false;
        break;
      }
    }
    return result;
  }

  collapsedAll():boolean{
    var result = true;
    for (var i=0 ; i<$(".parent .icon_triger").length ; i++){
      if ($($(".parent .icon_triger")[i]).text().trim() === "remove"){
        result = false;
        break;
      }
    }
    return result;
  }

  masterToggle(event) {
    if ($(event.target).text().trim() === "add") {
      for (var i=0 ; i<$(".body_table .icon_triger").length ; i++){
        if ($($(".body_table .icon_triger")[i]).text().trim() === "add"){
          $($(".body_table .icon_triger")[i]).click();
        }
      }
    } else {
      for (var i=0 ; i<$(".body_table .icon_triger").length ; i++){
        if ($($(".body_table .icon_triger")[i]).text().trim() === "remove"){
          $($(".body_table .icon_triger")[i]).click();
        }
      }
    }
  }
  onClickButtonCreate(mode: string, selectedproductcategoryid: string, selectedproductcategoryname: string,level: number) {
    this.dialogCreateEdit = this.dialog.open(CreateComponent,
        {
          width: '550px',
          height: '580',
          autoFocus: false,
          data: {
            selectedproductcategoryname: selectedproductcategoryname, selectedproductcategoryid: selectedproductcategoryid,level: level, mode: mode }
        });
      this.dialogCreateEdit.afterClosed().subscribe(result => {
        if (result.productcategoryCreated || result.productcategoryEdited) {
          this.snackBar.openFromComponent(SuccessComponent,
            {
              duration: 5000,
              data: result.message,
              panelClass: 'success-dialog',
              horizontalPosition: 'end'
            });

          //if (result.orgCreatedId !== "" && result.orgCreatedId !== null) {
          //  this.selectedOrgId = result.orgCreatedId;
          //  this.reloadAfterCreate = true;
          //  this.getAllOrganization();
          //}
        }
        this.getAllCategory();
        $("#headerCell_icontrigger .icon_triger").text('remove');
      } ,error => {
        let result = <any>error;
        this.snackBar.openFromComponent(FailComponent,
          {
            duration: 5000,
            data: result.messageCode,
            panelClass: 'fail-dialog',
            horizontalPosition: 'end'
          });
      });
    

  }
  del_productCategory(productCategoryId: any) {
    let _title = "XÁC NHẬN";
    let _content = "Bạn có chắc chắn muốn xóa?";
    this.dialogConfirm = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: '300px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });

    this.dialogConfirm.afterClosed().subscribe(resultPopup => {
      if (resultPopup) {
        this.productCategoryService.updateActiveProductCategory(productCategoryId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.snackBar.openFromComponent(SuccessComponent,
              {
                duration: 5000,
                data: result.messageCode,
                panelClass: 'success-dialog',
                horizontalPosition: 'end'
              });
            this.ngOnInit();
          } else {
            this.snackBar.openFromComponent(FailComponent,
              {
                duration: 5000,
                data: result.messageCode,
                panelClass: 'fail-dialog',
                horizontalPosition: 'end'
              });
          }
          this.ngOnInit();
        }, error => { });
        this.dialogConfirm.close();
      }
    });
  }

}
