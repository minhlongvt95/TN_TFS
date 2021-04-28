import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule } from 'ngx-loading';
//import { TreeViewComponent } from '@syncfusion/ej2-angular-navigations';
import { WarehouseRouting } from './warehouse.routing';

/* Services */
import { CategoryService } from "../shared/services/category.service";
import { WarehouseService } from "./services/warehouse.service";
import { EmployeeService} from "../employee/services/employee.service";
import { VendorService } from "./../vendor/services/vendor.service";
import { ProductCategoryService } from '../admin/components/product-category/services/product-category.service';
import { CustomerService } from '../customer/services/customer.service';
import { ProductService } from '../product/services/product.service';
import { GetPermission } from '../shared/permission/get-permission';
import { ImageUploadService } from '../shared/services/imageupload.service';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';

/* End */

/* Component */
import { WarehouseComponent } from './warehouse.component';
import { WarehouseCreateUpdateComponent } from './components/warehouse/warehouse-create-update/warehouse-create-update.component';
import { WarehouseListComponent } from './components/warehouse/warehouse-list/warehouse-list.component';
import { InventoryReceivingVoucherCreateUpdateComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-create-update/inventory-receiving-voucher-create-update.component';
import { PopupCreateSerialComponent } from './components/serial/pop-create-serial/pop-create-serial.component';
import { TreeWarehouseComponent } from './components/tree-warehouse/tree-warehouse.component';
import { InventoryReceivingVoucherListComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-list/inventory-receiving-voucher-list.component';
import { AddProductComponent } from './components/add-product/add-product.component';
import { InventoryDeliveryVoucherCreateUpdateComponent } from './components/inventory-delivery-voucher/inventory-delivery-voucher-create-update/inventory-delivery-voucher-create-update.component';
import { InventoryDeliveryVoucherListComponent } from './components/inventory-delivery-voucher/inventory-delivery-voucher-list/inventory-delivery-voucher-list.component';
/* End */
import { PanelModule } from 'primeng/panel';
import { FieldsetModule } from 'primeng/fieldset';
import { AccordionModule } from 'primeng/accordion';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { TableModule } from 'primeng/table';
import { CalendarModule } from 'primeng/calendar';
import { MultiSelectModule } from 'primeng/multiselect';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TreeModule } from 'primeng/tree';
import { DialogModule } from 'primeng/dialog';
import { EditorModule } from 'primeng/editor';
import { FileUploadModule } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { SidebarModule } from 'primeng/sidebar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

import { DeliveryvoucherCreateSerialComponent } from './components/serial/deliveryvoucher-create-serial/deliveryvoucher-create-serial.component';
import { InStockReportComponent } from './components/in-stock-report/in-stock-report.component';
import { InventoryReceivingVoucherCreateComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-create/inventory-receiving-voucher-create.component';
import { InventoryReceivingVoucherDetailComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-detail/inventory-receiving-voucher-detail.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    WarehouseRouting,
    FormsModule,
    ReactiveFormsModule,
    //TreeViewModule,
    PanelModule,
    FieldsetModule,
    AccordionModule,
    RadioButtonModule,
    InputTextModule,
    DropdownModule,
    AutoCompleteModule,
    TableModule,
    CalendarModule,
    MultiSelectModule,
    DynamicDialogModule,
    TreeModule,
    DialogModule,
    EditorModule,
    FileUploadModule,
    ToastModule,
    SidebarModule,
    ConfirmDialogModule,
    NgxLoadingModule.forRoot({}),   
  ],
  declarations: [
    WarehouseComponent,
    WarehouseCreateUpdateComponent,
    WarehouseListComponent,
    //TreeViewComponent
    PopupCreateSerialComponent,
    TreeWarehouseComponent,
    InventoryReceivingVoucherCreateUpdateComponent,
    InventoryReceivingVoucherListComponent,
    AddProductComponent,
    InventoryDeliveryVoucherCreateUpdateComponent,
    InventoryDeliveryVoucherListComponent,
    DeliveryvoucherCreateSerialComponent,
    InStockReportComponent,
    InventoryReceivingVoucherCreateComponent,
    InventoryReceivingVoucherDetailComponent,
  ],
  providers: [
    WarehouseComponent, 
    WarehouseService,
    VendorService,
    CategoryService,
    DynamicDialogRef,
    DialogService,
    MessageService,
    EmployeeService,
    ProductCategoryService,
    CustomerService,
    ProductService,
    GetPermission,
    ConfirmationService,
    ImageUploadService,
    ForderConfigurationService
  ],
  bootstrap: [PopupCreateSerialComponent, TreeWarehouseComponent],
  entryComponents: [PopupCreateSerialComponent, TreeWarehouseComponent, AddProductComponent, DeliveryvoucherCreateSerialComponent],
})
export class WarehouseModule { }
