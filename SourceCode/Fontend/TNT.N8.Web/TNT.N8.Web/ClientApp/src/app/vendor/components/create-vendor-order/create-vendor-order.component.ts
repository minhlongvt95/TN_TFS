import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, HostListener, Renderer2 } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { VendorOrderModel } from '../../models/vendorOrder.model';
import { VendorOrderDetailModel } from '../../models/vendorOrderDetail.model';
import { VendorOrderProductDetailProductAttributeValueModel } from '../../models/vendorOrderProductDetailProductAttributeValue.model';
import { VendorService } from "../../services/vendor.service";
import { GetPermission } from '../../../shared/permission/get-permission';
import { AddVendorOrderProductComponent } from '../add-vendor-order-product/add-vendor-order-product.component';
import { PopupAddEditCostVendorOrderComponent } from '../../components/popup-add-edit-cost-vendor-order/popup-add-edit-cost-vendor-order.component';

import * as $ from 'jquery';

import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';

import { QuickCreateVendorComponent } from '../../../shared/components/quick-create-vendor/quick-create-vendor.component';

class DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface ConfigDialogOrderDetail {
  isCreate: boolean;
  vendor: vendorCreateOrderModel;
  vendorOrderDetail: VendorOrderDetailModel,
  isProcurementRequestItem: boolean;
  quantityApproval: number;
}

interface ResultDialog {
  status: boolean,
  vendorOrderDetail: VendorOrderDetailModel
}

class orderStatus {
  orderStatusId: string;
  orderStatusCode: string;
  description: string;
}

class employeeModel {
  employeeId: string;
  employeeName: string;
  employeeCode: string;
}

class paymentMethod {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class vendorCreateOrderModel {
  fullAddressVendor: string;
  listVendorContact: Array<vendorContact>;
  paymentId: string;
  vendorEmail: string;
  vendorId: string;
  vendorCode: string;
  vendorName: string;
  vendorPhone: string;
  constructor() {
    this.listVendorContact = [];
  }
}

class vendorContact {
  contactId: string;
  fullName: string;
  phone: string;
  email: string;
}

class bankAccount {
  bankAccountId: string;
  objectId: string;
  accountName: string; //chủ tài khoản
  accountNumber: string; //số tài khoản
  bankName: string; //tên ngân hàng
  branchName: string; //chi nhánh
}

class Warehouse {
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
}

interface CostModel {
  index: number,
  costId: string,
  costCode: string,
  costName: string,
  costCodeName: string,
  unitPrice: any,
  statusId: string,
  organizationId: string
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  costModel: CostModel,
}

class VendorOrderProcurementRequestMapping {
  vendorOrderProcurementRequestMappingId: string;
  vendorOrderId: string;
  procurementRequestId: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;

  constructor() {
    this.vendorOrderProcurementRequestMappingId = null;
    this.vendorOrderId = null;
    this.procurementRequestId = null;
    this.active = null;
    this.createdById = null;
    this.createdDate = null;
    this.updatedById = null;
    this.updatedDate = null;
  }
}

class VendorOrderCostDetail {
  vendorOrderCostDetailId: string;
  costId: string;
  vendorOrderId: string;
  unitPrice: number;
  costName: string;
  active: boolean;
  createdById: string;
  createdDate: Date;

