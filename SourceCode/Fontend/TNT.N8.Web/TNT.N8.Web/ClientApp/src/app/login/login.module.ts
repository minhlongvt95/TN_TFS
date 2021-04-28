import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { LoginComponent } from './login.component';
import { LoginRouting } from './login.routing';
import { RecaptchaModule, RECAPTCHA_LANGUAGE } from 'ng-recaptcha';

import { AuthenticationService } from '../shared/services/authentication.service';
import { MD5 } from '../shared/adapter/md5';

@NgModule({
  imports: [
    RecaptchaModule,
    SharedModule,    
    LoginRouting
  ],
  declarations: [LoginComponent],
  providers: [
    AuthenticationService,
    {
      provide: RECAPTCHA_LANGUAGE,
      useValue: 'vi',
    },
    MD5
  ]
})
export class LoginModule { }
