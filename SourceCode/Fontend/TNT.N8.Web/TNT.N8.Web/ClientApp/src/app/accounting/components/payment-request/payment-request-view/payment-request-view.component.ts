import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';

import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { startWith, map } from 'rxjs/operators';
import { OrganizationpopupComponent } from '../../../../shared/components/organizationpopup/organizationpopup.component';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ContactService } from '../../../../shared/services/contact.service';
import { ImageUploadService } from '../../../../shared/services/imageupload.service';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { CategoryService } from '../../../../shared/services/category.service';
import { PositionService } from '../../../../shared/services/position.service';
import { WorkflowService } from '../../../../admin/components/workflow/services/workflow.service';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subscription, Observable } from 'rxjs';
import { PaymentRequestModel } from '../../../models/paymentRequest.model';
import { NgxLoadingComponent, ngxLoadingAnimationTypes } from 'ngx-loading';
import { PaymentRequestService } from '../../../services/payment-request.service';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FailComponent } from '../../../../shared/toast/fail/fail.component';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { Document } from '../../../../shared/models/document.model';

@Component({
  selector: 'app-payment-request-view',
  templateUrl: './payment-request-view.component.html',
  styleUrls: ['./payment-request-view.component.css']
})
export class PaymentRequestViewComponent implements OnInit {
  viewMode = true;
  @ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  loadingConfig: any = {
    'animationType': ngxLoadingAnimationTypes.circle,
    'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
    'backdropBorderRadius': '4px',
    'primaryColour': '#ffffff',
    'secondaryColour': '#999999',
    'tertiaryColour': '#ffffff'
  }
  requestPaymentId: string;
  loading: boolean = false;
  auth: any = JSON.parse(localStorage.getItem("auth"));
  pageFirstLoad: boolean = true;
  postionList: Array<any> = [];
  listStatus: Array<any> = [];
  listPaymenMethod: Array<any> = [];
  employeeList: Array<any> = [];
  filteredEmployee: Observable<string[]>;
  filteredEmployee2: Observable<string[]>;
  emptyString: string = '';
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  createPRForm: FormGroup;
  selectedOrgName: string = '';
  totalPRprice: number = 0;
  submitted: boolean = false;
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  featureCode: string = 'PDDXTT';
  isApproved: boolean = false;
  isRejected: boolean = false;
  isInApprovalProgress: boolean = true;

  prModel = new PaymentRequestModel();
  lstDocumentModel: Array<Document> = [];
  lstDocumentGuid: Array<string> = [];
  //Khai bao cac variable de upload file
  accept = 'image/*, video/*, audio/*, .zip, .rar, .pdf, .xls, .xlsx, .doc, .docx, .ppt, .pptx, .txt';
  files: File[] = [];
  progress: number;
  hasBaseDropZoneOver = false;
  lastFileAt: Date;
  httpEmitter: Subscription;
  sendableFormData: any;
  maxSize: number = 11000000;
  lastInvalids: any;
  baseDropValid: any;
  dragFiles: any;
  notes: any = [];

  dialogPopup: MatDialogRef<OrganizationpopupComponent>;
  dialogConfirmPopup: MatDialogRef<PopupComponent>;

  constructor(private translate: TranslateService,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog,
    private el: ElementRef,
    private formBuilder: FormBuilder,
    private positionService: PositionService,
    private workflowService: WorkflowService,
    private cateService: CategoryService,
    private employeeService: EmployeeService,
    private imageService: ImageUploadService,
    private contactService: ContactService,
    private paymentRequestService: PaymentRequestService,
  ) { }

  async ngOnInit() {
    this.route.params.subscribe(params => { this.requestPaymentId = params['requestId'] });
    const phonePattern = '^([+84]|[84]|0)+[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$';
    this.createPRForm = this.formBuilder.group({
      contentControl: ['', [Validators.required]],
      ordererControl: [{value: '' ,disabled: true}, [Validators.required]],
      phoneControl: ['', [Validators.pattern(phonePattern)]],
      orgControl: ['', [Validators.required]],
      approverControl: ['',[Validators.required]],
      positionControl: [{ disabled: true, value: '' }],
      descriptionControl: [''],
      totalMoneyControl: ['', [Validators.required]],
      paymentMethodControl: ['',[Validators.required]]
    });
    this.createPRForm.disable();
    this.getPaymentRequestById(this.requestPaymentId);
    await this.getMasterData();
  }

  get f() { return this.createPRForm.controls; }

