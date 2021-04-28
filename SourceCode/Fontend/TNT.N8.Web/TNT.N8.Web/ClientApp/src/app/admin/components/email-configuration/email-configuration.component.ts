import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import * as $ from 'jquery';

import { EmailConfigService } from '../../services/email-config.service';

/* primeng api */
import { MessageService, ConfirmationService } from 'primeng/api';
/* end primeng api */

import { categoryModel } from '../../models/configEmail.model';
import { EmployeeService } from '../../../employee/services/employee.service';
import { GetPermission } from '../../../shared/permission/get-permission';

class emailTemplateModel {
   emailTemplateId: string;
   emailTemplateName: string;
   emailTemplateTitle: string;
   emailTemplateContent: string;
   emailTemplateTypeId: string;
   emailTemplateStatusId: string;
   active: boolean;
   createdById: string;
   createdDate: Date;
   updatedById: string;
   updatedDate: Date;
   //adding property to show in table
   emailTemplateTypeName: string;
   emailTemplateStatusName: string;
}

@Component({
  selector: 'app-email-configuration',
  templateUrl: './email-configuration.component.html',
  styleUrls: ['./email-configuration.component.css']
})
export class EmailConfigurationComponent implements OnInit {
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  //master data
  listEmailType: Array<categoryModel> = [];
  listEmailStatus:  Array<categoryModel> = [];
  listEmailTemplate: Array<emailTemplateModel> = [];
  //table
  first: number = 0;
  rows: number = 10;
  selectedColumns: any[];
  colsListProduct: any[];
  //form
  searchForm: FormGroup;
  constructor(
    public messageService: MessageService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private emailConfigService: EmailConfigService,
    private employeeService: EmployeeService,
    private getPermission: GetPermission,
  ) { }

  ngOnInit() {
    this.initForm();
    this.initTable();

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getMasterdata();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/email-configuration/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.router.navigate(['/home']);
        }
        else {
          this.getMasterdata();
        }
      }
    });
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.searchForm = new FormGroup({
      'Name': new FormControl(''),
      'Title': new FormControl(''),
      'EmailType': new FormControl([]),
      'EmailStatus': new FormControl([])
    });
  }

  initTable() {
    this.colsListProduct = [
      { field: 'emailTemplateName', header: 'Tên mẫu email', textAlign: 'left', display: 'table-cell' },
      { field: 'emailTemplateTitle', header: 'Tiêu đề email', textAlign: 'left', display: 'table-cell' },
      { field: 'emailTemplateTypeName', header: 'Trạng thái cần gửi', textAlign: 'left', display: 'table-cell' },
      { field: 'emailTemplateStatusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListProduct;
  }

  async getMasterdata() {
    this.loading= true;
    let [masterdataResponse, initSearchResponse]: any = await Promise.all([
      this.emailConfigService.searchEmailConfigMasterdata(),
      this.emailConfigService.searchEmailTemplate('', '', [], [])
    ]);
    this.loading = false;

    if (masterdataResponse.statusCode === 200 && initSearchResponse.statusCode === 200) {
      //master data
      this.listEmailType = masterdataResponse.listEmailType;
      this.listEmailStatus = masterdataResponse.listEmailStatus;
      //search response
      this.listEmailTemplate = initSearchResponse.listEmailTemplateModel;
      if ( this.listEmailTemplate.length === 0) {
        this.clearToast();
        this.showToast('warn', 'Thông báo', 'Không có dữ liệu');
      } else {
        //set name email type and email status
       this.setNameEmailTypeAndStatus();
      }
  
    } else if (masterdataResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', masterdataResponse.messageCode);
    } else if (initSearchResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', initSearchResponse.messageCode);
    }
  }

  setNameEmailTypeAndStatus() {
    this.listEmailTemplate.forEach(email => {
      email.emailTemplateTypeName = this.listEmailType.find(e => e.categoryId == email.emailTemplateTypeId).categoryName;
      email.emailTemplateStatusName = this.listEmailStatus.find(e => e.categoryId == email.emailTemplateStatusId).categoryName;
    });
  }

  async searchEmailTemplate() {
      let name = this.searchForm.get('Name').value;
      let title = this.searchForm.get('Title').value;
      let listEmailTypeId= this.searchForm.get('EmailType').value.map(e => e.categoryId);
      let listEmailStatusid = this.searchForm.get('EmailStatus').value.map(e => e.categoryId);

      this.loading = true;
      let result: any = await  this.emailConfigService.searchEmailTemplate(name, title , listEmailTypeId , listEmailStatusid);
      this.loading = false;

      if (result.statusCode === 200) {
        //TODO: reset table
        this.listEmailTemplate = result.listEmailTemplateModel;
        if ( this.listEmailTemplate.length === 0) {
          this.clearToast();
          this.showToast('warn', 'Thông báo', 'Không có dữ liệu');
        } else {
             //set name email type and email status
       this.setNameEmailTypeAndStatus();
        }
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
  }

  addNewTemplate() {
    this.router.navigate(['/admin/email-create-template']);
  }

  goToDetail(rowData: emailTemplateModel) {
    this.router.navigate(['/admin/email-create-template',{ emailTemplateId: rowData.emailTemplateId}]);
  }

}
