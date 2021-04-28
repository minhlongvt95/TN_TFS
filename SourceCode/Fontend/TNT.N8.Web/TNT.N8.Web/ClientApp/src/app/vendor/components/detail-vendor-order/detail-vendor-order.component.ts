import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, HostListener, Renderer2 } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import * as $ from 'jquery';
import { VendorOrderModel } from '../../models/vendorOrder.model';
import { VendorOrderDetailModel } from '../../models/vendorOrderDetail.model';
import { VendorOrderProductDetailProductAttributeValueModel } from '../../models/vendorOrderProductDetailProductAttributeValue.model';
import { VendorService } from "../../services/vendor.service";
import { GetPermission } from '../../../shared/permission/get-permission';

import { AddVendorOrderProductComponent } from '../add-vendor-order-product/add-vendor-order-product.component';
import { PopupAddEditCostVendorOrderComponent } from '../../components/popup-add-edit-cost-vendor-order/popup-add-edit-cost-vendor-order.component';

import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';

import { TranslateService } from '@ngx-translate/core';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { QuickCreateVendorComponent } from '../../../shared/components/quick-create-vendor/quick-create-vendor.component';

class DiscountType {
  name: string;
  code: string;
  value: boolean;
}

interface ConfigDialogOrderDetail {
  isCreate: boolean;
  vendor: vendorCreateOrderModel;
  vendorOrderDetail: VendorOrderDetailModel;
  isProcurementRequestItem: boolean;
  quantityApproval: number;
}

interface ResultDialog {
  status: boolean,
  vendorOrderDetail: VendorOrderDetailModel
}

class PurchaseOrderStatus {
  purchaseOrderStatusId: string;
  purchaseOrderStatusCode: string;
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
  vendorName: string;
  vendorPhone: string;
  vendorCode: string;
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

/* Begin: Ghi chú */
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
  listFile: Array<FileInFolder>
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

interface FileNameExists {
  oldFileName: string;
  newFileName: string
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
/* End: Ghi chú */

class ItemInvalidModel {
  procurementRequestItemId: string;
  remainQuantity: number;
}

@Component({
  selector: 'app-detail-vendor-order',
  templateUrl: './detail-vendor-order.component.html',
  styleUrls: ['./detail-vendor-order.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class DetailVendorOrderComponent implements OnInit {
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  isManager: boolean = localStorage.getItem('IsManager') == "true" ? true : false;

  //permission
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionSendApprove: boolean = true;
  actionApprove: boolean = true;
  actionReject: boolean = true;

  auth: any = JSON.parse(localStorage.getItem("auth"));
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

  /* Form */
  createOrderForm: FormGroup;
  orderCodeControl: FormControl;
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
  //master data
  listOrderStatus: Array<PurchaseOrderStatus> = [];
  listEmployee: Array<employeeModel> = [];
  listPaymentMethod: Array<paymentMethod> = [];
  listVendorCreateOrderModel: Array<vendorCreateOrderModel> = [];
  listBankAccount: Array<bankAccount> = [];
  vendorOrderById: VendorOrderModel;
  listVendorOrderDetailById: Array<VendorOrderDetailModel> = [];
  listProcurementRequest: Array<any> = [];
  listContract: Array<any> = [];
  listWareHouse: Array<any> = [];
  listProcurementRequestId: Array<string> = [];

  listCurrentVendorContact: Array<vendorContact> = [];
  listCurrentBankAccount: Array<bankAccount> = [];

  optionCustomer: string = '2';
  listCustomer: Array<any> = [];
  customerEmail: string = '';
  customerPhone: string = '';
  fullAddress: string = '';
  leadName: string = '';
  leadEmail: string = '';
  leadPhone: string = '';
  leadFullAddress: string = '';
  displayApprovalReject: boolean = false;
  isApproval: boolean = false;
  descriptionApprovalReject: string = '';

  statusName: string;
  statusCode: string;
  actionAdd: boolean = true;
  //table
  cols: any[];
  selectedColumns: any[];
  selectedItem: any;

  colsCost: any[];
  selectedColumnsCost: any[];
  selectedItemCost: any;

  listCostModel: Array<CostModel> = [];

  selectedCostType: string = '0';

  /*Data popup inline*/
  display: boolean = false;
  popupProductName: string = '';
  popupCost: any = 0;

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

  vendorOrderId: string = this.emptyGuid;

  /* Begin: Ghi chú */
  noteContent: string = '';
  @ViewChild('fileNoteUpload', { static: true }) fileNoteUpload: FileUpload;
  listUpdateFileNote: Array<FileInFolder> = [];
  colsNoteFile: Array<any> = [];
  noteHistory: Array<Note> = [];

  messageConfirm: string = '';
  messageTitle: string = '';
  messageError: string = '';

  noteId: string = null;

  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  /* End: Ghi chú */

  isInvalidItem: boolean = false;
  isOpenPopupInvalidItem: boolean = false;
  isUserSendAproval: boolean = false; // có phải người gửi phê duyệt không
  listItemInvalidModel: Array<ItemInvalidModel> = [];

  constructor(
    private translate: TranslateService,
    private router: Router,
    private getPermission: GetPermission,
    private vendorService: VendorService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private renderer: Renderer2,
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private folderService: ForderConfigurationService) {
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
    let resource = "buy/vendor/detail-order/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      if (listCurrentActionResource.indexOf("send_approve") == -1) {
        this.actionSendApprove = false;
      }
      if (listCurrentActionResource.indexOf("approve") == -1) {
        this.actionApprove = false;
      }
      if (listCurrentActionResource.indexOf("reject") == -1) {
        this.actionReject = false;
      }
      this.route.params.subscribe(params => { this.vendorOrderId = params['vendorOrderId'] });
      this.setForm();
      this.setTable();
      this.getMasterData();
    }
  }

  setForm() {
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    this.orderCodeControl = new FormControl(null, [Validators.required]);
    this.orderDateControl = new FormControl(new Date(), [Validators.required]);
    this.orderStatusControl = new FormControl(null, [Validators.required]);
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
      orderCodeControl: this.orderCodeControl,
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
      totalCostControl: this.totalCostControl
    });
  }

