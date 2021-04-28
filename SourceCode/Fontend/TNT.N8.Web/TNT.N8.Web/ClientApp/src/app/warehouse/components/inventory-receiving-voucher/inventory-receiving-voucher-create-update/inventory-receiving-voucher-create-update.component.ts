import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { WarehouseService } from "../../../services/warehouse.service";
import { CategoryService } from "../../../../shared/services/category.service";
import { GetPermission } from '../../../../shared/permission/get-permission';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';

import { TreeWarehouseComponent } from '../../tree-warehouse/tree-warehouse.component';
import { PopupCreateSerialComponent } from '../../serial/pop-create-serial/pop-create-serial.component';
import { AddProductComponent } from '../../add-product/add-product.component';

import { InventoryReceivingVoucherModel } from '../../../models/InventoryReceivingVoucher.model';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { SerialModel } from '../../../models/serials.model';
import { SelectionModel } from '@angular/cdk/collections';
import * as $ from 'jquery';

//code moi
import { Message, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CustomerService } from '../../../../customer/services/customer.service';
import { NoteModel } from '../../../../shared/models/note.model';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService } from 'primeng/api';


interface TypeVoucher {
  name: string;
  type: string;
}

@Component({
  selector: 'app-inventory-receiving-voucher-create-update',
  templateUrl: './inventory-receiving-voucher-create-update.component.html',
  styleUrls: ['./inventory-receiving-voucher-create-update.component.css'],
  providers: [
    DialogService
  ]
})
export class InventoryReceivingVoucherCreateUpdateComponent implements OnInit {
  // createBusinessCusForm: FormGroup;
  // createInventoryReceivingVoucherCusFormType1: FormGroup;
  // createInventoryReceivingVoucherCusFormType2: FormGroup;
  // createInventoryReceivingVoucherCusFormType3: FormGroup;
  // informationVoucherForm: FormGroup;

  // shippeerControl: FormControl;
  // vendorControl: FormControl;
  // wareControl: FormControl;
  // wareControlType2: FormControl;
  // wareControlType3: FormControl;
  // customerControl: FormControl;
  // typeControl: FormControl;
  // vendorOrderControl: FormControl;
  // customerOrderControl: FormControl;
  // receivedDateControl: FormControl;
  // receivedHourControl: FormControl;
  // numberFileControl: FormControl;
  // intendedDateControl: FormControl;

  // radioButtonFollowVendorOrder: FormControl;
  // radioButtonFollowWarehouse: FormControl;
  // radioButtonFollowProductCheck: FormControl;
  // radioButtonFollowExpect: FormControl;
  // typeVoucherControl: FormControl;
  // rowDataNoteControl: FormControl;
  // rowDataQuantityControl: FormControl;
  // noteContentControl: FormControl;

  // rows = 10;
  // selection: SelectionModel<any>;
  // files: File[] = [];
  // listCategoryId: Array<string> = [];
  // auth: any = JSON.parse(localStorage.getItem("auth"));
  // sendableFormData: any;
  // totalInput: number = 0;
  // progress: number;
  // hasBaseDropZoneOver = false;
  // baseDropValid: any;
  // lastFileAt: Date;
  // accept = 'image/*, video/*, audio/*, .zip, .rar, .pdf, .xls, .xlsx, .doc, .docx, .ppt, .pptx, .txt';
  // maxSize: number = 11000000;
  // removable: boolean = true;
  // dragFiles: any;
  // warehouseInputId: any;
  // lastInvalids: any;
  // selectedVendorOrderId = [];
  // checkForm: number;
  // listVendor: Array<any> = [];
  // listVendorOrder: Array<any> = [];
  // listWarehouse: Array<any> = [];
  // filteredVendor: Observable<string[]>;
  // filteredVendorOrder: Observable<string[]>;
  // filteredWarehouse: Observable<string[]>;
  // dateNow: any;
  // createName: string;
  // storekeeperName: string;
  // noteHistory: Array<string> = [];
  // listSerial: Array<SerialModel> = [];
  // selectedList: Array<string> = [];
  // inventoryVoucher: InventoryReceivingVoucherModel = new InventoryReceivingVoucherModel;
  // //Code moi
  // filterVendorAutoComplete: any[];
  // cols: any[];
  // colsType3and4: any[];
  // idVendorSelection: any;
  // lstVendorOrder: any[];
  // lstCustomerOrder: any[];
  // lableCustomerOrderCode: any;
  // lableVendorOrderCode: any;
  // selectVendor: any;
  // selectCustomer: any;
  // selectedWarehouse: any;
  // selectedVendorOrder = [];
  // selectVendorOrder: any[] = [];
  // selectCustomerOrder: any[] = [];
  // listVendorOrderProduct: Array<any> = [];
  // listVendorOrderProductType3and4: Array<any> = [];
  // listStatus: Array<any> = [];
  // createEmpAccountList: Array<any> = [];
  // listCustomer: any[] = [];
  // frozenCols = [];
  // ref: DynamicDialogRef;
  // emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  // noteContent: string = '';
  // uploadedFiles: any[] = [];
  // idInventoryReceivingVoucher: string = '';
  // noteModel = new NoteModel();
  // isEdit = false;
  // isNKH = false;
  // isInvalidForm: boolean = false;
  // emitStatusChangeForm: any;
  // @ViewChild('toggleButton') toggleButton: ElementRef;
  // isOpenNotifiError: boolean = false;
  // @ViewChild('notifi') notifi: ElementRef;
  // /*Check user permission*/
  // listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  // actionView: boolean = true;
  // loading: boolean = false;
  // // Is possition fiexd
  // fixed: boolean = false;
  // withFiexd: string = "";
  // today: Date = new Date();
  // minDate: Date = new Date();
  // inventoryReceivingDate: Date;
  // SumQuantity: number = 0; // Tổng số lượng nhập
  // SumAmount: number = 0; // Tổng Thành Tiền
  // typeVoucher: Array<TypeVoucher> = [];
  // listProductId: Array<any> = [];
  // warehouseId: string = '';
  // intendedDate: Date = new Date("dd/MM/yy");
  // listInStock: Array<any> = [];
  constructor(
    public employeeService: EmployeeService,
    public dialogService: DialogService,
    public messageService: MessageService,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private translate: TranslateService,
    private categoryService: CategoryService,
    private getPermission: GetPermission,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private router: Router,
    private warehouseService: WarehouseService
  ) {
    // this.translate.setDefaultLang('vi');
    // this.renderer.listen('window', 'click', (e: Event) => {
    //   if (this.toggleButton && this.notifi) {
    //     if (!this.toggleButton.nativeElement.contains(e.target) &&
    //       !this.notifi.nativeElement.contains(e.target)
    //       //&&
    //       //!this.save.nativeElement.contains(e.target) &&
    //       //!this.saveAndCreate.nativeElement.contains(e.target)
    //     ) {
    //       this.isOpenNotifiError = false;
    //     }
    //   }
    // });
  }

