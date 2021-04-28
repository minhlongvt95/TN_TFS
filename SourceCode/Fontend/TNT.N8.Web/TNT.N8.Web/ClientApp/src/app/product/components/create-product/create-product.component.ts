import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, ViewRef, OnDestroy, Renderer2 } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { TreeProductCategoryComponent } from '../tree-product-category/tree-product-category.component';
import { AddEditSerialComponent } from '../add-edit-serial/add-edit-serial.component';
import { QuickCreateVendorComponent } from '../quick-create-vendor/quick-create-vendor.component';

import {
  ProductModel2,
  productMoneyUnitModel,
  productUnitModel,
  vendorModel,
  warehouseModel,
  ProductQuantityInWarehouseModel,
  ProductAttributeCategory,
  ProductAttributeCategoryValueModel,
  Serial,
  ProductImageModel, ProductVendorMappingModel
} from '../../models/product.model';

import { ProductService } from '../../services/product.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { TranslateService } from '@ngx-translate/core';

import { ImageUploadService } from '../../../shared/services/imageupload.service';

/* primeng api */
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { VendorDetailDialogComponent } from '../vendor-detail-dialog/vendor-detail-dialog.component';
import { ProductBomDialogComponent } from '../product-bom-dialog/product-bom-dialog.component';
/* end primeng api */

interface Data {
  name: string;
  value: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  productVendorMappingModel: ProductVendorMappingModel,
}

class image {
  source: string;
  imageSize: string;
  imageName: string;
  imageType: string;
  imageUrl: string;
  alt: string;
  title: string;
}

class NoteErr {
  public code: string;
  public name: string;
}

class InventoryReport {
  public warehouseId: string;
  public warehouseNameByLevel: string;
  public quantityMinimum: string;
  public quantityMaximum: string;
  public startQuantity: string;
  public openingBalance: string;
  public listSerial: Array<serialModel>;
  public note: string;

  constructor() {
    this.listSerial = [];
  }
}

class serialModel {
  public serialId: string;
  public serialCode: string;
  public productId: string;
  public warehouseId: string;
  public statusId: string;
}

class productCategoryModel {
  public productCategoryId: string;
  public productCategoryCode: string;
  public productCategoryName: string;
  public productCategoryLevel: string;
  public hasChildren: boolean;
  public productCategoryNameByLevel: string; //tên theo phân cấp
  public productCategoryListNameByLevel: Array<string>;

  constructor() {
    this.productCategoryListNameByLevel = [];
  }
}

class productAttributeModel {
  productAttributeName: string;
  productAttributeValue: Array<string>;

