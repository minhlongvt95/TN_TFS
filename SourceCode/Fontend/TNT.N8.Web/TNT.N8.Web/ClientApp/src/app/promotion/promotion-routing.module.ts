import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';
import { PromotionComponent } from './promotion.component';
import { PromotionCreateComponent } from './components/promotion-create/promotion-create.component';
import { PromotionListComponent } from './components/promotion-list/promotion-list.component';
import { PromotionDetailComponent } from './components/promotion-detail/promotion-detail.component';

const routes: Routes = [];

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: PromotionComponent,
        children: [
          {
            path: 'create',
            component: PromotionCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list',
            component: PromotionListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: PromotionDetailComponent,
            canActivate: [AuthGuard]
          }
        ]
      }
    ])
  ],
  exports: [RouterModule]
})
export class PromotionRoutingModule { }