  async ngOnInit() {
    // this.route.params.subscribe(params => {
    //   this.idInventoryReceivingVoucher = params['id'];
    // });
    // let resource = "war/warehouse/inventory-receiving-voucher/create/";
    // let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    // if (permission.status == false) {
    //   this.actionView = false;
    // }
    // else {
    //   let listCurrentActionResource = permission.listCurrentActionResource;
    //   if (listCurrentActionResource.indexOf("view") == -1) {
    //     this.actionView = false;
    //   }
    // }
    // this.getFormControl();
    // this.typeControl.setValue(1);
    // this.checkForm = this.typeControl.value;
    // this.loading = true;
    // this.getListVendor();
    // this.getListWareHouseChar();
    // this.getListCustomer();
    // await this.getMasterData();

    // this.dateNow = new Date();
    // this.receivedDateControl.setValue(this.dateNow);
    // this.inventoryReceivingDate = this.intendedDateControl.value;
    // this.inventoryVoucher.inventoryReceivingVoucherId = this.emptyGuid;
    // this.inventoryVoucher.licenseNumber = 1;
    // this.inventoryVoucher.inventoryReceivingVoucherType = 1;
    // this.inventoryVoucher.inventoryReceivingVoucherDate = this.dateNow;
    // this.inventoryVoucher.createdDate = this.dateNow;
    // this.inventoryVoucher.active = true;
    // this.typeVoucher = [
    //   { name: 'Nhập theo phiếu mua hàng', type: '1' },
    //   { name: 'Nhập kho theo đơn hàng trả lại', type: '2' },
    //   { name: 'Nhập hàng kiểm định', type: '3' },
    //   { name: 'Khác', type: '4' },
    // ]
    // //code moi
    // if (this.actionView) {
    //   this.cols = [
    //     { field: 'vendorOrderCode', header: 'Phiếu mua hàng', textAlign: 'left', color: '#f44336' },
    //     { field: 'productCode', header: 'Mã SP', width: '25%', textAlign: 'left', color: '#f44336' },
    //     { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'quantityRequire', header: 'SL cần nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'quantity', header: 'Số lượng nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'inStock', header: 'Tồn Kho', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'preOrder', header: 'Giữ trước', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'price', header: 'Giá nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'note', header: 'Ghi chú', width: '40%', textAlign: 'left', color: '#f44336' },
    //     { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
    //     // { field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'right', color: '#f44336' },
    //     // { field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'right', color: '#f44336' },
    //     // { field: 'exchangeRate', header: 'Tỉ giá', width: '30%', textAlign: 'right', color: '#f44336' },
    //   ];
    // } else {
    //   this.cols = [
    //     { field: 'vendorOrderCode', header: 'Phiếu mua hàng', textAlign: 'left', color: '#f44336' },
    //     { field: 'productCode', header: 'Mã SP', width: '25%', textAlign: 'left', color: '#f44336' },
    //     { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'quantityRequire', header: 'SL cần nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'quantity', header: 'Số lượng nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'inStock', header: 'Tồn Kho', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'preOrder', header: 'Giữ trước', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'price', header: 'Giá nhập', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'exchangeRate', header: 'Tỉ giá', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'note', header: 'Ghi chú', width: '40%', textAlign: 'left', color: '#f44336' },
    //     { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
    //   ];
    // }
    // this.frozenCols = [
    //   { field: 'vendorOrderCode', header: 'Phiếu mua hàng' },
    // ];
    // if (this.actionView) {
    //   this.colsType3and4 = [
    //     { field: 'productCode', header: 'Mã SP', width: '15%', textAlign: 'left', color: '#f44336' },
    //     { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'left', color: '#f44336' },
    //     //{ field: 'quantityRequire', header: 'SL cần nhập', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'quantity', header: 'SL nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'inStock', header: 'Tồn Kho', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'preOrder', header: 'Giữ trước', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'price', header: 'Giá nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'note', header: 'Ghi chú', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
    //     // { field: 'exchangeRate', header: 'Tỷ giá', width: '30%', textAlign: 'right', color: '#f44336' },
    //     // { field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'right', color: '#f44336' },
    //     // { field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'right', color: '#f44336' },
    //     // { field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'right', color: '#f44336' },
    //   ];
    // }
    // else {
    //   this.colsType3and4 = [
    //     { field: 'productCode', header: 'Mã SP', width: '15%', textAlign: 'left', color: '#f44336' },
    //     { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'left', color: '#f44336' },
    //     //{ field: 'quantityRequire', header: 'SL cần nhập', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'quantity', header: 'SL nhập', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'inStock', header: 'Tồn Kho', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'preOrder', header: 'Giữ trước', width: '30%', textAlign: 'right', color: '#f44336' },
    //     { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'price', header: 'Giá nhập', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'exchangeRate', header: 'Tỷ giá', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'center', color: '#f44336' },
    //     //{ field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'note', header: 'Ghi chú', width: '30%', textAlign: 'left', color: '#f44336' },
    //     { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
    //     { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
    //   ];

    // }


    // if (this.idInventoryReceivingVoucher != '' && this.idInventoryReceivingVoucher != null) {
    //   this.isEdit = true;
    //   this.warehouseService.getInventoryReceivingVoucherById(this.idInventoryReceivingVoucher, this.auth.UserId).subscribe(response => {
    //     let result = <any>response;
    //     this.inventoryVoucher = this.convertToObjectTypeScript(result.inventoryReceivingVoucher);
    //     //kiểm tra trạng thái của phiếu nhập kho
    //     var itemStatus = this.listStatus.find(f => f.categoryId == this.inventoryVoucher.statusId);
    //     if (itemStatus.categoryCode == 'NHK') {
    //       this.isNKH = true;
    //     };

    //     if (this.inventoryVoucher.inventoryReceivingVoucherType == 1 || this.inventoryVoucher.inventoryReceivingVoucherType == 2) {
    //       this.listVendorOrderProduct.push.apply(this.listVendorOrderProduct, result.inventoryReceivingVoucherMapping);
    //       if (this.listVendorOrderProduct.length > 0) {
    //         for (var i = 0; i < this.listVendorOrderProduct.length; ++i) {
    //           var itemService = this.listVendorOrderProduct[i];
    //           if (itemService.listSerial != null) {
    //             var lstSerialModel = new Array<SerialModel>();
    //             for (var j = 0; j < itemService.listSerial.length; ++j) {
    //               var itemSerialConvert = this.convertToSerialModel(itemService.listSerial[j]);
    //               lstSerialModel.push(itemSerialConvert);
    //             }
    //             itemService.listSerial = [];
    //             itemService.listSerial.push.apply(itemService.listSerial, lstSerialModel);
    //           }
    //           this.UpdateInStockAndPreOrder(this.inventoryVoucher.CreatedDate, this.listVendorOrderProduct[i], false);
    //           // calculator SumQuantity And SumAmount
    //           this.SumQuantity += this.listVendorOrderProduct[i].quantity;
    //           this.SumAmount += this.listVendorOrderProduct[i].sumAmount;
    //           this.listInStock.forEach(x => {
    //             // tồn kho
    //             if (x.productId == this.listVendorOrderProduct[i].productId) {
    //               this.listVendorOrderProduct[i].inStock = x.quantityInStock;
    //             }
    //             updatePreOrder(this.listVendorOrderProduct[i], x);
    //           });
    //         }
    //       }
    //       this.intendedDate = this.inventoryVoucher.InventoryReceivingVoucherDate;
    //       this.intendedDateControl.setValue(new Date(this.intendedDate));
    //       this.inventoryReceivingDate = this.intendedDate;
    //       this.lableVendorOrderCode = Array.from(new Set(this.listVendorOrderProduct.map(a => a.vendorOrderCode)));
    //     }
    //     else {
    //       this.listVendorOrderProductType3and4.push.apply(this.listVendorOrderProductType3and4, result.inventoryReceivingVoucherMapping);
    //       if (this.listVendorOrderProductType3and4.length > 0) {
    //         for (var i = 0; i < this.listVendorOrderProductType3and4.length; ++i) {
    //           var itemService = this.listVendorOrderProductType3and4[i];
    //           if (itemService.listSerial != null) {
    //             var lstSerialModel = new Array<SerialModel>();
    //             for (var j = 0; j < itemService.listSerial.length; ++j) {
    //               var itemSerialConvert = this.convertToSerialModel(itemService.listSerial[j]);
    //               lstSerialModel.push(itemSerialConvert);
    //             }
    //             itemService.listSerial = [];
    //             itemService.listSerial.push.apply(itemService.listSerial, lstSerialModel);
    //           }
    //           this.UpdateInStockAndPreOrder(this.inventoryVoucher.CreatedDate, this.listVendorOrderProductType3and4[i], false);
    //           // calculator SumQuantity And SumAmount
    //           this.SumQuantity += this.listVendorOrderProductType3and4[i].quantity;
    //           this.SumAmount += this.listVendorOrderProductType3and4[i].sumAmount;
    //         }
    //       }
    //       this.intendedDate = this.inventoryVoucher.InventoryReceivingVoucherDate;
    //       this.intendedDateControl.setValue(new Date(this.intendedDate));
    //       this.inventoryReceivingDate = this.intendedDate;
    //       this.lableVendorOrderCode = Array.from(new Set(this.listVendorOrderProductType3and4.map(a => a.vendorOrderCode)));
    //     }
    //     //load vendor
    //     if (result.selectVendor != null) {
    //       this.warehouseService.filterVendor().subscribe(response3 => {
    //         let result3 = <any>response3;
    //         this.listVendor = result3.vendorList;
    //         this.selectVendor = this.listVendor.find(f => f.vendorId == result.selectVendor.vendorId);

    //       }, error => { });
    //     }
    //     //Load customer
    //     if (result.selectCustomer != null) {
    //       this.warehouseService.filterCustomer().subscribe(response4 => {
    //         let result4 = <any>response4;
    //         this.listCustomer = result4.customer;
    //         this.selectCustomer = this.listCustomer.find(f => f.customerId == result.selectCustomer.customerId);
    //       });
    //       this.warehouseService.getListCustomerOrderByIdCustomerId(result.selectCustomer.customerId).subscribe(response5 => {
    //         let result5 = <any>response5;
    //         this.lstCustomerOrder = [];
    //         this.lstCustomerOrder = result5.listCustomerOrder;
    //         this.selectCustomerOrder = [];
    //         this.selectCustomerOrder.push.apply(this.selectCustomerOrder, result.listCustomerOrder);
    //       }, error => { });

    //     }

    //     //tao datasource cho vendororderdetail
    //     this.selectVendorOrder = [];
    //     this.selectVendorOrder.push.apply(this.selectVendorOrder, result.listVendorOrder);
    //     //add mac dinh cho warhouse
    //     this.selectedWarehouse = this.listWarehouse.find(f => f.warehouseId == this.inventoryVoucher.WarehouseId);
    //   }, error => { });

    //   this.getNoteHistory();
    // }
  }

