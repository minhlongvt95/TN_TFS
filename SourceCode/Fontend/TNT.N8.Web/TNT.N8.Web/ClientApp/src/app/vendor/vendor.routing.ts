import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { VendorComponent } from './vendor.component';
import { AuthGuard } from '../shared/guards/auth.guard';

import { CreateVendorComponent } from './components/create-vendor/create-vendor.component';
import { ListVendorComponent } from './components/list-vendor/list-vendor.component';
import { DetailVendorComponent } from './components/detail-vendor/detail-vendor.component';
import { DetailVendorOrderComponent } from './components/detail-vendor-order/detail-vendor-order.component';
import { CreateVendorOrderComponent } from './components/create-vendor-order/create-vendor-order.component';
import { ListVendorOrderComponent } from './components/list-vendor-order/list-vendor-order.component';
import { ListVendorQuoteComponent } from './components/list-vendor-quote/list-vendor-quote.component';
import { VendorProductPriceComponent } from './components/vendor-product-price/vendor-product-price.component';
import { VendorQuoteCreateComponent } from './components/vendor-quote-create/vendor-quote-create.component';
import { VendorQuoteDetailComponent } from './components/vendor-quote-detail/vendor-quote-detail.component';
import { VendorOrderReportComponent } from './components/vendor-order-report/vendor-order-report.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: VendorComponent,
        children: [
          {
            path: 'create',
            component: CreateVendorComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list',
            component: ListVendorComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: DetailVendorComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'create-order',
            component: CreateVendorOrderComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-order',
            component: ListVendorOrderComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail-order',
            component: DetailVendorOrderComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-vendor-quote',
            component: ListVendorQuoteComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-vendor-price',
            component: VendorProductPriceComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'vendor-quote-create',
            component: VendorQuoteCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'vendor-quote-detail',
            component: VendorQuoteDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'vendor-order-report',
            component: VendorOrderReportComponent,
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
export class VendorRouting {
}
