import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { ProcurementRequestListComponent } from './components/procurement-request-list/procurement-request-list.component';
import { ProcurementRequestCreateComponent } from './components/procurement-request-create/procurement-request-create.component';
import { ProcurementRequestComponent } from './procurement-request.component';
import { ProcurementRequestViewComponent } from './components/procurement-request-view/procurement-request-view.component';
import { ProcurementRequestReportListComponent } from './components/procurement-request-report-list/procurement-request-report-list.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ProcurementRequestComponent,
        children: [
          {
            path: 'create',
            component: ProcurementRequestCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list',
            component: ProcurementRequestListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'view',
            component: ProcurementRequestViewComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-report',
            component: ProcurementRequestReportListComponent,
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
export class ProcurementRequestRouting { }
