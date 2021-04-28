import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CustomerCreateComponent } from './components/customer-create/customer-create.component';
import { CustomerListComponent } from './components/customer-list/customer-list.component';
import { CustomerDetailComponent } from './components/customer-detail/customer-detail.component';
import { CustomerComponent } from './customer.component';
import { CustomerDashboardComponent } from './components/customer-dashboard/customer-dashboard.component';
import { CustomerQuoteCreateComponent } from './components/customer-quote-create/customer-quote-create.component';
import { AuthGuard } from '../shared/guards/auth.guard';
import { CustomerQuoteListComponent } from './components/customer-quote-list/customer-quote-list.component';
import { CustomerQuoteDetailComponent } from './components/customer-quote-detail/customer-quote-detail.component';
import { CustomerCareDashboardComponent } from './components/customer-care-dashboard/customer-care-dashboard.component';
import { CustomerCareCreateComponent } from './components/customer-care-create/customer-care-create.component';
import { FilterTestComponent } from './components/filter-test/filter-test.component';
import { CustomerCareDetailComponent } from './components/customer-care-detail/customer-care-detail.component';
import { CustomerCareListComponent } from './components/customer-care-list/customer-care-list.component';
import { CustomerQuoteDashboardComponent } from './components/customer-quote-dashboard/customer-quote-dashboard.component';
import { CustomerRequestApprovalComponent } from './components/customer-request-approval/customer-request-approval.component';
import { ApprovalQuoteListComponent } from './components/customer-quote-approval/customer-quote-approval.component';
import { PotentialCustomerCreateComponent } from './components/potential-customer-create/potential-customer-create.component'
import { PotentialCustomerDasboardComponent } from './components/potential-customer-dasboard/potential-customer-dasboard.component';
import { PotentialCustomerDetailComponent } from './components/potential-customer-detail/potential-customer-detail.component';
import { PotentialCustomerListComponent } from './components/potential-customer-list/potential-customer-list.component';
import { PotentialCustomerRequestApprovalComponent } from './components/potential-customer-request-approval/potential-customer-request-approval.component';
import { ListCustomerContactComponent } from './components/list-customer-contact/list-customer-contact.component';

@NgModule({
  imports: [RouterModule.forChild([
    {
      path: '',
      component: CustomerComponent,
      children: [
        {
          path: 'create',
          component: CustomerCreateComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'list',
          component: CustomerListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'dashboard',
          component: CustomerDashboardComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'detail',
          component: CustomerDetailComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'quote-create',
          component: CustomerQuoteCreateComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'quote-list',
          component: CustomerQuoteListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'quote-detail',
          component: CustomerQuoteDetailComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'care-dashboard',
          component: CustomerCareDashboardComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'care-create',
          component: CustomerCareCreateComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'filter-test',
          component: FilterTestComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'care-detail',
          component: CustomerCareDetailComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'care-list',
          component: CustomerCareListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'quote-dashboard',
          component: CustomerQuoteDashboardComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'request-approval',
          component: CustomerRequestApprovalComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'quote-approval',
          component: ApprovalQuoteListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'potential-customer-create',
          component: PotentialCustomerCreateComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'potential-customer-dashboard',
          component: PotentialCustomerDasboardComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'potential-customer-detail',
          component: PotentialCustomerDetailComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'potential-customer-list',
          component: PotentialCustomerListComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'potential-customer-request-approval',
          component: PotentialCustomerRequestApprovalComponent,
          canActivate: [AuthGuard]
        },
        {
          path: 'list-contact-customer',
          component: ListCustomerContactComponent,
          canActivate: [AuthGuard]
        }
      ]
    }
  ])],
  exports: [RouterModule]
})
export class CustomerRouting { }
