import { Component, OnInit, HostListener, ViewChild, ElementRef, Renderer2, ChangeDetectorRef } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import * as $ from 'jquery';
import { WarehouseService } from '../../../services/warehouse.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SanPhamPhieuNhapKhoModel } from '../../../models/sanPhamPhieuNhapKhoModel.model';
import { InventoryReceivingVoucherModel } from '../../../models/InventoryReceivingVoucher.model';
import { InventoryReceivingVoucherMapping } from '../../../models/inventoryReceivingVoucherMapping.model';
import { TreeNode } from 'primeng/api';
import { Table } from 'primeng/table';
import { FileUpload } from 'primeng/fileupload';
import { Router, ActivatedRoute } from '@angular/router';
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

class Product {
  productId: string;
  productName: string;
  productCode: string;
  productCodeName: string;
  productUnitName: string;
}

@Component({
  selector: 'app-inventory-receiving-voucher-create',
  templateUrl: './inventory-receiving-voucher-create.component.html',
  styleUrls: ['./inventory-receiving-voucher-create.component.css']
})
export class InventoryReceivingVoucherCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

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
  listWarehouse: Array<Warehouse> = [];
  listCustomer: Array<Customer> = [];
  listVendorOrder: Array<VendorOrder> = [];
  listProduct: Array<Product> = [];
  employeeCodeName: string = null;
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

  constructor(
    private getPermission: GetPermission,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
    private _warehouseService: WarehouseService,
    private ref: ChangeDetectorRef,
    private router: Router,
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

    let resource = "war/warehouse/inventory-receiving-voucher/create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.getMasterData();
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
  }

  setForm() {
    this.loaiPhieuNhapControl = new FormControl(null);
    this.doiTacControl = new FormControl(null, [Validators.required]);
    this.khoControl = new FormControl(null, [Validators.required]);
    this.donHangMuaControl = new FormControl({value: null, disabled: true}, [Validators.required]);
    this.phieuXuatKhoChipsControl = new FormControl(null);
    this.phieuKiemKeControl = new FormControl(null);
    this.expectedDateControl = new FormControl(null, [Validators.required]);
    this.inventoryReceivingVoucherDateControl = new FormControl(null, [Validators.required]);
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

  getMasterData() {
    this.loading = true;
    this._warehouseService.getMasterDataPhieuNhapKho().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listVendor = result.listVendor;
        this.listWarehouse = result.listWarehouse;
        this.listCustomer = result.listCustomer;
        this.employeeCodeName = result.employeeCodeName;

        this.setDefaultValue();
        this.loading = false;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValue() {
    //Loại phiếu nhập
    this.loaiPhieuNhapControl.setValue(this.listLoaiPhieuNhap[0]);

    //Ngày dự kiến nhập
    this.expectedDateControl.setValue(new Date());
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
    if (this.loaiPhieuNhapType == 1) {
      //Nhập theo phiếu mua hàng
      
      //enable list đơn hàng mua
      this.donHangMuaControl.enable();

      //Thay đổi tất cả vị trí của danh sách sản phẩm
      if (this.listItemDetail.length > 0) {
        this.thayDoiViTri();

        this.changeDonHangMua();
      }
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

      //Thay đổi tất cả vị trí của danh sách sản phẩm
      if (this.listItemDetail.length > 0) {
        this.thayDoiViTri();
      }
    }
  }

  /*Thay đổi tất cả vị trí của danh sách sản phẩm*/
  thayDoiViTri() {
    let warehouse: Warehouse = this.khoControl.value;

    this.listItemDetail.forEach(item => {
      item.warehouseId = warehouse.warehouseId;
      item.warehouseCodeName = warehouse.warehouseCodeName;

      //Nếu Kho chọn có Kho con thì highlight đỏ (hasChild=true), ngược lại không (hasChild=false)
      //item.error = warehouse.hasChild;
    });
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

      this._warehouseService.getDanhSachSanPhamCuaPhieu(listVendorOrderId, this.loaiPhieuNhapType, warehouse.warehouseId).subscribe(response => {
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

  /*Tạo mới phiếu nhập kho*/
  taoPhieuNhapKho(mode: boolean) {
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
    else if (!this.checkListItemDetail()) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Có sản phẩm không hợp lệ' };
      this.showMessage(msg);
    } else if (this.listItemDetail.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa có sản phẩm' };
      this.showMessage(msg);
    } else {
      let inventoryReceivingVoucherModel = this.mapDataToModelPhieuNhapKho();
      let listInventoryReceivingVoucherMapping = this.mapDataToModelDanhSachSanPham();

      this._warehouseService.createPhieuNhapKho(inventoryReceivingVoucherModel, listInventoryReceivingVoucherMapping, this.uploadedFiles).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          //Lưu và Thêm mới
          if (mode) {
            this.resetForm();
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm mới thành công' };
            this.showMessage(msg);
          }
          //Lưu
          else {
            //Chuyển đến trang chi tiết phiếu nhập kho
            this.router.navigate(['/warehouse/inventory-receiving-voucher/detail', { inventoryReceivingVoucherId: result.inventoryReceivingVoucherId }]);
          }
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
    inventoryReceivingVoucherModel.inventoryReceivingVoucherId = this.emptyGuid;
    inventoryReceivingVoucherModel.statusId = this.emptyGuid;
    inventoryReceivingVoucherModel.inventoryReceivingVoucherType = this.loaiPhieuNhapType;

    let kho: Warehouse = this.khoControl.value;
    inventoryReceivingVoucherModel.warehouseId = kho.warehouseId;

    inventoryReceivingVoucherModel.shiperName = this.shiperNameControl.value;
    inventoryReceivingVoucherModel.inventoryReceivingVoucherDate = this.inventoryReceivingVoucherDateControl.value ? this.inventoryReceivingVoucherDateControl.value : null;
    inventoryReceivingVoucherModel.inventoryReceivingVoucherTime = this.getTimeSpan(this.inventoryReceivingVoucherDateControl.value);
    inventoryReceivingVoucherModel.licenseNumber = ParseStringToFloat(this.licenseNumberControl.value);
    inventoryReceivingVoucherModel.createdById = this.emptyGuid;
    inventoryReceivingVoucherModel.createdDate = new Date();
    inventoryReceivingVoucherModel.expectedDate = this.expectedDateControl.value;
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
      inventoryReceivingVoucherMapping.inventoryReceivingVoucherId = this.emptyGuid;
      inventoryReceivingVoucherMapping.objectId = item.objectId;
      inventoryReceivingVoucherMapping.objectDetailId = item.objectDetailId;
      inventoryReceivingVoucherMapping.productId = item.productId;
      inventoryReceivingVoucherMapping.quantityRequest = item.quantityRequest;
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
    if (date) {
      let hours = date.getHours().toString();
      let minutes = date.getMinutes().toString();
  
      return hours + ':' + minutes;
    }
    else {
      return '00:00';
    }
  }

  resetForm() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
      this.isInvalidForm = false; //Ẩn icon-warning-active
    }

    //Loại phiếu nhập
    this.loaiPhieuNhapControl.setValue(this.listLoaiPhieuNhap[0]);
    this.loaiPhieuNhapType = 1;

    //Đối tác
    this.doiTacControl.reset();

    //Nhập kho tại
    this.khoControl.reset();

    //Lấy dữ liệu từ đơn hàng mua
    this.donHangMuaControl.setValue(null);
    this.donHangMuaControl.disable();
    this.donHangMuaControl.setValidators([Validators.required]);
    this.donHangMuaControl.updateValueAndValidity();

    //Ngày dự kiến nhập
    this.expectedDateControl.setValue(new Date());

    //Ngày nhập kho
    this.inventoryReceivingVoucherDateControl.reset();

    //Diễn giải
    this.descriptionControl.setValue(null);

    //Ngày lập phiếu
    this.createdDate = new Date();

    //Số chứng từ gốc kèm theo
    this.licenseNumberControl.setValue('0');

    //list sản phẩm
    this.listItemDetail = [];
    this.getTotalQuantityActual();

    //ghi chú
    this.noteControl.reset();

    //list file
    this.uploadedFiles = [];

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
