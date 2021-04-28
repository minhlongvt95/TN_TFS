import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { ProcurementRequestItemModel } from '../../models/procurementRequestItem.model';
import { ProcurementRequestModel } from '../../models/procurementRequest.model';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProcurementRequestService } from '../../services/procurement-request.service';
import * as $ from 'jquery';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { CreateRequestItemPopupComponent } from '../create-request-item-popup/create-request-item-popup.component';
import { OrganizationDialogComponent } from "../../../shared/components/organization-dialog/organization-dialog.component";
import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';
import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { FileUpload } from 'primeng/fileupload';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import { NoteService } from '../../../shared/services/note.service';
import { ForderConfigurationService } from '../../../admin/components/folder-configuration/services/folder-configuration.service';
import { EmailConfigService } from '../../../admin/services/email-config.service';
import { SendEmailModel } from '../../../admin/models/sendEmail.model';
import { DecimalPipe } from '@angular/common';

class employeeModel {
  employeeCode: string;
  employeeId: string;
  employeeName: string;
  positionId: string;
  positionName: string;
}

interface ProductVendorMapping {
  productVendorMappingId: string;
  vendorId: string;
  vendorName: string;
  vendorCode: string;
  productCode: string;
  productId: string;
  productName: string;
  productUnitName: string;
  vendorProductName: string;
  miniumQuantity: number;
  price: number;
  moneyUnitId: string;
  moneyUnitName: string;
  fromDate: Date;
  toDate: Date;
  createdById: string;
  createdDate: Date;
  listSuggestedSupplierQuoteId: Array<string>
}

class selectedOrganizationModel {
  selectedOrgId: string;
  selectedOrgName: string
}

class procurementRequestModel {
  procurementRequestId: string;
  procurementCode: string;
  procurementContent: string;
  requestEmployeeId: string;
  employeePhone: string;
  unit: string;
  approverId: string;
  approverPostion: string;
  approverPostionName: string;
  explain: string;
  statusId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  requestEmployeeName: string;
  approverName: string;
  statusName: string;
  statusCode: string;
  organizationName: string;
  totalMoney: number;
  vendorName: string;
  orderId: string;
}

class procurementRequestItemModel {
  procurementRequestItemId: string;
  productId: string;
  vendorId: string;
  quantity: number;
  quantityApproval: number;
  unitPrice: number;
  currencyUnit: string;
  exchangeRate: number;
  procurementRequestId: string;
  procurementPlanId: string;
  createdById: string;
  createdDate: Date;
  //label name
  vendorName: string;
  productName: string;
  productCode: string;
  procurementPlanCode: string;
  productUnit: string;
  amount: number;
  description: string;
  orderDetailType: string;
  discountType: boolean;
  discountValue: number;
  vat: number;
  unitId: string;
  nameMoneyUnit: string;
  incurredUnit: string;
  orderNumber: number;
}

class documentModel {
  documentId: string;
  name: string;
  objectId: string;
  extension: string;
  size: number;
  documentUrl: string;
  header: string;
  contentType: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  active: boolean;
}

class viewCardModel {
  statusName: string;
  procurementRequestCode: string;
  createdDate: Date;
  procurementRequestContent: string;
  requestEmployee: string; //chuối mã - tên nhân viên
  phone: string;
  unit: string; //phòng ban
  approverEmployee: string; //chuối mã - tên nhân viên
  position: string;
  desc: string;
  orderCode: string;
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

// data đơn vị tiền
class moneyUnit {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
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

interface FileNameExists {
  oldFileName: string;
  newFileName: string
}

@Component({
  selector: 'app-procurement-request-view',
  templateUrl: './procurement-request-view.component.html',
  styleUrls: ['./procurement-request-view.component.css'],
  providers: [
    ConfirmationService,
    MessageService,
    DialogService,
    DecimalPipe
  ]
})
export class ProcurementRequestViewComponent implements OnInit {
  @ViewChild('fileNoteUpload', { static: true }) fileNoteUpload: FileUpload;
  @ViewChild('fileUpload') fileUpload: FileUpload;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;  //Số chữ số thập phân sau dấu phẩy
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  //dialog
  selectedOrganization: selectedOrganizationModel = new selectedOrganizationModel;
  //routing
  procurementId: string;
  //master data
  isWorkFlowInActive: boolean;
  listApproverEmployeeId: Array<employeeModel> = [];
  procurementRequestModel: procurementRequestModel;
  listProcurementRequestItem: Array<procurementRequestItemModel> = [];
  listDocument: Array<documentModel> = [];
  listOrder: Array<any> = [];
  listOrderDetail: Array<any> = [];
  //form
  createdDate: Date = new Date();
  createPRForm: FormGroup;
  //talbe
  rows: number = 10;
  columns: Array<any> = [];
  selectedCulumns: Array<any> = [];
  selectedProcurementRequestItem: procurementRequestItemModel;
  colsFile: any;
  //file upload 
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  defaultAvatar: string = '/assets/images/no-avatar.png';
  orderObjProRequest: any = null;
  //toggle value
  viewProcurementRequest: viewCardModel = new viewCardModel();
  isViewProcurement: boolean = true;
  display: boolean = false;
  isApproval: boolean = false;
  isMesenger: boolean = false;
  descriptionApprovalReject: string = '';
  //sumary section
  amountNumber: number = 0;
  amountText: string = '';
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;

