import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { AuthGuard } from './guards/auth.guard';
import { AppInterceptor } from './guards/app.interceptor';
import { NgxCurrencyModule } from 'ngx-currency';
import { TreeViewModule as Tree } from '@syncfusion/ej2-angular-navigations';
import { RichTextEditorModule, RichTextEditorAllModule } from '@syncfusion/ej2-angular-richtexteditor';
import { DialogModule as DialogSyncfusionModule } from '@syncfusion/ej2-angular-popups';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { LayoutComponent } from './components/layout/layout.component';
import { MenuComponent } from './components/menu/menu.component';
import { NgxMaskModule, IConfig } from 'ngx-mask';

import { AuthenticationService } from './services/authentication.service';
import { CommonService } from './services/common.service';
import { CompanyService } from './services/company.service';
import { CategoryService } from './services/category.service';
import { ContactService } from './services/contact.service';
import { OrganizationService } from './services/organization.service';
import { ProvinceService } from './services/province.service';
import { PermissionService } from './services/permission.service';
import { DistrictService } from './services/district.service';
import { WardService } from './services/ward.service';
import { PositionService } from './services/position.service';
import { ImageUploadService } from './services/imageupload.service';
import { EmailService } from './services/email.service';
import { BankService } from './services/bank.service';
import { OrderstatusService } from './services/orderstatus.service';
import { CustomerlevelService } from './services/customerlevel.service';
import { UserModel } from './models/user.model';
import { PermissionModel } from './models/permission.model';
import { PermissionSetModel } from './models/permissionSet.model';
import { EventEmitterService } from './services/event-emitter.service';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { VendorService } from './services/vendor.service';
import { ProductService } from './services/product.service';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { SuccessComponent } from './toast/success/success.component';
import { WarningComponent } from './toast/warning/warning.component';
import { FailComponent } from './toast/fail/fail.component';
// import { ngfModule, ngf } from 'angular-file';
import { NoteService } from './services/note.service';
import { ChartModule } from 'angular-highcharts';
import { PopupComponent } from './components/popup/popup.component';
import { PopupConfirmComponent } from './components/popupConfirm/popupConfirm.component';
import { ChangepasswordComponent } from './components/changepassword/changepassword.component';
import { ChangepasswordsuccessComponent } from './components/changepasswordsuccess/changepasswordsuccess.component';
import { ApproveRejectPopupComponent } from './components/approverejectpopup/approverejectpopup.component';
import { TreeviewModule } from 'ngx-treeview';
import { ContactpopupComponent } from './components/contactpopup/contactpopup.component';
import { BankpopupComponent } from './components/bankpopup/bankpopup.component';
import { NumberToStringPipe } from './ConvertMoneyToString/numberToString.pipe';
import { OrganizationpopupComponent } from './components/organizationpopup/organizationpopup.component';
import { NotificationService } from './services/notification.service';
import { DashboardHomeService } from './services/dashboard-home.service';
import { SendEmailDialogComponent } from './components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from './components/send-sms-dialog/send-sms-dialog.component';
import { TemplateEmailLeadCusDialogComponent } from './components/template-email-dialog/template-email-dialog.component';
import { TemplateSmsLeadCusDialogComponent } from './components/template-sms-dialog/template-sms-dialog.component';
import { FormatDataPipe } from './format-data.pipe';
import { MyFilterPipe } from './FilterngFor/my-filter.pipe';

/*Primeng Control*/
import { DropdownModule } from 'primeng/dropdown';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { SplitButtonModule } from 'primeng/splitbutton';
import { ChartModule as PrimeChartModule } from 'primeng/chart';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { AccordionModule } from 'primeng/accordion';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SidebarModule } from 'primeng/sidebar';
import { CheckboxModule } from 'primeng/checkbox';
import { PaginatorModule } from 'primeng/paginator';
import { EditorModule } from 'primeng/editor';
import { RadioButtonModule } from 'primeng/radiobutton';
import { StepsModule } from 'primeng/steps';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TabViewModule } from 'primeng/tabview';
import { GalleriaModule } from 'primeng/galleria';
import { ChipsModule } from 'primeng/chips';
import { OrganizationChartModule } from 'primeng/organizationchart';
import { InputSwitchModule } from 'primeng/inputswitch';
import { GMapModule } from 'primeng/gmap';
import { PanelMenuModule } from 'primeng/panelmenu';
import { FullCalendarModule } from 'primeng/fullcalendar';

