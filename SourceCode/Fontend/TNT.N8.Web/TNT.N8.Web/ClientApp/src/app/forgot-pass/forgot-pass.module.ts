import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { ForgotPassComponent } from './forgot-pass.component';
import { ForgotPassRouting } from './forgot-pass.routing';
import { ForgotPassCheckComponent } from './components/forgot-pass-check/forgot-pass-check.component';
import { ForgotPassChangeComponent } from './components/forgot-pass-change/forgot-pass-change.component';

import { ForgotPassCheckService } from './components/forgot-pass-check/services/forgot-pass-check.service';
import { ForgotPassChangeService } from './components/forgot-pass-change/services/forgot-pass-change.service';
import { RecaptchaModule, RECAPTCHA_LANGUAGE } from 'ng-recaptcha';

@NgModule({
  imports: [
    RecaptchaModule,
    SharedModule,
    ForgotPassRouting
  ],
  declarations: [ForgotPassComponent, ForgotPassCheckComponent, ForgotPassChangeComponent],
  providers: [ForgotPassCheckService, ForgotPassChangeService,
    {
      provide: RECAPTCHA_LANGUAGE,
      useValue: 'vi',
    },
  ]
})
export class ForgotPassModule { }