  listVendorProductPrice: Array<ProductVendorMapping> = [];
  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  noteHistory: Array<Note> = [];
  colsNoteFile: Array<any> = [];

  messageConfirm: string = '';
  messageTitle: string = '';
  messageError: string = '';

  isManager: boolean = false;
  listEmailSendTo: Array<string> = [];
  isUserSendAproval: boolean = false; // có phải người gửi phê duyệt không

  quantityApprovalValid: number = 0;

  listMoneyUnit: Array<moneyUnit> = [];
  unitMoneyLabel: string = 'VND';

  constructor(private translate: TranslateService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private prService: ProcurementRequestService,
    private imageService: ImageUploadService,
    private messageService: MessageService,
    private dialogService: DialogService,
    private noteService: NoteService,
    private confirmationService: ConfirmationService,
    private folderService: ForderConfigurationService,
    private emailConfigService: EmailConfigService,
  ) { }

  async ngOnInit() {
    let resource = "buy/procurement-request/view/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.procurementRequestModel = new procurementRequestModel();
      this.procurementRequestModel.statusCode = ''
      this.initForm();
      this.initTable();
      this.route.params.subscribe(params => { this.procurementId = params['id'] });
      this.getMasterData();
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.createPRForm = new FormGroup({
      'PRContent': new FormControl('', [Validators.required]),
      'ApproverEmp': new FormControl(null), //, [Validators.required]
      'RequestEmp': new FormControl(null, [Validators.required]),
      'Postion': new FormControl(null),
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      'Organization': new FormControl(null), //, [Validators.required]),
      'Desc': new FormControl(''),
      'Status': new FormControl(''),
      'ProcurementRequestCode': new FormControl(''),
      'CreatedDate': new FormControl('')
    });
  }

  initTable() {
    this.colsNoteFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'left' },
      { field: 'updatedDate', header: 'Ngày tạo', width: '50%', textAlign: 'left' },
    ];

    this.columns = [
      { field: 'productName', header: 'Tên sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productCode', header: 'Mã sản phẩm', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'vendorName', header: 'Nhà cung cấp', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productUnit', header: 'Đơn vị tính', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'quantityApproval', header: 'Số lượng duyệt', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'procurementPlanCode', header: 'Mã dự toán', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'unitPrice', header: 'Đơn giá', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'exchangeRate', header: 'Tỷ giá', width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'amount', header: 'Thành tiền(VND)', width: '150px', textAlign: 'right', color: '#f44336' }
    ];
    this.selectedCulumns = this.columns;
    /*Table*/
    this.colsFile = [
      { field: 'name', header: 'Tên tài liệu', width: '50%', textAlign: 'left' },
      { field: 'size', header: 'Kích thước tài liệu', width: '50%', textAlign: 'left' },
    ];
  }

