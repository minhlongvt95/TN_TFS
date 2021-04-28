import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { EmployeeRouting } from './employee.routing';
import { DatePipe } from '@angular/common';

import { EmployeeComponent } from './employee.component';
import { ListComponent } from './components/list/list.component';
import { EmployeeQuitWorkComponent } from './components/employee-quit-work/employee-quit-work.component';
import { CommonService } from '../shared/services/common.service';
import { OrganizationService } from '../shared/services/organization.service';
import { EmployeeService } from './services/employee.service';
import { CreateEmployeeRequestService } from './services/employee-request/create-employee-request.service';
import { CategoryService } from '../shared/services/category.service';
import { PositionService } from '../shared/services/position.service';
import { PermissionService } from '../shared/services/permission.service';
import { EmployeeTimesheetService } from './services/employee-salary/employee-timesheet.service';
import { ListEmployeeRequestService } from './services/employee-request/list-employee-request.service';
import { EmployeeSalaryHandmadeImportService } from './services/employee-salary/employee-salary-handmade-import.service';
import { TeacherSalaryHandmadeImportService } from './services/teacher-salary/teacher-salary-handmade-import.service';
import {AssistantExportListService } from './services/assistant-salary/assistant-export-list.service';
import { AssistantSalaryHandmadeImportService } from './services/assistant-salary/assistant-salary-handmade-import.service';
import { AssistantSalaryFindService } from './services/assistant-salary/assistant-salary-find.service';
import { EmployeeListService } from './services/employee-list/employee-list.service';
import { EmailConfigService } from '../admin/services/email-config.service';

import { OrgSelectDialogComponent } from './components/org-select-dialog/org-select-dialog.component';
import { UnitSelectDialogComponent } from './components/unit-select-dialog/unit-select-dialog.component';
import { EmployeeSalaryListComponent } from './components/employee-salary/employee-salary-list/employee-salary-list.component';
import { CreateEmployeeRequestComponent } from './components/employee-request/create-employee-request/create-employee-request.component';
import { EmployeeTimesheetImportComponent } from './components/employee-salary/employee-timesheet-import/employee-timesheet-import.component';
import { EmployeeCreateComponent } from './components/employee-profile/employee-create/employee-create.component';
import { EmployeeDetailComponent } from './components/employee-profile/employee-detail/employee-detail.component';
import { ListEmployeeRequestComponent } from './components/employee-request/list-employee-request/list-employee-request.component';
import { BankService } from '../shared/services/bank.service';
import { EmployeeSalaryService } from './services/employee-salary/employee-salary.service';
import { EmployeeCreateSalaryPopupComponent } from './components/employee-profile/employee-create-salary-popup/employee-create-salary-popup.component'
import { EmployeeAllowanceService } from './services/employee-allowance/employee-allowance.service';
import { EmployeeInsuranceService } from './services/employee-insurance/employee-insurance.service';
import { EmployeeSalaryHandmadeImportComponent } from './components/employee-salary/employee-salary-handmade-import/employee-salary-handmade-import.component';
import { EmployeeMonthySalaryService } from './services/employee-salary/employee-monthy-salary.service';
import { EmployeeAssessmentService } from './services/employee-assessment/employee-assessment.service';
import { ImageUploadService } from '../shared/services/imageupload.service';
import { NoteService } from '../shared/services/note.service';
import { PopupComponent } from '../shared/components/popup/popup.component';
import { CustomerService } from '../customer/services/customer.service';
import { NgxLoadingModule } from 'ngx-loading';
import { ContactService } from '../shared/services/contact.service';
import { TeacherSalaryListComponent } from './components/employee-salary/teacher-salary-list/teacher-salary-list.component';
import { TeacherSapiGetComponent } from './components/employee-salary/teacher-sapi-get/teacher-sapi-get.component';
import { TeacherSalaryService } from './services/teacher-salary/teacher-salary.service';
import { TeacherSalaryHandmadeImportComponent } from './components/employee-salary/teacher-salary-handmade-import/teacher-salary-handmade-import.component';
import { AssistantSalaryListComponent } from './components/employee-salary/assistant-salary-list/assistant-salary-list.component';
import { AssistantSalaryExportComponent } from './components/employee-salary/assistant-salary-export/assistant-salary-export.component';
import { AssistantSalaryHandmadeImportComponent } from './components/employee-salary/assistant-salary-handmade-import/assistant-salary-handmade-import.component';
import { WorkflowService } from '../admin/components/workflow/services/workflow.service';
import {MatChipsModule} from '@angular/material/chips';
import { DetailEmployeeRequestComponent } from './components/employee-request/detail-employee-request/detail-employee-request.component';
import { EmployeeDashboardComponent } from './components/employee-dashboard/employee-dashboard.component';
import { OrgSelectMultiDialogComponent } from './components/org-select-multi-dialog/org-select-multi-dialog.component';
import { GetPermission } from '../shared/permission/get-permission';
import { EmailService } from '../shared/services/email.service';
@NgModule({
  imports: [
    SharedModule,
    EmployeeRouting,
    FormsModule,
    ReactiveFormsModule,
    MatChipsModule,
    NgxLoadingModule.forRoot({})
  ],
  declarations: [
    EmployeeComponent,
    ListComponent,
    OrgSelectDialogComponent,
    UnitSelectDialogComponent,
    CreateEmployeeRequestComponent,
    EmployeeSalaryListComponent,
    EmployeeTimesheetImportComponent,
    EmployeeCreateComponent,
    EmployeeDetailComponent,
    ListEmployeeRequestComponent,
    EmployeeCreateSalaryPopupComponent,
    EmployeeSalaryHandmadeImportComponent,
    TeacherSalaryListComponent,
    TeacherSapiGetComponent,
    TeacherSalaryHandmadeImportComponent,
    AssistantSalaryListComponent,
    AssistantSalaryExportComponent,
    AssistantSalaryHandmadeImportComponent,
    DetailEmployeeRequestComponent,
    EmployeeDashboardComponent,
    OrgSelectMultiDialogComponent, EmployeeQuitWorkComponent
  ],

  providers: [ContactService, CustomerService, NoteService, ImageUploadService, EmployeeAssessmentService,
    EmployeeMonthySalaryService, EmployeeInsuranceService, EmployeeAllowanceService, EmployeeSalaryService,
    BankService, CommonService, EmployeeService, CreateEmployeeRequestService, WorkflowService, GetPermission,
    OrganizationService, CategoryService, PositionService, PermissionService, MatSnackBarConfig,
    EmployeeTimesheetService, ListEmployeeRequestService, EmployeeSalaryHandmadeImportService, TeacherSalaryService,
    TeacherSalaryHandmadeImportService, AssistantExportListService, AssistantSalaryHandmadeImportService, AssistantSalaryFindService,
    EmployeeListService, EmailService, DatePipe, EmailConfigService
  ],
  entryComponents: [PopupComponent, OrgSelectDialogComponent, UnitSelectDialogComponent, EmployeeCreateSalaryPopupComponent,
    EmployeeSalaryHandmadeImportComponent, TeacherSapiGetComponent, TeacherSalaryHandmadeImportComponent,
    AssistantSalaryExportComponent, AssistantSalaryHandmadeImportComponent, EmployeeTimesheetImportComponent
  ],
  exports: [
    OrgSelectDialogComponent
  ],
  bootstrap: [EmployeeTimesheetImportComponent]
})
export class EmployeeModule { }
