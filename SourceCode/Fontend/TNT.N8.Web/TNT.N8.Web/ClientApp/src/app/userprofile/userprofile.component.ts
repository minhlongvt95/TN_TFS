import { Component, OnInit, ElementRef } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service"
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SuccessComponent } from '../shared/toast/success/success.component';
import { FailComponent } from '../shared/toast/fail/fail.component';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { PopupComponent } from '../shared/components/popup/popup.component';
import * as $ from 'jquery';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.css']
})
export class UserprofileComponent implements OnInit {
  dialogPopup: MatDialogRef<PopupComponent>;
  constructor(
    private el: ElementRef,
    private snackBar: MatSnackBar,
    private translate: TranslateService,
    private dialog: MatDialog,
    private authService: AuthenticationService,
    private route: ActivatedRoute,
    public dialogPop: MatDialog,
    private router: Router) 
  { }

  /*Khai bao FormControl*/
  profileForm: FormGroup;
  formEmail: FormControl;
  formFirstname: FormControl;
  formLastname: FormControl;
  formUsername: FormControl;
  /*End */

  /*Khai báo Info trong UserProfile */
  avatarUrl: string;
  email: string;
  firstname: string;
  lastname: string;
  username: string;
  userId: any = JSON.parse(localStorage.getItem("auth")).UserId;
  /*End*/

  toastMessage: string;

  ngOnInit() {
    this.getUserProfileById();

    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';
    this.formEmail = new FormControl('', [Validators.required, Validators.pattern(emailPattern)]);
    this.formFirstname = new FormControl('', Validators.required);
    this.formLastname = new FormControl('', Validators.required);
    this.formUsername = new FormControl('', Validators.required);

    this.profileForm = new FormGroup({
      formEmail: this.formEmail,
      formFirstname: this.formFirstname,
      formLastname: this.formLastname,
      formUsername: this.formUsername
    })
  }
  cancelCreate() {
    this.router.navigate(['/home']);

    // let _title = "XÁC NHẬN";
    // let _content = "Bạn có chắc chắn hủy? Các dữ liệu sẽ không được lưu.";
    // this.dialogPopup = this.dialogPop.open(PopupComponent,
    //   {
    //     width: '500px',
    //     height: '300px',
    //     autoFocus: false,
    //     data: { title: _title, content: _content }
    //   });

    // this.dialogPopup.afterClosed().subscribe(result => {
    //   if (result) {
    //     this.router.navigate(['/home']);
    //   }
    // });
  }
  UpdateUserProfile() {
    if (!this.profileForm.valid) {
      Object.keys(this.profileForm.controls).forEach(key => {
        if (this.profileForm.controls[key].valid == false) {
          this.profileForm.controls[key].markAsTouched();
        }
      });

      let target;

      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      this.authService.editUserProfile(this.userId, this.username, this.firstname, this.lastname, this.email, this.avatarUrl).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 200 || result.statusCode === 202) {
          this.translate.get('toast.userprofile.edit_success').subscribe(value => { this.toastMessage = value; });
          this.snackBar.openFromComponent(SuccessComponent,
            {
              duration: 5000,
              data: this.toastMessage,
              panelClass: 'success-dialog',
              horizontalPosition: 'end'
            });
          localStorage.setItem("Username", this.username);
          localStorage.setItem("UserAvatar", this.avatarUrl);
          localStorage.setItem("UserFullName", this.firstname.trim() + ' ' + this.lastname.trim());
          localStorage.setItem("UserEmail", this.email);
        } else {
          this.translate.get('toast.userprofile.edit_fail').subscribe(value => { this.toastMessage = value; });
          this.snackBar.openFromComponent(FailComponent, {
            duration: 5000,
            data: this.toastMessage,
            panelClass: 'fail-dialog',
            horizontalPosition: 'end'
          });
        }
        setTimeout(function () {
          window.location.reload();
        }, 5000);
      }, error => { })
    }
  }

  getUserProfileById() {
    this.authService.getUserProfile(this.userId).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 200 || result.statusCode === 202) {
        this.avatarUrl = result.avatarUrl;
        this.email = result.email;
        this.firstname = result.firstName;
        this.lastname = result.lastName;
        this.username = result.username;
      }
    }, error => { });
  }

  // Mo ImageUpload
  openImageUpload() {

  }
  //End
}
