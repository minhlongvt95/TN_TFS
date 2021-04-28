import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ProductVendorMappingModel } from '../../models/product.model';
import { ProductService } from '../../services/product.service';


interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  productVendorMappingModel: ProductVendorMappingModel,
}

class category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefauld: boolean;
}

class vendor {
  vendorId: string;
  vendorCode: string;
  vendorName: string;
}

interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  productVendorMappingModel: ProductVendorMappingModel,
}

@Component({
  selector: 'app-vendor-detail-dialog',
  templateUrl: './vendor-detail-dialog.component.html',
  styleUrls: ['./vendor-detail-dialog.component.css']
})

export class VendorDetailDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;
  /*End*/
  /*Các biến nhận giá trị trả về*/
  listUnitMoney: Array<category> = [];
  unitMoneyLabel: string = 'VND';
  unitMoneyOCLabel: string = 'VND';
  listUnitProduct: Array<category> = [];
  listVendor: Array<vendor> = [];

  cols: any[];
  serialNumer: boolean = false;
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();

  productVendorMappingModel: ProductVendorMappingModel = new ProductVendorMappingModel();
  /*Form sản phẩm dịch vụ*/
  vendorDialogForm: FormGroup;
  vendorControl: FormControl;
  vendorProductName: FormControl;
  vendorMiniumQuantity: FormControl;
  fromDate: FormControl;
  toDate: FormControl;
  moneyUnit: FormControl;
  unitPrice: FormControl;
  /*End*/

  /*End*/
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private productService: ProductService
  ) {
    this.isCreate = this.config.data.isCreate;
    if (!this.isCreate) {
      //Nếu là sửa
      this.productVendorMappingModel = this.config.data.productVendorMappingModel;
    }
  }

  async ngOnInit() {
    this.setForm();
    await this.getMasterdata();
    this.setDefaultValueForm();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  setForm() {
    /*Form Sản phẩm dịch vụ*/
    this.vendorControl = new FormControl(null, Validators.required);
    this.vendorProductName = new FormControl(null);
    this.vendorMiniumQuantity = new FormControl('0', [Validators.required]);
    this.fromDate = new FormControl(new Date(), Validators.required);
    this.toDate = new FormControl(null);
    this.moneyUnit = new FormControl(null, Validators.required);
    this.unitPrice = new FormControl('0', Validators.required);

    this.vendorDialogForm = new FormGroup({
      vendorControl: this.vendorControl,
      vendorProductName: this.vendorProductName,
      vendorMiniumQuantity: this.vendorMiniumQuantity,
      fromDate: this.fromDate,
      toDate: this.toDate,
      moneyUnit: this.moneyUnit,
      unitPrice: this.unitPrice,
    });
    /*End*/
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.productService.getMasterDataVendorDialog();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listUnitMoney = result.listProductMoneyUnit;
      this.listVendor = result.listVendor;
      this.listVendor.forEach(item => {
        item.vendorName = item.vendorCode + ' - ' + item.vendorName;
      });
    } else {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  cancel() {
    this.ref.close();
  }

  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      productVendorMappingModel: new ProductVendorMappingModel()
    };
    let unitPrice = parseFloat(this.unitPrice.value.replace(/,/g, ''));
    let quantity = parseFloat(this.vendorMiniumQuantity.value.replace(/,/g, ''));

    if (!this.vendorDialogForm.valid) {
      Object.keys(this.vendorDialogForm.controls).forEach(key => {
        if (this.vendorDialogForm.controls[key].valid == false) {
          this.vendorDialogForm.controls[key].markAsTouched();
        }
      });
    } else if (unitPrice <= 0) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được bằng 0' };
      this.showMessage(msg);
    } else if (unitPrice > 999000000000) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được vượt quá 999,000,000,000 VND' };
      this.showMessage(msg);
    }
    else if (quantity > 9999999999) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng tối thiểu không được vượt quá 9,999,999,999' };
      this.showMessage(msg);
    }
    else {
      result.productVendorMappingModel.VendorId = this.vendorDialogForm.controls['vendorControl'].value.vendorId;
      result.productVendorMappingModel.VendorCode = this.vendorDialogForm.controls['vendorControl'].value.vendorCode;
      result.productVendorMappingModel.VendorName = this.vendorDialogForm.controls['vendorControl'].value.vendorName;
      result.productVendorMappingModel.VendorProductName = this.vendorDialogForm.controls['vendorProductName'].value;
      result.productVendorMappingModel.MiniumQuantity = parseFloat(this.vendorDialogForm.controls['vendorMiniumQuantity'].value.replace(/,/g, ''));
      result.productVendorMappingModel.Price = parseFloat(this.vendorDialogForm.controls['unitPrice'].value.replace(/,/g, ''));
      result.productVendorMappingModel.MoneyUnitId = this.vendorDialogForm.controls['moneyUnit'].value.categoryId;
      result.productVendorMappingModel.MoneyUnitName = this.vendorDialogForm.controls['moneyUnit'].value.categoryName;
      let fromDate = this.vendorDialogForm.controls['fromDate'].value;
      result.productVendorMappingModel.FromDate = fromDate == null ? fromDate : convertToUTCTime(fromDate);
      let toDate = this.vendorDialogForm.controls['toDate'].value;
      result.productVendorMappingModel.ToDate = toDate == null ? toDate : convertToUTCTime(toDate);
      this.ref.close(result);
    }
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
    if (this.isCreate) {
      //Đơn vị tiền
      let toSelectUnitMoneyProduct = this.listUnitMoney.find(x => x.isDefauld == true);
      this.moneyUnit.setValue(toSelectUnitMoneyProduct);
    } else {
      let vendor = this.listVendor.find(x => x.vendorId == this.productVendorMappingModel.VendorId);
      let moneyUnit = this.listUnitMoney.find(x => x.categoryId == this.productVendorMappingModel.MoneyUnitId);
      this.vendorDialogForm.controls['vendorControl'].setValue(vendor);
      this.vendorDialogForm.controls['vendorProductName'].setValue(this.productVendorMappingModel.VendorProductName);
      this.vendorDialogForm.controls['vendorMiniumQuantity'].setValue(this.productVendorMappingModel.MiniumQuantity);
      this.vendorDialogForm.controls['unitPrice'].setValue(this.productVendorMappingModel.Price);
      this.vendorDialogForm.controls['moneyUnit'].setValue(moneyUnit);
      this.vendorDialogForm.controls['fromDate'].setValue(new Date(this.productVendorMappingModel.FromDate));
      if (this.productVendorMappingModel.ToDate != null && this.productVendorMappingModel.ToDate != undefined) {
        this.vendorDialogForm.controls['toDate'].setValue(new Date(this.productVendorMappingModel.ToDate));
      }
    }
  }
  /*End*/
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
