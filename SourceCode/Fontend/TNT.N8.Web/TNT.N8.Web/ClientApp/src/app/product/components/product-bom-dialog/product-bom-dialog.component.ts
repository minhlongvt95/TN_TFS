import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl, FormBuilder } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ProductService } from '../../services/product.service';
import { QuickCreateProductComponent } from '../../../shared/components/quick-create-product/quick-create-product.component';

class productModel {
  productId: string;
  productName: string;
  productCode: string;
  productUnitId: string;
  productUnitName: string;
  productCodeName: string;
  constructor() {
  }
}

class productBillOfMaterialsModel {
  productBillOfMaterialId: string;
  productId: string;
  productMaterialId: string;
  quantity: number;
  effectiveFromDate: Date;
  effectiveToDate: Date;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;

  //show in table
  productCode: string;
  productName: string;
  productUnitName: string;
}

class DynamicDialogConfigModel {
  isEdit: boolean;
  productBillOfMaterials: productBillOfMaterialsModel;
  listProductBillOfMaterials: Array<productBillOfMaterialsModel>;
}

@Component({
  selector: 'app-product-bom-dialog',
  templateUrl: './product-bom-dialog.component.html',
  styleUrls: ['./product-bom-dialog.component.css'],
  providers: [DialogService]
})
export class ProductBomDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();

  //DynamicDialogConfig variable
  dynamicDialogConfigData: DynamicDialogConfigModel;

  //form
  productBOMForm: FormGroup;

  //master data
  listProduct: Array<productModel> = [];

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private productService: ProductService,
    private formBuilder: FormBuilder,
    public dialogService: DialogService,
  ) {
    this.dynamicDialogConfigData = this.config.data;
  }

  ngOnInit() {
    this.initForm();
    this.getMasterdata();
  }

  initForm() {
    this.productBOMForm = this.formBuilder.group({
      'ProductCode': new FormControl(null, [Validators.required]),
      'ProductName': new FormControl('', []),
      'ProductUnitName': new FormControl('', []),
      'Quantity': new FormControl('0', [Validators.required]),
      'EffectiveFromDate': new FormControl(null, [Validators.required]),
      'EffectiveToDate': new FormControl(null, [Validators.required]),
    }, {
      validators: mustUniqueProduct('ProductCode', 'EffectiveFromDate', 'EffectiveToDate', this.dynamicDialogConfigData.listProductBillOfMaterials)
    })
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.productService.getDataCreateUpdateBOM();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listProduct = result.listProduct;
      /* hiển thị drop down dưới dạng mã - tên sản phẩm */
      this.listProduct.forEach(product => product.productCodeName = `${product.productCode} - ${product.productName}`);
    }

    /* patch data nếu edit product bom */
    if (this.dynamicDialogConfigData.isEdit) {
      this.patchDataToForm();
    }
  }

  patchDataToForm() {
    let product = this.listProduct.find(e => this.dynamicDialogConfigData.productBillOfMaterials.productMaterialId == e.productId);
    let quantity = this.dynamicDialogConfigData.productBillOfMaterials.quantity;
    this.productBOMForm.get('ProductCode').patchValue(product ? product : null);
    this.productBOMForm.get('ProductName').patchValue(this.dynamicDialogConfigData.productBillOfMaterials.productName);
    this.productBOMForm.get('ProductUnitName').patchValue(this.dynamicDialogConfigData.productBillOfMaterials.productUnitName);
    this.productBOMForm.get('Quantity').patchValue(quantity ? quantity.toString() : '0');
    this.productBOMForm.get('EffectiveFromDate').patchValue(this.dynamicDialogConfigData.productBillOfMaterials.effectiveFromDate ? new Date(this.dynamicDialogConfigData.productBillOfMaterials.effectiveFromDate) : null);
    this.productBOMForm.get('EffectiveToDate').patchValue(this.dynamicDialogConfigData.productBillOfMaterials.effectiveToDate ? new Date(this.dynamicDialogConfigData.productBillOfMaterials.effectiveToDate) : null);
  }

  cancel() {
    this.ref.close();
  }

  changeProduct(event: any) {
    if (!event.value) {
      /* bỏ chọn sản phẩm */
      this.productBOMForm.get('ProductName').patchValue('');
      this.productBOMForm.get('ProductUnitName').patchValue('');
      return;
    }

    let selectedProduct: productModel = event.value;
    /* patch tên và đơn vị tính của sản phẩm */
    this.productBOMForm.get('ProductName').patchValue(selectedProduct?.productName ?? '');
    this.productBOMForm.get('ProductUnitName').patchValue(selectedProduct?.productUnitName ?? '');
    return;
  }

  save() {
    if (!this.productBOMForm.valid) {
      Object.keys(this.productBOMForm.controls).forEach(key => {
        if (this.productBOMForm.controls[key].valid == false) {
          this.productBOMForm.controls[key].markAsTouched();
        }
      });
      return;
    }

    let productBillOfMaterial = this.mappingFormToModel();
    let result = { productBillOfMaterial };
    this.ref.close(result);
  }

  mappingFormToModel(): productBillOfMaterialsModel {
    let product: productModel = this.productBOMForm.get('ProductCode').value;
    let quantity = this.productBOMForm.get('Quantity').value;
    let effectiveFromDate: Date = this.productBOMForm.get('EffectiveFromDate').value;
    let effectiveToDate: Date = this.productBOMForm.get('EffectiveToDate').value;

    if (effectiveFromDate) {
      effectiveFromDate.setHours(0, 0, 0, 0)
    }
    if (effectiveToDate) {
      effectiveToDate.setHours(23, 59, 59, 999)
    }

    let result = new productBillOfMaterialsModel();
    /* tạo mới: newId; edit: patch id */
    if (!this.dynamicDialogConfigData.isEdit) {
      result.productBillOfMaterialId = this.emptyGuid;
      result.productId = this.emptyGuid;
    } else {
      result.productBillOfMaterialId = this.dynamicDialogConfigData.productBillOfMaterials.productBillOfMaterialId;
      result.productId = this.dynamicDialogConfigData.productBillOfMaterials.productId;
    }
    result.productMaterialId = product?.productId ?? null;
    result.quantity = quantity ? parseFloat(quantity.replace(/,/g, '')) : 0;
    result.effectiveFromDate = effectiveFromDate ? effectiveFromDate : null;
    result.effectiveToDate = effectiveToDate ? effectiveToDate : null;

    //show in the table
    result.productCode = product?.productCode ?? '';
    result.productName = product?.productName ?? '';
    result.productUnitName = product?.productUnitName ?? '';

    result.active = true;
    result.createdById = this.auth.UserId;
    result.createdDate = new Date();

    return result;
  }

  /* Mở popup Tạo nhanh sản phẩm */
  openQuickCreProductDialog() {
    let ref = this.dialogService.open(QuickCreateProductComponent, {
      data: {},
      header: 'Tạo nhanh sản phẩm',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        'overflow-x': 'hidden'
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let newProduct: productModel = result.newProduct;
        this.listProduct = [newProduct, ...this.listProduct];
        this.productBOMForm.get('ProductCode').patchValue(newProduct ? newProduct : null);
        let event = {
          value: newProduct
        }
        this.changeProduct(event);
      }
    });
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function mustUniqueProduct(productControlName: string, effectiveFromDateName: string, effectiveToDate: string, listProductBillOfMaterials: Array<productBillOfMaterialsModel>) {
  return (formGroup: FormGroup) => {
    // const control = formGroup.controls[controlName];

    const productControl = formGroup.controls[productControlName];
    const productControlValue: productBillOfMaterialsModel = productControl.value;

    const effectiveFromControl = formGroup.controls[effectiveFromDateName];
    const effectiveFromControlValue: Date = effectiveFromControl.value;
    const effectiveToControl = formGroup.controls[effectiveToDate];
    const effectiveToControlValue: Date = effectiveToControl.value;

    if (productControl.errors && !productControl.errors.mustUniqueProduct) {
      // return if another validator has already found an error on the matchingControl
      return;
    }

    if (productControlValue && effectiveFromControlValue && effectiveToControlValue) {
      let product = listProductBillOfMaterials.find(e => e.productMaterialId == productControlValue.productId);
      if (product) {
        // productControl.setErrors({ mustUniqueProduct: true });
        if (product?.effectiveFromDate && product?.effectiveToDate) {
          let checkDateOverlap = dateRangeOverlaps(product.effectiveFromDate, product.effectiveToDate, effectiveFromControlValue, effectiveToControlValue);
          if (checkDateOverlap) {
            productControl.setErrors({ mustUniqueProduct: true });
          } else {
            productControl.setErrors(null);
          }
        } else {
          productControl.setErrors(null);
        }
      } else {
        productControl.setErrors(null);
      }
    } else {
      productControl.setErrors(null);
    }
  }
}

function dateRangeOverlaps(a_start: Date, a_end: Date, b_start: Date, b_end: Date) {
  if (a_start <= b_start && b_start <= a_end) return true; // b starts in a
  if (a_start <= b_end && b_end <= a_end) return true; // b ends in a
  if (b_start < a_start && a_end < b_end) return true; // a in b
  return false;
}
