import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxLoadingModule } from 'ngx-loading';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { LeadRouting } from './lead.routing';
import { LeadComponent } from './lead.component';
import { LeadListComponent } from './components/list/list.component';
import { LeadDetailComponent } from './components/detail/detail.component';
import { LeadDashboardComponent } from './components/dashboard/dashboard.component';
import { PopupComponent } from "../shared/components/popup/popup.component";
import { CreateComponent } from './components/create/create.component';
import { UnfollowComponent } from './components/unfollow/unfollow.component';

import { CommonService } from '../shared/services/common.service';
import { CompanyService } from '../shared/services/company.service';
import { ContactService } from '../shared/services/contact.service';
import { LeadService } from './services/lead.service';
import { CategoryService } from '../shared/services/category.service';
import { EmployeeService } from '../employee/services/employee.service';
import { NoteService } from '../shared/services/note.service';
import { WardService } from '../shared/services/ward.service';
import { ProvinceService } from '../shared/services/province.service';
import { DistrictService } from '../shared/services/district.service';
import { EmailService } from '../shared/services/email.service';
import { EmailConfigService } from '../admin/services/email-config.service';
import { ReportComponent } from './components/report/report.component';
import { LeadConfirmDialogComponent } from './components/lead-confirm-dialog/lead-confirm-dialog.component';
import { CreateLeadComponent } from './components/create-lead/create-lead.component';
import { SendEmailDialogComponent } from '../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../shared/components/send-sms-dialog/send-sms-dialog.component';
import { TemplateEmailLeadCusDialogComponent } from '../shared/components/template-email-dialog/template-email-dialog.component';
import { TemplateSmsLeadCusDialogComponent } from '../shared/components/template-sms-dialog/template-sms-dialog.component';
import { LeadImportComponent } from './components/lead-import/lead-import.component';
import { LeadImportDuplicateComponent } from './components/lead-import-duplicate/lead-import-duplicate.component';
import { CustomerImportDuplicateComponent } from './components/lead-import-duplicate-customer/lead-import-duplicate-customer.component';
import { GetPermission } from '../shared/permission/get-permission';
import { LeadEditPersonInChargeComponent } from './components/lead-edit-person-in-charge/lead-edit-person-in-charge.component';
import { LeadApprovalComponent } from './components/lead-approval/lead-approval.component';
import { SetPicDialogComponent } from './components/set-pic-dialog/set-pic-dialog.component';
import { LeadImportDetailComponent } from './components/lead-import-detail/lead-import-detail.component';
import { QuickCreateLeadComponent } from './components/quick-create-lead/quick-create-lead.component';
import { KeyFilterModule } from 'primeng/keyfilter';
import { CustomerModule } from '../customer/customer.module';
import { CreatContactLeadDialogComponent } from './components/creat-contact-lead-dialog/creat-contact-lead-dialog.component';
import { LeadDetailDialogComponent } from './components/lead-detail-dialog/lead-detail-dialog.component';
import { LeadTemplateQuickSmsComponent } from './components/lead-template-quick-sms/lead-template-quick-sms.component';
import { LeadTemplateQuickEmailComponent } from './components/lead-template-quick-email/lead-template-quick-email.component';
import { LeadTemplateQuickGiftComponent } from './components/lead-template-quick-gift/lead-template-quick-gift.component';
import { LeadCareService } from './services/lead-care.service';
import { LeadMeetingDialogComponent } from './components/lead-meeting-dialog/lead-meeting-dialog.component';
import { ReportLeadComponent } from './components/report-lead/report-lead.component';
import { AddProtentialCustomerDialogComponent } from '../customer/components/add-protential-customer-dialog/add-protential-customer-dialog.component';

@NgModule({
  imports: [
    SharedModule,
    LeadRouting,
    FormsModule,
    ReactiveFormsModule,
    NgxLoadingModule.forRoot({}),
    KeyFilterModule,
    CustomerModule,
    MatIconModule,
  ],
  declarations: [
    LeadComponent,
    LeadListComponent,
    LeadDetailComponent,
    LeadDashboardComponent,
    CreateComponent,
    UnfollowComponent,
    ReportComponent,
    LeadConfirmDialogComponent,
    CreateLeadComponent,  
    LeadImportComponent,
    LeadImportDuplicateComponent,
    CustomerImportDuplicateComponent,
    LeadEditPersonInChargeComponent,
    LeadApprovalComponent,
    SetPicDialogComponent,
    LeadImportDetailComponent,
    QuickCreateLeadComponent,
    CreatContactLeadDialogComponent,
    LeadDetailDialogComponent,
    LeadTemplateQuickSmsComponent,
    LeadTemplateQuickEmailComponent,
    LeadTemplateQuickGiftComponent,
    LeadMeetingDialogComponent,
    ReportLeadComponent
  ],
  entryComponents: [
    CreateComponent, 
    LeadConfirmDialogComponent, 
    LeadImportComponent, 
    LeadImportDuplicateComponent,
    CustomerImportDuplicateComponent,
    SendEmailDialogComponent, 
    SendSmsDialogComponent, 
    TemplateEmailLeadCusDialogComponent, 
    TemplateSmsLeadCusDialogComponent, 
    LeadEditPersonInChargeComponent,
    SetPicDialogComponent,
    LeadImportDetailComponent,
    QuickCreateLeadComponent,
    CreatContactLeadDialogComponent,
    LeadDetailDialogComponent,
    LeadTemplateQuickSmsComponent,
    LeadTemplateQuickEmailComponent,
    LeadTemplateQuickGiftComponent,
    LeadMeetingDialogComponent,
  ],
  providers: [
    CommonService,
    LeadService, 
    CompanyService, 
    GetPermission,
    ContactService, 
    CategoryService, 
    EmployeeService, 
    NoteService, 
    WardService,
    ProvinceService, 
    DistrictService, 
    EmailService, 
    MatPaginatorIntl,
    MatSnackBarConfig,
    EmailConfigService,
    LeadCareService
  ],
  bootstrap: [CreateComponent, LeadConfirmDialogComponent]
})
export class LeadModule { }
