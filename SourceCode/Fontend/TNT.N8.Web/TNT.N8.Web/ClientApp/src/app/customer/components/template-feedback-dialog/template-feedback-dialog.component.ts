import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

//MODELS
import { CustomerCareFeedBack } from '../../models/customer-care-feed-back.model';

//SERVICES
import { CategoryService } from '../../../shared/services/category.service';
import { CustomerCareService } from '../../services/customer-care.service';

@Component({
  selector: 'app-template-feedback-dialog',
  templateUrl: './template-feedback-dialog.component.html',
  styleUrls: ['./template-feedback-dialog.component.css']
})
export class TemplateFeedbackDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  isCreate: boolean = null;

  customerCareTitle: string = null;
  customerCareContactTypeName: string = null;
  listFeedBackCode: Array<any> = [];

  customerCareFeedBackModel: CustomerCareFeedBack = new CustomerCareFeedBack();

  /*Form feed back*/
  formFeedBack: FormGroup;

  feedBackCodeControl: FormControl;
  feedBackContentControl: FormControl;
  /*End*/

  constructor(
    @Inject(MAT_DIALOG_DATA) public dataDialog: any,
    public dialogRef: MatDialogRef<TemplateFeedbackDialogComponent>,
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private customerCareService: CustomerCareService
  ) { 
    this.customerCareTitle = this.dataDialog.customerCareTitle;
    this.customerCareContactTypeName = this.dataDialog.customerCareContactTypeName;

    this.customerCareFeedBackModel.FeedBackFromDate = this.dataDialog.effecttiveFromDate;
    this.customerCareFeedBackModel.FeedBackToDate = this.dataDialog.effecttiveToDate;
    this.customerCareFeedBackModel.FeedbackType = this.dataDialog.customerCareContactType;
    this.customerCareFeedBackModel.CustomerId = this.dataDialog.customerId;
    this.customerCareFeedBackModel.CustomerCareId = this.dataDialog.customerCareId;
  }

  ngOnInit() {
    this.feedBackCodeControl = new FormControl(null, [Validators.required]);
    this.feedBackContentControl = new FormControl('', [Validators.required]);

    this.formFeedBack = new FormGroup({
      feedBackCodeControl: this.feedBackCodeControl,
      feedBackContentControl: this.feedBackContentControl
    });

    //get list feedback code
    this.categoryService.getAllCategoryByCategoryTypeCode('MHO').subscribe(response => {
      let result = <any>response;
      this.listFeedBackCode = result.category;
    }, error => { });

    //get feedback by customerId and customerCareId
    this.customerCareService.getCustomerCareFeedBackByCusIdAndCusCareId(this.customerCareFeedBackModel.CustomerId,
      this.customerCareFeedBackModel.CustomerCareId, this.userId).subscribe(response => {
      let result = <any>response;
      if (result.statusCode !== 200) {
        this.dialogRef.close({message: "faild", messageCode: result.messageCode});
      } else {
        if (result.customerCareFeedBack == null) {
          //Nếu chưa có feedback
          this.isCreate = true;
        } else {
          //Nếu đã có feedback
          this.isCreate = false;

          this.customerCareFeedBackModel.CustomerCareFeedBackId = result.customerCareFeedBack.customerCareFeedBackId;
          this.customerCareFeedBackModel.FeedBackCode = result.customerCareFeedBack.feedBackCode;
          this.customerCareFeedBackModel.FeedBackContent = result.customerCareFeedBack.feedBackContent;
          this.customerCareFeedBackModel.CreateDate = result.customerCareFeedBack.createDate;
          this.customerCareFeedBackModel.CreateById = result.customerCareFeedBack.createById;
        }
      }
    }, error => { });
  }

  saveFeedBack() {
    if (!this.formFeedBack.valid) { 
      Object.keys(this.formFeedBack.controls).forEach(key => {
        if (!this.formFeedBack.controls[key].valid ) {
          this.formFeedBack.controls[key].markAsTouched();
        }
      });
    } else {
      if (this.isCreate) {
        this.customerCareFeedBackModel.CreateDate = new Date();
        this.customerCareFeedBackModel.CreateById = this.userId;

        this.customerCareService.createCustomerCareFeedBack(this.customerCareFeedBackModel, this.userId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode !== 200) {
            this.dialogRef.close({message: "faild", messageCode: result.messageCode});
          } else {
            this.dialogRef.close({message: "success", messageCode: result.messageCode});
          }
        }, error => { this.dialogRef.close({message: "faild", messageCode: "error"}); });
      } else {
        this.customerCareFeedBackModel.UpdateDate = new Date();
        this.customerCareFeedBackModel.UpdateById = this.userId;

        this.customerCareService.updateCustomerCareFeedBack(this.customerCareFeedBackModel, this.userId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode !== 200) {
            this.dialogRef.close({message: "faild", messageCode: result.messageCode});
          } else {
            this.dialogRef.close({message: "success", messageCode: result.messageCode});
          }
        }, error => { this.dialogRef.close({message: "faild", messageCode: "error"}); });
      }
    }
  }

  onCancelClick() {
    this.dialogRef.close();
  }

}