  // @HostListener('document:scroll', [])
  // onScroll(): void {
  //   let num = window.pageYOffset;
  //   if (num > 100) {
  //     this.fixed = true;
  //     var width: number = $('#parent').width();
  //     this.withFiexd = width + 'px';
  //   } else {
  //     this.fixed = false;
  //     this.withFiexd = "";
  //   }
  // }

  // getListVendor() {
  //   this.warehouseService.filterVendor().subscribe(response => {
  //     let result = <any>response;
  //     this.listVendor = result.vendorList;
  //   }, error => { });
  // }

  // getListWareHouseChar() {
  //   this.warehouseService.getWareHouseCha().subscribe(response => {
  //     let result = <any>response;
  //     this.listWarehouse = result.listWareHouse;
  //   }, error => { });
  // }

  // getIdVendorSelected() {
  //   this.selectedVendorOrderId = [];
  //   for (let i = 0; i < this.selectedVendorOrder.length; i++) {
  //     this.selectedVendorOrderId.push(this.selectedVendorOrder[i].vendorOrderId);
  //   }
  // }

  // ChangeInventoryReceivingDate() {
  //   this.inventoryReceivingDate = this.intendedDateControl.value;
  // }

  // getFormControl() {
  //   this.vendorControl = new FormControl('', [Validators.required]);
  //   this.customerControl = new FormControl('', [Validators.required]);
  //   this.wareControl = new FormControl('', [Validators.required]);
  //   this.wareControlType2 = new FormControl('', [Validators.required]);
  //   this.wareControlType3 = new FormControl('', [Validators.required]);
  //   this.typeControl = new FormControl('');
  //   this.vendorOrderControl = new FormControl('', [Validators.required]);
  //   this.customerOrderControl = new FormControl('', [Validators.required]);
  //   this.shippeerControl = new FormControl('', [Validators.maxLength(200)]);
  //   this.receivedDateControl = new FormControl('');
  //   this.receivedHourControl = new FormControl('');
  //   this.numberFileControl = new FormControl('', [Validators.required]);
  //   this.radioButtonFollowVendorOrder = new FormControl('');
  //   this.radioButtonFollowWarehouse = new FormControl('');
  //   this.radioButtonFollowProductCheck = new FormControl('');
  //   this.radioButtonFollowExpect = new FormControl('');
  //   this.typeVoucherControl = new FormControl('');
  //   this.rowDataNoteControl = new FormControl('');
  //   this.rowDataQuantityControl = new FormControl('');
  //   this.noteContentControl = new FormControl('', [Validators.maxLength(201)]);
  //   this.intendedDateControl = new FormControl(this.today, [Validators.required]);

  //   this.createBusinessCusForm = new FormGroup({
  //     typeControl: this.typeControl,
  //     radioButtonFollowVendorOrder: this.radioButtonFollowVendorOrder,
  //     radioButtonFollowWarehouse: this.radioButtonFollowWarehouse,
  //     radioButtonFollowProductCheck: this.radioButtonFollowProductCheck,
  //     radioButtonFollowExpect: this.radioButtonFollowExpect,
  //     typeVoucherControl: this.typeVoucherControl,
  //     rowDataNoteControl: this.rowDataNoteControl,
  //     rowDataQuantityControl: this.rowDataQuantityControl,
  //     noteContentControl: this.noteContentControl
  //   });

