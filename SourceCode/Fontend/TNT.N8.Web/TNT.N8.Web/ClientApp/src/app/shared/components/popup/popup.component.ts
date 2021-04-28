import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';

export interface IDialogData {
  title: string;
  content: string;
  contentline: string;
  mode: string;
  ok: boolean;
  message: string;
}

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.css']
})
export class PopupComponent implements OnInit {
  popupForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<PopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private formBuilder: FormBuilder) { }

  onCancelClick(): void {
    this.data.ok = false;
    this.dialogRef.close(this.data.ok);
  }

  onOkClick(): void {
    this.data.ok = true;
    this.dialogRef.close(this.data.ok);
  }

  onRejectClick(): void{
    if(this.data.mode === 'reject'){
      if(this.popupForm.invalid){
        return;
      }else{
        this.dialogRef.close(this.data);
      }
    }else{
      this.dialogRef.close(this.data);
    }
  }

  ngOnInit() {
    this.popupForm = this.formBuilder.group({
        reasonControl: ['', [Validators.required]]
      });
  }
}
