import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule } from 'ngx-loading';

import { OrderRouting } from './order.routing';
import { OrderComponent } from './order.component';
import { ListOrderComponent } from '../order/components/list-order/list-order.component';
import { CreateComponent } from './components/create/create.component'
import { EmployeeService } from '../employee/services/employee.service';
import { CustomerService } from '../customer/services/customer.service';
import { VendorService } from "../vendor/services/vendor.service";
import { ProductService } from '../product/services/product.service';
import { CategoryService } from "../shared/services/category.service";
import { CustomerOrderService } from './services/customer-order.service';
import { BankService } from '../shared/services/bank.service';
import { OrderstatusService } from '../shared/services/orderstatus.service';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { ContactService } from '../shared/services/contact.service';
import { QuoteService } from '../customer/services/quote.service';
import { GetPermission } from '../shared/permission/get-permission';
import { SystemParameterService } from '../admin/services/system-parameter.service';
import { CurrencyPipe } from '@angular/common';
import { EmailConfigService } from '../admin/services/email-config.service';
import { NoteService } from '../shared/services/note.service';
import { ImageUploadService } from '../shared/services/imageupload.service';
import { ProductCategoryService } from '../admin/components/product-category/services/product-category.service';

import { CreateorderdetaildialogComponent } from './components/createorderdetaildialog/createorderdetaildialog.component';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { OrderDetailDialogComponent } from './components/order-detail-dialog/order-detail-dialog.component';
// import { AddEditCostQuoteDialogComponent } from '../customer/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { PopupAddEditCostQuoteDialogComponent } from '../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { ListProfitAccordingCustomerComponent } from './components/list-profit-according-customers/list-profit-according-customers.component';
import { OrderServiceCreateComponent } from './components/order-service-create/order-service-create.component';
import { PayOrderServiceComponent } from './components/pay-order-service/pay-order-service.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    OrderRouting,
    FormsModule,
    ReactiveFormsModule,
    NgxLoadingModule.forRoot({}),
  ],
  declarations: [
    OrderComponent, 
    ListOrderComponent, 
    ListProfitAccordingCustomerComponent,
    CreateComponent, 
    OrderDetailComponent, 
    CreateorderdetaildialogComponent, 
    OrderDetailDialogComponent, OrderServiceCreateComponent, PayOrderServiceComponent,
    // AddEditCostQuoteDialogComponent
  ],
  entryComponents: [
    CreateorderdetaildialogComponent,
    OrderDetailDialogComponent,
    PopupAddEditCostQuoteDialogComponent,
  ],
  providers: [
    OrderComponent, 
    ListOrderComponent,
    ListProfitAccordingCustomerComponent, 
    CreateComponent, 
    EmployeeService, 
    CustomerService, 
    VendorService, 
    ProductService, 
    CategoryService,
    CustomerOrderService, 
    BankService, 
    OrderstatusService, 
    ContactService, 
    QuoteService, 
    GetPermission, 
    SystemParameterService, 
    CurrencyPipe, 
    EmailConfigService,
    MessageService,
    ConfirmationService,
    DialogService,
    NoteService,
    ImageUploadService,
    ForderConfigurationService,
    ProductCategoryService
  ],
})
export class OrderModule { }
