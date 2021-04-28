import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule } from 'ngx-loading';

import { CommonService } from '../shared/services/common.service';
import { CategoryService } from '../shared/services/category.service';
import { OrganizationService } from '../shared/services/organization.service';
import { AccountingService } from '../accounting/services/accounting.service';
import { EmployeeService } from '../employee/services/employee.service';
import { CustomerService } from '../customer/services/customer.service';
import { VendorService } from '../vendor/services/vendor.service';
import { BankService } from '../shared/services/bank.service';
import { BankBookService } from '../accounting/services/bank-book.service';
import { PaymentRequestService } from '../accounting/services/payment-request.service';

import { AccountingRouting } from '../accounting/accounting.routing';
import { AccountingComponent } from '../accounting/accounting.component';

import { CashReceiptsListComponent } from '../accounting/components/cash-receipts/cash-receipts-list/cash-receipts-list.component';
import { CashPaymentsListComponent } from './components/cash-payments/cash-payments-list/cash-payments-list.component';
import { CashPaymentsCreateComponent } from './components/cash-payments/cash-payments-create/cash-payments-create.component';
import { CashReceiptsCreateComponent } from './components/cash-receipts/cash-receipts-create/cash-receipts-create.component';
import { CashReceiptsViewComponent } from './components/cash-receipts/cash-receipts-view/cash-receipts-view.component';
import { CashPaymentsViewComponent } from './components/cash-payments/cash-payments-view/cash-payments-view.component';
import { ReceivableVendorReportComponent } from './components/receivable-vendor/receivable-vendor-report/receivable-vendor-report.component';
import { ReceivableVendorDetailComponent } from './components/receivable-vendor/receivable-vendor-detail/receivable-vendor-detail.component';
import { ReceivableCustomerReportComponent } from './components/receivable-customer/receivable-customer-report/receivable-customer-report.component';
import { ReceivableCustomerDetailComponent } from './components/receivable-customer/receivable-customer-detail/receivable-customer-detail.component';
import { BankPaymentsCreateComponent } from './components/bank-payments/bank-payments-create/bank-payments-create.component';
import { BankReceiptsCreateComponent } from './components/bank-receipts/bank-receipts-create/bank-receipts-create.component';
import { BankPaymentsListComponent } from './components/bank-payments/bank-payments-list/bank-payments-list.component';
import { BankPaymentsDetailComponent } from './components/bank-payments/bank-payments-detail/bank-payments-detail.component';
import { BankReceiptsListComponent } from './components/bank-receipts/bank-receipts-list/bank-receipts-list.component';
import { BankReceiptViewComponent } from './components/bank-receipts/bank-receipt-view/bank-receipt-view.component';
import { CashBookComponent } from './components/cash-book/cash-book.component';
import { BankbookComponent } from './components/bankbook/bankbook.component';
import { CashBookService } from './services/cash-book.service';
import { SalesReportComponent } from './components/sales-report/sales-report.component';
import { BudgetListComponent } from './components/budget/budget-list/budget-list.component';
import { BudgetCreatePopupComponent } from './components/budget/budget-create-popup/budget-create-popup.component';
import { BudgetDetailComponent } from './components/budget/budget-detail/budget-detail.component';
import { MatDialogModule } from '@angular/material/dialog';
import { BudgetCreateService } from './services/budget-create.service';
import { BudgetListService } from './services/budget-list.service';
import { PaymentRequestCreateComponent } from './components/payment-request/payment-request-create/payment-request-create.component';
import { PaymentRequestListComponent } from './components/payment-request/payment-request-list/payment-request-list.component';
import { PaymentRequestViewComponent } from './components/payment-request/payment-request-view/payment-request-view.component';
import { CostCreateComponent } from './components/cost/cost-create/cost-create.component';

import { PositionService } from '../shared/services/position.service';
import { ContactService } from '../shared/services/contact.service';
import { WorkflowService } from '../admin/components/workflow/services/workflow.service';
import { ProductCategoryPopupComponent } from './components/budget/product-category-popup/product-category-popup.component';
import { ProductCategoryService } from '../admin/components/product-category/services/product-category.service';
import { GetPermission } from '../shared/permission/get-permission';
import {KeyFilterModule} from 'primeng/keyfilter';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    MatDialogModule,
    AccountingRouting,
    FormsModule,
    ReactiveFormsModule,
    KeyFilterModule,
    
    NgxLoadingModule.forRoot({})
  ],
  declarations: [
    AccountingComponent, 
    CashReceiptsListComponent, 
    CashPaymentsListComponent, 
    CashPaymentsCreateComponent,
    CashReceiptsCreateComponent,
    CashReceiptsViewComponent,
    BudgetDetailComponent,
    CashPaymentsViewComponent,
    ReceivableVendorReportComponent,
    ReceivableVendorDetailComponent,
    ReceivableCustomerReportComponent,
    ReceivableCustomerDetailComponent,
    BankPaymentsCreateComponent,
    BankReceiptsCreateComponent,
    BankPaymentsListComponent,
    BankPaymentsDetailComponent,
    BankReceiptsListComponent,
    BankReceiptViewComponent,
    CashBookComponent,
    BankbookComponent,
    SalesReportComponent,
    BudgetListComponent,
    BudgetCreatePopupComponent,
    PaymentRequestCreateComponent,
    PaymentRequestListComponent,
    PaymentRequestViewComponent,
    ProductCategoryPopupComponent,
    PaymentRequestService,
    WorkflowService,
    CostCreateComponent
  ],
  entryComponents : [BudgetCreatePopupComponent, ProductCategoryPopupComponent],
  providers: [
    PositionService,
    AccountingComponent, 
    CashReceiptsListComponent, 
    CommonService, 
    CategoryService, 
    OrganizationService, 
    AccountingService,
    EmployeeService, 
    CustomerService, 
    VendorService, 
    BankService, 
    BankBookService, 
    CashBookService, 
    ProductCategoryPopupComponent, 
    GetPermission,
    BudgetCreatePopupComponent, 
    BudgetCreateService, 
    BudgetListService, 
    ContactService, 
    PaymentRequestService, 
    WorkflowService, 
    ProductCategoryService, 
    MessageService, 
    DialogService, 
    ConfirmationService
  ]
})
export class AccountingModule { }
