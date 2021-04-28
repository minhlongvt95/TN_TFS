import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { LeadService } from '../../services/lead.service';
import { NoteService } from '../../../shared/services/note.service';
import { TranslateService } from '@ngx-translate/core';
import { ContactModel } from '../../../shared/models/contact.model';
import { SendEmailDialogComponent } from '../../../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../../../shared/components/send-sms-dialog/send-sms-dialog.component';
import { LeadImportComponent } from '../../components/lead-import/lead-import.component';
import { GetPermission } from '../../../shared/permission/get-permission';
import * as moment from 'moment';
import { NoteModel } from '../../../shared/models/note.model';
import { SetPicDialogComponent } from '../set-pic-dialog/set-pic-dialog.component';
import { MessageService, ConfirmationService, SortEvent, MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

class colorConfig {
  color: string;
}

const COLOR_CONFIG: Array<colorConfig> = [
  { color: "#ffcc00" },
  { color: "#002d9c" },
  { color: "#4d5358" },
  { color: "#f2f4f8" },
  { color: "#c1c7cd" },
  { color: "#78a9ff" },
  { color: "#ff3b30" },
  { color: "#34c759" },
  { color: "#5ac8fa" },
  { color: "#5856d6" },
  { color: "#af52de" },
  { color: "#ff2d55" },
]
class category{
  categoryId: string;
  categoryName: string;
  categoryCode: string
}

class interestedGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class leadTypeModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class potentialModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class statusModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}

class personalInchangeModel {
  employeeId: string;
  employeeCode: string;
  employeeName: string;
}

