import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { NgxLoadingModule } from 'ngx-loading';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { QueryBuilderModule } from 'angular2-query-builder';

import { CommonService } from '../shared/services/common.service';
import { CategoryService } from '../shared/services/category.service';
import { PermissionService } from '../shared/services/permission.service';
import { CustomerService } from './services/customer.service';
import { WardService } from '../shared/services/ward.service';
import { ProvinceService } from '../shared/services/province.service';
import { DistrictService } from '../shared/services/district.service';
import { ContactService } from '../shared/services/contact.service';
import { NoteService } from '../shared/services/note.service';
import { EmployeeService } from "../employee/services/employee.service";
import { BankService } from '../shared/services/bank.service';
import { EmailConfigService } from '../admin/services/email-config.service';
import { ProductCategoryService } from '../admin/components/product-category/services/product-category.service';
import { PromotionService } from '../promotion/services/promotion.service';

import { CustomerRouting } from './customer.routing';
import { CustomerComponent } from './customer.component';
import { CustomerCreateComponent } from './components/customer-create/customer-create.component';
import { CustomerListComponent } from './components/customer-list/customer-list.component';
import { CustomerDetailComponent } from './components/customer-detail/customer-detail.component';
import { BankpopupComponent } from "../shared/components/bankpopup/bankpopup.component";
import { ContactpopupComponent } from "../shared/components/contactpopup/contactpopup.component";
import { CustomerImportComponent } from './components/customer-import/customer-import.component';
import { CustomerDownloadTemplateComponent } from './components/customer-download-template/customer-download-template.component';
import { CustomerImportDuplicateComponent } from './components/customer-import-duplicate/customer-import-duplicate.component';
import { CustomerDashboardComponent } from './components/customer-dashboard/customer-dashboard.component';
import { AccountingService } from '../accounting/services/accounting.service';
import { CustomerQuoteCreateComponent } from './components/customer-quote-create/customer-quote-create.component';
import { CustomerQuoteDashboardComponent } from './components/customer-quote-dashboard/customer-quote-dashboard.component';
import { ProductService } from '../product/services/product.service';
import { VendorService } from "../vendor/services/vendor.service";
import { LeadService } from '../lead/services/lead.service';
import { CustomerQuoteListComponent } from './components/customer-quote-list/customer-quote-list.component';
import { QuoteService } from './services/quote.service';
import { CustomerCareService } from './services/customer-care.service';
import { CustomerQuoteDetailComponent } from './components/customer-quote-detail/customer-quote-detail.component';
import { CustomerCareDashboardComponent } from './components/customer-care-dashboard/customer-care-dashboard.component';
import { CustomerCareCreateComponent } from './components/customer-care-create/customer-care-create.component';
import { TemplateEmailDialogComponent } from './components/template-email-dialog/template-email-dialog.component';
import { FormatDataPipe } from './pipes/format-data.pipe';
import { TemplateSmsDialogComponent } from './components/template-sms-dialog/template-sms-dialog.component';
import { FilterTestComponent } from './components/filter-test/filter-test.component';
import { SendEmailDialogComponent } from '../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../shared/components/send-sms-dialog/send-sms-dialog.component';
import { TemplateEmailLeadCusDialogComponent } from '../shared/components/template-email-dialog/template-email-dialog.component';
import { TemplateSmsLeadCusDialogComponent } from '../shared/components/template-sms-dialog/template-sms-dialog.component';
import { MatIconModule } from '@angular/material/icon';
import { GetPermission } from '../shared/permission/get-permission';
import { CustomerCareDetailComponent } from './components/customer-care-detail/customer-care-detail.component';
import { CustomerCareListComponent } from './components/customer-care-list/customer-care-list.component';
import { TemplateFeedbackDialogComponent } from './components/template-feedback-dialog/template-feedback-dialog.component';
import { TemplateQuickEmailComponent } from './components/template-quick-email/template-quick-email.component';
import { TemplateQuickSmsComponent } from './components/template-quick-sms/template-quick-sms.component';
import { TemplateQuickGiftComponent } from './components/template-quick-gift/template-quick-gift.component';
import { AddQuestionAnswerDialogComponent } from './components/add-question-answer-dialog/add-question-answer-dialog.component';
import { EditQuestionAnswerDialogComponent } from './components/edit-question-answer-dialog/edit-question-answer-dialog.component';
import { UpdateTimeCustomerCareComponent } from './components/update-time-customer-care/update-time-customer-care.component';

import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { AddEditProductDialogComponent } from './components/add-edit-product-dialog/add-edit-product-dialog.component';
import { CustomerImportDetailComponent } from './components/customer-import-detail/customer-import-detail.component';
import { MeetingDialogComponent } from './components/meeting-dialog/meeting-dialog.component';
import { CustomerRequestApprovalComponent } from './components/customer-request-approval/customer-request-approval.component';
import { ApprovalQuoteListComponent } from './components/customer-quote-approval/customer-quote-approval.component';
import { SendEmailQuoteComponent } from './components/send-mail-popup-quote/send-mail-popup-quote.component';
// import { AddEditCostQuoteDialogComponent } from './components/add-edit-cost-quote/add-edit-cost-quote.component';
import { PopupAddEditCostQuoteDialogComponent } from '../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { AddQuoteVendorDialogComponent } from './components/add-quote-vendor-dialog/add-quote-vendor-dialog.component';
import { PotentialCustomerCreateComponent } from './components/potential-customer-create/potential-customer-create.component';
import { PotentialCustomerDasboardComponent } from './components/potential-customer-dasboard/potential-customer-dasboard.component';
import { PotentialCustomerDetailComponent } from './components/potential-customer-detail/potential-customer-detail.component';
import { PotentialCustomerListComponent } from './components/potential-customer-list/potential-customer-list.component';
import { PotentialCustomerRequestApprovalComponent } from './components/potential-customer-request-approval/potential-customer-request-approval.component';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { PontentialCustomerQuoteComponent } from './components/pontential-customer-quote/pontential-customer-quote.component';

