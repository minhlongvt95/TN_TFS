import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ProductService } from '../../../product/services/product.service';
import { ProductVendorMappingModel, SuggestedSupplierQuotesModel } from '../../models/vendor.model';
import { VendorService } from '../../services/vendor.service';

interface ResultDialog {
  status: boolean;
  message: string;
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

interface ProductVendorMapping {
  productVendorMappingId: string;
  vendorId: string;
  vendorName: string;
  vendorCode: string;
  productCode: string;
  productId: string;
  productName: string;
  productUnitName: string;
  vendorProductCode: string;
  vendorProductName: string;
  miniumQuantity: number;
  price: number;
  moneyUnitId: string;
  moneyUnitName: string;
  fromDate: Date;
  toDate: Date;
  createdById: string;
  createdDate: Date;
  listSuggestedSupplierQuoteId: Array<string>
}

@Component({
  selector: 'app-add-vendor-price-dialog',
  templateUrl: './add-vendor-price-dialog.component.html',
  styleUrls: ['./add-vendor-price-dialog.component.css'],
})
export class AddVendorPriceDialogComponent implements OnInit {

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  //Biến điều kiện
  loading: boolean = false;
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  productVendorMappingModel: ProductVendorMapping;
  listVendorProductPrice: Array<ProductVendorMapping> = [];
  listVendorProductPriceForEdit: Array<ProductVendorMapping> = [];
  listSuggestedSupplierQuote: Array<SuggestedSupplierQuotesModel> = [];
  /*Form sản phẩm dịch vụ*/
  vendorPriceDialogForm: FormGroup;
  productControl: FormControl;
  vendorControl: FormControl;
  vendorProductNameControl: FormControl;
  vendorProductCodeControl: FormControl;
  vendorMiniumQuantityControl: FormControl;
  fromDateControl: FormControl;
  toDateControl: FormControl;
  moneyUnitControl: FormControl;
  unitPriceControl: FormControl;
  suggestedSupplierQuoteControl: FormControl;
  productName: string = '';

  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  isCreate: boolean;
  /*Biến lưu giá trị trả về*/
  listUnitMoney: Array<category> = [];
  listVendor: Array<vendor> = [];
  listProduct: Array<any> = [];

  constructor(
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private productService: ProductService,
    private vendorService: VendorService,
  ) { }

  async ngOnInit() {
    this.isCreate = this.config.data.isCreate;
    this.listVendorProductPrice = this.config.data.listVendorProductPrice;
    if (!this.isCreate) {
      //Nếu là sửa
      this.productVendorMappingModel = this.config.data.productVendorMappingModel;
      // Nếu là sửa thì loại bỏ bản ghi đang được sửa khỏi list dữ liệu để check trùng
      this.listVendorProductPriceForEdit = this.listVendorProductPrice.filter(c => c != this.productVendorMappingModel);
    }
    this.setForm();
    await this.getMasterdata();
    this.setDefaultValueForm();
  }