  //   this.createInventoryReceivingVoucherCusFormType1 = new FormGroup({
  //     vendorControl: this.vendorControl,
  //     wareControl: this.wareControl,
  //     vendorOrderControl: this.vendorOrderControl,
  //     shippeerControl: this.shippeerControl,
  //     intendedDateControl: this.intendedDateControl,
  //   });

  //   this.createInventoryReceivingVoucherCusFormType2 = new FormGroup({
  //     customerControl: this.customerControl,
  //     wareControlType2: this.wareControlType2,
  //     customerOrderControl: this.customerOrderControl,
  //     shippeerControl: this.shippeerControl,
  //     intendedDateControl: this.intendedDateControl,
  //   });

  //   this.createInventoryReceivingVoucherCusFormType3 = new FormGroup({
  //     wareControlType3: this.wareControlType3,
  //     shippeerControl: this.shippeerControl,
  //     intendedDateControl: this.intendedDateControl,
  //   });

  //   this.informationVoucherForm = new FormGroup({
  //     receivedDateControl: this.receivedDateControl,
  //     receivedHourControl: this.receivedHourControl,
  //     numberFileControl: this.numberFileControl,
  //   });
  // }

  // clearDataVendor() {
  //   this.vendorControl.reset();
  // }

  // clearDataWarehouse() {
  //   this.wareControl.reset();
  // }

  // getDate() {
  //   return new Date();
  // }

  // onRemoveFile(index: number) {
  //   this.files.splice(index, 1);
  // }

  // onSearchNote() {
  //   //this.loading = true;
  //   //let fromDate;
  //   //if (this.searchNoteFromDate !== null) {
  //   //  fromDate = new Date(this.searchNoteFromDate);
  //   //  this.searchNoteFromDate = new Date(Date.UTC(fromDate.getFullYear(), fromDate.getMonth(), fromDate.getDate()));
  //   //}

  //   //let toDate;
  //   //if (this.searchNoteToDate !== null) {
  //   //  toDate = new Date(this.searchNoteToDate);
  //   //  this.searchNoteToDate = new Date(Date.UTC(toDate.getFullYear(), toDate.getMonth(), toDate.getDate()));
  //   //}

  //   //this.noteService.searchNote(this.searchNoteKeyword, this.searchNoteFromDate, this.searchNoteToDate, this.customerId).subscribe(response => {
  //   //  this.noteHistory = [];
  //   //  let result = <any>response;

  //   //  if (result.statusCode === 202 || result.statusCode === 200) {
  //   //    result.noteList.forEach(element => {
  //   //      this.noteModel.Description = element.description;
  //   //      this.noteModel.NoteId = element.noteId;
  //   //      this.noteModel.NoteTitle = element.noteTitle;
  //   //      setTimeout(() => {
  //   //        $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
  //   //        $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
  //   //      }, 1000);
  //   //    });

  //   //    this.noteHistory = result.noteList;
  //   //  } else {
  //   //    this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
  //   //  }

  //   //  this.loading = false;
  //   //}, error => { });
  // }

  // getNoteHistory() {
  //   this.customerService.getNoteHistory(this.idInventoryReceivingVoucher).subscribe(response => {
  //     const result = <any>response;
  //     if (result.statusCode === 202 || result.statusCode === 200) {
  //       this.noteHistory = result.listNote;
  //       result.listNote.forEach(element => {
  //         this.noteModel.Description = element.description;
  //         this.noteModel.NoteId = element.noteId;
  //         this.noteModel.NoteTitle = element.noteTitle;
  //         setTimeout(() => {
  //           $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
  //           $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
  //         }, 1000);
  //       });
  //     }
  //   });
  // }

  // refreshSearchFilter() {
  //   //this.searchNoteKeyword = "";
  //   //this.searchNoteFromDate = null;
  //   //this.searchNoteToDate = null;
  //   //this.getNoteHistory();
  // }

  // getListCustomer() {
  //   this.warehouseService.filterCustomer().subscribe(response => {
  //     let result = <any>response;
  //     this.listCustomer = result.customer;
  //     this.loading = false;
  //   });
  // }

  // async getMasterData() {
  //   //get Status
  //   var result: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('TPH');
  //   this.listStatus = result.category;
  //   var resultEMP: any = await this.employeeService.getAllEmployeeAsync();
  //   this.createEmpAccountList = resultEMP.employeeList;
  //   //Set default value cho nhân viên tạo và đề xuất cho
  //   let currentUser = this.createEmpAccountList.find(item => item.employeeId == this.auth.EmployeeId);
  //   var nameValue = currentUser.employeeCode + " - " + currentUser.employeeName;
  //   this.createName = nameValue;
  // }

  // createOrUpdateInventoryVoucher() {
  //   switch (this.inventoryVoucher.InventoryReceivingVoucherType.toString()) {
  //     case "1": {
  //       if (!this.createInventoryReceivingVoucherCusFormType1.valid) {
  //         Object.keys(this.createInventoryReceivingVoucherCusFormType1.controls).forEach(key => {
  //           if (this.createInventoryReceivingVoucherCusFormType1.controls[key].valid == false) {
  //             this.createInventoryReceivingVoucherCusFormType1.controls[key].markAsTouched();
  //           }
  //         });
  //         this.isInvalidForm = true;  //Hiển thị icon-warning-active
  //         this.isOpenNotifiError = true;  //Hiển thị message lỗi
  //         this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
  //           switch (validity) {
  //             case "VALID":
  //               this.isInvalidForm = false;
  //               break;
  //             case "INVALID":
  //               this.isInvalidForm = true;
  //               break;
  //           }
  //         });
  //       }
  //       else {
  //         this.Create();
  //       }
  //       break;
  //     }
  //     case "2": {
  //       if (!this.createInventoryReceivingVoucherCusFormType2.valid) {
  //         Object.keys(this.createInventoryReceivingVoucherCusFormType2.controls).forEach(key => {
  //           if (this.createInventoryReceivingVoucherCusFormType2.controls[key].valid == false) {
  //             this.createInventoryReceivingVoucherCusFormType2.controls[key].markAsTouched();
  //           }
  //         });
  //         this.isInvalidForm = true;  //Hiển thị icon-warning-active
  //         this.isOpenNotifiError = true;  //Hiển thị message lỗi
  //         this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
  //           switch (validity) {
  //             case "VALID":
  //               this.isInvalidForm = false;
  //               break;
  //             case "INVALID":
  //               this.isInvalidForm = true;
  //               break;
  //           }
  //         });
  //       }
  //       else {
  //         this.Create();
  //       }

