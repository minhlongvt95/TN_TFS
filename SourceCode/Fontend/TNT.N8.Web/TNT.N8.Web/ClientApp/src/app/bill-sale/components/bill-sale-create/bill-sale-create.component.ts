import { Component, OnInit, ViewChild, ElementRef, Renderer2, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import * as $ from 'jquery';
import { PopupAddEditCostQuoteDialogComponent } from '../../../shared/components/add-edit-cost-quote/add-edit-cost-quote.component';
import { BillSaleService } from '../../services/bill-sale.service';
import { BillSaleDialogComponent } from '../bill-sale-dialog/bill-sale-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { Item } from '@syncfusion/ej2-navigations';

class BillSale {
  billOfSaLeId: string;
  billOfSaLeCode: string;
  orderId: string;
  billDate: Date;
  endDate: Date;
  statusId: string;
  termsOfPaymentId: string;
  customerId: string;
  customerName: string;
  debtAccountId: string;
  customerAddress: string;
  mst: string;
  paymentMethodId: string;
  employeeId: string;
  description: string;
  note: string;
  accountBankId: string;
  invoiceSymbol: string;
  discountType: boolean;
  discountValue: number;
  listCost: Array<BillSaleCostModel> = [];
  listBillSaleDetail: Array<BillSaleDetailModel> = [];

}

class Category {
  active: boolean;
  categoryCode: string;
  categoryId: string;
  categoryName: string;
  categoryTypeCode: string;
  categoryTypeId: string;
  categoryTypeName: string;
  countCategoryById: number;
  createdById: string;
  createdDate: Date;
  isDefauld: boolean;
  isDefault: boolean;
  isEdit: boolean;
  updatedById: string;
  updatedDate: Date;
}

class Employee {
  employeeCode: string;
  employeeId: string;
  employeeName: string;
}

class Customer {
  customerCode: string;
  customerGroup: string;
  customerGroupId: string;
  customerId: string;
  customerName: string;
  customerPhone: string;
  fullAddress: string;
  personInCharge: string;
  personInChargeId: string;
  taxCode: string;
  name: string;
  listBankAccount: Array<BankAccountModel>;
}

class Order {
  customerId: string;
  customerName: string;
  orderCode: string;
  orderContractId: string;
  orderId: string;
  orderStatusName: string;
  paymentMethod: string;
  placeOfDelivery: string;
  seller: string;
  bankAccountId: string;
  discountType: boolean;
  discountValue: number;
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  quoteDetailModel: any,
}

class BillSaleCostModel {
  billOfSaleCostId: string;
  billOfSaleId: string;
  orderCostId: string;
  orderId: string;
  costId: string;
  quantity: number;
  unitPrice: number;
  costName: string;
  costCode: string;
  sumAmount: number;
}
class BillSaleCost {
  BillOfSaleCostId: string;
  BillOfSaleId: string;
  OrderCostId: string;
  OrderId: string;
  CostId: string;
  Quantity: number;
  UnitPrice: number;
  CostName: string;
  CostCode: string;
  SumAmount: number;
}

class BillSaleDetailModel {
  billOfSaleDetailId: string;
  billOfSaleId: string;
  vendorId: string;
  vendorName: string;
  productId: string;
  productCode: string;
  quantity: number;
  unitPrice: number;
  currencyUnit: string;
  exchangeRate: number;
  warehouseId: string;
  warehouseCode: string;
  moneyForGoods: number;
  accountId: string;
  accountDiscountId: string;
  vat: number;
  discountType: boolean;
  discountValue: number;
  description: string;
  orderDetailType: number;
  unitId: string;
  unitName: string;
  businessInventory: number;
  productName: string;
  actualInventory: number;
  incurredUnit: string;
  costsQuoteType: number;
  orderDetailId: string;
  orderId: string;
  explainStr: string;
  vatValue: number;
  orderNumber: number;
  listBillSaleDetailProductAttribute: Array<BillSaleDetailProductAttributeModel> = [];
}

class BillSaleDetailProductAttributeModel {
  billOfSaleCostProductAttributeId: string;
  orderProductDetailProductAttributeValueId: string;
  orderDetailId: string;
  billOfSaleDetailId: string;
  productId: string;
  productAttributeCategoryId: string;
  productAttributeCategoryValueId: string;
}

class OrderBillModel {
  orderId: string;
  orderCode: string;
  orderDate: Date;
  customerName: string;
  customerCode: string;
  totalOrder: number;
  totalQuantity: number;
  customerId: string;
}
class BankAccountModel {
  bankAccountId: string;
  objectId: string;
  objectType: string;
  accountNumber: string;
  bankName: string;
  branchName: string;
  bankDetail: string;
  accountName: string;
  name: string;
}

interface DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface ResultDialog {
  status: boolean,
  billSaleDetailModel: BillSaleDetailModel,
}

@Component({
  selector: 'app-bill-sale-create',
  templateUrl: './bill-sale-create.component.html',
  styleUrls: ['./bill-sale-create.component.css']
})
export class BillSaleCreateComponent implements OnInit {
  billSaleModel: BillSale = new BillSale();
  listBanking: Array<Category> = [];
  listStatus: Array<Category> = [];
  listEmployee: Array<Employee> = [];
  listCustomer: Array<Customer> = [];
  listCustomerCommon: Array<Customer> = [];
  listBankAccount: Array<BankAccountModel> = [];
  listOrder: Array<Order> = [];
  listOrderCommon: Array<Order> = [];
  listInforOrder: Array<OrderBillModel> = [];
  startDate: Date = null;
  maxEndDate: Date = new Date();
  order: OrderBillModel;
  billSaleDetailModel: BillSaleDetailModel = new BillSaleDetailModel();
  messageConfirm: string = '';
  listInventoryDeliveryVoucher: Array<any> = [];
  colsDelivery: any[];
  selectedColumnsDelivery: any[];

  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  /* End */
  /*Khai báo biến*/
  auth: any = JSON.parse(localStorage.getItem("auth"));
  employeeId: string = JSON.parse(localStorage.getItem('auth')).EmployeeId;
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResult: boolean = false;
  orderId: string = null;
  orderCode: string = null;
  discountValue: string = "0";
  /* Form */
  createBillForm: FormGroup;
  orderControl: FormControl;
  billDateControl: FormControl;
  endDateControl: FormControl;
  termsOfPaymentControl: FormControl;
  customerControl: FormControl;
  descriptionControl: FormControl;
  noteControl: FormControl;
  customerMSTControl: FormControl;
  customerAddressControl: FormControl;
  paymentMethodControl: FormControl;
  employeeControl: FormControl;
  bankAccountControl: FormControl;
  customerNameControl: FormControl;
  invoiceSymbolControl: FormControl;
  debtAccountControl: FormControl;
  /* End */

  cols: any;
  selectedColumns: any;
  colsCost: any;
  colsOrder: any;
  selectedColumnsCost: any;
  selectedColumnsOrder: any;
  TotalSumAmountCost: number;
  selectedItem: any;
  TotalSumAmountProduct: number;
  TotalSumVatProduct: number;
  CustomerOrderTotalDiscount
  CustomerOrderAmountAfterDiscount: number;
  TotalPriceInitial: number;
  AmountPriceProfit: any;
  discountTypeList: Array<DiscountType> = [
    { "name": "Theo %", "code": "PT", "value": true },
    { "name": "Số tiền", "code": "ST", "value": false }
  ];
  discountType: DiscountType = { "name": "Theo %", "code": "PT", "value": true };

  colLeft: number = 8;
  isShow: boolean = true;

  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;

  defaultNumberType = this.getDefaultNumberType();
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

  constructor(
    private translate: TranslateService,
    private billSaleService: BillSaleService,
    private messageService: MessageService,
    private router: Router,
    private route: ActivatedRoute,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private getPermission: GetPermission,
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

  ngOnInit() {
    let resource = "sal/bill-sale/create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      this.route.params.subscribe(params => { this.orderId = params['orderId'] });
      this.setTable();
      this.setForm();
      this.getMasterData();
    }
  }

  setForm() {
    this.orderControl = new FormControl(null, [Validators.required]); // Số hóa đơn
    this.invoiceSymbolControl = new FormControl(null, [Validators.required, forbiddenSpaceText]); // Kí hiệu hóa đơn
    this.employeeControl = new FormControl(null, [Validators.required]); // Nhân viên bán hàng
    this.endDateControl = new FormControl(null); // Ngày hết hạn
    this.termsOfPaymentControl = new FormControl(null);
    this.billDateControl = new FormControl(new Date(), [Validators.required]); // Ngày hóa đơn
    this.descriptionControl = new FormControl(null);
    this.noteControl = new FormControl(null);
    this.customerControl = new FormControl(null, [Validators.required]);
    this.customerMSTControl = new FormControl(null);
    this.customerAddressControl = new FormControl(null, [Validators.required]);
    this.paymentMethodControl = new FormControl(null, [Validators.required]);
    this.bankAccountControl = new FormControl(null);
    this.debtAccountControl = new FormControl(null); // Tài khoản nợ
    this.customerNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);

    this.createBillForm = new FormGroup({
      descriptionControl: this.descriptionControl,
      noteControl: this.noteControl,
      customerControl: this.customerControl,
      customerMSTControl: this.customerMSTControl,
      customerAddressControl: this.customerAddressControl,
      paymentMethodControl: this.paymentMethodControl,
      bankAccountControl: this.bankAccountControl,
      orderControl: this.orderControl,
      invoiceSymbolControl: this.invoiceSymbolControl,
      employeeControl: this.employeeControl,
      endDateControl: this.endDateControl,
      termsOfPaymentControl: this.termsOfPaymentControl,
      billDateControl: this.billDateControl,
      customerNameControl: this.customerNameControl,
      debtAccountControl: this.debtAccountControl
    });
  }

  getMasterData() {
    this.loading = true;
    this.billSaleService.getMasterDataBillSaleCreateEdit(true, this.orderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.billSaleModel = result.billSale;
        if (this.billSaleModel == null) {
          this.billSaleModel = new BillSale();
        }
        else {
          if (this.billSaleModel.listCost == null) {
            this.billSaleModel.listCost = [];
          } else {
            this.billSaleModel.listCost = result.billSale.listCost;
          }
          if (this.billSaleModel.listBillSaleDetail == null) {
            this.billSaleModel.listBillSaleDetail = [];
          } else {
            this.billSaleModel.listBillSaleDetail = result.billSale.listBillSaleDetail;
          }
        }

        this.listBanking = result.listBanking;
        this.listInventoryDeliveryVoucher = result.listInventoryDeliveryVoucher;
        this.listStatus = result.listStatus;
        this.listEmployee = result.listEmployee;
        this.listCustomer = result.listCustomer;
        this.listCustomerCommon = result.listCustomer;
        this.listCustomer.forEach(item => {
          let name: string;
          if (item.customerName == null) {
            name = "";
          } else {
            name = item.customerName;
          }
          item.name = item.customerCode + '-' + name;
        });
        this.listOrder = result.listOrder;
        this.listOrderCommon = result.listOrder;
        this.order = result.order;
        this.listInforOrder = [];
        if (this.order == null) {
          this.listInforOrder = [];
        } else {
          this.listInforOrder.push(this.order);
        }

        this.listCustomer.forEach(item => {
          item.listBankAccount.forEach(bank => {
            if (bank.bankName == null) {
              bank.name = " - " + bank.accountName;
            }
            else {
              bank.name = bank.bankName + " - " + bank.accountName;
            }
          });
        })

        if (this.orderId != null) {
          // Set giá trị cho hóa đơn
          let orderTemp = this.listOrder.find(x => x.orderId == this.orderId);
          if (orderTemp) {
            this.orderCode = orderTemp.orderCode;
          }
          let employee = this.listEmployee.find(x => x.employeeId == orderTemp.seller);
          this.employeeControl.setValue(employee);
          this.orderControl.setValue(orderTemp);
          let customer = this.listCustomer.find(x => x.customerId == this.order.customerId);
          this.customerControl.setValue(customer);
          this.customerNameControl.setValue(this.orderControl.value.customerName);
          let paymentMethod = this.listBanking.find(x => x.categoryId == this.orderControl.value.paymentMethod);
          this.paymentMethodControl.setValue(paymentMethod);
          let bankAccount = this.customerControl.value.listBankAccount.find(x => x.bankAccountId == this.orderControl.value.bankAccountId);
          this.bankAccountControl.setValue(bankAccount);
          this.customerAddressControl.setValue(this.customerControl.value.fullAddress);
          this.customerMSTControl.setValue(this.customerControl.value.taxCode);
        }

        this.caculatorCost();
        this.caculatorQuote();
      }
    });
  }

  setTable() {
    this.cols = [
      { field: 'Move', header: '#', width: '95px', textAlign: 'left', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm/dịch vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productName', header: 'Tên sản phẩm/dịch vụ', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'vendorName', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'unitName', header: 'Đơn vị tính', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'warehouseCode', header: 'Mã kho', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'actualInventory', header: 'Tồn kho thực tế', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'businessInventory', header: 'Tồn kho kinh doanh', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'currencyUnit', header: 'Đơn vị tiền', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'discountValue', header: 'Chiết khấu', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'moneyForGoods', header: 'Tiền hàng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'vat', header: 'Thuế suất', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'vatValue', header: 'Tiền thuế', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'account', header: 'Tài khoản doanh thu', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'accountDiscount', header: 'Tài khoản chiết khấu', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumns = this.cols;

    this.colsCost = [
      { field: 'costCode', header: 'Mã chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'costName', header: 'Tên chi phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'sumAmount', header: 'Thành tiền (VND)', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Xóa', width: '60px', textAlign: 'center', color: '#f44336' }
    ];
    this.selectedColumnsCost = this.colsCost;

    this.colsOrder = [
      { field: 'orderCode', header: 'Số đơn hàng', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'orderDate', header: 'Ngày đặt hàng', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'customerCode', header: 'Mã khách hàng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'customerName', header: 'Tên khách hàng', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'totalOrder', header: 'Tổng số tiền trên đơn hàng', width: '60px', textAlign: 'right', color: '#f44336' },
      { field: 'totalQuantity', header: 'Tổng số lượng trên đơn hàng', width: '60px', textAlign: 'right', color: '#f44336' },
    ];
    this.selectedColumnsOrder = this.colsOrder;

    this.colsDelivery = [
      { field: 'inventoryDeliveryVoucherCode', header: 'Mã phiếu nhập', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'inventoryDeliveryVoucherType', header: 'Loại phiếu nhập', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'createdDate', header: 'Ngày lập phiếu', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'inventoryDeliveryVoucherDate', header: 'Ngày xuất kho', width: '50px', textAlign: 'right', color: '#f44336' },
      { field: 'nameCreate', header: 'Người lập phiếu', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'nameStatus', header: 'Trạng thái', width: '50px', textAlign: 'left', color: '#f44336' },
    ];
    this.selectedColumnsDelivery = this.colsDelivery;
  }

  cancel() {

  }

  changeOrder() {
    let id = this.orderControl.value ? this.orderControl.value.orderId : null;
    if (this.orderControl.value) {
      this.listCustomer = this.listCustomerCommon.filter(x => x.customerId == this.orderControl.value.customerId);
      this.customerNameControl.setValue(this.orderControl.value.customerName);
      let employee = this.listEmployee.find(x => x.employeeId == this.orderControl.value.seller);
      this.employeeControl.setValue(employee);
      if (this.listCustomer.length == 1) {
        this.customerControl.setValue(this.listCustomer[0]);
        this.customerAddressControl.setValue(this.customerControl.value.fullAddress);
        this.customerMSTControl.setValue(this.customerControl.value.taxCode);
        let paymentMethod = this.listBanking.find(x => x.categoryId == this.orderControl.value.paymentMethod);
        this.paymentMethodControl.setValue(paymentMethod);
        let bankAccount = this.customerControl.value.listBankAccount.find(x => x.bankAccountId == this.orderControl.value.bankAccountId);
        this.bankAccountControl.setValue(bankAccount);
        this.discountType = this.discountTypeList.find(x => x.value == this.orderControl.value.discountType);
        this.discountValue = this.orderControl.value.discountValue;
      } else {
        this.customerAddressControl.setValue(null);
        this.customerMSTControl.setValue(null);
      }


      this.loading = true;
      this.billSaleService.getOrderByOrderId(id).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.order = result.order;
          this.listInforOrder = [];
          this.billSaleModel.listBillSaleDetail = [];
          this.billSaleModel.listBillSaleDetail = result.listBillSaleDetail;
          this.billSaleModel.listBillSaleDetail.forEach((item, index) => {
            item.orderNumber = item.orderNumber ? item.orderNumber : index + 1;
          })
          this.billSaleModel.listCost = result.listCost;
          this.caculatorCost();
          this.caculatorQuote();
          if (this.order == null) {
            this.listInforOrder = [];
          } else {
            this.listInforOrder.push(this.order);
          }
        }
      });
    } else {
      this.resetForm();
    }
  }

  showTotalBillSale() {
    this.isShow = !this.isShow;
    this.colLeft = this.isShow ? 8 : 12;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  /*Thêm sản phẩm dịch vụ*/
  addCustomerOrderDetail() {
    let customerGroupId = null;
    if (this.customerControl.value) {
      customerGroupId = this.customerControl.value.customerGroupId;
    }
    let orderDate = this.billDateControl.value;
    if (orderDate) {
      orderDate = convertToUTCTime(orderDate);
    } else {
      orderDate = convertToUTCTime(new Date());
    }

    let ref = this.dialogService.open(BillSaleDialogComponent, {
      data: {
        isCreate: true,
        warehouse: null,
        customerGroupId: customerGroupId,
        orderDate: orderDate
      },
      header: 'Thêm sản phẩm dịch vụ',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          let billSaleDetailModel: BillSaleDetailModel = result.billSaleDetailModel;
          billSaleDetailModel.orderNumber = this.billSaleModel.listBillSaleDetail.length + 1;
          if (billSaleDetailModel.discountType) {
            billSaleDetailModel.vatValue = this.roundNumber((((billSaleDetailModel.quantity * billSaleDetailModel.unitPrice * billSaleDetailModel.exchangeRate) - ((billSaleDetailModel.discountValue * billSaleDetailModel.quantity * billSaleDetailModel.unitPrice * billSaleDetailModel.exchangeRate) / 100)) * (billSaleDetailModel.vat / 100)), parseInt(this.defaultNumberType, 10));
          } else {
            billSaleDetailModel.vatValue = this.roundNumber((((billSaleDetailModel.quantity * billSaleDetailModel.unitPrice * billSaleDetailModel.exchangeRate) - (billSaleDetailModel.discountValue)) * (billSaleDetailModel.vat / 100)), parseInt(this.defaultNumberType, 10));
          }
          billSaleDetailModel.moneyForGoods = parseFloat(result.billSaleDetailModel.moneyForGoods.toString().replace(/,/g, ''));
          this.billSaleModel.listBillSaleDetail.push(billSaleDetailModel)
          this.caculatorQuote();
          this.restartCustomerOrderDetailModel();
        }
      }
    });
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case 0: {
        result = result;
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

  /*Sửa một sản phẩm dịch vụ*/
  onRowSelect(dataRow) {
    var index = this.billSaleModel.listBillSaleDetail.indexOf(dataRow);
    var OldArray = this.billSaleModel.listBillSaleDetail[index];
    let customerGroupId = null;
    if (this.customerControl.value) {
      customerGroupId = this.customerControl.value.customerGroupId;
    }
    let orderDate = this.billDateControl.value;
    if (orderDate) {
      orderDate = convertToUTCTime(orderDate);
    } else {
      orderDate = convertToUTCTime(new Date());
    }
    let titlePopup = '';
    if (OldArray.orderDetailType == 0) {
      titlePopup = 'Sửa sản phẩm dịch vụ';
    } else if (OldArray.orderDetailType == 1) {
      titlePopup = 'Sửa chi phí phát sinh';
    }

    let ref = this.dialogService.open(BillSaleDialogComponent, {
      data: {
        isCreate: false,
        billSaleDetailModel: OldArray,
        warehouse: null,
        customerGroupId: customerGroupId,
        orderDate: orderDate
      },
      header: titlePopup,
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: ResultDialog) => {
      if (result) {
        if (result.status) {
          this.billSaleModel.listBillSaleDetail.splice(index, 1);
          this.billSaleModel.listBillSaleDetail.push(result.billSaleDetailModel);

          this.caculatorQuote();
          this.restartCustomerOrderDetailModel();
        }
      }
    });
  }

  saveBillSale(isAdd: boolean) {
    if (!this.createBillForm.valid) {
      Object.keys(this.createBillForm.controls).forEach(key => {
        if (this.createBillForm.controls[key].valid == false) {
          this.createBillForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
    } else {
      this.loading = true;
      this.billSaleModel.description = this.descriptionControl.value;
      this.billSaleModel.note = this.noteControl.value;
      this.billSaleModel.employeeId = this.employeeControl.value ? this.employeeControl.value.employeeId : null;
      let endDate = this.endDateControl.value;
      if (this.endDateControl.value) {
        endDate = convertToUTCTime(endDate);
      }
      if (this.orderId == null) {
        this.billSaleModel.orderId = this.orderControl.value ? this.orderControl.value.orderId : null;
      }
      else {
        this.billSaleModel.orderId = this.orderId;
      }
      this.billSaleModel.discountType = this.discountType.value;
      this.billSaleModel.discountValue = parseFloat(this.discountValue.replace(/,/g, ''));
      this.billSaleModel.endDate = endDate;
      this.billSaleModel.invoiceSymbol = this.invoiceSymbolControl.value;
      this.billSaleModel.mst = this.customerMSTControl.value;
      this.billSaleModel.paymentMethodId = this.paymentMethodControl.value ? this.paymentMethodControl.value.categoryId : null;
      this.billSaleModel.termsOfPaymentId = this.termsOfPaymentControl.value;
      this.billSaleModel.accountBankId = this.bankAccountControl.value ? this.bankAccountControl.value.bankAccountId : null;
      let billDate = this.billDateControl.value;
      if (this.billDateControl.value) {
        billDate = convertToUTCTime(billDate);
      }
      this.billSaleModel.billDate = billDate;
      this.billSaleModel.customerAddress = this.customerAddressControl.value;
      this.billSaleModel.customerId = this.customerControl.value ? this.customerControl.value.customerId : null;
      this.billSaleModel.customerName = this.customerNameControl.value;
      this.billSaleModel.debtAccountId = this.debtAccountControl.value ? this.debtAccountControl.value.employeeId : null;
      this.billSaleService.addOrEditBillSale(true, this.billSaleModel).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          if (isAdd) {
            this.orderId = null;
            this.ngOnInit();
          } else {
            this.router.navigate(['/bill-sale/detail', { billSaleId: result.billSaleId }]);
          }
        }
      });
    }
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  changeCustomer() {
    if (this.customerControl.value) {
      this.customerNameControl.setValue(this.customerControl.value.customerName);
      this.customerAddressControl.setValue(this.customerControl.value.fullAddress);
      this.customerMSTControl.setValue(this.customerControl.value.taxCode);
      this.listOrder = this.listOrderCommon.filter(x => x.customerId == this.customerControl.value.customerId);
      if (this.listOrder.length == 1) {
        this.orderControl.setValue(this.listOrder[0]);
        let employee = this.listEmployee.find(x => x.employeeId == this.listOrder[0].seller);
        this.employeeControl.setValue(employee);
        this.customerNameControl.setValue(this.listOrder[0].customerName);
        let paymentMethod = this.listBanking.find(x => x.categoryId == this.orderControl.value.paymentMethod);
        this.paymentMethodControl.setValue(paymentMethod);
        let bankAccount = this.customerControl.value.listBankAccount.find(x => x.bankAccountId == this.orderControl.value.bankAccountId);
        this.bankAccountControl.setValue(bankAccount);
        this.discountType = this.discountTypeList.find(x => x.value == this.orderControl.value.discountType);
        this.discountValue = this.orderControl.value.discountValue;

        this.loading = true;
        this.billSaleService.getOrderByOrderId(this.orderControl.value.orderId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.order = result.order;
            this.listInforOrder = [];
            this.billSaleModel.listBillSaleDetail = [];
            this.billSaleModel.listBillSaleDetail = result.listBillSaleDetail;
            this.billSaleModel.listCost = result.listCost;
            this.caculatorQuote();
            this.caculatorCost();
            if (this.order == null) {
              this.listInforOrder = [];
            } else {
              this.listInforOrder.push(this.order);
            }
          }
        });
      } else {
        this.paymentMethodControl.setValue(null);
        this.bankAccountControl.setValue(null);
        this.discountType = this.discountTypeList.find(x => x.value == true);
        this.discountValue = '0';
        this.listInventoryDeliveryVoucher = [];
        this.listInforOrder = [];
        this.billSaleModel.listBillSaleDetail = [];
        this.employeeControl.setValue(null);
        this.caculatorQuote();
        this.caculatorCost();
      }
    } else {
      this.resetForm();
    }
  }

  resetForm() {
    this.listOrder = this.listOrderCommon.filter(x => x != null);
    this.listCustomer = this.listCustomerCommon.filter(x => x != null);
    this.orderControl.setValue(null);
    this.customerControl.setValue(null);
    this.customerNameControl.setValue(null);
    this.customerAddressControl.setValue(null);
    this.customerMSTControl.setValue(null);
    this.paymentMethodControl.setValue(null);
    this.bankAccountControl.setValue(null);
    this.discountType = this.discountTypeList.find(x => x.value == true);
    this.discountValue = '0';
    this.listInventoryDeliveryVoucher = [];
    this.listInforOrder = [];
    this.employeeControl.setValue(null);
    this.billSaleModel.listBillSaleDetail = [];
  }

  addCostQuote() {
    let ref = this.dialogService.open(PopupAddEditCostQuoteDialogComponent, {
      data: {
        isCreate: true
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
          let cost = new BillSaleCostModel();
          cost.costId = result.quoteDetailModel.CostId;
          cost.costName = result.quoteDetailModel.CostName;
          cost.quantity = result.quoteDetailModel.Quantity;
          cost.unitPrice = result.quoteDetailModel.UnitPrice;
          cost.costCode = result.quoteDetailModel.CostCode;
          cost.sumAmount = result.quoteDetailModel.SumAmount;
          this.billSaleModel.listCost.push(cost);
          this.caculatorCost();
        }
      }
    });
  }

  onRowCostSelect(dataRow) {
    //Nếu có quyền sửa thì mới cho sửa
    var index = this.billSaleModel.listCost.indexOf(dataRow);
    let cost = this.billSaleModel.listCost[index];
    var OldArray: BillSaleCost = new BillSaleCost();
    OldArray.BillOfSaleCostId = cost.billOfSaleCostId;
    OldArray.BillOfSaleId = cost.billOfSaleId;
    OldArray.CostCode = cost.costCode;
    OldArray.CostId = cost.costId;
    OldArray.CostName = cost.costName;
    OldArray.OrderCostId = cost.orderCostId;
    OldArray.OrderId = cost.orderId;
    OldArray.Quantity = cost.quantity;
    OldArray.SumAmount = cost.sumAmount;
    OldArray.UnitPrice = cost.unitPrice;
    let titlePopup = 'Sửa chi phí';

    let ref = this.dialogService.open(PopupAddEditCostQuoteDialogComponent, {
      data: {
        isCreate: false,
        quoteDetailModel: OldArray
      },
      header: titlePopup,
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
          this.billSaleModel.listCost.splice(index, 1);
          cost.costCode = result.quoteDetailModel.CostCode;
          cost.costId = result.quoteDetailModel.CostId;
          cost.costName = result.quoteDetailModel.CostName;
          cost.quantity = result.quoteDetailModel.Quantity;
          cost.sumAmount = result.quoteDetailModel.SumAmount;
          cost.unitPrice = result.quoteDetailModel.UnitPrice;

          this.billSaleModel.listCost.push(cost);

          // /*Tính lại tổng tiền của đơn hàng*/

          this.caculatorCost();
          this.restartCustomerOrderDetailModel();
        }
      }
    });
  }

  caculatorCost() {
    this.TotalSumAmountCost = 0;
    this.billSaleModel.listCost.forEach(item => {
      this.TotalSumAmountCost = this.TotalSumAmountCost + item.sumAmount;
    });
    if (this.discountType.value) {
      this.CustomerOrderTotalDiscount = this.TotalSumAmountProduct * parseFloat(this.discountValue.replace(/,/g, '')) / 100;
    } else {
      this.CustomerOrderTotalDiscount = this.discountValue.replace(/,/g, '');
    }
    this.CustomerOrderAmountAfterDiscount = this.TotalSumAmountProduct + this.TotalSumVatProduct + this.TotalSumAmountCost - this.CustomerOrderTotalDiscount;
  }

  caculatorQuote() {
    /*Tính lại tổng tiền của đơn hàng*/
    this.CustomerOrderTotalDiscount = 0
    this.TotalSumVatProduct = 0;
    this.TotalSumAmountProduct = 0
    this.CustomerOrderAmountAfterDiscount = 0;
    this.billSaleModel.listBillSaleDetail.forEach(item => {
      let discount = 0;
      if (item.discountType) {
        discount = item.quantity * item.unitPrice * item.exchangeRate * item.discountValue / 100;
      }
      else {
        discount = item.discountValue;
      }
      item.vatValue = (item.quantity * item.unitPrice * item.exchangeRate - discount) * (item.vat / 100);
      this.TotalSumAmountProduct = this.TotalSumAmountProduct + item.exchangeRate * item.quantity * item.unitPrice - discount;
      this.TotalSumVatProduct = this.TotalSumVatProduct + (item.quantity * item.exchangeRate * item.unitPrice - discount) * item.vat / 100;
    });
    if (this.discountType.value) {
      this.CustomerOrderTotalDiscount = this.TotalSumAmountProduct * parseFloat(this.discountValue.replace(/,/g, '')) / 100;
    } else {
      this.CustomerOrderTotalDiscount = this.discountValue;
    }
    this.CustomerOrderAmountAfterDiscount = this.TotalSumAmountProduct + this.TotalSumVatProduct + this.TotalSumAmountCost - this.CustomerOrderTotalDiscount;
  }

  changeDiscountValue() {
    let discountValue = 0;
    if (this.discountValue.trim() == '') {
      discountValue = 0;
      this.discountValue = '0';
    } else {
      discountValue = parseFloat(this.discountValue.replace(/,/g, ''));
    }

    let discountType = this.discountType;
    let codeDiscountType = discountType.code;
    //Nếu loại chiết khấu là theo % thì giá trị discountValue không được lớn hơn 100%
    if (codeDiscountType == "PT") {
      if (discountValue > 100) {
        discountValue = 100;
        this.discountValue = '100';
      }
    } else {
      if (discountValue > this.TotalSumAmountProduct) {
        discountValue = this.TotalSumAmountProduct;
        this.discountValue = this.TotalSumAmountProduct.toString();
      }
    }

    /*Tính lại tổng tiền của đơn hàng*/
    this.CustomerOrderTotalDiscount = 0
    let discount = 0;
    if (codeDiscountType == "PT") {
      discount = this.TotalSumAmountProduct * parseFloat(this.discountValue.replace(/,/g, '')) / 100;
    }
    else {
      discount = parseFloat(this.discountValue.replace(/,/g, ''));
    }
    this.CustomerOrderTotalDiscount = this.CustomerOrderTotalDiscount + discount;
    this.CustomerOrderAmountAfterDiscount = this.TotalSumAmountProduct + this.TotalSumVatProduct - this.CustomerOrderTotalDiscount + this.TotalSumAmountCost;
  }

  changeDiscountType(value) {
    this.discountValue = '0';
  }

  restartCustomerOrderDetailModel() {
    var item: BillSaleDetailModel = {
      billOfSaleDetailId: this.emptyGuid,
      billOfSaleId: this.emptyGuid,
      vendorId: this.emptyGuid,
      vendorName: null,
      productId: this.emptyGuid,
      productCode: null,
      quantity: 0,
      unitPrice: 0,
      currencyUnit: null,
      exchangeRate: 0,
      warehouseId: this.emptyGuid,
      warehouseCode: null,
      moneyForGoods: 0,
      accountId: this.emptyGuid,
      accountDiscountId: this.emptyGuid,
      vat: 0,
      discountType: false,
      discountValue: 0,
      description: null,
      orderDetailType: 0,
      unitId: this.emptyGuid,
      unitName: null,
      businessInventory: 0,
      productName: null,
      actualInventory: 0,
      incurredUnit: null,
      costsQuoteType: 0,
      orderDetailId: this.emptyGuid,
      explainStr: null,
      orderId: this.emptyGuid,
      listBillSaleDetailProductAttribute: [],
      vatValue: null,
      orderNumber: 0
    };

    this.billSaleDetailModel = item;
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow, event: Event) {
    this.translate.get('order.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        this.billSaleModel.listBillSaleDetail = this.billSaleModel.listBillSaleDetail.filter(x => x != dataRow);
        //Đánh lại số OrderNumber
        this.billSaleModel.listBillSaleDetail.forEach((item, index) => {
          item.orderNumber = index + 1;
        });
      }
    });
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteCostItem(dataRow) {
    this.translate.get('order.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        this.billSaleModel.listCost = this.billSaleModel.listCost.filter(x => x != dataRow);
      }
    });
  }

  /* Chuyển item lên một cấp */
  moveUp(data: BillSaleDetailModel) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.billSaleModel.listBillSaleDetail.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.billSaleModel.listBillSaleDetail = this.billSaleModel.listBillSaleDetail.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.billSaleModel.listBillSaleDetail = [...this.billSaleModel.listBillSaleDetail, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.billSaleModel.listBillSaleDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: BillSaleDetailModel) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.billSaleModel.listBillSaleDetail.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.billSaleModel.listBillSaleDetail = this.billSaleModel.listBillSaleDetail.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.billSaleModel.listBillSaleDetail = [...this.billSaleModel.listBillSaleDetail, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.billSaleModel.listBillSaleDetail.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }
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