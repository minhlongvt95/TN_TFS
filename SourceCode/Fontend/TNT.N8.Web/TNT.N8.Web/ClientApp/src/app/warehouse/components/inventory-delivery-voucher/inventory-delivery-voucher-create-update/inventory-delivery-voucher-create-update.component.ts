import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { WarehouseService } from "../../../services/warehouse.service";
import { CategoryService } from "../../../../shared/services/category.service";
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { TreeWarehouseComponent } from '../../tree-warehouse/tree-warehouse.component';
import { DeliveryvoucherCreateSerialComponent } from '../../serial/deliveryvoucher-create-serial/deliveryvoucher-create-serial.component';
import { AddProductComponent } from '../../add-product/add-product.component';
import { InventoryDeliveryVoucher } from '../../../models/InventoryDeliveryVoucher.model';
import { EmployeeService } from '../../../../employee/services/employee.service';
import * as $ from 'jquery';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CustomerService } from '../../../../customer/services/customer.service';
import { NoteModel } from '../../../../shared/models/note.model';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-inventory-delivery-voucher-create-update',
  templateUrl: './inventory-delivery-voucher-create-update.component.html',
  styleUrls: ['./inventory-delivery-voucher-create-update.component.css'],
  providers: [
    DialogService
  ]
})
export class InventoryDeliveryVoucherCreateUpdateComponent implements OnInit {
  createBusinessCusForm: FormGroup;
  createInventoryDeliveryVoucherCusFormType1: FormGroup;
  createInventoryDeliveryVoucherCusFormType2: FormGroup;
  createInventoryDeliveryVoucherCusFormTypeAnother: FormGroup;
  informationVoucherForm: FormGroup;

  receiverControlType1: FormControl;
  vendorControl: FormControl;

  wareControlType1: FormControl;
  wareControlType2: FormControl;
  wareControlType3: FormControl;

  receiverControlType3: FormControl
  reasonControlType1: FormControl;
  reasonControlType3: FormControl;

  typeControl: FormControl;

  vendorOrderControl: FormControl;
  receiverControlType2: FormControl;
  reasonControlType2: FormControl;

  customerOrderControl: FormControl;
  receivedDateControl: FormControl;
  receivedHourControl: FormControl;
  numberFileControl: FormControl;

  radioButtonFollowVendorOrder: FormControl;
  radioButtonFollowWarehouse: FormControl;
  radioButtonFollowProductCheck: FormControl;
  radioButtonFollowExpect: FormControl;
  rowDataNoteControl: FormControl;
  rowDataQuantityControl: FormControl;
  noteContentControl: FormControl;

  //Code moi
  dateNow: any;
  inventoryVoucher: InventoryDeliveryVoucher = new InventoryDeliveryVoucher;
  listCategoryId: Array<string> = [];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listVendor: Array<any> = [];
  listWarehouse: Array<any> = [];
  noteHistory: Array<string> = [];
  createName: string;
  files: File[] = [];
  loading: boolean = false;

  filterVendorAutoComplete: any[];
  cols: any[];
  colsType3and4: any[];
  idVendorSelection: any;
  lstVendorOrder: any[];
  lstCustomerOrder: any[];

  selectVendor: any;
  selectCustomer: any;
  selectedWarehouse: any;
  selectedVendorOrder = [];
  selectVendorOrder: any;
  selectCustomerOrder: any;
  listVendorOrderProduct: Array<any> = [];
  listVendorOrderProductType3and4: Array<any> = [];
  listCustomer: any[] = [];
  frozenCols = [];
  ref: DynamicDialogRef;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  noteContent: string = '';
  uploadedFiles: any[] = [];
  idInventoryDeliveryVoucher: string = '';
  noteModel = new NoteModel();
  isEdit = false;
  isNKH = false;
  listStatus: Array<any> = [];
  createEmpAccountList: Array<any> = [];
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  isEditDisableCreate: boolean = true;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionView: boolean = true;
  // Is possition fiexd
  fixed:boolean = false;
  withFiexd: string = "";