  //       break;
  //     }
  //     case "3": {
  //       if (!this.createInventoryReceivingVoucherCusFormType3.valid) {
  //         Object.keys(this.createInventoryReceivingVoucherCusFormType3.controls).forEach(key => {
  //           if (this.createInventoryReceivingVoucherCusFormType3.controls[key].valid == false) {
  //             this.createInventoryReceivingVoucherCusFormType3.controls[key].markAsTouched();
  //           }
  //         });
  //         this.isInvalidForm = true;  //Hiển thị icon-warning-active
  //         this.isOpenNotifiError = true;  //Hiển thị message lỗi
  //         this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
  //           switch (validity) {
  //             case "VALID":
  //               this.isInvalidForm = false;
  //               break;
  //             case "INVALID":
  //               this.isInvalidForm = true;
  //               break;
  //           }
  //         });
  //       }
  //       else {
  //         this.Create();
  //       }
  //       break;
  //     }
  //     case "4": {
  //       if (!this.createInventoryReceivingVoucherCusFormType3.valid) {
  //         Object.keys(this.createInventoryReceivingVoucherCusFormType3.controls).forEach(key => {
  //           if (this.createInventoryReceivingVoucherCusFormType3.controls[key].valid == false) {
  //             this.createInventoryReceivingVoucherCusFormType3.controls[key].markAsTouched();
  //           }
  //         });
  //         this.isInvalidForm = true;  //Hiển thị icon-warning-active
  //         this.isOpenNotifiError = true;  //Hiển thị message lỗi
  //         this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
  //           switch (validity) {
  //             case "VALID":
  //               this.isInvalidForm = false;
  //               break;
  //             case "INVALID":
  //               this.isInvalidForm = true;
  //               break;
  //           }
  //         });
  //       }
  //       else {
  //         this.Create();
  //       }
  //       break;
  //     }
  //     default:
  //   }
  // }

  // UpdateInStockAndPreOrder(fromDate: any, item: any, oncheckStock: boolean) {
  //   let listInStock: Array<any> = [];
  //   this.listProductId.push(item.productId);
  //   this.warehouseId = item.wareHouseId;
  //   this.warehouseService.searchInStockReport(fromDate, item.productCode, "", item.wareHouseId)
  //     .subscribe(response => {
  //       let result: any = response;
  //       this.loading = false;
  //       if (result.statusCode == 200) {
  //         listInStock = result.listResult;
  //         listInStock.forEach(x => {
  //           // tồn kho
  //           if (x.productId == item.productId) {
  //             item.inStock = x.quantityInStock;
  //             // giữu trước
  //             updatePreOrder(item, x);
  //             if (oncheckStock) {
  //               // kiểm tra khả dụng
  //               let tmp = item.inStock + item.quantityRequire;
  //               if (tmp < x.quantityInStockMaximum) {
  //                 this.messageService.add({
  //                   severity: 'info',
  //                   summary: 'Kiểm tra khả dụng:',
  //                   detail: 'Mã hàng "' + item.productCode + '" thuộc kho "' + item.wareHouseName + '" tồn kho hiện thời phù hợp'
  //                 });
  //                 updatePreOrder(item, x);
  //               } else {
  //                 this.messageService.add({
  //                   severity: 'warn',
  //                   summary: 'Kiểm tra khả dụng:',
  //                   detail: 'Mã hàng "' + item.productCode + '" thuộc kho "' + item.wareHouseName + '" tồn kho hiện thời âm'
  //                 });
  //                 updatePreOrder(item, x);
  //               }
  //             }
  //           }
  //         });
  //       } else {
  //         this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: result.messageCode });
  //       }
  //     });
  // }

  // OnCheckInStock(date: any) {
  //   if (this.listProductId.length <= 0) {
  //     this.messageService.add({ key: 'error', severity: 'error', summary: "Kiểm tra khả dụng", detail: "cần chọn ít nhất một sản phẩm" });
  //   }
  //   else if (this.warehouseId == "") {
  //     this.messageService.add({ key: 'error', severity: 'error', summary: "Kiểm tra khả dụng", detail: "cần chọn kho nhập sản phẩm" });
  //   } else {

  //     if (this.inventoryVoucher.InventoryReceivingVoucherType == 1 ||
  //       this.inventoryVoucher.InventoryReceivingVoucherType == 2) {
  //       if (this.listVendorOrderProduct.length > 0) {
  //         for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
  //           this.UpdateInStockAndPreOrder(date, this.listVendorOrderProduct[i], true);
  //         }
  //       } else {
  //         this.messageService.add({ key: 'error', severity: 'error', summary: "Kiểm tra khả dụng", detail: "cần chọn ít nhất một sản phẩm" });
  //       }
  //     }
  //     else {
  //       if (this.listVendorOrderProductType3and4.length > 0) {
  //         for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
  //           this.UpdateInStockAndPreOrder(date, this.listVendorOrderProductType3and4[i], true);
  //         }
  //       } else {
  //         this.messageService.add({ key: 'error', severity: 'error', summary: "Kiểm tra khả dụng", detail: "cần chọn ít nhất một sản phẩm" });
  //       }
  //     }
  //   }
  // }

  // Create() {
  //   this.intendedDate = this.intendedDateControl.value;
  //   this.inventoryVoucher.InventoryReceivingVoucherDate = convertToUTCTime(this.intendedDate);
  //   if (this.inventoryVoucher.InventoryReceivingVoucherType == 1 || this.inventoryVoucher.InventoryReceivingVoucherType == 2) {
  //     if (this.listVendorOrderProduct.length == 0 || this.listVendorOrderProduct.length == null) {
  //       this.messageService.clear();
  //       this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn ít nhất một sản phẩm", detail: 'Lưu phiếu nhập kho' });
  //     }
  //     else {
  //       var isError = false;
  //       for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
  //         if (this.listVendorOrderProduct[i].error == true) {
  //           isError = true;
  //           break;
  //         }
  //       }
  //       if (isError == true) {
  //         this.messageService.clear();
  //         this.messageService.add({ key: 'error', severity: 'error', summary: "Có lỗi trong danh sách sản phẩm", detail: 'Danh sách sản phẩm' });
  //         return;
  //       }
  //       this.loading = true;

  //       this.warehouseService.createOrUpdateInventoryVoucher(this.inventoryVoucher, this.listVendorOrderProduct, this.files, this.noteContent, this.auth.UserId).subscribe(response => {
  //         var result = <any>response;
  //         this.messageService.clear();
  //         this.isEdit = true;
  //         this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Lưu phiếu nhập kho' });
  //         this.loading = false;
  //         this.router.navigate(['/warehouse/inventory-receiving-voucher/details', { id: result.inventoryReceivingVoucherId }]);
  //       }, error => {
  //         this.messageService.clear();
  //         this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Lưu phiếu nhập kho' });
  //       });
  //     }
  //   } else {
  //     if (this.listVendorOrderProductType3and4.length == 0 || this.listVendorOrderProductType3and4.length == null) {
  //       this.messageService.clear();
  //       this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn ít nhất một sản phẩm", detail: 'Lưu phiếu nhập kho' });
  //     }
  //     else {

  //       var isError = false;
  //       for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
  //         if (this.listVendorOrderProductType3and4[i].error == true) {
  //           isError = true;
  //           break;
  //         }
  //       }
  //       if (isError == true) {
  //         this.messageService.clear();
  //         this.messageService.add({ key: 'error', severity: 'error', summary: "Có lỗi trong danh sách sản phẩm", detail: 'Danh sách sản phẩm' });
  //         return;
  //       }

