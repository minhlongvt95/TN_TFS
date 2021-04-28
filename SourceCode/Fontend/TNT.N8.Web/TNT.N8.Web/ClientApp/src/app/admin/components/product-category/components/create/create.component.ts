import { Component, OnInit, ElementRef, Inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { ProductCategoryModel } from '../../models/productcategory.model';
import { TranslateService } from '@ngx-translate/core';
import { ProductCategoryService } from '../../services/product-category.service';
import { FailComponent } from '../../../../../shared/toast/fail/fail.component';  
import { PopupComponent } from '../../../../../shared/components/popup/popup.component';


import * as $ from 'jquery';
import { debounce } from 'rxjs/operators';

export interface IDialogData {
  //selectedOrgId: string;
  //selectedOrgName: string;
  productcategoryCreated: boolean;
  productcategoryCreatedId: string;
  productcategoryEdited: boolean;
  selectedproductcategoryid: string;
  productCategoryCodeList: Array<string>;

  level: number;
  selectedproductcategoryname: string;
  mode: string;
  message: string;
}

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  productCategoryCodeListX: Array<any>;
  productCategoryList: Array<ProductCategoryModel>;
  dialogPopup: MatDialogRef<PopupComponent>;

  productCategoryForm: FormGroup;
  productCategoryName: FormControl;
  productCategoryParentID: FormControl;
  productCategoryCode: FormControl;
  productCategoryDescription: FormControl;

  constructor(private el: ElementRef,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<CreateComponent>,
    public snackBar: MatSnackBar,
    public dialogPop: MatDialog,
    private productCategoryService: ProductCategoryService) {
    translate.setDefaultLang('vi');
  }
  productCategoryModel: ProductCategoryModel = {
    ProductCategoryId: '',
    Name: '',
    Level: 0,
    ParentID: null,
    Description: '',
    ProductCategoryCode: '',
    Active: true,
    ProductCategoryChildList: null,
  }

  async ngOnInit() {
    await this.getMasterData();
    const prt = '[a-zA-Z0-9]*';
    this.productCategoryName = new FormControl('', [Validators.required]);
    this.productCategoryCode = new FormControl('', [Validators.required, checkDuplicateCode(this.productCategoryCodeListX), Validators.pattern(prt)]);
    this.productCategoryDescription = new FormControl('');
    this.productCategoryParentID = new FormControl('');

    this.productCategoryForm = new FormGroup({
      productCategoryName: this.productCategoryName,
      productCategoryParentID: this.productCategoryParentID,
      productCategoryCode: this.productCategoryCode,
      productCategoryDescription: this.productCategoryDescription
    });

     if (this.data.mode === 'edit') {
      this.productCategoryService.getProductCategoryById(this.data.selectedproductcategoryid).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.productCategoryModel = <ProductCategoryModel>({
            ProductCategoryId: result.productCategory.productCategoryId,
            Name: result.productCategory.productCategoryName,
            Level: result.productCategory.productCategoryLevel,
            ParentID: result.productCategory.parentId,
            Description: result.productCategory.description,
            ProductCategoryCode: result.productCategory.productCategoryCode,
          });
          //var io = result.productCategory.productCategoryCode.toLowerCase();
          var indexEdit = this.productCategoryCodeListX.indexOf(result.productCategory.productCategoryCode.toLowerCase());
          this.productCategoryCodeListX.splice(indexEdit, 1);
        }
      }, error => { });
    }

  }
  async getMasterData() {
    var result: any = await this.productCategoryService.getListCodeAsync();
    this.productCategoryCodeListX = result.productCategoryCodeList;
  }
  createProductCategory() {
    if (!this.productCategoryForm.valid) {
      Object.keys(this.productCategoryForm.controls).forEach(key => {
        if (this.productCategoryForm.controls[key].valid === false) {
          this.productCategoryForm.controls[key].markAsTouched();
        }
      });
      let target;
      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      var parentID = this.data.selectedproductcategoryid == null ? "" : this.data.selectedproductcategoryid;
      var OutLevel = this.data.level == null ? 0 : this.data.level+1; 
      this.productCategoryService.createProductCategory(this.productCategoryModel.Name, OutLevel,
        this.productCategoryModel.ProductCategoryCode, this.productCategoryModel.Description,
        parentID,
      ).subscribe(response => {
        let result = <any>response;
        this.data.message = result.messageCode;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.data.productcategoryCreated = true;
          this.data.productcategoryCreatedId = result.ProductCategoryID;
          this.dialogRef.close(this.data);
        } else {
          this.data.productcategoryCreated = false;
          this.dialogRef.close(this.data);
        }
      }, error => {
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

  }
  editProductCategory() {
    if (!this.productCategoryForm.valid) {
      Object.keys(this.productCategoryForm.controls).forEach(key => {
        if (this.productCategoryForm.controls[key].valid === false) {
          this.productCategoryForm.controls[key].markAsTouched();
        }
      });
      let target;
      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      var parentID = this.data.selectedproductcategoryid == null ? "" : this.data.selectedproductcategoryid;
      this.productCategoryService.editProductCategory(parentID,this.productCategoryModel.Name,
        this.productCategoryModel.ProductCategoryCode, this.productCategoryModel.Description
      ).subscribe(response => {
        let result = <any>response;
        this.data.message = result.messageCode;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.data.productcategoryCreated = true;
          this.data.productcategoryCreatedId = result.ProductCategoryID;
          this.dialogRef.close(this.data);
        } else {
          this.data.productcategoryCreated = false;
          this.dialogRef.close(this.data);
        }
      }, error => {
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
  }
  onSaveClick() {
    if (this.data.mode === 'create') {
    this.createProductCategory();
    } else {
      this.editProductCategory();
    }
  }
  onCancelClick() {
    let _title = "XÁC NHẬN";
    let _content = "Bạn có muốn hủy tạo,cập nhật sản phẩm dịch vụ?";
    this.dialogPopup = this.dialogPop.open(PopupComponent,
      {
        width: '500px',
        height: '250px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      if (result) {
        this.data.productcategoryCreated = false;
        this.dialogRef.close(this.data);
      }
      else {
        return;
      }
    });


  }

}
function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'checkDuplicateCode': true };
      }
      return null;
    }
  }
}


