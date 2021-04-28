import { Component, OnInit, ViewChild, ElementRef, HostListener, Renderer2 } from '@angular/core';
import { AddEditSerialComponent } from '../add-edit-serial/add-edit-serial.component';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  DetailProductModel,
  ProductModel2,
  productMoneyUnitModel,
  productUnitModel,
  vendorModel,
  warehouseModel,
  ProductQuantityInWarehouseModel,
  ProductAttributeCategory,
  ProductAttributeCategoryValueModel,
  Serial,
  InventoryReportModel
} from '../../models/product.model';
import * as $ from 'jquery';

import { ProductService } from '../../services/product.service';
import { TreeProductCategoryComponent } from '../tree-product-category/tree-product-category.component';
import { QuickCreateVendorComponent } from '../quick-create-vendor/quick-create-vendor.component';
import { VendorModel } from '../../../vendor/models/vendor.model';
import { ProductModel, ProductVendorMappingModel, ProductAttributeModel } from '../../models/product.model';
import { GetPermission } from '../../../shared/permission/get-permission';
import { ProductCategoryService } from '../../../admin/components/product-category/services/product-category.service';
import { WarehouseService } from "../../../warehouse/services/warehouse.service";

/* primeng api */
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { VendorDetailDialogComponent } from '../vendor-detail-dialog/vendor-detail-dialog.component';
import { ProductBomDialogComponent } from '../product-bom-dialog/product-bom-dialog.component';
/* end primeng api */

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  productVendorMappingModel: ProductVendorMappingModel,
}

interface Data {
  name: string;
  value: string;
}

interface ProductVendorMapping {
  vendorId: string;
  vendorName: string;
  vendorCode: string;
  vendorProductName: string;
  miniumQuantity: number;
  price: number;
  unitPriceId: string;
  moneyUnitName: string;
  fromDate: Date;
  toDate: Date;
  createdById: string;
  createdDate: Date;
  orderNumber: number;
}

class NoteErr {
  public code: string;
  public name: string;
}

class InventoryReport {
  public inventoryReportId: string;
  public warehouseId: string;
  public productId: string;
  public warehouseNameByLevel: string;
  public quantityMinimum: string;
  public quantityMaximum: string;
  public startQuantity: string;
  public openingBalance: string;
  public listSerial: Array<serialModel>;
  public note: string;

  public isError: boolean;

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
  public createdDate: Date;

  constructor() {
    this.serialId = '00000000-0000-0000-0000-000000000000';
    this.statusId = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
  }
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

class productAttributeCategoryModel {
  productAttributeCategoryId: string;
  productAttributeCategoryName: string;
  productAttributeCategoryValue: Array<productAttributeCategoryValueModel>;
  productAttributeCategoryValueDisplay: string;