  //       this.loading = true;

  //       this.warehouseService.createOrUpdateInventoryVoucher(this.inventoryVoucher, this.listVendorOrderProductType3and4, this.files, this.noteContent, this.auth.UserId).subscribe(response => {
  //         var result = <any>response;
  //         this.messageService.clear();
  //         this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Lưu phiếu nhập kho' });
  //         this.loading = false;

  //         this.router.navigate(['/warehouse/inventory-receiving-voucher/details', { id: result.inventoryReceivingVoucherId }]);
  //       }, error => {
  //         this.messageService.clear();
  //         this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Lưu phiếu nhập kho' });
  //       });
  //     }
  //   }
  // }

  // //code moi
  // filterVendor(event) {
  //   this.filterVendorAutoComplete = [];
  //   for (let i = 0; i < this.listVendor.length; i++) {
  //     let vendor = this.listVendor[i];
      
  //     if (vendor.vendorName.toLowerCase().includes(event.query.toLowerCase())) {
  //       this.filterVendorAutoComplete.push(vendor);
  //     }

  //   }
  // }

  // selectVendorFn(event) {
  //   //emty selectVendorOrder da chon tuoc
  //   if (event.value == null) return;
  //   this.selectVendorOrder = [];
  //   this.warehouseService.getVendorOrderByVendorId(event.value.vendorId).subscribe(response => {
  //     let result = <any>response;
  //     this.lstVendorOrder = [];
  //     this.lstVendorOrder = result.listVendorOrder;

  //   }, error => { });
  // }

  // selectCustomerFn(event) {
  //   if (event.value == null) return;
  //   //emty selectVendorOrder da chon tuoc
  //   this.selectCustomerOrder = [];
  //   this.warehouseService.getListCustomerOrderByIdCustomerId(event.value.customerId).subscribe(response => {
  //     let result = <any>response;
  //     this.lstCustomerOrder = [];
  //     this.lstCustomerOrder = result.listCustomerOrder;
  //   }, error => { });

  // }

  // closePanelVendorOrder() {
  //   this.SumQuantity = 0; // Tổng số lượng nhập
  //   this.SumAmount = 0; // Tổng thành tiền
  //   let createdDate = this.inventoryVoucher.CreatedDate

  //   let lstVendorOrderIdSelectedArray: any[] = [];
  //   this.selectVendorOrder.forEach(function (vendororderobject) {
  //     lstVendorOrderIdSelectedArray.push(vendororderobject.vendorOrderId);
  //   });

  //   this.warehouseService.getVendorOrderDetailByVenderOrderId(lstVendorOrderIdSelectedArray, 1).subscribe(responseProduct => {
  //     let resultProduct = <any>responseProduct;
  //     this.listVendorOrderProduct = resultProduct.listOrderProduct;
  //     for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
  //       if (this.selectedWarehouse != null) {
  //         this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
  //         this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
  //       }

  //       this.UpdateInStockAndPreOrder(createdDate, this.listVendorOrderProduct[i], false);

  //       this.listVendorOrderProduct[i].choose = true;
  //       this.listVendorOrderProduct[i].chooseChild = true;
  //       this.listVendorOrderProduct[i].error = false;
  //       this.SumQuantity += this.listVendorOrderProduct[i].quantity;
  //       this.SumAmount += this.listVendorOrderProduct[i].sumAmount;
  //     }
  //   }, error => { });
  // }

  // closePanelCustomerOrder() {
  //   let lstCustomerOrderIdSelectedArray: any[] = [];
  //   this.selectCustomerOrder.forEach(function (customeroderobject) {
  //     lstCustomerOrderIdSelectedArray.push(customeroderobject.orderId);
  //   });

  //   this.warehouseService.getCustomerOrderDetailByCustomerOrderId(lstCustomerOrderIdSelectedArray, 1).subscribe(responseProduct => {
  //     let resultProduct = <any>responseProduct;
  //     this.listVendorOrderProduct = resultProduct.listOrderProduct;
  //     for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
  //       if (this.selectedWarehouse != null) {
  //         this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
  //         this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
  //       }

  //       this.UpdateInStockAndPreOrder(this.inventoryVoucher.CreatedDate, this.listVendorOrderProduct[i], false);

  //       this.listVendorOrderProduct[i].choose = true;
  //       this.listVendorOrderProduct[i].chooseChild = true;
  //       this.listVendorOrderProduct[i].error = false;
  //       this.SumQuantity += this.listVendorOrderProduct[i].quantity;
  //       this.SumAmount += this.listVendorOrderProduct[i].sumAmount;
  //     }
  //   }, error => { });
  // }

  // onchangeWarehousedropdown(event) {
  //   for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
  //     if (this.selectedWarehouse != null) {
  //       this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
  //       this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
  //     }
  //   }
  // }

  // showTreeWarehouse(rowData) {
  //   let createdDate = this.inventoryVoucher.CreatedDate;
  //   if (this.inventoryVoucher.WarehouseId == null || this.inventoryVoucher.WarehouseId == '') {
  //     this.messageService.clear();
  //     this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn kho cha trước", detail: 'Danh sách sản phẩm' });
  //   }
  //   else {
  //     this.ref = this.dialogService.open(TreeWarehouseComponent, {
  //       header: 'Chọn kho nhập',
  //       width: '30%',
  //       baseZIndex: 10000,
  //       data: { object: this.inventoryVoucher.WarehouseId, productId: null }
  //     });

  //     this.ref.onClose.subscribe(async (item: any) => {
  //       if (item) {

  //         this.UpdateInStockAndPreOrder(createdDate, rowData, false);
  //         // giữ trước
  //         if (rowData.inStock > rowData.quantityRequire) {
  //           rowData.preOrder = rowData.quantityRequire;
  //         } else {
  //           rowData.preOrder = rowData.inStock;
  //         }
  //       }
  //     });
  //   }
  // }

  // showCreateSerial(item: any) {
  //   this.ref = this.dialogService.open(PopupCreateSerialComponent, {
  //     header: 'NHẬP SERIAL',
  //     width: '35%',
  //     //contentStyle: { "max-height": "350px" },
  //     baseZIndex: 10000,
  //     data: { object: item }
  //   });
  // }

  // changeWarhouse(event: any) {
  //   if (event.value == null) return;
  //   this.inventoryVoucher.WarehouseId = event.value.warehouseId;
  //   this.warehouseId = event.value.warehouseId;
  //   if (this.listVendorOrderProduct != null) {
  //     for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
  //       if (this.selectedWarehouse != null) {
  //         if (this.listVendorOrderProduct[i].wareHouseId === this.emptyGuid) {
  //           this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
  //           this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
  //         }
  //       }
  //       this.listVendorOrderProduct[i].choose = true;
  //       this.listVendorOrderProduct[i].chooseChild = true;
  //     }
  //   }
  //   if (this.listVendorOrderProductType3and4 != null) {
  //     for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
  //       if (this.selectedWarehouse != null) {
  //         if (this.listVendorOrderProductType3and4[i].wareHouseId === null || this.listVendorOrderProductType3and4[i].wareHouseId === '') {
  //           this.listVendorOrderProductType3and4[i].wareHouseName = this.selectedWarehouse.warehouseName;
  //           this.listVendorOrderProductType3and4[i].wareHouseId = this.selectedWarehouse.warehouseId;
  //         }
  //       }
  //     }
  //   }
  // }

