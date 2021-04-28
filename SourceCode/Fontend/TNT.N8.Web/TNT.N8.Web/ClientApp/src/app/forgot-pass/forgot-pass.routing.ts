import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ForgotPassComponent } from './forgot-pass.component';
import { ForgotPassCheckComponent } from './components/forgot-pass-check/forgot-pass-check.component';
import { ForgotPassChangeComponent } from './components/forgot-pass-change/forgot-pass-change.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: ForgotPassComponent,
        children: [
          {
            path: 'check',
            component: ForgotPassCheckComponent
          },
          {
            path: 'change',
            component: ForgotPassChangeComponent
          },
          {
            path: 'change/:code',
            component: ForgotPassChangeComponent
          }
        ]
      }
    ])
  ],
  exports: [
    RouterModule
  ]
})
export class ForgotPassRouting {
}
