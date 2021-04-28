import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AdminComponent } from './admin.component';
import { OrganizationComponent } from './components/organization/organization.component';
import { MasterdataComponent } from './components/masterdata/masterdata.component';
import { AuthGuard } from '../shared/guards/auth.guard';
import { PermissionComponent } from './components/permission/permission.component';
import { PermissionDetailComponent } from './components/permission-detail/permission-detail.component';
import { CreateComponent } from './components/product-category/components/create/create.component';
import { ListProductCategoryComponent } from './components/product-category/components/list-product-category/list-product-category.component';
import { ConfigLevelCustomerComponent } from './components/config-level-customer/config-level-customer.component';
import { CompanyConfigurationComponent } from './components/company-configuration/company-configuration.component';
import { WorkflowCreateComponent } from './components/workflow/components/workflow-create/workflow-create.component';
import { WorkflowDetailComponent } from './components/workflow/components/workflow-detail/workflow-detail.component';
import { WorkflowListComponent } from './components/workflow/components/workflow-list/workflow-list.component';
import { SystemParameterComponent } from './components/system-parameter/system-parameter.component';
import { PermissionCreateComponent } from './components/permission-create/permission-create.component';
import { EmailConfigurationComponent } from './components/email-configuration/email-configuration.component';
import { EmailCreateTemplateComponent } from './components/email-create-template/email-create-template.component';
import { FolderConfigurationComponent } from './components/folder-configuration/folder-configuration.component';
import { NotificationSettingComponent } from './components/notification-setting/notification-setting.component';
import { NotificationSettingDetailComponent } from './components/notification-setting-detail/notification-setting-detail.component';
import { NotificationSettingListComponent } from './components/notification-setting-list/notification-setting-list.component';
import { MenuBuildComponent } from './components/menu-build/menu-build.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: AdminComponent,
        children: [
          {
            path: 'organization',
            component: OrganizationComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'masterdata',
            component: MasterdataComponent,
            canActivate: [AuthGuard]
            },
          {
            path: 'permission',
            component: PermissionComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'permission-create',
            component: PermissionCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'permission-detail',
            component: PermissionDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'product-category-create',
            component: CreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list-product-category',
            component: ListProductCategoryComponent,
            canActivate: [AuthGuard]
          },
          {
           path: 'config-level-customer',
           component: ConfigLevelCustomerComponent,
           canActivate: [AuthGuard]
          },
          {
            path: 'company-config',
            component: CompanyConfigurationComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'system-parameter',
            component: SystemParameterComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'workflow/workflow-create',
            component: WorkflowCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'workflow/workflow-list',
            component: WorkflowListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'workflow/workflow-detail',
            component: WorkflowDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'email-configuration',
            component: EmailConfigurationComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'email-create-template',
            component: EmailCreateTemplateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'folder-config',
            component: FolderConfigurationComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'notifi-setting',
            component: NotificationSettingComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'notifi-setting-detail',
            component: NotificationSettingDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'notifi-setting-list',
            component: NotificationSettingListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'menu-build',
            component: MenuBuildComponent,
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
export class AdminRouting {
}
