import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxLoadingModule } from 'ngx-loading';
import { ProcurementRequestCreateComponent } from './components/procurement-request-create/procurement-request-create.component';
import { ProcurementRequestComponent } from './procurement-request.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProcurementRequestRouting } from './procurement-request.routing';
import { SharedModule } from '../shared/shared.module';
import { EmployeeService } from '../employee/services/employee.service';
import { VendorService } from "../vendor/services/vendor.service";
import { ProductService } from '../product/services/product.service';
import { PositionService } from '../shared/services/position.service';
import { OrganizationpopupComponent } from "../shared/components/organizationpopup/organizationpopup.component";
import { ProcurementRequestItemDialogComponent } from './components/procurement-request-item-dialog/procurement-request-item-dialog.component';
import { MatSnackBarConfig } from '@angular/material/snack-bar';
import { ProcurementRequestListComponent } from './components/procurement-request-list/procurement-request-list.component';
import { CategoryService } from '../shared/services/category.service';

import { ProcurementRequestService } from '../procurement-request/services/procurement-request.service';
import { WorkflowService } from '../admin/components/workflow/services/workflow.service';
import { ImageUploadService } from '../shared/services/imageupload.service';
import { ProcurementRequestViewComponent } from './components/procurement-request-view/procurement-request-view.component';
import { ContactService } from '../shared/services/contact.service';
import { GetPermission } from '../shared/permission/get-permission';
import { CreateRequestItemPopupComponent } from './components/create-request-item-popup/create-request-item-popup.component';
import { ForderConfigurationService } from '../admin/components/folder-configuration/services/folder-configuration.service';
import { NoteService } from '../shared/services/note.service';
import { ProcurementRequestReportListComponent } from './components/procurement-request-report-list/procurement-request-report-list.component';
import { EmailConfigService } from '../admin/services/email-config.service';
import { ProductCategoryService } from '../admin/components/product-category/services/product-category.service';

@NgModule({
	imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    ReactiveFormsModule,
    ProcurementRequestRouting,
    NgxLoadingModule.forRoot({})
	],
  providers: [
    ProcurementRequestService, 
    CategoryService, 
    EmployeeService, 
    VendorService, 
    GetPermission, 
    NoteService, 
    ForderConfigurationService,
    PositionService, 
    MatSnackBarConfig, 
    WorkflowService, 
    ImageUploadService, 
    ProductService, 
    ContactService,
    EmailConfigService,
    ProductCategoryService
  ],
	declarations: [
    ProcurementRequestComponent, 
    ProcurementRequestCreateComponent, 
    ProcurementRequestReportListComponent,
    ProcurementRequestListComponent, 
    ProcurementRequestItemDialogComponent, 
    ProcurementRequestViewComponent, 
    CreateRequestItemPopupComponent,
  ],  
	entryComponents: [
    ProcurementRequestItemDialogComponent, 
    OrganizationpopupComponent, 
    CreateRequestItemPopupComponent,
  ]
})
export class ProcurementRequestModule { }
