import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { ImageSettingsModel } from '@syncfusion/ej2-angular-richtexteditor';

import * as $ from 'jquery';

/* primeng api */
import { MessageService, ConfirmationService } from 'primeng/api';
/* end primeng api */
import { ToolbarService, LinkService, ImageService, HtmlEditorService, TableService, RichTextEditorComponent } from '@syncfusion/ej2-angular-richtexteditor';

import { categoryModel } from '../../models/configEmail.model';
import { EmailTemplateModel } from '../../models/createEmailTemplate.model'

import { EmailConfigService } from '../../services/email-config.service';

import { ImageUploadService } from '../../../shared/services/imageupload.service';

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

class emailTemplateTokenModel {
  tokenCode: string;
  tokenLabel: string;
}

@Component({
  selector: 'app-email-create-template',
  templateUrl: './email-create-template.component.html',
  styleUrls: ['./email-create-template.component.css'],
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService, TableService]
})
export class EmailCreateTemplateComponent implements OnInit {
  @ViewChild('templateRTE', { static: true }) rteEle: RichTextEditorComponent;
  loading: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  display;
  //master data
  listEmailType: Array<categoryModel> = [];
  listEmailStatus: Array<categoryModel> = [];
  listEmailTemplateToken:  Array<emailTemplateTokenModel> = [];
  //routing 
  isEdit: boolean = false;
  paramEmailTemplateId: string;
  //table 
  selectedColumns: any[];
  first: number = 0;
  rows: number = 10;
  //attach files
  uploadedFiles: any[] = [];
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';

  contenModel: string = "";

  validatorEditor: boolean = true;

  //rteValue: string = this.rteObj.contentModule.getText();
  //form controls
  createEmailForm: FormGroup;

  constructor(
    private imageService: ImageUploadService,
    public messageService: MessageService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private emailConfigService: EmailConfigService,
    private route: ActivatedRoute,
  ) { }

