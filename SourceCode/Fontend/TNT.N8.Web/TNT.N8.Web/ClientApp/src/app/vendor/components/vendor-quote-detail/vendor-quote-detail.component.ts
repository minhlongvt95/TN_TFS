import { Component, ElementRef, HostListener, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { ConfirmationService, SortEvent } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';

import { ProductService } from '../../../product/services/product.service';
import { SuggestedSupplierQuotesDetailModel, SuggestedSupplierQuotesModel } from '../../models/vendor.model';
import { VendorService } from '../../services/vendor.service';

import { ActivatedRoute, Router } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';

import * as $ from 'jquery';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { Workbook } from 'exceljs';
import { saveAs } from "file-saver";
import { DatePipe, DecimalPipe } from '@angular/common';
import { format } from 'url';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
import { SendMailVendorQuoteDialogComponent } from '../send-mail-vendor-quote-dialog/send-mail-vendor-quote-dialog.component';
pdfMake.vfs = pdfFonts.pdfMake.vfs;


interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
}

interface Vendor {
  vendorId: string;
  vendorCode: string;
  vendorName: string;
  vendorCodeName: string;
}

interface Employee {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  employeeCodeName: string;
}

interface Product {
  productId: string;
  productCode: string;
  productName: string;
  productCodeName: string;
}

class NoteErr {
  public code: string;
  public name: string;
}

interface ProcurementRequest {
  procurementRequestId: string;
  procurementCode: string;
  procurementContent: string;
  requestEmployeeId: string;
  employeePhone: string;
  createdById: string;
  createdDate: string;
  listProcurementRequestItem: Array<ProcurementRequestItem>;
}

interface ProcurementRequestItem {
  procurementRequestItemId: string;
  productId: string;
  productName: string;
  productCode: string;
  vendorId: string;
  vendorName: string;
  quantity: number;
  unit: string;
  unitPrice: number;
  unitName: string;
  procurementRequestId: string;
  procurementPlanId: string;
  procurementPlanCode: string;
  createdById: string;
  createdDate: string;
  currencyUnit: string;
  exchangeRate: number;
  productUnitId: string;
  quantityApproval: number;
}

interface productVendorMapping {
  productVendorMappingId: string;
  productId: string;
  vendorId: string;
}

interface InforExportExcel {
  companyName: string,
  address: string,
  phone: string,
  website: string,
  email: string,
  textTotalMoney: string
}

class suggestedSupplierQuotesDetailModel {
  suggestedSupplierQuoteDetailId: string;
  suggestedSupplierQuoteId: string;
  productId: string;
  productCode: string;
  productName: string;
  quantity: string;
  note: string;
  active: boolean;
  procurenmentRequest: string;
  vendorId: string;
  productUnitName: string;

  status: boolean;
  public listNoteErr: Array<string>;
  constructor() {
    this.listNoteErr = [];
  }
}

@Component({
  selector: 'app-vendor-quote-detail',
  templateUrl: './vendor-quote-detail.component.html',
  styleUrls: ['./vendor-quote-detail.component.css']
})
export class VendorQuoteDetailComponent implements OnInit {

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  //Biến điều kiện
  loading: boolean = false;
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  minYear: number = 2000;
  currentYear: number = (new Date()).getFullYear();
  maxEndDate: Date = new Date();
  isCreate: boolean = true;
  cols: any[];
  selectedColumns: any[];
  @ViewChild('myTable') myTable: Table;

  /*BIẾN LƯU GIÁ TRỊ TRẢ VỀ*/
  listVendor: Array<Vendor> = [];
  listProduct: Array<Product> = [];
  listProcurementRequest: Array<ProcurementRequest> = [];
  listProcurementRequestItem: Array<ProcurementRequestItem> = [];
  listSuggestedSupplierQuotesDetail: Array<suggestedSupplierQuotesDetailModel> = [];
  listEmployee: Array<Employee> = [];
  listStatus: Array<Category> = [];
  listProductVendorMapping: Array<productVendorMapping> = [];
  viewModeCode: string = null;
  statusName: string = '';
  saleBiddingCode: string = '';
  quoteCode: string = '';
  suggestedSupplierQuote: string = '';
  /*END*/

  /*MODEL*/
  suggestedSupplierQuoteRequestModel = new SuggestedSupplierQuotesModel();
  /*END*/
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionDeleteProduct: boolean = true;
  //define note
  listNoteErr: Array<NoteErr> = [
    { code: "quantity_error", name: "Số lượng không được để trống" },
  ];

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;

  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới
  /*FORM DIALOG*/
  SupplierQuoteForm: FormGroup;
  vendorControl: FormControl;
  picControl: FormControl;
  recommendedDateControl: FormControl;
  quoteTemDateControl: FormControl;
  procurementRequestControl: FormControl;
  descriptionControl: FormControl;

  productControl: FormControl;
  /*END*/
  inforExportExcel: InforExportExcel = {
    companyName: '',
    address: '',
    phone: '',
    website: '',
    email: '',
    textTotalMoney: ''
  }


