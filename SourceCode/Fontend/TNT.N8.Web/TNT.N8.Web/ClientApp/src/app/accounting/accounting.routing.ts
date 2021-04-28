import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../shared/guards/auth.guard';

import { AccountingComponent } from '../accounting/accounting.component';
import { CashReceiptsListComponent } from '../accounting/components/cash-receipts/cash-receipts-list/cash-receipts-list.component';
import { CashPaymentsListComponent } from './components/cash-payments/cash-payments-list/cash-payments-list.component';
import { CashPaymentsCreateComponent } from './components/cash-payments/cash-payments-create/cash-payments-create.component';
import { CashReceiptsCreateComponent } from './components/cash-receipts/cash-receipts-create/cash-receipts-create.component';
import { CashPaymentsViewComponent } from './components/cash-payments/cash-payments-view/cash-payments-view.component';
import { CashReceiptsViewComponent } from './components/cash-receipts/cash-receipts-view/cash-receipts-view.component';
import { ReceivableVendorReportComponent } from './components/receivable-vendor/receivable-vendor-report/receivable-vendor-report.component';
import { BankPaymentsCreateComponent } from './components/bank-payments/bank-payments-create/bank-payments-create.component';
import { ReceivableVendorDetailComponent } from './components/receivable-vendor/receivable-vendor-detail/receivable-vendor-detail.component';
import { BankReceiptsCreateComponent } from './components/bank-receipts/bank-receipts-create/bank-receipts-create.component';
import { BankPaymentsListComponent } from './components/bank-payments/bank-payments-list/bank-payments-list.component';
import { BankPaymentsDetailComponent } from './components/bank-payments/bank-payments-detail/bank-payments-detail.component';
import { ReceivableCustomerReportComponent } from './components/receivable-customer/receivable-customer-report/receivable-customer-report.component';
import { ReceivableCustomerDetailComponent } from './components/receivable-customer/receivable-customer-detail/receivable-customer-detail.component';
import { BankReceiptsListComponent } from './components/bank-receipts/bank-receipts-list/bank-receipts-list.component';
import { BankReceiptViewComponent } from './components/bank-receipts/bank-receipt-view/bank-receipt-view.component';
import { CashBookComponent } from './components/cash-book/cash-book.component';
import { BankbookComponent } from './components/bankbook/bankbook.component';
import { SalesReportComponent } from './components/sales-report/sales-report.component';
import { BudgetListComponent } from './components/budget/budget-list/budget-list.component';
import { PaymentRequestCreateComponent } from './components/payment-request/payment-request-create/payment-request-create.component';
import { PaymentRequestListComponent } from './components/payment-request/payment-request-list/payment-request-list.component';
import { BudgetDetailComponent } from './components/budget/budget-detail/budget-detail.component';
import { PaymentRequestViewComponent } from './components/payment-request/payment-request-view/payment-request-view.component';
import { CostCreateComponent } from './components/cost/cost-create/cost-create.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: AccountingComponent,
        children: [
          {
            path: 'cash-receipts-list',
            component: CashReceiptsListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cash-payments-list',
            component: CashPaymentsListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cash-payments-create',
            component: CashPaymentsCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bank-payments-create',
            component: BankPaymentsCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bank-payments-list',
            component: BankPaymentsListComponent,
            canActivate: [AuthGuard]
          }, {
            path: 'bank-payments-detail',
            component: BankPaymentsDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bank-receipts-detail',
            component: BankReceiptViewComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bank-receipts-create',
            component: BankReceiptsCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bank-receipts-list',
            component: BankReceiptsListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cash-receipts-create',
            component: CashReceiptsCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cash-payments-view',
            component: CashPaymentsViewComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cash-receipts-view',
            component: CashReceiptsViewComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'receivable-vendor-report',
            component: ReceivableVendorReportComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'receivable-vendor-detail',
            component: ReceivableVendorDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'receivable-customer-report',
            component: ReceivableCustomerReportComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'receivable-customer-detail',
            component: ReceivableCustomerDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cash-book',
            component: CashBookComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'bank-book',
            component: BankbookComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'budget-list',
            component: BudgetListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'budget-detail',
            component: BudgetDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'sales-report',
            component: SalesReportComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'payment-request-create',
            component: PaymentRequestCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'payment-request-list',
            component: PaymentRequestListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'payment-request-detail',
            component: PaymentRequestViewComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'cost-create',
            component: CostCreateComponent,
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
export class AccountingRouting { }
