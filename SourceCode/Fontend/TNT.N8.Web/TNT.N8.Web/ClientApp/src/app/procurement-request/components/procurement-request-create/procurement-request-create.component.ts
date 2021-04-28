import { Component, OnInit, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ProcurementRequestModel } from '../../models/procurementRequest.model';
import { ProcurementRequestItemModel } from '../../models/procurementRequestItem.model';
import { Router } from '@angular/router';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { ProcurementRequestService } from '../../services/procurement-request.service';
import { GetPermission } from '../../../shared/permission/get-permission';
import { OrganizationDialogComponent } from "../../../shared/components/organization-dialog/organization-dialog.component";
import { CreateRequestItemPopupComponent } from '../create-request-item-popup/create-request-item-popup.component';
import { NumberToStringPipe } from '../../../shared/ConvertMoneyToString/numberToString.pipe';
import { MessageService, ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { FileUpload } from 'primeng/fileupload';

class employeeModel {
  employeeCode: string;
  employeeId: string;
  employeeName: string;
  positionId: string;
  positionName: string;
  organizationName: string;
  organizationId: string;
}

class selectedOrganizationModel {
  selectedOrgId: string;
  selectedOrgName: string
}

class procurementRequestItemModel {
  procurementRequestItemId: string;
  productId: string;
  vendorId: string;
  quantity: number;
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
  nameMoneyUnit: string;
  discountType: boolean;
  discountValue: number;
  vat: number;
  unitId: string;
  incurredUnit: string;
  orderNumber: number;
}

@Component({
  selector: 'app-procurement-request-create',
  templateUrl: './procurement-request-create.component.html',
  styleUrls: ['./procurement-request-create.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class ProcurementRequestCreateComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;  //Số chữ số thập phân sau dấu phẩy
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionImport: boolean = true;
  //dialog
  selectedOrganization: selectedOrganizationModel = new selectedOrganizationModel;
  listProcurementRequestItem: Array<procurementRequestItemModel> = [];
  //form
  createdDate: Date = new Date();
  createPRForm: FormGroup;
  //masterdata
  currentEmployeeModel: employeeModel = new employeeModel();
  listApproverEmployeeId: Array<employeeModel> = [];
  //talbe
  rows: number = 10;
  organizationName: string = '';
  columns: Array<any> = [];
  selectedCulumns: Array<any> = [];
  selectedProcurementRequestItem: procurementRequestItemModel;
  //file upload
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  //sumary section
  amountNumber: number = 0;
  amountText: string = '';
  files: File[] = [];
  messageConfirm: string = '';
  messageTitle: string = '';
  messageError: string = '';

  //table
  colunmProductName: string;

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private router: Router,
    private messageService: MessageService,
    private dialogService: DialogService,
    private prService: ProcurementRequestService,
    private imageService: ImageUploadService) { }

  async ngOnInit() {
    let resource = "buy/procurement-request/create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("edit") == -1) {
        this.actionEdit = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }

      this.initForm();
      this.initTable();
      this.getMasterData();
    }
  }

  fixed: boolean = false;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
    } else {
      this.fixed = false;
    }
  }

  initTable() {
    this.columns = [
      { field: 'productName', header: "Tên sản phẩm", width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productCode', header: "Mã sản phẩm", width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'vendorName', header: "Nhà cung cấp", width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'procurementPlanCode', header: "Mã dự toán", width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'productUnit', header: "Đơn vị tính", width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'quantity', header: "Số lượng", width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'unitPrice', header: "Đơn giá", width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'exchangeRate', header: "Tỷ giá", width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'nameMoneyUnit', header: "Đơn vị tiền", width: '150px', textAlign: 'right', color: '#f44336' },
      { field: 'amount', header: "Thành tiền (VND)", width: '150px', textAlign: 'right', color: '#f44336' }
    ];
    this.selectedCulumns = this.columns;
  }

  initForm() {
    this.createPRForm = new FormGroup({
      'PRContent': new FormControl('', [Validators.required]),
      'ApproverEmp': new FormControl(null), //, [Validators.required]
      'RequestEmp': new FormControl(null, [Validators.required]),
      'Postion': new FormControl(null),
      'Phone': new FormControl('', [Validators.pattern(this.getPhonePattern())]),
      'Organization': new FormControl(null), //, [Validators.required]),
      'Desc': new FormControl('')
    });
  }

  patchDataToForm() {
    let _requestEmp = this.currentEmployeeModel.employeeCode + " - " + this.currentEmployeeModel.employeeName;
    this.createPRForm.get('RequestEmp').patchValue(_requestEmp);
    this.createPRForm.get('RequestEmp').disable();
    this.createPRForm.get('Postion').disable();
    this.createPRForm.get('Organization').disable();

    this.createPRForm.get('Postion').patchValue(this.currentEmployeeModel.positionName);
    this.createPRForm.get('Organization').patchValue(this.currentEmployeeModel.organizationName);
    this.organizationName = this.currentEmployeeModel.organizationName;
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  showErrorMessage(msg: Array<string>) {
    this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
    msg.forEach(item => {
      this.messageService.add({ severity: 'error', summary: this.messageTitle, detail: item });
    });
  }

  clearToast() {
    this.messageService.clear();
  }

  gotoBudgetList() {
    this.router.navigate(['/accounting/budget-list']);
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
        prItemEdit: rowData
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
          //trừ amount cũ, cộng amount mới
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
    this.amountNumber -= rowData.amount;
    this.handdleAmount();
  }

  handdleAmount() {
    const moneyPipe = new NumberToStringPipe();
    this.amountText = moneyPipe.transform(this.amountNumber, this.defaultNumberType);
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.prService.getDataCreateProcurementRequest(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.currentEmployeeModel = result.currentEmployeeModel;
      this.listApproverEmployeeId = result.listApproverEmployeeId;
      this.patchDataToForm();
    } else {
      this.clearToast();
      this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
      this.showToast('error', this.messageTitle, result.messageCode);
    }
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

  cancel() {
    this.router.navigate(['/procurement-request/list']);
  }

  save() {
    let messageError: Array<string> = [];
    //show message
    if (this.createPRForm.get('PRContent').hasError('required')) {
      this.translate.get('procurement-request.messages_error.no_input_content').subscribe(value => { this.messageError = value; });
      messageError.push(this.messageError);
    }

    if (messageError.length > 0) {
      this.clearToast();
      this.showErrorMessage(messageError);
    }

    if (!this.createPRForm.valid) {
      Object.keys(this.createPRForm.controls).forEach(key => {
        if (!this.createPRForm.controls[key].valid) {
          this.createPRForm.controls[key].markAsTouched();
          return;
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
      this.loading = true;
      this.prService.createProcurementRequest(this.uploadedFiles, procurementRequestModel, listPRItem, this.auth.UserId).subscribe(response => {
        this.loading = false;
        const result = <any>response;

        if (result.statusCode === 202 || result.statusCode === 200) {
          if (this.files.length > 0) {
            this.uploadFiles(this.files);
          }
          // this.router.navigate(['/procurement-request/list']);
          this.router.navigate(['/procurement-request/view', { id: result.procurementRequestId }])
        } else {
          this.clearToast();
          this.translate.get('procurement-request.messages_title.title_info').subscribe(value => { this.messageTitle = value; });
          this.showToast('error', this.messageTitle, result.messageCode);
        }
      }, error => { this.loading = false; });
    }
  }

  // Upload file to server
  uploadFiles(files: File[]) {
    this.imageService.uploadFile(files).subscribe(response => { }, error => { });
  }

  mapFormToProcurementRequestModel(): ProcurementRequestModel {
    let newPR = new ProcurementRequestModel();
    newPR.procurementRequestId = this.emptyGuid;
    newPR.procurementCode = '';
    newPR.procurementContent = this.createPRForm.get('PRContent').value;

    newPR.requestEmployeeId = this.auth.UserId;

    newPR.employeePhone = this.createPRForm.get('Phone').value;
    newPR.unit = this.currentEmployeeModel.organizationId;
    newPR.approverPostion = this.currentEmployeeModel.positionId;
    newPR.explain = this.createPRForm.get('Desc').value;

    newPR.statusId = this.emptyGuid;
    newPR.createdById = this.auth.UserId;
    newPR.createdDate = new Date();

    return newPR;
  }

  getListProcurementRequstItem(): Array<ProcurementRequestItemModel> {
    let result: Array<ProcurementRequestItemModel> = [];
    this.listProcurementRequestItem.forEach(item => {
      let newItem = new ProcurementRequestItemModel();
      newItem.procurementRequestItemId = this.emptyGuid;
      newItem.productId = item.productId;
      newItem.vendorId = item.vendorId;
      newItem.quantity = item.quantity;
      newItem.unitPrice = item.unitPrice;
      newItem.currencyUnit = item.currencyUnit;
      newItem.exchangeRate = item.exchangeRate;
      newItem.description = item.description;
      newItem.discountType = item.discountType;
      newItem.discountValue = item.discountValue;
      newItem.incurredUnit = item.incurredUnit;
      newItem.orderDetailType = item.orderDetailType;
      newItem.procurementPlanId = item.procurementPlanId;
      newItem.orderNumber = item.orderNumber;
      newItem.createdById = this.auth.UserId;
      newItem.createdDate = new Date();
      result.push(newItem);
    });
    return result;
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
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



