import { Component, OnInit, Inject, ElementRef } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as $ from 'jquery';

export interface IDialogData {
  isApprove: boolean;
  ok: boolean;
  rejectMessage: string;
}

@Component({
  selector: 'app-approverejectpopup',
  templateUrl: './approverejectpopup.component.html',
  styleUrls: ['./approverejectpopup.component.css']
})
export class ApproveRejectPopupComponent implements OnInit {
  approveForm: FormGroup;
  reasonControl: FormControl;

  constructor(
    public dialogRef: MatDialogRef<ApproveRejectPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private el: ElementRef) {
    this.reasonControl = new FormControl('', [Validators.required]);
    this.approveForm = new FormGroup({
      reasonControl: this.reasonControl
    });
  }

  onCancelClick(): void {
    this.data.ok = false;
    this.dialogRef.close(this.data);
  }

  onOkClick(): void {
    if (this.data.isApprove) {
      this.data.ok = true;
      this.dialogRef.close(this.data);
    } else {
      if (!this.approveForm.valid) {
        Object.keys(this.approveForm.controls).forEach(key => {
          if (this.approveForm.controls[key].valid == false) {
            this.approveForm.controls[key].markAsTouched();
          }
        });

        let target;

        target = this.el.nativeElement.querySelector('.form-control');

        if (target) {
          $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
          target.focus();
        }
      } else {
        this.data.ok = true;
        this.dialogRef.close(this.data);
      }
    }
  }

  ngOnInit() {
  }
}