  public tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', 'Image', '|', 'ClearFormat', 'Print',
      'SourceCode', 'FullScreen', '|', 'Undo', 'Redo', 'CreateTable']
  };
  public insertImageSettings: ImageSettingsModel = { allowedTypes: ['.jpeg', '.jpg', '.png'], display: 'inline', width: 'auto', height: 'auto', saveFormat: 'Base64', saveUrl: null, path: null, }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  ngOnInit() {
    this.initForm();
    this.initTable();
    this.route.params.subscribe(params => {
      this.paramEmailTemplateId = params['emailTemplateId'];
      this.getMasterdata(this.paramEmailTemplateId);
    });
  }

  initTable() {
    this.selectedColumns = [
      { field: 'tokenLabel', header: 'Mã Token', textAlign: 'left', display: 'table-cell' },
      { field: 'tokenCode', header: 'Tên Token', textAlign: 'left', display: 'table-cell' }     
    ];
  }

  initForm() {
    this.createEmailForm = new FormGroup({
      'EmailName': new FormControl('', Validators.required),
      'EmailTitle': new FormControl('', Validators.required),
      'EmailType': new FormControl(null, Validators.required),
      'CCEmail': new FormControl([]),
      'Active': new FormControl(true)
    });
    // tach rieng email content thanh ngModel

  }

  async getMasterdata(paramEmailTemplateId: string) {
    if (paramEmailTemplateId === undefined) {
      this.isEdit = false;
    } else {
      this.isEdit = true;
    }
    this.loading = true;
    let result: any = await this.emailConfigService.createUpdateEmailTemplateMasterdata(paramEmailTemplateId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listEmailType = result.listEmailType;
      this.listEmailStatus = result.listEmailStatus;
      this.listEmailTemplateToken = result.listEmailTemplateToken;
      let emailTemplateModel: emailTemplateModel = result.emailTemplateModel;
      let listEmailToCC = result.listEmailToCC;
      //nếu isEdit = true thì patch value
      if (this.isEdit == true) this.patchValueEmailTemplate(emailTemplateModel, listEmailToCC);
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  cancel() {
    this.router.navigate(['/admin/email-configuration']);
  }

  addTokenToContent(token: string) {
    let htmlValue = this.rteEle.getHtml();
    
    if (token === "[ACCESS_SYSTEM]") {
      let button = `<a class="e-rte-anchor" href="http://[ACCESS_SYSTEM]/" 
      title="Truy cập hệ thống" target="_blank" 
      style=" background-color: rgb(255, 255, 255); 
      color: rgb(46, 46, 241); text-decoration: underline; 
      font-family: Roboto, &quot;Segoe UI&quot;, GeezaPro, &quot;DejaVu Serif&quot;, sans-serif, -apple-system, BlinkMacSystemFont; 
      font-size: 14px; font-style: normal; font-weight: 400; text-align: start; text-indent: 0px; 
      background-color: #0f62fe;;
      color: #fff;
      padding: 5px 7px;
      white-space: normal;">Truy cập hệ thống</a>`;
      this.contenModel = button + htmlValue;
      return; 
    }

    this.contenModel = token + htmlValue;
  }

  patchValueEmailTemplate(emailTemplateModel: emailTemplateModel, listEmailToCC: Array<String>) {
    this.createEmailForm.get('EmailName').setValue(emailTemplateModel.emailTemplateName);
    this.createEmailForm.get('EmailTitle').setValue(emailTemplateModel.emailTemplateTitle);
    this.createEmailForm.get('EmailType').setValue(this.listEmailType.find(e => e.categoryId == emailTemplateModel.emailTemplateTypeId));
    this.createEmailForm.get('CCEmail').setValue(listEmailToCC);
    let activeValue: boolean = false;
    let compareActiveValue = this.listEmailStatus.find(e => e.categoryId == emailTemplateModel.emailTemplateStatusId);
    if (compareActiveValue.categoryCode === "CHL") {
      activeValue = true;
    } else {
      activeValue = false;
    }
    this.createEmailForm.get('Active').setValue(activeValue);
    this.contenModel = emailTemplateModel.emailTemplateContent;
  }

  async createEmail() {
    let checkEditor = this.validateEditor();
    if (!this.createEmailForm.valid || checkEditor === false) {
      Object.keys(this.createEmailForm.controls).forEach(key => {
        if (!this.createEmailForm.controls[key].valid) {
          this.createEmailForm.controls[key].markAsTouched();
        }
      });
    } else {
      let emailTemplateModel = this.mappingFormToModel();
      let listCCEmail: Array<string> = this.getCCEmail();
      this.loading = true;
      let result: any = await this.emailConfigService.createUpdateEmailTemplate(emailTemplateModel, listCCEmail, this.auth.UserId);
      
      this.loading = false;
      if (result.statusCode === 200) {
        
        if (this.isEdit == false) {
          this.router.navigate(['/admin/email-configuration']);
        } else {
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Chỉnh sửa thành công');
        }
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    }
  }

  validateEditor(): boolean {
    let text = this.rteEle.getText();
    if (text.trim() === "") {
      $("#defaultRTE").addClass("error-border");
      $("#errorTextEditor").addClass("error-text-editor");
      this.validatorEditor = false;
      return false;
    } else {
      $("#defaultRTE").removeClass("error-border");
      $("#errorTextEditor").removeClass("error-text-editor");
      this.validatorEditor = true;
    }
    return true;
  }
  
  mappingFormToModel(): EmailTemplateModel {
    let htmlValue = this.rteEle.getHtml();
    let emailTemplateModel = new EmailTemplateModel();
    if (this.isEdit == false) {
      emailTemplateModel.EmailTemplateId = this.emptyGuid;
    } else {
      emailTemplateModel.EmailTemplateId = this.paramEmailTemplateId;
    }
    emailTemplateModel.EmailTemplateName = this.createEmailForm.get('EmailName').value;
    emailTemplateModel.EmailTemplateTitle = this.createEmailForm.get('EmailTitle').value;
    emailTemplateModel.EmailTemplateContent = htmlValue;
    emailTemplateModel.EmailTemplateTypeId = this.createEmailForm.get('EmailType').value.categoryId;
    let status = this.createEmailForm.get('Active').value;
    if (status == true) {
      emailTemplateModel.EmailTemplateStatusId = this.listEmailStatus.find(e => e.categoryCode == "CHL").categoryId;
    } else {
      emailTemplateModel.EmailTemplateStatusId = this.listEmailStatus.find(e => e.categoryCode == "HHL").categoryId;
    }
    emailTemplateModel.Active = true;
    emailTemplateModel.CreatedById = this.emptyGuid;
    emailTemplateModel.CreatedDate = new Date();
    emailTemplateModel.UpdatedById = this.emptyGuid;
    emailTemplateModel.UpdatedDate = new Date();
    return emailTemplateModel
  }

  getCCEmail(): Array<string> {
    return this.createEmailForm.get('CCEmail').value;
  }

  /* Thay đổi EmailType */
  changeEmailType() {
    //Load các token tương ứng với EmailType
    let emailTemplateTypeId = this.createEmailForm.get('EmailType').value.categoryId;
    this.listEmailTemplateToken = [];

    this.emailConfigService.getTokenForEmailTypeId(emailTemplateTypeId).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listEmailTemplateToken = result.listEmailTemplateToken;
      } else {
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

}
