import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';
import { BillSaleComponent } from './bill-sale.component';
import { BillSaleCreateComponent } from './components/bill-sale-create/bill-sale-create.component';
import { BillSaleDetailComponent } from './components/bill-sale-detail/bill-sale-detail.component';
import { BillSaleListComponent } from './components/bill-sale-list/bill-sale-list.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: BillSaleComponent,
        children: [
          {
            path: 'list',
            component: BillSaleListComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'create',
            component: BillSaleCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: BillSaleDetailComponent,
            canActivate: [AuthGuard]
          },
          // {
          //   path: 'dashboard',
          //   component: SaleBiddingDashboardComponent,
          //   canActivate: [AuthGuard]
          // },
          // {
          //   path: 'approved',
          //   component: SaleBiddingApprovedComponent,
          //   canActivate: [AuthGuard]
          // }
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
 
})
export class BillSaleRouting { }