  async getMasterData() {
    this.loading = true;
    this.amountNumber = 0;
    let result: any = await this.prService.getDataEditProcurementRequest(this.procurementId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.isWorkFlowInActive = result.isWorkFlowInActive;
      this.listApproverEmployeeId = result.listApproverEmployeeId;
      this.listEmailSendTo = result.listEmailSendTo;
      this.procurementRequestModel = result.procurementRequestEntityModel;
      this.listProcurementRequestItem = result.listProcurementRequestItemEntityModel;
      // this.listVendorProductPrice = result.listVendorProductPrice;
      this.listProcurementRequestItem.forEach((val, index) => {
        val.orderNumber = val.orderNumber ? val.orderNumber : index + 1;

        if(val.orderDetailType == '1'){ 
          val.productName = val.description;
          val.productUnit = val.incurredUnit;
        }
        if (val.quantityApproval == null) {
          val.quantityApproval = 0;
        }
      })
      this.listDocument = result.listDocumentModel;
      this.listOrder = result.listOrder;
      this.listOrderDetail = result.listOrderDetail;
      this.noteHistory = result.listNote;

      if (this.auth.UserId == this.procurementRequestModel.updatedById) {
        this.isUserSendAproval = true;
      }

      this.patchDataToView(this.procurementRequestModel);
      this.patchDataToForm(this.procurementRequestModel);

      this.handleNoteContent();
    } else {
      this.clearToast();
      this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
      this.showToast('error', this.messageTitle, result.messageCode);
    }
    // this.prService.searchVendorProductPrice(productId, vendorId, quantity).subscribe(response => {
    //   let result = <any>response;
    //   this.loading = false;
    //   if (result.statusCode == 200) {
    //     var vendorProductPrice = result.vendorProductPrice
    //     if (vendorProductPrice) {
    //       this.createPRItemForm.get('Price').setValue(vendorProductPrice.price);
    //     } else {
    //       this.createPRItemForm.get('Price').setValue(0);
    //     }
    //     this.calculateAmount();
    //   }
    // });
  }

  changeQuantityApproval() {
    this.listProcurementRequestItem.forEach(element => {
      if (element.quantityApproval === null || element.quantityApproval === undefined || element.quantityApproval.toString() === '') {
        element.quantityApproval = 0;
      }
      else {
        let quantityApproval = parseFloat(element.quantityApproval.toString().replace(/,/g, ''));
        let quantity = parseFloat(element.quantity.toString().replace(/,/g, ''));
        if (quantityApproval > quantity) {
          element.quantityApproval = element.quantity;
        }
      }
    });
  }

  patchDataToView(procurementRequestModel: procurementRequestModel) {
    /*  viewProcurementRequest */
    this.viewProcurementRequest.statusName = procurementRequestModel.statusName;
    this.viewProcurementRequest.procurementRequestCode = procurementRequestModel.procurementCode;
    this.viewProcurementRequest.createdDate = procurementRequestModel.createdDate;
    this.viewProcurementRequest.procurementRequestContent = procurementRequestModel.procurementContent;
    this.viewProcurementRequest.requestEmployee = procurementRequestModel.requestEmployeeName;
    this.viewProcurementRequest.phone = procurementRequestModel.employeePhone;
    /* đơn vị */
    this.viewProcurementRequest.unit = procurementRequestModel.organizationName;
    /* người phê duyệt */
    let _approver = this.listApproverEmployeeId.find(e => e.employeeId == procurementRequestModel.approverId);
    this.viewProcurementRequest.approverEmployee = _approver ? _approver.employeeCode + " - " + _approver.employeeName : "";
    this.viewProcurementRequest.position = procurementRequestModel.approverPostionName;
    this.viewProcurementRequest.desc = procurementRequestModel.explain;
    // if (procurementRequestModel.orderId !== '' && procurementRequestModel.orderId !== null && procurementRequestModel.orderId !== undefined) {
    //   this.viewProcurementRequest.orderCode = this.listOrder.find(o => o.orderId == procurementRequestModel.orderId).orderCode;
    // }
  }

  updateDataToView(procurementRequestModel: ProcurementRequestModel) {
    /* nội dung yêu cầu */
    this.viewProcurementRequest.procurementRequestContent = this.createPRForm.get('PRContent').value;
    /* người yêu cầu */
    this.viewProcurementRequest.requestEmployee = this.createPRForm.get('RequestEmp').value;
    /* số điện thoại */
    this.viewProcurementRequest.phone = this.createPRForm.get('Phone').value;
    /* đơn vị */
    this.viewProcurementRequest.unit = this.selectedOrganization.selectedOrgName;
    /* người phê duyệt */
    let _approver: employeeModel = this.createPRForm.get('ApproverEmp').value;
    this.viewProcurementRequest.approverEmployee = _approver ? _approver.employeeCode + " - " + _approver.employeeName : "";
    this.viewProcurementRequest.position = this.createPRForm.get('Postion').value;
    /* diễn giả */
    this.viewProcurementRequest.desc = this.createPRForm.get('Desc').value;
  }