  getPaymentRequestById(id: string) {
    this.notes = [];

    this.paymentRequestService.getRequestPaymentById(id, this.auth.UserId).subscribe(
      response => {
        let result = <any>response;
        this.prModel = <PaymentRequestModel>({
          RequestPaymentId: result.requestPaymentEntityModel.requestPaymentId,
          RequestPaymentCode: result.requestPaymentEntityModel.requestPaymentCode,
          RequestPaymentCreateDate: result.requestPaymentEntityModel.requestPaymentCreateDate,
          RequestPaymentNote: result.requestPaymentEntityModel.requestPaymentNote,
          RequestEmployee: result.requestPaymentEntityModel.requestEmployee,
          RequestEmployeePhone: result.requestPaymentEntityModel.requestEmployeePhone,
          RequestBranch: result.requestPaymentEntityModel.requestBranch,
          ApproverId: result.requestPaymentEntityModel.approverId,
          PostionApproverId: result.requestPaymentEntityModel.postionApproverId,
          TotalAmount: result.requestPaymentEntityModel.totalAmount,
          PaymentType: result.requestPaymentEntityModel.paymentType,
          Description: result.requestPaymentEntityModel.description,
          NumberCode: result.requestPaymentEntityModel.numberCode,
          YearCode: result.requestPaymentEntityModel.yearCode,
          StatusID: result.requestPaymentEntityModel.statusID,
          CreateDate: result.requestPaymentEntityModel.createDate,
          CreateById: result.requestPaymentEntityModel.createById,
          RequestEmployeeName: result.requestPaymentEntityModel.requestEmployeeName,
          OrganizationName: result.requestPaymentEntityModel.branchName
        });
        this.createPRForm.controls['ordererControl'].setValue(this.prModel.RequestEmployeeName);
        this.selectedOrgName = this.prModel.OrganizationName;
        this.isApproved = result.isApprove;
        this.isInApprovalProgress = result.isSendingApprove;
        this.isRejected = result.isReject;
        this.notes = result.notes;



        if (result.lstDocument.length > 0) {
          for (var i = 0; i < result.lstDocument.length; ++i) {
            var model = this.convertToModel(result.lstDocument[i]);
            this.lstDocumentModel.push(model);
          }
        }
        //this.files = result.lstDoc;
      }, error => { });
  }

  convertToModel(item: any) {
    var model = <Document>({
      DocumentId: item.documentId,
      Active: item.Active,
      ContentType: item.contentType,
      DocumentUrl: item.documentUrl,
      Extension: item.extension,
      Header: item.header,
      Name: item.name,
      ObjectId: item.objectId,
      Size: item.size,
      CreatedById: item.createdById,
      CreatedDate: item.createdDate,
      UpdatedById: item.updatedById,
      UpdatedDate: item.updatedDate
    });
    return model;
  }

  openOrgPopup() {
    this.dialogPopup = this.dialog.open(OrganizationpopupComponent,
      {
        width: '550px',
        height: '580',
        autoFocus: false,
        data: {}
      });
    this.dialogPopup.afterClosed().subscribe(result => {
      if (result != undefined) {
        this.selectedOrgName = result.selectedOrgName;
        this.prModel.RequestBranch = result.selectedOrgId;
      }
    });
  }

  async getMasterData() {
    await this.getListEmployee();

    this.positionService.getAllPosition().subscribe(response => {
      let result = <any>response;
      this.postionList = result.listPosition;
    }, error => { });

    let rs: any = await this.cateService.getAllCategoryByCategoryTypeCodeAsyc('DDU');
    this.listStatus = rs.category;

    let rsmt: any = await this.cateService.getAllCategoryByCategoryTypeCodeAsyc('PTO');
    this.listPaymenMethod = rsmt.category;
  }

  async getListEmployee() {
    let result: any = await this.employeeService.searchEmployeeAsync(this.emptyString, this.emptyString, this.emptyString, this.emptyString, this.emptyString);
    this.employeeList = result.employeeList;
    const approver = this.employeeList.find(emp => emp.employeeId == this.prModel.ApproverId);
    if (approver != undefined){
      this.prModel.ApprovedName = approver.employeeCode + ' - ' + approver.employeeName;
      this.createPRForm.controls['approverControl'].setValue(this.prModel.ApprovedName)
    }
    let listMapEmpId = result.listIdentityMapEmpId;
    this.employeeList.forEach(function (item) {
      const toSelectOrderer = listMapEmpId.find(c => c.employeeId == item.employeeId);
      item.employeeCode = toSelectOrderer.employeeCode;
      item.contactId = toSelectOrderer.contactId;
    });
    this.filteredEmployee = this.createPRForm.controls['ordererControl'].valueChanges
      .pipe(
        startWith(''),
        map(state => state ? this._filter(state, this.employeeList) : this.employeeList.slice())
      );
    this.filteredEmployee2 = this.createPRForm.controls['approverControl'].valueChanges
      .pipe(
        startWith(''),
        map(state => state ? this._filter(state, this.employeeList) : this.employeeList.slice())
      );
  }

