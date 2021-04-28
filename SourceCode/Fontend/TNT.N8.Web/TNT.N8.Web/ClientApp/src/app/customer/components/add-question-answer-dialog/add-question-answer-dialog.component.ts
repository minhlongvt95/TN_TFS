import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

//SERVICES
import { CustomerService } from '../../services/customer.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  listCustomerAdditionalInformation: Array<any>  //list câu hỏi và câu trả lời sau khi thêm/sửa
}

@Component({
  selector: 'app-add-question-answer-dialog',
  templateUrl: './add-question-answer-dialog.component.html',
  styleUrls: ['./add-question-answer-dialog.component.css']
})
export class AddQuestionAnswerDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;

  customerAdditionalInformationId: string = null;
  customerId: string = null;

  formAdditional: FormGroup;
  questionControl: FormControl;
  answerControl: FormControl;

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private customerService: CustomerService,
    private messageService: MessageService
  ) {
    this.customerId = this.config.data.customerId;
    if (this.config.data.customerAdditionalInformationId != null) {
      this.customerAdditionalInformationId = this.config.data.customerAdditionalInformationId;
    }
  }

  ngOnInit() {
    this.questionControl = new FormControl('', [Validators.required, Validators.maxLength(200), forbiddenSpaceText]);
    this.answerControl = new FormControl('', [Validators.maxLength(300)]);

    this.formAdditional = new FormGroup({
      questionControl: this.questionControl,
      answerControl: this.answerControl
    });

    if (this.customerAdditionalInformationId) {
      //Cập nhật
      this.questionControl.setValue(this.config.data.question);
      this.answerControl.setValue(this.config.data.answer);
    }
  }

  awaitResponse: boolean = false;
  sendAddtional() {
    let resultDialog: ResultDialog = {
      status: false,  //Hủy
      listCustomerAdditionalInformation: []
    };
    
    if (!this.formAdditional.valid) { 
      Object.keys(this.formAdditional.controls).forEach(key => {
        if (!this.formAdditional.controls[key].valid ) {
          this.formAdditional.controls[key].markAsTouched();
        }
      });
    } else {
      this.awaitResponse = true;
      this.customerService.createCustomerAdditional(this.customerAdditionalInformationId, this.questionControl.value, 
                this.answerControl.value, this.customerId, this.userId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode == 200) {
          resultDialog.status = true;
          resultDialog.listCustomerAdditionalInformation = result.listCustomerAdditionalInformation;
          this.ref.close(resultDialog);
        } else {
          this.awaitResponse = false;
          let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
          this.showMessage(msg);
        }
      }, error => { });
    }
  }

  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      listCustomerAdditionalInformation: []
    };
    this.ref.close(result);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}