import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { DialogImportPotentialcustomerComponent } from './components/dialog-import-potentialcustomer/dialog-import-potentialcustomer.component';
import { AddProtentialCustomerDialogComponent } from './components/add-protential-customer-dialog/add-protential-customer-dialog.component';
import { AddCustomerDialogComponent } from './components/add-customer-dialog/add-customer-dialog.component';
import { ListCustomerContactComponent } from './components/list-customer-contact/list-customer-contact.component';
import { PotentialConversionDialogComponent } from './components/potential-conversion-dialog/potential-conversion-dialog.component';

@NgModule({
  imports: [
    CommonModule,
    CustomerRouting,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MatIconModule,
    NgxLoadingModule.forRoot({}),
    NgMultiSelectDropDownModule.forRoot(),
    QueryBuilderModule
  ],
  declarations: [
    CustomerComponent,
    CustomerCreateComponent,
    CustomerListComponent,
    CustomerDetailComponent,
    CustomerImportComponent,
    CustomerDownloadTemplateComponent,
    CustomerImportDuplicateComponent,
    CustomerDashboardComponent,
    CustomerQuoteCreateComponent,
    CustomerQuoteListComponent,
    CustomerQuoteDetailComponent,
    CustomerCareDashboardComponent,
    CustomerCareCreateComponent,
    TemplateEmailDialogComponent,
    FormatDataPipe,
    TemplateSmsDialogComponent,
    FilterTestComponent,
    CustomerCareDetailComponent,
    CustomerCareListComponent,
    TemplateFeedbackDialogComponent,
    TemplateQuickEmailComponent,
    TemplateQuickSmsComponent,
    TemplateQuickGiftComponent,
    AddQuestionAnswerDialogComponent,
    EditQuestionAnswerDialogComponent,
    CustomerQuoteDashboardComponent,
    UpdateTimeCustomerCareComponent,
    AddEditProductDialogComponent,
    CustomerImportDetailComponent,
    MeetingDialogComponent,
    CustomerRequestApprovalComponent,
    ApprovalQuoteListComponent,
    SendEmailQuoteComponent,
    // AddEditCostQuoteDialogComponent,
    AddQuoteVendorDialogComponent,
    PotentialCustomerCreateComponent,
    PotentialCustomerDasboardComponent,
    PotentialCustomerDetailComponent,
    PotentialCustomerListComponent,
    PotentialCustomerRequestApprovalComponent,
    PontentialCustomerQuoteComponent,
    DialogImportPotentialcustomerComponent,
    AddProtentialCustomerDialogComponent,
    AddCustomerDialogComponent,
    ListCustomerContactComponent,
    PotentialConversionDialogComponent
  ],
  providers: [
    AccountingService,
    CommonService,
    CategoryService,
    PermissionService,
    MatSnackBarConfig,
    CustomerService,
    WardService,
    ProvinceService,
    DistrictService,
    ContactService,
    NoteService,
    EmployeeService,
    BankService,
    ProductService,
    VendorService,
    LeadService,
    QuoteService,
    CustomerCareService,
    GetPermission,
    MessageService,
    ConfirmationService,
    DialogService,
    EmailConfigService,
    ForderConfigurationService,
    DynamicDialogRef,
    DynamicDialogConfig,
    ProductCategoryService,
    PromotionService
  ],
  entryComponents: [
    BankpopupComponent,
    ContactpopupComponent,
    CustomerImportComponent,
    CustomerDownloadTemplateComponent,
    SendEmailDialogComponent,
    CustomerImportDuplicateComponent,
    TemplateEmailDialogComponent,
    SendSmsDialogComponent,
    TemplateSmsDialogComponent,
    TemplateFeedbackDialogComponent,
    TemplateQuickEmailComponent,
    TemplateEmailLeadCusDialogComponent,
    TemplateQuickSmsComponent,
    TemplateQuickGiftComponent,
    AddQuestionAnswerDialogComponent,
    TemplateSmsLeadCusDialogComponent,
    EditQuestionAnswerDialogComponent,
    UpdateTimeCustomerCareComponent,
    AddEditProductDialogComponent,
    CustomerImportDetailComponent,
    MeetingDialogComponent,
    SendEmailQuoteComponent,
    PopupAddEditCostQuoteDialogComponent,
    AddQuoteVendorDialogComponent,
    PontentialCustomerQuoteComponent,
    DialogImportPotentialcustomerComponent,
    AddProtentialCustomerDialogComponent,
    AddCustomerDialogComponent,
    PotentialConversionDialogComponent
  ],
  exports: [
    TemplateQuickEmailComponent,
    TemplateQuickSmsComponent,
    TemplateQuickGiftComponent,
    MeetingDialogComponent,
    AddProtentialCustomerDialogComponent,
    AddCustomerDialogComponent,
  ]
})
export class CustomerModule { }