  setForm() {
    this.productControl = new FormControl(null, [Validators.required]);
    this.vendorControl = new FormControl(null, [Validators.required]);
    this.vendorProductCodeControl = new FormControl('');
    this.vendorProductNameControl = new FormControl('');
    this.vendorMiniumQuantityControl = new FormControl('0', [Validators.required]);
    this.fromDateControl = new FormControl(new Date(), [Validators.required]);
    this.toDateControl = new FormControl(null);
    this.moneyUnitControl = new FormControl(null, [Validators.required]);
    this.unitPriceControl = new FormControl('0', [Validators.required]);
    this.suggestedSupplierQuoteControl = new FormControl([]);

    this.vendorPriceDialogForm = new FormGroup({
      productControl: this.productControl,
      vendorControl: this.vendorControl,
      vendorProductCodeControl: this.vendorProductCodeControl,
      vendorProductNameControl: this.vendorProductNameControl,
      vendorMiniumQuantityControl: this.vendorMiniumQuantityControl,
      fromDateControl: this.fromDateControl,
      toDateControl: this.toDateControl,
      moneyUnitControl: this.moneyUnitControl,
      unitPriceControl: this.unitPriceControl,
      suggestedSupplierQuoteControl: this.suggestedSupplierQuoteControl,
    });
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.productService.getMasterDataVendorDialog();
    this.loading = false;
    if (result.statusCode == 200) {
      this.listUnitMoney = result.listProductMoneyUnit;
      this.listVendor = result.listVendor;
      this.listProduct = result.listProduct;
      this.listProduct.forEach(item => {
        item.productCodeName = item.productCode + ' - ' + item.productName;
      });

      this.listSuggestedSupplierQuote = result.listSuggestedSupplierQuote;
    } else {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      message: '',
    };
    let unitPrice = parseFloat(this.unitPriceControl.value.replace(/,/g, ''));
    let quantity = parseFloat(this.vendorMiniumQuantityControl.value.replace(/,/g, ''));

    if (!this.vendorPriceDialogForm.valid) {
      Object.keys(this.vendorPriceDialogForm.controls).forEach(key => {
        if (this.vendorPriceDialogForm.controls[key].valid == false) {
          this.vendorPriceDialogForm.controls[key].markAsTouched();
        }
      });
    } else if (unitPrice <= 0) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được bằng 0' };
      this.showMessage(msg);
    } else if (unitPrice > 999000000000) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đơn giá không được vượt quá 999,000,000,000 VND' };
      this.showMessage(msg);
    } else if (quantity > 999999) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Số lượng tối thiểu không vượt quá 999,999' };
      this.showMessage(msg);
    } else {
      // map dữ liệu từ form vào model
      let productVendorMapping: ProductVendorMappingModel = this.mappingForm();
      let listSuggestedSupplierQuote: Array<SuggestedSupplierQuotesModel> = this.suggestedSupplierQuoteControl.value;
      let listSuggestedSupplierQuoteId: Array<string> = [];
      if (listSuggestedSupplierQuote) {
        listSuggestedSupplierQuoteId = listSuggestedSupplierQuote.map(c => c.suggestedSupplierQuoteId);
      }
      // Kiểm tra dữ liệu đã tồn tại trong hệ thống
      let check: ProductVendorMapping;

      if (this.isCreate) {
        // kiểm tra dữ liệu trong hệ thống nếu tạo mới
        check = this.listVendorProductPrice.find(c => c.productId == productVendorMapping.ProductId && c.vendorId == productVendorMapping.VendorId
          && c.miniumQuantity == productVendorMapping.MiniumQuantity && c.moneyUnitId == productVendorMapping.MoneyUnitId
          && this.calcDaysDiff(new Date(c.fromDate), new Date(productVendorMapping.FromDate)) == 0);
      } else {
        // kiểm tra dữ liệu khi sửa 1 bản ghi
        check = this.listVendorProductPriceForEdit.find(c => c.productId == productVendorMapping.ProductId && c.vendorId == productVendorMapping.VendorId
          && c.miniumQuantity == productVendorMapping.MiniumQuantity && c.moneyUnitId == productVendorMapping.MoneyUnitId
          && this.calcDaysDiff(new Date(c.fromDate), new Date(productVendorMapping.FromDate)) == 0);
      }

      if (check) {
        let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: 'Đã tồn tại bản ghi trong hệ thống!' };
        this.showMessage(msg);
      } else {
        this.vendorService.createVendorProductPrice(productVendorMapping, listSuggestedSupplierQuoteId).subscribe(response => {
          let result01 = <any>response;
          result.status = true;
          result.message = result01.messageCode;
          this.ref.close(result);
        });
      }
    }
  }

  mappingForm(): ProductVendorMappingModel {
    let productVendorMapping = new ProductVendorMappingModel();
    // Nếu là sửa thì truyền id xuống, nếu là tạo mới thì id = guid.empty
    if (this.isCreate == false) {
      productVendorMapping.ProductVendorMappingId = this.productVendorMappingModel.productVendorMappingId;
    }
    productVendorMapping.ProductId = this.productControl.value.productId;
    productVendorMapping.VendorId = this.vendorControl.value.vendorId;
    productVendorMapping.VendorProductCode = this.vendorProductCodeControl.value;
    productVendorMapping.VendorProductName = this.vendorProductNameControl.value;
    productVendorMapping.MiniumQuantity = parseFloat(this.vendorMiniumQuantityControl.value.replace(/,/g, ''));
    productVendorMapping.Price = parseFloat(this.unitPriceControl.value.replace(/,/g, ''));
    productVendorMapping.MoneyUnitId = this.moneyUnitControl.value.categoryId;
    productVendorMapping.FromDate = this.fromDateControl.value == null ? this.fromDateControl.value : convertToUTCTime(this.fromDateControl.value);
    productVendorMapping.ToDate = this.toDateControl.value == null ? this.toDateControl.value : convertToUTCTime(this.toDateControl.value);

    return productVendorMapping;
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
    if (this.isCreate) {
      //Đơn vị tiền
      let toSelectUnitMoneyProduct = this.listUnitMoney.find(x => x.isDefauld == true);
      this.moneyUnitControl.setValue(toSelectUnitMoneyProduct);
    } else {
      let vendor = this.listVendor.find(x => x.vendorId == this.productVendorMappingModel.vendorId);
      let moneyUnit = this.listUnitMoney.find(x => x.categoryId == this.productVendorMappingModel.moneyUnitId);
      let product = this.listProduct.find(x => x.productId == this.productVendorMappingModel.productId);
      this.productName = product.productName;
      this.vendorControl.setValue(vendor);
      this.productControl.setValue(product);
      this.vendorProductCodeControl.setValue(this.productVendorMappingModel.vendorProductCode);
      this.vendorProductNameControl.setValue(this.productVendorMappingModel.vendorProductName);
      this.vendorMiniumQuantityControl.setValue(this.productVendorMappingModel.miniumQuantity);
      this.unitPriceControl.setValue(this.productVendorMappingModel.price);
      this.moneyUnitControl.setValue(moneyUnit);
      this.fromDateControl.setValue(new Date(this.productVendorMappingModel.fromDate));
      // Ngày hiệu lực đến có thể null
      if (this.productVendorMappingModel.toDate != null) {
        this.toDateControl.setValue(new Date(this.productVendorMappingModel.toDate));
      }

      //Giấy đê nghị báo giá nhà cung cấp
      let listSelected = this.listSuggestedSupplierQuote.filter(x => this.productVendorMappingModel.listSuggestedSupplierQuoteId.includes(x.suggestedSupplierQuoteId));
      this.suggestedSupplierQuoteControl.setValue(listSelected);
    }
  }

  changeProduct(event: any) {
    if (event.value == null) {
      this.productName = '';
      return;
    }
    this.productName = event.value.productName;
  }

  /*End*/
  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  cancel() {
    this.ref.close();
    // this.confirmationService.confirm({
    //   message: 'Hành động này không thể hoàn tác. Bạn có muốn tiếp tục?',
    //   key: 'popup',
    //   accept: () => {
    //     this.ref.close();
    //   }
    // });
  }

  calcDaysDiff(dateFrom, dateTo): number {
    let currentDate = new Date(dateTo);
    let dateSent = new Date(dateFrom);

    return Math.floor((Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()) - Date.UTC(dateSent.getFullYear(), dateSent.getMonth(), dateSent.getDate())) / (1000 * 60 * 60 * 24));
  }

}
function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

