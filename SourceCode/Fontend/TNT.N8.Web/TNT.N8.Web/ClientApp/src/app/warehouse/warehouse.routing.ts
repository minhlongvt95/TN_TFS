import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { WarehouseComponent } from './warehouse.component';
import { WarehouseCreateUpdateComponent } from './components/warehouse/warehouse-create-update/warehouse-create-update.component';
import { InventoryReceivingVoucherCreateUpdateComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-create-update/inventory-receiving-voucher-create-update.component';
import { WarehouseListComponent } from './components/warehouse/warehouse-list/warehouse-list.component';
import { InventoryReceivingVoucherListComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-list/inventory-receiving-voucher-list.component';
import { InventoryDeliveryVoucherCreateUpdateComponent } from './components/inventory-delivery-voucher/inventory-delivery-voucher-create-update/inventory-delivery-voucher-create-update.component';
import { InventoryDeliveryVoucherListComponent } from './components/inventory-delivery-voucher/inventory-delivery-voucher-list/inventory-delivery-voucher-list.component';
import { InStockReportComponent } from './components/in-stock-report/in-stock-report.component';
import { InventoryReceivingVoucherCreateComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-create/inventory-receiving-voucher-create.component';
import { InventoryReceivingVoucherDetailComponent } from './components/inventory-receiving-voucher/inventory-receiving-voucher-detail/inventory-receiving-voucher-detail.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: WarehouseComponent,
        children: [
          {
            path: 'warehouse',
            component: WarehouseCreateUpdateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-receiving-voucher/create-update',
            component: InventoryReceivingVoucherCreateUpdateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-receiving-voucher/create',
            component: InventoryReceivingVoucherCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-receiving-voucher/detail',
            component: InventoryReceivingVoucherDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-receiving-voucher/details',
            component: InventoryReceivingVoucherCreateUpdateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-receiving-voucher/list',
            component: InventoryReceivingVoucherListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list',
            component: WarehouseListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-delivery-voucher/create-update',
            component: InventoryDeliveryVoucherCreateUpdateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-delivery-voucher/details',
            component: InventoryDeliveryVoucherCreateUpdateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'inventory-delivery-voucher/list',
            component: InventoryDeliveryVoucherListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'in-stock-report/list',
            component: InStockReportComponent,
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
export class WarehouseRouting { }
