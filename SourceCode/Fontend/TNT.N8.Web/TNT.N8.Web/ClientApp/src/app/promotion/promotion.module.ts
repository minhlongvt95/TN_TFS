import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

import { PromotionRoutingModule } from './promotion-routing.module';
import { PromotionComponent } from './promotion.component';
import { PromotionCreateComponent } from './components/promotion-create/promotion-create.component';

import { PromotionService } from './services/promotion.service';
import { PromotionListComponent } from './components/promotion-list/promotion-list.component';
import { PromotionDetailComponent } from './components/promotion-detail/promotion-detail.component';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';

@NgModule({
  declarations: [
    PromotionComponent,
    PromotionCreateComponent,
    PromotionListComponent,
    PromotionDetailComponent
  ],
  imports: [
    CommonModule,
    PromotionRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    NgMultiSelectDropDownModule.forRoot(),
  ],
  providers: [
    PromotionService,
    ForderConfigurationService
  ],
  schemas: [
    NO_ERRORS_SCHEMA
  ]
})
export class PromotionModule { }
