import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ProductComponent } from './product.component';
import { AuthGuard } from '../shared/guards/auth.guard';

import { ListProductComponent } from './components/list-product/list-product.component';
import { CreateProductComponent } from './components/create-product/create-product.component';
import { DetailProductComponent } from './components/detail-product/detail-product.component';
import { PriceListComponent } from './components/price-list/price-list.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ProductComponent,
        children: [
          {
            path: 'list',
            component: ListProductComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'create',
            component: CreateProductComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: DetailProductComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'price-list',
            component: PriceListComponent,
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
export class ProductRouting {
}