  patchDataToForm(procurementRequestModel: procurementRequestModel) {
    this.createPRForm.get('Status').patchValue(procurementRequestModel.statusName);
    this.createPRForm.get('ProcurementRequestCode').patchValue(procurementRequestModel.procurementCode);
    this.createPRForm.get('CreatedDate').patchValue(procurementRequestModel.createdDate);

    this.createPRForm.get('PRContent').patchValue(procurementRequestModel.procurementContent);
    this.createPRForm.get('RequestEmp').patchValue(procurementRequestModel.requestEmployeeName);
    if (procurementRequestModel.employeePhone == 'null'){
      this.createPRForm.get('Phone').setValue('');
    }else{
      this.createPRForm.get('Phone').patchValue(procurementRequestModel.employeePhone);
    }

    /* người phê duyệt */
    let _approver = this.listApproverEmployeeId.find(e => e.employeeId == procurementRequestModel.approverId);
    this.createPRForm.get('ApproverEmp').patchValue(_approver ? _approver : null);
    this.createPRForm.get('Postion').patchValue(procurementRequestModel.approverPostionName);
    /* diễn giải  */
    this.createPRForm.get('Desc').patchValue(procurementRequestModel.explain);
    /* đơn vị */
    this.selectedOrganization = new selectedOrganizationModel();
    this.selectedOrganization.selectedOrgId = procurementRequestModel.unit;
    this.selectedOrganization.selectedOrgName = procurementRequestModel.organizationName;
    this.createPRForm.get('Organization').patchValue(this.selectedOrganization.selectedOrgName);

    this.createPRForm.get('RequestEmp').disable();
    this.createPRForm.get('Postion').disable();

    if (procurementRequestModel.orderId !== '' && procurementRequestModel.orderId !== null && procurementRequestModel.orderId !== undefined) {
      let orderObj = this.listOrder.find(o => o.orderId == procurementRequestModel.orderId);
      this.orderObjProRequest = orderObj;
    }

    /* tỉnh tổng đơn hàng */
    this.listProcurementRequestItem.forEach(e => {
      let discount = 0;
      let vat = 0;
      if (e.discountType) {
        discount = (e.quantity * e.unitPrice * e.exchangeRate * e.discountValue) / 100;
      }
      else {
        discount = e.discountValue;
      }

      vat = (e.quantity * e.unitPrice * e.exchangeRate - discount) * (e.vat / 100);
      e.amount = this.roundNumber((e.exchangeRate * e.quantity * e.unitPrice) - discount + vat, parseInt(this.defaultNumberType, 10));

      this.amountNumber += e.amount;
    });

    this.handdleAmount();
  }

  // fixed: boolean = false;
  // @HostListener('document:scroll', [])
  // onScroll(): void {
  //   let num = window.pageYOffset;
  //   if (num > 100) {
  //     this.fixed = true;
  //   } else {
  //     this.fixed = false;
  //   }
  // }

  fixed: boolean = false;
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
        this.withCol = $('#parentTH').width() + 190;
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width() + 30;
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }
  cancel() {
    this.router.navigate(['/procurement-request/list']);
  }

  toggleViewVisitCard() {
    this.isViewProcurement = !this.isViewProcurement;
  }

  gotoCreateQuoteVendor() {
    this.router.navigate(['/vendor/vendor-quote-create', { procurementRequestId: this.procurementId }]);
  }

