import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { SaleBiddingService } from '../../services/sale-bidding.service';
import { MessageService } from 'primeng/api';

import { CostsQuocte } from '../../models/costs-quocte.model';
import { SaleBiddingDetailProductAttribute } from '../../models/product-attribute-category-value.model';
import { VendorService } from '../../../vendor/services/vendor.service';


interface ResultDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  costsQuocteModel: CostsQuocte,
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

interface SuggestedSupplierCostsQuocte {
  SuggestedSupplierCostsQuocteId: string,
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
  selector: 'app-sale-bidding-vendor-dialog',
  templateUrl: './sale-bidding-vendor-dialog.component.html',
  styleUrls: ['./sale-bidding-vendor-dialog.component.css']
})
export class SaleBiddingVendorDialogComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  /*Các biến nhận giá trị trả về*/
  unitMoneyLabel: string = 'VND';
  unitMoneyOCLabel: string = 'VND';
  listVendor: Array<Vendor> = [];
  //   listProduct: Array<Product> = [];
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
  saleBiddingId: string;
  selectedColumns: any[];
  selectedQuoteVendor: any[] = [];
  listObjectAttrNameProduct: Array<ObjectAttrNameProduct> = [];
  listObjectAttrValueProduct: Array<ObjectAttrValueProduct> = [];
  listAttrProduct: Array<GroupAttrProduct> = [];
  listCostsQuocteModel: Array<CostsQuocte> = [];
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
    private saleBiddingService: SaleBiddingService,
    private vendorService: VendorService,
    private router: Router,
    private messageService: MessageService
  ) {
    //Nếu là sửa
    this.listCostsQuocteModel = this.config.data.listCostsQuocteModel;
    this.saleBiddingId = this.config.data.saleBiddingId;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }
  trimspace() {
    this.listData.forEach(element => {
      element.description = element.description == null?"":element.description.trim();
      if (element.Quantity == null || element.Quantity == undefined || element.Quantity == '') {
        element.Quantity = 0;
      }
    });
  }
  ngOnInit() {
    this.loading = true;
    this.getMasterData();
    // this.quoteService.getDataQuoteAddEditProductDialog().subscribe(response => {
    //   let result: any = response;
    //   this.loading = false;
    //   if (result.statusCode == 200) {
    //     this.listProduct = result.listProduct;
    //     this.listUnitProduct = result.listUnitProduct;

    //   } else {
    //     let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
    //     this.showMessage(msg);
    //   }
    // });
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

    this.listData = this.listCostsQuocteModel;
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
      { field: 'incurredUnit', header: 'Mã sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
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
      costsQuocteModel: null
    };
    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save(check) {
    let countVendorNull = 0;
    let countQuantity = 0;
    if (this.selectedQuoteVendor.length == 0) {
      let msg1 = { severity: 'error', summary: 'Thông báo:', detail: 'Bạn chưa chọn sản phẩm dịch vụ nào' };
      this.showMessage(msg1);
    }
    else {
      this.selectedQuoteVendor.forEach(item => {
        if (item.vendor === undefined || item.vendor === null) {
          countVendorNull++;
        }
        if (item.quantity == 0) {
          countQuantity++;
        }
      });
      if (countVendorNull !== 0) {
        let msg1 = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn nhà cung cấp' };
        this.showMessage(msg1);
      }
      else if (countQuantity !== 0) {
        let msg1 = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa nhập số lượng' };
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
            ObjectType: "SALEBIDDING",
            ObjectId: this.saleBiddingId,
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
            let detailVendorQuote: SuggestedSupplierCostsQuocte = {
              SuggestedSupplierCostsQuocteId: this.emptyGuid,
              SuggestedSupplierQuoteId: this.emptyGuid,
              ProductId: detail.ProductId,
              Quantity: detail.Quantity,
              Note: detail.Description,
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
              SuggestedSupplierCostsQuocteList: objDetail
            }
          }
          paramRequest.push(objVendorQuote)
          let result: any = await this.vendorService.createVendorQuoteAsyn(objDetail, obj, this.auth.UserId, index);
          if (result.statusCode == 200) {
            this.ref.close();
            if (check) {
              this.router.navigate(['/vendor/list-vendor-quote']);
            }
            let msg1 = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo báo giá nhà cung cấp thành công' };
            this.showMessage(msg1);
          }
          else {
            let msg1 = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg1);
          }
        });
      }
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
