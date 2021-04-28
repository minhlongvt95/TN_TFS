import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SalesComponent } from './sales.component';
import { SalesDashboardComponent } from './components/sales-dashboard/sales-dashboard.component';
import { TopRevenueComponent } from './components/statistic/top-revenue/top-revenue.component';
import { AuthGuard } from '../shared/guards/auth.guard';
import { ContractCreateComponent } from './components/contract-create/contract-create.component';
import { ContractDetailComponent } from './components/contract-detail/contract-detail.component';
import { ContractListComponent } from './components/contract-list/contract-list.component';
import { ContractDashboardComponent } from './components/contract-dashboard/contract-dashboard.component';
import { ProductRevenueComponent } from './components/statistic/product-revenue/product-revenue.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: SalesComponent,
        children: [
          {
            path: 'dashboard',
            component: SalesDashboardComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'top-revenue',
            component: TopRevenueComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'contract-create',
            component: ContractCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'contract-detail',
            component: ContractDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'contract-list',
            component: ContractListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'contract-dashboard',
            component: ContractDashboardComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-revenue',
            component: ProductRevenueComponent,
            canActivate: [AuthGuard]
          }
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class SalesRouting {
}
