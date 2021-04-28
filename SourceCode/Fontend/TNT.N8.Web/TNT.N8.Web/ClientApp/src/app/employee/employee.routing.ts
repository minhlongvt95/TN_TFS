import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { EmployeeComponent } from './employee.component';
import { AuthGuard } from '../shared/guards/auth.guard';

import { ListComponent } from './components/list/list.component';
import { EmployeeCreateComponent } from './components/employee-profile/employee-create/employee-create.component';
import { EmployeeDetailComponent } from './components/employee-profile/employee-detail/employee-detail.component';
import { CreateEmployeeRequestComponent } from './components/employee-request/create-employee-request/create-employee-request.component';
import { ListEmployeeRequestComponent } from './components/employee-request/list-employee-request/list-employee-request.component';
import { EmployeeSalaryListComponent } from './components/employee-salary/employee-salary-list/employee-salary-list.component';
import { EmployeeTimesheetImportComponent } from './components/employee-salary/employee-timesheet-import/employee-timesheet-import.component';
import { TeacherSalaryListComponent } from './components/employee-salary/teacher-salary-list/teacher-salary-list.component';
import { AssistantSalaryListComponent } from './components/employee-salary/assistant-salary-list/assistant-salary-list.component';
import { DetailEmployeeRequestComponent } from './components/employee-request/detail-employee-request/detail-employee-request.component';
import { EmployeeDashboardComponent } from './components/employee-dashboard/employee-dashboard.component';
import { EmployeeQuitWorkComponent } from './components/employee-quit-work/employee-quit-work.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: EmployeeComponent,
        children: [
          {
            path: 'dashboard',
            component: EmployeeDashboardComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'list',
            component: ListComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'create',
            component: EmployeeCreateComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'detail',
            component: EmployeeDetailComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'request/list',
            component: ListEmployeeRequestComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'request/create',
            component: CreateEmployeeRequestComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'request/detail',
            component: DetailEmployeeRequestComponent,
            canActivate: [AuthGuard]
          },
          {
            path: 'employee-salary/list',
            component: EmployeeSalaryListComponent,
            canActivate: [AuthGuard]
          },
          //,
          //{
          //  path: 'employee-salary/import',
          //  component: EmployeeTimesheetImportComponent,
          //  canActivate: [AuthGuard]
          //}
          //,
          {
            path: 'teacher-salary/list',
            component: TeacherSalaryListComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'assistant-salary/list',
            component: AssistantSalaryListComponent,
            canActivate: [AuthGuard]
          }
          ,
          {
            path: 'employee-quit-work',
            component: EmployeeQuitWorkComponent,
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
export class EmployeeRouting {
}
