import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl, FormControlName } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { MessageService } from 'primeng/api';
import { QuoteService } from '../../../shared/services/quote.services';
import { ContractCostDetail } from '../../models/contract.model';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  contractCostModel: any,
}

interface Cost {
  costId: string;
  costCode: string;
  costName: string;
  costCodeName: string;
  organizationId: string;
  statusId: string;
  active: boolean;
}

@Component({
  selector: 'app-add-edit-contract-cost-dialog',
  templateUrl: './add-edit-contract-cost-dialog.component.html',
  styleUrls: ['./add-edit-contract-cost-dialog.component.css']
})

export class AddEditContractCostDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  /*FORM THÔNG TIN CHI PHÍ*/
  contractCostForm: FormGroup;
  costControl: FormControl;
  costNameControl: FormControl;
  quantityControl: FormControl;
  priceControl: FormControl;
  /*END*/

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listCost: Array<Cost> = [];
  amount: number = 0;
  contractCostDetailModel: ContractCostDetail;
  /*END*/

  /*BIẾN ĐIỀU KIÊN*/
  isCreate: boolean = true;
  isEdit: boolean = true;
  /*END*/


  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private quoteService: QuoteService,
  ) {
    this.isCreate = this.config.data.isCreate;
    // true: thêm mới || false: sửa bản ghi cũ
    if (!this.isCreate) {
      this.isEdit = this.config.data.isEdit;
      this.contractCostDetailModel = this.config.data.contractCostDetailModel;
    }
  }

  ngOnInit() {
    this.setForm();
    this.loading = true;
    this.quoteService.getMasterDataCreateCost(this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let status = result.listStatus.find(c => c.categoryCode == "DSD").categoryId;
        this.listCost = result.listCost.filter(s => s.statusId == status);
        this.setDefaultValue();
      } else {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      contractCostModel: new Object()
    };

    let quantity = parseFloat(this.quantityControl.value.replace(/,/g, ''));
    let priceAmount = parseFloat(this.priceControl.value.replace(/,/g, ''));

    if (!this.contractCostForm.valid) {
      Object.keys(this.contractCostForm.controls).forEach(key => {
        if (this.contractCostForm.controls[key].valid == false) {
          this.contractCostForm.controls[key].markAsTouched();
        }
      });
    } else if (quantity <= 0) {
      this.clear();
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng không thể bằng 0' };
      this.showMessage(msg);
    } else if (priceAmount <= 0) {
      this.clear();
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không thể bằng 0' };
      this.showMessage(msg);
    } else {
      let cost = this.costControl.value;
      let costObj = this.listCost.find(x => x.costId == cost.costId);

      result.contractCostModel.CostId = cost.costId;
      result.contractCostModel.CostCode = costObj.costCode;
      result.contractCostModel.CostName = costObj.costName;
      result.contractCostModel.Quantity = quantity;
      result.contractCostModel.UnitPrice = priceAmount;
      result.contractCostModel.SumAmount = quantity * priceAmount;

      this.ref.close(result);
    }
  }

  setDefaultValue() {
    if (!this.isCreate) {
      let cost = this.listCost.find(c => c.costId == this.contractCostDetailModel.CostId);
      this.costControl.setValue(cost);
      this.costNameControl.setValue(cost.costName);
      this.quantityControl.setValue(this.contractCostDetailModel.Quantity);
      this.priceControl.setValue(this.contractCostDetailModel.UnitPrice);
      this.amount = this.contractCostDetailModel.SumAmount;
    }
  }

  setForm() {
    /*Form thêm chi phí phát sinh*/
    this.costControl = new FormControl(null, [Validators.required]);
    this.costNameControl = new FormControl('');
    this.quantityControl = new FormControl('0');
    this.priceControl = new FormControl('0');

    this.contractCostForm = new FormGroup({
      costControl: this.costControl,
      costNameControl: this.costNameControl,
      quantityControl: this.quantityControl,
      priceControl: this.priceControl
    });
  }

  //Event thi thay đổi chi phí
  changeCost(event: any) {
    if (event == null) {
      this.costNameControl.setValue('');
      return;
    }
    let cost = this.listCost.find(c => c.costId == event.costId);
    this.costNameControl.setValue(cost.costName);
  }

  /*Event khi thay đổi Số lượng*/
  changeQuantityCost() {
    let quantity = this.quantityControl.value;
    if (!quantity) {
      this.quantityControl.setValue('0');
    }

    this.calculatorAmountCost();
  }
  /*End*/

  /*Event khi thay đổi Đơn giá*/
  changePriceCost() {
    let price = this.priceControl.value;
    if (!price) {
      this.priceControl.setValue('0');
    }

    this.calculatorAmountCost();
  }
  /*End*/

  /*Tính các giá trị: amount */
  calculatorAmountCost() {
    let quantity: number = parseFloat(this.quantityControl.value.replace(/,/g, ''));
    let price: number = parseFloat(this.priceControl.value.replace(/,/g, ''));

    this.amount = this.roundNumber((quantity * price), parseInt(this.defaultNumberType, 10));
  }
  /*End*/

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      contractCostModel: null
    };
    this.ref.close(result);
  }
  /*End*/

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case 0: {
        result = result;
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

}

//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidator(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) > number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
}