import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SaleBiddingRouting } from './sale-bidding.routing';
import { SaleBiddingComponent } from './sale-bidding.component';
import { SaleBiddingCreateComponent } from './components/sale-bidding-create/sale-bidding-create.component';
import { SaleBiddingListComponent } from './components/sale-bidding-list/sale-bidding-list.component';
import { SaleBiddingDashboardComponent } from './components/sale-bidding-dashboard/sale-bidding-dashboard.component';

import { GetPermission } from '../shared/permission/get-permission';

import { TabMenuModule } from 'primeng/tabmenu';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { SaleBiddingService } from './services/sale-bidding.service';
import { SaleBiddingDialogComponent } from './components/sale-bidding-dialog/sale-bidding-dialog.component'

import { ProductService } from '../product/services/product.service';
import { SaleBiddingDetailComponent } from './components/sale-bidding-detail/sale-bidding-detail.component';
import { CustomerService } from "../customer/services/customer.service";
import { TemplateQuickEmailComponent } from '../customer/components/template-quick-email/template-quick-email.component';
import { TemplateQuickSmsComponent } from '../customer/components/template-quick-sms/template-quick-sms.component';
import { TemplateQuickGiftComponent } from '../customer/components/template-quick-gift/template-quick-gift.component';
import { MeetingDialogComponent } from '../customer/components/meeting-dialog/meeting-dialog.component';
import { CustomerCareService } from '../customer/services/customer-care.service';
import { CustomerModule } from '../customer/customer.module';
import { CategoryService } from '../shared/services/category.service';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { SaleBiddingApprovedComponent } from './components/sale-bidding-approved/sale-bidding-approved.component';
import { StepsModule } from 'primeng/steps';
import { SaleBiddingVendorDialogComponent } from './components/sale-bidding-vendor-dialog/sale-bidding-vendor-dialog.component';

@NgModule({

  imports: [
    CommonModule,
    SaleBiddingRouting,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    TabMenuModule,
    CustomerModule,
    StepsModule
  ],
  declarations: [
    SaleBiddingComponent,
    SaleBiddingCreateComponent,
    SaleBiddingListComponent,
    SaleBiddingDashboardComponent,
    SaleBiddingDialogComponent,
    SaleBiddingDetailComponent,
    SaleBiddingApprovedComponent,
    SaleBiddingVendorDialogComponent,
  ],
  bootstrap: [],
  entryComponents: [
    SaleBiddingDialogComponent,
    TemplateQuickEmailComponent,
    TemplateQuickSmsComponent,
    TemplateQuickGiftComponent,
    MeetingDialogComponent,
    SaleBiddingVendorDialogComponent
  ],
  providers: [
    SaleBiddingComponent,
    GetPermission,
    MessageService,
    ConfirmationService,
    DialogService,
    SaleBiddingService,
    ProductService,
    CustomerService,
    CustomerCareService,
    CategoryService,
    ForderConfigurationService
  ]
})
export class SaleBiddingModule { }
