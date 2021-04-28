import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ForgotPassModel } from '../../models/forgotPass.model';
import { ForgotPassCheckService } from './services/forgot-pass-check.service';

@Component({
  selector: 'app-forgot-pass-check',
  templateUrl: './forgot-pass-check.component.html',
  styleUrls: ['./forgot-pass-check.component.css']
})
export class ForgotPassCheckComponent implements OnInit {
  model: ForgotPassModel = {
    UserId: '',
    UserName: '',
    FullName: '',
    EmailAddress: '',
    Password: '',
    Re_password: ''
  };
  loading = false;
  state: number = 1;
  success: boolean = false;
  forgotPassMessageCode: string = '';
  capcharLogo: string;
  capcharInput: string = '';
  capcharCheck: string = '';

  isCheckCaptcha: boolean = false;
  validateCaptcha: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private translate: TranslateService,
    private forgotPassCheckService: ForgotPassCheckService
  ) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.capcharText();
  }

  capcharText() {
    if (this.capcharCheck == null || this.capcharCheck === '' || this.capcharCheck === undefined) this.capcharCheck = '1234567890';
    var random = this.capcharCheck[Math.floor(Math.random() * this.capcharCheck.length)];
    for (let i = 0; i < 3; i++) {
      random = random + this.capcharCheck[Math.floor(Math.random() * this.capcharCheck.length)];
    }
    this.capcharLogo = random;
  }

  resolved(captchaResponse: string) {
    if (captchaResponse != null) {
      this.isCheckCaptcha = true;
      this.validateCaptcha = false;
    }
    else {
      this.isCheckCaptcha = false;
      this.validateCaptcha = true;
    }

  }

  check_user() {
    if (this.capcharInput !== this.capcharLogo) {
      this.validateCaptcha = true;
      // this.capcharInput = '';
      this.capcharText();
      return;
    }
    this.loading = true;
    this.forgotPassCheckService.check_user(this.model)
      .subscribe(res => {
        let result = <any>res;
        if (result && result.statusCode === 202) {
          this.model.UserId = result.userId;
          this.model.FullName = result.fullName;
          this.model.EmailAddress = result.emailAddress;
          this.send()
            .then((result) => {
              this.loading = false;
              this.success = true;
            })
            .catch((error) => {
              this.loading = false;
              this.state = -1;
              this.forgotPassMessageCode = error.messageCode;
            })
        } else {
          this.loading = false;
          this.state = -1;
          this.forgotPassMessageCode = result.messageCode;
        }
      },
        error => {
          this.loading = false;
        });
  }

  re_send() {
    this.loading = true;
    this.send()
      .then((result) => {
        this.loading = false;
      })
      .catch((error) => {
        this.loading = false;
        this.forgotPassMessageCode = error.messageCode;
      })
  }

  send() {

    if (this.isCheckCaptcha) {
      this.validateCaptcha = true;
      this.loading = false;
      return;
    }

    return new Promise((resolve, reject) => {
      this.forgotPassCheckService.send_email(this.model)
        .subscribe(res => {
          let result = <any>res;
          if (result && result.statusCode === 200) {
            resolve(result);
          }
          else {
            reject(result);
          }
        })
    })
  }

  onKey(event: any) {
    this.state = 1;
  }

  redirectLogin() {
    this.router.navigate(['login']);
  }

}
