import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { QuoteService } from '../../services/quote.service';
import { MessageService } from 'primeng/api';

import { QuoteDetail } from '../../models/quote-detail.model';
import { QuoteProductDetailProductAttributeValue } from '../../models/quote-product-detail-product-attribute-value.model';
import { VendorService } from '../../../vendor/services/vendor.service';


interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: QuoteDetail,
}

interface Category {
  categoryId: string,
  categoryCode: string,
  categoryName: string,
  isDefault: boolean;
}

interface Vendor {
  vendorId: string,
  vendorCode: string,
  vendorName: string
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface ObjectAttrNameProduct {
  productAttributeCategoryId: string,
  productAttributeCategoryName: string  //Màu sắc
}

interface ObjectAttrValueProduct {
  productAttributeCategoryValueId: string,
  productAttributeCategoryValue: string, //Đỏ, Vàng, Xanh, Trắng...
  productAttributeCategoryId: string
}

interface GroupAttrProduct {
  AttrName: ObjectAttrNameProduct,
  AttrValue: Array<ObjectAttrValueProduct>,
  SelectedAttrValue: ObjectAttrValueProduct
}

interface SuggestedSupplierQuotes {
  SuggestedSupplierQuoteId: string,
  SuggestedSupplierQuote: string,
  StatusId: string,
  VendorId: string,
  PersonInChargeId: string,
  RecommendedDate: Date,
  QuoteTermDate: Date,
  ObjectType: string,
  ObjectId: string,
  Note: string,
  Active: boolean,
  CreatedDate: Date,
  CreatedById: string,
  UpdatedDate: Date,
  UpdatedById: string,
  TenantId: string,
}

interface SuggestedSupplierQuoteDetail {
  SuggestedSupplierQuoteDetailId: string,
  SuggestedSupplierQuoteId: string,
  ProductId: string,
  Quantity: number,
  Note: string,
  Active: boolean,
  CreatedDate: Date,
  CreatedById: string,
  UpdatedDate: Date,
  UpdatedById: string,
  TenantId: string,
}

@Component({
  selector: 'app-quote-vendor-dialog',
  templateUrl: './add-quote-vendor-dialog.component.html',
  styleUrls: ['./add-quote-vendor-dialog.component.css']
})
export class AddQuoteVendorDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  /*Các biến nhận giá trị trả về*/
  unitMoneyLabel: string = 'VND';
  unitMoneyOCLabel: string = 'VND';
  listVendor: Array<Vendor> = [];
  amountProduct: number = 0;  //amountProduct = quantityProduct * priceProduct (sản phẩm dịch vụ)
  amountVatProduct: number = 0; //tiền thuế GTGT (sản phẩm dịch vụ)
  amountPriceInitialProduct: number = 0; //tiền thuế GTGT (sản phẩm dịch vụ)
  amountDiscountProduct: number = 0; //tiền chiết khấu (sản phẩm dịch vụ)
  amountOC: number = 0;
  amountVatOC: number = 0;
  amountDiscountOC: number = 0;
  unitPriceVon: number = 0;
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  cols: any[];
  quoteId: string;
  selectedColumns: any[];
  selectedQuoteVendor: any[] = [];
  listObjectAttrNameProduct: Array<ObjectAttrNameProduct> = [];
  listObjectAttrValueProduct: Array<ObjectAttrValueProduct> = [];
  listAttrProduct: Array<GroupAttrProduct> = [];
  listQuoteDetailModel: Array<QuoteDetail> = [];
  IsPriceInitial: boolean = false;
  listData: any[];
  /*End*/

