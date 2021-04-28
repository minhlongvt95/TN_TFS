import { NgModule } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { NgxLoadingModule } from 'ngx-loading';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { VendorComponent } from './vendor.component';
import { CreateVendorComponent } from './components/create-vendor/create-vendor.component';
import { CommonService } from '../shared/services/common.service';
import { VendorService } from './services/vendor.service';
import { CategoryService } from '../shared/services/category.service';
import { WardService } from '../shared/services/ward.service';
import { ProvinceService } from '../shared/services/province.service';
import { DistrictService } from '../shared/services/district.service';
import { BankService } from '../shared/services/bank.service';
import { CustomerOrderService } from '../order/services/customer-order.service';
import { ContactService } from '../shared/services/contact.service';
import { VendorRouting } from './vendor.routing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ListVendorComponent } from './components/list-vendor/list-vendor.component';
import { DetailVendorComponent } from './components/detail-vendor/detail-vendor.component';
import { CreateVendorOrderComponent } from './components/create-vendor-order/create-vendor-order.component';
import { EmployeeService } from "../employee/services/employee.service";
import { BankpopupComponent } from "../shared/components/bankpopup/bankpopup.component";
import { ContactpopupComponent } from "../shared/components/contactpopup/contactpopup.component";
import { ProductService } from '../product/services/product.service';
import { OrderstatusService } from "../shared/services/orderstatus.service";
import { ListVendorOrderComponent } from './components/list-vendor-order/list-vendor-order.component';
import { DetailVendorOrderComponent } from './components/detail-vendor-order/detail-vendor-order.component';
import { VendorOrderProductComponent } from './components/vendor-order-product/vendor-order-product.component';
import { ProcurementRequestService } from '../procurement-request/services/procurement-request.service';
import { GetPermission } from '../shared/permission/get-permission';


import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { AddVendorContactDialogComponent } from './components/add-vendor-contact-dialog/add-vendor-contact-dialog.component';
import { AddVendorOrderProductComponent } from './components/add-vendor-order-product/add-vendor-order-product.component';
import { ListVendorQuoteComponent } from './components/list-vendor-quote/list-vendor-quote.component';
import { VendorProductPriceComponent } from './components/vendor-product-price/vendor-product-price.component';
import { AddVendorPriceDialogComponent } from './components/add-vendor-price-dialog/add-vendor-price-dialog.component';
import { ProductModule } from '../product/product.module';
import { ButtonModule } from 'primeng/button';
import { VendorQuoteCreateComponent } from './components/vendor-quote-create/vendor-quote-create.component';
import { VendorQuoteDetailComponent } from './components/vendor-quote-detail/vendor-quote-detail.component';
import { PopupAddEditCostVendorOrderComponent } from './components/popup-add-edit-cost-vendor-order/popup-add-edit-cost-vendor-order.component';
import { SendMailVendorQuoteDialogComponent } from './components/send-mail-vendor-quote-dialog/send-mail-vendor-quote-dialog.component';

import { NoteService } from '../shared/services/note.service';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { VendorOrderReportComponent } from './components/vendor-order-report/vendor-order-report.component';
import { QuickCreateVendorComponent } from '../shared/components/quick-create-vendor/quick-create-vendor.component';

@NgModule({
  imports: [
    SharedModule,
    CommonModule,
    VendorRouting,
    FormsModule,
    ReactiveFormsModule,
    NgxLoadingModule.forRoot({}),
    ProductModule,
    ButtonModule,
  ],
  declarations: [
    VendorComponent,
    CreateVendorComponent,
    ListVendorComponent,
    DetailVendorComponent,
    CreateVendorOrderComponent,
    ListVendorOrderComponent,
    DetailVendorOrderComponent,
    VendorOrderProductComponent,
    AddVendorContactDialogComponent,
    AddVendorOrderProductComponent,
    ListVendorQuoteComponent,
    VendorProductPriceComponent,
    AddVendorPriceDialogComponent,
    VendorQuoteCreateComponent,
    VendorQuoteDetailComponent,
    PopupAddEditCostVendorOrderComponent,
    SendMailVendorQuoteDialogComponent,
    VendorOrderReportComponent
  ],
  providers: [
    ProcurementRequestService,
    CommonService,
    VendorService,
    CategoryService,
    WardService,
    ProvinceService,
    DistrictService,
    GetPermission,
    MatSnackBarConfig,
    ContactService,
    EmployeeService,
    BankService,
    ProductService,
    OrderstatusService,
    CustomerOrderService,
    MessageService,
    ConfirmationService,
    DialogService,
    DecimalPipe,
    DatePipe,
    NoteService,
    ForderConfigurationService
  ],
  entryComponents: [
    BankpopupComponent,
    ContactpopupComponent,
    VendorOrderProductComponent,
    AddVendorContactDialogComponent,
    AddVendorOrderProductComponent,
    AddVendorPriceDialogComponent,
    PopupAddEditCostVendorOrderComponent,
    SendMailVendorQuoteDialogComponent,
    QuickCreateVendorComponent
  ]
})
export class VendorModule { }
