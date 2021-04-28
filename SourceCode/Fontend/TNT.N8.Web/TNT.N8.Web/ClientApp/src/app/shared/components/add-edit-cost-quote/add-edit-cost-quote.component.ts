import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { QuoteService } from '../../services/quote.services';
import { MessageService } from 'primeng/api';

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: any,
}

interface Category {
  categoryId: string,
  categoryCode: string,
  categoryName: string,
  isDefault: boolean;
}

@Component({
  selector: 'app-add-edit-cost-quote',
  templateUrl: './add-edit-cost-quote.component.html',
  styleUrls: ['./add-edit-cost-quote.component.css']
})
export class PopupAddEditCostQuoteDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  /*End*/

  /*Các biến nhận giá trị trả về*/
  listCost: Array<any> = [];
  amountProduct: number = 0;  //amountProduct = quantityProduct * priceProduct (sản phẩm dịch vụ)
  cols: any[];
  quoteCostDetailModel: any;
  isEdit: boolean = true;
  /*End*/

  /*Form sản phẩm dịch vụ*/
  productForm: FormGroup;
  costControl: FormControl; //y
  costNameControl: FormControl; //y
  quantityCostControl: FormControl; //y
  priceCostControl: FormControl; //y
  /*End*/

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private quoteService: QuoteService,
    private messageService: MessageService
  ) {
    this.isCreate = this.config.data.isCreate;
    if (!this.isCreate) {
      //Nếu là sửa
      this.quoteCostDetailModel = this.config.data.quoteDetailModel;
      let edit: boolean = this.config.data.isEdit;
      if (edit != true && edit != false) {
        this.isEdit = true;
      } else {
        this.isEdit = this.config.data.isEdit;
      }
      this.selectedOrderDetailType = 0;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngOnInit() {
    this.setForm();
    this.loading = true;
    this.quoteService.getMasterDataCreateCost(this.auth.UserId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let status = result.listStatus.find(c => c.categoryCode == "DSD").categoryId;
        //Lấy các chi phí Đang sử dụng
        this.listCost = result.listCost.filter(s => s.statusId == status);

        this.setDefaultValueForm();

        this.setTable();
      } else {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    /*Form Sản phẩm dịch vụ*/
    this.costControl = new FormControl(null, [Validators.required]);
    this.costNameControl = new FormControl({ value: '', disabled: true });
    this.quantityCostControl = new FormControl('0');
    this.priceCostControl = new FormControl('0');

    this.productForm = new FormGroup({
      costControl: this.costControl,
      costNameControl: this.costNameControl,
      quantityCostControl: this.quantityCostControl,
      priceCostControl: this.priceCostControl
    });
    /*End*/
  }

  setTable() {
    this.cols = [
      { field: 'AttrName', header: 'Tên thuộc tính', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'AttrValue', header: 'Giá trị', width: '50%', textAlign: 'left', color: '#f44336' },
    ];
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
    if (this.isCreate) {

    } else {
      if (this.selectedOrderDetailType == 0) {
        let toSelectCost = this.listCost.find(x => x.costId == this.quoteCostDetailModel.costId);
        this.costControl.setValue(toSelectCost);

        let costName = toSelectCost.costName;
        this.costNameControl.setValue(costName);


        this.quantityCostControl.setValue(this.quoteCostDetailModel.quantity.toString());
        this.priceCostControl.setValue(this.quoteCostDetailModel.unitPrice.toString());

        //Gán giá trị lại cho các biến lưu số thành tiền
        this.calculatorAmountProduct();
      }
    }
  }
  /*End*/

  /*Event khi thay đổi OrderDetailType (Sản phẩm dịch vụ hoặc Chi phí phát sinh)*/
  changeOrderDetailType(orderDetailType: number) {
    this.selectedOrderDetailType = orderDetailType; //thay đổi kiểu dữ liệu từ text => number
  }
  /*End*/

  /*Event khi thay đổi sản phẩm dịch vụ*/
  changeProduct(product: any) {
    /*reset và setValue các control còn lại*/
    this.costNameControl.setValue('');
    this.quantityCostControl.setValue('0');
    this.priceCostControl.setValue('0');
    this.amountProduct = 0;
    /*End*/

    if (product) {
      let costName = this.listCost.find(x => x.costId == product.costId).costName;
      this.costNameControl.setValue(costName);
    }
  }
  /*End*/

  /*Event khi thay đổi Số lượng*/
  changeQuantityProduct() {
    let quantity = this.quantityCostControl.value;
    if (!quantity) {
      this.quantityCostControl.setValue('0');
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Event khi thay đổi Đơn giá*/
  changePriceProduct() {
    let price = this.priceCostControl.value;
    if (!price) {
      this.priceCostControl.setValue('0');
    }

    this.calculatorAmountProduct();
  }
  /*End*/

  /*Tính các giá trị: amountProduct, amountVatProduct, amountDiscountProduct*/
  calculatorAmountProduct() {
    let quantity: number = parseFloat(this.quantityCostControl.value.replace(/,/g, ''));
    let price: number = parseFloat(this.priceCostControl.value.replace(/,/g, ''));

    this.amountProduct = this.roundNumber((quantity * price), parseInt(this.defaultNumberType, 10));
  }
  /*End*/

  /*Event Hủy dialog*/
  cancel() {
    let result: ResultDialog = {
      status: false,  //Hủy
      quoteDetailModel: null
    };
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      quoteDetailModel: new Object()
    };

    if (this.selectedOrderDetailType == 0) {
      /*Nếu là thêm Sản phẩm dịch vụ*/
      let quantity = parseFloat(this.quantityCostControl.value.replace(/,/g, ''));
      let priceAmount = parseFloat(this.priceCostControl.value.replace(/,/g, ''));
      
      if (!this.productForm.valid) {
        Object.keys(this.productForm.controls).forEach(key => {
          if (this.productForm.controls[key].valid == false) {
            this.productForm.controls[key].markAsTouched();
          }
        });
      } else if (quantity <= 0) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập số lượng' };
        this.showMessage(msg);
      } else if (priceAmount <= 0) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa nhập đơn giá' };
        this.showMessage(msg);
      } else {
        let cost = this.costControl.value;
        let costObj = this.listCost.find(x => x.costId == cost.costId);

        result.quoteDetailModel.costId = cost.costId;
        result.quoteDetailModel.costCode = costObj.costCode;
        result.quoteDetailModel.costName = costObj.costName;
        result.quoteDetailModel.quantity = quantity;
        result.quoteDetailModel.unitPrice = priceAmount;
        result.quoteDetailModel.sumAmount = quantity * priceAmount;

        this.ref.close(result);
      }
    }
  }
  /*End*/

  /*Reset form sản phẩm dịch vụ*/
  resetProductForm() {
    this.costControl.reset();
    this.costNameControl.setValue('');
    this.quantityCostControl.setValue('0');
    this.priceCostControl.setValue('0');

    //Gán giá trị lại cho các biến lưu số thành tiền
    this.amountProduct = 0;
  }
  /*End*/

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
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

//Không được nhập chỉ có khoảng trắng
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
    if (control.value !== undefined && (isNaN(control.value) || control.value < min || control.value > max)) {
      return { 'ageRange': true };
    }
    return null;
  };
}