  resetForm() {
    this.createOrderForm.reset();

    this.orderDateControl.setValue((new Date()));
    this.orderStatusControl.setValue(null);
    this.ordererControl.setValue(null);
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

    this.colsNoteFile = [
      { field: 'fileName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'size', header: 'Kích thước', width: '50%', textAlign: 'left' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'left' },
    ];
  }

  resetTable() {
    this.listVendorOrderDetail = [];
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

  close() {
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
  addCustomerOrderDetail(): boolean {
    const currentVendor: vendorCreateOrderModel = this.vendorControl.value;
    if (!currentVendor) {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Chọn nhà cung cấp' };
      this.showMessage(msg);
      return false;
    }

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
        "max-height": "600px",
        // "overflow": "auto"
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
    if (this.actionEdit && this.statusCode == 'DRA') {
      //Kiểm tra xem item có phải là từ đề xuất hay không
      let isProcurementRequestItem = dataRow.procurementRequestItemId ? true : false;
      let quantityApproval = 0;
      if (isProcurementRequestItem) {
        if (dataRow.vendorOrderDetailId && dataRow.vendorOrderDetailId != this.emptyGuid) {
          let result: any = await this.vendorService.getQuantityApproval(dataRow.vendorOrderDetailId, dataRow.procurementRequestItemId, dataRow.productId);
          //So cua chinh no
          quantityApproval = result.quantityApproval;
        }
        else {
          quantityApproval = dataRow.quantity;
        }
      }

      const currentVendor: vendorCreateOrderModel = this.vendorControl.value;

      let data: ConfigDialogOrderDetail = {
        isCreate: false,
        vendor: currentVendor,
        vendorOrderDetail: dataRow,
        isProcurementRequestItem: isProcurementRequestItem,
        quantityApproval: quantityApproval
      }

      let ref = this.dialogService.open(AddVendorOrderProductComponent, {
        data: data,
        header: 'Chỉnh sửa sản phẩm dịch vụ',
        width: '70%',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "280px",
          "max-height": "600px",
          // "overflow": "auto"
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
  }

  buildDescription(productName: string, selectedAttributeName: string): string {
    let _selectedAttributeName = "";
    if (selectedAttributeName) _selectedAttributeName = "(" + selectedAttributeName + ")";
    let result = productName + " " + _selectedAttributeName;
    return result;
  }

  /*Xóa một sản phẩm dịch vụ*/
  deleteItem(dataRow: VendorOrderDetailModel) {
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

  patchVendorOrderById(vendorOrder: VendorOrderModel, listProcurementRequestId: Array<string>) {
    this.orderDateControl.setValue(new Date(vendorOrder.vendorOrderDate));
    //mã đơn hàng
    let oderCode = vendorOrder.vendorOrderCode;
    this.orderCodeControl.setValue(oderCode ? oderCode : null);
    //trạng thái
    let status = this.listOrderStatus.find(e => e.purchaseOrderStatusId == vendorOrder.statusId);
    this.orderStatusControl.setValue(status ? status : null);
    this.statusCode = status.purchaseOrderStatusCode;
    this.statusName = status.description;
    //người tạo
    let orderer = this.listEmployee.find(e => e.employeeId == vendorOrder.orderer);
    this.ordererControl.setValue(orderer ? orderer : null);
    //diễn giả
    let description = vendorOrder.description;
    this.descriptionControl.setValue(description ? description : "");
    //Ghi chú
    let note = vendorOrder.note;
    this.noteControl.setValue(note ? note : "");

    //Phiếu đề xuất mua hàng
    let listSelectedProcurementRequest = this.listProcurementRequest.filter(x => listProcurementRequestId.includes(x.procurementRequestId));
    if (listSelectedProcurementRequest) this.procurementRequestControl.setValue(listSelectedProcurementRequest);

    //Hợp đồng

    //Mã kho
    if (vendorOrder.warehouseId) {
      let warehouse = this.listWareHouse.find(x => x.warehouseId == vendorOrder.warehouseId);
      this.warehouseControl.setValue(warehouse ? warehouse : null);
    }

    //Thông tin nhà cung cấp
    //nhà cung cấp
    let vendor = this.listVendorCreateOrderModel.find(e => e.vendorId == vendorOrder.vendorId);
    this.vendorControl.setValue(vendor ? vendor : null);
    if (vendor) this.patchVendorData(vendor.vendorId);

    //Thông tin chi phí
    this.selectedCostType = vendorOrder.typeCost;

    //Thông tin giao hàng
    //ngày nhận
    let receivedDate = vendorOrder.receivedDate;
    this.receivedDateControl.setValue(receivedDate ? new Date(receivedDate) : null);

    //giờ nhận
    let receivedHour: any = vendorOrder.receivedHour;
    if (receivedHour) {
      receivedHour = convertTimeSpanToDate(receivedHour);
      this.receivedHourControl.setValue(receivedHour);
    }

    //tên người nhận
    // let recipientName = vendorOrder.recipientName;
    // this.recipientNameControl.setValue(recipientName ? recipientName : '');

    //tên người giao hàng
    let shipperName = vendorOrder.shipperName;
    this.shipperNameControl.setValue(shipperName ? shipperName : '');

    //Địa điểm xuất hàng
    let locationOfShipment = vendorOrder.locationOfShipment;
    this.locationOfShipmentControl.setValue(locationOfShipment ? locationOfShipment : '');

    //địa điểm giao hàng
    let placeOfDelivery = vendorOrder.placeOfDelivery;
    this.placeOfDeliveryControl.setValue(placeOfDelivery ? placeOfDelivery : '');

    //số điện thoại
    let recipientPhone = vendorOrder.recipientPhone;
    this.recipientPhoneControl.setValue(recipientPhone ? recipientPhone : '');

    //email
    let recipientEmail = vendorOrder.recipientEmail;
    this.recipientEmailControl.setValue(recipientEmail ? recipientEmail : '');

    //ghi chu
    let shippingNote = vendorOrder.shippingNote;
    this.shippingNoteControl.setValue(shippingNote ? shippingNote : '');

    //sumary section
    let discountType = this.discountTypeList.find(e => e.value == vendorOrder.discountType);
    let discountValue = vendorOrder.discountValue;
    this.discountTypeControl.setValue(discountType ? discountType : null);
    this.discountValueControl.setValue(discountValue ? discountValue : 0);
    //this.getSumarySection();
    this.setValidatorForDiscount();
  }

  patchVendorOrderDetail(listVendorOrderDetail: Array<VendorOrderDetailModel>) {
    this.listVendorOrderDetail = listVendorOrderDetail;
    this.listVendorOrderDetail.forEach((orderdetail, index) => {
      //mapping lại curency name và currency id
      orderdetail.currencyUnitName = orderdetail.currencyUnitName;
      orderdetail.sumAmount = this.calculatorAmountProduct(orderdetail);
      orderdetail.listVendorOrderProductDetailProductAttributeValue = orderdetail.vendorOrderProductDetailProductAttributeValue;

      /* sản phẩm dịch vụ */
      if (orderdetail.orderDetailType == 0) {
        orderdetail.description = orderdetail.productName; //this.buildDescription(orderdetail.productName, orderdetail.selectedAttributeName);
        orderdetail.productUnitId = orderdetail.unitId;
        orderdetail.orderNumber = orderdetail.orderNumber ? orderdetail.orderNumber : index + 1;
      }
      /* chi phí khác */
      else if (orderdetail.orderDetailType == 1) {
        orderdetail.description = orderdetail.description;
        orderdetail.unitName = orderdetail.incurredUnit;
      }
    });
  }

  isBank: boolean = true;
  patchVendorData(vendorId: string) {
    this.listCurrentVendorContact = [];
    this.vendorContactControl.reset();
    let currentVendor: vendorCreateOrderModel = this.listVendorCreateOrderModel.find(e => e.vendorId == vendorId);
    let vendor: VendorOrderModel = this.vendorOrderById;
    this.vendorControl.setValue(currentVendor);

    if (this.vendorOrderById) {

      this.listCurrentVendorContact = currentVendor.listVendorContact;
      if (this.listCurrentVendorContact.length > 0) {
        let vendorContact = this.listCurrentVendorContact.find(e => e.contactId == this.vendorOrderById.vendorContactId);
        this.vendorContactControl.setValue(vendorContact);
      }
      let payment = this.listPaymentMethod.find(e => e.categoryId == vendor.paymentMethod);
      if (payment) {
        this.paymentControl.setValue(payment);
        if (payment.categoryCode == "BANK") {
          this.isBank = true;
          //show tài khoản ngân hàng
          this.listCurrentBankAccount = this.listBankAccount.filter(e => e.objectId == currentVendor.vendorId);
          if (this.listCurrentBankAccount.length > 0) {
            let bankAccountVendor = this.listCurrentBankAccount.find(e => e.bankAccountId == this.vendorOrderById.bankAccountId);
            this.bankAccountControl.setValue(bankAccountVendor);
          }
        } else {
          this.isBank = false;
        }
      }
    }

  }

  /*Tính tổng tiền trên mỗi sản phẩm*/
  calculatorAmountProduct(vendorOrderDetail: VendorOrderDetailModel): number {
    let amount = vendorOrderDetail.quantity * vendorOrderDetail.unitPrice * vendorOrderDetail.exchangeRate;
    let amountCK = 0;
    if (vendorOrderDetail.discountType == true) {
      amountCK = amount * vendorOrderDetail.discountValue / 100;
    } else {
      amountCK = vendorOrderDetail.discountValue;
    }
    let amountTax = (amount - amountCK) * vendorOrderDetail.vat / 100;
    let amountProduct = this.roundNumber((amount - amountCK + amountTax), parseInt(this.defaultNumberType, 10));
    return amountProduct;
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
      } else {
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
      //show tài khoản ngân hàng
      if (this.vendorControl.value) {
        this.listCurrentBankAccount = this.listBankAccount.filter(e => e.objectId == currentVendor.vendorId);
        if (this.listCurrentBankAccount.length == 1) {
          this.bankAccountControl.setValue(this.listCurrentBankAccount[0]);
        }
      }
    } else {
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

  /* Event: Click nút Phê duyệt hoặc Từ chối Đơn hàng mua (approver = true: phê duyệt, approver = false: từ chối) */
  approveOrReject(approver: boolean) {
    this.displayApprovalReject = true;
    this.isApproval = approver;
  }

  /* Lưu Phê duyệt hoặc Từ Chối */
  async aprrovalRejectVendorOrder() {
    this.vendorService.approvalOrRejectVendorOrder(this.vendorOrderId, this.isApproval,
      this.descriptionApprovalReject).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          if (this.isApproval) {
            let listItemInvalidModel: Array<ItemInvalidModel> = result.listItemInvalidModel;

            if (listItemInvalidModel?.length > 0) {
              this.isOpenPopupInvalidItem = true;

              //Highlight các item bị lỗi
              let listProcurementRequestItemId = listItemInvalidModel.map(x => x.procurementRequestItemId);
              this.listVendorOrderDetail.forEach(item => {
                if (listProcurementRequestItemId.includes(item.procurementRequestItemId)) {
                  item.isInvalidItemRequest = true;
                }
              });
            } else {
              this.showToast('success', 'Thông báo', 'Phê duyệt thành công');
              this.getMasterData();
            }
          }
          else {
            this.showToast('success', 'Thông báo', 'Từ chối phê duyệt thành công');
            this.getMasterData();
          }
          this.displayApprovalReject = false;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.vendorService.getDataEditVendorOrder(this.vendorOrderId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listEmployee = result.listEmployeeModel;
      this.listOrderStatus = result.listOrderStatus;
      this.listPaymentMethod = result.listPaymentMethod;
      this.listVendorCreateOrderModel = result.vendorCreateOrderModel;
      this.listBankAccount = result.listBankAccount;
      this.vendorOrderById = result.vendorOrderById;
      this.listProcurementRequest = result.listProcurementRequest ? result.listProcurementRequest : [];
      this.listWareHouse = result.listWareHouse ? result.listWareHouse : [];
      this.listProcurementRequestId = result.listProcurementRequestId;

      if (this.auth.UserId == this.vendorOrderById.updatedById) {
        this.isUserSendAproval = true;
      }

      /* Hiển thị lại Dòng thời gian */
      this.noteHistory = result.listNote;
      this.handleNoteContent();

      this.listVendorOrderDetailById = result.listVendorOrderDetailById;
      if (this.listVendorCreateOrderModel.length > 0) {
        this.listVendorCreateOrderModel.forEach(item => {
          item.vendorName = item.vendorCode + ' - ' + item.vendorName;
        });
      }

      if (this.listVendorOrderDetailById.length != 0) this.patchVendorOrderDetail(this.listVendorOrderDetailById);
      if (this.vendorOrderById) this.patchVendorOrderById(this.vendorOrderById, this.listProcurementRequestId);

      this.listCostModel = result.listVendorOrderCostDetail ? result.listVendorOrderCostDetail : [];
      this.mapDataCostToForm();

      this.getSumarySection();

      this.disableForm();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  /* Disable Form */
  disableForm() {
    //Nếu trạng thái đơn hàng mua không phải Nháp thì disable tất cả control và button
    if (this.statusCode != 'DRA') {
      this.createOrderForm.disable();
    } else {
      this.createOrderForm.enable();
    }
  }

  /* Hiển thị lại Thông tin chi phí */
  mapDataCostToForm() {
    this.listCostModel.forEach((item, _index) => {
      item.index = _index + 1;
    });

    //Tính lại Tổng chi phí
    this.setTotalCost();
  }

  async editOrder(isSendApproval: boolean) {
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

      let listVendorOrderProcurementRequestMapping = [];
      let listSelectedProcurementRequest = this.procurementRequestControl.value;

      if (listSelectedProcurementRequest.length > 0) {
        listSelectedProcurementRequest.forEach(item => {
          let vendorOrderProcurementRequestMapping = new VendorOrderProcurementRequestMapping();

          vendorOrderProcurementRequestMapping.vendorOrderProcurementRequestMappingId = this.emptyGuid;
          vendorOrderProcurementRequestMapping.vendorOrderId = vendorOrder.vendorOrderId;
          vendorOrderProcurementRequestMapping.procurementRequestId = item.procurementRequestId;
          vendorOrderProcurementRequestMapping.active = true;
          vendorOrderProcurementRequestMapping.createdById = this.auth.UserId;
          vendorOrderProcurementRequestMapping.createdDate = new Date();

          listVendorOrderProcurementRequestMapping = [...listVendorOrderProcurementRequestMapping, vendorOrderProcurementRequestMapping];
        });
      }

      let listVendorOrderCostDetail = [];
      if (this.listCostModel.length > 0) {
        this.listCostModel.forEach(item => {
          let vendorOrderCostDetail = new VendorOrderCostDetail();

          vendorOrderCostDetail.vendorOrderCostDetailId = this.emptyGuid;
          vendorOrderCostDetail.costId = item.costId;
          vendorOrderCostDetail.vendorOrderId = vendorOrder.vendorOrderId;
          vendorOrderCostDetail.unitPrice = item.unitPrice;
          vendorOrderCostDetail.costName = item.costName;
          vendorOrderCostDetail.active = true;
          vendorOrderCostDetail.createdById = this.auth.UserId;
          vendorOrderCostDetail.createdDate = new Date();

          listVendorOrderCostDetail = [...listVendorOrderCostDetail, vendorOrderCostDetail];
        });
      }

      this.loading = true;
      let result: any = await this.vendorService.updateVendorOrderByIdAsync(vendorOrder, listVendorOrderDetail, this.auth.UserId, isSendApproval,
        listVendorOrderProcurementRequestMapping, listVendorOrderCostDetail);
      this.loading = false;
      if (result.statusCode == 200 || result.statusCode == 202) {
        this.listItemInvalidModel = result.listItemInvalidModel;

        if (this.listItemInvalidModel.length > 0) {
          this.isInvalidItem = true;
          this.isOpenPopupInvalidItem = true;

          //Highlight các item bị lỗi
          let listProcurementRequestItemId = this.listItemInvalidModel.map(x => x.procurementRequestItemId);
          this.listVendorOrderDetail.forEach(item => {
            if (listProcurementRequestItemId.includes(item.procurementRequestItemId)) {
              item.isInvalidItemRequest = true;
            }
          });
        } else {
          this.isInvalidItem = false;
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Chỉnh sửa đơn hàng mua thành công' };
          this.showMessage(msg);
          this.getMasterData();
        }
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    }
  }

  mapFormtoVendorOrder(): VendorOrderModel {
    let vendorOrderModel: VendorOrderModel = new VendorOrderModel();
    vendorOrderModel.vendorOrderId = this.vendorOrderById.vendorOrderId;
    //mã đơn hàng
    vendorOrderModel.vendorOrderCode = this.vendorOrderById.vendorOrderCode;
    //status
    let status: PurchaseOrderStatus = this.orderStatusControl.value;
    vendorOrderModel.statusId = status.purchaseOrderStatusId;

    let orderDate = this.orderDateControl.value;
    vendorOrderModel.vendorOrderDate = orderDate ? convertToUTCTime(orderDate) : null; // new Date();

    vendorOrderModel.customerOrderId = null;
    //người order
    let orderer: employeeModel = this.ordererControl.value;
    vendorOrderModel.orderer = orderer ? orderer.employeeId : null;
    vendorOrderModel.description = this.descriptionControl.value.trim();
    vendorOrderModel.note = this.noteControl.value.trim();
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

    //thông giao hàng
    // vendorOrderModel.recipientName = this.recipientNameControl.value.trim();
    vendorOrderModel.shipperName = this.shipperNameControl.value.trim();
    vendorOrderModel.locationOfShipment = this.locationOfShipmentControl.value.trim();
    vendorOrderModel.placeOfDelivery = this.placeOfDeliveryControl.value.trim();
    vendorOrderModel.recipientPhone = this.recipientPhoneControl.value.trim();
    vendorOrderModel.recipientEmail = this.recipientEmailControl.value.trim();
    vendorOrderModel.shippingNote = this.shippingNoteControl.value.trim();

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
      vendorOrderDetail.vendorOrderId = this.vendorOrderById.vendorOrderId;
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
        productAttr.vendorOrderProductDetailProductAttributeValueId = this.emptyGuid;
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

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
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
    if (this.actionEdit && this.statusCode == 'DRA') {
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

            let sumAmountDiscount = 0;
            if (item.discountType == true) {
              sumAmountDiscount = this.roundNumber(((item.discountValue * item.quantity * item.unitPrice * item.exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
            } else {
              sumAmountDiscount = item.discountValue;
            }

            //Giá nhập kho cho từng sản phẩm
            item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount + item.cost) / item.quantity, 0);

            //Giá trị nhập kho cho từng sản phẩm
            item.priceValueWarehouse = this.roundNumber(item.quantity * item.priceWarehouse, 0);
          });
        }
        //Nếu Theo giá trị
        else if (this.selectedCostType == '1') {
          //Lấy tổng thành tiền
          let totalAmount = 0;
          this.listVendorOrderDetail.forEach(item => {
            let sumAmountDiscount = 0;
            if (item.discountType == true) {
              sumAmountDiscount = this.roundNumber(((item.discountValue * item.quantity * item.unitPrice * item.exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
            } else {
              sumAmountDiscount = item.discountValue;
            }

            totalAmount += (item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount);
            item.isEditCost = false;
          });

          //Tính Chi phí mua hàng, Giá nhập kho, Giá trị nhập kho
          this.listVendorOrderDetail.forEach(item => {
            let sumAmountDiscount = 0;
            if (item.discountType == true) {
              sumAmountDiscount = this.roundNumber(((item.discountValue * item.quantity * item.unitPrice * item.exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
            } else {
              sumAmountDiscount = item.discountValue;
            }

            //Chi phí mua hàng cho từng sản phẩm
            item.cost = this.roundNumber(totalCost * (item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount) / totalAmount, 0);

            //Giá nhập kho cho từng sản phẩm
            item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount + item.cost) / item.quantity, 0);

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

        let sumAmountDiscount = 0;
        if (this.dataRow.discountType == true) {
          sumAmountDiscount = this.roundNumber(((this.dataRow.discountValue * this.dataRow.quantity * this.dataRow.unitPrice * this.dataRow.exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
        } else {
          sumAmountDiscount = this.dataRow.discountValue;
        }

        //giá nhập kho:
        this.dataRow.priceWarehouse = this.roundNumber((this.dataRow.quantity * this.dataRow.unitPrice * this.dataRow.exchangeRate - sumAmountDiscount + this.dataRow.cost) / this.dataRow.quantity, 0);

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

          let sumAmountDiscount = 0;
          if (item.discountType == true) {
            sumAmountDiscount = this.roundNumber(((item.discountValue * item.quantity * item.unitPrice * item.exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
          } else {
            sumAmountDiscount = item.discountValue;
          }
          reaminTotalAmount += (item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount);
        });

        this.listVendorOrderDetail.forEach((item, index) => {
          if (item.isEditCost != true) {
            if (this.selectedCostType == '0') {
              //chi phí mua hàng:
              item.cost = this.roundNumber(remainTotalCost * item.quantity / remainTotalQuantity, 0);

              let sumAmountDiscount = 0;
              if (item.discountType == true) {
                sumAmountDiscount = this.roundNumber(((item.discountValue * item.quantity * item.unitPrice * item.exchangeRate) / 100), parseInt(this.defaultNumberType, 10));
              } else {
                sumAmountDiscount = item.discountValue;
              }

              //giá nhập kho:
              item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount + item.cost) / item.quantity, 0);

              //giá trị nhập kho:
              item.priceValueWarehouse = this.roundNumber(item.quantity * item.priceWarehouse, 0);
            } else if (this.selectedCostType == '1') {
              //chi phí mua hàng:
              item.cost = this.roundNumber(remainTotalCost * (item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount) / reaminTotalAmount, 0);

              //giá nhập kho:
              item.priceWarehouse = this.roundNumber((item.quantity * item.unitPrice * item.exchangeRate - sumAmountDiscount + item.cost) / item.quantity, 0);

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

  /* BEGIN: Chức năng Ghi chú */

  /* Event thay đổi nội dung ghi chú */
  currentTextChange: string = '';
  changeNoteContent(event) {
    this.currentTextChange = event.textValue;
  }

  cancelNote() {
    this.translate.get('procurement-request.messages_confirm.cancel_confirm_note').subscribe(value => { this.messageTitle = value; });
    this.confirmationService.confirm({
      message: this.messageTitle,
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();
        }
        this.listUpdateFileNote = [];
        this.isEditNote = false;
      }
    });
  }

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
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

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedFiles = [];
  }

  /*Event khi xóa 1 file trong comment đã lưu trên server*/
  deleteNoteFile(file: FileInFolder) {
    this.translate.get('procurement-request.messages_confirm.delete_confirm').subscribe(value => { this.messageTitle = value; });
    this.confirmationService.confirm({
      message: this.messageTitle,
      accept: () => {
        let index = this.listUpdateFileNote.indexOf(file);
        this.listUpdateFileNote.splice(index, 1);
      }
    });
  }

  downloadNoteFile(fileInfor: NoteDocument) {
    this.imageService.downloadFile(fileInfor.documentName, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;
      var name = fileInfor.documentName;

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
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    this.loading = true;

    let noteModel = new NoteModel();

    /*Tạo mới ghi chú*/
    if (!this.noteId) {
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.vendorOrderId;
      noteModel.ObjectType = 'VENDORORDER';
      noteModel.NoteTitle = this.messageTitle;
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    /*Update ghi chú*/
    else {
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.vendorOrderId;
      noteModel.ObjectType = 'VENDORORDER';
      noteModel.NoteTitle = this.messageTitle;
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }

    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectId = this.emptyGuid;
      fileUpload.FileInFolder.objectType = 'NOTE';
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload)
    });

    this.noteService.createNoteForObject(noteModel, listFileUploadModel, "QLMHCON").subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        /*Reshow Time Line*/
        this.noteHistory = result.listNote;

        this.uploadedFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateFileNote = [];
        this.noteId = null;
        this.isEditNote = false;

        this.handleNoteContent();

        let msg = { severity: 'success', summary: 'Thông báo', detail: 'Thêm ghi chú thành công' };
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

        // $('#' + element.noteId).find('.note-title').append($.parseHTML(element.noteTitle));
        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateFileNote = this.noteHistory.find(x => x.noteId == this.noteId).listFile;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.translate.get('procurement-request.messages_confirm.delete_confirm_note').subscribe(value => { this.messageTitle = value; });
    this.confirmationService.confirm({
      message: this.messageTitle,
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);

            this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
            this.translate.get('procurement-request.create.del_note_success').subscribe(value => { this.messageConfirm = value; });
            let msg = { severity: 'success', summary: this.messageTitle, detail: this.messageConfirm };
            this.showMessage(msg);
          } else {
            this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
            let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
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
      this.translate.get('procurement-request.create.hiden').subscribe(value => { this.toggle_note_label = value; });
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.translate.get('procurement-request.create.show').subscribe(value => { this.toggle_note_label = value; });
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

  /* Nếu là ảnh thì mở tab mới, nếu là tài liệu thì download */
  openItem(name, fileExtension, fileInFolderId) {
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

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  /* END: Chức năng Ghi chú */


  /* Xóa Đơn hàng mua */
  removeVendorOrder() {
    this.confirmationService.confirm({
      message: "Bạn có chắc chắn muốn xóa Đơn hàng mua này?",
      accept: () => {
        this.loading = true;
        this.vendorService.removeVendorOrder(this.vendorOrderId).subscribe(response => {
          let result: any = response;
          this.loading = false;

          if (result.statusCode == 200) {
            this.router.navigate(['vendor/list-order']);
          } else {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(msg);
          }
        });
      }
    });
  }

  /* Hủy đơn hàng mua */
  cancelVendorOrder() {
    //Chuyển đơn hàng mua sang trạng thái Hủy
    this.vendorService.cancelVendorOrder(this.vendorOrderId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /* Chuyển đơn hàng mua về trạng thái Mới tạo */
  draftVendorOrder(isCancelApproval: boolean) {
    this.vendorService.draftVendorOrder(this.vendorOrderId, isCancelApproval).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        this.getMasterData();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /* Mở popup Tạo nhanh nhà cung cấp */
  openQuickCreVendorModal() {
    //this.displayModal = true;
    let ref = this.dialogService.open(QuickCreateVendorComponent, {
      data: {},
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

  /* Cập nhật lại số lượng thực tế phê duyệt */
  updateQuantityItem() {
    if (this.listItemInvalidModel.length > 0) {
      /* #region  Xóa các item đã đặt hết số lượng */
      let listItemInvalidRemove = this.listItemInvalidModel.filter(x => {
        if (x.remainQuantity == 0) {
          return true;
        }
      }).map(x => x.procurementRequestItemId);
      listItemInvalidRemove.forEach(_itemRequestId => {
        let vendorDetail = this.listVendorOrderDetail.find(x => x.procurementRequestItemId == _itemRequestId);

        /* #region  Xóa item trong list sản phẩm và tính toán lại tổng số tiền */
        this.listVendorOrderDetail = this.listVendorOrderDetail.filter(e => e != vendorDetail);
        this.allocation();
        this.getSumarySection();
        this.setValidatorForDiscount();

        //Kiểm tra list sản phẩm trong phiếu đề xuất đã bị xóa hết trong danh sách sp/dv chưa?
        if (vendorDetail.procurementRequestId) {
          let listSelected = this.procurementRequestControl.value;
          if (listSelected.length > 0) {
            let hasValue = this.listVendorOrderDetail.find(x => x.procurementRequestId == vendorDetail.procurementRequestId);

            //Nếu không còn sản phẩm nào trong phiếu đề xuất thì bỏ checked trong listSelected
            if (!hasValue) {
              listSelected = listSelected.filter(x => x.procurementRequestId != vendorDetail.procurementRequestId);
              this.procurementRequestControl.setValue(listSelected);
            }
          }
        }
        /* #endregion */

        listItemInvalidRemove = listItemInvalidRemove.filter(x => x != _itemRequestId);
      });
      /* #endregion */

      /* #region  Cập nhật lại số lượng các item */
      let listChangeItem = this.listItemInvalidModel.filter(x => { if (x.remainQuantity > 0) return true });
      if (listChangeItem.length > 0) {
        this.listVendorOrderDetail.forEach(item => {
          let itemChange = listChangeItem.find(x => x.procurementRequestItemId == item.procurementRequestItemId);

          if (itemChange) {
            item.quantity = itemChange.remainQuantity;

            //Tính lại thành tiền sau thuế
            let amount = item.quantity * item.exchangeRate * item.unitPrice;
            let discount = 0;
            if (item.discountType) {
              discount = this.roundNumber(amount * item.discountValue / 100, parseInt(this.defaultNumberType, 10));
            } else {
              discount = item.discountValue;
            }
            let amountVat = this.roundNumber((amount - discount) * item.vat / 100, parseInt(this.defaultNumberType, 10));
            item.sumAmount = amount - discount + amountVat;
            item.sumAmountDiscount = discount;

            item.isInvalidItemRequest = false;
          }
        });

        this.allocation();
        this.getSumarySection();
        this.setValidatorForDiscount();
      }
      /* #endregion */
    }
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
};

function convertDateToTimeSpan(Time: Date): string {
  let result = '';
  let Hour = Time.getHours().toString();
  let Minute = Time.getMinutes().toString();
  result = Hour + ":" + Minute;

  return result;
}

function convertTimeSpanToDate(TimeSpan: string): Date {
  let result = new Date();
  let index = TimeSpan.indexOf(':');
  let hour = parseInt(TimeSpan.slice(0, index), 10);
  let minute = parseInt(TimeSpan.slice(index + 1, TimeSpan.length), 10);
  result.setHours(hour);
  result.setMinutes(minute);

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