  // Khai báo bi?n ki?m tra option autocomplete có du?c ch?n dúng chua ???
	validationPickAutoComplete = true;
	checkPickedOption(event){
		if (this.prModel.ApproverId == null || this.prModel.ApproverId == ""){
			this.validationPickAutoComplete = false;
		} else {
			const approver = this.employeeList.find(emp => emp.employeeId == this.prModel.ApproverId);
			if (approver != undefined){
				if (event.target.value.trim() !== (approver.employeeCode.trim() + ' - ' + approver.employeeName.trim())){
					this.validationPickAutoComplete = false;
				} else this.validationPickAutoComplete = true;
			}
		}
		if (!this.validationPickAutoComplete){
			this.createPRForm.controls['approverControl'].setErrors({'incorrectPickerAutocomplete': true});
		} else {
			this.createPRForm.controls['approverControl'].setErrors(null);
		}
	}
	// End

  selectOrderer(event: MatAutocompleteSelectedEvent) {
    this.createPRForm.controls['ordererControl'].setValue(event.option.viewValue);
    this.prModel.RequestEmployee = event.option.value.employeeId;
    this.contactService.getContactById(event.option.value.contactId, this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.prModel.RequestEmployeePhone = result.contact.phone == null ? '' : result.contact.phone;
    })
  }

  selectApprover(event: MatAutocompleteSelectedEvent) {
    this.createPRForm.controls['approverControl'].setValue(event.option.viewValue);
    this.prModel.ApprovedName = event.option.viewValue;
    this.prModel.ApproverId = event.option.value;
    let emp = this.employeeList.find(e => e.employeeId === event.option.value);
    this.prModel.PostionApproverId = emp.positionId;
  }

  private _filter(value: string, array: any) {
    const _value = <string>value.toString().toLowerCase();
    return array.filter(state =>
      state.employeeName.toLowerCase().includes(_value) || state.employeeCode.toLowerCase().includes(_value));
  }

  // // Upload file to server
  //uploadFiles(files: File[]) {
  //	this.imageService.uploadFile(files).subscribe(response => { }, error => { });
  // }
  onRemoveDocument(index: number) {
    this.lstDocumentModel.splice(index,1)
  }
  getDate() {
    return new Date();
  }
  onRemoveFile(index: number) {
    this.files.splice(index, 1);
  }
  cancel() {
    this.router.navigate(['/accounting/payment-request-list']);
  }
  checkApprover() {
    return this.auth.EmployeeId == this.prModel.ApproverId;
  }
  checkExistSize(item: any) {
    if (typeof item.size !== 'undefined') {
      return true;
    }
    else {
      return false;
    }
  }
  editPR() {
    let result:Array<string> = [];
    if (this.lstDocumentModel.length > 0) {
      result = this.lstDocumentModel.map(a => a.DocumentId);
    }
    if (this.createPRForm.valid){
      this.paymentRequestService.editRequestPayment(this.files, this.prModel, result, this.auth.UserId).subscribe(response => {
        let result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          this.getPaymentRequestById(this.requestPaymentId);
          this.changeMode(false);
        }
        else {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        }
      }, error => {
        let result = <any>error;
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      });
    }
  }
  changeMode(value){
    if (value){
      this.createPRForm.enable();
      this.createPRForm.controls['ordererControl'].disable();
      this.createPRForm.controls['positionControl'].disable();
    } else{
      this.createPRForm.disable();
    }
  }

  sendApprove(){
    this.isInApprovalProgress = true;
    this.isApproved = false;
    this.isRejected = false;
    this.workflowService.nextWorkflowStep(this.featureCode, this.requestPaymentId, '', this.isRejected, 
      '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
        }
      }, error => {});
    }

    goToNextStep(step: string){
      let _title = "XÁC NHẬN";
      let _content = "";
      let _mode = "";
      let _height = "";
      if(step == 'approve'){
        this.isApproved = true;
        _content = "Bạn có chắc chắn muốn phê duyệt đề xuất này.";
        _height = '250px';
      }else {
        this.isRejected = true;
        _mode = "reject";
        _content = "Bạn có chắc chắn muốn từ chối đề xuất này.";
        _height = '300px';
      }

      this.dialogConfirmPopup = this.dialog.open(PopupComponent,
      {
        width: '500px',
        height: _height,
        autoFocus: false,
        data: { title: _title, content: _content, mode: _mode }
      });

      this.dialogConfirmPopup.afterClosed().subscribe(result => {
        if (result != undefined && (result || result.ok)) {
          let msg = result.message != null ? result.message : '';
          //Bắt đầu phê duyệt hoặc từ chối
          this.workflowService.nextWorkflowStep(this.featureCode, this.requestPaymentId, msg , this.isRejected, 
            '', this.isApproved, this.isInApprovalProgress).subscribe(response => {
              const presult = <any>response;
              if (presult.statusCode === 202 || presult.statusCode === 200) {
                this.snackBar.openFromComponent(SuccessComponent, { data: presult.messageCode, ...this.successConfig });
                this.getPaymentRequestById(this.requestPaymentId);
              }
            }, error => {});
          }else{
            this.isApproved = false;
            this.isRejected = false;
          }
        });
    }
}