  constructor() {
    this.vendorOrderCostDetailId = null;
    this.costId = null;
    this.vendorOrderId = null;
    this.unitPrice = null;
    this.costName = null;
    this.active = null;
    this.createdById = null;
    this.createdDate = null;
  }
}

@Component({
  selector: 'app-create-vendor-order',
  templateUrl: './create-vendor-order.component.html',
  styleUrls: ['./create-vendor-order.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class CreateVendorOrderComponent implements OnInit {
  /*Khai báo biến*/
  loading: boolean = false;
  awaitResult: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  auth: any = JSON.parse(localStorage.getItem("auth"));
  createPermission: string = "vendor-order/create";
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  actionAdd: boolean = true;
  fixed: boolean = false;
  withFiexd: string = "";
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
  //routing
  vendorId: string;
  procurementId: string;
  /* Form */
  createOrderForm: FormGroup;
  orderDateControl: FormControl;
  orderStatusControl: FormControl;
  ordererControl: FormControl;
  descriptionControl: FormControl;
  noteControl: FormControl;
  vendorControl: FormControl;
  vendorContactControl: FormControl;
  paymentControl: FormControl;
  bankAccountControl: FormControl;
  receivedDateControl: FormControl;
  receivedHourControl: FormControl;
  // recipientNameControl: FormControl;
  shipperNameControl: FormControl;
  locationOfShipmentControl: FormControl;
  placeOfDeliveryControl: FormControl;
  shippingNoteControl: FormControl;
  recipientPhoneControl: FormControl;
  recipientEmailControl: FormControl;
  discountTypeControl: FormControl;
  discountValueControl: FormControl;
  procurementRequestControl: FormControl;
  contractControl: FormControl;
  warehouseControl: FormControl;
  totalCostControl: FormControl;
  /* End */

  /* Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  /* End */
  listOrderStatus: Array<orderStatus> = [];
  listEmployee: Array<employeeModel> = [];
  listPaymentMethod: Array<paymentMethod> = [];
  listVendorCreateOrderModel: Array<vendorCreateOrderModel> = [];
  listBankAccount: Array<bankAccount> = [];
  listProcurementRequest: Array<any> = [];
  listCurrentVendorContact: Array<vendorContact> = [];
  listCurrentBankAccount: Array<bankAccount> = [];
  objProcurementRequest: any;
  listContract: Array<any> = [];
  listWareHouse: Array<any> = [];
  listCostModel: Array<CostModel> = [];
  listVendorOrderProcurementRequestMapping: Array<VendorOrderProcurementRequestMapping> = [];
  listVendorOrderCostDetail: Array<VendorOrderCostDetail> = [];

  optionCustomer: string = '2';
  listCustomer: Array<any> = [];
  customerEmail: string = '';
  customerPhone: string = '';
  fullAddress: string = '';
  leadName: string = '';
  leadEmail: string = '';
  leadPhone: string = '';
  leadFullAddress: string = '';

  //table
  cols: any[];
  selectedColumns: any[];
  selectedItem: any;

  colsCost: any[];
  selectedColumnsCost: any[];
  selectedItemCost: any;

  isBank: boolean = true;

  listVendorOrderDetail: Array<VendorOrderDetailModel> = [];
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];

  totalAmountBeforeVat: number = 0; //tổng thành tiền trước thuế
  totalAmountVat: number = 0; //Tổng tiền thuế
  totalAmountCost: number = 0; //Tổng tiền chi phí mua hàng
  totalAmountBeforeDiscount: number = 0;
  totalAmountAferDiscount: number = 0; //tổng tiền sau khi trừ chiết khấu
  discountPerOrder: number = 0; //chiết khấu theo đơn hàng
  dataRow: VendorOrderDetailModel = null;

  selectedCostType: string = '0';

  /*Data popup inline*/
  display: boolean = false;
  popupProductName: string = '';
  popupCost: any = 0;

  /*Data popup Tạo nhanh nhà cung cấp*/
  listVendorGroup: Array<any> = [];

  constructor(
    private ref: ChangeDetectorRef,
    private router: Router,
    private getPermission: GetPermission,
    private vendorService: VendorService,
    private route: ActivatedRoute,
    private el: ElementRef,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
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
    let resource = "buy/vendor/create-order/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }

      this.route.params.subscribe(params => {
        if (params['vendorId']) {
          //đi từ nhà cung cấp
          this.vendorId = params['vendorId'];
        }
        if (params['procurementId']) {
          //đi từ đề xuất mua hàng
          this.procurementId = params['procurementId'];
        }
      });

      this.setForm();
      this.setTable();
      this.getMasterData();
    }
  }

  setForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    this.orderDateControl = new FormControl(new Date(), [Validators.required]);
    this.orderStatusControl = new FormControl(null); //, [Validators.required]);
    this.ordererControl = new FormControl(null, [Validators.required]);
    this.descriptionControl = new FormControl(null);
    this.noteControl = new FormControl(null);
    this.vendorControl = new FormControl(null, [Validators.required]);
    this.vendorContactControl = new FormControl(null);
    this.paymentControl = new FormControl(null);
    this.bankAccountControl = new FormControl(null);
    this.receivedDateControl = new FormControl(new Date(), [Validators.required]);
    this.receivedHourControl = new FormControl(null);
    // this.recipientNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.shipperNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.locationOfShipmentControl = new FormControl(null);
    this.placeOfDeliveryControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.recipientPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern())]);
    this.recipientEmailControl = new FormControl(null, [Validators.pattern(emailPattern)]);
    this.shippingNoteControl = new FormControl(null);
    this.discountTypeControl = new FormControl(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl = new FormControl('0', ValidationMaxValuePT(100));
    this.procurementRequestControl = new FormControl([]);
    this.contractControl = new FormControl(null);
    this.warehouseControl = new FormControl(null);
    this.totalCostControl = new FormControl({ value: '0', disabled: true });

    this.createOrderForm = new FormGroup({
      orderDateControl: this.orderDateControl,
      orderStatusControl: this.orderStatusControl,
      ordererControl: this.ordererControl,
      descriptionControl: this.descriptionControl,
      noteControl: this.noteControl,
      vendorControl: this.vendorControl,
      vendorContactControl: this.vendorContactControl,
      paymentControl: this.paymentControl,
      bankAccountControl: this.bankAccountControl,
      receivedDateControl: this.receivedDateControl,
      receivedHourControl: this.receivedHourControl,
      // recipientNameControl: this.recipientNameControl,
      shipperNameControl: this.shipperNameControl,
      locationOfShipmentControl: this.locationOfShipmentControl,
      placeOfDeliveryControl: this.placeOfDeliveryControl,
      recipientPhoneControl: this.recipientPhoneControl,
      recipientEmailControl: this.recipientEmailControl,
      shippingNoteControl: this.shippingNoteControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl,
      procurementRequestControl: this.procurementRequestControl,
      contractControl: this.contractControl,
      warehouseControl: this.warehouseControl,
      totalCostControl: this.totalCostControl,
    });
  }

  resetForm() {
    this.createOrderForm.reset();

    this.orderDateControl.setValue((new Date()));
    this.orderStatusControl.setValue(null);
    let emp = this.listEmployee.find(c => c.employeeId == this.auth.EmployeeId);
    this.ordererControl.setValue(emp);
    this.descriptionControl.setValue(null);
    this.noteControl.setValue(null);
    this.vendorControl.setValue(null);
    this.vendorContactControl.setValue(null);
    this.paymentControl.setValue(null);
    this.bankAccountControl.setValue(null);
    this.receivedDateControl.setValue(new Date());
    this.receivedHourControl.setValue(null);
    // this.recipientNameControl.setValue(null);
    this.shipperNameControl.setValue(null);
    this.locationOfShipmentControl.setValue(null);
    this.placeOfDeliveryControl.setValue(null);
    this.placeOfDeliveryControl.setValue(null);
    this.recipientPhoneControl.setValue(null);
    this.recipientEmailControl.setValue(null);
    this.shippingNoteControl.setValue(null);
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl.setValue('0');
    this.procurementRequestControl.setValue([]);
    this.contractControl.setValue(null);
    this.warehouseControl.setValue(null);
    this.totalCostControl.setValue('0');
  }

  setTable() {
    this.cols = [
      { field: 'description', header: 'Tên hàng', width: '150px', textAlign: 'left', color: '#f44336' },
      // { field: 'vendorName', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '80px', textAlign: 'right', color: '#f44336' },
      { field: 'unitName', header: 'Đơn vị tính', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '140px', textAlign: 'right', color: '#f44336' },
      { field: 'currencyUnitName', header: 'Đơn vị tiền', width: '90px', textAlign: 'left', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '65px', textAlign: 'right', color: '#f44336' },
      { field: 'vat', header: 'Thuế GTGT (%)', width: '120px', textAlign: 'right', color: '#f44336' },
      { field: 'discountValue', header: 'Chiết khấu', width: '140px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền sau thuế (VND)', width: '200px', textAlign: 'right', color: '#f44336' },
      { field: 'cost', header: 'Chi phí mua hàng', width: '140px', textAlign: 'right', color: '#f44336' },
      { field: 'priceWarehouse', header: 'Giá nhập kho', width: '140px', textAlign: 'right', color: '#f44336' },
      { field: 'priceValueWarehouse', header: 'Giá trị nhập kho', width: '140px', textAlign: 'right', color: '#f44336' },
      { field: 'procurementCode', header: 'Phiếu đề xuất mua hàng', width: '200px', textAlign: 'center', color: '#f44336' },
      { field: 'delete', header: 'Chức năng', width: '100px', textAlign: 'center', color: '#f44336' },
    ];
    this.selectedColumns = this.cols;

    this.colsCost = [
      { field: 'costCode', header: 'Mã chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'costName', header: 'Tên chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Số tiền', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsCost = this.colsCost;
  }

  resetTable() {
    this.listVendorOrderDetail = [];
    this.listCostModel = [];
  }

  resetSumarySection() {
    this.totalAmountBeforeDiscount = 0;
    this.discountTypeControl.setValue(this.discountTypeList.find(x => x.code == "PT"));
    this.discountValueControl.setValue('0');
    this.totalAmountBeforeVat = 0;  //Tổng thành tiền trước thuế
    this.totalAmountVat = 0;  //Tổng tiền thuế
    this.totalAmountCost = 0; //Tổng tiền chi phí mua hàng
    this.discountPerOrder = 0;  //Tổng tiền chiết khấu
    this.totalAmountAferDiscount = 0; //Tổng thanh toán
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  cancel() {
    this.confirmationService.confirm({
      message: `Các thay đổi sẽ không được lưu lại. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn huỷ?`,
      accept: () => {
        this.router.navigate(['vendor/list-order']);
      }
    });
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    const currentVendor: vendorCreateOrderModel = this.vendorControl.value;
    // if (!currentVendor) {
    //   let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Chọn nhà cung cấp' };
    //   this.showMessage(msg);
    //   return false;
    // }

    let data: ConfigDialogOrderDetail = {
      isCreate: true,
      vendor: currentVendor,
      vendorOrderDetail: null,
      isProcurementRequestItem: false,
      quantityApproval: 0,
    }

    let ref = this.dialogService.open(AddVendorOrderProductComponent, {
      data: data,
      header: 'Thêm sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let vendorOrderDetail = result.vendorOrderDetail;
          vendorOrderDetail.orderNumber = this.listVendorOrderDetail.length + 1;
          this.listVendorOrderDetail = [...this.listVendorOrderDetail, vendorOrderDetail];

          this.allocation();
          this.getSumarySection();
          this.setValidatorForDiscount();
        }
      }
    });
  }

  /*Sửa một sản phẩm dịch vụ*/
  async onRowSelect(dataRow: VendorOrderDetailModel) {
    //Kiểm tra xem item có phải là từ đề xuất hay không
    let isProcurementRequestItem = dataRow.procurementRequestItemId ? true : false;
    let quantityApproval = 0;
    if (isProcurementRequestItem) {
      let result: any = await this.vendorService.getQuantityApproval(dataRow.vendorOrderDetailId, dataRow.procurementRequestItemId, dataRow.productId);
      // quantityApproval = dataRow.quantity;
      quantityApproval = result.quantityApproval;
    }
    const currentVendor: vendorCreateOrderModel = this.vendorControl.value;
    let data: ConfigDialogOrderDetail = {
      isCreate: false,
      vendor: currentVendor,
      vendorOrderDetail: dataRow,
      isProcurementRequestItem: isProcurementRequestItem,
      quantityApproval: quantityApproval,
    }

    let ref = this.dialogService.open(AddVendorOrderProductComponent, {
      data: data,
      header: 'Chỉnh sửa sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let vendorOrderDetail = result.vendorOrderDetail;
          let index = this.listVendorOrderDetail.indexOf(dataRow);
          if (index !== -1) {
            this.listVendorOrderDetail[index] = vendorOrderDetail;
            this.allocation();
            this.getSumarySection();
            this.setValidatorForDiscount();
          }
        }
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow: VendorOrderDetailModel, event: Event) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listVendorOrderDetail = this.listVendorOrderDetail.filter(e => e != dataRow);
        //Đánh lại số OrderNumber
        this.listVendorOrderDetail.forEach((item, index) => {
          item.orderNumber = index + 1;
        });
        this.allocation();
        this.getSumarySection();
        this.setValidatorForDiscount();

        //Kiểm tra list sản phẩm trong phiếu đề xuất đã bị xóa hết trong danh sách sp/dv chưa?
        if (dataRow.procurementRequestId) {
          let listSelected = this.procurementRequestControl.value;
          if (listSelected.length > 0) {
            let hasValue = this.listVendorOrderDetail.find(x => x.procurementRequestId == dataRow.procurementRequestId);

            //Nếu không còn sản phẩm nào trong phiếu đề xuất thì bỏ checked trong listSelected
            if (!hasValue) {
              listSelected = listSelected.filter(x => x.procurementRequestId != dataRow.procurementRequestId);
              this.procurementRequestControl.setValue(listSelected);
            }
          }
        }
      }
    });
  }

  patchVendorData(vendorId: string) {
    this.listCurrentVendorContact = [];
    this.vendorContactControl.reset();
    let currentVendor: vendorCreateOrderModel = this.listVendorCreateOrderModel.find(e => e.vendorId == vendorId);

    this.vendorControl.setValue(currentVendor);

    this.listCurrentVendorContact = currentVendor.listVendorContact;
    if (this.listCurrentVendorContact.length === 1) {
      this.vendorContactControl.setValue(this.listCurrentVendorContact[0]);
    }

    let payment = this.listPaymentMethod.find(e => e.categoryId == currentVendor.paymentId);
    if (payment) {
      this.paymentControl.setValue(payment);
      if (payment.categoryCode == "BANK") {
        //show tài khoản ngân hàng
        this.listCurrentBankAccount = this.listBankAccount.filter(e => e.objectId == currentVendor.vendorId);
        if (this.listCurrentBankAccount.length == 1) {
          this.bankAccountControl.setValue(this.listCurrentBankAccount[0]);
        }
      }
    }
  }

  /* events */
  changeVendor(event: any): boolean {
    this.listCurrentVendorContact = [];
    // this.listVendorOrderDetail = [];
    this.vendorContactControl.reset();
    if (event.value === null) return false;
    let currentVendor: vendorCreateOrderModel = event.value;
    this.listCurrentVendorContact = currentVendor.listVendorContact;
    if (this.listCurrentVendorContact.length === 1) {
      this.vendorContactControl.setValue(this.listCurrentVendorContact[0]);
    }

    let payment = this.listPaymentMethod.find(e => e.categoryId == currentVendor.paymentId);
    if (payment) {
      this.paymentControl.setValue(payment);
      if (payment.categoryCode == "BANK") {
        this.isBank = true;
        //show tài khoản ngân hàng
        this.listCurrentBankAccount = this.listBankAccount.filter(e => e.objectId == currentVendor.vendorId);
        if (this.listCurrentBankAccount.length == 1) {
          this.bankAccountControl.setValue(this.listCurrentBankAccount[0]);
        }
      }else{
        this.isBank = false;
      }
    }
    return false;
  }

  changePaymentMethod(event: any): boolean {
    this.listCurrentBankAccount = [];
    this.bankAccountControl.reset();
    if (event.value === null) return false;
    let currentPaymentMethod: paymentMethod = event.value;
    let currentVendor: vendorCreateOrderModel = this.vendorControl.value;
    if (currentPaymentMethod.categoryCode == "BANK") {
      this.isBank = true;
      if (this.vendorControl.value) {
        //show tài khoản ngân hàng
        this.listCurrentBankAccount = this.listBankAccount.filter(e => e.objectId == currentVendor.vendorId);
        if (this.listCurrentBankAccount.length == 1) {
          this.bankAccountControl.setValue(this.listCurrentBankAccount[0]);
        }
      }
    }
    else {
      this.isBank = false;
    }
    return false;
  }

  changeDiscountType(event: any) {
    this.discountValueControl.reset();
    this.discountValueControl.setValue('0');
    this.setValidatorForDiscount();
    this.getSumarySection();
  }

  changeDiscountValue(event: any) {
    this.getSumarySection();
  }
  /* end events */

  async getMasterData() {
    this.loading = true;
    let result: any = await this.vendorService.getDataCreateVendorOrder(this.auth.UserId);
    this.loading = false;

    if (result.statusCode == 200) {
      this.listEmployee = result.listEmployeeModel;
      this.listOrderStatus = result.listOrderStatus;
      this.listPaymentMethod = result.listPaymentMethod;
      this.listVendorCreateOrderModel = result.vendorCreateOrderModel;
      this.listVendorCreateOrderModel.forEach(item => {
        item.vendorName = item.vendorCode + ' - ' + item.vendorName;
      });
      this.listBankAccount = result.listBankAccount;
      this.listProcurementRequest = result.listProcurementRequest;
      this.listWareHouse = result.listWareHouse;
      this.listVendorGroup = result.listVendorGroup;
      this.setDefaultFormValues();

      if (this.vendorId) this.patchVendorData(this.vendorId);
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  setDefaultFormValues() {
    // set user đăng nhập là người tạo
    let emp = this.listEmployee.find(c => c.employeeId == this.auth.EmployeeId);
    this.ordererControl.setValue(emp);
    if (this.procurementId !== null && this.procurementId !== undefined && this.procurementId !== '') {
      this.objProcurementRequest = this.listProcurementRequest.find(p => p.procurementRequestId == this.procurementId);

      if (this.objProcurementRequest !== null && this.objProcurementRequest !== undefined) {
        this.procurementRequestControl.setValue([this.objProcurementRequest]);
        let vendorId = null;
        this.objProcurementRequest.listDetail.forEach(order => {
          let vendorOrderDetail: VendorOrderDetailModel = new VendorOrderDetailModel();
          vendorOrderDetail.vendorOrderDetailId = this.emptyGuid;
          vendorOrderDetail.vendorOrderId = this.emptyGuid;
          vendorOrderDetail.description = order.productName;
          vendorOrderDetail.vendorId = order.vendorId;
          vendorOrderDetail.vendorName = order.vendorName;
          vendorOrderDetail.productId = order.productId;
          vendorOrderDetail.quantity = order.quantityApproval ? order.quantityApproval : order.quantity;
          vendorOrderDetail.unitName = order.unitName;
          vendorOrderDetail.unitPrice = order.unitPrice;
          vendorOrderDetail.productUnitId = order.productUnitId;
          vendorOrderDetail.incurredUnit = null;
          vendorOrderDetail.currencyUnit = order.currencyUnit;
          vendorOrderDetail.currencyUnitName = order.currencyUnitName;
          vendorOrderDetail.exchangeRate = (order.exchangeRate === null || order.exchangeRate === undefined) ? 1 : order.exchangeRate;
          vendorOrderDetail.vat = 0;
          vendorOrderDetail.discountType = false;
          vendorOrderDetail.discountValue = 0;
          vendorOrderDetail.sumAmountDiscount = 0;
          vendorOrderDetail.sumAmountVat = 0;
          vendorOrderDetail.sumAmount = (order.quantityApproval ? order.quantityApproval : order.quantity) * order.unitPrice * vendorOrderDetail.exchangeRate;
          vendorOrderDetail.productName = order.productName;
          vendorOrderDetail.selectedAttributeName = '';
          vendorOrderDetail.orderDetailType = order.orderDetailType;
          vendorOrderDetail.isReshow = true;
          vendorOrderDetail.procurementRequestItemId = order.procurementRequestItemId;
          vendorOrderDetail.procurementRequestId = order.procurementRequestId;
          vendorOrderDetail.procurementCode = order.procurementCode;

          this.listVendorOrderDetail.push(vendorOrderDetail);

          if (vendorId === null && order.vendorId !== null && order.vendorId !== undefined && order.vendorId !== '') {
            vendorId = order.vendorId;
          }
        });
        if (vendorId !== null) {
          let objVendor = this.listVendorCreateOrderModel.find(v => v.vendorId === vendorId);
          this.vendorControl.setValue(objVendor);
        }
        this.allocation();
        this.getSumarySection();
      }
    }
  }

  async createOrder(type: boolean) {
    if (!this.createOrderForm.valid) {
      Object.keys(this.createOrderForm.controls).forEach(key => {
        if (this.createOrderForm.controls[key].valid == false) {
          this.createOrderForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.createOrderForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else if (this.listVendorOrderDetail.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Phải có ít nhất một sản phẩm dịch vụ được chọn' };
      this.showMessage(msg);
    } else {
      //valid
      let vendorOrder = this.mapFormtoVendorOrder();
      let listVendorOrderDetail = this.mapOrderDetail();

      this.listVendorOrderProcurementRequestMapping = [];
      let listSelectedProcurementRequest = this.procurementRequestControl.value;

      if (listSelectedProcurementRequest.length > 0) {
        listSelectedProcurementRequest.forEach(item => {
          let vendorOrderProcurementRequestMapping = new VendorOrderProcurementRequestMapping();

          vendorOrderProcurementRequestMapping.vendorOrderProcurementRequestMappingId = this.emptyGuid;
          vendorOrderProcurementRequestMapping.vendorOrderId = this.emptyGuid;
          vendorOrderProcurementRequestMapping.procurementRequestId = item.procurementRequestId;
          vendorOrderProcurementRequestMapping.active = true;
          vendorOrderProcurementRequestMapping.createdById = this.auth.UserId;
          vendorOrderProcurementRequestMapping.createdDate = new Date();

          this.listVendorOrderProcurementRequestMapping = [...this.listVendorOrderProcurementRequestMapping, vendorOrderProcurementRequestMapping];
        });
      }

      this.listVendorOrderCostDetail = [];
      if (this.listCostModel.length > 0) {
        this.listCostModel.forEach(item => {
          let vendorOrderCostDetail = new VendorOrderCostDetail();

          vendorOrderCostDetail.vendorOrderCostDetailId = this.emptyGuid;
          vendorOrderCostDetail.costId = item.costId;
          vendorOrderCostDetail.vendorOrderId = this.emptyGuid;
          vendorOrderCostDetail.unitPrice = item.unitPrice;
          vendorOrderCostDetail.costName = item.costName;
          vendorOrderCostDetail.active = true;
          vendorOrderCostDetail.createdById = this.auth.UserId;
          vendorOrderCostDetail.createdDate = new Date();

          this.listVendorOrderCostDetail = [...this.listVendorOrderCostDetail, vendorOrderCostDetail];
        });
      }

      let countVendor = this.listVendorOrderDetail.filter(v => v.vendorId == vendorOrder.vendorId);
      if (countVendor.length !== this.listVendorOrderDetail.length) {
        var objectVendorId = countVendor.map(c => c.vendorId);
        var lstObj = this.listVendorOrderDetail.filter(c => objectVendorId.indexOf(c.vendorId));
        var messageProduct = ``;

        lstObj.forEach(item => {
          if (item.description !== null && item.description !== undefined && item.description !== '') {
            messageProduct = messageProduct + `<div style="padding-left: 30px;"> - ` + item.description + `</div>`;
          }
        });

        this.confirmationService.confirm({
          message: `Các sản phẩm dịch vụ: <br/>` + messageProduct +
            `<br/>Đang được bán bởi nhà cung cấp khác nhà cung cấp đã chọn!<br/>Bạn có chắc chắn muốn tạo đơn đặt hàng này?`,
          accept: async () => {
            this.awaitResult = true;
            let result: any = await this.vendorService.createVendorOrderAsync(vendorOrder, listVendorOrderDetail,
              this.listVendorOrderProcurementRequestMapping, this.listVendorOrderCostDetail);
            this.loading = false;
            if (result.statusCode == 200 || result.statusCode == 202) {
              if (type == true) {
                this.resetForm();
                this.resetTable();
                this.resetSumarySection();
                this.awaitResult = false;
                let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng mua thành công' };
                this.showMessage(msg);
              } else {
                let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng mua thành công' };
                this.showMessage(msg);
                setTimeout(time => {
                  this.router.navigate(['/vendor/detail-order', { vendorOrderId: result.vendorOrderId }]);
                }, 2000);
              }
            } else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          }
        });
      }
      else {
        this.awaitResult = true;
        let result: any = await this.vendorService.createVendorOrderAsync(vendorOrder, listVendorOrderDetail,
          this.listVendorOrderProcurementRequestMapping, this.listVendorOrderCostDetail);
        this.loading = false;
        if (result.statusCode == 200 || result.statusCode == 202) {
          if (type == true) {
            this.resetForm();
            this.resetTable();
            this.resetSumarySection();
            this.awaitResult = false;
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng mua thành công' };
            this.showMessage(msg);
          } else {
            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng mua thành công' };
            this.showMessage(msg);
            setTimeout(time => {
              this.router.navigate(['/vendor/detail-order', { vendorOrderId: result.vendorOrderId }]);
            }, 2000);
          }
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      }
    }
  }

  mapFormtoVendorOrder(): VendorOrderModel {
    let vendorOrderModel: VendorOrderModel = new VendorOrderModel();
    vendorOrderModel.vendorOrderId = this.emptyGuid;
    vendorOrderModel.statusId = this.emptyGuid;
    vendorOrderModel.vendorOrderCode = '';
    let orderDate = this.orderDateControl.value;
    vendorOrderModel.vendorOrderDate = orderDate ? convertToUTCTime(orderDate) : null; // new Date();
    vendorOrderModel.customerOrderId = null;

    //người order
    let orderer: employeeModel = this.ordererControl.value;
    vendorOrderModel.orderer = orderer ? orderer.employeeId : null;
    vendorOrderModel.description = this.descriptionControl.value;
    vendorOrderModel.note = this.noteControl.value;

    //nhà cung cấp
    let vendor: vendorCreateOrderModel = this.vendorControl.value;
    vendorOrderModel.vendorId = vendor ? vendor.vendorId : null;

    //thông tin liên hệ nhà cung cấp
    let vendorContact: vendorContact = this.vendorContactControl.value;
    vendorOrderModel.vendorContactId = vendorContact ? vendorContact.contactId : null;

    //thông tin thanh toán nhà cung cấp
    let payment: paymentMethod = this.paymentControl.value;
    vendorOrderModel.paymentMethod = payment ? payment.categoryId : null;
    let bankAccount: bankAccount = this.bankAccountControl.value;
    vendorOrderModel.bankAccountId = bankAccount ? bankAccount.bankAccountId : null;

    //ngày giờ nhận
    let receivedDate = this.receivedDateControl.value;
    vendorOrderModel.receivedDate = receivedDate ? convertToUTCTime(receivedDate) : null;
    let receivedHour = this.receivedHourControl.value;
    if (receivedHour) {
      receivedHour = convertDateToTimeSpan(receivedHour);
      vendorOrderModel.receivedHour = receivedHour;
    }

    //thông tin nhận
    // vendorOrderModel.recipientName = this.recipientNameControl.value;
    vendorOrderModel.shipperName = this.shipperNameControl.value;
    vendorOrderModel.locationOfShipment = this.locationOfShipmentControl.value;
    vendorOrderModel.placeOfDelivery = this.placeOfDeliveryControl.value;
    vendorOrderModel.recipientPhone = this.recipientPhoneControl.value;
    vendorOrderModel.recipientEmail = this.recipientEmailControl.value;
    vendorOrderModel.shippingNote = this.shippingNoteControl.value;

    //tính tổng tiền sản phẩm đơn hàng
    let amountOrder = this.calculateAmountOrder();
    vendorOrderModel.amount = amountOrder;
    let discountType: DiscountType = this.discountTypeControl.value;
    vendorOrderModel.discountType = discountType.value;
    vendorOrderModel.discountValue = ParseStringToFloat(this.discountValueControl.value);
    vendorOrderModel.createdById = this.auth.UserId;
    vendorOrderModel.createdDate = new Date();
    vendorOrderModel.active = true;

    vendorOrderModel.contractId = this.contractControl.value == null ? null : this.contractControl.value.contractId;  //Cần sửa tên:: contractId

    //Mã kho
    let warehouse: Warehouse = this.warehouseControl.value;
    vendorOrderModel.warehouseId = warehouse == null ? null : warehouse.warehouseId;

    //Loại chi phí
    vendorOrderModel.typeCost = this.selectedCostType;

    return vendorOrderModel;
  }

  calculateAmountOrder(): number {
    let result: number = 0;
    let totalAmountBeforeVat = 0; //Tổng tiền trước thuế
    let totalAmountVat = 0; //Tổng tiền thuế
    let totalAmountCost = ParseStringToFloat(this.totalCostControl.value.toString()); //Tổng chi phí mua hàng
    let discountPerOrder = 0;

    this.listVendorOrderDetail.forEach(e => {
      //Tổng tiền trước thuế
      if (e.discountType == true) {
        let discountValue = 0;
        discountValue = (e.quantity * e.unitPrice * e.exchangeRate) * e.discountValue / 100;
        //Tổng tiền trước thuế
        totalAmountBeforeVat += (e.quantity * e.unitPrice * e.exchangeRate) - discountValue;

        //Tổng tiền thuế
        totalAmountVat += ((e.quantity * e.unitPrice * e.exchangeRate - discountValue) * e.vat / 100);
      } else {
        //Tổng tiền trước thuế
        totalAmountBeforeVat += e.quantity * e.unitPrice * e.exchangeRate - e.discountValue;

        //Tổng tiền thuế
        totalAmountVat += ((e.quantity * e.unitPrice * e.exchangeRate - e.discountValue) * e.vat / 100);
      }
    });

    let discountType: DiscountType = this.discountTypeControl.value;
    let discountValue = ParseStringToFloat(this.discountValueControl.value);

    //Tổng tiền chiết khấu
    if (discountType.value == true) {
      discountPerOrder = ((totalAmountBeforeVat + totalAmountVat) * discountValue) / 100;
    } else {
      discountPerOrder = discountValue;
    }

    //Tổng thanh toán
    result = totalAmountBeforeVat + totalAmountVat + totalAmountCost - discountPerOrder;

    return result;
  }

  getSumarySection() {
    let result: number = 0;
    this.totalAmountBeforeVat = 0; //Tổng tiền trước thuế
    this.totalAmountVat = 0; //Tổng tiền thuế
    this.totalAmountCost = ParseStringToFloat(this.totalCostControl.value.toString()); //Tổng chi phí mua hàng

    this.listVendorOrderDetail.forEach(e => {
      if (e.discountType == true) {
        let discountValue = 0;
        discountValue = (e.quantity * e.unitPrice * e.exchangeRate) * e.discountValue / 100;
        //Tổng tiền trước thuế
        this.totalAmountBeforeVat += (e.quantity * e.unitPrice * e.exchangeRate) - discountValue;

        //Tổng tiền thuế
        this.totalAmountVat += ((e.quantity * e.unitPrice * e.exchangeRate - discountValue) * e.vat / 100);
      } else {
        //Tổng tiền trước thuế
        this.totalAmountBeforeVat += e.quantity * e.unitPrice * e.exchangeRate - e.discountValue;

        //Tổng tiền thuế
        this.totalAmountVat += ((e.quantity * e.unitPrice * e.exchangeRate - e.discountValue) * e.vat / 100);
      }
    });

    result = this.totalAmountBeforeVat + this.totalAmountVat;

    let discountType: DiscountType = this.discountTypeControl.value;
    let discountValue = ParseStringToFloat(this.discountValueControl.value);

    //Tổng tiền chiết khấu
    if (discountType.value == true) {
      this.discountPerOrder = (result * discountValue) / 100;
    } else {
      this.discountPerOrder = discountValue;
    }

    //Tổng thanh toán
    this.totalAmountAferDiscount = result + this.totalAmountCost - this.discountPerOrder;
  }

  mapOrderDetail(): Array<VendorOrderDetailModel> {
    let selectedVendor: vendorCreateOrderModel = this.vendorControl.value;
    let listVendorOrderDetail: Array<VendorOrderDetailModel> = [];
    this.listVendorOrderDetail.forEach(order => {
      let vendorOrderDetail: VendorOrderDetailModel = new VendorOrderDetailModel();
      vendorOrderDetail.vendorOrderDetailId = this.emptyGuid;
      vendorOrderDetail.vendorId = selectedVendor.vendorId;
      vendorOrderDetail.vendorOrderId = this.emptyGuid;
      vendorOrderDetail.productId = order.productId;
      vendorOrderDetail.quantity = order.quantity;
      vendorOrderDetail.unitPrice = order.unitPrice;
      vendorOrderDetail.currencyUnit = order.currencyUnit;
      vendorOrderDetail.exchangeRate = order.exchangeRate;
      vendorOrderDetail.vat = order.vat;
      vendorOrderDetail.discountType = order.discountType;
      vendorOrderDetail.discountValue = order.discountValue;
      vendorOrderDetail.unitId = order.productUnitId;
      vendorOrderDetail.incurredUnit = order.incurredUnit;
      if (order.orderDetailType == 0) {
        vendorOrderDetail.description = '';
      } else {
        vendorOrderDetail.description = order.description;
      }
      vendorOrderDetail.orderDetailType = order.orderDetailType;
      vendorOrderDetail.orderNumber = order.orderNumber;
      vendorOrderDetail.createdById = this.auth.UserId;
      vendorOrderDetail.createdDate = new Date();
      vendorOrderDetail.active = true;

      vendorOrderDetail.procurementRequestId = order.procurementRequestId;
      vendorOrderDetail.procurementRequestItemId = order.procurementRequestItemId;
      vendorOrderDetail.cost = order.cost;
      vendorOrderDetail.priceWarehouse = order.priceWarehouse;
      vendorOrderDetail.priceValueWarehouse = order.priceValueWarehouse;
      vendorOrderDetail.isEditCost = order.isEditCost == true ? order.isEditCost : false;

      //map thuộc tính
      vendorOrderDetail.vendorOrderProductDetailProductAttributeValue = [];
      order.listVendorOrderProductDetailProductAttributeValue.forEach(_productAttr => {
        let productAttr: VendorOrderProductDetailProductAttributeValueModel = new VendorOrderProductDetailProductAttributeValueModel();
        productAttr.vendorOrderDetailId = this.emptyGuid;
        productAttr.productId = order.productId;
        productAttr.vendorOrderProductDetailProductAttributeValueId = this.emptyGuid
        productAttr.productAttributeCategoryId = _productAttr.productAttributeCategoryId;
        productAttr.productAttributeCategoryValueId = _productAttr.productAttributeCategoryValueId;
        vendorOrderDetail.vendorOrderProductDetailProductAttributeValue.push(productAttr);
      });
      listVendorOrderDetail.push(vendorOrderDetail);
    });

    return listVendorOrderDetail;
  }

  setValidatorForDiscount() {
    let discountType: DiscountType = this.discountTypeControl.value;
    if (discountType.value == true) {
      //phần trăm
      this.discountValueControl.setValidators([ValidationMaxValuePT(100)]);
    } else {
      //số tiền
      let totalAmountBeforeDiscount = this.calculateAmountOrder();
      this.discountValueControl.setValidators([ValidationMaxValueST(totalAmountBeforeDiscount)]);
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  /* Event thay đổi Phiếu đề xuất mua hàng */
  changeProcurementRequest(event: any) {
    let listSelected = event.value;
    if (listSelected.length > 0) {
      let listProductDetail = [];
      listSelected.forEach(item => {
        if (item.listDetail.length > 0) {
          item.listDetail.forEach(_product => {
            listProductDetail = [...listProductDetail, _product];
          });
        }
      });

      if (listProductDetail.length > 0) {
        let listProductByRequest: Array<VendorOrderDetailModel> = [];

        listProductDetail.forEach(order => {
          let vendorOrderDetail: VendorOrderDetailModel = new VendorOrderDetailModel();
          vendorOrderDetail.vendorOrderDetailId = this.emptyGuid;
          vendorOrderDetail.vendorOrderId = this.emptyGuid;
          vendorOrderDetail.description = order.productName;
          vendorOrderDetail.vendorId = order.vendorId;
          vendorOrderDetail.vendorName = order.vendorName;
          vendorOrderDetail.productId = order.productId;
          vendorOrderDetail.quantity = order.quantityApproval ? order.quantityApproval : order.quantity;
          vendorOrderDetail.unitName = order.unitName;
          vendorOrderDetail.unitPrice = order.unitPrice;
          vendorOrderDetail.productUnitId = order.productUnitId;
          vendorOrderDetail.incurredUnit = null;
          vendorOrderDetail.currencyUnit = order.currencyUnit;
          vendorOrderDetail.currencyUnitName = order.currencyUnitName;
          vendorOrderDetail.exchangeRate = (order.exchangeRate === null || order.exchangeRate === undefined) ? 1 : order.exchangeRate;
          vendorOrderDetail.vat = 0;
          vendorOrderDetail.discountType = false;
          vendorOrderDetail.discountValue = 0;
          vendorOrderDetail.sumAmountDiscount = 0;
          vendorOrderDetail.sumAmountVat = 0;
          vendorOrderDetail.sumAmount = (order.quantityApproval ? order.quantityApproval : order.quantity) * order.unitPrice * vendorOrderDetail.exchangeRate;
          vendorOrderDetail.productName = order.productName;
          vendorOrderDetail.selectedAttributeName = '';
          vendorOrderDetail.orderDetailType = 0;
          vendorOrderDetail.isReshow = true;
          vendorOrderDetail.procurementRequestItemId = order.procurementRequestItemId;
          vendorOrderDetail.procurementRequestId = order.procurementRequestId;
          vendorOrderDetail.procurementCode = order.procurementCode;

          listProductByRequest = [...listProductByRequest, vendorOrderDetail];
        });

        //Nếu item có trong list listVendorOrderDetail mà không có trong list listProductByRequest thì
        //xóa item đó đi (trừ các item có isReshow = true)

        let listItemRemove: Array<VendorOrderDetailModel> = [];
        this.listVendorOrderDetail.forEach(item => {
          if (item.isReshow == true) {
            let hasValue = listProductByRequest.find(x => x.procurementRequestItemId == item.procurementRequestItemId);
            if (!hasValue) {
              listItemRemove = [...listItemRemove, item];
            }
          }
        });

        this.listVendorOrderDetail = this.listVendorOrderDetail.filter(x => !listItemRemove.includes(x));

        listProductByRequest.forEach(product => {
          let hasValue = this.listVendorOrderDetail.find(x => x.procurementRequestItemId == product.procurementRequestItemId);

          if (!hasValue) {
            this.listVendorOrderDetail = [...this.listVendorOrderDetail, product];
          }
        });
      }
    } else {
      this.listVendorOrderDetail = this.listVendorOrderDetail.filter(x => x.isReshow != true);
    }
    this.allocation();
    this.getSumarySection();
  }

  /* Thêm Chi phí */
  addCost() {
    let ref = this.dialogService.open(PopupAddEditCostVendorOrderComponent, {
      data: {
        isCreate: true,
        currentIndex: Math.max(...this.listCostModel.map(o => o.index), 0)
      },
      header: 'Thêm chi phí',
      width: '30%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultCostDialog) => {
      if (result) {
        if (result.status) {
          this.listCostModel = [...this.listCostModel, result.costModel];

          //Tính lại tổng chi phí
          this.setTotalCost();
          this.allocation();
          this.getSumarySection();
        }
      }
    });
  }

  /* Sửa Chi phí */
  onRowCostSelect(rowData: CostModel) {
    let ref = this.dialogService.open(PopupAddEditCostVendorOrderComponent, {
      data: {
        isCreate: false,
        currentIndex: rowData.index,
        costModel: rowData
      },
      header: 'Sửa chi phí',
      width: '30%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultCostDialog) => {
      if (result) {
        if (result.status) {
          let modifyCost = this.listCostModel.find(x => x.index == result.costModel.index);
          let index = this.listCostModel.indexOf(modifyCost);
          this.listCostModel[index] = result.costModel;

          //Tính lại tổng chi phí
          this.setTotalCost();
          this.allocation();
          this.getSumarySection();
        }
      }
    });
  }

  /* Xóa Chi phí */
  deleteCostItem(rowData: CostModel) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.listCostModel = this.listCostModel.filter(e => e != rowData);

        //Tính lại tổng chi phí
        this.setTotalCost();
        this.allocation();
        this.getSumarySection();
      }
    });
  }

  /* Tính tổng chi phí */
  setTotalCost() {
    let totalCost = 0;
    this.listCostModel.map(x => x.unitPrice).forEach(cost => { totalCost += cost });
    this.totalCostControl.setValue(totalCost.toString());
  }

  /* Phân bổ Chi phí */
  allocation() {
    if (this.selectedCostType) {
      if (this.listVendorOrderDetail.length > 0) {
        //Lấy Tổng chi phí
        let totalCost = ParseStringToFloat(this.totalCostControl.value.toString());

        //Nếu Theo số lượng
        if (this.selectedCostType == '0') {
          //Lấy Tổng số lượng
          let totalQuantity = 0;
          this.listVendorOrderDetail.forEach(item => {
            totalQuantity += item.quantity;
            item.isEditCost = false;
          });

          //Tính Chi phí mua hàng, Giá nhập kho, Giá trị nhập kho
          this.listVendorOrderDetail.forEach(item => {
            //Chi phí mua hàng cho từng sản phẩm
            item.cost = this.roundNumber(totalCost * item.quantity / totalQuantity, 0);

            //Giá nhập kho cho từng sản phẩm
            item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount + item.cost) / item.quantity, 0);

            //Giá trị nhập kho cho từng sản phẩm
            item.priceValueWarehouse = this.roundNumber(item.quantity * item.priceWarehouse, 0);
          });
        }
        //Nếu Theo giá trị
        else if (this.selectedCostType == '1') {
          //Lấy tổng thành tiền
          let totalAmount = 0;
          this.listVendorOrderDetail.forEach(item => {
            totalAmount += (item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount);
            item.isEditCost = false;
          });

          //Tính Chi phí mua hàng, Giá nhập kho, Giá trị nhập kho
          this.listVendorOrderDetail.forEach(item => {
            //Chi phí mua hàng cho từng sản phẩm
            item.cost = this.roundNumber(totalCost * (item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount) / totalAmount, 0);

            //Giá nhập kho cho từng sản phẩm
            item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount + item.cost) / item.quantity, 0);

            //Giá trị nhập kho cho từng sản phẩm
            item.priceValueWarehouse = this.roundNumber(item.quantity * item.priceWarehouse, 0);
          });
        }
      } else {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa thêm sản phẩm/dịch vụ' };
        this.showMessage(msg);
      }
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn loại phân bổ' };
      this.showMessage(msg);
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

  /* Event: Bật popup Sửa chi phí mua hàng của 1 sản phẩm */
  editCost(dataRow: VendorOrderDetailModel) {
    this.dataRow = null;
    this.dataRow = dataRow;

    //Nếu tổng chi phí đang = 0 thì không cho sửa
    let totalCost = ParseStringToFloat(this.totalCostControl.value.toString());
    if (totalCost > 0) {
      this.display = true;
      this.popupProductName = dataRow.description;
      this.popupCost = dataRow.cost ? dataRow.cost : 0;
    } else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Chưa có tổng chi phí' };
      this.showMessage(msg);
    }
  }

  /* Event: Thay đổi Chi phí mua hàng trong popup */
  changePopupCost() {
    if (!this.popupCost || this.popupCost == '') {
      this.popupCost = 0;
    }
  }

  /* Event: Hủy thay đổi Chi phí mua hàng trong popup */
  cancelPopup() {
    this.display = false;
    this.popupProductName = '';
    this.popupCost = 0;
    this.dataRow = null;
  }

  /* Event: Lưu thay đổi Chi phí mua hàng trong popup */
  savePopup() {
    let _index = this.listVendorOrderDetail.indexOf(this.dataRow);

    let totalCost = ParseStringToFloat(this.totalCostControl.value.toString());
    let cost = ParseStringToFloat(this.popupCost.toString());

    //tổng chi phí đã sửa
    let _totalCostEdit = 0;
    this.listVendorOrderDetail.forEach((item, index) => {
      if (item.isEditCost == true && index != _index) {
        _totalCostEdit += item.cost;
      }
    });

    let count = this.listVendorOrderDetail.length;
    let count_edit = this.listVendorOrderDetail.filter(x => x.isEditCost == true).length;

    //Nếu chi phí mua hàng của sản phẩm lớn hơn Tổng chi phí thì không cho sửa
    if (cost <= totalCost) {
      if (cost + _totalCostEdit > totalCost) {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Chi phí sản phẩm vượt quá chi phí còn lại' };
        this.showMessage(msg);
      }
      else if (this.dataRow.isEditCost != true && (count - count_edit) == 1) {
        let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bản phải phân bổ lại ở Thông tin chi phí' };
        this.showMessage(msg);
      }
      else {
        this.display = false;

        /*Begin: Tính lại các loại chi phí của sản phẩm được sửa*/

        //chi phí mua hàng:
        this.dataRow.cost = cost;

        //giá nhập kho:
        this.dataRow.priceWarehouse = this.roundNumber((this.dataRow.quantity * this.dataRow.unitPrice * this.dataRow.exchangeRate - this.dataRow.sumAmountDiscount + this.dataRow.cost) / this.dataRow.quantity, 0);

        //giá trị nhập kho:
        this.dataRow.priceValueWarehouse = this.roundNumber(this.dataRow.quantity * this.dataRow.priceWarehouse, 0);

        this.dataRow.isEditCost = true;

        this.listVendorOrderDetail[_index] = this.dataRow;

        /*End: Tính lại các loại chi phí của sản phẩm được sửa*/

        /*Begin: Tính lại chi phí cho các sản phẩm khác*/

        //tổng chi phí đã sửa
        let totalCostEdit = 0;
        this.listVendorOrderDetail.forEach(item => {
          if (item.isEditCost == true) {
            totalCostEdit += item.cost;
          }
        });

        let remainTotalCost = totalCost - totalCostEdit;
        let remainTotalQuantity = 0;
        let reaminTotalAmount = 0;

        this.listVendorOrderDetail.forEach((item, index) => {
          if (item.isEditCost != true) {
            remainTotalQuantity += item.quantity;
          }
          reaminTotalAmount += (item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount);
        });

        this.listVendorOrderDetail.forEach((item, index) => {
          if (item.isEditCost != true) {
            if (this.selectedCostType == '0') {
              //chi phí mua hàng:
              item.cost = this.roundNumber(remainTotalCost * item.quantity / remainTotalQuantity, 0);

              //giá nhập kho:
              item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount + item.cost) / item.quantity, 0);

              //giá trị nhập kho:
              item.priceValueWarehouse = this.roundNumber(item.quantity * item.priceWarehouse, 0);
            } else if (this.selectedCostType == '1') {
              //chi phí mua hàng:
              item.cost = this.roundNumber(remainTotalCost * (item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount) / reaminTotalAmount, 0);

              //giá nhập kho:
              item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - item.sumAmountDiscount + item.cost) / item.quantity, 0);

              //giá trị nhập kho:
              item.priceValueWarehouse = this.roundNumber(item.quantity * item.priceWarehouse, 0);
            }
          }
        });

        /*End: Tính lại chi phí cho các sản phẩm khác*/

        this.getSumarySection();
      }
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Chi phí sản phẩm lớn hơn Tổng chi phí' };
      this.showMessage(msg);
    }
  }

  /* Mở popup Tạo nhanh nhà cung cấp */
  openQuickCreVendorModal() {
    let ref = this.dialogService.open(QuickCreateVendorComponent, {
      data: { },
      header: 'Tạo nhanh nhà cung cấp',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        this.listVendorCreateOrderModel = [];
        this.listVendorCreateOrderModel = result.listVendor;
        //set luôn nhà cung cấp vừa tạo cho control nhà cung cấp
        let newVendor = this.listVendorCreateOrderModel.find(x => x.vendorId == result.newVendorId);
        this.vendorControl.setValue(newVendor ? newVendor : null);

        //Hiển thị lại thông tin của nhà cung cấp vừa tạo
        let event = {
          value: newVendor ? newVendor : null
        }
        this.changeVendor(event);
      }
    });
  }

  /* Chuyển item lên một cấp */
  moveUp(data: VendorOrderDetailModel) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listVendorOrderDetail.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listVendorOrderDetail = this.listVendorOrderDetail.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listVendorOrderDetail = [...this.listVendorOrderDetail, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listVendorOrderDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: VendorOrderDetailModel) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listVendorOrderDetail.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listVendorOrderDetail = this.listVendorOrderDetail.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listVendorOrderDetail = [...this.listVendorOrderDetail, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listVendorOrderDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }
}


function checkDuplicateCode(array: Array<any>): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null) {
      if (array.indexOf(control.value.toLowerCase()) !== -1 && control.value.toLowerCase() !== "") {
        return { 'duplicateCode': true };
      }
      return null;
    }
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
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
};

function convertDateToTimeSpan(Time: Date): string {
  let result = '';
  let Hour = Time.getHours().toString();
  let Minute = Time.getMinutes().toString();
  result = Hour + ":" + Minute;

  return result;
}

function ValidationMaxValuePT(max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      let value = ParseStringToFloat(control.value);
      if (value > max) return { 'maxValuePT': true }
    }
    return null;
  }
}

function ValidationMaxValueST(max: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } => {
    if (control.value !== null && control.value !== undefined) {
      let value = ParseStringToFloat(control.value);
      if (value > max) return { 'maxValueST': true }
    }
    return null;
  }
}


