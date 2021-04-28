import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';

export interface IDialogData {
  title: string;
  content: string;
  mode: string;
  ok: boolean;
  message: string;
}

@Component({
  selector: 'app-popupConfirm',
  templateUrl: './popupConfirm.component.html',
  styleUrls: ['./popupConfirm.component.css']
})
export class PopupConfirmComponent implements OnInit {
  popupForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<PopupConfirmComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private formBuilder: FormBuilder) { }

  onCancelClick(): void {
    this.data.ok = false;
    this.dialogRef.close(this.data.ok);
  }
  

  ngOnInit() {
    this.popupForm = this.formBuilder.group({
        reasonControl: ['', [Validators.required]]
      });
  }
}
