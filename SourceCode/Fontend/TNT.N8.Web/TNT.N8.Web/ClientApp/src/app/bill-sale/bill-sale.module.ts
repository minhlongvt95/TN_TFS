import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { GetPermission } from '../shared/permission/get-permission';

import { TabMenuModule } from 'primeng/tabmenu';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ProductService } from '../product/services/product.service';

import { CustomerService } from "../customer/services/customer.service";
import { TemplateQuickEmailComponent } from '../customer/components/template-quick-email/template-quick-email.component';
import { TemplateQuickSmsComponent } from '../customer/components/template-quick-sms/template-quick-sms.component';
import { TemplateQuickGiftComponent } from '../customer/components/template-quick-gift/template-quick-gift.component';
import { MeetingDialogComponent } from '../customer/components/meeting-dialog/meeting-dialog.component';
import { CustomerCareService } from '../customer/services/customer-care.service';
import { CustomerModule } from '../customer/customer.module';
import { CategoryService } from '../shared/services/category.service';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { StepsModule } from 'primeng/steps';
import { DatePipe } from '@angular/common';

import { BillSaleRouting } from './bill-sale.routing';
import { BillSaleComponent } from './bill-sale.component';
import { BillSaleCreateComponent } from './components/bill-sale-create/bill-sale-create.component';
import { BillSaleDetailComponent } from './components/bill-sale-detail/bill-sale-detail.component';
import { BillSaleListComponent } from './components/bill-sale-list/bill-sale-list.component';
import { BillSaleService } from '../bill-sale/services/bill-sale.service';
import { BillSaleDialogComponent } from './components/bill-sale-dialog/bill-sale-dialog.component';

import { CustomerOrderService } from '../order/services/customer-order.service';


@NgModule({
  imports: [
    CommonModule,
    BillSaleRouting,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    TabMenuModule,
    CustomerModule,
    StepsModule
  ],
  declarations: [
    BillSaleComponent,
    BillSaleCreateComponent,
    BillSaleDetailComponent,
    BillSaleListComponent,
    BillSaleDialogComponent
  ],
  bootstrap: [],
  entryComponents: [
    BillSaleDialogComponent
  ],
  providers: [
    BillSaleComponent,
    BillSaleService,
    DatePipe,
    CustomerOrderService
  ]
})
export class BillSaleModule { }
