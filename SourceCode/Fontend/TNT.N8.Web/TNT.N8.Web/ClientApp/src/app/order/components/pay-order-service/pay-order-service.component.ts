import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Table } from 'primeng/table';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { CustomerOrderService } from '../../services/customer-order.service';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { FormControl, Validators, FormGroup, AbstractControl, ValidatorFn } from '@angular/forms';
import * as pdfMake from 'pdfmake/build/pdfmake.js';
import * as pdfFonts from 'pdfmake/build/vfs_fonts.js';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import { saveAs } from "file-saver";
import { DecimalPipe } from '@angular/common';

class LocalAddress {
  localAddressId: string;
  localAddressCode: string;
  localAddressName: string;
  address: string;
  description: string;
  branchId: string;
  active: boolean;

  listLocalPoint: Array<LocalPoint>;
}

class LocalPoint {
  localPointId: string;
  localPointCode: string;
  localPointName: string;
  address: string;
  description: string;
  statusId: number;
  localAddressId: string;
  statusName: string;
  active: boolean;
}

class Customer {
  customerId: string;
  customerCode: string;
  customerName: string;
  point: number;
  payPoint: number;
}

@Component({
  selector: 'app-pay-order-service',
  templateUrl: './pay-order-service.component.html',
  styleUrls: ['./pay-order-service.component.css'],
  providers: [
    DatePipe, DecimalPipe
  ]
})
export class PayOrderServiceComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  awaitResult: boolean = false;

  @ViewChild('myTable') myTable: Table;
  @ViewChild('tableOrderetail') tableOrderetail: Table;

  colsListOrder: any;
  selectedColumns: any[];
  colsOrderDetail: any;

  listLocalAddress: Array<LocalAddress> = []; //list Khu vực
  listLocalPoint: Array<LocalPoint> = []; //list tất cả Điểm 
  listCurrentLocalPoint: Array<LocalPoint> = []; //list Điểm hiện tại

  listOrder: Array<any> = [];
  isGlobalFilter: string = null;

  listOrderDetail: Array<any> = [];

  dialogCustomer: boolean = false;

  payOrderForm: FormGroup;
  customerNameControl: FormControl;
  customerPhoneControl: FormControl;
  discountTypeControl: FormControl;
  discountValueControl: FormControl;
  pointControl: FormControl;
  customerMoneyControl: FormControl;
  pointAfterControl: FormControl;
  feedbackMoneyControl: FormControl;

  customer: Customer = null;
  totalPrice: number = 0;
  listDiscountType: Array<any> = [
    {
      name: 'Theo %',
      value: true
    },
    {
      name: 'Theo số tiền',
      value: false
    }
  ];
  isShowPointCustomer: boolean = false;
  pointCustomer: number = 0;
  pointRate: number = 0;
  moneyRate: number = 0;
  totalPriceAfter: number = 0;

  constructor(
    private router: Router,
    private customerOrderService: CustomerOrderService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
    private decimalPipe: DecimalPipe
  ) { }

  ngOnInit(): void {
    this.setForm();
    this.initTable();
    this.getDataDefault();
  }

  setForm() {
    this.customerNameControl = new FormControl({value: null, disabled: true});
    this.customerPhoneControl = new FormControl(null, [Validators.pattern(this.getPhonePattern())]);
    this.discountTypeControl = new FormControl(null);
    this.discountValueControl = new FormControl('0');
    this.pointControl = new FormControl('0');
    this.customerMoneyControl = new FormControl('0');
    this.pointAfterControl = new FormControl({value: '0', disabled: true});
    this.feedbackMoneyControl = new FormControl({value: '0', disabled: true});


    this.payOrderForm = new FormGroup({
      customerNameControl: this.customerNameControl,
      customerPhoneControl: this.customerPhoneControl,
      discountTypeControl: this.discountTypeControl,
      discountValueControl: this.discountValueControl,
      pointControl: this.pointControl,
      customerMoneyControl: this.customerMoneyControl,
      pointAfterControl: this.pointAfterControl,
      feedbackMoneyControl: this.feedbackMoneyControl
    });
  }

  initTable() {
    this.colsListOrder = [
      { field: 'orderCode', header: 'Mã', textAlign: 'left', display: 'table-cell' },
      // { field: 'customerName', header: 'Khách hàng', textAlign: 'left', display: 'table-cell' },
      { field: 'orderDate', header: 'Ngày đặt hàng', textAlign: 'right', display: 'table-cell' },
      // { field: 'orderStatusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
      { field: 'amount', header: 'Tổng giá trị', textAlign: 'right', display: 'table-cell' },
      // { field: 'sellerName', header: 'Nhân viên', textAlign: 'left', display: 'table-cell' },
      // { field: 'listOrderDetail', header: 'Chi tiết', textAlign: 'left', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListOrder;

    this.colsOrderDetail = [
      { field: 'productName', header: 'Mặt hàng', textAlign: 'left', width: '120px', display: 'table-cell' },
      { field: 'unitPrice', header: 'Đơn giá', textAlign: 'right', width: '75px', display: 'table-cell' },
      { field: 'quantity', header: 'SL', textAlign: 'right', width: '45px', display: 'table-cell' },
      { field: 'sumAmount', header: 'Thành tiền', textAlign: 'right', width: '75px', display: 'table-cell' },
    ];
  }

  getDataDefault() {
    this.loading = true;
    this.customerOrderService.getMasterDataPayOrderService().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listLocalAddress = result.listLocalAddress;
        this.listLocalPoint = result.listLocalPoint;
        this.pointRate = result.pointRate;
        this.moneyRate = result.moneyRate;

        this.setDefaultValue();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValue() {
    this.listLocalAddress.forEach(item => {
      item.active = false;
    });
    //Thêm option Tất cả vào Khu vực
    let item: LocalAddress = {
      localAddressId: this.emptyGuid,
      localAddressCode: 'all',
      localAddressName: 'Tất cả',
      address: null,
      branchId: this.emptyGuid,
      description: null,
      listLocalPoint: [],
      active: true
    }
    this.listLocalAddress.unshift(item);
    this.listCurrentLocalPoint = this.listLocalPoint;

    let defaultDiscountType = this.listDiscountType.find(x => x.value == true);
    this.discountTypeControl.setValue(defaultDiscountType);
  }

  /*Thay đổi khu vực*/
  changeLocalAddress(localAddress: LocalAddress) {
    //reset list Đơn hàng
    this.listOrder = [];

    //highlight Khu vực được chọn
    this.listLocalAddress.forEach(item => {
      item.active = false;
      if (item.localAddressId == localAddress.localAddressId) {
        item.active = true;
      }
    });

    //Lấy các Điểm theo Khu vực
    this.customerOrderService.getLocalPointByLocalAddress(localAddress.localAddressId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listCurrentLocalPoint = result.listLocalPoint;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
    
    //bỏ chọn tất cả các Điểm
    this.listCurrentLocalPoint.forEach(item => {
      item.active = false;
    });
  }

  /*Thay đổi Điểm*/
  changeLocalPoint(localPoint: LocalPoint) {
    //highlight Điểm được chọn
    this.listCurrentLocalPoint.forEach(item => {
      item.active = false;
      if (item.localPointId == localPoint.localPointId) {
        item.active = true;
      }
    });

    //Lấy listOrder theo Điểm
    this.customerOrderService.getListOrderByLocalPoint(localPoint.localPointId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listOrder = result.listOrder;
        this.listOrder.forEach(item => {
          item.orderDate = this.datePipe.transform(item.orderDate, 'dd/MM/yyyy');
        });
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Mở popup thanh toán Đơn hàng*/
  openDialog() {
    this.totalPrice = 0;
    this.tableOrderetail.reset();
    let listOrderId = this.listOrder.map(x => x.orderId);
    this.listOrderDetail = [];
    if (listOrderId?.length > 0) {
      this.customer = null;

      //Lấy list sản phẩm theo đơn hàng
      this.customerOrderService.getListOrderDetailByOrder(listOrderId[0]).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.listOrderDetail = result.listOrderDetail;
          this.listOrderDetail.forEach(item => {
            this.totalPrice += item.sumAmount;
          });

          //Tính lại Tổng điểm sau hóa đơn
          this.calculPointAfter();

          this.dialogCustomer = true;
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không có đơn hàng để thanh toán' };
      this.showMessage(msg);
    }
  }

  /*Kiểm tra xem khách hàng có số điện thoại được nhập đã tồn tại trên hệ thống hay không*/
  checkExistsCustomer() {
    this.customer = null;
    let customerPhone: string = null;
    this.isShowPointCustomer = false;
    this.pointCustomer = 0;
    this.pointControl.setValue('0');
    this.pointAfterControl.setValue('0');
    this.discountValueControl.setValue('0');
    this.customerMoneyControl.setValue('0');
    this.feedbackMoneyControl.setValue('0');

    customerPhone = this.customerPhoneControl.value?.trim();

    if (customerPhone && customerPhone != "" && this.customerPhoneControl.valid) {
      this.customerOrderService.checkExistsCustomerByPhone(customerPhone).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          //Trả về Khách hàng
          this.customer = result.customer;

          this.customerNameControl.setValue(this.customer.customerName);
          this.customerNameControl.enable();

          //Nếu là khách hàng mới thì không show điểm của khách hàng
          if (this.customer.customerCode == "KHL001") {
            this.isShowPointCustomer = false;
            this.pointCustomer = 0;
            this.pointAfterControl.setValue('0');
          }
          //Nếu là khách hàng cũ thì show điểm của khách hàng
          else {
            this.isShowPointCustomer = true;
            this.pointCustomer = this.customer.point;

            //Tính điểm sau hóa đơn cho khách hàng
            let pointAfter = this.pointCustomer + this.roundNumber(this.totalPrice / this.pointRate, 0);
            this.pointAfterControl.setValue(pointAfter.toString());
          }

          //Tính lại Tổng điểm sau hóa đơn
          this.calculPointAfter();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
    else {
      this.customerNameControl.setValue(null);
      this.customerNameControl.disable();
      this.isShowPointCustomer = false;
      this.pointCustomer = 0;
      this.pointControl.setValue('0');
      this.pointAfterControl.setValue('0');
      this.discountValueControl.setValue('0');
      this.customerMoneyControl.setValue('0');
      this.feedbackMoneyControl.setValue('0');

      //Tính lại Tổng điểm sau hóa đơn
      this.calculPointAfter();
    }
  }

  /*Thay đổi loại chiết khấu*/
  changeDiscountType() {
    this.discountValueControl.setValue('0');

    //Tính lại Tổng điểm sau hóa đơn
    this.calculPointAfter();
  }

  /*Thay đổi giá trị chiết khấu*/
  changeDiscountValue() {
    let discountValueString = this.discountValueControl.value;
    let discountValue = 0;
    if (discountValueString && discountValueString.trim() != "") {
      discountValue = ParseStringToFloat(discountValueString);
    }
    else {
      this.discountValueControl.setValue('0');
      discountValue = 0;
    }

    if (this.discountTypeControl.value.value) {
      if (discountValue > 100) {
        this.discountValueControl.setValue('100');
      }
    }
    else {
      if (discountValue > this.totalPrice) {
        this.discountValueControl.setValue(this.totalPrice.toString());
      }
    }

    //Tính lại Tổng điểm sau hóa đơn
    this.calculPointAfter();
  }

  /*Thay đổi số thanh toán bằng điểm*/
  changePoint() {
    let pointString = this.pointControl.value;
    if (pointString && pointString.trim() != "") {
      let point = ParseStringToFloat(pointString);
      
      if (point > this.pointCustomer) {
        this.pointControl.setValue(this.pointCustomer.toString());
      }
      else {
        this.pointControl.setValue(point.toString());
      }

      //Tính lại Tổng điểm sau hóa đơn
      this.calculPointAfter();
    }
    else {
      this.pointControl.setValue('0');
    }
  }

  /*Thay đổi số tiền khách đưa*/
  changeCustomerMoney() {
    let moneyString = this.customerMoneyControl.value;
    let money = 0;
    if (moneyString && moneyString.trim() != "") {
      money = ParseStringToFloat(moneyString);
    }
    else {
      this.customerMoneyControl.setValue('0');
    }

    this.calculPointAfter();
  }

  /*Tính lại Tổng điểm sau hóa đơn*/
  calculPointAfter() {
    //Tổng điểm sau hóa đơn
    let point = ParseStringToFloat(this.pointControl.value);
    let discount = 0;
    if (this.discountTypeControl.value.value) {
      discount = this.totalPrice * ParseStringToFloat(this.discountValueControl.value) / 100;
    }
    else {
      discount = ParseStringToFloat(this.discountValueControl.value);
    }

    //Tính tiền phải thanh toán thực tế
    this.totalPriceAfter = this.roundNumber(this.totalPrice - discount - point * this.moneyRate, parseInt(this.defaultNumberType, 10));

    //Tính Tổng điểm sau hóa đơn 
    let currentPoint = this.roundNumber(this.totalPriceAfter / this.pointRate, 0);
    let pointAfter = this.roundNumber(this.pointCustomer - (discount / this.pointRate) - point + currentPoint, 0);
    this.pointAfterControl.setValue(pointAfter.toString());

    let moneyString = this.customerMoneyControl.value;
    let money = ParseStringToFloat(moneyString);
    if (money < this.totalPriceAfter) {
      this.feedbackMoneyControl.setValue('0');
    }
    else {
      let feedbackMoney = this.roundNumber((this.totalPriceAfter - money), parseInt(this.defaultNumberType, 10));
      this.feedbackMoneyControl.setValue(feedbackMoney.toString());

      this.customerMoneyControl.setValidators(null);
      this.customerMoneyControl.updateValueAndValidity();
    }
  }

  /*Thanh toán đơn hàng của Điểm hiện tại*/
  payOrder() {
    let listOrderId = this.listOrder.map(x => x.orderId);
    let localPoint = this.listCurrentLocalPoint.find(x => x.active == true);
    let discountType: boolean = this.discountTypeControl.value.value;
    let discountValue: number = ParseStringToFloat(this.discountValueControl.value.toString());
    let point: number = ParseStringToFloat(this.pointAfterControl.value); //Tổng điểm sau khi thanh toán (điểm hiện tại)
    let payPoint: number = ParseStringToFloat(this.pointControl.value); //Điểm đã dùng để thanh toán

    if (listOrderId.length > 0 && localPoint) {
      //kiểm tra số tiền thanh toán của khách hàng
      let moneyString = this.customerMoneyControl.value;
      let money = ParseStringToFloat(moneyString);
      if (this.totalPriceAfter < 0) {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Số tiền phải thanh toán không hơp lệ' };
        this.showMessage(msg);
      }
      else if (money < this.totalPriceAfter) {
        /*Bật validator kiểm tra*/
        this.customerMoneyControl.setValidators([compareNumberValidator(this.totalPriceAfter)]);
        this.customerMoneyControl.updateValueAndValidity();
        /*End*/
      }
      else {
        this.awaitResult = true;
        this.loading = true;
        this.customerOrderService.payOrderByLocalPoint(listOrderId,
          this.customer?.customerId, this.customerNameControl.value?.trim(), this.customerPhoneControl.value?.trim(),
          discountType, discountValue, point, payPoint).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.exportPdf(new Date(result.orderDate), result.customerName, result.customerPhone, result.cashierName);

            let listLocalPointId = result.listLocalPointId;
            //reset list Đơn hàng
            this.listOrder = [];

            //reset form Thông tin thanh toán
            this.refreshPayForm();

            //reset customer
            this.customer = null;

            //reset status của các Điểm thuộc Khu vực hiện tại về trạng thái sẵn sàng
            this.listCurrentLocalPoint.forEach(item => {
              var exists = listLocalPointId.find(x => x == item.localPointId);
              if (exists) {
                item.statusId = 1;
              }
            });

            this.awaitResult = false;
            
            //đóng popup
            this.dialogCustomer = false;

            let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Thanh toán thành công' };
            this.showMessage(msg);
          }
          else {
            this.awaitResult = false;
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Không có đơn hàng để thanh toán' };
      this.showMessage(msg);
    }
  }

  /*Hủy Dialog thanh toán*/
  cancelPayOrder() {
    this.refreshPayForm();

    this.dialogCustomer = false;
  }

  refreshPayForm() {
    this.payOrderForm.reset();
    this.customerNameControl.disable();
    let defaultDiscountType = this.listDiscountType.find(x => x.value == true);
    this.discountTypeControl.setValue(defaultDiscountType);
    this.pointControl.setValue('0');
    this.pointAfterControl.setValue('0');
    this.discountValueControl.setValue('0');
    this.customerMoneyControl.setValue('0');
    this.customerMoneyControl.setValidators(null);
    this.customerMoneyControl.updateValueAndValidity();
    this.feedbackMoneyControl.setValue('0');
    this.isShowPointCustomer = false;
    this.pointCustomer = 0;
  }

  exportPdf(orderDate: Date, customerName: string, customerPhone: string, cashierName: string) {
    //Tính tiền chiết khấu
    let discount = 0;
    if (this.discountTypeControl.value.value) {
      discount = this.totalPrice * ParseStringToFloat(this.discountValueControl.value) / 100;
    }
    else {
      discount = ParseStringToFloat(this.discountValueControl.value);
    }

    //Thanh toán bằng điểm
    let point = ParseStringToFloat(this.pointControl.value);

    //Tổng thanh toán
    let totalPriceAfter = this.totalPriceAfter;

    //Tiền khách đưa
    let customerMoney = ParseStringToFloat(this.customerMoneyControl.value);

    //Tiền thừa trả khách
    let feedbackMoney = ParseStringToFloat(this.feedbackMoneyControl.value);

    //Tổng điểm sau hóa đơn
    let pointAfter = ParseStringToFloat(this.pointAfterControl.value);

    let documentDefinition: any = {
      pageSize: {
        width: 300.75,
        height: 'auto'
      },
      pageMargins: [5, 5, 5, 5],
      content: [
        {
          table: {
            widths: [50, 200],
            body: [
              [
                {
                  stack: [
                    {
                      text: "" + "ĐỘC QUÁN".toUpperCase(),
                      style: { fontSize: 7 },
                      alignment: 'left',
                      lineHeight: 0.75
                    },
                  ],
                },
                {
                  stack: [
                    {
                      text: "Chi nhánh: " + "81 Đường Phan Kế Bính, Phường Cống Vị, Quận Ba Đình Hà Nội",
                      style: { fontSize: 7 },
                      alignment: 'left'
                    },
                    {
                      text: '',
                      margin: [0, 2, 0, 2]
                    },
                    {
                      text: 'Điện thoại: ' + "097 881 58 95",
                      style: { fontSize: 7 },
                      alignment: 'left'
                    }
                  ],
                }
              ],
            ]
          },
          layout: {
            defaultBorder: false
          },
          lineHeight: 1
        },
        {
          text: '',
          margin: [0, 10, 0, 10]
        },
        {
          text: 'Ngày bán: ' + this.convertDateToString(orderDate),
          style: { fontSize: 7 },
          alignment: 'left',
          margin: [0, 4, 0, 4]
        },
        {
          text: 'HÓA ĐƠN BÁN HÀNG',
          style: 'header',
          alignment: 'center',
          margin: [0, 4, 0, 4]
        },
        {
          text: 'Khách hàng: ' + customerName,
          style: { fontSize: 7 },
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: "Số điện thoại: " + customerPhone,
          style: { fontSize: 7 },
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: "Nhân viên thu ngân: " + cashierName,
          style: { fontSize: 7 },
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          style: 'table',
          table: {
            widths: [80, 40, 50, 85],
            headerRows: 1,
            dontBreakRows: true,
            body: [
              [
                { text: 'Mặt hàng', style: 'tableHeader', alignment: 'center' },
                { text: 'Đơn giá', style: 'tableHeader', alignment: 'center' },
                { text: 'Số lượng', style: 'tableHeader', alignment: 'center' },
                { text: 'Thành tiền', style: 'tableHeader', alignment: 'center' },
              ],
            ]
          },
          layout: {
            defaultBorder: true,
            paddingTop: function (i, node) { return 2; },
            paddingBottom: function (i, node) { return 2; }
          }
        },
        {
          text: '',
          margin: [0, -15.5, 0, -15.5]
        },
        {
          style: 'table',
          table: {
            widths: [188, 85],
            headerRows: 1,
            dontBreakRows: true,
            body: [
              [
                { text: 'Tổng tiền hàng:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(this.totalPrice), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
              [
                { text: 'Chiết khấu:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(discount), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
              [
                { text: 'Thanh toán bằng điểm:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(point), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
              [
                { text: 'Tổng thanh toán:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(totalPriceAfter), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
              [
                { text: 'Tiền khách đưa:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(customerMoney), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
              [
                { text: 'Tiền thừa trả khách:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(feedbackMoney), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
              [
                { text: 'Tổng điểm sau hóa đơn:', style: 'tableHeader', alignment: 'right' },
                { text: this.decimalPipe.transform(pointAfter), style: { fontSize: 7, bold: true }, alignment: 'right' },
              ],
            ]
          },
          layout: {
            defaultBorder: true,
            paddingTop: function (i, node) { return 2; },
            paddingBottom: function (i, node) { return 2; }
          }
        },
        {
          text: '',
          style: { fontSize: 7 },
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Hóa đơn chỉ xuất trong ngày',
          style: { fontSize: 7, italics: true, bold: true },
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Hóa đơn VAT xin liên hệ 0976392466',
          style: { fontSize: 7, italics: true, bold: true },
          alignment: 'left',
          margin: [0, 2, 0, 2]
        },
        {
          text: 'Cảm ơn quý khách hàng',
          style: { fontSize: 7, italics: true, bold: true },
          alignment: 'center',
          margin: [0, 2, 0, 2]
        },
      ],
      styles: {
        header: {
          fontSize: 10,
          bold: true
        },
        timer: {
          fontSize: 10,
          italics: true
        },
        table: {
          margin: [0, 15, 0, 15]
        },
        tableHeader: {
          fontSize: 7,
          bold: true
        },
        tableLines: {
          fontSize: 7,
        },
        StyleItalics: {
          italics: true
        }
      }
    };

    this.listOrderDetail.forEach(item => {
      let option = [
        { text: item.productName, style: 'tableLines', alignment: 'left' },
        { text: this.decimalPipe.transform(item.unitPrice), style: 'tableLines', alignment: 'right' },
        { text: this.decimalPipe.transform(item.quantity), style: 'tableLines', alignment: 'right' },
        { text: this.decimalPipe.transform(item.sumAmount), style: 'tableLines', alignment: 'right' },
      ];
      documentDefinition.content[7].table.body.push(option);
    });

    pdfMake.createPdf(documentDefinition).open();
  }

  goToOrderDetail(orderId: string) {
    this.router.navigate(['/order/order-detail', { customerOrderID: orderId }]);
  }

  goToCustomerDetail(customerId: string) {

  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

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

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'orderDate') {
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

  convertDateToString(date: Date) {
    return this.datePipe.transform(date, "dd/MM/yyyy HH:mm");
  }
}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
}

//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidator(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value && (parseFloat(control.value.replace(/,/g, '')) < number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
}