  constructor() {
    this.productAttributeValue = [];
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

class productBillOfMaterialsModelRequest {
  ProductBillOfMaterialId: string;
  ProductId: string;
  ProductMaterialId: string;
  Quantity: number;
  EffectiveFromDate: Date;
  EffectiveToDate: Date;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
}

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class CreateProductComponent implements OnInit {
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  //define note
  listNoteErr: Array<NoteErr> = [
    { code: "required_min_quanty", name: "Không được để trống số lượng tồn kho tối thiểu" },
    { code: "min_quantity_less_max_quantity", name: "Số lượng tối thiểu không được lớn hơn số lượng tối đa" },
    { code: "required_start_quanty", name: "Không được để trống số lượng tồn kho đầu kỳ" }
  ];

  /* Action*/
  actionAdd: boolean = true;
  /*END*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  
  //master data
  listProperty: Array<productMoneyUnitModel> = [];
  listPriceInventory: Array<productMoneyUnitModel> = [];
  listProductUnit: Array<productUnitModel> = [];
  listVendor: Array<vendorModel> = [];
  listWarehouse: Array<warehouseModel> = [];
  listWarehouseLevel0: Array<warehouseModel> = [];
  listProductCode: Array<string> = [];
  productVendorMapping: ProductVendorMappingModel = new ProductVendorMappingModel();
  listProductVendorMapping: Array<ProductVendorMappingModel> = [];

  //table
  rows: number = 10;
  colsAttribute: any;
  colVentor: any;
  selectedColumns: any;

  /*Thông tin kho*/
  listWarehouseInventory: Array<warehouseModel> = [];
  selectedWarehouseInventory: warehouseModel;
  colsInventory: any;
  listInventory: Array<InventoryReport> = [];
  /*End*/

  colsProductBOM: any;
  selectedColsProductBOM: any;

  //form controls
  productForm: FormGroup;
  attributeForm: FormGroup;
  
  //images
  images: image[] = [];

  //tab Thông tin chung
  selectedProductCategory: productCategoryModel;

  //tab Thông tin kho
  selectedWarehouseForMinQuanty: warehouseModel;
  selectedWarehouseStartQuantity: warehouseModel;

  //tab thuộc tính
  listProductAttribute: Array<productAttributeModel> = [];

  //tab BOM
  listProductBillOfMaterials: Array<productBillOfMaterialsModel> = [];

  // is possition fiexd
  fixed: boolean = false;
  withFiexd: string = "";
  actionEdit: boolean = true;
  followInventory: boolean = true;
  serialNumer: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  isSerialNumberRequired: boolean;
  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  data: Array<Data> = [
    { name: '1561', value: 'NY' },
    { name: '1672', value: 'RM' },
  ];
  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate', { static: true }) saveAndCreate: ElementRef;
  @ViewChild('save', { static: true }) save: ElementRef;

  constructor(
    private imageService: ImageUploadService,
    public messageService: MessageService,
    private translate: TranslateService,
    private el: ElementRef,
    private getPermission: GetPermission,
    private dialogService: DialogService,
    private productService: ProductService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
  ) {
    translate.setDefaultLang('vi');
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target) &&
          !this.saveAndCreate.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  async ngOnInit() {
    //Check permission
    let resource = "sal/product/create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.initTable();
      this.initForm();
      this.initGalleria();
      this.getMasterData();
    }
  }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initTable() {
    //Bảng tồn kho
    this.colsInventory = [
      { field: 'warehouseNameByLevel', header: 'Vị trí', textAlign: 'left', width: '120px' },
      { field: 'quantityMinimum', header: 'Số lượng tồn kho tối thiểu', textAlign: 'right', width: '180px' },
      { field: 'quantityMaximum', header: 'Số lượng tồn kho tối đa', textAlign: 'right', width: '180px' },
      { field: 'startQuantity', header: 'Số lượng tồn kho đầu kỳ', textAlign: 'right', width: '180px' },
      { field: 'openingBalance', header: 'Dư đầu', textAlign: 'right', width: '180px' },
      { field: 'addSerial', header: 'Số Serial', textAlign: 'center', width: '120px' },
      // { field: 'note', header: 'Ghi chú', textAlign: 'left', width: '120px' },
      { field: 'actions', header: 'Thao tác', textAlign: 'center', width: '100px' },
    ];

    //bảng thuộc tính
    this.colsAttribute = [
      { field: 'productAttributeName', header: 'Tên thuộc tính', textAlign: 'left' },
      { field: 'productAttributeValue', header: 'Giá trị', textAlign: 'left' },
    ];

    this.colVentor = [
      { field: 'Move', header: '#', textAlign: 'center', width: '50px' },
      { field: 'VendorName', header: 'Tên NCC', textAlign: 'left' },
      { field: 'VendorProductName', header: 'Sản phẩm phía NCC', textAlign: 'left' },
      { field: 'FromDate', header: 'Ngày hiệu lực từ', textAlign: 'right' },
      { field: 'ToDate', header: 'Ngày hiệu lực đến', textAlign: 'right' },
      { field: 'MiniumQuantity', header: 'Số lượng tối thiểu', textAlign: 'right' },
      { field: 'Price', header: 'Giá', textAlign: 'right' },
      { field: 'MoneyUnitName', header: 'Tiền tệ', textAlign: 'left' },
    ];
    this.selectedColumns = this.colVentor;

    this.colsProductBOM = [
      { field: 'productCode', header: 'Mã nguyên vật liệu', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'productName', header: 'Tên nguyên vật liệu', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'productUnitName', header: 'Đơn vị tính', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'quantity', header: 'Số lượng', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'effectiveFromDate', header: 'Hiệu lực từ', textAlign: 'right', display: 'table-cell', width: '100px' },
      { field: 'effectiveToDate', header: 'Hiệu lực đến', textAlign: 'right', display: 'table-cell', width: '100px' },
    ];
    this.selectedColsProductBOM = this.colsProductBOM;
  }

  initForm() {
    this.productForm = new FormGroup({
      'FolowInventory': new FormControl(true),
      'ManagerSerialNumber': new FormControl(false),
      'ProductCategory': new FormControl(null, Validators.required),
      'ProductCode': new FormControl('', [Validators.required, checkBlankString()]),
      'ProductName': new FormControl('', [Validators.required, checkBlankString()]),
      'VAT': new FormControl('0', [Validators.required, ValidationMaxValue(100)]),
      'ExWarehousePrice': new FormControl('0'),
      'GuaranteeTime': new FormControl(''),
      'Description': new FormControl(''),

      'ProductUnit': new FormControl(null, Validators.required),
      'InventoryPrices': new FormControl(null, Validators.required),
      'Property': new FormControl(null, Validators.required),
      'WarehouseAccount': new FormControl(null),
      'RevenueAccount': new FormControl(null),
      'PayableAccount': new FormControl(null),
      'ImportTax': new FormControl('0'),
      'CostAccount': new FormControl(null),
      'AccountReturns': new FormControl(null),
    });

    this.attributeForm = new FormGroup({
      'AttributeName': new FormControl('', Validators.required),
      'AttributeValues': new FormControl([], Validators.required)
    });
  }

  resetProductForm() {
    this.productForm.reset();
    this.selectedProductCategory = undefined;
    this.productForm.patchValue({
      'FolowInventory': true,
      'ManagerSerialNumber': false,
      'ProductCategory': '',
      'ProductCode': '',
      'ProductName': '',
      'ProductUnit': null,
      'VAT': '0',
      'ExWarehousePrice': '0',
      'GuaranteeTime': '',
      'Description': '',
      'InventoryPrices': null,
      'Property': null,
      'WarehouseAccount': null,
      'RevenueAccount': null,
      'PayableAccount': null,
      'ImportTax': '0',
      'CostAccount': null,
      'AccountReturns': null
    });
    this.listProductVendorMapping = [];
  }

  resetAttributeForm() {
    this.attributeForm.reset();
    this.attributeForm.patchValue({
      'AttributeName': '',
      'AttributeValues': []
    });
  }

  resetProductInWarehouse() {
    this.listWarehouseInventory = [];
    this.listWarehouseLevel0 = this.listWarehouse.filter(e => e.warehouseParent == null);
  }

  initGalleria() {
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.productService.getMasterdataCreateProduct();
    this.loading = false;
    if (result.statusCode === 200) {
      this.listProductCode = result.listProductCode;
      this.listProductUnit = result.listProductUnit;
      this.listVendor = result.listVendor;
      this.listWarehouse = result.listWarehouse;
      this.listWarehouseLevel0 = this.listWarehouse.filter(e => e.warehouseParent == null); //lấy kho nút 0
      this.listProperty = result.listProperty;
      this.listPriceInventory = result.listPriceInventory;
      this.getListWarehouseStartQuantity();
      //set duplicate code validation
      this.productForm.get('ProductCode').setValidators([Validators.required, checkDuplicateCode(this.listProductCode), checkBlankString()]);
      this.productForm.updateValueAndValidity();
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  getListWarehouseStartQuantity() {
    this.listWarehouseInventory = [];

    this.listWarehouseLevel0.forEach(warehouse => {
      this.getLastWarehouse(warehouse);
    });

    this.listWarehouseInventory.forEach(warehouse => {
      let listParentId = this.getListParentId(warehouse, []);
      let listParentName: Array<string> = [];
      listParentId.forEach(warehouseId => {
        let warehouse = this.listWarehouse.find(e => e.warehouseId == warehouseId);
        listParentName.push(warehouse.warehouseName);
      });
      warehouse.warehouseNameByLevel = listParentName.reverse().join(' > ');
    });
  }

  getLastWarehouse(currentWarehouse: warehouseModel) {
    let findChildWarehouse = this.listWarehouse.filter(e => e.warehouseParent === currentWarehouse.warehouseId);
    if (findChildWarehouse.length === 0) {
      this.listWarehouseInventory = [...this.listWarehouseInventory, currentWarehouse];
      return;
    }
    findChildWarehouse.forEach(warehouse => {
      this.getLastWarehouse(warehouse);
    });
  }

  getListParentId(currentWarehouse: warehouseModel, listParentIdReponse: Array<string>): Array<string> {
    listParentIdReponse.push(currentWarehouse.warehouseId);
    let parent = this.listWarehouse.find(e => e.warehouseId == currentWarehouse.warehouseParent);
    if (parent === undefined) return listParentIdReponse;
    listParentIdReponse.push(parent.warehouseId);
    //find parent of parent
    let parentOfParent = this.listWarehouse.find(e => e.warehouseId == parent.warehouseParent);
    if (parentOfParent === undefined) {
      return listParentIdReponse;
    } else {
      this.getListParentId(parentOfParent, listParentIdReponse);
    }
    return listParentIdReponse;
  }

  openProductCategoryDialog() {
    let ref = this.dialogService.open(TreeProductCategoryComponent, {
      header: 'Thông tin danh mục',
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
        if (result.status) {
          this.selectedProductCategory = result.productCategory;
          this.productForm.get('ProductCategory').setValue(this.selectedProductCategory.productCategoryNameByLevel)
        }
      }
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  onCreateNewVendor() {
    let ref = this.dialogService.open(QuickCreateVendorComponent, {
      header: 'Tạo nhanh nhà cung cấp',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "100px",
        "max-height": "600px",
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          let newVendor: vendorModel = result.vendorModel;
          // this.listVendor = [newVendor, ...this.listVendor];
          // let listOldVendors = this.productForm.get('Vendors').value;
          // let listNewVendors = [newVendor, ...listOldVendors];
          // this.productForm.get('Vendors').setValue(listNewVendors);
        }
      }
    });
  }

  isDisplayRequired: boolean = true;
  changeFollowInventory(event: any) {
    if (event == null) return false;
    if (event == true) {
      this.isDisplayRequired = true;
      this.productForm.controls['InventoryPrices'].setValidators(Validators.required);
      this.productForm.controls['WarehouseAccount'].setValidators(Validators.required);
      this.productForm.controls['InventoryPrices'].updateValueAndValidity();
      this.productForm.controls['WarehouseAccount'].updateValueAndValidity();
    } else {
      this.isDisplayRequired = false;
      this.productForm.controls['InventoryPrices'].setValidators(null);
      this.productForm.controls['WarehouseAccount'].setValidators(null);
      this.productForm.controls['InventoryPrices'].updateValueAndValidity();
      this.productForm.controls['WarehouseAccount'].updateValueAndValidity();
    }
  }

  changeMangagerSeriralNumber(event: any) {
    this.isSerialNumberRequired = event;
  }

  /*Thêm tồn kho*/
  changeWarehouseInventory(event: any) {
    let selectedItem: warehouseModel = event.value;

    let inventory = new InventoryReport();
    inventory.warehouseId = selectedItem.warehouseId;
    inventory.warehouseNameByLevel = selectedItem.warehouseNameByLevel;
    inventory.quantityMinimum = "0";
    inventory.quantityMaximum = null;
    inventory.startQuantity = "0";
    inventory.openingBalance = "0";

    this.listInventory = [...this.listInventory, inventory];
    
    this.listWarehouseInventory = this.listWarehouseInventory.filter(e => e.warehouseId !== selectedItem.warehouseId);
  }

  /*Thêm Serial*/
  addSerial(rowData: InventoryReport) {
    let ref = this.dialogService.open(AddEditSerialComponent, {
      header: 'Thêm Serial',
      data:
      {
        inventoryReport: rowData,
        listInventory: this.listInventory
      },
      width: '40%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
      }
    });
    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          let addedProductSerialModel: InventoryReport = result.inventoryReport;
          rowData.listSerial = addedProductSerialModel.listSerial;
        }
      }
    });
  }

  /*Xóa tồn kho*/
  deleteInventory(rowData: InventoryReport) {
    this.listInventory = this.listInventory.filter(e => e.warehouseId !== rowData.warehouseId);
    let warehouse: warehouseModel = this.listWarehouse.find(e => e.warehouseId == rowData.warehouseId);
    this.listWarehouseInventory = [...this.listWarehouseInventory, warehouse];
    this.selectedWarehouseInventory = null;
  }

  addProductAttribute() {
    if (!this.attributeForm.valid) {
      Object.keys(this.attributeForm.controls).forEach(key => {
        if (!this.attributeForm.controls[key].valid) {
          this.attributeForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      let productAttribute = new productAttributeModel();
      productAttribute.productAttributeName = this.attributeForm.get('AttributeName').value;
      productAttribute.productAttributeValue = this.attributeForm.get('AttributeValues').value;
      this.listProductAttribute = [...this.listProductAttribute, productAttribute];
      this.resetAttributeForm();
    }
  }


  deleteAttribute(rowData: productAttributeModel) {
    this.listProductAttribute = this.listProductAttribute.filter(e => e !== rowData);
  }

  async createProduct(type: boolean) {
    //type = true: Lưu và thêm mới; type = false: Lưu;
    if (!this.productForm.valid) {
      Object.keys(this.productForm.controls).forEach(key => {
        if (!this.productForm.controls[key].valid) {
          this.productForm.controls[key].markAsTouched();
        }
      });
      let target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
    }
    else {
      //check status list tồn kho tối thiểu
      let minquantyStatus = this.checkStatusMinQuanty();
      if (minquantyStatus === false) {
        this.clearToast();
        this.showToast('error', 'Thông báo', 'Danh sách tồn kho tối thiểu không hợp lệ');
        return;
      }
      //check status list tồn kho đầu kỳ
      let startquantyStatus = this.checkStatusStartQuanty();
      if (startquantyStatus === false) {
        this.clearToast();
        this.showToast('error', 'Thông báo', 'Danh sách tồn kho đầu kỳ không hợp lệ');
        return;
      }
      //valid
      let productModel: ProductModel2 = this.mappingProductForm();

      let listInventoryReport = this.mapDataListInventoryReport();

      let listAttribute = this.mappingAttribute(this.listProductAttribute);
      let listProductBillOfMaterials = this.getListProductBillOfMaterials();
      let fileList: File[] = this.getFileList();
      let listProductImage: Array<ProductImageModel> = this.getListProductImage();

      this.loading = true;
      let uploadResult: any = await this.imageService.uploadProductImageAsync(fileList);
      
      let result: any = await this.productService.createProductAsync(
        productModel, 
        this.listProductVendorMapping, 
        listAttribute, 
        listProductBillOfMaterials,
        listInventoryReport,
        listProductImage, 
        this.auth.userId);
      this.loading = false;
      if (result.statusCode === 200) {
        switch (type) {
          case true:
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Tạo sản phẩm thành công');
            this.resetProductForm();
            this.resetProductInWarehouse();
            this.resetAttributeForm();
            this.listProductAttribute = [];
            break;
          case false:
            this.router.navigate(['/product/detail', { productId: result.productId }]);
            break;
          default:
            break;
        }

        this.isInvalidForm = false;
        this.isOpenNotifiError = false;
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    }
  }

  cancel() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác, bạn có chắc chắn muốn hủy?',
      accept: () => {
        this.router.navigate(['/product/list']);
      }
    });
  }

  checkStatusMinQuanty(): boolean {
    let dataStatus = true;

    // this.listMinQuantityProduct.forEach(rowData => {
    //   //reset status
    //   rowData.status = true;
    //   rowData.listNoteErr = [];
    //   //check empty string
    //   if (rowData.minQuantity.trim() === '') {
    //     rowData.status = false;
    //     rowData.listNoteErr = [...rowData.listNoteErr, this.listNoteErr.find(e => e.code == 'required_min_quanty').name];
    //   }
    //   if (rowData.maxQuantity != null && rowData.maxQuantity != undefined) {
    //     if (rowData.maxQuantity.trim() !== '' && rowData.minQuantity.trim() !== '') {
    //       let minQuantity = ParseStringToFloat(rowData.minQuantity);
    //       let maxQuantity = ParseStringToFloat(rowData.maxQuantity);
    //       if (minQuantity > maxQuantity) {
    //         rowData.status = false;
    //         rowData.listNoteErr = [...rowData.listNoteErr, this.listNoteErr.find(e => e.code == 'min_quantity_less_max_quantity').name];
    //       }
    //     }
    //   }
    //   //check dataStatus
    //   if (rowData.status === false) dataStatus = false;
    // });

    return dataStatus;
  }

  checkStatusStartQuanty(): boolean {
    let dataStatus = true;

    // this.listStartQuantityProduct.forEach(rowData => {
    //   //reset status
    //   rowData.status = true;
    //   rowData.listNoteErr = [];
    //   //check empty string
    //   if (rowData.startQuantity.trim() === '') {
    //     rowData.status = false;
    //     rowData.listNoteErr = [...rowData.listNoteErr, this.listNoteErr.find(e => e.code == 'required_min_quanty').name];
    //   }

    //   //check dataStatus
    //   if (rowData.status === false) dataStatus = false;
    // });

    return dataStatus;
  }


  mappingProductForm(): ProductModel2 {
    let newProduct = new ProductModel2();
    newProduct.ProductId = this.emptyGuid;
    newProduct.ProductCategoryId = this.selectedProductCategory.productCategoryId;
    newProduct.ProductName = this.productForm.get('ProductName').value;
    newProduct.ProductCode = this.productForm.get('ProductCode').value;
    newProduct.Price2 = 0; //không dùng trường này
    newProduct.ProductUnitId = this.productForm.get('ProductUnit').value.categoryId;
    newProduct.ProductDescription = this.productForm.get('Description').value;
    newProduct.Vat = ParseStringToFloat(this.productForm.get('VAT').value);
    newProduct.ExWarehousePrice = ParseStringToFloat(this.productForm.get('ExWarehousePrice').value);
    newProduct.GuaranteeTime = ParseStringToFloat(this.productForm.get('GuaranteeTime').value)
    //default values
    newProduct.CreatedById = this.emptyGuid;
    newProduct.CreatedDate = new Date();
    newProduct.UpdatedById = null;
    newProduct.UpdatedDate = null;
    newProduct.Active = true;
    newProduct.Quantity = 0; //Không dùng trường này
    newProduct.MinimumInventoryQuantity = 0;
    newProduct.CalculateInventoryPricesId = this.productForm.get('InventoryPrices').value ? this.productForm.get('InventoryPrices').value.categoryId : null;
    newProduct.PropertyId = this.productForm.get('Property').value ? this.productForm.get('Property').value.categoryId : null;
    newProduct.WarehouseAccountId = this.productForm.get('WarehouseAccount').value ? this.emptyGuid : null;
    newProduct.RevenueAccountId = this.productForm.get('RevenueAccount').value ? this.emptyGuid : null;
    newProduct.PayableAccountId = this.productForm.get('PayableAccount').value ? this.emptyGuid : null;
    newProduct.ImportTax = this.productForm.get('ImportTax').value;
    newProduct.CostPriceAccountId = this.productForm.get('CostAccount').value ? this.emptyGuid : null;
    newProduct.AccountReturnsId = this.productForm.get('AccountReturns').value ? this.emptyGuid : null;
    newProduct.FolowInventory = this.productForm.get('FolowInventory').value;
    newProduct.ManagerSerialNumber = this.productForm.get('ManagerSerialNumber').value;

    return newProduct
  }

  addVendorProductDetail() {
    let ref = this.dialogService.open(VendorDetailDialogComponent, {
      data: {
        isCreate: true
      },
      header: 'Thêm chi tiết sản phẩm nhà cung cấp',
      width: '70%',
      baseZIndex: 999,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.productVendorMapping = result.productVendorMappingModel;
          this.productVendorMapping.OrderNumber = this.listProductVendorMapping.length + 1;
          this.listProductVendorMapping.push(this.productVendorMapping);
        }
      }
    });
  }

  /*Sửa một sản phẩm nhà cung cấp*/
  onRowSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    if (this.actionEdit) {
      var index = this.listProductVendorMapping.indexOf(dataRow);
      var OldArray = this.listProductVendorMapping[index];

      let ref = this.dialogService.open(VendorDetailDialogComponent, {
        data: {
          isCreate: false,
          productVendorMappingModel: OldArray
        },
        header: "Sửa chi tiết sản phẩm nhà cung cấp",
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: ResultDialog) => {
        if (result) {
          if (result.status) {
            this.listProductVendorMapping[index] = result.productVendorMappingModel;
          }
        }
      });
    }
  }

  /*Xóa sản phẩm nhà cung cấp*/
  deleteItem(dataRow) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listProductVendorMapping = this.listProductVendorMapping.filter(e => e != dataRow)
        //Đánh lại số OrderNumber
        this.listProductVendorMapping.forEach((item, index) => {
          item.OrderNumber = index + 1;
        });
      }
    });
  }

  mapDataListInventoryReport() {
    let listInventoryReport: Array<ProductQuantityInWarehouseModel> = [];

    this.listInventory.forEach(item => {
      let newItem = new ProductQuantityInWarehouseModel();
      newItem.WarehouseId = item.warehouseId;
      newItem.QuantityMinimum = ParseStringToFloat(item.quantityMinimum);
      if (item.quantityMaximum && item.quantityMaximum != '') {
        newItem.QuantityMaximum = ParseStringToFloat(item.quantityMaximum);
      }
      newItem.StartQuantity = ParseStringToFloat(item.startQuantity);
      newItem.OpeningBalance = ParseStringToFloat(item.openingBalance);
      newItem.Note = item.note == null ? item.note : item.note.trim();

      item.listSerial.forEach(serial => {
        //map serial value
        let newSerial: Serial = new Serial();
        newSerial.SerialCode = serial.serialCode;
        newSerial.WarehouseId = item.warehouseId;

        newItem.ListSerial = [...newItem.ListSerial, newSerial];
      });

      listInventoryReport = [...listInventoryReport, newItem];
    });

    return listInventoryReport;
  }

  mappingAttribute(listProductAttribute: Array<productAttributeModel>): Array<ProductAttributeCategory> {
    let listAttribute: Array<ProductAttributeCategory> = [];
    listProductAttribute.forEach(attribute => {
      let newAttribute = new ProductAttributeCategory();
      newAttribute.ProductAttributeCategoryId = this.emptyGuid;
      newAttribute.ProductAttributeCategoryName = attribute.productAttributeName;
      attribute.productAttributeValue.forEach(attributeValue => {
        let newAttributeValue = new ProductAttributeCategoryValueModel();
        newAttributeValue.ProductAttributeCategoryValueId = this.emptyGuid;
        newAttributeValue.ProductAttributeCategoryValue1 = attributeValue;
        newAttributeValue.ProductAttributeCategoryId = this.emptyGuid;
        //default values
        newAttributeValue.CreatedById = this.emptyGuid;
        newAttributeValue.CreatedDate = new Date();
        newAttributeValue.UpdatedById = this.emptyGuid;
        newAttributeValue.UpdatedDate = null;
        newAttributeValue.Active = true;

        newAttribute.ProductAttributeCategoryValue = [...newAttribute.ProductAttributeCategoryValue, newAttributeValue];
      });
      //default values
      newAttribute.CreatedById = this.emptyGuid;
      newAttribute.CreatedDate = new Date();
      newAttribute.UpdatedById = null;
      newAttribute.UpdatedDate = null;
      newAttribute.Active = true;

      listAttribute = [...listAttribute, newAttribute];
    });

    return listAttribute;
  }

  getListProductBillOfMaterials(): Array<productBillOfMaterialsModelRequest> {
    let result: Array<productBillOfMaterialsModelRequest> = [];

    this.listProductBillOfMaterials?.forEach(bom => {
      let newItem = new productBillOfMaterialsModelRequest();
      newItem.ProductBillOfMaterialId = bom.productBillOfMaterialId;
      newItem.ProductId = bom.productId;
      newItem.ProductMaterialId = bom.productMaterialId;
      newItem.Quantity = bom.quantity;
      newItem.EffectiveFromDate = bom.effectiveFromDate ? convertToUTCTime(bom.effectiveFromDate) : null;
      newItem.EffectiveToDate = bom.effectiveToDate ? convertToUTCTime(bom.effectiveToDate) : null;
      newItem.Active = bom.active;
      newItem.CreatedById = this.auth.UserId;
      newItem.CreatedDate = new Date();

      result = [newItem, ...result];
    });

    return result;
  }

  uploadImage(event: any) {
    //reset list image
    this.images = [];
    for (let file of event.files) {
      let fileReader = new FileReader();
      fileReader.readAsDataURL(file);
      fileReader.onload = (reader: any) => {
        let newImage = new image();
        newImage.source = reader.target.result; //base64
        newImage.imageSize = file.size;
        newImage.imageName = file.name;
        newImage.imageType = file.type;
        newImage.title = '';
        newImage.alt = '';
        this.images = [...this.images, newImage];
      };
    }
  }

  removeImage(event: any) {
    let fileReader = new FileReader();
    let file = event.file;
    fileReader.readAsDataURL(file);
    fileReader.onload = (reader: any) => {
      let duplicateImage = new image();
      duplicateImage.source = reader.target.result;
      this.images = this.images.filter(e => e.source !== duplicateImage.source);
    };
  }
  clearImage() {
    //clear all image
    this.images = [];
  }

  getFileList(): File[] {
    let files: File[] = [];
    this.images.forEach((image, index) => {
      image.imageName = this.hashImageName(image.imageName, index);
      let file = this.convertDataURItoFile(image.source, image.imageName, image.imageType);
      files.push(file)
    });
    return files;
  }

  getListProductImage(): Array<ProductImageModel> {
    let result: Array<ProductImageModel> = [];

    this.images.forEach(image => {
      let newProductImageModel: ProductImageModel = new ProductImageModel();
      newProductImageModel.ProductImageId = this.emptyGuid;
      newProductImageModel.ProductId = this.emptyGuid;
      newProductImageModel.ImageName = image.imageName;
      newProductImageModel.ImageSize = image.imageSize;
      newProductImageModel.ImageUrl = '';
      newProductImageModel.Active = true;
      newProductImageModel.CreatedById = this.emptyGuid;
      newProductImageModel.CreatedDate = new Date();
      newProductImageModel.UpdatedById = this.emptyGuid;
      newProductImageModel.UpdatedDate = new Date();

      result.push(newProductImageModel);
    })

    return result;
  }

  hashImageName(imageName: string, index: number): string {
    let newName = imageName;
    let hash = Date.parse(new Date().toString()).toString() + index.toString();
    let lastIndex = imageName.lastIndexOf('.');
    let array1 = newName.slice(0, lastIndex);
    let array2 = newName.slice(lastIndex);
    newName = array1 + "_" + hash + array2;
    return newName;
  };

  convertDataURItoFile(dataURI, fileName, type) {
    // convert base64 to raw binary data held in a string
    // doesn't handle URLEncoded DataURIs - see SO answer #6850276 for code that does this
    var byteString = atob(dataURI.split(',')[1]);
    // separate out the mime component
    var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0]
    // write the bytes of the string to an ArrayBuffer
    var ab = new ArrayBuffer(byteString.length);
    var ia = new Uint8Array(ab);
    for (var i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }
    // write the ArrayBuffer to a blob, and you're done
    var blob: any = new Blob([ia], { type: mimeString });
    //A Blob() is almost a File() - it's just missing the two properties below which we will add
    blob.lastModifiedDate = new Date();
    blob.name = fileName;
    //Cast to a File() type
    let file = new File([blob], fileName, { type: type })
    return file;
  }

  /* Mở dialog thêm nguyên vật liệu sản phẩm */
  addBillOfMaterial() {
    let ref = this.dialogService.open(ProductBomDialogComponent, {
      data: {
        listProductBillOfMaterials: this.listProductBillOfMaterials
      },
      header: 'Thêm mới định mức nguyên vật liệu',
      width: '50%',
      baseZIndex: 999,
      contentStyle: {
        "min-height": "380px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let productBillOfMaterial: productBillOfMaterialsModel = result.productBillOfMaterial;
        this.listProductBillOfMaterials = [productBillOfMaterial, ...this.listProductBillOfMaterials];
      }
    });
  }

  editProductBOM(rowData: productBillOfMaterialsModel) {
    let ref = this.dialogService.open(ProductBomDialogComponent, {
      data: {
        isEdit: true,
        productBillOfMaterials: rowData,
        listProductBillOfMaterials: this.listProductBillOfMaterials.filter(e => e != rowData)
      },
      header: 'Chỉnh sửa định mức nguyên vật liệu',
      width: '50%',
      baseZIndex: 999,
      contentStyle: {
        "min-height": "380px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let productBillOfMaterial: productBillOfMaterialsModel = result.productBillOfMaterial;
        /* update bản ghi */
        let index = this.listProductBillOfMaterials.findIndex(e => e == rowData);
        this.listProductBillOfMaterials[index] = productBillOfMaterial;
        this.listProductBillOfMaterials = [...this.listProductBillOfMaterials];
      }
    });
  }

  removeProductBOM(rowData: productBillOfMaterialsModel) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa nguyên vật liệu này?',
      accept: () => {
        this.listProductBillOfMaterials = this.listProductBillOfMaterials.filter(e => e != rowData);
      }
    });
  }
  /* end */

  scroll(el: HTMLElement) {
    el.scrollIntoView(true);
  }

  /* Chuyển item lên một cấp */
  moveUp(data: ProductVendorMappingModel) {
    let currentOrderNumber = data.OrderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listProductVendorMapping.find(x => x.OrderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = preOrderNumber;

    //Xóa 2 item
    this.listProductVendorMapping = this.listProductVendorMapping.filter(x =>
      x.OrderNumber != preOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listProductVendorMapping = [...this.listProductVendorMapping, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listProductVendorMapping.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: ProductVendorMappingModel) {
    let currentOrderNumber = data.OrderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listProductVendorMapping.find(x => x.OrderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.OrderNumber = currentOrderNumber;
    data.OrderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listProductVendorMapping = this.listProductVendorMapping.filter(x =>
      x.OrderNumber != nextOrderNumber && x.OrderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listProductVendorMapping = [...this.listProductVendorMapping, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listProductVendorMapping.sort((a, b) =>
      (a.OrderNumber > b.OrderNumber) ? 1 : ((b.OrderNumber > a.OrderNumber) ? -1 : 0));
  }
}

function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() !== "") {
        let duplicateCode = array.find(e => e.trim().toLowerCase() === control.value.trim().toLowerCase());
        if (duplicateCode !== undefined) {
          return { 'duplicateCode': true };
        }
      }
    }
    return null;
  }
}

function checkEmptyControl(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (array.length === 0) {
      return { 'checkEmptyControl': true };
    }
    return null;
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

function checkBlankString(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      if (control.value.trim() === "") {
        return { 'blankString': true };
      }
    }
    return null;
  }
}

function ValidationMaxValue(max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      let value = ParseStringToFloat(control.value);
      if (value > max) return { 'maxValue': true }
    }
    return null;
  }
}

