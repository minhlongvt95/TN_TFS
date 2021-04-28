import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { OrderComponent } from './order.component';
import { ListOrderComponent } from '../order/components/list-order/list-order.component';
import { CreateComponent } from '../order/components/create/create.component';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { ListProfitAccordingCustomerComponent } from './components/list-profit-according-customers/list-profit-according-customers.component';
import { OrderServiceCreateComponent } from './components/order-service-create/order-service-create.component';
import { PayOrderServiceComponent } from './components/pay-order-service/pay-order-service.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: OrderComponent,
        children: [
          {
            path: 'list',
            component: ListOrderComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'create',
            component: CreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'order-detail',
            component: OrderDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-profit-according-customers',
            component: ListProfitAccordingCustomerComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'order-service-create',
            component: OrderServiceCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'pay-order-service',
            component: PayOrderServiceComponent,
            canActivate: [AuthGuard]
          },
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class OrderRouting { }
