import { Component, OnInit, Inject, ElementRef } from '@angular/core';
import { Router } from '@angular/router';

import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { ChangepasswordsuccessComponent } from '../changepasswordsuccess/changepasswordsuccess.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import * as $ from 'jquery';

export interface DATADIALOG {
  accountName :string,
  name: string,
  email: string,
  avatar: string
}
@Component({
  selector: 'app-changepassword',
  templateUrl: './changepassword.component.html',
  styleUrls: ['./changepassword.component.css']
})
export class ChangepasswordComponent implements OnInit {
  dialogPopup: MatDialogRef<ChangepasswordsuccessComponent>;
  oldPassword: string;
  newPassword: string;
  newPasswordConfirm: string;
  passwordForm: FormGroup;
  formOldPass: FormControl;
  formNewPass: FormControl;
  formNewPassConfirm: FormControl;

  constructor(
    private router: Router,
    public dialog : MatDialog,
    public dialogRef: MatDialogRef<ChangepasswordComponent>,
    private authService: AuthenticationService,
    public builder: FormBuilder,
    private el: ElementRef,
    public snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: DATADIALOG) {
    this.formOldPass = new FormControl('', [Validators.required]);
    this.formNewPass = new FormControl('', [Validators.required]);
    this.formNewPassConfirm = new FormControl('', [Validators.required]);

    this.passwordForm = builder.group({
      formOldPass: this.formOldPass,
      formNewPass: this.formNewPass,
      formNewPassConfirm: this.formNewPassConfirm
    });
  }

  ngOnInit() {

  }

  changepassword() {
    if(this.newPassword === this.newPasswordConfirm){
      if (!this.passwordForm.valid) {
        Object.keys(this.passwordForm.controls).forEach(key => {
          if (this.passwordForm.controls[key].valid === false) {
            this.passwordForm.controls[key].markAsTouched();
          }
        });
  
        let target;
        target = this.el.nativeElement.querySelector('.form-control.ng-invalid');
        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
      } else {
        var auth = JSON.parse(localStorage.getItem("auth"));
        var userId = auth.UserId;
        this.authService.changePassword(userId, this.oldPassword, this.newPassword).subscribe(response => {
            let result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              this.openChangePasswordSuccess();
            } else {
              this.snackBar.openFromComponent(FailComponent,
                {
                  duration: 5000,
                  data: result.messageCode,
                  panelClass: 'fail-dialog',
                  horizontalPosition: 'end'
                });
            }
          },
          error => {});
      }
    } else{
      this.snackBar.openFromComponent(FailComponent,
        {
          duration: 5000,
          data: 'Mật khẩu mới không khớp',
          panelClass: 'fail-dialog',
          horizontalPosition: 'end'
        });
    }
    
  }

  openChangePasswordSuccess(){
    let _account = this.data.accountName;
    this.dialogPopup = this.dialog.open(ChangepasswordsuccessComponent,
      {
        data: { accountName: _account }
      });
    this.dialogRef.close();
    this.dialogPopup.afterClosed().subscribe(result => {
      this.router.navigate(['/login']);
    });
  }
  cancel(){
    this.dialogRef.close();
  }
}

