import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LeadComponent } from './lead.component';
import { AuthGuard } from '../shared/guards/auth.guard';

import { LeadListComponent } from './components/list/list.component';
import { LeadDetailComponent } from './components/detail/detail.component';
import { LeadDashboardComponent } from './components/dashboard/dashboard.component';
import { UnfollowComponent } from './components/unfollow/unfollow.component';
import { ReportComponent } from './components/report/report.component';
import { CreateLeadComponent } from './components/create-lead/create-lead.component';
import { LeadApprovalComponent } from './components/lead-approval/lead-approval.component';
import { ReportLeadComponent } from './components/report-lead/report-lead.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: LeadComponent,
        children: [
          {
            path: 'list',
            component: LeadListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'create-lead',
            component: CreateLeadComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'dashboard',
            component: LeadDashboardComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: LeadDetailComponent,
            canActivate: [AuthGuard]
          },
          // {
          //   path: 'unfollow',
          //   component: UnfollowComponent,
          //   canActivate: [AuthGuard]
          // },
          // {
          //   path: 'report',
          //   component: UnfollowComponent,
          //   canActivate: [AuthGuard]
          // },
          {
            path: 'approval',
            component: LeadApprovalComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'report-lead',
            component: ReportLeadComponent,
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
export class LeadRouting {
}