  // convertToObjectTypeScript(object: any) {
  //   var result = new InventoryReceivingVoucherModel();
  //   if (typeof object.warehouseId !== 'undefined') {
  //     result.WarehouseId = object.warehouseId;
  //   }
  //   if (typeof object.storekeeper !== 'undefined') {
  //     result.Storekeeper = object.storekeeper;
  //   }
  //   if (typeof object.statusId !== 'undefined') {
  //     result.StatusId = object.statusId;
  //   }
  //   if (typeof object.shiperName !== 'undefined') {
  //     result.ShiperName = object.shiperName;
  //   }
  //   if (typeof object.licenseNumber !== 'undefined') {
  //     result.LicenseNumber = object.licenseNumber;
  //   }
  //   if (typeof object.inventoryReceivingVoucherType !== 'undefined') {
  //     result.InventoryReceivingVoucherType = object.inventoryReceivingVoucherType;
  //   }
  //   if (typeof object.inventoryReceivingVoucherTime !== 'undefined') {
  //     result.InventoryReceivingVoucherTime = object.inventoryReceivingVoucherTime;
  //   }
  //   if (typeof object.inventoryReceivingVoucherId !== 'undefined') {
  //     result.InventoryReceivingVoucherId = object.inventoryReceivingVoucherId;
  //   }
  //   if (typeof object.inventoryReceivingVoucherDate !== 'undefined') {
  //     result.InventoryReceivingVoucherDate = new Date(object.inventoryReceivingVoucherDate);
  //   }
  //   if (typeof object.inventoryReceivingVoucherCode !== 'undefined') {
  //     result.InventoryReceivingVoucherCode = object.inventoryReceivingVoucherCode;
  //   }
  //   if (typeof object.active !== 'undefined') {
  //     result.Active = object.active;
  //   }
  //   if (typeof object.createdDate !== 'undefined') {
  //     result.CreatedDate = new Date(object.createdDate);
  //   }
  //   if (typeof object.nameCreate !== 'undefined') {
  //     result.NameCreate = object.nameCreate;
  //   }
  //   if (typeof object.nameStorekeeper !== 'undefined') {
  //     result.NameStorekeeper = object.nameStorekeeper;
  //   }
  //   if (typeof object.nameStorekeeper !== 'undefined') {
  //     result.NameStorekeeper = object.nameStorekeeper;
  //   }
  //   if (typeof object.createdById !== 'undefined') {
  //     result.CreatedById = object.createdById;
  //   }

  //   return result;
  // }

  // convertToSerialModel(object: any) {
  //   var result = new SerialModel();
  //   if (typeof object.serialId !== 'undefined') {
  //     result.SerialId = object.serialId;
  //   }
  //   if (typeof object.serialCode !== 'undefined') {
  //     result.SerialCode = object.serialCode;
  //   }
  //   if (typeof object.productId !== 'undefined') {
  //     result.ProductId = object.productId;
  //   }
  //   if (typeof object.warehouseId !== 'undefined') {
  //     result.WarehouseId = object.warehouseId;
  //   }
  //   if (typeof object.active !== 'undefined') {
  //     result.Active = object.active;
  //   }
  //   if (typeof object.createdDate !== 'undefined') {
  //     result.CreatedDate = object.createdDate;
  //   }
  //   return result;
  // }

  // // Kiem tra noteText > 250 hoac noteDocument > 3
  // tooLong(note): boolean {
  //   if (note.noteDocList.length > 3) return true;
  //   var des = $.parseHTML(note.description);
  //   var count = 0;
  //   for (var i = 0; i < des.length; i++) {
  //     count += des[i].textContent.length;
  //     if (count > 250) return true;
  //   }
  //   return false;
  // }

  // showAddProduct() {
  //   this.SumQuantity = 0; // Tổng số lượng nhập
  //   this.SumAmount = 0; // Tổng thành tiền
  //   this.ref = this.dialogService.open(AddProductComponent, {
  //     header: 'THÊM SẢN PHẨM',
  //     width: '50%',
  //     //contentStyle: { "max-height": "350px" },
  //     baseZIndex: 10000,
  //     data: { object: 1 }
  //   });
  //   this.ref.onClose.subscribe((item: any) => {
  //     if (item) {

  //       this.listVendorOrderProductType3and4.push.apply(this.listVendorOrderProductType3and4, item.listVendorOrderProduct);

  //       if (this.listVendorOrderProductType3and4 != null) {
  //         for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
  //           if (this.selectedWarehouse != null) {
  //             if (this.listVendorOrderProductType3and4[i].wareHouseId === null || this.listVendorOrderProductType3and4[i].wareHouseId === '') {
  //               this.listVendorOrderProductType3and4[i].wareHouseName = this.selectedWarehouse.warehouseName;
  //               this.listVendorOrderProductType3and4[i].wareHouseId = this.selectedWarehouse.warehouseId;
  //             }
  //           }
  //           let quantityRequire = this.listVendorOrderProductType3and4[i].quantityRequire;
  //           this.listVendorOrderProductType3and4[i].quantity = parseFloat(quantityRequire.replace(/,/g, ''));
  //           this.listVendorOrderProductType3and4[i].quantityRequire = parseFloat(quantityRequire.replace(/,/g, ''));
  //           this.UpdateInStockAndPreOrder(this.inventoryVoucher.CreatedDate, this.listVendorOrderProductType3and4[i], false);
  //           // calculator SumQuantity And SumAmount
  //           this.SumQuantity += parseFloat(quantityRequire.replace(/,/g, ''));
  //           this.SumAmount += this.listVendorOrderProductType3and4[i].sumAmount;
  //         }
  //       }

  //     }
  //   });
  // }

  // cancelRow(rowData: any) {
  //   var index = this.listVendorOrderProductType3and4.findIndex(f => f.productId == rowData.productId);
  //   this.listVendorOrderProductType3and4.splice(index, 1);
  // }

  // cancelRowType1(rowData: any) {
  //   var index = this.listVendorOrderProduct.findIndex(f => f.productId == rowData.productId);
  //   this.listVendorOrderProduct.splice(index, 1);
  // }

  // clearAllData() {
  //   this.listVendorOrderProductType3and4 = [];
  // }

  // clearAllDataType1() {
  //   this.listVendorOrderProduct = [];
  // }

  // scroll(el: HTMLElement) {
  //   el.scrollIntoView();
  // }

  // Edit() {
  //   this.isEdit = false;
  // }

  // Cancel() {
  //   if (this.idInventoryReceivingVoucher != '' && this.idInventoryReceivingVoucher != null) {
  //     this.isEdit = true;
  //   }
  //   else {
  //     this.router.navigate(['/warehouse/inventory-receiving-voucher/list']);
  //   }
  // }

