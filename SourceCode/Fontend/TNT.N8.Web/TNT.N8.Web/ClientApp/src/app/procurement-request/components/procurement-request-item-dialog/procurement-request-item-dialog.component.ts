import { Component, OnInit, ViewChild, ElementRef, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { ProcurementRequestItemModel } from '../../models/procurementRequestItem.model';
import { VendorService } from "../../../vendor/services/vendor.service";
import { ProductService } from '../../../product/services/product.service';
import { CategoryService } from "../../../shared/services/category.service";
import { ProcurementRequestService } from "../../services/procurement-request.service";

export interface IDialogData {
  productValue: string;
  prItemData: ProcurementRequestItemModel;
  prItemDataOutput: ProcurementRequestItemModel;
  isTrue: Boolean;
  isEdit: Boolean;
  vendorIdPicked: string;
  vendorNamePicked: string;
  budgetIdWasPicked: string;
  budgetCodeWasPicked: string;
}

@Component({
  selector: 'app-procurement-request-item-dialog',
  templateUrl: './procurement-request-item-dialog.component.html',
  styleUrls: ['./procurement-request-item-dialog.component.css']
})
export class ProcurementRequestItemDialogComponent implements OnInit {
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();

  createPRItemForm: FormGroup;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listCategoryId: Array<string> = [];
  listVendor: Array<any> = [];
  listProductByVendor: Array<any> = [];
  unitmoney: Array<any> = [];
  listunit: Array<any> = [];
  prPlanList: Array<any> = [];
  filteredVendorOptions: Observable<string[]>;
  filteredProductOptions: Observable<string[]>;
  filteredProcurementPlanOptions: Observable<string[]>;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  oldData: ProcurementRequestItemModel = {
    procurementRequestItemId: this.emptyGuid,
    productId: '',
    productName: '',
    productCode: '',
    vendorId: '',
    vendorName: '',
    quantity: 0,
    quantityApproval: 0,
    unitPrice: 0,
    unit: '',
    unitString: '',
    totalPrice: 0,
    procurementRequestId: '',
    procurementPlanId: '',
    procurementPlanCode: '',
    createdById: this.auth.UserId,
    createdDate: new Date(),
    updatedById: '',
    updatedDate: null,
    currencyUnit: '',
    exchangeRate: 0,
    description: '',
    discountValue: 0,
    discountType: false,
    incurredUnit: '',
    orderDetailType: null,
    vat: 0,
    orderNumber: 0,
  };
  prItemModel: ProcurementRequestItemModel = {
    procurementRequestItemId: this.emptyGuid,
    productId: '',
    productName: '',
    productCode: '',
    vendorId: '',
    vendorName: '',
    quantity: 0,
    quantityApproval: 0,
    unitPrice: 0,
    unit: '',
    unitString: '',
    totalPrice: 0,
    procurementRequestId: '',
    procurementPlanId: '',
    procurementPlanCode: '',
    createdById: this.auth.UserId,
    createdDate: new Date(),
    updatedById: '',
    updatedDate: null,
    currencyUnit: '',
    exchangeRate: 1,
    description: '',
    discountValue: 0,
    discountType: false,
    incurredUnit: '',
    orderDetailType: null,
    vat: 0,
    orderNumber: 0,
  };
  constructor(private formBuilder: FormBuilder,
    private el: ElementRef,
    private categoryService: CategoryService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<ProcurementRequestItemDialogComponent>,
    private productService: ProductService,
    private vendorSevice: VendorService,
    private prService: ProcurementRequestService) { }

  async ngOnInit() {
    this.createPRItemForm = this.formBuilder.group({
      vendorControl: ['', [Validators.required]],
      productNameControl: ['', [Validators.required]],
      productCodeControl: [{ disabled: true, value: '' }],
      procurementPlanControl: ['', [Validators.required]],
      contentControl: [''],
      unitControl: [''],
      quantityControl: ['', [Validators.min(0.000001)]],
      unitPriceControl: [{ value: '', disabled: false }],
      totalControl: [{ disabled: true, value: '' }]
    });
    await this.getListVendor();
    await this.getMasterData();
  }
  getMasterData() {
    /**Get Product Unit */
    this.categoryService.getAllCategoryByCategoryTypeCode('DNH').subscribe(response => {
      let result = <any>response;
      this.listunit = result.category;
    }, error => { });

    this.prService.getAllProcurementPlan(this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.prPlanList = result.procurementPlanList;
      this.filteredProcurementPlanOptions = this.createPRItemForm.controls['procurementPlanControl'].valueChanges.pipe(
        startWith(null),
        map((item: string) => item ? this._filterProcurementPlan(item, this.prPlanList) : this.prPlanList.slice()));
    }, error => { });
    // Get Info Item
    if (this.data.isEdit) {
      this.prItemModel = this.data.prItemData;
      this.createPRItemForm.controls['vendorControl'].setValue(this.prItemModel.vendorName);
      this.createPRItemForm.controls['vendorControl'].disable();
      this.createPRItemForm.controls['procurementPlanControl'].setValue(this.prItemModel.procurementPlanCode);
      this.getListProduct(this.prItemModel.vendorId);
    } else {
      // Get Info Item when vendor has been picked
      if (this.data.vendorIdPicked !== '') {
        this.prItemModel.vendorId = this.data.vendorIdPicked;
        this.prItemModel.vendorName = this.data.vendorNamePicked;
        this.createPRItemForm.controls['vendorControl'].setValue(this.data.vendorNamePicked);
        this.createPRItemForm.controls['vendorControl'].disable();
        this.getListProduct(this.data.vendorIdPicked);
      }

      if (this.data.budgetIdWasPicked !== '') {
        this.prItemModel.procurementPlanId = this.data.budgetIdWasPicked;
        this.prItemModel.procurementPlanCode = this.data.budgetCodeWasPicked;
        this.createPRItemForm.controls['procurementPlanControl'].setValue(this.data.budgetCodeWasPicked);
        this.createPRItemForm.controls['procurementPlanControl'].disable();
      }
    }
  }
  private _filterProcurementPlan(value: any, array: any): string[] {
    return array.filter(plan =>
      plan.procurementPlanCode.toLowerCase().includes(value.toString().toLowerCase().trim()));
  }

  selectedUnit(event: any) {
    let target = event.source.selected._element.nativeElement;
    let selectedData = {
      value: event.value,
      text: target.innerText.trim()
    };
    this.prItemModel.unitString = selectedData.text;
  }

  selectedPRPlan(event: MatAutocompleteSelectedEvent) {
    this.createPRItemForm.controls['procurementPlanControl'].setValue(event.option.viewValue);
    this.prItemModel.procurementPlanCode = event.option.viewValue;
    this.prItemModel.procurementPlanId = event.option.value;
  }

  //Autocomplete VEndor
  selectedVendorFn(event: MatAutocompleteSelectedEvent): void {
    // Disappear sp/dv khi thay doi nha cung cap
    if (event.option.value !== this.prItemModel.vendorId) {
      this.createPRItemForm.controls['productNameControl'].setValue(null);
    }
    this.createPRItemForm.controls['vendorControl'].setValue(event.option.viewValue);
    this.prItemModel.vendorId = event.option.value;
    this.prItemModel.vendorName = event.option.viewValue;
    this.getListProduct(event.option.value);
  }

  private _filterVendor(value: any, array: any): string[] {
    return array.filter(vendor =>
      vendor.vendorCode.toLowerCase().indexOf(value.toLowerCase()) >= 0 || vendor.vendorName.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  }
  getListVendor() {
    this.vendorSevice.searchVendor("", "", this.listCategoryId, this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.listVendor = result.vendorList;
      this.filteredVendorOptions = this.createPRItemForm.controls['vendorControl'].valueChanges.pipe(
        startWith(null),
        map((item: string) => item ? this._filterVendor(item, this.listVendor) : this.listVendor.slice()));

    }, error => { });
  }

  //Autocomplete Product
  selectedProductFn(event: MatAutocompleteSelectedEvent): void {
    this.createPRItemForm.controls['productNameControl'].setValue(event.option.viewValue);
    this.prItemModel.productName = event.option.viewValue;
    this.prItemModel.productId = event.option.value;
    var productObject = this.listProductByVendor.filter(product => product.productId.toLowerCase().indexOf(event.option.value.toLowerCase()) >= 0);
    this.createPRItemForm.controls['unitPriceControl'].setValue(productObject[0].price1);
    this.createPRItemForm.controls['quantityControl'].setValue(0);
    this.createPRItemForm.controls['productCodeControl'].setValue(productObject[0].productCode);
    this.prItemModel.quantity = null;
    //Set up for Unit Product
    //this.customerOrderDetailModel.UnitId = productObject[0].productUnitId;
    var unitObject = this.listunit.filter(unit => unit.categoryId.toLowerCase().indexOf(productObject[0].productUnitId.toLowerCase()) >= 0);
    this.prItemModel.unit = unitObject[0].categoryId;
    this.prItemModel.unitString = unitObject[0].categoryName;
    //End Unit Product
  }

  private _filterProduct(value: any, array: any): string[] {
    return array.filter(vendor =>
      vendor.productName.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  }

  getListProduct(VendorID: string) {
    this.productService.getProductByVendorID(VendorID).subscribe(response => {
      let result = <any>response;
      this.listProductByVendor = result.lstProduct;
      if (this.data.isEdit) {
        const toSelectProduct = this.listProductByVendor.find(c => c.productId == this.data.prItemData.productId);
        if (typeof toSelectProduct !== "undefined") {
          if (toSelectProduct != null) {
            this.createPRItemForm.controls['productNameControl'].setValue(toSelectProduct.productName);
          }
        }
      }
      this.filteredProductOptions = this.createPRItemForm.controls['productNameControl'].valueChanges.pipe(
        startWith(null),
        map((item: string) => item ? this._filterProduct(item, this.listProductByVendor) : this.listProductByVendor.slice()));

    }, error => { });
  }
  //End Autocomplete Product

  onPriceOrQuantityChange() {
    this.prItemModel.totalPrice = this.prItemModel.unitPrice * this.prItemModel.quantity;
  }

  cancel() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }

  saveItem() {
    if (this.createPRItemForm.valid) {
      this.data.isTrue = true;
      this.data.prItemDataOutput = this.prItemModel;
      this.dialogRef.close(this.data);
    }
    else {
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  onKeyPress(event: any) {
    const pattern = /^[0-9\.]$/;
    const inputChar = String.fromCharCode(event.charCode);
    if (!pattern.test(inputChar)) {
      event.preventDefault();
    }
  }

}
