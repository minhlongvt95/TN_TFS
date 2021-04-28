import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SaleBiddingComponent } from './sale-bidding.component';
import { AuthGuard } from '../shared/guards/auth.guard';

import { SaleBiddingListComponent } from './components/sale-bidding-list/sale-bidding-list.component';
import { SaleBiddingCreateComponent } from './components/sale-bidding-create/sale-bidding-create.component';
import { SaleBiddingDetailComponent } from './components/sale-bidding-detail/sale-bidding-detail.component';
import { SaleBiddingDashboardComponent } from './components/sale-bidding-dashboard/sale-bidding-dashboard.component';
import { SaleBiddingApprovedComponent } from './components/sale-bidding-approved/sale-bidding-approved.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: SaleBiddingComponent,
        children: [
          {
            path: 'list',
            component: SaleBiddingListComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'create',
            component: SaleBiddingCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: SaleBiddingDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'dashboard',
            component: SaleBiddingDashboardComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'approved',
            component: SaleBiddingApprovedComponent,
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
export class SaleBiddingRouting { }