  // Delete() {
  //   this.confirmationService.confirm({
  //     message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
  //     header: 'Thông báo',
  //     icon: 'pi pi-exclamation-triangle',
  //     accept: () => {
  //       if (this.idInventoryReceivingVoucher != '' && this.idInventoryReceivingVoucher != null) {
  //         this.warehouseService.deleteInventoryReceivingVoucher(this.inventoryVoucher.InventoryReceivingVoucherId).subscribe(response => {
  //           var result = <any>response;
  //           this.messageService.clear();
  //           this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Xóa phiếu nhập kho' });
  //           this.router.navigate(['/warehouse/inventory-receiving-voucher/list']);
  //         }, error => {
  //           this.messageService.clear();
  //           this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Xóa phiếu nhập kho' });
  //         });
  //       }
  //       else {
  //         this.router.navigate(['/warehouse/inventory-receiving-voucher/list']);
  //       }

  //     },
  //     reject: () => {
  //       return;
  //     }
  //   });
  // }

  // changeTable(event) {
  //   switch (event.value.type) {
  //     case "1": {
  //       this.inventoryVoucher.InventoryReceivingVoucherType = 1;
  //       this.createInventoryReceivingVoucherCusFormType2.reset();
  //       this.createInventoryReceivingVoucherCusFormType3.reset();
  //       this.intendedDateControl.setValue(new Date);
  //       this.inventoryReceivingDate = new Date();
  //       this.listVendorOrderProduct = [];
  //       this.listVendorOrderProductType3and4 = [];
  //       break;
  //     }
  //     case "2": {
  //       this.inventoryVoucher.InventoryReceivingVoucherType = 2;
  //       this.createInventoryReceivingVoucherCusFormType1.reset();
  //       this.vendorControl.reset();
  //       this.createInventoryReceivingVoucherCusFormType3.reset();
  //       this.intendedDateControl.setValue(new Date);
  //       this.inventoryReceivingDate = new Date();
  //       this.listVendorOrderProduct = [];
  //       this.listVendorOrderProductType3and4 = [];

  //       break;
  //     }
  //     case "3": {
  //       this.inventoryVoucher.InventoryReceivingVoucherType = 3;
  //       this.createInventoryReceivingVoucherCusFormType1.reset();
  //       this.vendorControl.reset();
  //       this.createInventoryReceivingVoucherCusFormType2.reset();
  //       this.intendedDateControl.setValue(new Date);
  //       this.inventoryReceivingDate = new Date();
  //       this.listVendorOrderProduct = [];
  //       this.listVendorOrderProductType3and4 = [];
  //       break;
  //     }
  //     case "4": {
  //       this.inventoryVoucher.InventoryReceivingVoucherType = 4;
  //       this.createInventoryReceivingVoucherCusFormType1.reset();
  //       this.vendorControl.reset();
  //       this.createInventoryReceivingVoucherCusFormType2.reset();
  //       this.intendedDateControl.setValue(new Date);
  //       this.inventoryReceivingDate = new Date();
  //       this.listVendorOrderProduct = [];
  //       this.listVendorOrderProductType3and4 = [];

  //       break;
  //     }
  //     default:
  //   }
  // }

  // toggleNotifiError() {
  //   this.isOpenNotifiError = !this.isOpenNotifiError;
  // }

  // sumTotal(rowdata: any) {
  //   let quantity = 0;
  //   let price = 0;
  //   let exchangeRate = 0;
  //   if (rowdata.quantity <= 0) {
  //     rowdata.error = true;
  //     this.messageService.clear();
  //     this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng nhập cần lớn hơn 0 và nhỏ hơn hoặc bằng số lượng nhập", detail: 'Danh sách ' });
  //     return;
  //   }
  //   //product Amount
  //   quantity = parseFloat(rowdata.quantity.replace(/,/g, ''));
  //   price = parseFloat(rowdata.price);

  //   if (quantity <= 0 || price <= 0) return;
  //   if (quantity > rowdata.quantityRequire || quantity <= 0) {
  //     rowdata.error = true;
  //     this.messageService.clear();
  //     this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng nhập cần lớn hơn 0 và nhỏ hơn hoặc bằng số lượng nhập", detail: 'Danh sách ' });
  //     return;
  //   } else {
  //     rowdata.error = false;
  //   }
  //   if (rowdata.exchangeRate == null) {
  //     exchangeRate = 1;
  //   } else {
  //     exchangeRate = parseFloat(rowdata.exchangeRate);
  //     if (exchangeRate <= 0) {
  //       exchangeRate = 1;
  //     }
  //   }
    
  //   var productAmount = quantity * price * exchangeRate;
    
  //   var Vat = 0;
  //   if (rowdata.vat !== null) {
  //     let vat = parseFloat(rowdata.vat);
  //     if (vat > 0) {
  //       Vat = (productAmount * vat) / 100;
  //     }
  //   }
  //   var discountAcmount = 0;
    
  //   if (rowdata.discountType == true) {
  //     if (rowdata.discountValue !== null) {
  //       let discountValue = parseFloat(rowdata.discountValue);
  //       if (discountValue > 0) {
  //         discountAcmount = (productAmount * discountValue) / 100;
  //       }
  //     }
  //   }
  //   else {
  //     if (rowdata.discountValue !== null) {
  //       let discountValue = parseFloat(rowdata.discountValue.replace(/,/g, ''));
  //       discountAcmount = discountValue;
  //     }
  //   }
  //   rowdata.sumAmount = productAmount + Vat - discountAcmount;
  // }

  // changeStatus() {
  //   this.loading = true;
  //   this.warehouseService.changeStatusInventoryReceivingVoucher(this.inventoryVoucher.InventoryReceivingVoucherId).subscribe(response => {
  //     var result = <any>response;
  //     this.messageService.clear();
  //     this.isEdit = true;
  //     this.isNKH = true;
  //     this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Nhập kho' });
  //     this.loading = false;
  //     this.router.navigate(['/warehouse/inventory-receiving-voucher/details', { id: this.inventoryVoucher.InventoryReceivingVoucherId }]);
  //   }, error => {
  //     this.messageService.clear();
  //     this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Nhập kho' });
  //   });
  // }

  // checklengthNote(rowdata: any) {
  //   if (rowdata.note.length > 200) {
  //     rowdata.error = true;
  //     this.messageService.clear();
  //     this.messageService.add({ key: 'error', severity: 'error', summary: "Không được nhập quá 200 ký tự", detail: 'Danh sách ' });
  //     return;
  //   }
  //   else {
  //     rowdata.note = rowdata.note.trim();
  //   }
  // }
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

function updatePreOrder(VendorOrderProduct: any, inStockReport: any) {
  let a = VendorOrderProduct.quantityRequire;
  let b = Math.abs(inStockReport.quantityInStockMaximum - inStockReport.quantityInStock);

  // giữ trước
  if (b > a) {
    VendorOrderProduct.preOrder = a;
  } else {
    VendorOrderProduct.preOrder = b;
  }

  return null;
};