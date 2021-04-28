import { Component, OnInit, ElementRef, Inject, HostListener, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { TranslateService } from '@ngx-translate/core';
import { FailComponent } from '../../../../../shared/toast/fail/fail.component';
import { SuccessComponent } from '../../../../../shared/toast/success/success.component';
import { WorkflowModel } from '../../models/workflow.model';
import { WorkflowStepModel } from '../../models/workflow-step.model';
import { WorkflowService } from '../../services/workflow.service';
import { CategoryService } from '../../../../../shared/services/category.service';
import { PositionService } from "../../../../../shared/services/position.service";
import { EmployeeService } from '../../../../../employee/services/employee.service';
import { EmployeeModel } from '../../../../../employee/models/employee.model';
import { Observable } from 'rxjs';
import { startWith ,  map } from 'rxjs/operators';
import { ValidatorsCommon } from '../../../../../shared/CustomValidation/ValidationCommon';
import * as moment from 'moment';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { WarningComponent } from '../../../../../shared/toast/warning/warning.component';
import { GetPermission } from '../../../../../shared/permission/get-permission';

@Component({
  selector: 'app-workflow-detail',
  templateUrl: './workflow-detail.component.html',
  styleUrls: ['./workflow-detail.component.css']
})

export class WorkflowDetailComponent implements OnInit {
  @HostListener('document:keyup', ['$event'])
  handleDeleteKeyboardEvent(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      //this.createWorkflow();
    }
  }

  // Loading config
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
  loading: boolean = false;

  wfStatusCode: string = 'TWO';
  systemFeatureCode: string = 'SYSFEA';

  isPosition: boolean = true;
  systemFeatures: Array<any> = [];
  wfStatus: Array<any> = [];
  postionList: Array<any> = [];
  employeeList: Array<any> = [];
  finalStep: number = 2;
  submitted = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  filteredEmployee: Observable<string[]>;
  filteredEmployeeFinal: Observable<string[]>;
  emptyString: string = '';
  workflowId: string = '';
  isdisabled: boolean = true;

  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  createWorkflowForm: FormGroup;
  workflowStepFormList: Array<any> = [];
  stepList: Array<any> = [];

  workflowModel: WorkflowModel = {
    WorkFlowId: this.emptyGuid,
    Name: "",
    Description: "",
    SystemFeatureId: "",
    WorkflowCode: "",
    StatusId: "",
    CreatedDate: new Date(),
    CreatedBy: this.userId,
    UpdatedDate: null,
    UpdatedBy: "",
    WorkflowSteps: null
  };

  workflowStepModel: WorkflowStepModel = {
    WorkflowStepId: this.emptyGuid,
    StepNumber: 2,
    NextStepNumber: 0,
    ApprovebyPosition: true,
    ApproverPositionId: "",
    ApproverId: "",
    WorkflowId: this.emptyGuid,
    BackStepNumber: 0,
    ApprovedText: "",
    NotApprovedText: "",
    RecordStatus: ""
  };

  firstStepModel: WorkflowStepModel = {
    WorkflowStepId: this.emptyGuid,
    StepNumber: 1,
    NextStepNumber: 2,
    ApprovebyPosition: true,
    ApproverPositionId: "",
    ApproverId: "",
    WorkflowId: this.emptyGuid,
    BackStepNumber: 0,
    ApprovedText: "",
    NotApprovedText: "",
    RecordStatus: ""
  };
  lastStepModel: WorkflowStepModel = {
    WorkflowStepId: this.emptyGuid,
    StepNumber: 2,
    NextStepNumber: 0,
    ApprovebyPosition: true,
    ApproverPositionId: "",
    ApproverId: "",
    WorkflowId: this.emptyGuid,
    BackStepNumber: 1,
    ApprovedText: "",
    NotApprovedText: "",
    RecordStatus: ""
  };
  
  actionEdit: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    private categoryService: CategoryService,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router,
    private el: ElementRef,
    private employeeService: EmployeeService,
    private formBuilder: FormBuilder,
    private positionService: PositionService,
    private workflowService: WorkflowService
  ) { }

  ngOnInit() {
    this.createWorkflowForm = this.formBuilder.group({
      systemFeaturesControl: ['', [Validators.required]],
      nameControl: ['', [Validators.required]],
      codeControl: ['', Validators.required],
      statusControl: ['', Validators.required],
      descriptionControl: [''],
      employeeControl: [''],
      employeeControlFinal: ['']
    });

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getInItData();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/workflow/workflow-detail/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("edit") == -1) {
            this.actionEdit = false;
          }
          this.getInItData();
        }
      }
    }, error => { });
  }

  async getInItData() {
    this.loading = true;
    await this.getMasterData();
    this.disableControl();
    this.route.params.subscribe(params => { this.workflowId = params['workflowId']; });
    this.getWorkflowById(this.workflowId);
  }

  editMode(value: boolean) {
    if (value) {
      this.enableControl();
    } else {
      this.disableControl();
    }
  }

  enableControl() {
    this.createWorkflowForm.controls['systemFeaturesControl'].enable();
    this.createWorkflowForm.controls['nameControl'].enable();
    this.createWorkflowForm.controls['codeControl'].enable();
    this.createWorkflowForm.controls['statusControl'].enable();
    this.createWorkflowForm.controls['descriptionControl'].enable();
    this.isdisabled = false;
  }

  disableControl() {
    this.createWorkflowForm.controls['systemFeaturesControl'].disable();
    this.createWorkflowForm.controls['nameControl'].disable();
    this.createWorkflowForm.controls['codeControl'].disable();
    this.createWorkflowForm.controls['statusControl'].disable();
    this.createWorkflowForm.controls['descriptionControl'].disable();
    this.isdisabled = true;
  }

  /*Quay tro lai Workflow List*/
  goBack() {
    if (this.isdisabled) {
      this.router.navigate(['/admin/workflow/workflow-list']);
    } else {
      this.loading = true;
      this.disableControl();
      this.getWorkflowById(this.workflowId);
    }
  }
  /*Ket thuc*/

  async getMasterData() {
    await this.getListEmployee();

    this.positionService.getAllPosition().subscribe(response => {
      let result = <any>response;
      this.postionList = result.listPosition;
    }, error => { });

    this.categoryService.getAllCategoryByCategoryTypeCode(this.wfStatusCode).subscribe(response => {
      let result = <any>response;
      this.wfStatus = result.category;
    }, error => { });

    this.workflowService.getAllSystemFeature().subscribe(response => {
      let result = <any>response;
      this.systemFeatures = result.systemFeatureList;
    }, error => { });
  }

  get f() { return this.createWorkflowForm.controls; }

  updateWorkflowById() {
    this.submitted = true;
    this.workflowStepFormList.unshift(this.firstStepModel);
    this.workflowStepFormList.push(this.lastStepModel);
    if (this.createWorkflowForm.invalid) {
      return;
    } else {
      for (var i = 0; i < this.workflowStepFormList.length; i++) {
        this.workflowStepFormList[i].StepNumber = i + 1;
      }
      this.workflowService.updateWorkflowById(this.workflowModel, this.userId, this.workflowStepFormList).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          this.getWorkflowById(this.workflowId);
          this.workflowStepFormList = [];
          this.stepList = [];
          this.disableControl();
        } else {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        }
      }, error => {
        const result = <any>error;
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      });
    }
  }

  getWorkflowById(id: string) {
    this.workflowService.getWorkflowById(id).subscribe(response => {
      const result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.workflowModel = <WorkflowModel>({
          WorkFlowId: result.workflow.workFlowId,
          Name: result.workflow.name,
          Description: result.workflow.description,
          SystemFeatureId: result.workflow.systemFeatureId,
          WorkflowCode: result.workflow.workflowCode,
          StatusId: result.workflow.statusId,
          CreatedDate: result.workflow.createdDate,
          CreatedBy: result.workflow.createdBy,
          UpdatedDate: result.workflow.updatedDate,
          UpdatedBy: result.workflow.updatedBy
        });


        for (var i = 0; i < result.workflow.workflowStepList.length; i++) {
          var model = <WorkflowStepModel>({
            WorkflowStepId: result.workflow.workflowStepList[i].workflowStepId,
            StepNumber: result.workflow.workflowStepList[i].stepNumber,
            NextStepNumber: result.workflow.workflowStepList[i].nextStepNumber,
            ApprovebyPosition: result.workflow.workflowStepList[i].approvebyPosition,
            ApproverPositionId: result.workflow.workflowStepList[i].approverPositionId,
            ApproverId: result.workflow.workflowStepList[i].approverId,
            WorkflowId: result.workflow.workflowStepList[i].workflowId,
            BackStepNumber: result.workflow.workflowStepList[i].backStepNumber,
            ApprovedText: result.workflow.workflowStepList[i].approvedText,
            NotApprovedText: result.workflow.workflowStepList[i].notApprovedText,
            RecordStatus: result.workflow.workflowStepList[i].recordStatus
          });

          if (i == 0) {
            this.firstStepModel = model;
          } else if (i == result.workflow.workflowStepList.length - 1) {
            this.lastStepModel = model;
          } else {
            this.workflowStepFormList.push(model);
          }

          this.stepList.push(i + 1);
        }
        this.finalStep = result.workflow.workflowStepList.length;
        this.loading = false;
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        this.loading = false;
      }
    }, error => {
      const result = <any>error;
      this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      this.loading = false;
    });
  }

  addStep() {
    var item = {
      WorkflowStepId: this.emptyGuid,
      StepNumber: this.workflowStepFormList.length + 2,
      NextStepNumber: 0,
      ApprovebyPosition: true,
      ApproverPositionId: "",
      ApproverId: "",
      WorkflowId: this.emptyGuid,
      BackStepNumber: 0,
      ApprovedText: "",
      NotApprovedText: "",
      RecordStatus: ""
    };
    this.workflowStepFormList.push(item);
    this.lastStepModel.StepNumber += 1;
    this.lastStepModel.BackStepNumber += 1;
    this.finalStep = this.workflowStepFormList.length + 2;
    this.stepList.push(this.workflowStepFormList.length + 2);
  }

  deleteStep(index) {
    for (var i = index; i < this.workflowStepFormList.length; i++) {
      if (i + 1 < this.workflowStepFormList.length) {
        this.workflowStepFormList[i + 1].StepNumber -= 1;
      }
    }
    this.workflowStepFormList.splice(index, 1);
    this.finalStep -= 1;
    this.stepList.pop();
  }

  async getListEmployee() {
    var result: any = await this.employeeService.searchEmployeeAsync(this.emptyString, this.emptyString, this.emptyString, this.emptyString, this.emptyString);
    this.employeeList = result.employeeList;
    var listMapEmpId = result.listIdentityMapEmpId;
    this.employeeList.forEach(function (item) {
      const toSelectOrderer = listMapEmpId.find(c => c.employeeId == item.employeeId);
      if (typeof toSelectOrderer !== "undefined") {
        item.employeeCode = toSelectOrderer.employeeCode;
      }
    });
    this.filteredEmployee = this.createWorkflowForm.controls['employeeControl'].valueChanges
      .pipe(
        startWith(''),
        map(state => state ? this._filterOrderer(state, this.employeeList) : this.employeeList.slice())
      );
    this.filteredEmployeeFinal = this.createWorkflowForm.controls['employeeControlFinal'].valueChanges
      .pipe(
        startWith(''),
        map(state => state ? this._filterOrderer(state, this.employeeList) : this.employeeList.slice())
      );
  }

  selectOrderer(event: MatAutocompleteSelectedEvent) {
    this.createWorkflowForm.controls['employeeControl'].setValue(event.option.viewValue);
    //this.vendorOrderModel.Orderer = event.option.value;
  }

  selectEmployee(event: MatAutocompleteSelectedEvent) {
    this.createWorkflowForm.controls['employeeControlFinal'].setValue(event.option.viewValue);
    //this.vendorOrderModel.Orderer = event.option.value;
  }

  private _filterOrderer(value: string, array: any) {
    return array.filter(state =>
      state.employeeName.toLowerCase().indexOf(value.toLowerCase()) >= 0 || state.identity.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  }
}
