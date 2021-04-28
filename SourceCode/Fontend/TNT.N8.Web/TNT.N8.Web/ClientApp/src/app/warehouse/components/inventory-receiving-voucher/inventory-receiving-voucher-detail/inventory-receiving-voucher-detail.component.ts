import { Component, OnInit, HostListener, ViewChild, ElementRef, Renderer2, ChangeDetectorRef  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { WarehouseService } from '../../../services/warehouse.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import * as $ from 'jquery';
import { TreeNode } from 'primeng/api';
import { Table } from 'primeng/table';
import { SanPhamPhieuNhapKhoModel } from '../../../models/sanPhamPhieuNhapKhoModel.model';
import { PhieuNhapKho } from '../../../models/phieuNhapKho.model';
import { InventoryReceivingVoucherModel } from '../../../models/InventoryReceivingVoucher.model';
import { InventoryReceivingVoucherMapping } from '../../../models/inventoryReceivingVoucherMapping.model';
import { FileUpload } from 'primeng/fileupload';
import { ImageUploadService } from '../../../../shared/services/imageupload.service';
import { ForderConfigurationService } from '../../../../admin/components/folder-configuration/services/folder-configuration.service';
import { GetPermission } from '../../../../shared/permission/get-permission';

class LoaiPhieuNhap {
  name: string;
  type: number;
}

class Vendor {
  vendorId: string;
  vendorCode: string;
  vendorName: string;
  vendorCodeName: string;
}

class Warehouse {
  warehouseId: string;
  warehouseParent: string;
  hasChild: boolean;
  warehouseCode: string;
  warehouseName: string;
  warehouseCodeName: string;
}

class Customer {
  customerId: string;
  customerName: string;
  customerCode: string;
  customerCodeName: string;
}

class VendorOrder {
  vendorOrderId: string;
  vendorOrderCode: string;
  vendorDescripton: string;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

interface NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}

interface Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

class Product {
  productId: string;
  productName: string;
  productCode: string;
  productCodeName: string;
  productUnitName: string;
}

@Component({
  selector: 'app-inventory-receiving-voucher-detail',
  templateUrl: './inventory-receiving-voucher-detail.component.html',
  styleUrls: ['./inventory-receiving-voucher-detail.component.css']
})
export class InventoryReceivingVoucherDetailComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  inventoryReceivingVoucherId: string;
  isDisabled: boolean = true;

  /* Cố định thanh chứa button đầu trang */
  fixed: boolean = false;
  isShow: boolean = true;
  colLeft: number = 8;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }
  /* End */

  /* Upload File */
  @ViewChild('fileUpload') fileUpload: FileUpload;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  colsFile: any;
  listFileUpload: Array<any> = [];
  /* End */

  /* Dòng thời gian */
  noteHistory: Array<Note> = [];
  defaultAvatar: string = '/assets/images/no-avatar.png';
  /* End */
  
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';

  /* Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  /* End */

  colsDonHangMua: any;
  colsPhieuXuatKho: any;
  colsKiemKe: any;
  colsDieuChuyen: any;
  colsNhapKhac: any;

  listItemDetail: Array<SanPhamPhieuNhapKhoModel> = [];
  selectedIndex: number = null;

  /*Dữ liệu*/
  listLoaiPhieuNhap: Array<LoaiPhieuNhap> = [
    {
      name: 'Nhập theo phiếu mua hàng',
      type: 1
    },
    {
      name: 'Nhập hàng bán bị trả lại',
      type: 2
    },
    {
      name: 'Nhập kiểm kê',
      type: 3
    },
    {
      name: 'Nhập điều chuyển',
      type: 4
    },
    {
      name: 'Nhập khác',
      type: 5
    }
  ];
  loaiPhieuNhapType: number = 1;
  listCurrentChips: Array<string> = [];
  listVendor: Array<Vendor> = [];
  listAllWarehouse: Array<Warehouse> = [];
  listWarehouse: Array<Warehouse> = [];
  listCustomer: Array<Customer> = [];
  listProduct: Array<Product> = [];
  listVendorOrder: Array<VendorOrder> = [];
  listSelectedVendorOrderId: Array<string> = [];
  inventoryReceivingVoucherCode: string = null; //Mã phiếu nhập kho
  statusName: string = null;  //Tên trạng thái
  statusCode: string = null;  //Mã trạng thái
  employeeCodeName: string = null;  //Người lập phiếu
  createdDate: Date = new Date(); //Ngày lập phiếu
  totalQuantityActual: number = 0;  //Tổng số lượng thực nhập
  /*End*/

  createForm: FormGroup;
  loaiPhieuNhapControl: FormControl;
  doiTacControl: FormControl;
  khoControl: FormControl;
  donHangMuaControl: FormControl;
  phieuXuatKhoChipsControl: FormControl;
  phieuKiemKeControl: FormControl;
  expectedDateControl: FormControl;
  inventoryReceivingVoucherDateControl: FormControl;
  shiperNameControl: FormControl;
  descriptionControl: FormControl;
  licenseNumberControl: FormControl;
  noteControl: FormControl;
  productControl: FormControl;

  /*Popup Kho list Kho con*/
  choiceKho: boolean = false;
  listDetailWarehouse: TreeNode[];
  selectedWarehouseChilren: TreeNode;
  /*End*/

  /*Popup cảnh báo*/
  openCanhBao: boolean = false;
  messageCanhBao: string = null;
  /*End*/

  constructor(
    private getPermission: GetPermission,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute,
    private router: Router,
    private _warehouseService: WarehouseService,
    private renderer: Renderer2,
    private ref: ChangeDetectorRef,
    private imageService: ImageUploadService,
    private folderService: ForderConfigurationService,
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

  ngOnInit(): void {
    this.initTable();
    this.setForm();

    let resource = "war/warehouse/inventory-receiving-voucher/detail";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.route.params.subscribe(params => {
        this.inventoryReceivingVoucherId = params['inventoryReceivingVoucherId'];
      });
      
      this.getMasterData(2);
    }
  }

  initTable() {
    this.colsDonHangMua = [
      { field: 'orderCode', header: 'Phiếu mua hàng', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'productCode', header: 'Mã SP', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'description', header: 'Diễn giải', width: '190px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'unitName', header: 'ĐV', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'warehouseCodeName', header: 'Vị trí', width: '190px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'quantityRequest', header: 'SL cần nhập', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityReservation', header: 'Giữ trước', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityActual', header: 'SL thực nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];

    this.colsPhieuXuatKho = [
      { field: 'orderCode', header: 'Phiếu xuất kho', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'productCode', header: 'Mã SP', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'description', header: 'Diễn giải', width: '190px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'unitName', header: 'ĐV', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'warehouseCodeName', header: 'Vị trí', width: '190px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'quantityRequest', header: 'SL cần nhập', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityReservation', header: 'Giữ trước', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityActual', header: 'SL thực nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'averagePrice', header: 'Giá trung bình', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'priceProduct', header: 'Giá nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];

    this.colsKiemKe = [
      { field: 'orderCode', header: 'Bảng kiểm kê', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'productCode', header: 'Mã SP', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'description', header: 'Diễn giải', width: '190px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'unitName', header: 'ĐV', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'warehouseCodeName', header: 'Vị trí', width: '190px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'quantityRequest', header: 'SL cần nhập', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityReservation', header: 'Giữ trước', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityActual', header: 'SL thực nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'averagePrice', header: 'Giá trung bình', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'priceProduct', header: 'Giá nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];

    this.colsDieuChuyen = [
      { field: 'orderCode', header: 'Phiếu xuất kho', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'productCode', header: 'Mã SP', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'description', header: 'Diễn giải', width: '190px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'unitName', header: 'ĐV', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'warehouseCodeName', header: 'Vị trí', width: '190px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'quantityRequest', header: 'SL cần nhập', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityReservation', header: 'Giữ trước', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityActual', header: 'SL thực nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'averagePrice', header: 'Giá trung bình', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'priceProduct', header: 'Giá nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];

    this.colsNhapKhac = [
      { field: 'productCode', header: 'Mã SP', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'description', header: 'Diễn giải', width: '190px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'unitName', header: 'ĐV', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'warehouseCodeName', header: 'Vị trí', width: '190px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'quantityRequest', header: 'SL cần nhập', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityReservation', header: 'Giữ trước', width: '120px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'quantityActual', header: 'SL thực nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'priceProduct', header: 'Giá nhập', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền', width: '150px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];

    this.colsFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'size', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];
  }

  setForm() {
    this.loaiPhieuNhapControl = new FormControl(null);
    this.doiTacControl = new FormControl(null, [Validators.required]);
    this.khoControl = new FormControl(null, [Validators.required]);
    this.donHangMuaControl = new FormControl({value: null, disabled: true}, [Validators.required]);
    this.phieuXuatKhoChipsControl = new FormControl(null);
    this.phieuKiemKeControl = new FormControl(null);
    this.expectedDateControl = new FormControl(null, [Validators.required]);
    this.inventoryReceivingVoucherDateControl = new FormControl(null);
    this.shiperNameControl = new FormControl(null);
    this.descriptionControl = new FormControl(null);
    this.licenseNumberControl = new FormControl('0');
    this.noteControl = new FormControl(null);
    this.productControl = new FormControl(null);
    
    this.createForm = new FormGroup({
      loaiPhieuNhapControl: this.loaiPhieuNhapControl,
      doiTacControl: this.doiTacControl,
      khoControl: this.khoControl,
      donHangMuaControl: this.donHangMuaControl,
      phieuXuatKhoChipsControl: this.phieuXuatKhoChipsControl,
      phieuKiemKeControl: this.phieuKiemKeControl,
      expectedDateControl: this.expectedDateControl,
      inventoryReceivingVoucherDateControl: this.inventoryReceivingVoucherDateControl,
      shiperNameControl: this.shiperNameControl,
      descriptionControl: this.descriptionControl,
      licenseNumberControl: this.licenseNumberControl,
      noteControl: this.noteControl,
      productControl: this.productControl
    });
  }

  /*
  * mode = 1: giữ nguyên giá trị isDisabled và form
  * mode = 2: isDisabled = true và disabled form
  */
  getMasterData(mode: number) {
    this.loading = true;
    this.awaitResult = true;
    this._warehouseService.getDetailPhieuNhapKho(this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listVendor = result.listVendor;
        this.listAllWarehouse = result.listAllWarehouse;
        this.listWarehouse = result.listWarehouse;
        this.listCustomer = result.listCustomer;
        this.listVendorOrder = result.listVendorOrder;
        this.listProduct = result.listProduct;
        this.listSelectedVendorOrderId = result.listSelectedVendorOrderId;
        this.listFileUpload = result.listFileUpload;
        this.noteHistory = result.noteHistory;
        this.handleNoteContent();
        this.setDataToForm(mode, result.phieuNhapKho, result.listItemDetail);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*
  * mode = 1: giữ nguyên giá trị isDisabled và form
  * mode = 2: isDisabled = true và disabled form
  */
  setDataToForm(mode: number, phieuNhapKho: PhieuNhapKho, listItemDetail: Array<SanPhamPhieuNhapKhoModel>) {
    //Loại phiếu nhập
    let loaiPhieuNhap = this.listLoaiPhieuNhap.find(x => x.type == phieuNhapKho.inventoryReceivingVoucherType);
    this.loaiPhieuNhapType = loaiPhieuNhap.type;
    this.loaiPhieuNhapControl.setValue(loaiPhieuNhap);

    //Đối tác (Nhà cung cấp)
    this.doiTacControl.setValue(this.listVendor.find(x => x.vendorId == phieuNhapKho.partnersId));

    //Nhập kho tại
    this.khoControl.setValue(this.listWarehouse.find(x => x.warehouseId == phieuNhapKho.warehouseId));

    //Ngày dự kiến nhập
    this.expectedDateControl.setValue(new Date(phieuNhapKho.expectedDate));

    //Ngày nhập kho
    this.inventoryReceivingVoucherDateControl.setValue(phieuNhapKho.inventoryReceivingVoucherDate == null ? null : new Date(phieuNhapKho.inventoryReceivingVoucherDate));

    //Họ và Tên người giao
    this.shiperNameControl.setValue(phieuNhapKho.shiperName);

    //Diễn giải
    this.descriptionControl.setValue(phieuNhapKho.description);

    //Mã phiếu
    this.inventoryReceivingVoucherCode = phieuNhapKho.inventoryReceivingVoucherCode;

    //Tên Trạng thái
    this.statusName = phieuNhapKho.statusName;

    //Mã trạng thái
    this.statusCode = phieuNhapKho.statusCode;

    //Người lập phiếu
    this.employeeCodeName = phieuNhapKho.employeeCodeName;

    //Ngày lập phiếu
    this.createdDate = new Date(phieuNhapKho.createdDate);

    //Tổng số lượng thực nhập
    this.totalQuantityActual = phieuNhapKho.totalQuantityActual;

    //Số chứng từ gốc kèm theo
    this.licenseNumberControl.setValue(phieuNhapKho.licenseNumber);

    //Ghi chú
    this.noteControl.setValue(phieuNhapKho.note);

    if (this.loaiPhieuNhapType == 1) {
      //Nhập theo phiếu mua hàng

      //List selected Đơn hàng mua
      let listSelectedDangHangMua = this.listVendorOrder.filter(x => this.listSelectedVendorOrderId.includes(x.vendorOrderId));
      this.donHangMuaControl.setValue(listSelectedDangHangMua);
    }
    else if (this.loaiPhieuNhapType == 2) {
      //Nhập hàng bán bị trả lại
      
    }
    else if (this.loaiPhieuNhapType == 3) {
      //Nhập kiểm kê

    }
    else if (this.loaiPhieuNhapType == 4) {
      //Nhập điều chuyển

    }
    else if (this.loaiPhieuNhapType == 5) {
      //Nhập khác

      this.donHangMuaControl.setValidators(null);
      this.donHangMuaControl.updateValueAndValidity();
    }

    this.listItemDetail = [];
    listItemDetail.forEach((item, index) => {
      let newItem = new SanPhamPhieuNhapKhoModel();
      newItem.index = index + 1;
      newItem.inventoryReceivingVoucherMappingId = item.inventoryReceivingVoucherMappingId;
      newItem.inventoryReceivingVoucherId = item.inventoryReceivingVoucherId;
      newItem.objectId = item.objectId;
      newItem.objectDetailId = item.objectDetailId;
      newItem.productId = item.productId;
      newItem.quantityRequest = item.quantityRequest;
      newItem.quantityReservation = item.quantityReservation;
      newItem.quantityActual = item.quantityActual.toString();
      newItem.priceAverage = item.priceAverage;
      newItem.priceProduct = item.priceProduct;
      newItem.amount = item.amount;
      newItem.warehouseId = item.warehouseId;
      newItem.orderCode = item.orderCode;
      newItem.productCode = item.productCode;
      newItem.description = item.description;
      newItem.unitName = item.unitName;
      newItem.warehouseName = item.warehouseName;
      newItem.warehouseCodeName = item.warehouseCodeName;

      this.listItemDetail = [...this.listItemDetail, newItem];
    });

    if (mode == 2) {
      /*Disabled form*/
      this.isDisabled = true;
      this.disabledForm();
      /*End*/
    }

    setTimeout(() => {
      this.awaitResult = false;
    }, 3000);
  }

  disabledForm() {
    this.createForm.disable();
  }

  /*Thay đổi loại phiếu nhập kho*/
  changeLoaiPhieuNhap() {
    //Thay đổi form theo Loại phiếu nhập
    let selectedLoaiPhieuNhap: LoaiPhieuNhap = this.loaiPhieuNhapControl.value;
    this.loaiPhieuNhapType = selectedLoaiPhieuNhap.type;
    this.ref.detectChanges();

    //reset list đối tác
    this.doiTacControl.reset();

    //reset list chọn kho
    this.khoControl.reset();

    //reset list đơn hàng mua
    this.listVendorOrder = [];
    this.donHangMuaControl.reset();

    //reset list phiếu kiểm kê
    this.phieuKiemKeControl.reset();

    //reset list phiếu xuất kho
    this.phieuXuatKhoChipsControl.reset();

    //reset list sản phẩm
    this.listItemDetail = [];
    this.getTotalQuantityActual();
    this.filterGlobal = '';
    this.myTable.reset();

    //thử dữ liệu mẫu
    // this.phieuXuatKhoChipsControl.setValue(['Ngọc', 'Lan', 'PO-2002110123', 'PO-2002110123']);
    if (this.loaiPhieuNhapType == 1) {
      //Nhập theo phiếu mua hàng
      
      this.donHangMuaControl.disable();
      this.donHangMuaControl.setValidators([Validators.required]);
      this.donHangMuaControl.updateValueAndValidity();
    }
    else if (this.loaiPhieuNhapType == 2) {
      //Nhập hàng bán bị trả lại
      this.donHangMuaControl.setValidators(null);
      this.donHangMuaControl.updateValueAndValidity();
    }
    else if (this.loaiPhieuNhapType == 3) {
      //Nhập kiểm kê
      this.donHangMuaControl.setValidators(null);
      this.donHangMuaControl.updateValueAndValidity();
    }
    else if (this.loaiPhieuNhapType == 4) {
      //Nhập điều chuyển
      this.donHangMuaControl.setValidators(null);
      this.donHangMuaControl.updateValueAndValidity();
    }
    else if (this.loaiPhieuNhapType == 5) {
      //Nhập khác
      this.donHangMuaControl.setValidators(null);
      this.donHangMuaControl.updateValueAndValidity();

      //Lấy listProduct
      this.loading = true;
      this._warehouseService.getListProductPhieuNhapKho().subscribe(response => {
        let result: any = response;
        this.loading = false;

        if (result.statusCode == 200) {
          this.listProduct = result.listProduct;
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Thay đổi Đối tác (Nhà cung cấp)*/
  changeDoiTac() {
    let vendor: Vendor = this.doiTacControl.value;

    //reset list chọn kho
    this.khoControl.reset();

    //reset list đơn hàng mua
    this.listVendorOrder = [];
    this.donHangMuaControl.reset();

    //reset list phiếu kiểm kê
    this.phieuKiemKeControl.reset();

    //reset list phiếu xuất kho
    this.phieuXuatKhoChipsControl.reset();

    //reset list sản phẩm
    this.listItemDetail = [];
    this.getTotalQuantityActual();
    this.filterGlobal = '';
    this.myTable.reset();

    if (vendor) {
      if (this.loaiPhieuNhapType == 1) {
        //Nhập theo phiếu mua hàng
  
        //Lấy list Đơn hàng mua có trạng thái Đơn hàng mua
        this._warehouseService.getVendorOrderByVendorId(vendor.vendorId).subscribe(response => {
          let result: any = response;
  
          if (result.statusCode == 200) {
            this.listVendorOrder = result.listVendorOrder;
          }
          else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
      else if (this.loaiPhieuNhapType == 2) {
        //Nhập hàng bán bị trả lại
        
      }
      else if (this.loaiPhieuNhapType == 3) {
        //Nhập kiểm kê
  
      }
      else if (this.loaiPhieuNhapType == 4) {
        //Nhập điều chuyển
  
      }
      else if (this.loaiPhieuNhapType == 5) {
        //Nhập khác
  
      }
    }
  }

  /*Thay đổi Kho*/
  changeNhapKhoTai() {
    //enable list đơn hàng mua
    this.donHangMuaControl.enable();

    //Thay đổi tất cả vị trí của danh sách sản phẩm
    if (this.listItemDetail.length > 0) {
      let warehouse: Warehouse = this.khoControl.value;

      this.listItemDetail.forEach(item => {
        item.warehouseId = warehouse.warehouseId;
        item.warehouseCodeName = warehouse.warehouseCodeName;

        //Nếu Kho chọn có Kho con thì highlight đỏ (hasChild=true), ngược lại không (hasChild=false)
        //item.error = warehouse.hasChild;
      });

      this.changeDonHangMua();
    }
  }

  /*Hiển thị dialog phiếu xuất kho: Bị trả lại hoặc Điều chuyển*/
  showDialogPhieuXuatKho(mode: string) {
    if (mode == 'tra_lai') {

    }
    else if (mode == 'dieu_chuyen') {

    }
  }

  /*Khi thêm chips bằng tay sẽ không cập nhật vào list chips*/
  addChips() {
    let listCurrentChips = ['Giang', 'Ngọc', 'Linh'];

    this.phieuXuatKhoChipsControl.setValue(listCurrentChips);
  }

  /*Thay đổi đơn hàng mua*/
  changeDonHangMua() {
    this.listItemDetail = [];
    this.getTotalQuantityActual();
    this.filterGlobal = '';
    this.myTable.reset();

    let listSelectedVendorOrder: Array<VendorOrder> = this.donHangMuaControl.value;

    //gán vị trí (Kho) cho sản phẩm
    let warehouse: Warehouse = this.khoControl.value;
    
    if (listSelectedVendorOrder.length > 0) {
      let listVendorOrderId = listSelectedVendorOrder.map(x => x.vendorOrderId);

      this._warehouseService.getDanhSachSanPhamCuaPhieu(listVendorOrderId, this.loaiPhieuNhapType, warehouse.warehouseId, this.inventoryReceivingVoucherId).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listItemDetail = result.listItemDetail;
      
          this.listItemDetail.forEach((item, index) => {
            item.index = index + 1;
            if (warehouse) {
              item.warehouseId = warehouse.warehouseId;
              item.warehouseCodeName = warehouse.warehouseCodeName;

              //Nếu Kho chọn có Kho con thì highlight đỏ (hasChild=true), ngược lại không (hasChild=false)
              //item.error = warehouse.hasChild;
            } else {
              //Nếu chưa chọn Kho thì highlight đỏ
              //item.error = true;
            }
          });

          this.getTotalQuantityActual();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Thay đổi phiếu kiểm kê*/
  changePhieuKiemKe() {

  }

  onViewDetail(data: any) {

  }

  /*Hiển thị popup list kho con*/
  thayDoiKho(data: SanPhamPhieuNhapKhoModel) {
    this.selectedIndex = data.index;
    this.selectedWarehouseChilren = null;

    let warehouse: Warehouse = this.khoControl.value;

    //Lấy list kho con nếu có
    this._warehouseService.getDanhSachKhoCon(warehouse.warehouseId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        let listWarehouse = result.listWarehouse;
        this.listDetailWarehouse = this.list_to_tree(listWarehouse, data.warehouseId);
        this.choiceKho = true;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  list_to_tree(listWarehouse: Array<Warehouse>, selectedId: string) {
    let list: Array<TreeNode> = [];
    listWarehouse.forEach(item => {
      let node: TreeNode = {
        label: item.warehouseCodeName,
        expanded: true,
        expandedIcon: "",
        collapsedIcon: "",
        data: {
          'warehouseId': item.warehouseId,
          'warehouseParent': item.warehouseParent,
          'hasChild': item.hasChild
        },
        children: []
      };

      list = [...list, node];
    });

    var map = {}, node, roots = [], i;

    for (i = 0; i < list.length; i += 1) {
      map[list[i].data.warehouseId] = i; // initialize the map
      list[i].children = []; // initialize the children
    }

    for (i = 0; i < list.length; i += 1) {
      node = list[i];
      if (node.data.warehouseParent !== null) {
        // if you have dangling branches check that map[node.parentId] exists
        list[map[node.data.warehouseParent]].children.push(node);
      } else {
        roots.push(node);
      }

      // Vì selected trong trường hợp này luôn có children = [] nên ta có thể xác định nó tại đây
      if (node.data.warehouseId == selectedId) {
        this.selectedWarehouseChilren = node;
      }
    }
    return roots;
  }

  /*event: khi click vào node (kho)*/
  choiceKhoCon(data: any) {
    if (data.node.children.length != 0) {
      this.selectedWarehouseChilren = null;
    }
  }

  /*Xác nhận Chọn kho con*/
  selectedKhoCon() {
    if (this.selectedWarehouseChilren) {
      let sanPhamPhieuNhapKho = this.listItemDetail.find(x => x.index == this.selectedIndex);
      sanPhamPhieuNhapKho.warehouseId = this.selectedWarehouseChilren.data.warehouseId;
      sanPhamPhieuNhapKho.warehouseCodeName = this.selectedWarehouseChilren.label;
      sanPhamPhieuNhapKho.error = this.selectedWarehouseChilren.data.hasChild;
      this.choiceKho = false;

      //Tính số giữ trước của sản phẩm
      // this._warehouseService.getSoGTCuaSanPhamTheoKho(sanPhamPhieuNhapKho.productId, this.selectedWarehouseChilren.data.warehouseId, sanPhamPhieuNhapKho.quantityRequest).subscribe(response => {
      //   let result: any = response;

      //   if (result.statusCode == 200) {
      //     sanPhamPhieuNhapKho.warehouseId = this.selectedWarehouseChilren.data.warehouseId;
      //     sanPhamPhieuNhapKho.warehouseCodeName = this.selectedWarehouseChilren.label;
      //     sanPhamPhieuNhapKho.quantityReservation = result.quantityReservation;
      //     sanPhamPhieuNhapKho.error = this.selectedWarehouseChilren.data.hasChild;
      //     this.choiceKho = false;
      //   }
      //   else {
      //     let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      //     this.showMessage(msg);
      //   }
      // });
    }
  }

  /*Hủy chọn kho con*/
  cancelSelectedKhoCon() {
    this.selectedIndex = null;
    this.selectedWarehouseChilren = null;
    this.choiceKho = false;
  }

  /*Thay đổi số lượng cần nhập*/
  changeQuantityRequest(rowData: SanPhamPhieuNhapKhoModel) {
    if (rowData.quantityRequest.toString() == '') {
      rowData.quantityRequest = 0;
    }
  }

  /*Thay đổi số lượng thực nhập*/
  changeQuantityActual(rowData: SanPhamPhieuNhapKhoModel) {
    if (rowData.quantityActual == '') {
      rowData.quantityActual = '0';
    }

    //Số lượng thực nhập luôn luôn <= Số lượng giữ trước
    let quantityActual = ParseStringToFloat(rowData.quantityActual.toString());

    if (quantityActual > rowData.quantityReservation) {
      rowData.quantityActual = rowData.quantityReservation.toString();
    }

    if (this.loaiPhieuNhapType == 1) {
      //Nhập theo phiếu mua hàng

      
    }
    else if (this.loaiPhieuNhapType == 2) {
      //Nhập hàng bán bị trả lại
      
    }
    else if (this.loaiPhieuNhapType == 3) {
      //Nhập kiểm kê

    }
    else if (this.loaiPhieuNhapType == 4) {
      //Nhập điều chuyển

    }
    else if (this.loaiPhieuNhapType == 5) {
      //Nhập khác

    }

    this.getTotalQuantityActual();
  }

  /*Thay đổi Giá nhập*/
  changePriceProduct(rowData: SanPhamPhieuNhapKhoModel) {
    if (rowData.priceProduct == '') {
      rowData.priceProduct = '0';
    }

    this.getTotalQuantityActual();
  }

  /*Thêm sản phẩm*/
  changeProduct(event: any) {
    let warehouse: Warehouse = this.khoControl.value;

    if (warehouse) {
      let product: Product = event.value;
      let total = this.listItemDetail.length;

      let newItem = new SanPhamPhieuNhapKhoModel();
      newItem.index = total + 1;
      newItem.warehouseId = warehouse.warehouseId;
      newItem.warehouseName = warehouse.warehouseName;
      newItem.warehouseCodeName = warehouse.warehouseCodeName;
      newItem.productId = product.productId;
      newItem.productCode = product.productCode;
      newItem.description = product.productName;
      newItem.unitName = product.productUnitName;
      newItem.quantityRequest = 0;
      newItem.quantityReservation = 0;
      newItem.quantityActual = '0';
      newItem.priceProduct = '0';
      newItem.amount = 0;

      this.listItemDetail = [...this.listItemDetail, newItem];

      //reset dropdown list
      this.productControl.reset();
    }
    else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn kho' };
      this.showMessage(msg);
    }
  }

  /*Thay đổi Số chứng từ gốc kèm theo*/
  changeLicenseNumber() {
    let licenseNumber = this.licenseNumberControl.value;

    if (licenseNumber == '') {
      this.licenseNumberControl.setValue('0');
    }
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  goBack() {
    this.router.navigate(['/warehouse/inventory-receiving-voucher/list']);
  }

  /*Enable Form*/
  switchDisabled() {
    if (this.statusCode == 'NHA') {
      this.isDisabled = false;
      this.createForm.enable();
    }
    else if (this.statusCode == 'CHO') {
      this.isDisabled = false;
      this.khoControl.enable();
      this.expectedDateControl.enable();
      this.inventoryReceivingVoucherDateControl.enable();
      this.shiperNameControl.enable();
      this.descriptionControl.enable();
    }
    else if (this.statusCode == 'SAS') {
      this.isDisabled = false;
      this.expectedDateControl.enable();
      this.inventoryReceivingVoucherDateControl.enable();
      this.shiperNameControl.enable();
      this.descriptionControl.enable();
    }
  }

  /*Sửa phiếu nhập kho*/
  suaPhieuNhapKho() {
    if (!this.createForm.valid) {
      Object.keys(this.createForm.controls).forEach(key => {
        if (this.createForm.controls[key].valid == false) {
          this.createForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.createForm.statusChanges.subscribe((validity: string) => {
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
    else if (this.listItemDetail.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa có sản phẩm' };
      this.showMessage(msg);
    } else {
      let inventoryReceivingVoucherModel = this.mapDataToModelPhieuNhapKho();
      let listInventoryReceivingVoucherMapping = this.mapDataToModelDanhSachSanPham();

      this._warehouseService.suaPhieuNhapKho(inventoryReceivingVoucherModel, listInventoryReceivingVoucherMapping).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.getMasterData(2);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Kiểm tra list sản phẩm*/
  checkListItemDetail(): boolean {
    let result = true;

    //Nếu có ít nhất 1 sản phẩm lỗi thì trả về false
    if (this.listItemDetail.find(x => x.error == true)) {
      result = false;
    }

    return result;
  }

  mapDataToModelPhieuNhapKho() {
    let inventoryReceivingVoucherModel = new InventoryReceivingVoucherModel();
    inventoryReceivingVoucherModel.inventoryReceivingVoucherId = this.inventoryReceivingVoucherId;
    inventoryReceivingVoucherModel.statusId = this.emptyGuid;
    inventoryReceivingVoucherModel.inventoryReceivingVoucherType = this.loaiPhieuNhapType;

    let kho: Warehouse = this.khoControl.value;
    inventoryReceivingVoucherModel.warehouseId = kho.warehouseId;

    inventoryReceivingVoucherModel.shiperName = this.shiperNameControl.value;
    inventoryReceivingVoucherModel.inventoryReceivingVoucherDate = this.inventoryReceivingVoucherDateControl.value ? convertToUTCTime(this.inventoryReceivingVoucherDateControl.value) : null;
    inventoryReceivingVoucherModel.inventoryReceivingVoucherTime = this.getTimeSpan(this.inventoryReceivingVoucherDateControl.value);
    inventoryReceivingVoucherModel.licenseNumber = ParseStringToFloat(this.licenseNumberControl.value);
    inventoryReceivingVoucherModel.createdById = this.emptyGuid;
    inventoryReceivingVoucherModel.createdDate = new Date();
    inventoryReceivingVoucherModel.expectedDate = convertToUTCTime(this.expectedDateControl.value);
    inventoryReceivingVoucherModel.description = this.descriptionControl.value;
    inventoryReceivingVoucherModel.note = this.noteControl.value;

    if (this.loaiPhieuNhapType == 2) {
      let customer: Customer = this.doiTacControl.value;
      inventoryReceivingVoucherModel.partnersId = customer.customerId;
    }
    else {
      let vendor: Vendor = this.doiTacControl.value;
      inventoryReceivingVoucherModel.partnersId = vendor.vendorId;
    }

    return inventoryReceivingVoucherModel;
  }

  mapDataToModelDanhSachSanPham() {
    let listInventoryReceivingVoucherMapping: Array<InventoryReceivingVoucherMapping> = [];

    this.listItemDetail.forEach(item => {
      let inventoryReceivingVoucherMapping = new InventoryReceivingVoucherMapping();
      inventoryReceivingVoucherMapping.inventoryReceivingVoucherMappingId = this.emptyGuid;
      inventoryReceivingVoucherMapping.inventoryReceivingVoucherId = this.inventoryReceivingVoucherId;
      inventoryReceivingVoucherMapping.objectId = item.objectId;
      inventoryReceivingVoucherMapping.objectDetailId = item.objectDetailId;
      inventoryReceivingVoucherMapping.productId = item.productId;
      inventoryReceivingVoucherMapping.quantityRequest = ParseStringToFloat(item.quantityRequest.toString());
      inventoryReceivingVoucherMapping.quantityActual = ParseStringToFloat(item.quantityActual.toString());
      inventoryReceivingVoucherMapping.quantitySerial = 0;
      inventoryReceivingVoucherMapping.priceProduct = ParseStringToFloat(item.priceProduct.toString());
      inventoryReceivingVoucherMapping.warehouseId = item.warehouseId;
      inventoryReceivingVoucherMapping.quantityReservation = item.quantityReservation;

      listInventoryReceivingVoucherMapping.push(inventoryReceivingVoucherMapping);
    });
    
    return listInventoryReceivingVoucherMapping;
  }

  /*Lấy giờ nhập kho*/
  getTimeSpan(date: Date) {
    let hours = date.getHours().toString();
    let minutes = date.getMinutes().toString();

    return hours + ':' + minutes;
  }

  resetForm() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
      this.isInvalidForm = false; //Ẩn icon-warning-active
    }

    //Loại phiếu nhập
    this.loaiPhieuNhapControl.setValue(this.listLoaiPhieuNhap[0]);

    //Đối tác
    this.doiTacControl.reset();

    //Nhập kho tại
    this.khoControl.reset();

    //Lấy dữ liệu từ đơn hàng mua
    this.donHangMuaControl.setValue(null);
    this.donHangMuaControl.disable();

    //Ngày dự kiến nhập
    this.expectedDateControl.setValue(new Date());

    //Ngày nhập kho
    this.inventoryReceivingVoucherDateControl.setValue(null);

    //Diễn giải
    this.descriptionControl.setValue(null);

    //Ngày lập phiếu
    this.createdDate = new Date();

    //Số chứng từ gốc kèm theo
    this.licenseNumberControl.setValue('0');

    //list sản phẩm
    this.listItemDetail = [];
    this.getTotalQuantityActual();
  }

  /*Xóa phiếu nhập kho*/
  xoaPhieuNhapKho() {
    this.confirmationService.confirm({
      message: `Dữ liệu sẽ không thể hoàn tác, bạn chắc chắn muốn xóa?`,
      accept: () => {
        this.awaitResult = true;
        this.loading = true;
        this._warehouseService.xoaPhieuNhapKho(this.inventoryReceivingVoucherId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.router.navigate(['/warehouse/inventory-receiving-voucher/list']);
          }
          else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /*Nhân bản phiếu nhập kho*/
  nhanBanPhieuNhapKho() {
    this.awaitResult = true;
    this._warehouseService.nhanBanPhieuNhapKho(this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: "Nhân bản thành công" };
        this.showMessage(mgs);
        this.router.navigate(['/warehouse/inventory-receiving-voucher/detail', { inventoryReceivingVoucherId: result.inventoryReceivingVoucherId }]);
        this.inventoryReceivingVoucherId = result.inventoryReceivingVoucherId;
        this.getMasterData(2);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Tính tổng số lượng thực nhập*/
  getTotalQuantityActual() {
    this.totalQuantityActual = this.listItemDetail.map(e => ParseStringToFloat(e.quantityActual)).reduce((a, b) => a + b, 0);
  
    if (this.loaiPhieuNhapType == 1) {
      //Nhập theo phiếu mua hàng
      
    }
    else if (this.loaiPhieuNhapType == 2) {
      //Nhập hàng bán bị trả lại
      
    }
    else if (this.loaiPhieuNhapType == 3) {
      //Nhập kiểm kê

    }
    else if (this.loaiPhieuNhapType == 4) {
      //Nhập điều chuyển

    }
    else if (this.loaiPhieuNhapType == 5) {
      //Nhập khác
      
      //Tính lại Thành tiền = Số lượng thực nhập * giá nhập
      this.listItemDetail.forEach(item => {
        let quantityActual = ParseStringToFloat(item.quantityActual.toString());
        let priceProduct = ParseStringToFloat(item.priceProduct.toString());

        item.amount = this.roundNumber(quantityActual * priceProduct, 2);
      });
    }
  }

  /* Làm tròn 1 số */
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

  /*Không giữ phần*/
  khongGiuPhan() {
    //Lưu list sản phấm và Cập nhật số giữ trước của tất cả sản phẩm = 0
    this.awaitResult = true;
    this._warehouseService.khongGiuPhanPhieuNhapKho(this.listItemDetail, this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.getMasterData(2);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Đặt về Nháp*/
  datVeNhap() {
    this.awaitResult = true;
    this._warehouseService.datVeNhapPhieuNhapKho(this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.getMasterData(2);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Kiểm tra khả dụng*/
  kiemTraKhaDung() {
    this.awaitResult = true;
    this._warehouseService.kiemTraKhaDungPhieuNhapKho(this.listItemDetail, this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.getMasterData(2);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Đánh dấu cần làm*/
  danhDauCanLamPhieuNhapKho() {
    let check = false;
    this.listItemDetail.forEach(item => {
      if (item.quantityReservation > 0) {
        check = true;
      }
    });

    //Nếu có ít nhất một sản phẩm có Số giữ trước > 0 thì chuyển sang Trạng thái Sẵn sàng (check == true)
    //Ngược lại nếu tất cả sản phẩm có Số giữ trước = 0 thì chuyển sang Trạng thái Chờ nhập (check == false)
    this._warehouseService.danhDauCanLamPhieuNhapKho(this.inventoryReceivingVoucherId, check).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.getMasterData(2);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Hủy phiếu nhập kho*/
  huyPhieuNhapKho() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy Phiếu nhập kho này?',
      accept: () => {
        this.awaitResult = true;
        this._warehouseService.huyPhieuNhapKho(this.inventoryReceivingVoucherId).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.getMasterData(2);
          }
          else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /*Nhập kho*/
  nhapKhoPhieuNhapKho() {
    let check = false;
    let count = 0;
    let countPass = 0;
    let total = this.listItemDetail.length;
    this.listItemDetail.forEach(item => {
      let warehouse = this.listAllWarehouse.find(x => x.warehouseId == item.warehouseId);
      item.error = warehouse.hasChild;
      if (warehouse.hasChild) {
        check = warehouse.hasChild;
        count++;
      }

      //Tính số sản phẩm hơp lệ
      if (ParseStringToFloat(item.quantityActual.toString()) > 0 && !warehouse.hasChild) {
        countPass++;
      }
    });

    if (total == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Không có sản phẩm để nhập kho' };
      this.showMessage(msg);
    }
    else if (count == total) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Tất cả sản phẩm đang không ở vị trí hợp lệ' };
      this.showMessage(msg);
    }
    else if (countPass == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Số lượng thực nhập tất cả sản phẩm đang bằng 0' };
      this.showMessage(msg);
    }
    else {
      if (check) {
        this.confirmationService.confirm({
          message: 'Có sản phẩm ở vị trí không hợp lệ, nếu đồng ý tiếp tục sản phẩm không hợp lệ sẽ bị xóa?',
          accept: () => {
            this.kiemTraNhapKho();
          }
        });
      }
      else {
        this.kiemTraNhapKho();
      }
    }
  }

  /*Kiểm tra nhập kho*/
  kiemTraNhapKho() {
    this.awaitResult = true;
    this._warehouseService.kiemTraNhapKho(this.listItemDetail, this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;
      
      if (result.statusCode == 200) {
        if (result.mode == 1) {
          this.getMasterData(2);
        }
        else if (result.mode == 2) {
          let listMaSanPhamKhongHopLe: Array<string> = result.listMaSanPhamKhongHopLe;
          this.messageCanhBao = listMaSanPhamKhongHopLe.toString();
          this.openCanhBao = true;
          
          this.getMasterData(2);
        }
        else if (result.mode == 3) {
          let listMaSanPhamKhongHopLe: Array<string> = result.listMaSanPhamKhongHopLe;
          this.messageCanhBao = listMaSanPhamKhongHopLe.toString();
          this.openCanhBao = true;
          this.awaitResult = false;
        }
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: any) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.folderService.deleteFile(file.fileInFolderId).subscribe(response => {
          let result: any = response;

          if (result.statusCode == 200) {
            this.listFileUpload = this.listFileUpload.filter(x => x.fileInFolderId != file.fileInFolderId);

            let msg = { severity: 'success', summary: 'Thông báo', detail: 'Xóa file thành công' };
            this.showMessage(msg);
          }
          else {
            let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInFolderId: string, name: string, fileExtension: string) {
    this.folderService.downloadFile(fileInFolderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        var binaryString = atob(result.fileAsBase64);
        var fileType = result.fileType;
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var idx = 0; idx < binaryLen; idx++) {
          var ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        var file = new Blob([bytes], { type: fileType });
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(file);
        } else {
          var fileURL = URL.createObjectURL(file);
          if (fileType.indexOf('image') !== -1) {
            window.open(fileURL);
          } else {
            var anchor = document.createElement("a");
            anchor.download = name.substring(0, name.lastIndexOf('_')) + "." + fileExtension;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event upload list file*/
  myUploader(event: any) {
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileInFolderId = this.emptyGuid;
      fileUpload.FileInFolder.folderId = this.emptyGuid;
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectId = this.inventoryReceivingVoucherId;
      fileUpload.FileInFolder.objectType = 'QLNK';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload);
    });

    this.folderService.uploadFileByFolderType("QLNK", listFileUploadModel, this.inventoryReceivingVoucherId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        this.listFileUpload = result.listFileInFolder;

        let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm file thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {
    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }
    // noteFile
    let shortcontent_file = $('#' + nodeid).find('.short-content-file');
    let fullcontent_file = $('#' + nodeid).find('.full-content-file');
    let continue_ = $('#' + nodeid).find('.continue')
    if (shortcontent_file.css("display") === "none") {
      continue_.css("display", "block");
      fullcontent_file.css("display", "none");
      shortcontent_file.css("display", "block");
    } else {
      continue_.css("display", "none");
      fullcontent_file.css("display", "block");
      shortcontent_file.css("display", "none");
    }
    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Thu nhỏ';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
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

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

}

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = String(str).replace(/,/g, '');
  return parseFloat(str);
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
}