  constructor(
    public employeeService: EmployeeService,
    public dialogService: DialogService,
    public messageService: MessageService,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    private renderer: Renderer2,
    private router: Router,
    private confirmationService: ConfirmationService,
    private warehouseService: WarehouseService) {

    this.translate.setDefaultLang('vi');
    this.renderer.listen('window', 'click', (e: Event) => {
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target)
        ) {
          this.isOpenNotifiError = false;
        }
      }
    });

  }

  async ngOnInit() {
    this.route.params.subscribe(params => {
      this.idInventoryDeliveryVoucher = params['id'];
    });
    let resource = "war/warehouse/inventory-delivery-voucher/create-update/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.actionView = false;
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("view") == -1) {
        this.actionView = false;
      }
    }

    this.getFormControl();
    this.typeControl.setValue(1);
    await this.getMasterData();

    this.dateNow = new Date();
    this.receivedDateControl.setValue(this.dateNow);
    this.inventoryVoucher.InventoryDeliveryVoucherId = this.emptyGuid;
    this.inventoryVoucher.LicenseNumber = 1;
    this.inventoryVoucher.InventoryDeliveryVoucherType = 1;
    this.inventoryVoucher.InventoryDeliveryVoucherDate = this.dateNow;
    this.inventoryVoucher.NameCreate = '';
    this.inventoryVoucher.NameObject = '';
    this.inventoryVoucher.NameStatus = '';
    this.inventoryVoucher.CreatedDate = this.dateNow;
    this.inventoryVoucher.CreatedById = this.emptyGuid;
    this.inventoryVoucher.Active = true;
    //code moi
    if (this.actionView) {
      this.cols = [
        //{ field: 'vendorOrderCode', header: 'Phiếu mua hàng' },
        { field: 'productCode', header: 'Mã SP', width: '25%', textAlign: 'center', color: '#f44336' },
        { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantityRequire', header: 'SL cần xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantityInventory', header: 'SL tồn kho', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantity', header: 'Số lượng xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'price', header: 'Giá xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'exchangeRate', header: 'Tỉ giá', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Ghi chú', width: '40%', textAlign: 'center', color: '#f44336' },
        { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
      ];
    } else {
      this.cols = [
        //{ field: 'vendorOrderCode', header: 'Phiếu mua hàng' },
        { field: 'productCode', header: 'Mã SP', width: '25%', textAlign: 'center', color: '#f44336' },
        { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantityRequire', header: 'SL cần xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantityInventory', header: 'SL tồn kho', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantity', header: 'Số lượng xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'price', header: 'Giá xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'exchangeRate', header: 'Tỉ giá', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Ghi chú', width: '40%', textAlign: 'center', color: '#f44336' },
        { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
      ];

    }
    this.frozenCols = [
      { field: 'vendorOrderCode', header: 'Phiếu mua hàng' },
    ];

    if (this.actionView) {
      this.colsType3and4 = [
        { field: 'productCode', header: 'Mã SP', width: '15%', textAlign: 'center', color: '#f44336' },
        { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'quantityRequire', header: 'SL cần xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantityInventory', header: 'SL tồn kho', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantity', header: 'SL xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'price', header: 'Giá xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'exchangeRate', header: 'Tỷ giá', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Ghi chú', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
      ];
    }
    else {
      this.colsType3and4 = [
        { field: 'productCode', header: 'Mã SP', width: '15%', textAlign: 'center', color: '#f44336' },
        { field: 'productName', header: 'Diễn giải', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'unitName', header: 'Đơn vị tính', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantityInventory', header: 'SL tồn kho', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'quantity', header: 'SL xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'wareHouseId', header: 'Vị trí', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'price', header: 'Giá xuất', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'nameMoneyUnit', header: 'Đơn vị tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'exchangeRate', header: 'Tỷ giá', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'vat', header: 'VAT(%)', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'discountValue', header: 'Chiết khấu', width: '30%', textAlign: 'center', color: '#f44336' },
        //{ field: 'sumAmount', header: 'Thành tiền', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'note', header: 'Ghi chú', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'totalSerial', header: 'Số serial', width: '30%', textAlign: 'center', color: '#f44336' },
        { field: 'box', header: '', width: '2%', textAlign: 'center', color: '#f44336' },
        ]
    }
    if (this.idInventoryDeliveryVoucher != '' && this.idInventoryDeliveryVoucher != null) {
      this.isEdit = true;
      this.warehouseService.getInventoryDeliveryVoucherById(this.idInventoryDeliveryVoucher, this.auth.UserId).subscribe(response => {
        let result = <any>response;
        this.inventoryVoucher = this.convertToObjectTypeScript(result.inventoryDeliveryVoucher);
        //kiểm tra trạng thái của phiếu nhập kho
        var itemStatus = this.listStatus.find(f => f.categoryId == this.inventoryVoucher.StatusId);
        if (itemStatus.categoryCode == 'NHK') {
          this.isNKH = true;
        };

        if (this.inventoryVoucher.InventoryDeliveryVoucherType == 1 || this.inventoryVoucher.InventoryDeliveryVoucherType == 2) {
          this.listVendorOrderProduct.push.apply(this.listVendorOrderProduct, result.inventoryDeliveryVoucherMappingModel);
        }
        else {
          this.listVendorOrderProductType3and4.push.apply(this.listVendorOrderProductType3and4, result.inventoryDeliveryVoucherMappingModel);
        }
        //add mac dinh cho warhouse
        this.selectedWarehouse = this.listWarehouse.find(f => f.warehouseId == this.inventoryVoucher.WarehouseId);
      }, error => { });
      this.getNoteHistory();
    }
  }

  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }

  async getMasterData() {
    this.loading = true;
    //get Status
    var result: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc('TPHX');
    this.listStatus = result.category;
    var resultEMP: any = await this.employeeService.getAllEmployeeAsync();
    this.createEmpAccountList = resultEMP.employeeList;
    let currentUser = this.createEmpAccountList.find(item => item.employeeId == this.auth.EmployeeId);
    var nameValue = currentUser.employeeCode + " - " + currentUser.employeeName;
    this.createName = nameValue;
    //
    this.warehouseService.getWareHouseCha().subscribe(response => {
      let result = <any>response;
      this.listWarehouse = result.listWareHouse;
    }, error => { });
    //
    var resultCustomerOrder: any = await this.warehouseService.inventoryDeliveryVoucherFilterCustomerOrderAsyc();
    this.lstCustomerOrder = resultCustomerOrder.listCustomerOrder;
    //
    var resultVendorOrder: any = await this.warehouseService.inventoryDeliveryVoucherFilterVendorOrderAsyc();
    this.lstVendorOrder = resultVendorOrder.listVendorOrder;

    this.loading = false;
  }

  getNoteHistory() {
    //this.loading = true;
    this.customerService.getNoteHistory(this.idInventoryDeliveryVoucher).subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.noteHistory = result.listNote;
        result.listNote.forEach(element => {
          this.noteModel.Description = element.description;
          this.noteModel.NoteId = element.noteId;
          this.noteModel.NoteTitle = element.noteTitle;
          setTimeout(() => {
            $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
            $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
          }, 1000);
        });
      } else {
      }
    });
  }

  getCustomerOrder() {
    this.warehouseService.inventoryDeliveryVoucherFilterCustomerOrder().subscribe(response => {
      let result = <any>response;
      this.lstCustomerOrder = [];
      this.lstCustomerOrder = result.listCustomerOrder;

    }, error => { });
  }

  getVendorOrder() {
    this.warehouseService.inventoryDeliveryVoucherFilterVendorOrder().subscribe(response => {
      let result = <any>response;
      this.lstVendorOrder = [];
      this.lstVendorOrder = result.listVendorOrder;
    }, error => { });
  }

  getListWareHouseChar() {
    this.warehouseService.getWareHouseCha().subscribe(response => {
      let result = <any>response;
      this.listWarehouse = result.listWareHouse;
    }, error => { });
  }

  getFormControl() {
    this.vendorControl = new FormControl('', [Validators.required]);
    this.wareControlType3 = new FormControl('', [Validators.required]);
    this.receiverControlType3 = new FormControl('', [Validators.maxLength(200)]);
    this.reasonControlType3 = new FormControl('', [Validators.maxLength(200)]);
    this.typeControl = new FormControl('');
    this.vendorOrderControl = new FormControl('', [Validators.required]);
    this.receiverControlType2 = new FormControl('', [Validators.maxLength(200)]);
    this.reasonControlType2 = new FormControl('', [Validators.maxLength(200)]);
    this.wareControlType2 = new FormControl('', [Validators.required]);

    this.customerOrderControl = new FormControl('', [Validators.required]);
    this.receiverControlType1 = new FormControl('', [Validators.maxLength(200)]);
    this.reasonControlType1 = new FormControl('', [Validators.maxLength(200)]);
    this.wareControlType1 = new FormControl('', [Validators.required]);

    this.receivedDateControl = new FormControl('');
    this.receivedHourControl = new FormControl('');
    this.numberFileControl = new FormControl('', [Validators.required]);
    this.radioButtonFollowVendorOrder = new FormControl('');
    this.radioButtonFollowWarehouse = new FormControl('');
    this.radioButtonFollowProductCheck = new FormControl('');
    this.radioButtonFollowExpect = new FormControl('');
    this.rowDataNoteControl = new FormControl('');
    this.rowDataQuantityControl = new FormControl('');
    this.noteContentControl = new FormControl('',[Validators.maxLength(201)]);

    this.createBusinessCusForm = new FormGroup({
      typeControl: this.typeControl,
      radioButtonFollowVendorOrder: this.radioButtonFollowVendorOrder,
      radioButtonFollowWarehouse: this.radioButtonFollowWarehouse,
      radioButtonFollowProductCheck: this.radioButtonFollowProductCheck,
      radioButtonFollowExpect: this.radioButtonFollowExpect,
      rowDataNoteControl: this.rowDataNoteControl,
      rowDataQuantityControl: this.rowDataQuantityControl,
      noteContentControl: this.noteContentControl
    });

    this.createInventoryDeliveryVoucherCusFormType1 = new FormGroup({
      reasonControlType1: this.reasonControlType1,
      wareControlType1: this.wareControlType1,
      customerOrderControl: this.customerOrderControl,
      receiverControlType1: this.receiverControlType1
    });

    this.createInventoryDeliveryVoucherCusFormType2 = new FormGroup({
      wareControlType2: this.wareControlType2,
      vendorOrderControl: this.vendorOrderControl,
      receiverControlType2: this.receiverControlType2,
      reasonControlType2: this.reasonControlType2
    });

    this.createInventoryDeliveryVoucherCusFormTypeAnother = new FormGroup({
      wareControlType3: this.wareControlType3,
      receiverControlType3: this.receiverControlType3,
      reasonControlType3: this.reasonControlType3
    });

    this.informationVoucherForm = new FormGroup({
      receivedDateControl: this.receivedDateControl,
      receivedHourControl: this.receivedHourControl,
      numberFileControl: this.numberFileControl
    });
  }

  createOrUpdateInventoryVoucher() {
    switch (this.inventoryVoucher.InventoryDeliveryVoucherType.toString()) {
      case "1": {
        if (!this.createInventoryDeliveryVoucherCusFormType1.valid) {
          Object.keys(this.createInventoryDeliveryVoucherCusFormType1.controls).forEach(key => {
            if (this.createInventoryDeliveryVoucherCusFormType1.controls[key].valid == false) {
              this.createInventoryDeliveryVoucherCusFormType1.controls[key].markAsTouched();
            }
          });
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
          this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
            switch (validity) {
              case "VALID":
                this.isInvalidForm = false;
                break;
              case "INVALID":
                this.isInvalidForm = true;
                break;
            }
          });
        }
        else {
          this.Create();
        }
        break;
      }
      case "2": {
        if (!this.createInventoryDeliveryVoucherCusFormType2.valid) {
          Object.keys(this.createInventoryDeliveryVoucherCusFormType2.controls).forEach(key => {
            if (this.createInventoryDeliveryVoucherCusFormType2.controls[key].valid == false) {
              this.createInventoryDeliveryVoucherCusFormType2.controls[key].markAsTouched();
            }
          });
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
          this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
            switch (validity) {
              case "VALID":
                this.isInvalidForm = false;
                break;
              case "INVALID":
                this.isInvalidForm = true;
                break;
            }
          });
        }
        else {
          this.Create();
        }
        break;
      }
      default: {
        if (!this.createInventoryDeliveryVoucherCusFormTypeAnother.valid) {
          Object.keys(this.createInventoryDeliveryVoucherCusFormTypeAnother.controls).forEach(key => {
            if (this.createInventoryDeliveryVoucherCusFormTypeAnother.controls[key].valid == false) {
              this.createInventoryDeliveryVoucherCusFormTypeAnother.controls[key].markAsTouched();
            }
          });
          this.isInvalidForm = true;  //Hiển thị icon-warning-active
          this.isOpenNotifiError = true;  //Hiển thị message lỗi
          this.emitStatusChangeForm = this.createBusinessCusForm.statusChanges.subscribe((validity: string) => {
            switch (validity) {
              case "VALID":
                this.isInvalidForm = false;
                break;
              case "INVALID":
                this.isInvalidForm = true;
                break;
            }
          });
        }
        else {
          this.Create();
        }
        break;
      }
    }
  }

  Create() {
    if (this.inventoryVoucher.InventoryDeliveryVoucherType == 1 || this.inventoryVoucher.InventoryDeliveryVoucherType == 2) {
      if (this.listVendorOrderProduct.length == 0 || this.listVendorOrderProduct.length == null) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn ít nhất một sản phẩm", detail: 'Lưu phiếu xuất kho' });
      }
      else {
        var isError = false;
        for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
          if (this.listVendorOrderProduct[i].error == true) {
            isError = true;
            break;
          }
        }
        if (isError == true) {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: "Có lỗi trong danh sách sản phẩm", detail: 'Danh sách sản phẩm' });
          return;
        }

        this.loading = true;
        this.warehouseService.createUpdateInventoryDeliveryVoucher(this.inventoryVoucher, this.listVendorOrderProduct, this.files, this.noteContent, this.auth.UserId).subscribe(response => {
          var result = <any>response;
          this.messageService.clear();
          this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Lưu phiếu xuất kho' });
          this.router.navigate(['/warehouse/inventory-delivery-voucher/details', { id: result.inventoryDeliveryVoucherId }]);
          this.isEdit = true;
          this.loading = false;

        }, error => {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Lưu phiếu xuất kho' });
        });
      }
    } else {
      if (this.listVendorOrderProductType3and4.length == 0 || this.listVendorOrderProductType3and4.length == null) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn ít nhất một sản phẩm", detail: 'Lưu phiếu xuất kho' });
      }
      else {
        var isError = false;
        for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
          if (this.listVendorOrderProductType3and4[i].error == true) {
            isError = true;
            break;
          }
        }
        if (isError == true) {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: "Có lỗi trong danh sách sản phẩm", detail: 'Danh sách sản phẩm' });
          return;
        }

        this.loading = true;
        this.warehouseService.createUpdateInventoryDeliveryVoucher(this.inventoryVoucher, this.listVendorOrderProductType3and4, this.files, this.noteContent, this.auth.UserId).subscribe(response => {
          var result = <any>response;
          this.messageService.clear();
          this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Lưu phiếu xuất kho' });
          this.router.navigate(['/warehouse/inventory-delivery-voucher/details', { id: result.inventoryDeliveryVoucherId }]);
          this.isEdit = true;
          this.loading = false;

        }, error => {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Lưu phiếu xuất kho' });
        });
      }
    }
  }

  //code moi
  filterVendor(event) {
    this.filterVendorAutoComplete = [];
    for (let i = 0; i < this.listVendor.length; i++) {
      let vendor = this.listVendor[i];
      if (vendor.vendorName.toLowerCase().includes(event.query.toLowerCase())) {
        this.filterVendorAutoComplete.push(vendor);
      }
    }
  }

  changeVendorOrder(event: any) {
    if (event.value == null) return;
    let lstVendorOrderIdSelectedArray: any[] = [];
    lstVendorOrderIdSelectedArray.push(event.value.vendorOrderId);
    this.inventoryVoucher.ObjectId = event.value.vendorOrderId;

    this.warehouseService.getVendorOrderDetailByVenderOrderId(lstVendorOrderIdSelectedArray,2).subscribe(responseProduct => {
      let resultProduct = <any>responseProduct;
      this.listVendorOrderProduct = resultProduct.listOrderProduct;
      for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
        if (this.selectedWarehouse != null) {
          this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
          this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
        }
        this.listVendorOrderProduct[i].error = false;
        this.listVendorOrderProduct[i].choose = true;
        this.listVendorOrderProduct[i].chooseChild = true;
      }
    }, error => { });
  }

  changeCustomerOrder(event: any) {
    if (event.value == null) return;
    let lstCustomerOrderIdSelectedArray: any[] = [];

    lstCustomerOrderIdSelectedArray.push(event.value.orderId);
    this.inventoryVoucher.ObjectId = event.value.orderId;
    this.warehouseService.getCustomerOrderDetailByCustomerOrderId(lstCustomerOrderIdSelectedArray,2).subscribe(responseProduct => {
      let resultProduct = <any>responseProduct;
      this.listVendorOrderProduct = resultProduct.listOrderProduct;
      for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
        if (this.selectedWarehouse != null) {
          this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
          this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
        }
        this.listVendorOrderProduct[i].error = false;
        this.listVendorOrderProduct[i].quantityInventory = 0;
        this.listVendorOrderProduct[i].choose = true;
        this.listVendorOrderProduct[i].chooseChild = true;
      }
    }, error => { });
  }

  onchangeWarehousedropdown(event) {
    for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
      if (this.selectedWarehouse != null) {
        this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
        this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
      }
    }
  }

  showTreeWarehouse(rowData) {
    if (this.inventoryVoucher.WarehouseId == null || this.inventoryVoucher.WarehouseId == '') {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Cần chọn kho cha trước", detail: 'Danh sách sản phẩm' });
    }
    else {
      this.ref = this.dialogService.open(TreeWarehouseComponent, {
        header: 'Chọn kho xuất',
        width: '30%',
        baseZIndex: 10000,
        data: { object: this.inventoryVoucher.WarehouseId, productId: rowData.productId }
      });

      this.ref.onClose.subscribe((item: any) => {
        if (item) {
          rowData.wareHouseName = item.warhousename;
          rowData.wareHouseId = item.warhouseId;
        }
      });
    }
  }

  showCreateSerial(item: any) {
    this.ref = this.dialogService.open(DeliveryvoucherCreateSerialComponent, {
      header: 'NHẬP SERIAL',
      width: '35%',
      //contentStyle: { "max-height": "350px" },
      baseZIndex: 10000,
      closable: false,
      data: { object: item }
    });
  }

  changeWarhouse(event: any) {
    if (event.value == null) return;
    this.inventoryVoucher.WarehouseId = event.value.warehouseId;
    if (this.listVendorOrderProduct != null) {
      for (let i = 0; i < this.listVendorOrderProduct.length; i++) {
        if (this.selectedWarehouse != null) {
          //if (this.listVendorOrderProduct[i].wareHouseId === this.emptyGuid) {
            this.listVendorOrderProduct[i].wareHouseName = this.selectedWarehouse.warehouseName;
            this.listVendorOrderProduct[i].wareHouseId = this.selectedWarehouse.warehouseId;
          //}
        }
        this.listVendorOrderProduct[i].choose = true;
        this.listVendorOrderProduct[i].chooseChild = true;
      }
    }
    if (this.listVendorOrderProductType3and4 != null) {
      for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
        if (this.selectedWarehouse != null) {
          //if (this.listVendorOrderProductType3and4[i].wareHouseId === null || this.listVendorOrderProductType3and4[i].wareHouseId === '') {
            this.listVendorOrderProductType3and4[i].wareHouseName = this.selectedWarehouse.warehouseName;
            this.listVendorOrderProductType3and4[i].wareHouseId = this.selectedWarehouse.warehouseId;
          //}
        }
        //this.listVendorOrderProduct[i].choose = true;
        //this.listVendorOrderProduct[i].chooseChild = true;
      }
    }
  }
  
  convertToObjectTypeScript(object: any) {
    var result = new InventoryDeliveryVoucher();
    if (typeof object.warehouseId !== 'undefined') {
      result.WarehouseId = object.warehouseId;
    }
    if (typeof object.objectId !== 'undefined') {
      result.ObjectId = object.objectId;
      this.customerOrderControl.setValue(object.objectId);
      this.vendorOrderControl.setValue(object.objectId);
    }
    if (typeof object.statusId !== 'undefined') {
      result.StatusId = object.statusId;
    }
    if (typeof object.receiver !== 'undefined') {
      result.Receiver = object.receiver;
    }
    if (typeof object.reason !== 'undefined') {
      result.Reason = object.reason;
    }
    if (typeof object.licenseNumber !== 'undefined') {
      result.LicenseNumber = object.licenseNumber;
    }
    if (typeof object.inventoryDeliveryVoucherType !== 'undefined') {
      result.InventoryDeliveryVoucherType = object.inventoryDeliveryVoucherType;
    }
    if (typeof object.inventoryDeliveryVoucherTime !== 'undefined') {
      result.InventoryDeliveryVoucherTime = object.inventoryDeliveryVoucherTime;
    }
    if (typeof object.inventoryDeliveryVoucherId !== 'undefined') {
      result.InventoryDeliveryVoucherId = object.inventoryDeliveryVoucherId;
    }
    if (typeof object.inventoryDeliveryVoucherDate !== 'undefined') {
      result.InventoryDeliveryVoucherDate = new Date(object.inventoryDeliveryVoucherDate);
    }
    if (typeof object.inventoryDeliveryVoucherCode !== 'undefined') {
      result.InventoryDeliveryVoucherCode = object.inventoryDeliveryVoucherCode;
    }
    if (typeof object.active !== 'undefined') {
      result.Active = object.active;
    }
    if (typeof object.createdDate !== 'undefined') {
      result.CreatedDate = new Date(object.createdDate);
    }
    if (typeof object.createdById !== 'undefined') {
      result.CreatedById = object.createdById;
    }
    if (typeof object.nameObject !== 'undefined') {
      result.NameObject = object.nameObject;
    }
    if (typeof object.nameCreate !== 'undefined') {
      result.NameCreate = object.nameCreate;
    } 
    if (typeof object.nameStatus !== 'undefined') {
      result.NameStatus = object.nameStatus;
    }
    if (typeof object.nameOutOfStock !== 'undefined') {
      result.NameOutOfStock = object.nameOutOfStock;
    }
    return result;
  }

  // Kiem tra noteText > 250 hoac noteDocument > 3
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  showAddProduct() {
    this.ref = this.dialogService.open(AddProductComponent, {
      header: 'THÊM SẢN PHẨM',
      width: '50%',
      //contentStyle: { "max-height": "350px" },
      baseZIndex: 10000,
      data: { object: 2 }
    });
    this.ref.onClose.subscribe((item: any) => {
      if (item) {
        this.listVendorOrderProductType3and4.push.apply(this.listVendorOrderProductType3and4, item.listVendorOrderProduct);

        if (this.listVendorOrderProductType3and4 != null) {
          for (let i = 0; i < this.listVendorOrderProductType3and4.length; i++) {
            if (this.selectedWarehouse != null) {
              if (this.listVendorOrderProductType3and4[i].wareHouseId === null || this.listVendorOrderProductType3and4[i].wareHouseId === '') {
                this.listVendorOrderProductType3and4[i].wareHouseName = this.selectedWarehouse.warehouseName;
                this.listVendorOrderProductType3and4[i].wareHouseId = this.selectedWarehouse.warehouseId;
              }
            } 
            //this.listVendorOrderProduct[i].quantityInventory = 0;
            this.listVendorOrderProductType3and4[i].quantity = this.listVendorOrderProductType3and4[i].quantityRequire;
            this.listVendorOrderProductType3and4[i].quantityInventory = 0;
          }
        }
      }
    });
  }

  cancelRow(rowData: any) {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        var index = this.listVendorOrderProductType3and4.findIndex(f => f.productId == rowData.productId);
        this.listVendorOrderProductType3and4.splice(index, 1);
      },
      reject: () => {
        return;
      }
    });
  }

  cancelRowType1(rowData: any) {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        var index = this.listVendorOrderProduct.findIndex(f => f.productId == rowData.productId);
        this.listVendorOrderProduct.splice(index, 1);
      },
      reject: () => {
        return;
      }
    });
  }

  clearAllData() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.listVendorOrderProductType3and4 = [];
      },
      reject: () => {
        return;
      }
    });
  }

  clearAllDataType1() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.listVendorOrderProduct = [];
      },
      reject: () => {
        return;
      }
    });
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  Edit() {
    this.isEdit = false;
  }

  Cancel() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (this.idInventoryDeliveryVoucher != '' && this.idInventoryDeliveryVoucher != null) {
          this.isEdit = true;
        }
        else {
          this.router.navigate(['/warehouse/inventory-delivery-voucher/list']);
        }
      },
      reject: () => {
        return;
      }
    });
  }

  changeTable(code: any) {
    this.listVendorOrderProduct = [];
    this.listVendorOrderProductType3and4 = [];
    switch (code) {
      case 1: {
        this.createInventoryDeliveryVoucherCusFormType2.reset();
        this.createInventoryDeliveryVoucherCusFormTypeAnother.reset();
        break;
      }
      case 2: {
        this.createInventoryDeliveryVoucherCusFormType1.reset();
        this.createInventoryDeliveryVoucherCusFormTypeAnother.reset();
        break;
      }
      default: {
        this.createInventoryDeliveryVoucherCusFormType1.reset();
        this.createInventoryDeliveryVoucherCusFormType2.reset();
        this.createInventoryDeliveryVoucherCusFormTypeAnother.reset();
        break;
      }
    }
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  Delete() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (this.idInventoryDeliveryVoucher != '' && this.idInventoryDeliveryVoucher != null) {
          this.warehouseService.deleteInventoryDeliveryVoucher(this.inventoryVoucher.InventoryDeliveryVoucherId).subscribe(response => {
            var result = <any>response;
            this.messageService.clear();
            this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Xóa phiếu xuất kho' });
            this.router.navigate(['/warehouse/inventory-delivery-voucher/list']);
          }, error => {
            this.messageService.clear();
            this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Xóa phiếu xuất kho' });
          });
        }
        else {
          this.router.navigate(['/warehouse/inventory-delivery-voucher/list']);
        }

      },
      reject: () => {
        return;
      }
    });
  }

  changeStatus() {
    this.warehouseService.changeStatusInventoryDeliveryVoucher(this.inventoryVoucher.InventoryDeliveryVoucherId, this.auth.UserId).subscribe(response => {
      var result = <any>response;
      this.messageService.clear();
      this.isEdit = true;
      this.isNKH = true;
      this.messageService.add({ key: 'success', severity: 'success', summary: result.messageCode, detail: 'Xuất kho' });
      this.router.navigate(['/warehouse/inventory-delivery-voucher/details', { id: this.inventoryVoucher.InventoryDeliveryVoucherId }]);
      window.location.reload();
    }, error => {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: error.messageCode, detail: 'Xuất kho' });
    });
  }

  onSearchNote() {

  }

  refreshSearchFilter() {

  }

  sumTotal(rowdata: any) {
    let quantity = 0;
    let price = 0;
    let exchangeRate = 0;
    if (rowdata.quantity <= 0
      //|| rowdata.price <= 0
      //|| rowdata.quantity == null || rowdata.price == null
    ) {
      rowdata.error = true;
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng xuất cần lớn hơn 0 và nhỏ hơn hoặc bằng số lượng cần xuất", detail: 'Danh sách ' });
      return;
    }
    //product Amount
    quantity = parseFloat(rowdata.quantity.replace(/,/g, ''));
    price = parseFloat(rowdata.price);

    if (quantity <= 0 || price <= 0) return;
    if (quantity > rowdata.quantityRequire || quantity <= 0) {
      rowdata.error = true;
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng xuất cần lớn hơn 0 và nhỏ hơn hoặc bằng số lượng cần xuất", detail: 'Danh sách ' });
      return;
    } else {
      rowdata.error = false;
    }
    if (rowdata.exchangeRate == null) {
      exchangeRate = 1;
    } else {
      exchangeRate = parseFloat(rowdata.exchangeRate);
      if (exchangeRate <= 0) {
        exchangeRate = 1;
      }
    }
    //var productAmount = rowdata.quantity * rowdata.price * rowdata.exchangeRate;
    var productAmount = quantity * price * exchangeRate;
    //Vat
    var Vat = 0;
    if (rowdata.vat !== null) {
      let vat = parseFloat(rowdata.vat);
      if (vat > 0) {
        Vat = (productAmount * vat) / 100;
      }
    }
    var discountAcmount = 0;
    //Discount
    if (rowdata.discountType == true) {
      if (rowdata.discountValue !== null) {
        let discountValue = parseFloat(rowdata.discountValue);
        if (discountValue > 0) {
          discountAcmount = (productAmount * discountValue) / 100;
        }
      }
    }
    else {
      if (rowdata.discountValue !== null) {
        let discountValue = parseFloat(rowdata.discountValue.replace(/,/g, ''));
        discountAcmount = discountValue;
      }
    }
    rowdata.sumAmount = productAmount + Vat - discountAcmount;
  }
}