  gotoCreateOrderPay() {
    this.loading = true;
    this.router.navigate(['/vendor/create-order', { procurementId: this.procurementId }]);
  }
  goDetaiOrder() {
    let url = this.router.serializeUrl(this.router.createUrlTree(['/order/order-detail', { customerOrderID: this.orderObjProRequest.orderId }]));
    window.open(url, '_blank');
  }
  async save() {
    if (!this.createPRForm.valid) {
      Object.keys(this.createPRForm.controls).forEach(key => {
        if (!this.createPRForm.controls[key].valid) {
          this.createPRForm.controls[key].markAsTouched();
        }
      });
    } else if (this.listProcurementRequestItem.length === 0) {
      this.clearToast();
      this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
      this.translate.get('procurement-request.messages_error.no_product').subscribe(value => { this.messageError = value; });
      this.showToast('error', this.messageTitle, this.messageError);
    } else {
      //valid
      let procurementRequestModel = this.mapFormToProcurementRequestModel();
      let listPRItem = this.getListProcurementRequstItem();
      let listDocumentId = this.listDocument.map(e => e.documentId);
      this.loading = true;
      let result: any = await this.prService.editProcurementRequestAsync(procurementRequestModel, listPRItem, listDocumentId, this.uploadedFiles, this.auth.UserId);
      this.loading = false;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.updateDataToView(procurementRequestModel);
        this.listDocument = result.listDocumentEntityModel;
        // Xóa file trong control
        this.uploadedFiles = [];
        if (this.fileUpload) {
          this.fileUpload.clear();
        }
        this.clearToast();
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        this.translate.get('procurement-request.create.edit_success').subscribe(value => { this.messageConfirm = value; });
        this.showToast('success', this.messageTitle, this.messageConfirm);
        this.isViewProcurement = true;
      } else {
        this.clearToast();
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        this.showToast('error', this.messageTitle, result.messageCode);
      }
    }
  }

  mapFormToProcurementRequestModel(): ProcurementRequestModel {
    let newPR = new ProcurementRequestModel();
    newPR.procurementRequestId = this.procurementId
    newPR.procurementCode = this.createPRForm.get('ProcurementRequestCode').value;
    newPR.procurementContent = this.createPRForm.get('PRContent').value;

    newPR.requestEmployeeId = this.procurementRequestModel.requestEmployeeId;

    newPR.employeePhone = this.createPRForm.get('Phone').value;

    newPR.unit = this.selectedOrganization.selectedOrgId;

    let _approver: employeeModel = this.createPRForm.get('ApproverEmp').value;
    if (_approver !== undefined && _approver !== null) {
      newPR.approverId = _approver.employeeId;
      newPR.approverPostion = _approver.positionId;
    }

    newPR.explain = this.createPRForm.get('Desc').value;

    newPR.statusId = this.emptyGuid;
    newPR.updatedById = this.auth.UserId;
    newPR.updatedDate = new Date();

    return newPR;
  }

  getListProcurementRequstItem(): Array<ProcurementRequestItemModel> {
    let result: Array<ProcurementRequestItemModel> = [];
    this.listProcurementRequestItem.forEach(item => {
      let newItem = new ProcurementRequestItemModel();
      newItem.procurementRequestItemId = item.procurementRequestItemId;
      newItem.productId = item.productId;
      newItem.vendorId = item.vendorId;
      newItem.quantity = item.quantity;
      newItem.quantityApproval = item.quantityApproval;
      newItem.unitPrice = item.unitPrice;
      newItem.currencyUnit = item.currencyUnit;
      newItem.exchangeRate = item.exchangeRate;
      newItem.procurementPlanId = item.procurementPlanId;
      newItem.description = item.description;
      newItem.createdById = this.auth.UserId;
      newItem.createdDate = new Date();
      newItem.incurredUnit = item.incurredUnit;
      newItem.discountType = item.discountType;
      newItem.discountValue = item.discountValue;
      newItem.orderDetailType = item.orderDetailType;
      newItem.orderNumber = item.orderNumber;
      result.push(newItem);
    });
    return result;
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteFile(file: documentModel) {
    this.translate.get('procurement-request.messages_confirm.delete_confirm').subscribe(value => { this.messageConfirm = value; });
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: () => {
        this.listDocument = this.listDocument.filter(e => e != file);
      }
    });
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: documentModel) {
    this.imageService.downloadFile(fileInfor.name, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
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
          anchor.download = fileInfor.name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    });
  }

  openOrganizationDialog() {
    this.translate.get('procurement-request.create.unit_choose').subscribe(value => { this.messageTitle = value; });
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      header: this.messageTitle,
      data: {
        chooseFinancialIndependence: false
      },
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.selectedOrganization.selectedOrgId = result.selectedOrgId;
          this.selectedOrganization.selectedOrgName = result.selectedOrgName;
          this.createPRForm.get('Organization').patchValue(this.selectedOrganization.selectedOrgName);
        }
      }
    });
  }

  addPRItem() {
    this.translate.get('procurement-request.create.create_product').subscribe(value => { this.messageTitle = value; });
    let ref = this.dialogService.open(CreateRequestItemPopupComponent, {
      header: this.messageTitle,
      data: {
        
      },
      width: '75%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          let prItem: procurementRequestItemModel = result.prItem;
          prItem.orderNumber = this.listProcurementRequestItem.length + 1;
          this.listProcurementRequestItem = [...this.listProcurementRequestItem, prItem];
          this.amountNumber += prItem.amount;
          prItem.quantityApproval = 0;
          this.handdleAmount();
        }
      }
    });
  }

  editPRItem(rowData: procurementRequestItemModel) {
    this.translate.get('procurement-request.create.create_product').subscribe(value => { this.messageTitle = value; });
    let ref = this.dialogService.open(CreateRequestItemPopupComponent, {
      header: this.messageTitle,
      data: {
        isEdit: true,
        prItemEdit: rowData,
      },
      width: '75%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          let prItem: procurementRequestItemModel = result.prItem;
          //replace rowdata cũ
          const index = this.listProcurementRequestItem.indexOf(rowData);
          this.listProcurementRequestItem[index] = prItem;
          this.listProcurementRequestItem = [...this.listProcurementRequestItem];

          this.amountNumber = this.amountNumber - rowData.amount + prItem.amount;
          this.handdleAmount();
        }
      }
    });
  }

  deletePRItem(rowData: procurementRequestItemModel) {
    this.listProcurementRequestItem = this.listProcurementRequestItem.filter(e => e != rowData);
    //Đánh lại số OrderNumber
    this.listProcurementRequestItem.forEach((item, index) => {
      item.orderNumber = index + 1;
    });
    this.amountNumber = 0;
    this.listProcurementRequestItem.forEach(x => {
      this.amountNumber += x.amount;
    })
  }

  handdleAmount() {
    const moneyPipe = new NumberToStringPipe();
    this.amountText = moneyPipe.transform(this.amountNumber, this.defaultNumberType);
  }

  gotoBudgetList() {
    this.router.navigate(['/accounting/budget-list']);
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
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

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
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

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }

  // Event thay đổi nội dung ghi chú
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
        this.listUpdateNoteDocument = [];
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
  deleteNoteFile(file: NoteDocument) {
    this.translate.get('procurement-request.messages_confirm.delete_confirm').subscribe(value => { this.messageTitle = value; });
    this.confirmationService.confirm({
      message: this.messageTitle,
      accept: () => {
        let index = this.listUpdateNoteDocument.indexOf(file);
        this.listUpdateNoteDocument.splice(index, 1);
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
    this.listNoteDocumentModel = [];

    /*Upload file mới nếu có*/
    if (this.uploadedFiles.length > 0) {
      let listFileNameExists: Array<FileNameExists> = [];
      let result: any = await this.imageService.uploadFileForOptionAsync(this.uploadedFiles, 'PROCREQ');

      listFileNameExists = result.listFileNameExists;

      for (var x = 0; x < this.uploadedFiles.length; ++x) {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = this.uploadedFiles[x].name;
        let fileExists = listFileNameExists.find(f => f.oldFileName == this.uploadedFiles[x].name);
        if (fileExists) {
          noteDocument.DocumentName = fileExists.newFileName;
        }
        noteDocument.DocumentSize = this.uploadedFiles[x].size.toString();
        this.listNoteDocumentModel.push(noteDocument);
      }
    }

    let noteModel = new NoteModel();

    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      this.translate.get('procurement-request.create.text_add_note').subscribe(value => { this.messageTitle = value; });
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.procurementId;
      noteModel.ObjectType = 'PROCREQ';
      noteModel.NoteTitle = this.messageTitle;
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/

      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.procurementId;
      noteModel.ObjectType = 'PROCREQ';
      noteModel.NoteTitle = this.messageTitle;
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();

      //Thêm file cũ đã lưu nếu có
      this.listUpdateNoteDocument.forEach(item => {
        let noteDocument = new NoteDocumentModel();
        noteDocument.DocumentName = item.documentName;
        noteDocument.DocumentSize = item.documentSize;
        noteDocument.UpdatedById = item.updatedById;
        noteDocument.UpdatedDate = item.updatedDate;

        this.listNoteDocumentModel.push(noteDocument);
      });
    }

    this.noteService.createNoteForOrderDetail(noteModel, this.listNoteDocumentModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        /*Reshow Time Line*/
        this.noteHistory = result.listNote;
        let file = result.noteId;
        let listFileUploadModel: Array<FileUploadModel> = [];
        this.uploadedFiles.forEach(item => {
          let fileUpload: FileUploadModel = new FileUploadModel();
          fileUpload.FileInFolder = new FileInFolder();
          fileUpload.FileInFolder.active = true;
          let index = item.name.lastIndexOf(".");
          let name = item.name.substring(0, index);
          fileUpload.FileInFolder.fileName = name;
          fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
          fileUpload.FileInFolder.size = item.size;
          fileUpload.FileInFolder.objectId = file;
          fileUpload.FileInFolder.objectType = 'NOTE';
          fileUpload.FileSave = item;
          listFileUploadModel.push(fileUpload)
          // });

          this.folderService.uploadFileByFolderType("QLDXMH", listFileUploadModel, file).subscribe(response => {
            let result: any = response;
            this.loading = false;
            if (result.statusCode == 200) {
              // this.listDetailFolder[0].listFile;
              this.uploadedFiles = [];
              // this.fileNoteUpload.files = [];
              this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
              this.translate.get('procurement-request.create.add_file_success').subscribe(value => { this.messageConfirm = value; });
              let msg = { severity: 'success', summary: this.messageTitle, detail: this.messageConfirm };
              this.showMessage(msg);
            } else {
              this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
              let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        });

        this.uploadedFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
        }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;

        this.handleNoteContent();
        this.getMasterData();

        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        this.translate.get('procurement-request.create.add_note_success').subscribe(value => { this.messageConfirm = value; });
        let msg = { severity: 'success', summary: this.messageTitle, detail: this.messageConfirm };
        this.showMessage(msg);
        // this.showDialogAddFile = false;
      } else {
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        let msg = { severity: 'error', summary: this.messageTitle, detail: result.messageCode };
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

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
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

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
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
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  approveOrReject(approver: boolean) {
    if (approver) {
      this.listProcurementRequestItem.forEach(val => {
        if (val.quantityApproval === null) {
          this.messageError = "Số lượng phê duyệt không được để trống";
        } else if (val.quantityApproval == 0) {
          this.messageError = "Số lượng phê duyệt không được bằng 0";
        } else {
          this.quantityApprovalValid++;
        }
      })
      if (this.quantityApprovalValid == this.listProcurementRequestItem.length) {
        if (approver === true) {
          this.translate.get('procurement-request.messages_confirm.confirm_approval').subscribe(value => { this.messageConfirm = value; });
        }
        this.confirmationService.confirm({
          message: this.messageConfirm,
          accept: async () => {
            this.isApproval = approver;
            this.isMesenger = false;
            this.descriptionApprovalReject = '';
            if (approver === null) {
              this.aprrovalRejectProcurement();
            } else {
              this.display = true;
            }
          }
        });
      } else {
        this.messageService.add({ severity: 'error', detail: this.messageError, summary: 'Thông báo' })
      }
      this.quantityApprovalValid = 0;
    } else {
      if (approver === false) {
        this.translate.get('procurement-request.messages_confirm.confirm_reject').subscribe(value => { this.messageConfirm = value; });
      }
      else {
        this.translate.get('procurement-request.messages_confirm.confirm_send_approval').subscribe(value => { this.messageConfirm = value; });
      }
      this.confirmationService.confirm({
        message: this.messageConfirm,
        accept: async () => {
          this.isApproval = approver;
          this.isMesenger = false;
          this.descriptionApprovalReject = '';
          if (approver === null) {
            this.aprrovalRejectProcurement();
          } else {
            this.display = true;
          }
        }
      });
    }
  }
  async aprrovalRejectProcurement() {
    if (this.isApproval == false) {
      if (this.descriptionApprovalReject === null || this.descriptionApprovalReject === undefined || this.descriptionApprovalReject.trim() === '') {
        this.isMesenger = true;
        return;
      }
    }
    let listPRItem = this.getListProcurementRequstItem();
    let result: any = await this.prService.approvalOrReject(this.procurementId, this.auth.UserId, this.isApproval, this.descriptionApprovalReject, listPRItem);
    this.clearToast();
    this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });

    //Gửi phê duyệt
    if (this.isApproval === null) {
      //Gửi email cho người có quyền phê duyệt
      let sendEmailModel = new SendEmailModel();
      sendEmailModel.OrganizationName = this.procurementRequestModel.organizationName;
      sendEmailModel.ProcurementCode = this.viewProcurementRequest.procurementRequestCode;
      sendEmailModel.ListSendToEmail = this.listEmailSendTo;

      this.emailConfigService.sendEmail(10, sendEmailModel).subscribe(response => {
        let result: any = response;
      });

      this.translate.get('procurement-request.create.send_approval_success').subscribe(value => { this.messageConfirm = value; });
      this.showToast('success', this.messageTitle, this.messageConfirm);
    }
    else {
      //Đồng ý Phê duyệt
      if (this.isApproval) {
        this.translate.get('procurement-request.create.approval_success').subscribe(value => { this.messageConfirm = value; });
        this.showToast('success', this.messageTitle, this.messageConfirm);
      }
      //Từ chối
      else {
        this.translate.get('procurement-request.create.reject_success').subscribe(value => { this.messageConfirm = value; });
        this.showToast('success', this.messageTitle, this.messageConfirm);
      }
    }
    this.getMasterData();
    this.display = false;
  }

  changeStatus(type) {
    if (type === "NEW") {
      this.translate.get('procurement-request.messages_confirm.confirm_edit_new').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "CANCEL_APPROVAL") {
      this.translate.get('procurement-request.messages_confirm.confirm_cancel_approval').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "DELETE") {
      this.translate.get('procurement-request.messages_confirm.delete_confirm_order').subscribe(value => { this.messageConfirm = value; });
    }
    if (type === "CANCEL") {
      this.translate.get('procurement-request.messages_confirm.confirm_cancel').subscribe(value => { this.messageConfirm = value; });
    }
    this.confirmationService.confirm({
      message: this.messageConfirm,
      accept: async () => {
        let result: any = await this.prService.changeStatus(this.procurementId, this.auth.UserId, type);
        this.clearToast();
        this.display = false;
        this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
        if (type === "NEW") {
          this.translate.get('procurement-request.create.new_success').subscribe(value => { this.messageConfirm = value; });
          this.showToast('success', this.messageTitle, this.messageConfirm);
          this.getMasterData();
        }
        if (type === "CANCEL") {
          this.translate.get('procurement-request.create.cancel_success').subscribe(value => { this.messageConfirm = value; });
          this.showToast('success', this.messageTitle, this.messageConfirm);
          this.getMasterData();
        }
        if (type === "DELETE") {
          this.router.navigate(['/procurement-request/list']);
        }
        if (type === "CANCEL_APPROVAL") {
          this.translate.get('procurement-request.create.cancel_approval_success').subscribe(value => { this.messageConfirm = value; });
          this.showToast('success', this.messageTitle, this.messageConfirm);
          this.getMasterData();
        }
      }
    });
  }
  changeDescript() {
    if (this.descriptionApprovalReject === null || this.descriptionApprovalReject === undefined || this.descriptionApprovalReject.trim() === '') {
      this.isMesenger = true;
    } else {
      this.isMesenger = false;
    }
  }

  /* Chuyển item lên một cấp */
  moveUp(data: procurementRequestItemModel) {
    let currentOrderNumber = data.orderNumber;
    let preOrderNumber = currentOrderNumber - 1;
    let pre_data = this.listProcurementRequestItem.find(x => x.orderNumber == preOrderNumber);

    //Đổi số OrderNumber của 2 item
    pre_data.orderNumber = currentOrderNumber;
    data.orderNumber = preOrderNumber;

    //Xóa 2 item
    this.listProcurementRequestItem = this.listProcurementRequestItem.filter(x =>
      x.orderNumber != preOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listProcurementRequestItem = [...this.listProcurementRequestItem, pre_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listProcurementRequestItem.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }

  /* Chuyển item xuống một cấp */
  moveDown(data: procurementRequestItemModel) {
    let currentOrderNumber = data.orderNumber;
    let nextOrderNumber = currentOrderNumber + 1;
    let next_data = this.listProcurementRequestItem.find(x => x.orderNumber == nextOrderNumber);

    //Đổi số OrderNumber của 2 item
    next_data.orderNumber = currentOrderNumber;
    data.orderNumber = nextOrderNumber;

    //Xóa 2 item
    this.listProcurementRequestItem = this.listProcurementRequestItem.filter(x =>
      x.orderNumber != nextOrderNumber && x.orderNumber != currentOrderNumber);

    //Thêm lại item trước với số OrderNumber đã thay đổi
    this.listProcurementRequestItem = [...this.listProcurementRequestItem, next_data, data];

    //Sắp xếp lại danh sách sản phẩm/dịch vụ theo số OrderNumber
    this.listProcurementRequestItem.sort((a, b) =>
      (a.orderNumber > b.orderNumber) ? 1 : ((b.orderNumber > a.orderNumber) ? -1 : 0));
  }
}