  fixed: boolean = false;
  withFiexd: string = "";
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 110) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  constructor(
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private productService: ProductService,
    private vendorService: VendorService,
    private el: ElementRef,
    private router: Router,
    private renderer: Renderer2,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe,
    private dialogService: DialogService,
  ) {
    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        }
      }
    });
  }

  async ngOnInit() {
    this.route.params.subscribe(params => {
      this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId = params['SuggestedSupplierQuoteId'];
      let resource = "buy/vendor/vendor-quote-detail/";
      let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
      if (permission.status == false) {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
        setTimeout(() => {
          this.showMessage(msg);
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 1000);
        }, 0);
      } else {

        let listCurrentActionResource = permission.listCurrentActionResource;
        if (listCurrentActionResource.indexOf("edit") == -1) {
          this.actionEdit = false;
        }
        if (listCurrentActionResource.indexOf("add") == -1) {
          this.actionAdd = false;
        } else {
          this.actionAdd = true;
        }
        if (listCurrentActionResource.indexOf("delete") == -1) {
          this.actionDelete = false;
        }
      }
    });
    this.setTable();
    this.setForm();
    await this.getMasterData();
  }

  setForm() {
    this.vendorControl = new FormControl(null, [Validators.required]);
    this.picControl = new FormControl(null);
    this.recommendedDateControl = new FormControl(new Date(), [Validators.required]);
    this.quoteTemDateControl = new FormControl(new Date(), [Validators.required]);
    this.procurementRequestControl = new FormControl(null);
    this.descriptionControl = new FormControl('');
    this.productControl = new FormControl(null);

    this.SupplierQuoteForm = new FormGroup({
      vendorControl: this.vendorControl,
      picControl: this.picControl,
      recommendedDateControl: this.recommendedDateControl,
      quoteTemDateControl: this.quoteTemDateControl,
      procurementRequestControl: this.procurementRequestControl,
      descriptionControl: this.descriptionControl,

      productControl: this.productControl,
    });
  }
  setTable() {

    this.cols = [
      { field: 'productCode', header: 'Mã hàng', textAlign: 'left', color: '#f44336', width: '200px' },
      { field: 'productName', header: 'Tên hàng', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', textAlign: 'right', color: '#f44336', width: '120px' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', color: '#f44336' },
      { field: 'noteErr', header: 'Thông báo', textAlign: 'left', color: '#f44336' }
    ];
    this.selectedColumns = this.cols.filter(c => c.field == 'productCode' || c.field == 'productName' || c.field == 'note' || c.field == 'quantity'
      || c.field == "noteErr");
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.vendorService.getMasterDataCreateSuggestedSupplierQuoteAsync(this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listSuggestedSupplierQuotesDetail = [];
      this.listVendor = result.listVendor;
      this.listProduct = result.listProduct;
      this.listProcurementRequest = result.listProcurementRequest;
      this.listProcurementRequestItem = result.listProcurementRequestItem;
      this.listEmployee = result.listEmployee;
      this.suggestedSupplierQuoteRequestModel = result.suggestedSupplierQuotes;
      this.listStatus = result.listStatus;
      this.inforExportExcel = result.inforExportExcel;
      this.listSuggestedSupplierQuotesDetail = result.suggestedSupplierQuotes.listVendorQuoteDetail;
      this.listProductVendorMapping = result.listProductVendorMapping;
      let status = this.listStatus.find(c => c.categoryId == this.suggestedSupplierQuoteRequestModel.statusId);
      this.viewModeCode = status != null ? status.categoryCode : '';
      this.statusName = status != null ? status.categoryName : '';

      this.suggestedSupplierQuote = this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuote || '';
      this.saleBiddingCode = this.suggestedSupplierQuoteRequestModel.saleBiddingCode || '';
      this.quoteCode = this.suggestedSupplierQuoteRequestModel.quoteCode || '';
      this.listEmployee.forEach(item => {
        item.employeeCodeName = item.employeeCode + ' - ' + item.employeeName;
      });
      this.listVendor.forEach(item => {
        item.vendorCodeName = item.vendorCode + ' - ' + item.vendorName;
      });
      this.listProduct.forEach(item => {
        item.productCodeName = item.productCode + ' - ' + item.productName;
      });
      this.setAuthorization();
      this.setDefault();
      this.exportPdf(null);
    }
  }

  setDefault() {
    this.recommendedDateControl.setValue(new Date(this.suggestedSupplierQuoteRequestModel.recommendedDate));
    this.quoteTemDateControl.setValue(new Date(this.suggestedSupplierQuoteRequestModel.quoteTermDate));

    let employee = this.listEmployee.find(c => c.employeeId == this.suggestedSupplierQuoteRequestModel.personInChargeId);
    this.picControl.setValue(employee);

    let procurement = this.listProcurementRequest.find(c => c.procurementRequestId == this.suggestedSupplierQuoteRequestModel.procurementRequestId);
    this.procurementRequestControl.setValue(procurement);

    let vendor = this.listVendor.find(c => c.vendorId == this.suggestedSupplierQuoteRequestModel.vendorId);
    this.vendorControl.setValue(vendor);
    this.descriptionControl.setValue(this.suggestedSupplierQuoteRequestModel.note);
  }

  changeProduct(event: any) {
    if (event.value == null) return;

    let model = this.mappingProductToSupplierQuoteDetailModel(event.value);
    this.listSuggestedSupplierQuotesDetail.push(model);
  }

  mappingProductToSupplierQuoteDetailModel(product: any): suggestedSupplierQuotesDetailModel {
    let model = new suggestedSupplierQuotesDetailModel();
    model.productId = product.productId;
    model.productCode = product.productCode;
    model.productName = product.productName;
    model.quantity = '';
    model.note = '';
    return model;
  }

  mappingProcurementRequestItemToSupplierItem(array: Array<ProcurementRequestItem>): suggestedSupplierQuotesDetailModel[] {
    let listSuggestedSupplierQuotesDetailTemp = new Array<suggestedSupplierQuotesDetailModel>();
    if (array != null) {
      array.forEach(item => {
        let model = new suggestedSupplierQuotesDetailModel();
        model.productId = item.productId;
        model.productCode = item.productCode;
        model.productName = item.productName;
        model.quantity = item.quantity.toString();
        model.note = '';

        listSuggestedSupplierQuotesDetailTemp.push(model);
      });
    }

    return listSuggestedSupplierQuotesDetailTemp;
  }

  mappingSuggestedSupplierQuoteDetailModel(array: Array<suggestedSupplierQuotesDetailModel>): SuggestedSupplierQuotesDetailModel[] {
    let listSuggestedSupplierQuotesDetailTemp = new Array<SuggestedSupplierQuotesDetailModel>();
    if (array != null) {
      array.forEach(item => {
        let model = new SuggestedSupplierQuotesDetailModel();
        model.productId = item.productId;
        model.productCode = item.productCode;
        model.productName = item.productName;
        model.quantity = parseFloat(item.quantity.toString().replace(/,/g, ''));
        model.note = item.note;
        model.createdById = this.auth.UserId;
        model.active = true;
        listSuggestedSupplierQuotesDetailTemp.push(model);
      });
    }

    return listSuggestedSupplierQuotesDetailTemp;
  }

  mappingForm(): SuggestedSupplierQuotesModel {
    let model = new SuggestedSupplierQuotesModel();

    model.suggestedSupplierQuoteId = this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId;
    let vendor = this.vendorControl.value;
    model.vendorId = vendor == null ? model.vendorId : vendor.vendorId;
    let pic = this.picControl.value;
    model.personInChargeId = pic == null ? null : pic.employeeId;
    model.recommendedDate = convertToUTCTime(this.recommendedDateControl.value);
    model.quoteTermDate = convertToUTCTime(this.quoteTemDateControl.value);
    model.note = this.descriptionControl.value;
    let procurementRequest = this.procurementRequestControl.value;
    model.procurementRequestId = procurementRequest == null ? model.procurementRequestId : procurementRequest.procurementRequestId;

    return model;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  createVendorQuote() {
    if (!this.SupplierQuoteForm.valid) {
      Object.keys(this.SupplierQuoteForm.controls).forEach(key => {
        if (!this.SupplierQuoteForm.controls[key].valid) {
          this.SupplierQuoteForm.controls[key].markAsTouched();
        }
      });

      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.SupplierQuoteForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else {

      if (this.listSuggestedSupplierQuotesDetail.length == 0) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: "Danh sản phẩm báo giá NCC không thể trống!" };
        this.showMessage(msg);
        return;
      }

      let check_quantity = this.checkStatusQuantity();
      if (check_quantity === false) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: "Danh sách sản phẩm báo giá NCC không hợp lệ!" };
        this.showMessage(msg);
        return;
      }
      let suggestedSupplierQuoteModel = this.mappingForm();
      let suggestedSupplierQuoteDetail = this.mappingSuggestedSupplierQuoteDetailModel(this.listSuggestedSupplierQuotesDetail);
      let isCheckProductOfVendor: boolean = true;
      let listProductIdOfVendor = this.listProductVendorMapping.filter(c => c.vendorId == suggestedSupplierQuoteModel.vendorId).map(c => c.productId);
      suggestedSupplierQuoteDetail.forEach(item => {
        if (!listProductIdOfVendor.includes(item.productId)) {
          isCheckProductOfVendor = false;
        }
      });

      if (!isCheckProductOfVendor) {
        this.confirmationService.confirm({
          message: 'Có sản phẩm không thuộc nhà cung cấp đã chọn. Bạn có chắc muốn tiếp tục?',
          key: "popup",
          accept: () => {
            this.loading = true;
            this.vendorService.createOrUpdateSuggestedSupplierQuote(suggestedSupplierQuoteModel, suggestedSupplierQuoteDetail, this.auth.UserId).subscribe(reponse => {
              let result: any = reponse;
              this.loading = false;
              if (result.statusCode == 200) {
                let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
                this.showMessage(mgs);
                if (this.emitStatusChangeForm) {
                  this.emitStatusChangeForm.unsubscribe();
                  this.isInvalidForm = false; //Ẩn icon-warning-active
                }
                this.awaitResult = false;
              } else {
                let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                this.showMessage(mgs);
              }
            });
          }
        });
      } else {
        this.loading = true;
        this.vendorService.createOrUpdateSuggestedSupplierQuote(suggestedSupplierQuoteModel, suggestedSupplierQuoteDetail, this.auth.UserId).subscribe(reponse => {
          let result: any = reponse;
          this.loading = false;
          if (result.statusCode == 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            }
            this.awaitResult = false;
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        });
      }
    }
  }

  resetForm() {
    this.vendorControl.setValue(null);
    let employee = this.listEmployee.find(c => c.employeeId == this.auth.EmployeeId);
    this.picControl.setValue(employee);
    this.procurementRequestControl.setValue(null);
    this.recommendedDateControl.setValue(new Date());
    this.quoteTemDateControl.setValue(new Date());
    this.descriptionControl.setValue('');
    this.SupplierQuoteForm.updateValueAndValidity();

    this.listSuggestedSupplierQuotesDetail = [];
  }

  checkDifferentProductVendor(): boolean {
    let check: boolean = false;
    for (let i = 0; i < this.listSuggestedSupplierQuotesDetail.length; i++) {
      if (i == this.listSuggestedSupplierQuotesDetail.length) break;
      if (this.listSuggestedSupplierQuotesDetail[i].vendorId != this.listSuggestedSupplierQuotesDetail[i + 1].vendorId) {
        check = true;
        break;
      }
    }
    return check;
  }

  del_product(rowData: suggestedSupplierQuotesDetailModel) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      key: "popup",
      accept: () => {
        let index = this.listSuggestedSupplierQuotesDetail.indexOf(rowData);
        this.listSuggestedSupplierQuotesDetail.splice(index, 1);
      }
    });
  }

  deleteSuggestedSupplierQuoteRequest() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      key: 'popup',
      accept: () => {
        this.loading = true;
        this.vendorService.deleteSuggestedSupplierQuoteRequest(this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId, this.auth.UserId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 200) {
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            this.router.navigate(['/vendor/list-vendor-quote']);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
          }
        }, error => { });
      }
    });
  }


  checkStatusQuantity(): boolean {
    let dataStatus = true;
    this.listSuggestedSupplierQuotesDetail.forEach(rowData => {
      //reset status
      rowData.status = true;
      rowData.listNoteErr = [];
      //check empty string
      if (rowData.quantity.toString() === '') {
        rowData.status = false;
        rowData.listNoteErr = [...rowData.listNoteErr, this.listNoteErr.find(e => e.code == 'quantity_error').name];
      }
      //check dataStatus
      if (rowData.status === false) dataStatus = false;
    });
    return dataStatus;
  }

  changeProcurementRequest(event: any) {
    if (event.value == null) {
      this.picControl.setValue(null);
      this.vendorControl.setValue(null);
      this.listSuggestedSupplierQuotesDetail = [];
      return;
    }
    let personInCharge = this.listEmployee.find(c => c.employeeId == event.value.requestEmployeeId);
    this.picControl.setValue(personInCharge);

    let listProcurementRequestItem = this.listProcurementRequestItem.filter(c => c.procurementRequestId == event.value.procurementRequestId);
    this.listSuggestedSupplierQuotesDetail = this.mappingProcurementRequestItemToSupplierItem(listProcurementRequestItem);
  }

  dateFieldFormat: string = 'DD/MM/YYYY';

  sortColumnInList(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'createdDate') {
        const date1 = moment(value1, this.dateFieldFormat);
        const date2 = moment(value2, this.dateFieldFormat);

        let result: number = -1;
        if (moment(date2).isBefore(date1, 'day')) { result = 1; }

        return result * event.order;
      }
      /**End */

      let result = null;

      if (value1 == null && value2 != null)
        result = -1;
      else if (value1 != null && value2 == null)
        result = 1;
      else if (value1 == null && value2 == null)
        result = 0;
      else if (typeof value1 === 'string' && typeof value2 === 'string')
        result = value1.localeCompare(value2);
      else
        result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;

      return (event.order * result);
    });
  }

  setAuthorization() {
    switch (this.viewModeCode) {
      case "MOI":
        // Trạng thái mới
        this.SupplierQuoteForm.enable();
        this.actionDelete = true;
        break;
      case "DNG":
        // Trạng thái đề nghị
        this.SupplierQuoteForm.disable();
        this.actionDelete = false;
        break;
      case "HUY":
        //Trạng thái hủy
        this.SupplierQuoteForm.disable();
        this.actionDelete = false;
        break;
      default:
        break;
    }
  }

  async setViewMode(code: string) {
    let viewModeCode = code;
    let status: Category = this.listStatus.find(e => e.categoryCode == viewModeCode);
    let statusId = status ? status.categoryId : null;

    switch (viewModeCode) {
      case "MOI":
        //Trạng thái mới
        this.loading = true;
        let draftResult: any = await this.vendorService.changeStatusVendorQuoteAsync(this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId, statusId, this.auth.UserId);
        if (draftResult.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: draftResult.messageCode };
          this.showMessage(msg);
          this.viewModeCode = viewModeCode;
          this.statusName = status.categoryName;
          this.setAuthorization();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: draftResult.messageCode };
          this.showMessage(msg);
        }
        this.loading = false;
        break;
      case "DNG":
        //Trạng thái đề nghị
        //chuyen trang thai lead thanh xac nhan
        this.loading = true;
        let suggestionResult: any = await this.vendorService.changeStatusVendorQuoteAsync(this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId, statusId, this.auth.UserId);
        this.loading = false;
        if (suggestionResult.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: suggestionResult.messageCode };
          this.showMessage(msg);
          this.viewModeCode = viewModeCode;
          this.statusName = status.categoryName;
          await this.getMasterData();
          this.setAuthorization();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: suggestionResult.messageCode };
          this.showMessage(msg);
        }
        break;
      case "HUY":
        this.loading = true;
        let cancelResult: any = await this.vendorService.changeStatusVendorQuoteAsync(this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId, statusId, this.auth.UserId);
        this.loading = false;
        if (cancelResult.statusCode == 200) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: cancelResult.messageCode };
          this.showMessage(msg);
          this.viewModeCode = viewModeCode;
          this.statusName = status.categoryName;
          this.setAuthorization();
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: cancelResult.messageCode };
          this.showMessage(msg);
        }
        break;
      default:
        break;
    }
  }
  goToSaleBidding() {
    if (this.suggestedSupplierQuoteRequestModel.objectType == "SALEBIDDING") {
      this.router.navigate(['/sale-bidding/detail', { saleBiddingId: this.suggestedSupplierQuoteRequestModel.objectId }]);
    }
  }

  goToQuote() {
    if (this.suggestedSupplierQuoteRequestModel.objectType == "QUOTE") {
      this.router.navigate(['/customer/quote-detail', { quoteId: this.suggestedSupplierQuoteRequestModel.objectId }]);
    }
  }

  exportExcel() {
    let employeePic = this.listEmployee.find(c => c.employeeId == this.auth.EmployeeId);
    let employee = this.listEmployee.find(c => c.employeeId == this.suggestedSupplierQuoteRequestModel.personInChargeId);
    let vendor = this.listVendor.find(c => c.vendorId == this.suggestedSupplierQuoteRequestModel.vendorId);
    var dateNow = new Date();

    let vendorQuoteNameExport = convertStringCusName(vendor.vendorName);

    let imgBase64 = this.getBase64Logo();
    let dateUTC = new Date();

    // getMonth() trả về index trong mảng nên cần cộng thêm 1
    let title = 'Đề nghị báo giá' + "_" + dateUTC.getUTCFullYear() + (dateUTC.getMonth() + 1) + dateUTC.getDate();
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(title, { views: [{ showGridLines: false }] });
    worksheet.pageSetup.margins = {
      left: 0.25, right: 0.25,
      top: 0.75, bottom: 0.75,
      header: 0.3, footer: 0.3
    };
    worksheet.pageSetup.paperSize = 9;  //A4 : 9

    /* Image */
    var imgLogo = workbook.addImage({
      base64: imgBase64,
      extension: 'png',
    });

    worksheet.addImage(imgLogo, {
      tl: { col: 0, row: 0 },
      ext: { width: 155, height: 100 }
    });

    let dataRow1 = [];
    dataRow1[3] = this.inforExportExcel.companyName.toUpperCase();  //Tên công ty
    let row1 = worksheet.addRow(dataRow1);
    row1.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`C${row1.number}:H${row1.number}`);
    row1.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow2 = [];
    dataRow2[3] = 'Địa chỉ: ' + this.inforExportExcel.address;  //Địa chỉ
    let row2 = worksheet.addRow(dataRow2);
    row2.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row2.number}:H${row2.number}`);
    row2.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow3 = [];
    dataRow3[3] = 'Điện thoại: ' + this.inforExportExcel.phone;  //Số điện thoại
    let row3 = worksheet.addRow(dataRow3);
    row3.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row3.number}:H${row3.number}`);
    row3.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow4 = [];
    dataRow4[3] = 'Email: ' + this.inforExportExcel.email;
    let row4 = worksheet.addRow(dataRow4);
    row4.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row4.number}:H${row4.number}`);
    row4.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow5 = [];
    dataRow5[3] = 'Website dịch vụ: ' + this.inforExportExcel.website;  //Địa chỉ website
    let row5 = worksheet.addRow(dataRow5);
    row5.font = { name: 'Times New Roman', size: 11, color: { argb: '003366' } };
    worksheet.mergeCells(`C${row5.number}:H${row5.number}`);
    row5.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);
    worksheet.addRow([]);
    worksheet.addRow([]);

    let dataRow9 = [];
    dataRow9[3] = 'ĐỀ NGHỊ BÁO GIÁ';
    let row9 = worksheet.addRow(dataRow9);
    row9.font = { name: 'Times New Roman', size: 16, bold: true };
    row9.height = 40;
    worksheet.mergeCells(`C${row9.number}:F${row9.number}`);
    row9.alignment = { vertical: 'middle', horizontal: 'center' };

    let dataRow10 = [];
    dataRow10[3] = 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear();
    let row10 = worksheet.addRow(dataRow10);
    row10.font = { name: 'Times New Roman', size: 10, bold: false };
    worksheet.mergeCells(`C${row10.number}:F${row10.number}`);
    row10.alignment = { vertical: 'middle', horizontal: 'center' };

    let dataRow11 = [];
    dataRow11[1] = 'Số phiếu:  ' + this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuote;
    let row11 = worksheet.addRow(dataRow11);
    row11.font = { name: 'Times New Roman', size: 10, bold: false };
    worksheet.mergeCells(`A${row11.number}:H${row11.number}`);
    row11.alignment = { vertical: 'middle', horizontal: 'right' };

    let dataRow12 = [];
    dataRow12[2] = 'Người đề nghị: ' + employee.employeeName || '';
    let row12 = worksheet.addRow(dataRow12);
    row12.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`B${row12.number}:H${row12.number}`);
    row12.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow13 = [];
    dataRow13[2] = 'Đơn vị: ' + vendor.vendorCode || '' + '-' + vendor.vendorName || '';
    let row13 = worksheet.addRow(dataRow13);
    row13.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`B${row13.number}:H${row13.number}`);
    row13.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow14 = [];
    dataRow14[2] = 'Diễn giải: ' + this.suggestedSupplierQuoteRequestModel.note;
    let row14 = worksheet.addRow(dataRow14);
    row14.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`B${row14.number}:H${row14.number}`);
    row14.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow15 = [];
    dataRow15[2] = 'Hạn báo giá: ' + this.datePipe.transform(this.suggestedSupplierQuoteRequestModel.quoteTermDate, 'dd/MM/yyyy');
    let row15 = worksheet.addRow(dataRow15);
    row15.font = { name: 'Times New Roman', size: 10, bold: true };
    worksheet.mergeCells(`B${row15.number}:H${row15.number}`);
    row15.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    let dataRow16 = [];
    dataRow16[2] = 'Kính gửi quý công ty, Chúng tôi đang quan tâm một số sản phẩm, dịch vụ của Quý công ty như bảng dưới đây. Kính mong quý công ty gửi lại báo giá cho các sản phẩm/ dịch vụ này cho chúng tôi trước ngày hạn báo giá. Xin chân thành cảm ơn!';
    let row16 = worksheet.addRow(dataRow16);
    row16.font = { name: 'Cambria', size: 10, italic: true };
    row16.height = 25;
    worksheet.mergeCells(`B${row16.number}:H${row16.number}`);
    row16.alignment = { vertical: 'top', horizontal: 'left', wrapText: true };

    worksheet.addRow([]);
    /* Header row */
    let dataHeaderRow = ['STT', 'Mã hàng', 'Tên hàng', 'Đơn vị tính', 'Số lượng', 'Ghi chú', '', ''];
    let headerRow = worksheet.addRow(dataHeaderRow);
    headerRow.font = { name: 'Times New Roman', size: 10, bold: true };
    dataHeaderRow.forEach((item, index) => {
      if (index == 5) {
        worksheet.mergeCells(`F${headerRow.number}:H${headerRow.number}`)
      }
      headerRow.getCell(index + 1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      headerRow.getCell(index + 1).alignment = { vertical: 'middle', horizontal: 'center', wrapText: true };
      headerRow.getCell(index + 1).fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: 'E3EDF8' }
      };
    });
    headerRow.height = 40;

    /* Data table */
    let data: Array<any> = []; //[1, 'Dịch vụ CNTT', 'Gói', '2', '6.000.000', '12.000.000']

    for (var index = 0; index < this.listSuggestedSupplierQuotesDetail.length; ++index) {
      let row: Array<any> = [];
      row[0] = index + 1;
      row[1] = this.listSuggestedSupplierQuotesDetail[index].productCode;
      row[2] = this.listSuggestedSupplierQuotesDetail[index].productName;
      row[3] = this.listSuggestedSupplierQuotesDetail[index].productUnitName;
      row[4] = this.decimalPipe.transform((this.listSuggestedSupplierQuotesDetail[index].quantity).toString());
      row[5] = this.listSuggestedSupplierQuotesDetail[index].note;

      data.push(row);
    }

    data.forEach((el, index, array) => {
      let row = worksheet.addRow(el);
      row.font = { name: 'Times New Roman', size: 11 };

      row.getCell(1).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(1).alignment = { vertical: 'middle', horizontal: 'center' };

      row.getCell(2).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(2).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(3).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(3).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(4).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(4).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };

      row.getCell(5).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(5).alignment = { vertical: 'middle', horizontal: 'right', wrapText: true };

      row.getCell(6).border = { left: { style: "thin" }, top: { style: "thin" }, bottom: { style: "thin" }, right: { style: "thin" } };
      row.getCell(6).alignment = { vertical: 'middle', horizontal: 'left', wrapText: true };
      worksheet.mergeCells(`F${row.number}:H${row.number}`);

    });

    worksheet.addRow([]);
    worksheet.addRow([]);

    let dataFooter2 = [];
    let date = (new Date()).getDate();
    let month = (new Date()).getMonth() + 1;
    let fullYear = (new Date()).getFullYear();
    dataFooter2[6] = 'Hà Nội, Ngày ' + date + ' tháng ' + month + ' năm ' + fullYear;
    let footer2 = worksheet.addRow(dataFooter2);
    footer2.font = { name: 'Times New Roman', size: 11, bold: false };
    worksheet.mergeCells(`F${footer2.number}:H${footer2.number}`);
    footer2.alignment = { vertical: 'top', horizontal: 'center' };

    let dataFooter3 = [];
    dataFooter3[2] = "NGƯỜI ĐỀ NGHỊ"
    dataFooter3[6] = "PHỤ TRÁCH";
    let footer3 = worksheet.addRow(dataFooter3);
    footer3.font = { name: 'Times New Roman', size: 11, bold: true };
    worksheet.mergeCells(`F${footer3.number}:H${footer3.number}`);
    worksheet.mergeCells(`B${footer3.number}:C${footer3.number}`)
    footer3.alignment = { vertical: 'top', horizontal: 'center' };

    /* fix with for column */
    worksheet.getColumn(1).width = 5;
    worksheet.getColumn(2).width = 20;
    worksheet.getColumn(3).width = 35;
    worksheet.getColumn(4).width = 15;
    worksheet.getColumn(5).width = 15;
    worksheet.getColumn(6).width = 20;

    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      saveAs.saveAs(blob, title);
    })
  }

  exportPdf(type: string) {
    let employeePic = this.listEmployee.find(c => c.employeeId == this.auth.EmployeeId);
    let employee = this.listEmployee.find(c => c.employeeId == this.suggestedSupplierQuoteRequestModel.personInChargeId);
    let vendor = this.listVendor.find(c => c.vendorId == this.suggestedSupplierQuoteRequestModel.vendorId);
    let dateNow = new Date();
    let imgBase64 = this.getBase64Logo();

    let documentDefinition: any = {
      pageSize: 'A4',
      pageMargins: [20, 20, 20, 20],
      content: [
        {
          table: {
            widths: ['*', '400'],
            body: [
              [
                {
                  stack: [
                    {
                      image: imgBase64, width: 100, height: 60
                    }
                  ],
                },
                {
                  stack: [
                    {
                      text: "" + this.inforExportExcel.companyName.toUpperCase(),
                      style: {
                        fontSize: 10,
                        bold: true,
                      },
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Địa chỉ: ' + this.inforExportExcel.address,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Điện thoại: ' + this.inforExportExcel.phone,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Email: ' + this.inforExportExcel.email,
                      style: 'timer',
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Website dịch vụ: ' + this.inforExportExcel.website,
                      style: 'timer',
                      alignment: 'left'
                    },
                  ],
                }
              ],
            ]
          },
          layout: {
            defaultBorder: false
          },
          lineHeight: 0.75
        },
        {
          text: '',
          margin: [0, 15, 0, 15]
        },
        {
          text: 'ĐỀ NGHỊ BÁO GIÁ',
          style: 'header',
          alignment: 'center'
        },
        {
          text: 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
          fontSize: 10,
          alignment: 'center'
        },
        {
          text: 'Số phiếu:  ' + this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuote,
          fontSize: 10,
          alignment: 'right',
          margin: [0, 10, 0, 10]
        },
        {
          text: 'Người đề nghị: ' + employee.employeeName || '',
          style: 'default',
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Đơn vị: ' + vendor.vendorCode || '' + '-' + vendor.vendorName || '',
          style: 'default',
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Diễn giải: ' + this.suggestedSupplierQuoteRequestModel.note,
          style: 'default',
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Hạn báo giá: ' + this.datePipe.transform(this.suggestedSupplierQuoteRequestModel.quoteTermDate, 'dd/MM/yyyy'),
          style: 'default',
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Kính gửi quý công ty, Chúng tôi đang quan tâm một số sản phẩm, dịch vụ của Quý công ty như bảng dưới đây. Kính mong quý công ty gửi lại báo giá cho các sản phẩm/ dịch vụ này cho chúng tôi trước ngày hạn báo giá. Xin chân thành cảm ơn!',
          style: {
            fontSize: 10,
            italics: true
          },
          alignment: 'left',
          margin: [0, 5, 0, 2]
        },
        {
          style: 'table',
          table: {
            widths: [20, 60, 165, 49, 49, 160, 0],
            headerRows: 1,
            dontBreakRows: true,
            body: [
              [
                { text: 'STT', style: 'tableHeader', alignment: 'center' },
                { text: 'Mã hàng', style: 'tableHeader', alignment: 'center' },
                { text: 'Tên hàng', style: 'tableHeader', alignment: 'center' },
                { text: 'Số lượng', style: 'tableHeader', alignment: 'center' },
                { text: 'Đơn vị', style: 'tableHeader', alignment: 'center' },
                { text: 'Ghi chú', style: 'tableHeader', alignment: 'center' },
              ],
            ],
          },
          layout: {
            defaultBorder: true,
            paddingTop: function (i, node) { return 2; },
            paddingBottom: function (i, node) { return 2; }
          }
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right',
              margin: [0, 2, 0, 2]
            },
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right',
              margin: [0, 2, 0, 2]
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: '',
              style: { fontSize: 10, bold: true },
              alignment: 'right'
            },
            {
              width: '50%',
              text: 'Ngày ' + dateNow.getDate() + ' tháng ' + (dateNow.getMonth() + 1) + ' năm ' + dateNow.getFullYear(),
              style: { fontSize: 10, bold: false, italics: true },
              alignment: 'center'
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: 'NGƯỜI ĐỀ NGHỊ',
              style: { fontSize: 10, bold: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            },
            {
              width: '50%',
              text: 'PHỤ TRÁCH',
              style: { fontSize: 10, bold: true },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            }
          ]
        },
        {
          columns: [
            {
              width: '50%',
              text: employee.employeeName,
              style: { fontSize: 10, bold: false },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            },
            {
              width: '50%',
              text: employeePic.employeeName,
              style: { fontSize: 10, bold: false },
              alignment: 'center',
              margin: [0, 2, 0, 2]
            }
          ]
        }
      ],
      styles: {
        header: {
          fontSize: 18.5,
          bold: true
        },
        default: {
          fontSize: 10,
          bold: true
        },
        timer: {
          fontSize: 10,
          color: '#1F497D'
        },
        table: {
          margin: [0, 15, 0, 15]
        },
        tableHeader: {
          fontSize: 10,
          bold: true,
          fillColor: '#E3EDF8',
          margin: [0, 2, 0, 2],
        },
        tableLine: {
          fontSize: 10,
        },
        tableLines: {
          fontSize: 9,
        },
        tableLiness: {
          fontSize: 7,
        },
        StyleItalics: {
          italics: true
        }
      }
    };

    this.listSuggestedSupplierQuotesDetail.forEach((item, index) => {
      let col7 = "";
      let option = [
        { text: index + 1, style: 'tableLines', alignment: 'center' },
        {
          text: item.productCode,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: item.productName,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: item.productUnitName,
          style: 'tableLines',
          alignment: 'left'
        },
        {
          text: this.decimalPipe.transform(item.quantity).toString(),
          style: 'tableLines',
          alignment: 'right'
        },

        {
          text: item.note,
          style: 'tableLines',
          alignment: 'left'
        },
        { text: col7, style: 'tableLines', alignment: 'right' },
      ];
      documentDefinition.content[10].table.body.push(option);
    });

    let vendorQuoteNameExport = convertStringCusName(vendor.vendorName);
    let title = 'Đề nghị báo giá' + '_' + dateNow.getUTCFullYear() + (dateNow.getMonth() + 1) + dateNow.getDate();

    if (type === 'download') {
      pdfMake.createPdf(documentDefinition).download(title + '.pdf');
    }
    else {
      pdfMake.createPdf(documentDefinition).getBase64(async function (encodedString) {
        localStorage.setItem('base64PDFQuote', encodedString);
      });
    }
  }
  sendQuote() {
    let base64 = localStorage.getItem('base64PDFQuote');
    let vendor = this.listVendor.find(c => c.vendorId == this.suggestedSupplierQuoteRequestModel.vendorId);
    let vendorCode = vendor != null ? vendor.vendorCode : '';
    let vendorName = vendor != null ? vendor.vendorName : '';

    let ref = this.dialogService.open(SendMailVendorQuoteDialogComponent, {
      data: {
        sendTo: this.suggestedSupplierQuoteRequestModel.email,
        suggestedSupplierQuoteId: this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId,
        vendorId: vendor.vendorId,
        vendorCode: vendorCode,
        vendorName: vendorName,
        base64: base64
      },
      header: 'Gửi Email',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Gửi email thành công" };
        this.showMessage(mgs);
      }
      this.resetForm();
      this.ngOnInit();
    });
  }

  goToVendorPriceList() {
    this.router.navigate(['/vendor/list-vendor-price', { suggestedSupplierQuoteId: this.suggestedSupplierQuoteRequestModel.suggestedSupplierQuoteId }]);
  }

  getBase64Logo() {
    let base64Logo = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "Logo");
    return base64Logo?.systemValueString;
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function convertStringCusName(str: string) {
  if (str.includes("-", 0)) {
    str = str.replace("-", " ");
  }
  while (str.includes("  ", 0)) {
    str = str.replace("  ", " ");
  }
  while (str.includes(" ", 0)) {
    str = str.replace(" ", "_");
  }
  return str;
}
