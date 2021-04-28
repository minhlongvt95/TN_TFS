import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ProductComponent } from './product.component';
import { ListProductComponent } from './components/list-product/list-product.component';
import { ProductRouting } from './product.routing';
import { ProductService } from './services/product.service';
import { VendorService } from "../vendor/services/vendor.service";
import { WarehouseService } from "../warehouse/services/warehouse.service";
import { VendorModel } from "../vendor/models/vendor.model";
import { ProductCategoryService } from "../admin/components/product-category/services/product-category.service";
import { CreateProductComponent } from './components/create-product/create-product.component';
import { CategoryService } from '../shared/services/category.service';
import { DetailProductComponent } from './components/detail-product/detail-product.component';
import { QuickCreateVendorComponent } from './components/quick-create-vendor/quick-create-vendor.component';
import { GetPermission } from '../shared/permission/get-permission';
import { AddEditSerialComponent } from './components/add-edit-serial/add-edit-serial.component';
import { VendorDetailDialogComponent } from './components/vendor-detail-dialog/vendor-detail-dialog.component';
import { PriceListComponent } from './components/price-list/price-list.component';
import { ProductBomDialogComponent } from './components/product-bom-dialog/product-bom-dialog.component';
@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    ProductRouting,
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [
    ProductComponent, 
    ListProductComponent, 
    CreateProductComponent, 
    DetailProductComponent, 
    QuickCreateVendorComponent, 
    AddEditSerialComponent, 
    VendorDetailDialogComponent, 
    PriceListComponent, 
    ProductBomDialogComponent
  ],
  bootstrap: [
    QuickCreateVendorComponent, 
    VendorDetailDialogComponent
  ],
  entryComponents: [
    QuickCreateVendorComponent, 
    AddEditSerialComponent, 
    VendorDetailDialogComponent, 
    PriceListComponent, 
    ProductBomDialogComponent
  ],
  providers: [
    ProductComponent, 
    ListProductComponent, 
    ProductService, 
    VendorService, 
    ProductCategoryService, 
    CategoryService, 
    GetPermission,
    WarehouseService
  ]
})
export class ProductModule { }
