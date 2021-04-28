import { Component, OnInit, ElementRef, Inject } from '@angular/core';

import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { FormGroup } from '@angular/forms';
import { ProductCategoryModel } from '../../../../admin/components/product-category/models/productcategory.model';
import { ProductCategoryService } from '../../../../admin/components/product-category/services/product-category.service';
import * as $ from 'jquery';

export interface IDialogData {
  selectedproductCategoryId: string;
  selectedName: string;
}

@Component({
  selector: 'app-product-category-popup',
  templateUrl: './product-category-popup.component.html',
  styleUrls: ['./product-category-popup.component.css']
})

export class ProductCategoryPopupComponent implements OnInit {

  productCategoryList: Array<ProductCategoryModel>;
  productCategoryListLevel0: Array<ProductCategoryModel>;
  productCategoryForm: FormGroup;

  constructor(private el: ElementRef,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<ProductCategoryPopupComponent>,
    private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.productCategoryForm = new FormGroup({});   
    this.getAllCategory();
  }
  getAllCategory() {
    this.productCategoryService.getAllProductCategory().subscribe(response => {
      let result = <any>response;
      this.productCategoryList = result.productCategoryList.filter(item => item.productCategoryLevel == 0);//result.productCategoryList;
      //this.productCategoryListLevel0 = result.productCategoryList.filter(item => item.productCategoryLevel == 0);
    }, error => { });
    
    
    setTimeout(() => {
      this.checkSelected();
      $(".tree-item table td:last-child").click((evt) => {
        $(".tree-item table td:last-child").removeClass('selected-item');
        $(evt.currentTarget).toggleClass('selected-item');
        this.data.selectedproductCategoryId = $(evt.currentTarget).closest('table').attr('id');
        this.data.selectedName = $(evt.currentTarget).closest('table').attr('title');
       
      });
    }, 500);
  }

  checkSelected(){
    const element = $(".tree-item table td:last-child");
    for (let i=0; i<element.length;i++){
      if ($(element[i]).closest('table').attr('id') === this.data.selectedproductCategoryId){
        $(element[i]).toggleClass('selected-item');
        break;
      }
    }
  }

  hasChild(prodC): boolean {
    return ((prodC.productCategoryChildList.length > 0) ? true : false);
  }

  changeIcon(event) {
    const icon = $(event.target).text().trim();
    $(event.target).text(icon === 'arrow_drop_down' ? 'arrow_right' : 'arrow_drop_down');
  }

  onSaveClick() {
    this.dialogRef.close(this.data);
  }
  onCancelClick() {
    this.dialogRef.close();
  }

}
