import { Component, OnInit, ViewEncapsulation, ElementRef, ViewChild, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';

import { LeadService } from '../../services/lead.service';
import { TranslateService } from '@ngx-translate/core';
import { GetPermission } from '../../../shared/permission/get-permission';

import { ContactModel } from '../../../shared/models/contact.model';

import { SendEmailDialogComponent } from '../../../shared/components/send-email-dialog/send-email-dialog.component';
import { SendSmsDialogComponent } from '../../../shared/components/send-sms-dialog/send-sms-dialog.component';

import * as $ from 'jquery';
import * as moment from 'moment';

import { MessageService, ConfirmationService, SortEvent, MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

import { QuickCreateLeadComponent } from '../quick-create-lead/quick-create-lead.component';

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
}

@Component({
  selector: 'app-lead-approval',
  templateUrl: './lead-approval.component.html',
  styleUrls: ['./lead-approval.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class LeadApprovalComponent implements OnInit {
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
  isShowListWaitingForApprove: boolean = true;
  @ViewChild('myTable') myTable: Table;
  actions: MenuItem[] = [];
  //master data
  listInterestedGroup: Array<interestedGroupModel> = [];
  listLeadType: Array<leadTypeModel> = [];
  listPersonalInchange: Array<personalInchangeModel> = [];
  listPotential: Array<potentialModel> = [];
  listStatus: Array<statusModel> = [];
  listLead: Array<leadSearchModel> = [];
  //list action in page
  actionAdd: boolean = true;
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  actionSMS: boolean = true;
  actionEmail: boolean = true;
  actionApprove: boolean = true;
  actionReject: boolean = true;

  constructor(private translate: TranslateService,
    private dialogService: DialogService,
    private getPermission: GetPermission,
    private leadService: LeadService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    //Check permission
    let resource = "crm/lead/approval";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }

    let listCurrentActionResource = permission.listCurrentActionResource;
    if (listCurrentActionResource.indexOf("add") == -1) {
      this.actionAdd = false;
    }
    if (listCurrentActionResource.indexOf("edit") == -1) {
      this.actionEdit = false;
    }
    if (listCurrentActionResource.indexOf("sms") == -1) {
      this.actionSMS = false;
    }
    if (listCurrentActionResource.indexOf("email") == -1) {
      this.actionEmail = false;
    }
    if (listCurrentActionResource.indexOf("delete") == -1) {
      this.actionDelete = false;
    }
    if (listCurrentActionResource.indexOf("approve") == -1) {
      this.actionApprove = false;
    }
    if (listCurrentActionResource.indexOf("reject") == -1) {
      this.actionReject = false;
    }

    this.initTable();
    this.initForm();
    this.getMasterData();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initTable() {
    this.colsListLead = [
      { field: 'fullName', header: 'Tên', textAlign: 'left', display: 'table-cell' },
      { field: 'email', header: 'Email', textAlign: 'left', display: 'table-cell' },
      { field: 'phone', header: 'Số điện thoại', textAlign: 'left', display: 'table-cell' },
      { field: 'personInChargeFullName', header: 'Người phụ trách', textAlign: 'left', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListLead;
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
      'WaitingForApproval': new FormControl(true),
      'StartDate': new FormControl(null),
      'EndDate': new FormControl(null),
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
    this.searchLeadForm.get('WaitingForApproval').setValue(true);
    this.searchLeadForm.get('StartDate').setValue(null);
    this.searchLeadForm.get('EndDate').setValue(null);
  }

  resetTable() {
    // this.listLead = [];
    // // this.myTable.sortOrder = 0;
    // // this.myTable.sortField = '';
    // this.myTable.reset();
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
    this.listLead.forEach(lead => {
      switch (lead.statusCode) {
        case "DSA"://Đang khảo sát
          lead.backgroundColorForStatus = "#ff9500";
          break;
        case "MOI"://Mới
          lead.backgroundColorForStatus = "#007aff";
          break;
        case "CHO"://Chờ phản hồi
          lead.backgroundColorForStatus = "#ffcc00";
          break;
        case "NDO"://Ngừng theo dõi
          lead.backgroundColorForStatus = "#4d5358";
          break;
        case "DGI"://Đang báo giá
          lead.backgroundColorForStatus = "#5856d6";
          break;
        case "KHD"://Ký hợp đồng
          lead.backgroundColorForStatus = "#34c759";
          break;
        default:
          break;
      }
    });
  }

  goToDetail(rowData: leadSearchModel) {
    this.router.navigate(['/lead/detail', { leadId: rowData.leadId }]);
  }

  goToCreateLead() {
      // this.router.navigate(['/lead/create-lead']);
      let ref = this.dialogService.open(QuickCreateLeadComponent, {
        header: 'Import Khách hàng tiềm năng',
        width: '85%',
        baseZIndex: 1050,
        contentStyle: {
          "min-height": "400px",
          "max-height": "800px",
        }
      });
  
      ref.onClose.subscribe((result: any) => {
        if (result) {
          if (result.status === true) {
                this.searchLead(true)
            }     
          }
      });
  }

  refreshFilter() {
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

  onChangeAction(rowData: leadSearchModel) {
    this.actions = [];
    let createOrder: MenuItem = {
      id: '2', label: 'Chuyển thành khách hàng', icon: 'pi pi-shopping-cart', command: () => {
        this.createCustomerToLead(rowData.leadId, rowData.phone, rowData.email);
      }
    }
    let sendSMS: MenuItem = {
      id: '3', label: 'Gửi SMS', icon: 'pi pi-envelope', command: () => {
        this.send_sms([rowData.leadId]);
      }
    }

    let sendEmail: MenuItem = {
      id: '4', label: 'Gửi Email', icon: 'pi pi-comment', command: () => {
        this.send_email([rowData.leadId]);
      }
    }

    let deleteCustomer: MenuItem = {
      id: '5', label: 'Xóa khách hàng', icon: 'pi pi-trash', command: () => {
        this.del_lead(rowData.leadId);
      }
    }

    if (this.actionAdd === true) {
      this.actions.push(createOrder);
    }

    if (this.actionSMS === true) {
      this.actions.push(sendSMS);
    }

    if (this.actionEmail === true) {
      this.actions.push(sendEmail);
    }

    if (this.actionDelete === true && rowData.canDeleteLead == true) {
      this.actions.push(deleteCustomer);
    }
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
            //let _title = 'Thông báo';
            let _content = 'Đã tồn tại khách hàng trong hệ thống. Bạn có muốn cập nhật thông tin khách hàng có số điện thoại [' + phone + '] hoặc email [' + email + '] này không?';
            this.confirmationService.confirm({
              message: _content,
              accept: () => {
                //this.loading = true;
                this.leadService.checkDuplicateCustomer(email, phone, leadId, true, this.auth.UserId).subscribe(response => {
                  this.loading = false;
                  let result = <any>response;
                  if (result.statusCode === 202 || result.statusCode === 200) {
                    this.leadService.checkDuplicateCustomer(email, phone, leadId, true, this.auth.UserId).subscribe(response => { });
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
            this.showToast('success', 'Thông báo', 'Xóa khách hàng thành công');
          } else {
            this.showToast('error', 'Thông báo', 'Xóa khách hàng thất bại');
          }
        }, error => { this.loading = false; });
      }
    });
  }

  approveOrReject(isApprove: boolean) {
    if (this.listSelectedLead.length === 0) {
      this.clearToast();
      this.showToast('warn', 'Thông báo', 'Chọn khách hàng cần phê duyệt');
      return;
    }
    let leadListId: Array<string> = this.listSelectedLead.map(e => e.leadId);
    switch (isApprove) {
      case true://Phê duyệt
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn phê duyệt danh sách này?',
          accept: () => {
            this.loading = true;
            this.leadService.approveOrRejectLeadUnfollow(leadListId, isApprove, this.auth.UserId).subscribe(response => {
              this.loading = false;
              let result = <any>response;
              if (result.statusCode === 202 || result.statusCode === 200) {
                this.listSelectedLead = [];
                this.listLead = this.listLead.filter(e => !leadListId.includes(e.leadId));
                this.showToast('success', 'Thông báo', 'Phê duyệt thành công');
              } else {
                this.showToast('error', 'Thông báo', result.messageCode);
              }
            }, error => { this.loading = false; });
          }
        });
        break;
      case false://Từ chối
        this.confirmationService.confirm({
          message: 'Bạn có chắc chắn muốn từ chối danh sách này?',
          accept: () => {
            this.loading = true;
            this.leadService.approveOrRejectLeadUnfollow(leadListId, isApprove, this.auth.UserId).subscribe(response => {
              this.loading = false;
              let result = <any>response;
              if (result.statusCode === 202 || result.statusCode === 200) {
                this.listSelectedLead = [];
                this.listLead = this.listLead.filter(e => !leadListId.includes(e.leadId));
                this.showToast('success', 'Thông báo', 'Từ chối thành công');
              } else {
                this.showToast('error', 'Thông báo', result.messageCode);
              }
            }, error => { this.loading = false; });
          }
        });
        break;
      default:
        break;
    }
  }

  async searchLead(quickCreatedLead: boolean) {
    let _name = this.searchLeadForm.get('FullName').value;
    let _phone = this.searchLeadForm.get('Phone').value;
    let _email = this.searchLeadForm.get('Email').value;
    let _leadType = this.searchLeadForm.get('LeadType').value.map(e => e.categoryId);
    let _potential = this.searchLeadForm.get('Potential').value.map(e => e.categoryId);
    let _status = this.searchLeadForm.get('Status').value.map(e => e.categoryId);
    let _interestedGroup = this.searchLeadForm.get('InterestedGroup').value.map(e => e.categoryId);
    let picvalue = this.searchLeadForm.get('Pic').value;
    let _pic = picvalue == null ? [] : [picvalue.employeeId];
    let _isHasPic = this.searchLeadForm.get('IsHasPic').value;
    let _waitingForApproval = this.searchLeadForm.get('WaitingForApproval').value;
    let _startDate = this.searchLeadForm.get('StartDate').value;
    let _endDate = this.searchLeadForm.get('EndDate').value;
    let contactModel: ContactModel = new ContactModel();
    contactModel.FirstName = _name;
    contactModel.LastName = '';
    contactModel.Phone = _phone;
    contactModel.Email = _email;
    this.loading = true;
    let result: any = await this.leadService.searchLeadAsync(contactModel, _potential, _status, _interestedGroup, _pic, _leadType, _isHasPic, _startDate, _endDate, _waitingForApproval, this.auth.UserId, [], [], []);
    this.loading = false;
    if (result.statusCode === 200) {
      //nếu tìm tất cả lead chứ không phải tìm lead cần phê duyệt thì ẩn button phê duyệt/từ chối
      if (_waitingForApproval == false) {
        this.isShowListWaitingForApprove = false;
      } else {
        this.isShowListWaitingForApprove = true;
      }
      this.resetTable();
      this.listLead = result.listLead;
      this.handleBackGroundColorForStatus();
      if (this.listLead.length === 0) {
        this.clearToast();
        this.showToast('warn', 'Thông báo', 'Không có dữ liệu');
      }else {
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

  async getMasterData() {
    this.loading = true;
    let contactModel: ContactModel = new ContactModel();
    contactModel.FirstName = '';
    contactModel.LastName = '';
    contactModel.Phone = '';
    contactModel.Email = '';
    let [dataResponse, searchResponse]: any = await Promise.all([
      this.leadService.getDataSearchLead(this.auth.UserId),
      this.leadService.searchLeadAsync(contactModel, [], [], [], [], [], false, null, null, true, this.auth.UserId, [], [], [])
    ]);
    this.loading = false;
    if (dataResponse.statusCode === 200 && searchResponse.statusCode) {
      this.listInterestedGroup = dataResponse.listInterestedGroup;
      this.listLeadType = dataResponse.listLeadType;
      this.listPersonalInchange = dataResponse.listPersonalInchange;
      this.listPotential = dataResponse.listPotential;
      this.listStatus = dataResponse.listStatus;
      this.listLead = searchResponse.listLead;
      this.isShowFilterLeft = false;
      this.leftColNumber = 12;
      this.rightColNumber = 0;
      this.handleBackGroundColorForStatus();
      if (this.listLead.length === 0) {
        this.clearToast();
        this.showToast('warn', 'Thông báo', 'Không có dữ liệu');
      }
    } else if (dataResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', dataResponse.messageCode);
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', searchResponse.messageCode);
    }
  }
}
