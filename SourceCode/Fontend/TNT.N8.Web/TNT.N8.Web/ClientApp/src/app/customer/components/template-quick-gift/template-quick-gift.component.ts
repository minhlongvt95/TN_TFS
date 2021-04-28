import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { FormGroup, Validators, FormControl, ValidatorFn, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

//MODELS
import { CustomerCareModel } from '../../models/customer-care.model';

//SERVICES
import { CustomerCareService } from '../../services/customer-care.service';
import { CategoryService } from '../../../shared/services/category.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

interface ResultDialog {
  status: boolean
}

interface Category {
  categoryId: string,
  categoryCode: string,
  categoryName: string
}

@Component({
  selector: 'app-template-quick-gift',
  templateUrl: './template-quick-gift.component.html',
  styleUrls: ['./template-quick-gift.component.css']
})
export class TemplateQuickGiftComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  loading: boolean = false;
  awaitResult: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  customerId: string = '';
  customerType: number = null;  //Khách hàng doanh nghiệp: 1, Khách hàng cá nhân 2
  isCreateBylead: boolean = false;

  /*Form send quick gift*/
  formQuickGift: FormGroup;

  titleControl: FormControl;
  giftControl: FormControl;
  quantityControl: FormControl;
  /*End*/

  listDefaultIdCusPer: Array<any> = [];
  listDefaultIdCusEnterprise: Array<any> = [];

  listGift: Array<Category> = [];

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerCareService: CustomerCareService,
    private categoryService: CategoryService,
  ) {
    this.customerId = this.config.data.customerId;
    this.customerType = this.config.data.customerType;
    if (this.config.data.isCreateBylead === true) {
      this.isCreateBylead = true;
    }
  }

  ngOnInit() {
    this.titleControl = new FormControl(null, [Validators.required, forbiddenSpaceText, Validators.maxLength(200)]);
    this.giftControl = new FormControl(null, [Validators.required]);
    this.quantityControl = new FormControl('1', [Validators.required, ageRangeValidator(1, 200)]);

    this.formQuickGift = new FormGroup({
      titleControl: this.titleControl,
      giftControl: this.giftControl,
      quantityControl: this.quantityControl
    });

    //list loại quà
    this.categoryService.getAllCategoryByCategoryTypeCode("LQU").subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        this.listGift = result.category;
      } else {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  sendGift() {
    if (!this.formQuickGift.valid) {
      Object.keys(this.formQuickGift.controls).forEach(key => {
        if (!this.formQuickGift.controls[key].valid) {
          this.formQuickGift.controls[key].markAsTouched();
        }
      });
    } else {
      let giftCustomerType1 = null;
      let giftTypeId1 = null;
      let giftTotal1 = null;

      let giftCustomerType2 = null;
      let giftTypeId2 = null;
      let giftTotal2 = null;

      if (this.customerType == 1) {
        //Khách hàng doanh nghiệp
        giftCustomerType1 = this.customerType;
        let toSlectedGift: Category = this.giftControl.value;
        giftTypeId1 = toSlectedGift.categoryId;
        giftTotal1 = parseFloat(this.quantityControl.value.replace(/,/g, ''));
      } else if (this.customerType == 2) {
        //Khách hàng cá nhân
        giftCustomerType2 = this.customerType;
        let toSlectedGift: Category = this.giftControl.value;
        giftTypeId2 = toSlectedGift.categoryId;
        giftTotal2 = parseFloat(this.quantityControl.value.replace(/,/g, ''));
      }

      this.awaitResult = true;

      // if (this.isCreateBylead === true) {
      //   this.customerCareService.sendGiftSupportLead(this.titleControl.value.trim(), giftCustomerType1, giftTypeId1, giftTotal1,
      //     giftCustomerType2, giftTypeId2, giftTotal2, this.customerId).subscribe(response => {
      //       let result = <any>response;
      //       if (result.statusCode == 200) {
      //         let resultDialog: ResultDialog = {
      //           status: true
      //         };

      //         this.ref.close(resultDialog);
      //       } else {
      //         this.awaitResult = false;
      //         let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      //         this.showMessage(msg);
      //       }
      //     });
      // } else {
        this.customerCareService.sendQuickGift(this.titleControl.value.trim(), giftCustomerType1, giftTypeId1, giftTotal1,
          giftCustomerType2, giftTypeId2, giftTotal2, this.customerId).subscribe(response => {
            let result = <any>response;
            if (result.statusCode == 200) {
              let resultDialog: ResultDialog = {
                status: true
              };

              this.ref.close(resultDialog);
            } else {
              this.awaitResult = false;
              let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
      // }
    }
  }

  changeQuantity() {
    if (!this.quantityControl.value) {
      this.quantityControl.setValue('1');
    } else if (parseFloat(this.quantityControl.value.replace(/,/g, '')) < 1) {
      this.quantityControl.setValue('1');
    }
  }

  cancel() {
    this.ref.close();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
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

//So sánh giá trị nhập vào có thuộc khoảng xác định hay không?
function ageRangeValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (isNaN(control.value) ||
      parseFloat(control.value.replace(/,/g, '')) < min ||
      parseFloat(control.value.replace(/,/g, '')) > max)) {
      return { 'ageRange': true };
    }
    return null;
  };
}