/*End*/

import { NgxCleaveDirectiveModule } from 'ngx-cleave-directive';
import { OrganizationDialogComponent } from './components/organization-dialog/organization-dialog.component';
import { GetPermission } from './permission/get-permission';
import { PopupAddEditCostQuoteDialogComponent } from './components/add-edit-cost-quote/add-edit-cost-quote.component';
import { QuoteService } from './services/quote.services';
import { QuickCreateVendorComponent } from './components/quick-create-vendor/quick-create-vendor.component';
import { QuickCreateProductComponent } from './components/quick-create-product/quick-create-product.component';
import { TreeProductCategoryComponent } from '../product/components/tree-product-category/tree-product-category.component';
import { DialogOrganizationPermissionsionComponent } from './components/dialog-organization-permissionsion/dialog-organization-permissionsion.component';
import { PromotionApplyPopupComponent } from './components/promotion-apply-popup/promotion-apply-popup.component';
import { PromotionProductMappingApplyPopupComponent } from './components/promotion-product-mapping-apply-popup/promotion-product-mapping-apply-popup.component';
import { CustomerService } from '../customer/services/customer.service';

export const customCurrencyMaskConfig = {
  align: 'right',
  allowNegative: true,
  allowZero: true,
  decimal: '',
  precision: 0,
  prefix: '',
  suffix: '',
  thousands: ',',
  nullable: true
};
export const options: Partial<IConfig> | (() => Partial<IConfig>) = {};

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatInputModule,
    MatFormFieldModule,
    MatExpansionModule,
    MatTabsModule,
    MatTableModule,
    MatCheckboxModule,
    MatSelectModule,
    MatSnackBarModule,
    MatRadioModule,
    MatMenuModule,
    MatTooltipModule,
    MatChipsModule,
    MatAutocompleteModule,
    MatPaginatorModule,
    MatSortModule,
    MatSlideToggleModule,
    // ngfModule,
    ChartModule,
    MatDialogModule,
    TreeviewModule,
    Tree,
    NgxCurrencyModule.forRoot(customCurrencyMaskConfig),
    NgxMaskModule.forRoot(options),
    DropdownModule,
    SplitButtonModule,
    PrimeChartModule,
    AutoCompleteModule,
    InputTextModule,
    CalendarModule,
    NgxCleaveDirectiveModule,
    TableModule,
    MultiSelectModule,
    MessagesModule,
    MessageModule,
    ToastModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    DialogModule,
    TooltipModule,
    AccordionModule,
    ConfirmDialogModule,
    SidebarModule,
    CheckboxModule,
    PaginatorModule,
    RadioButtonModule,
    EditorModule,
    StepsModule,
    FileUploadModule,
    InputTextareaModule,
    TabViewModule,
    GalleriaModule,
    ChipsModule,
    RichTextEditorModule,
    DialogSyncfusionModule,
    RichTextEditorAllModule,
    OrganizationChartModule,
    InputSwitchModule,
    GMapModule,
    PanelMenuModule,
    FullCalendarModule
    // TreeProductCategoryComponent
  ],
  entryComponents: [
    SuccessComponent,
    FailComponent,
    WarningComponent,
    PopupComponent,
    PopupConfirmComponent,
    ApproveRejectPopupComponent,
    OrganizationpopupComponent,
    OrganizationDialogComponent,
    PopupAddEditCostQuoteDialogComponent,
    QuickCreateProductComponent,
    TreeProductCategoryComponent,
    PromotionApplyPopupComponent,
    PromotionProductMappingApplyPopupComponent
  ],
  declarations: [
    AuthGuard,
    HeaderComponent,
    FooterComponent,
    UserModel,
    AuthenticationService,
    AppInterceptor,
    CommonService,
    CompanyService,
    ContactService,
    CategoryService,
    UserModel,
    PermissionModel,
    PermissionSetModel,
    LayoutComponent,
    MenuComponent,
    SuccessComponent,
    FailComponent,
    WarningComponent,
    OrganizationService,
    NoteService,
    ProvinceService,
    DistrictService,
    QuoteService,
    WardService,
    PositionService,
    ImageUploadService,
    PopupComponent,
    PopupConfirmComponent,
    PermissionService,
    EmailService,
    BankService,
    FormatDataPipe,
    ChangepasswordComponent,
    ChangepasswordsuccessComponent,
    WarningComponent,
    ApproveRejectPopupComponent,
    ContactpopupComponent,
    BankpopupComponent,
    OrderstatusService,
    NumberToStringPipe,
    CustomerlevelService,
    OrganizationpopupComponent,
    NotificationService,
    DashboardHomeService,
    SendEmailDialogComponent,
    SendSmsDialogComponent,
    TemplateEmailLeadCusDialogComponent,
    TemplateSmsLeadCusDialogComponent,
    MyFilterPipe,
    OrganizationDialogComponent,
    PopupAddEditCostQuoteDialogComponent,
    QuickCreateVendorComponent,
    VendorService,
    ProductService,
    QuickCreateProductComponent,
    TreeProductCategoryComponent,
    DialogOrganizationPermissionsionComponent,
    PromotionApplyPopupComponent,
    PromotionProductMappingApplyPopupComponent,
  ],
  exports: [
    CommonModule,
    RouterModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    AuthGuard,
    HeaderComponent,
    FooterComponent,
    LayoutComponent,
    UserModel,
    AuthenticationService,
    CommonService,
    CompanyService,
    ContactService,
    CategoryService,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatInputModule,
    MatFormFieldModule,
    MatExpansionModule,
    MatTabsModule,
    MatTableModule,
    MatCheckboxModule,
    MatSelectModule,
    MatSnackBarModule,
    MatRadioModule,
    MatMenuModule,
    MatTooltipModule,
    MatNativeDateModule,
    MatChipsModule,
    MatAutocompleteModule,
    MatPaginatorModule,
    MatSortModule,
    MatSlideToggleModule,
    // ngfModule,
    ChartModule,
    MatDialogModule,
    PopupComponent,
    PopupConfirmComponent,
    ChangepasswordComponent,
    ChangepasswordsuccessComponent,
    ApproveRejectPopupComponent,
    TreeviewModule,
    Tree,
    NgxCurrencyModule,
    NumberToStringPipe,
    SendEmailDialogComponent,
    SendSmsDialogComponent,
    TemplateEmailLeadCusDialogComponent,
    TemplateSmsLeadCusDialogComponent,
    MyFilterPipe,
    NgxMaskModule,
    DropdownModule,
    SplitButtonModule,
    PrimeChartModule,
    AutoCompleteModule,
    InputTextModule,
    CalendarModule,
    NgxCleaveDirectiveModule,
    TableModule,
    MultiSelectModule,
    MessagesModule,
    MessageModule,
    ToastModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    DialogModule,
    TooltipModule,
    AccordionModule,
    ConfirmDialogModule,
    SidebarModule,
    CheckboxModule,
    PaginatorModule,
    RadioButtonModule,
    EditorModule,
    FullCalendarModule,
    StepsModule,
    FileUploadModule,
    InputTextareaModule,
    TabViewModule,
    GalleriaModule,
    ChipsModule,
    RichTextEditorModule,
    DialogSyncfusionModule,
    RichTextEditorAllModule,
    PopupAddEditCostQuoteDialogComponent,
    OrganizationChartModule,
    QuickCreateVendorComponent,
    InputSwitchModule,
    GMapModule,
    PanelMenuModule,
    TreeProductCategoryComponent,
    DialogOrganizationPermissionsionComponent,
    PromotionApplyPopupComponent,
    PromotionProductMappingApplyPopupComponent
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AppInterceptor,
      multi: true,
    },
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
    CommonService,
    ImageUploadService,
    AuthenticationService,
    EmailService,
    CompanyService,
    OrganizationService,
    NotificationService,
    DashboardHomeService,
    EventEmitterService,
    MessageService,
    ConfirmationService,
    DialogService,
    GetPermission,
    QuoteService,
    VendorService,
    ProductService,
    CustomerService
  ],
  bootstrap: [
    PopupComponent,
    PopupConfirmComponent,
    ChangepasswordComponent,
    ChangepasswordsuccessComponent,
    ApproveRejectPopupComponent
  ]
})
export class SharedModule {

}