  constructor() {
    this.productAttributeCategoryValue = [];
  }
}

class productAttributeCategoryValueModel {
  productAttributeCategoryValueId: string;
  productAttributeCategoryValue1: string;
  productAttributeCategoryId: string;
}

class customerOrderModel {
  customerId: string;
  customerName: string;
  orderCode: string;
  orderDate: string;
}

class productImageModel {
  imageName: string;
  imageSize: string;
  imageUrl: string;
  productId: string;
  productImageId: string;
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

class ProductImageResponseModel {
  fileAsBase64: any;
  fileName: string;
  fileType: string;
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
  selector: 'app-detail-product',
  templateUrl: './detail-product.component.html',
  styleUrls: ['./detail-product.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class DetailProductComponent implements OnInit {
  loading: boolean = false;
  paramProductId: string;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listNoteErr: Array<NoteErr> = [
    { code: "required_min_quanty", name: "Không được để trống số lượng tồn kho tối thiểu" },
    { code: "min_quantity_less_max_quantity", name: "Số lượng tối thiểu không được lớn hơn số lượng tối đa" },
    { code: "required_start_quanty", name: "Không được để trống số lượng tồn kho đầu kỳ" }
  ];

  /* Action*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  /*END*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();

  isOpenNotifiError: boolean = false;
  isInvalidForm: boolean = false;
  //master data
  listProperty: Array<productMoneyUnitModel> = [];
  listPriceInventory: Array<productMoneyUnitModel> = [];
  listProductCategory: Array<productCategoryModel> = [];
  listProductMoneyUnit: Array<productMoneyUnitModel> = [];
  listProductUnit: Array<productUnitModel> = [];
  listVendor: Array<vendorModel> = [];
  listWarehouse: Array<warehouseModel> = [];
  listWarehouseLevel0: Array<warehouseModel> = []; //danh sách kho cho tồn kho tối thiểu
  listWarehouseStartQuantity1: Array<warehouseModel> = []; //danh sách kho cho tồn kho đầu kỳ
  listWarehouseStartQuantity: Array<warehouseModel> = []; //danh sách kho cho tồn kho đầu kỳ
  listProductCode: Array<string> = [];
  productCategoryList: Array<productCategoryModel> = [];
  listCustomerOrder: Array<customerOrderModel> = [];
  listProductImage: Array<productImageModel> = [];
  listImageDetail: Array<ProductImageResponseModel> = [];
  canDeleteProduct: boolean;
  //images
  images: image[] = [];
  //table
  rows: number = 10;
  colsFile: any[];
  colsMinQuantity: any;
  colsStartQuantity: any;
  colsAttribute: any;
  colsCustomerOrder: any;
  colVentor: any;
  selectedColumns: any;
  colsProductBOM: any;
  selectedColsProductBOM: any;
  //form controls
  productForm: FormGroup;
  attributeForm: FormGroup;
  //tab Thông tin chung
  selectedProductCategory: productCategoryModel = new productCategoryModel();
  productVendorMapping: ProductVendorMappingModel = new ProductVendorMappingModel();
  listProductVendorMapping: Array<ProductVendorMappingModel> = [];
  //tab Thông tin kho
  selectedWarehouseForMinQuanty: warehouseModel;
  selectedWarehouseStartQuantity: warehouseModel;
  //tab thuộc tính
  listProductAttribute: Array<productAttributeCategoryModel> = [];
  //tab BOM
  listProductBillOfMaterials: Array<productBillOfMaterialsModel> = [];
  //upload
  //listImageUploaded: Array<productImageModel> = [];
  // is possition fiexd
  fixed: boolean = false;
  withFiexd: string = "";
  isSerialNumberRequired: boolean;
  data: Array<Data> = [
    { name: '1561', value: 'NY' },
    { name: '1672', value: 'RM' },
  ];
  @ViewChild('toggleButton') toggleButton: ElementRef;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;

  /*Thông tin kho*/
  listWarehouseInventory: Array<warehouseModel> = [];
  selectedWarehouseInventory: warehouseModel;
  colsInventory: any;
  listInventory: Array<InventoryReport> = [];
  /*End*/

  constructor(
    private translate: TranslateService,
    private el: ElementRef,
    private getPermission: GetPermission,
    private productService: ProductService,
    private router: Router,
    private route: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    public messageService: MessageService,
    private productCategoryService: ProductCategoryService,
    private warehouseService: WarehouseService,
    private renderer: Renderer2,
  ) {
    this.translate.setDefaultLang('vi');
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
          !this.save.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  ngOnInit() {
    //Check permission
    let resource = "sal/product/detail";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      this.route.params.subscribe(params => {
        this.paramProductId = params['productId'];
      });
      this.initTable();
      this.initForm();
      this.getMasterData()
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

  initTable() {
    //Bảng tồn kho
    this.colsInventory = [
      { field: 'warehouseNameByLevel', header: 'Vị trí', textAlign: 'left', width: '120px', color: '#f44336' },
      { field: 'quantityMinimum', header: 'Số lượng tồn kho tối thiểu', textAlign: 'right', width: '180px', color: '#f44336' },
      { field: 'quantityMaximum', header: 'Số lượng tồn kho tối đa', textAlign: 'right', width: '180px', color: '#f44336' },
      { field: 'startQuantity', header: 'Số lượng tồn kho đầu kỳ', textAlign: 'right', width: '180px', color: '#f44336' },
      { field: 'openingBalance', header: 'Dư đầu', textAlign: 'right', width: '180px', color: '#f44336' },
      { field: 'addSerial', header: 'Số Serial', textAlign: 'center', width: '120px', color: '#f44336' },
      // { field: 'note', header: 'Ghi chú', textAlign: 'left', width: '120px', color: '#f44336' },
      { field: 'actions', header: 'Thao tác', textAlign: 'center', width: '100px', color: '#f44336' },
    ];

    //bảng thuộc tính
    this.colsAttribute = [
      { field: 'productAttributeCategoryName', header: 'Tên thuộc tính', textAlign: 'left' },
      { field: 'productAttributeCategoryValueDisplay', header: 'Giá trị', textAlign: 'left' },
    ];

    //bảng customer order
    this.colsCustomerOrder = [
      { field: 'customerName', header: 'Tên khách hàng', textAlign: 'left' },
      { field: 'orderCode', header: 'Mã đơn hàng', textAlign: 'left' },
      { field: 'orderDate', header: 'Ngày tạo đơn hàng', textAlign: 'right' },
    ];

    this.colsFile = [
      { field: 'imageName', header: 'Tên ảnh', width: '50%', textAlign: 'left' },
      { field: 'imageSize', header: 'Kích thước', width: '50%', textAlign: 'left' },
    ];

    this.colVentor = [
      { field: 'Move', header: '#', textAlign: 'center', width: '20px' },
      { field: 'VendorName', header: 'Tên NCC', textAlign: 'left', width: '100px' },
      { field: 'VendorProductName', header: 'Sản phẩm phía NCC', textAlign: 'left', width: '100px' },
      { field: 'FromDate', header: 'Ngày hiệu lực từ', textAlign: 'right', width: '100px' },
      { field: 'ToDate', header: 'Ngày hiệu lực đến', textAlign: 'right', width: '100px' },
      { field: 'MiniumQuantity', header: 'Số lượng tối thiểu', textAlign: 'right', width: '100px' },
      { field: 'Price', header: 'Giá', textAlign: 'right', width: '100px' },
      { field: 'MoneyUnitName', header: 'Tiền tệ', textAlign: 'left', width: '100px' },
    ]
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
      'VAT': new FormControl('0', [ValidationMaxValue(100)]),
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

  resetAttributeForm() {
    this.attributeForm.reset();
    this.attributeForm.patchValue({
      'AttributeName': '',
      'AttributeValues': []
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  async getMasterData() {
    this.loading = true;
    //let result: any = await this.productService.getMasterdataCreateProduct();
    let [masterdataResponse, productByIdResponse, productCategoryResponse]: any = await Promise.all([
      this.productService.getMasterdataCreateProduct(),
      this.productService.getProductByIDAsync(this.paramProductId),
      this.productCategoryService.getAllProductCategoryAsync()
    ]);

    if (masterdataResponse.statusCode === 200 && productByIdResponse.statusCode === 200 && productCategoryResponse.statusCode === 200) {
      //master data
      this.listProductImage = productByIdResponse.listProductImage;
      let listImageUrl = this.listProductImage.map(e => e.imageUrl);
      let imageResult: any = await this.productService.downloadProductImage(listImageUrl);
      this.loading = false;
      this.listImageDetail = imageResult.listProductImageResponseModel;
      this.convertBase64ToImage(this.listImageDetail);
      this.mappingImageValue();
      this.listProductCode = masterdataResponse.listProductCode;
      this.listProductMoneyUnit = masterdataResponse.listProductMoneyUnit;
      this.listProductUnit = masterdataResponse.listProductUnit;
      this.listVendor = masterdataResponse.listVendor;
      this.listWarehouse = masterdataResponse.listWarehouse;
      this.listProperty = masterdataResponse.listProperty;
      this.listPriceInventory = masterdataResponse.listPriceInventory;
      this.listWarehouseLevel0 = this.listWarehouse.filter(e => e.warehouseParent == null); //lấy kho nút 0
      //product category
      this.productCategoryList = productCategoryResponse.productCategoryList;

      this.getListWarehouseStartQuantity();
      //product by Id
      let productModel: DetailProductModel = productByIdResponse.product;
      // this.listProductVendorMapping = productByIdResponse.lstProductVendorMapping;
      let tempProductVendorMapping: Array<ProductVendorMapping> = productByIdResponse.lstProductVendorMapping;
      tempProductVendorMapping.forEach((item, index) => {
        let temp = new ProductVendorMappingModel();
        let vendor = this.listVendor.find(c => c.vendorId == item.vendorId);
        let unitMoney = this.listProductMoneyUnit.find(c => c.categoryId == item.unitPriceId);
        if (vendor) {
          temp.VendorName = vendor.vendorName;
          temp.VendorCode = vendor.vendorCode;
        }
        temp.VendorId = item.vendorId;
        temp.VendorProductName = item.vendorProductName;
        temp.Price = item.price;
        temp.MoneyUnitId = item.unitPriceId;
        if (unitMoney) {
          temp.MoneyUnitName = unitMoney.categoryName;
        }
        temp.MiniumQuantity = item.miniumQuantity;
        temp.FromDate = item.fromDate;
        temp.ToDate = item.toDate;
        temp.CreatedById = item.createdById;
        temp.CreatedDate = item.createdDate;
        temp.OrderNumber = item.orderNumber ? item.orderNumber : index + 1;

        this.listProductVendorMapping.push(temp);
      });

      this.listCustomerOrder = productByIdResponse.lstCustomerOrder;
      this.listProductBillOfMaterials = productByIdResponse.listProductBillOfMaterials;
      this.listProductBillOfMaterials?.forEach(bom => {
        bom.effectiveFromDate = bom.effectiveFromDate ? new Date(bom.effectiveFromDate) : null;
        bom.effectiveToDate = bom.effectiveToDate ? new Date(bom.effectiveToDate) : null;
      });

      this.setInventoryReportData(productByIdResponse.listInventory);
      this.mappingModelToForm(productModel);//tab thông tin chung
      let listAttribute: Array<productAttributeCategoryModel> = productByIdResponse.lstProductAttributeCategory;
      this.mappingToAttributeTable(listAttribute);
      this.canDeleteProduct = productByIdResponse.canDelete;
      //set duplicate code validation
      //remove code của sản phẩm hiện tại trong danh sách check trùng
      this.listProductCode = this.listProductCode.filter(e => e !== productModel.productCode);
      this.productForm.get('ProductCode').setValidators([Validators.required, checkDuplicateCode(this.listProductCode), checkBlankString()]);
      this.productForm.updateValueAndValidity();
    } else if (masterdataResponse.statusCode !== 200) {
      this.loading = false;
      this.clearToast();
      this.showToast('error', 'Thông báo', masterdataResponse.messageCode);
    } else if (productByIdResponse.statusCode !== 200) {
      this.loading = false;
      this.clearToast();
      this.showToast('error', 'Thông báo', productByIdResponse.messageCode);
    } else if (productCategoryResponse.statusCode !== 200) {
      this.loading = false;
      this.clearToast();
      this.showToast('error', 'Thông báo', productCategoryResponse.messageCode);
    }
  }

  setInventoryReportData(listInventory: Array<InventoryReport>) {
    listInventory.forEach(item => {
      let newRecord = new InventoryReport();
      newRecord.inventoryReportId = item.inventoryReportId;
      newRecord.warehouseId = item.warehouseId;
      newRecord.productId = item.productId;
      newRecord.warehouseNameByLevel = this.listWarehouseInventory.find(e => e.warehouseId == item.warehouseId).warehouseNameByLevel;
      newRecord.quantityMinimum = item.quantityMinimum.toString();
      newRecord.quantityMaximum = item.quantityMaximum == null ? '' : item.quantityMaximum.toString();
      newRecord.startQuantity = item.startQuantity.toString();
      newRecord.openingBalance = item.openingBalance.toString();
      newRecord.note = item.note;
      newRecord.listSerial = item.listSerial;
      
      //Thêm vào list
      this.listInventory = [... this.listInventory, newRecord];

      //xóa kho trong drop down list nếu kho đã tồn tại trong bảng
      this.listWarehouseInventory = this.listWarehouseInventory.filter(e => e.warehouseId !== item.warehouseId);
    });
  }

  convertBase64ToImage(listProductImage: Array<ProductImageResponseModel>) {
    this.images = [];
    listProductImage?.forEach(productImage => {
      //add to pri
      let imageType = "data:" + productImage.fileType + ";base64,";
      let imageSource = imageType + productImage.fileAsBase64;
      let newImage: image = new image();
      newImage.source = imageSource; //base64
      this.images = [...this.images, newImage];
    });
  }

  mappingImageValue() {
    this.listProductImage?.forEach(image => {
      let detailImage = this.listImageDetail?.find(imageDetail => imageDetail.fileName == image.imageName);

      if (detailImage) {
        let imageType = "data:" + detailImage.fileType + ";base64,";
        let imageSource = imageType + detailImage.fileAsBase64;
        image.imageUrl = imageSource;
      }
    });
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
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

  mappingModelToForm(productModel: DetailProductModel) {
    this.selectedProductCategory = this.productCategoryList.find(e => e.productCategoryId == productModel.productCategoryId);
    let listParentId = this.getListParentProductCategoryId(this.selectedProductCategory, []);
    let listParentName: Array<string> = [];
    listParentId.forEach(productcategoryId => {
      let productcategoryById = this.productCategoryList.find(e => e.productCategoryId == productcategoryId);
      listParentName.push(productcategoryById.productCategoryName);
    });


    this.selectedProductCategory.productCategoryNameByLevel = listParentName.reverse().join(' > ');
    this.productForm.get('ProductCategory').setValue(this.selectedProductCategory.productCategoryNameByLevel);
    this.productForm.get('ProductCode').setValue(productModel.productCode);
    this.productForm.get('ProductName').setValue(productModel.productName);
    this.productForm.get('ProductUnit').setValue(this.listProductUnit.find(e => e.categoryId === productModel.productUnitId));
    this.productForm.get('VAT').setValue(productModel.vat);
    this.productForm.get('ImportTax').setValue(productModel.importTax);
    this.productForm.get('ExWarehousePrice').setValue(productModel.exWarehousePrice);
    this.productForm.get('GuaranteeTime').setValue(productModel.guaranteeTime);
    this.productForm.get('Description').setValue(productModel.productDescription);
    this.productForm.get('InventoryPrices').setValue(this.listPriceInventory.find(c => c.categoryId === productModel.calculateInventoryPricesId));
    this.productForm.get('Property').setValue(this.listProperty.find(c => c.categoryId === productModel.propertyId));
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

  getListParentProductCategoryId(currentProductCategory: productCategoryModel, listParentIdReponse: Array<string>): Array<string> {
    listParentIdReponse.push(currentProductCategory.productCategoryId);
    let parent = this.productCategoryList.find(e => e.productCategoryId == currentProductCategory.parentId);
    if (parent === undefined) return listParentIdReponse;
    listParentIdReponse.push(parent.productCategoryId);
    //find parent of parent
    let parentOfParent = this.productCategoryList.find(e => e.productCategoryId == parent.parentId);
    if (parentOfParent === undefined) {
      return listParentIdReponse;
    } else {
      this.getListParentProductCategoryId(parentOfParent, listParentIdReponse);
    }
    return listParentIdReponse;
  }

  mappingToAttributeTable(listAttribute: Array<productAttributeCategoryModel>) {
    listAttribute.forEach(attribute => {
      var listName = attribute.productAttributeCategoryValue.map(e => e.productAttributeCategoryValue1);
      attribute.productAttributeCategoryValueDisplay = listName.join(', ')
    });

    listAttribute.forEach(attribute => {
      this.listProductAttribute = [...this.listProductAttribute, attribute];
    });
  }

  openProductCategoryDialog() {
    let ref = this.dialogService.open(TreeProductCategoryComponent, {
      header: 'Thông tin danh mục',
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
          this.selectedProductCategory = result.productCategory;
          this.productForm.get('ProductCategory').setValue(this.selectedProductCategory.productCategoryNameByLevel)
        }
      }
    });
  }

  onCreateNewVendor() {
    let ref = this.dialogService.open(QuickCreateVendorComponent, {
      header: 'Tạo nhanh nhà cung cấp',
      width: '40%',
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
          this.listVendor = [newVendor, ...this.listVendor];
          let listOldVendors = this.productForm.get('Vendors').value;
          let listNewVendors = [newVendor, ...listOldVendors];
          this.productForm.get('Vendors').setValue(listNewVendors);
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

    //tạo Báo cáo tồn kho
    let newItem = new ProductQuantityInWarehouseModel();
    newItem.WarehouseId = selectedItem.warehouseId;
    newItem.ProductId = this.paramProductId;

    this.warehouseService.createItemInventoryReport(newItem).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        //Thêm vào danh sách Tồn kho
        inventory.inventoryReportId = result.inventoryReportId;
        this.listInventory = [...this.listInventory, inventory];

        //Xóa vị trí đã chọn trong DropdownList
        this.listWarehouseInventory = this.listWarehouseInventory.filter(e => e.warehouseId !== selectedItem.warehouseId);

        this.clearToast();
        this.showToast('success', 'Thông báo', 'Thêm thành công');
      }
      else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  /*Sửa số tồn kho*/
  updateItemReventory(rowData: InventoryReport) {
    rowData.isError = false;

    /*Kiểm tra số tồn kho tối thiểu và tối đa*/
    if (rowData.quantityMinimum == '' || !rowData.quantityMinimum) {
      rowData.quantityMinimum = '0';
    }

    let quantityMinimum = ParseStringToFloat(rowData.quantityMinimum);

    if (rowData.quantityMaximum != '' && rowData.quantityMaximum) {
      let quantityMaximum = ParseStringToFloat(rowData.quantityMaximum);

      if (quantityMinimum > quantityMaximum) {
        rowData.isError = true;
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Số tồn kho tối thiểu lớn hơn Số tồn kho tối đa' };
        this.showMessage(msg);
      }
    }
    else {
      rowData.quantityMaximum = null;
    }
    /*End*/

    /*Kiểm tra Số tồn kho đầu kỳ*/
    let quantitySerial = rowData.listSerial.length;
    if (rowData.startQuantity == '' || !rowData.startQuantity) {
      if (quantitySerial == 0) {
        rowData.startQuantity = '0';
      }
      else {
        rowData.startQuantity = quantitySerial.toString();
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Đã tồn tại Số serial' };
        this.showMessage(msg);
      }
    }
    else {
      let startQuantity = ParseStringToFloat(rowData.startQuantity);

      if (startQuantity < quantitySerial) {
        rowData.startQuantity = quantitySerial.toString();
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Đã tồn tại Số serial' };
        this.showMessage(msg);
      }
    }
    /*End*/

    /*Kiểm tra Số Dư đầu*/
    if (rowData.openingBalance == '' || !rowData.openingBalance) {
      rowData.openingBalance = '0';
    }
    /*End*/

    /*Nếu không có lỗi thì lưu*/

    if (rowData.isError == false) {
      let item = new ProductQuantityInWarehouseModel();
      item.InventoryReportId = rowData.inventoryReportId;
      item.WarehouseId = rowData.warehouseId;
      item.ProductId = rowData.productId;
      item.QuantityMinimum = ParseStringToFloat(rowData.quantityMinimum);
      item.QuantityMaximum = rowData.quantityMaximum ? ParseStringToFloat(rowData.quantityMaximum) : null;
      item.StartQuantity = ParseStringToFloat(rowData.startQuantity);
      item.OpeningBalance = ParseStringToFloat(rowData.openingBalance);
      item.Note = rowData.note?.trim() ?? "";
      item.ListSerial = [];

      this.warehouseService.updateItemInventoryReport(item).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {

        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }

    /*End*/
  }

  /*Thêm Serial*/
  addSerial(rowData: InventoryReport) {

    let inventoryReport = new InventoryReport();
    inventoryReport.inventoryReportId = rowData.inventoryReportId;
    inventoryReport.warehouseId = rowData.warehouseId;
    inventoryReport.productId = this.paramProductId;
    inventoryReport.warehouseNameByLevel = rowData.warehouseNameByLevel;
    inventoryReport.quantityMinimum = rowData.quantityMinimum;
    inventoryReport.quantityMaximum = rowData.quantityMaximum;
    inventoryReport.startQuantity = rowData.startQuantity;
    inventoryReport.openingBalance = rowData.openingBalance;
    inventoryReport.listSerial = [];

    rowData.listSerial.forEach(item => {
      let convertSerial = new serialModel();

      convertSerial.serialId = item.serialId;
      convertSerial.serialCode = item.serialCode;
      convertSerial.productId = this.paramProductId;
      convertSerial.warehouseId = item.warehouseId;
      convertSerial.statusId = item.statusId;
      convertSerial.createdDate = item.createdDate;

      inventoryReport.listSerial.push(convertSerial);
    });

    let ref = this.dialogService.open(AddEditSerialComponent, {
      header: 'Thêm Serial',
      data:
      {
        inventoryReport: inventoryReport,
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

          let listNewSerial: Array<Serial> = [];
          if (addedProductSerialModel.listSerial.length > 0) {
            addedProductSerialModel.listSerial.forEach(serial => {
              let newSerial = new Serial();
              newSerial.SerialId = serial.serialId;
              newSerial.SerialCode = serial.serialCode;
              newSerial.ProductId = this.paramProductId;
              newSerial.WarehouseId = rowData.warehouseId;
              newSerial.StatusId = serial.statusId;
    
              listNewSerial.push(newSerial);
            });
          }
          
          this.warehouseService.createUpdateSerial(listNewSerial, rowData.warehouseId, this.paramProductId).subscribe(response => {
            let _result: any = response;

            if (_result.statusCode == 200) {
              rowData.listSerial = _result.listSerial;
            }
            else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: _result.messageCode };
              this.showMessage(msg);
            }
          });
        }
      }
    });
  }

  /*Xóa tồn kho*/
  deleteInventory(rowData: InventoryReport) {
    this.confirmationService.confirm({
      message: 'Dữ liệu sau khi xóa sẽ không thể hoàn tác, Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.warehouseService.deleteItemInventoryReport(rowData.inventoryReportId).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.listInventory = this.listInventory.filter(e => e.warehouseId !== rowData.warehouseId);
            let warehouse: warehouseModel = this.listWarehouse.find(e => e.warehouseId == rowData.warehouseId);
            this.listWarehouseInventory = [...this.listWarehouseInventory, warehouse];
            this.selectedWarehouseInventory = null;
          }
          else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
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
      let productAttribute = new productAttributeCategoryModel();
      productAttribute.productAttributeCategoryName = this.attributeForm.get('AttributeName').value;
      let listProductAttributeValue: Array<string> = this.attributeForm.get('AttributeValues').value;
      productAttribute.productAttributeCategoryValueDisplay = listProductAttributeValue.join(', ');
      //add new value to model
      let listProductAttributeValueModel: Array<productAttributeCategoryValueModel> = [];
      listProductAttributeValue.forEach(attributeValue => {
        let newObj: productAttributeCategoryValueModel = new productAttributeCategoryValueModel();
        newObj.productAttributeCategoryId = this.emptyGuid;
        newObj.productAttributeCategoryValueId = this.emptyGuid;
        newObj.productAttributeCategoryValue1 = attributeValue;
        listProductAttributeValueModel.push(newObj)
      });
      productAttribute.productAttributeCategoryId = this.emptyGuid;
      productAttribute.productAttributeCategoryValue = listProductAttributeValueModel;
      this.listProductAttribute = [...this.listProductAttribute, productAttribute];
      this.resetAttributeForm();
    }
  }

  deleteAttribute(rowData: productAttributeCategoryModel) {
    this.listProductAttribute = this.listProductAttribute.filter(e => e !== rowData);
  }

  cancel() {
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
    //   if (rowData.maxQuantity.trim() !== '' && rowData.minQuantity !== '') {
    //     let minQuantity = ParseStringToFloat(rowData.minQuantity);
    //     let maxQuantity = ParseStringToFloat(rowData.maxQuantity);
    //     if (minQuantity > maxQuantity) {
    //       rowData.status = false;
    //       rowData.listNoteErr = [...rowData.listNoteErr, this.listNoteErr.find(e => e.code == 'min_quantity_less_max_quantity').name];
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

  async updateProduct() {
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
        this.showToast('error', 'Thông báo', 'Danh sách tồn kho không hợp lệ');
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
      let listAttribute = this.mappingAttribute(this.listProductAttribute);
      
      let listProductBillOfMaterials = this.getListProductBillOfMaterials();

      this.loading = true;
      let result: any = await this.productService.updateProductAsync(productModel, this.listProductVendorMapping, listAttribute, listProductBillOfMaterials, [], [], this.auth.UserId);
      this.loading = false;
      if (result.statusCode === 200) {
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Chỉnh sửa sản phẩm/dịch vụ thành công');
        this.isInvalidForm = false;
        this.isOpenNotifiError = false;
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    }
  }

  mappingProductForm(): ProductModel2 {
    let newProduct = new ProductModel2();
    newProduct.ProductId = this.paramProductId;
    newProduct.ProductCategoryId = this.selectedProductCategory.productCategoryId;
    newProduct.ProductName = this.productForm.get('ProductName').value;
    newProduct.ProductCode = this.productForm.get('ProductCode').value;
    newProduct.Price1 = 0;
    newProduct.Price2 = 0; //không dùng trường này
    newProduct.ProductUnitId = this.productForm.get('ProductUnit').value.categoryId;
    newProduct.ProductDescription = this.productForm.get('Description').value;
    newProduct.Vat = ParseStringToFloat(this.productForm.get('VAT').value);
    // newProduct.ProductMoneyUnitId = this.productForm.get('MoneyUnit').value.categoryId;
    // newProduct.ExWarehousePrice = ParseStringToFloat(this.productForm.get('ExWarehousePrice').value);
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

  getListVendorId(): Array<string> {
    let listVendorId: Array<string> = [];
    let listVendorValue: Array<vendorModel> = this.productForm.get('Vendors').value;
    listVendorValue.forEach(vendor => {
      listVendorId.push(vendor.vendorId);
    });
    return listVendorId;
  }

  mappingAttribute(listProductAttribute: Array<productAttributeCategoryModel>): Array<ProductAttributeCategory> {
    let listAttribute: Array<ProductAttributeCategory> = [];
    listProductAttribute.forEach(attribute => {

      if (attribute.productAttributeCategoryId !== this.emptyGuid) {
        //giữ nguyên giá trị cũ
        let oldAttribute = new ProductAttributeCategory();
        oldAttribute.ProductAttributeCategoryId = attribute.productAttributeCategoryId;
        oldAttribute.ProductAttributeCategoryName = attribute.productAttributeCategoryName;
        attribute.productAttributeCategoryValue.forEach(attributeValue => {
          let oldAttributeValue = new ProductAttributeCategoryValueModel();
          oldAttributeValue.ProductAttributeCategoryValueId = attributeValue.productAttributeCategoryValueId;
          oldAttributeValue.ProductAttributeCategoryValue1 = attributeValue.productAttributeCategoryValue1;
          oldAttributeValue.ProductAttributeCategoryId = attributeValue.productAttributeCategoryId;
          //default values
          oldAttributeValue.CreatedById = this.emptyGuid;
          oldAttributeValue.CreatedDate = new Date();
          oldAttributeValue.UpdatedById = this.emptyGuid;
          oldAttributeValue.UpdatedDate = null;
          oldAttributeValue.Active = true;

          oldAttribute.ProductAttributeCategoryValue = [...oldAttribute.ProductAttributeCategoryValue, oldAttributeValue];
        });
        //default values
        oldAttribute.CreatedById = this.emptyGuid;
        oldAttribute.CreatedDate = new Date();
        oldAttribute.UpdatedById = null;
        oldAttribute.UpdatedDate = null;
        oldAttribute.Active = true;

        listAttribute = [...listAttribute, oldAttribute];
      } else {
        //tạo mới
        let newAttribute = new ProductAttributeCategory();
        newAttribute.ProductAttributeCategoryId = this.emptyGuid;
        newAttribute.ProductAttributeCategoryName = attribute.productAttributeCategoryName;
        attribute.productAttributeCategoryValue.forEach(attributeValue => {
          let newAttributeValue = new ProductAttributeCategoryValueModel();
          newAttributeValue.ProductAttributeCategoryValueId = this.emptyGuid;
          newAttributeValue.ProductAttributeCategoryValue1 = attributeValue.productAttributeCategoryValue1;
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
      }
    });

    return listAttribute;
  }

  goToDetail(id: string) {
    this.router.navigate(['/order/order-detail', { customerOrderID: id }]);
  }

  goToDetailCustomer(customerId: string) {
    this.router.navigate(['/customer/detail', { customerId: customerId, contactId: this.emptyGuid }]);
  }

  deleteProduct() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa sản phẩm này này?',
      accept: () => {
        this.productService.updateActiveProduct(this.paramProductId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            this.router.navigate(['product/list']);
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
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

  // del_product() {
  //   let _title = "XÁC NHẬN";
  //   let _content = "Bạn có chắc chắn muốn xóa?";
  //   this.dialogConfirm = this.dialog.open(PopupComponent,
  //     {
  //       width: '500px',
  //       height: '300px',
  //       autoFocus: false,
  //       data: { title: _title, content: _content }
  //     });

  //   this.dialogConfirm.afterClosed().subscribe(resultPopup => {
  //     if (resultPopup) {
  //       this.productService.updateActiveProduct(this.paramProductId).subscribe(response => {
  //         let result = <any>response;
  //         if (result.statusCode === 202 || result.statusCode === 200) {
  //           this.snackBar.openFromComponent(SuccessComponent,
  //             {
  //               duration: 5000,
  //               data: result.messageCode,
  //               panelClass: 'success-dialog',
  //               horizontalPosition: 'end'
  //             });
  //           this.router.navigate(['product/list']);
  //         } else {
  //           this.snackBar.openFromComponent(FailComponent,
  //             {
  //               duration: 5000,
  //               data: result.messageCode,
  //               panelClass: 'fail-dialog',
  //               horizontalPosition: 'end'
  //             });
  //         }
  //         this.ngOnInit();
  //       }, error => { });
  //       this.dialogConfirm.close();
  //     }
  //   });
  // }

  // onKeyPress(event: any) {
  //   const pattern = /^[0-9\.]$/;
  //   const inputChar = String.fromCharCode(event.charCode);
  //   if (!pattern.test(inputChar)) {
  //     event.preventDefault();
  //   }
  // }

  uploadImage(event: any) {

  }

  removeImage(event: any) {

  }

  clearImage() {

  }

  deleteFile(rowData: any) {

  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView(true);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function ParseStringToFloat(str: any) {
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
      let value = control.value.toString().replace(/,/g, '');
      if (value > max) return { 'maxValue': true }
    }
    return null;
  }
}