  /*Form sản phẩm dịch vụ*/
  productForm: FormGroup;
  quantityProductControl: FormControl;
  descriptionControl: FormControl;
  vendorControl: FormControl;
  /*End*/

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private vendorService: VendorService,
    private router: Router,
    private messageService: MessageService
  ) {
    //Nếu là sửa
    this.listQuoteDetailModel = this.config.data.listQuoteDetailModel;
    this.quoteId = this.config.data.quoteId;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  trimspace() {
    this.listData.forEach(element => {
      element.Description = element.Description.trim();
      if (element.Quantity == null || element.Quantity == undefined || element.Quantity == '') {
        element.Quantity = 0;
      }
    });
  }

  ngOnInit() {
    this.loading = true;
    this.getMasterData();
  }

  async getMasterData() {
    this.loading = true;
    let [initSearchResponse]: any = await Promise.all([
      this.vendorService.searchVendorAsync('', '', [], this.auth.UserId)
    ]);
    this.loading = false;
    if (initSearchResponse.statusCode === 200) {
      this.listVendor = initSearchResponse.vendorList;
    }

    this.listData = this.listQuoteDetailModel;
    this.listData.forEach(item => {
      if (item.orderDetailType == 0) {
        item.vendor = this.listVendor.find(v => v.vendorId == item.vendorId);
      }
      else {
        item.vendor = null;
      }
    });
    this.setDefaultValueForm();
    this.setTable();
  }

  setTable() {
    this.cols = [
      { field: 'productCode', header: 'Mã sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'description', header: 'Ghi chú', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'vendorId', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
    ];
    this.selectedColumns = this.cols;
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
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
  save(check) {
    let countVendorNull = 0;
    let countQuantity = 0;
    
    this.selectedQuoteVendor.forEach(item => {
      if (item.vendor === undefined || item.vendor === null) {
        countVendorNull++;
      }
      if (item.quantity == 0) {
        countQuantity++;
      }
    });
    if (this.selectedQuoteVendor.length == 0) {
      let msg1 = { key: "popup", severity: 'warn', summary: 'Thông báo:', detail: 'Chưa chọn sản phẩm' };
      this.showMessage(msg1);
    }
    else if (countVendorNull !== 0) {
      let msg1 = { key: "popup", severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn nhà cung cấp' };
      this.showMessage(msg1);
    }
    else if (countQuantity !== 0) {
      let msg1 = { key: "popup", severity: 'error', summary: 'Thông báo:', detail: 'Chưa nhập số lượng' };
      this.showMessage(msg1);
    }
    else {
      var vendorList = [];
      this.selectedQuoteVendor.forEach(item => {
        item.vendorId = item.vendor.vendorId;
        let countVendor = vendorList.find(c => c == item.vendor.vendorId);
        if (countVendor === null || countVendor === undefined) {
          vendorList.push(item.vendor.vendorId);
        }
      })
      let paramRequest: any[] = [];
      vendorList.forEach(async (item, index) => {
        let vendorQuote = this.selectedQuoteVendor.filter(s => s.vendorId == item);
        let obj: SuggestedSupplierQuotes = {
          SuggestedSupplierQuoteId: this.emptyGuid,
          SuggestedSupplierQuote: null,
          StatusId: this.emptyGuid,
          VendorId: item,
          PersonInChargeId: this.auth.EmployeeId,
          RecommendedDate: null,
          QuoteTermDate: null,
          ObjectType: "QUOTE",
          ObjectId: this.quoteId,
          Note: "",
          Active: true,
          CreatedDate: new Date(),
          CreatedById: this.auth.UserId,
          UpdatedDate: null,
          UpdatedById: null,
          TenantId: null
        }
        var objDetail = [];
        vendorQuote.forEach(detail => {
          let detailVendorQuote: SuggestedSupplierQuoteDetail = {
            SuggestedSupplierQuoteDetailId: this.emptyGuid,
            SuggestedSupplierQuoteId: this.emptyGuid,
            ProductId: detail.productId,
            Quantity: detail.quantity,
            Note: detail.description,
            Active: true,
            CreatedDate: new Date(),
            CreatedById: this.auth.UserId,
            UpdatedDate: null,
            UpdatedById: null,
            TenantId: null
          }
          objDetail.push(detailVendorQuote);
        });
        let objVendorQuote = {
          SuggestedSupplierQuoteList: {
            SuggestedSupplierQuotes: obj,
            SuggestedSupplierQuoteDetailList: objDetail
          }
        }
        paramRequest.push(objVendorQuote)

        let result: any = await this.vendorService.createVendorQuoteAsyn(objDetail, obj, this.auth.UserId, index);
        if (result.statusCode == 200) {
          if (check) {
            this.router.navigate(['/vendor/list-vendor-quote']);
          } 
          else {
            let msg1 = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo báo giá nhà cung cấp thành công' };
            this.showMessage(msg1);
            this.ref.close();
          }
        }                    
      });
    }
  }
  /*End*/

  /*Reset form chi phí khác*/
  resetOtherCostsForm() {
  }
  /*End*/

  /*Reset form sản phẩm dịch vụ*/
  resetProductForm() {
    this.listAttrProduct = [];  //list group thuộc tính của sản phẩm
    //Gán giá trị lại cho các biến lưu số thành tiền
    this.amountProduct = 0;
    this.amountVatProduct = 0;
    this.amountDiscountProduct = 0;
  }
  /*End*/

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
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
