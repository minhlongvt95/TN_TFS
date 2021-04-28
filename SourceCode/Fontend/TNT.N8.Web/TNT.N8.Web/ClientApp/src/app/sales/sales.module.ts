import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmployeeModule } from '../employee/employee.module';
import {ChartModule} from 'primeng/chart';

import { SalesRouting } from './sales.routing';

//components
import { SalesComponent } from './sales.component';
import { SalesDashboardComponent } from './components/sales-dashboard/sales-dashboard.component';
import { TopRevenueComponent } from './components/statistic/top-revenue/top-revenue.component';
import { OrgSelectDialogComponent } from '../employee/components/org-select-dialog/org-select-dialog.component';

//services
import { SalesDashboardService } from './components/sales-dashboard/services/sales-dashboard.service';
import { TopRevenueService } from './components/statistic/services/top-revenue.service';
import { OrganizationService } from '../shared/services/organization.service';
import { GetPermission } from '../shared/permission/get-permission';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ContractCreateComponent } from './components/contract-create/contract-create.component';
import { ContractDetailComponent } from './components/contract-detail/contract-detail.component';
import { ContractService } from './services/contract.service';
import { CustomerModule } from '../customer/customer.module';
import { AddEditProductContractDialogComponent } from './components/add-edit-product-contract-dialog/add-edit-product-contract-dialog.component';
import { ContractListComponent } from './components/contract-list/contract-list.component';
import { ContractDashboardComponent } from './components/contract-dashboard/contract-dashboard.component';
import { LeadModule } from '../lead/lead.module';
import { AddEditContractCostDialogComponent } from './components/add-edit-contract-cost-dialog/add-edit-contract-cost-dialog.component';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { ProductRevenueComponent } from './components/statistic/product-revenue/product-revenue.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    SalesRouting,
    FormsModule,
    ReactiveFormsModule,
    EmployeeModule,
    CustomerModule,
    ChartModule,
    LeadModule
  ],
  declarations: [SalesComponent, SalesDashboardComponent, TopRevenueComponent, ContractCreateComponent, ContractDetailComponent, AddEditProductContractDialogComponent, ContractListComponent, ContractListComponent, ContractDashboardComponent, AddEditContractCostDialogComponent, ProductRevenueComponent],
  providers: [
    SalesDashboardService,
    TopRevenueService,
    GetPermission,
    OrganizationService,
    MessageService,
    ConfirmationService,
    DialogService,
    ContractService,
    ForderConfigurationService
  ],
  entryComponents: [
    AddEditProductContractDialogComponent,
    AddEditContractCostDialogComponent
  ],
})
export class SalesModule { }
