import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ForgotPassModel } from '../../models/forgotPass.model';
import { ForgotPassChangeService } from './services/forgot-pass-change.service';

@Component({
  selector: 'app-forgot-pass-change',
  templateUrl: './forgot-pass-change.component.html',
  styleUrls: ['./forgot-pass-change.component.css']
})
export class ForgotPassChangeComponent implements OnInit {

  model: ForgotPassModel = {
    UserId: '',
    UserName: '',
    FullName: '',
    EmailAddress: '',
    Password: '',
    Re_password: ''
  };
  code: string = '';
  disable: boolean = false;
  loading = false;
  state: number = 1;
  check_pass: number = 1;
  check_re_pass: number = 1;
  patt = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$/;
  success: boolean = false;
  check_pass_message: string = '';
  check_re_pass_message: string = '';
  resetPassMessageCode: string = '';

  constructor(
    public route: ActivatedRoute,
    private router: Router,
    private translate: TranslateService,
    private forgotPassChangeService: ForgotPassChangeService
  ) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.code = this.route.snapshot.params['code'];
    this.check_code(this.code)
      .then((result) => {
        this.success = true;
      })
      .catch((error) => {
        this.success = false;
      })
  }

  check_code(code: string) {
    return new Promise((resolve, reject) => {
      this.forgotPassChangeService.check_code(code)
        .subscribe(result => {
          if (result && result.statusCode === 202) {
            this.model.UserName = result.userName;
            this.model.UserId = result.userId;
            resolve(result);
          }
          else {
            this.resetPassMessageCode = result.messageCode;
            reject(result);
          }
        })
    })
  }

  change_pass() {
    this.state = 1;
    this.disable = true;
    this.loading = true;
    this.reset_pass(this.model.UserId, this.model.Password)
      .then((result) => {
        this.success = false;
        this.disable = false;
        this.loading = false;
      })
      .catch((error) => {
        this.success = false;
        this.disable = false;
        this.loading = false;
      })
  }

  check_compare_pass(re_password: string, password: string) {
    if (re_password != password) {
      return false;
    }
    return true;
  }

  reset_pass(UserId: string, Password: string) {
    return new Promise((resolve, reject) => {
      this.forgotPassChangeService.reset_pass(UserId, Password)
        .subscribe(result => {
          if (result && result.statusCode === 202) {
            this.resetPassMessageCode = "Mật khẩu của tài khoản đã thay đổi thành công!";
            resolve(result);
          }
          else {
            this.resetPassMessageCode = result.messageCode;
            reject(result);
          }
        })
    });
  }

  onKeyPass(event: any) {
    this.resetPassMessageCode = null;
    if (this.model.Password != '') {
      if (this.model.Re_password == '') {
        if (!this.patt.test(this.model.Password)) {
          this.check_pass = -1;
          this.check_pass_message = "Mật khẩu phải có tối thiểu 8 ký tự bao gồm cả chữ cái, chữ số, kí tự đặc biệt";
          this.disable = true;
        } else {
          this.check_pass = 1;
        }
      } else {
        if (!this.patt.test(this.model.Password)) {
          this.check_pass = -1;
          this.check_pass_message = "Mật khẩu phải có tối thiểu 8 ký tự bao gồm cả chữ cái, chữ số, kí tự đặc biệt";
          this.disable = true;
        } else {
          if (this.model.Re_password != this.model.Password) {
            this.check_pass = -1;
            this.check_pass_message = "Mật khẩu nhập lại phải trùng với mật khẩu mới";
            this.disable = true;
          } else {
            this.check_pass = 1;
            this.check_re_pass = 1;
            this.check_pass_message = "";
            this.disable = false;
          }
        }
      }
    } else {
      if (this.model.Re_password != '') {
        this.check_pass = -1;
        this.check_pass_message = "Mật khẩu nhập lại phải trùng với mật khẩu mới";
        this.disable = true;
      } else {
        this.check_pass = 1;
        this.check_pass_message = "";
        this.disable = false;
      }
    }
  }

  onKeyRePass(event: any) {
    this.resetPassMessageCode = null;
    if (this.model.Re_password != '') {
      if (this.model.Password != '') {
        if (this.model.Re_password != this.model.Password) {
          this.check_re_pass = -1;
          this.check_re_pass_message = "Mật khẩu nhập lại phải trùng với mật khẩu mới";
          this.disable = true;
        } else {
          if (this.check_pass == 1) {
            this.check_re_pass = 1;
            this.check_pass_message = '';
            this.check_re_pass_message = '';
            this.disable = false;
          } else {
            this.check_re_pass = 1;
            this.check_re_pass_message = '';
          }
        }
      } else {
        this.check_re_pass = -1;
        this.check_re_pass_message = "Mật khẩu nhập lại phải trùng với mật khẩu mới";
        this.disable = true;
      }
    } else {
      if (this.model.Password == '') {
        this.check_pass = 1;
        this.check_pass_message = '';
        this.disable = false;
      }
      this.check_re_pass = 1;
      this.check_re_pass_message = '';
    }
  }

  redirectLogin() {
    this.router.navigate(['login']);
  }

}