class leadSearchModel {
  leadId: string;
  contactId: string;
  email: string;
  phone: string;
  fullName: string;
  personInChargeId: string;
  personInChargeFullName: string;
  statusCode: string;
  statusName: string;
  waitingForApproval: boolean;
  backgroundColorForStatus: string;
  canDeleteLead: boolean;
  customerId: string;
  businessTypeId: string;
  investmentFundId: string;
  probabilityId: string;
  expectedSale: number;
  customerName: string;
  businessTypeName: string;
  investmentFundName: string;
  probabilityName: string;
  isStatusConnect: number;
  statusSuportId: string;
  statusSupportName: string;
}

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
  // encapsulation: ViewEncapsulation.None,

})
export class LeadListComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  innerWidth: number = 0; //number window size first
  minYear: number = 2010;
  currentYear: number = (new Date()).getFullYear();
  //display on html
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  //form
  searchLeadForm: FormGroup;
  //table
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  colsListLead: any[];
  selectedColumns: any[];
  rows: number = 10;
  listSelectedLead: Array<leadSearchModel> = [];
  @ViewChild('myTable') myTable: Table;
  actions: MenuItem[] = [];
  //master data
  listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<leadTypeModel> = [];
  listPersonalInchange: Array<personalInchangeModel> = [];
  listPotential: Array<potentialModel> = [];
  listStatus: Array<statusModel> = [];
  listLead: Array<leadSearchModel> = [];
  listCusGroup: Array<category> = [];
  listSource: Array<category> = [];
  listArea: Array<any> = [];
  //pop-up variable

  //list action in page
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDownload: boolean = true;
  actionSMS: boolean = true;
  actionEmail: boolean = true;
  actionImport: boolean = true;
  actionDelete: boolean = true;
  isGlobalFilter: string = '';
  /*Check user permission*/

  //Khai bao cac permission
  userPermission: any = localStorage.getItem('UserPermission').split(',');
  createPermission: string = 'lead/create';
  assignPermisison: string = 'lead/assign';
  unFollowPermisison: string = 'lead/unfollow';
  exportExcelPermission: string = 'lead/exportexcel';
  importExcelPermission: string = 'lead/importexcel';
  viewLeadList: string = 'lead/list';
  importExcel: string = 'lead/importexcel';

  constructor(
    private translate: TranslateService,
    private dialogService: DialogService,
    private getPermission: GetPermission,
    private leadService: LeadService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private noteService: NoteService
  ) {
    this.translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  async ngOnInit() {
    //Check permission
    let resource = "crm/lead/list";
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
      if (listCurrentActionResource.indexOf("download") == -1) {
        this.actionDownload = false;
      }
      if (listCurrentActionResource.indexOf("sms") == -1) {
        this.actionSMS = false;
      }
      if (listCurrentActionResource.indexOf("email") == -1) {
        this.actionEmail = false;
      }
      if (listCurrentActionResource.indexOf("import") == -1) {
        this.actionImport = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }

      this.initTable();
      this.initForm();

      let param = this.router.parseUrl(this.router.url);

      switch (param.queryParams.status) {
        case undefined:
          this.getMasterData(null);
          break;
        default:
          this.getMasterData(param.queryParams.status);
          break;
      }
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initTable() {
    this.colsListLead = [
      { field: 'fullName', header: 'Tên cơ hội', textAlign: 'left', display: 'table-cell', width: "170px" },
      { field: 'probabilityName', header: 'Xác suất', textAlign: 'center', display: 'table-cell', width: '100px' },
      { field: 'expectedSale', header: 'Doanh thu mong đợi', textAlign: 'right', display: 'table-cell', width: '170px' },
      { field: 'customerName', header: 'Khách Hàng', textAlign: 'left', display: 'table-cell', width: '150px' },
      { field: 'email', header: 'Email', textAlign: 'left', display: 'table-cell', width: '140px' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'right', display: 'table-cell', width: '120px' },
      { field: 'personInChargeFullName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell', width: '200px' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell', width: '140px' },
      { field: 'statusSupportName', header: 'Trạng thái phụ', textAlign: 'center', display: 'table-cell', width: '140px' },
    ];
    this.selectedColumns = this.colsListLead.filter(e => 
      e.field == "fullName" || e.field == "customerName" || e.field == "email" || e.field == "phone"
      || e.field == "statusName" || e.field == "statusSupportName");
  }

  initForm() {
    this.searchLeadForm = new FormGroup({
      'FullName': new FormControl(''),
      'Phone': new FormControl(''),
      'Email': new FormControl(''),
      'LeadType': new FormControl([]),
      'Potential': new FormControl([]),
      'Status': new FormControl([]),
      'InterestedGroup': new FormControl([]),
      'Pic': new FormControl(null),
      'IsHasPic': new FormControl(false),
      'StartDate': new FormControl(null),
      'EndDate': new FormControl(null),
      "LeadArea": new FormControl([]),
      "LeadSource": new FormControl([]),
      "LeadGroup": new FormControl([])
    });
  }

  resetForm() {
    this.searchLeadForm.reset();
    this.searchLeadForm.get('FullName').setValue('');
    this.searchLeadForm.get('Phone').setValue('');
    this.searchLeadForm.get('Email').setValue('');
    this.searchLeadForm.get('LeadType').setValue([]);
    this.searchLeadForm.get('Potential').setValue([]);
    this.searchLeadForm.get('Status').setValue([]);
    this.searchLeadForm.get('InterestedGroup').setValue([]);
    this.searchLeadForm.get('Pic').setValue(null);
    this.searchLeadForm.get('IsHasPic').setValue(false);
    this.searchLeadForm.get('StartDate').setValue(null);
    this.searchLeadForm.get('EndDate').setValue(null);
    this.searchLeadForm.get('LeadArea').setValue([]);
    this.searchLeadForm.get('LeadSource').setValue([]);
    this.searchLeadForm.get('LeadGroup').setValue([]);
  }

  resetTable() {
    if (this.myTable) {
      this.myTable.reset();
    }
  }

  async searchLead(quickCreatedLead: boolean) {
    let _name = this.searchLeadForm.get('FullName').value != null ? this.searchLeadForm.get('FullName').value.trim() : this.searchLeadForm.get('FullName').value;
    let _phone = this.searchLeadForm.get('Phone').value != null ? this.searchLeadForm.get('Phone').value.trim() : this.searchLeadForm.get('Phone').value;
    let _email = this.searchLeadForm.get('Email').value != null ? this.searchLeadForm.get('Email').value.trim() : this.searchLeadForm.get('Email').value;
    let _leadType = this.searchLeadForm.get('LeadType').value.map(e => e.categoryId);
    let _potential = this.searchLeadForm.get('Potential').value.map(e => e.categoryId);
    let _status = this.searchLeadForm.get('Status').value.map(e => e.categoryId);
    let _interestedGroup = this.searchLeadForm.get('InterestedGroup').value.map(e => e.categoryId);
    let picvalue = this.searchLeadForm.get('Pic').value;
    let _pic = picvalue == null ? [] : [picvalue.employeeId];
    let _isHasPic = this.searchLeadForm.get('IsHasPic').value;
    let _startDate = this.searchLeadForm.get('StartDate').value;
    let _endDate = this.searchLeadForm.get('EndDate').value;
    let _source = this.searchLeadForm.get('LeadSource').value ? this.searchLeadForm.get('LeadSource').value.map(c => c.categoryId) : [];
    let _area = this.searchLeadForm.get('LeadArea').value ? this.searchLeadForm.get('LeadArea').value.map(c => c.geographicalAreaId) : [];
    let _group = this.searchLeadForm.get('LeadGroup').value ? this.searchLeadForm.get('LeadGroup').value.map(c => c.categoryId) : [];

    let contactModel: ContactModel = new ContactModel();

    if (_startDate) {
      _startDate.setHours(0, 0, 0, 0);
      _startDate = convertToUTCTime(_startDate);
    }
    if (_endDate) {
      _endDate.setHours(23, 59, 59, 999);
      _endDate = convertToUTCTime(_endDate);
    }
    contactModel.FirstName = _name;
    contactModel.LastName = '';
    contactModel.Phone = _phone;
    contactModel.Email = _email;
    this.loading = true;
    let result: any = await this.leadService.searchLeadAsync(contactModel, _potential, _status, _interestedGroup, _pic, _leadType, _isHasPic, _startDate, _endDate, false, this.auth.UserId, _source, _area, _group);
    this.loading = false;
    if (result.statusCode === 200) {
      this.resetTable();
      this.listLead = result.listLead;
      this.handleBackGroundColorForStatus();
      if (this.listLead.length === 0) {
        this.clearToast();
        this.showToast('warn', 'Thông báo', 'Không có dữ liệu');
      } else {
        //nếu tìm sau khi tạo nhanh thì show message tạo thành công
        if (quickCreatedLead) {
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Tạo khách hàng tiềm năng thành công');
        }
      }
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  async getMasterData(statusCode: string) {
    this.loading = true;
    let contactModel: ContactModel = new ContactModel();
    contactModel.FirstName = '';
    contactModel.LastName = '';
    contactModel.Phone = '';
    contactModel.Email = '';

    let dataResponse: any = await this.leadService.getDataSearchLead(this.auth.UserId);
    if (dataResponse.statusCode === 200) {
      this.listInterestedGroup = dataResponse.listInterestedGroup;
      this.listLeadType = dataResponse.listLeadType;
      this.listPersonalInchange = dataResponse.listPersonalInchange;
      this.listArea = dataResponse.listArea;
      this.listCusGroup = dataResponse.listCusGroup;
      this.listSource = dataResponse.listSource;
      this.listPersonalInchange.forEach(item => {
        item.employeeName = item.employeeCode + ' - ' + item.employeeName;
      });
      this.listPotential = dataResponse.listPotential;
      this.listStatus = dataResponse.listStatus;
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', dataResponse.messageCode);
      return;
    }
    //tìm theo điều kiện
    let listStatusId = []; //danh sách status tìm kiếm
    if (statusCode !== null) {
      let _statusId = this.listStatus.find(e => e.categoryCode == statusCode).categoryId;
      listStatusId.push(_statusId);
      //thay đổi form search
      let newStatus = this.listStatus.filter(e => e.categoryCode == statusCode);
      this.searchLeadForm.get('Status').setValue(newStatus);
    }

    let searchResponse: any = await this.leadService.searchLeadAsync(contactModel, [], listStatusId, [], [], [], false, null, null, false, this.auth.UserId, [], [], []);
    this.loading = false;
    if (searchResponse.statusCode === 200) {

      this.listLead = searchResponse.listLead;
      this.isShowFilterLeft = false;
      this.leftColNumber = 12;
      this.rightColNumber = 0;
      this.handleBackGroundColorForStatus();
      if (this.listLead.length === 0) {
        this.clearToast();
        this.showToast('warn', 'Thông báo', 'Không có dữ liệu');
      }
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', searchResponse.messageCode);
    }
  }

  dateFieldFormat: string = 'DD/MM/YYYY';
  customSort(event: SortEvent) {
    event.data.sort((data1, data2) => {
      let value1 = data1[event.field];
      let value2 = data2[event.field];

      /**Customize sort date */
      if (event.field == 'quoteDate') {
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

  handleBackGroundColorForStatus() {
    this.listLead.forEach((lead, index) => {
      switch (lead.statusCode) {
        case "APPR"://Xac nhan
          lead.backgroundColorForStatus = "#34c759";
          break;
        case "CLOSE"://dong
          lead.backgroundColorForStatus = "#5ac8fa";
          break;
        case "DRAFT"://nhap
          lead.backgroundColorForStatus = "#78a9ff";
          break;
        case "CANC"://huy
          lead.backgroundColorForStatus = "#ff2d55";
          break;
        default:
          break;
      }
    });
  }

  setPersonalInChange(lead: leadSearchModel) {
    let listLeadId = [];
    listLeadId.push(lead.leadId)
    let ref = this.dialogService.open(SetPicDialogComponent, {
      header: 'Gán người phụ trách',
      width: '30%',
      data: {
        listLeadId: listLeadId,
        customerId: lead.customerId
      },
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "270px",
        "max-height": "270px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          switch (result.status) {
            case true:
              this.clearToast();
              this.showToast('success', 'Thông báo', 'Gán người phụ trách thành công');
              this.searchLead(false);
              break;
            case false:
              this.clearToast();
              this.showToast('error', 'Thông báo', 'Gán người phụ trách thất bại');
              break;
            default:
              break;
          }
        }
      }
    });
  }

  unfollowLead() {
    if (this.listSelectedLead.length == 0) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', 'Chọn khách hàng tiềm năng cần ngừng theo dõi');
      return;
    }
    let listLeadId = this.listSelectedLead.map(e => e.leadId);
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn gửi phê duyệt? Việc Ngừng theo dõi cần sự phê duyệt của các cấp khác',
      accept: async () => {
        this.loading = true;
        let result: any = await this.leadService.unfollowListLead(listLeadId, this.auth.UserId);
        if (result.statusCode === 200) {
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Ngừng theo dõi thành công');
          this.listSelectedLead = [];
          this.searchLead(false);
        } else {
          this.clearToast();
          this.showToast('error', 'Thông báo', result.messageCode);
        }
      },
      reject: () => {

      }
    });
  }

  goToDetail(rowData: leadSearchModel) {
    this.router.navigate(['/lead/detail', { leadId: rowData.leadId }]);
  }

  goToCreateLead() {
    this.router.navigate(['/lead/create-lead']);
  }

  refreshFilter() {
    this.leftColNumber = 12;
    this.rightColNumber = 0;
    this.listLead = [];
    this.isGlobalFilter = '';
    this.isShowFilterLeft = false;
    this.resetTable();
    this.resetForm();
    this.searchLead(false);
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  async cloneLead(leadId: string, contactId: string) {
    this.loading = true;
    let result: any = await this.leadService.cloneLeadAsync(leadId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode == 200) {
      let note: NoteModel = new NoteModel();
      note.Type = 'NEW';
      note.Description = 'Mức độ tiềm năng - <b>' + result.potential + '</b>, trạng thái - <b>' + result.statusName + '</b>, chưa có người phụ trách';
      this.noteService.createNote(note, result.leadId, null, this.auth.UserId).subscribe(response => {
        var _noteresult = <any>response;
        if (_noteresult.statusCode !== 200) {
          this.clearToast();
          this.showToast('error', 'Thông báo', _noteresult.messageCode);
        }
        this.router.navigate(['/lead/detail', { 'leadId': result.leadId }]);
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Nhân bản thành công' };
        this.showMessage(msg);
      });
    } else {
      let msg = { severity: 'success', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clearMessage() {
    this.messageService.clear();
  }

  onChangeAction(rowData: leadSearchModel) {
    let statusCode = rowData.statusCode;
    this.actions = [];
    let createQuote: MenuItem = {
      id: '1', label: 'Tạo báo giá', icon: 'pi pi-clone', command: () => {
        this.goToCustomerQuote(rowData.leadId, rowData.contactId);
      }
    }
    let createSaleBidding: MenuItem = {
      id: '2', label: 'Tạo hồ sơ thầu', icon: 'pi pi-clone', command: () => {
        this.createSaleBidding(rowData.leadId);
      }
    }
    let replyLead: MenuItem = {
      id: '3', label: 'Nhân bản', icon: 'pi pi-shopping-cart', command: () => {
        this.cloneLead(rowData.leadId, rowData.contactId);
      }
    }
    let sendSMS: MenuItem = {
      id: '4', label: 'Gửi SMS', icon: 'pi pi-envelope', command: () => {
        this.send_sms([rowData.leadId]);
      }
    }
    let sendEmail: MenuItem = {
      id: '5', label: 'Gửi Email', icon: 'pi pi-comment', command: () => {
        this.send_email([rowData.leadId]);
      }
    }
    let deleteCustomer: MenuItem = {
      id: '6', label: 'Xóa cơ hội', icon: 'pi pi-trash', command: () => {
        this.del_lead(rowData.leadId);
      }
    }
    let assignedPersonInCharge: MenuItem = {
      id: '7', label: 'Gán người phụ trách', icon: 'pi pi-pencil', command: () => {
        this.setPersonalInChange(rowData);
      }
    }
    this.actions.push(assignedPersonInCharge);
    switch (statusCode) {
      case 'DRAFT':
        if (this.actionDelete === true && rowData.canDeleteLead == true) {
          this.actions.push(deleteCustomer);
        }
        this.actions.push(replyLead);
        if (this.actionSMS === true) {
          this.actions.push(sendSMS);
        }
        if (this.actionEmail === true) {
          this.actions.push(sendEmail);
        }
        break;
      case 'APPR':
        if (rowData.isStatusConnect == 0) {
          if (rowData.personInChargeId != null) {
            this.actions.push(createSaleBidding);
          }
          if (this.actionAdd === true) {
            this.actions.push(createQuote);
          }
          this.actions.push(replyLead);
          if (this.actionSMS === true) {
            this.actions.push(sendSMS);
          }
          if (this.actionEmail === true) {
            this.actions.push(sendEmail);
          }
        } else if (rowData.isStatusConnect == 1) {
          this.actions.push(replyLead);
        } else if (rowData.isStatusConnect == 2) {
          this.actions.push(replyLead);
          if (this.actionAdd === true) {
            this.actions.push(createQuote);
          }
        }
        break;
      case 'CANC':
        this.actions.push(replyLead);
        break;
      default:
        this.actions.push(replyLead);
        break;
    }
  }

  download() {
    this.leadService.downloadTemplateLead(this.auth.UserId).subscribe(response => {
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.excelFile);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.nameFile + ".xlsm";
        link.download = fileName;
        link.click();
      }
    }, error => { });
  }

  importLead() {
    let ref = this.dialogService.open(LeadImportComponent, {
      header: 'Import khách hàng tiềm năng',
      width: '50%',
      baseZIndex: 1030,
      contentStyle: {
        "max-height": "800px",
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status === true) {
          this.searchLead(false);
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Import thành công');
        }
      }
    });
  }

  //redirect đến trang tạo báo giá
  goToCustomerQuote(leadId: any, contactId: any) {
    this.router.navigate(['/customer/quote-create', {
      occasionId: leadId,
      contactId: contactId
    }]);
  }

  createSaleBidding(leadId: any) {
    this.router.navigate(['/sale-bidding/create', { leadId: leadId }]);
  }

  createCustomerToLead(leadId: any, phone: any, email: any) {
    if (leadId != null) {
      if (phone != null || email != null) {
        this.loading = true;
        this.leadService.checkDuplicateCustomer(email, phone, leadId, false, this.auth.UserId).subscribe(response => {
          this.loading = false;
          const result = <any>response;
          var customerId = result.customerId;
          var contactId = result.contactId;
          if (result.isDuplicate) {
            //Khach hang da ton tai
            let _content = 'Đã tồn tại khách hàng trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng có số điện thoại [' + phone + '] hoặc email [' + email + '] này không?';
            this.confirmationService.confirm({
              message: _content,
              accept: () => {
                this.leadService.checkDuplicateCustomer(email, phone, leadId, true, this.auth.UserId).subscribe(response => {
                  this.loading = false;
                  let result = <any>response;
                  if (result.statusCode === 202 || result.statusCode === 200) {
                    this.router.navigate(['/customer/detail', { customerId: customerId, contactId: contactId }]);
                  } else {
                  }
                }, error => { this.loading = false; });
              }
            });
          } else {
            //Insert Lead to Customer
            this.router.navigate(['/customer/detail', { customerId: customerId, contactId: contactId }]);
          }
        });
      }
    }
  }

  //send sms
  // send_sms(leadId: any) {
  //   if (leadId != null) {
  //     this.selectedLeadIdList.push(leadId);
  //   }
  //   if (this.selectedLeadIdList.length === 0) {
  //     this.translate.get('lead.select_no_item').subscribe(value => { this.messages = value; });
  //     this.snackBar.openFromComponent(WarningComponent, { data: this.messages, ...this.warningConfig });
  //   } else {
  //     this.dialogSendSMS = this.dialog.open(SendSmsDialogComponent,
  //       {
  //         width: '1200px',
  //         height: '380px',
  //         autoFocus: false,
  //         data: { leadIdList: this.selectedLeadIdList }
  //       });

  //     this.dialogSendSMS.afterClosed().subscribe(result => {
  //       if (result) {
  //         if (result.message == "success") {
  //           this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
  //           this.ngOnInit();
  //         } else {
  //           this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
  //         }
  //         this.selectedLeadIdList = [];
  //       }
  //       if (leadId != null) {
  //         this.selectedLeadIdList.splice(this.selectedLeadIdList.indexOf(leadId), 1);
  //       }
  //     }, error => { });
  //   }
  // }

  //send email
  send_email(listLeadId: Array<string>) {
    let ref = this.dialogService.open(SendEmailDialogComponent, {
      data: {
        leadIdList: listLeadId
      },
      header: 'Gửi email',
      width: '70%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "280px",
        "max-height": "800px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          switch (result.message) {
            case "success":
              this.showToast('success', 'Thông báo', 'Gửi email thành công');
              break;
            case "warning":
              //  let msg2 = { severity:'warn', summary: 'Thông báo:', detail: result.messageCode};
              this.showToast('warn', 'Thông báo', result.messageCode);
              break;
            default:
              this.showToast('error', 'Thông báo', result.messageCode);
              break;
          }
        }
      }
    });

  }

  send_sms(listLeadId: Array<string>) {
    let ref = this.dialogService.open(SendSmsDialogComponent, {
      data: {
        leadIdList: listLeadId
      },
      header: 'Gửi SMS',
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
        if (result.status) {
          if (result.message == "success") {
            this.showToast('success', 'Thông báo', 'Gửi SMS thành công');
          } else {
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        }
      }
    });
  }

  del_lead(leadId: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.leadService.changeLeadStatusToDelete(leadId).subscribe(response => {
          this.loading = false;
          let result = <any>response;
          if (result.statusCode === 202 || result.statusCode === 200) {
            //remove from list
            this.listLead = this.listLead.filter(e => e.leadId != leadId);
            this.showToast('success', 'Thông báo', result.messageCode);
          } else {
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        }, error => { this.loading = false; });
      }
    });
  }

}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

