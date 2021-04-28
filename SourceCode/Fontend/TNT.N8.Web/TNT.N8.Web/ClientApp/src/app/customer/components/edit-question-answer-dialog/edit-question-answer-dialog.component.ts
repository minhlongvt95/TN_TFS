import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

//SERVICES
import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-edit-question-answer-dialog',
  templateUrl: './edit-question-answer-dialog.component.html',
  styleUrls: ['./edit-question-answer-dialog.component.css']
})
export class EditQuestionAnswerDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;

  customerAdditionalInformationId: string = null;
  question: string = null;
  answer: string = "";

  formAdditional: FormGroup;
  questionControl: FormControl;
  answerControl: FormControl;

  constructor(
    @Inject(MAT_DIALOG_DATA) public dataDialog: any,
    public dialogRef: MatDialogRef<EditQuestionAnswerDialogComponent>,
    private fb: FormBuilder,
    private customerService: CustomerService
  ) {
    this.customerAdditionalInformationId = this.dataDialog.customerAdditionalInformationId;
    this.question = this.dataDialog.question;
    this.answer = this.dataDialog.answer;
  }

  ngOnInit() {
    this.questionControl = new FormControl('', [Validators.required]);
    this.answerControl = new FormControl('');

    this.formAdditional = new FormGroup({
      questionControl: this.questionControl,
      answerControl: this.answerControl
    });
  }

  sendAddtional() {
    if (!this.formAdditional.valid) { 
      Object.keys(this.formAdditional.controls).forEach(key => {
        if (!this.formAdditional.controls[key].valid ) {
          this.formAdditional.controls[key].markAsTouched();
        }
      });
    } else {
      this.customerService.editCustomerAdditional(this.customerAdditionalInformationId, this.question, this.answer, this.userId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode !== 200) {
          this.dialogRef.close({message: "faild", messageCode: result.messageCode});
        } else {
          this.dialogRef.close({message: "success", messageCode: result.messageCode});
        }
      }, error => { });
    }
  }

  onCancelClick() {
    this.dialogRef.close();
  }

}
