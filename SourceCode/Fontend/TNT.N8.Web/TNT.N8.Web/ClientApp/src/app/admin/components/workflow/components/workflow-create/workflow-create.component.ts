import { Component, OnInit, ElementRef, Inject, HostListener } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
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
import { startWith, map } from 'rxjs/operators';
import { ValidatorsCommon } from '../../../../../shared/CustomValidation/ValidationCommon';
import * as moment from 'moment';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { WarningComponent } from '../../../../../shared/toast/warning/warning.component';
import { GetPermission } from '../../../../../shared/permission/get-permission';

@Component({
  selector: 'app-workflow-create',
  templateUrl: './workflow-create.component.html',
  styleUrls: ['./workflow-create.component.css']
})

export class WorkflowCreateComponent implements OnInit {
  // @HostListener('document:keyup', ['$event'])
  // handleDeleteKeyboardEvent(event: KeyboardEvent) {
  //   if (event.key === 'Enter') {
  //     this.createWorkflow('save');
  //   }
  // }

  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  wfStatusCode: string = 'TWO';
  systemFeatureCode: string = 'SYSFEA';

  stepList: Array<any> = [1, 2];
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

  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  createWorkflowForm: FormGroup;
  systemFeaturesControl: FormControl;
  nameControl: FormControl;
  codeControl: FormControl;
  statusControl: FormControl;
  descriptionControl: FormControl;
  employeeControl: FormControl;
  employeeControlFinal: FormControl;

  workflowStepFormList: Array<any> = [];
  workflowStepList: Array<any> = [];

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

  async ngOnInit() {
    this.setForm();

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getMasterData();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/workflow/workflow-create/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.snackBar.openFromComponent(WarningComponent, { data: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ", ...this.warningConfig });
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("add") == -1) {
            this.actionAdd = false;
          }
          this.getMasterData();
        }
      }
    }, error => { });
  }

  setForm() {
    this.systemFeaturesControl = new FormControl('', [Validators.required]);
    this.nameControl = new FormControl('', [Validators.required]);
    this.codeControl = new FormControl('', [Validators.required]);
    this.statusControl = new FormControl('', [Validators.required]);
    this.descriptionControl = new FormControl('');

    this.createWorkflowForm = new FormGroup({
      systemFeaturesControl: this.systemFeaturesControl,
      nameControl: this.nameControl,
      codeControl: this.codeControl,
      statusControl: this.statusControl,
      descriptionControl: this.descriptionControl,
    });
  }

  /*Quay tro lai Workflow List*/
  goBack() {
    this.router.navigate(['/admin/workflow/workflow-list']);
  }
  /*Ket thuc*/

  getMasterData() {
    this.workflowService.getMasterDataCreateWorkflow().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.postionList = result.listPosition;
        this.wfStatus = result.listStatus;
        this.systemFeatures = result.listSystemFeature;
        this.employeeList = result.listEmployee;
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      }
    });
  }

  createWorkflow(mode: string) {
    if (!this.createWorkflowForm.valid) {
      Object.keys(this.createWorkflowForm.controls).forEach(key => {
        if (this.createWorkflowForm.controls[key].valid == false) {
          this.createWorkflowForm.controls[key].markAsTouched();
        }
      });
    } else {
      this.submitted = true;
      this.workflowStepFormList.unshift(this.firstStepModel);
      this.workflowStepFormList.push(this.lastStepModel);

      /*Map data to Model*/

      this.workflowModel.Name = this.nameControl.value.trim();
      this.workflowModel.WorkflowCode = this.codeControl.value.trim();
      this.workflowModel.Description = this.descriptionControl.value ? this.descriptionControl.value.trim() : "";
      this.workflowModel.SystemFeatureId = this.systemFeaturesControl.value ? this.systemFeaturesControl.value.systemFeatureId : null;
      this.workflowModel.StatusId = this.statusControl.value ? this.statusControl.value.categoryId : "";

      /*End*/
      
      this.workflowService.createWorkflow(this.workflowModel, this.userId, this.workflowStepFormList).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
          if (mode == 'save') {
            this.router.navigate(['/admin/workflow/workflow-list']);
          } else if (mode == 'new') {
            this.resetFieldValue();
          }
        } else {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        }
      }, error => {
        const result = <any>error;
        this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
      });
    }
  }

  resetFieldValue() {
    this.createWorkflowForm.reset();
    this.workflowStepFormList = [];
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
}
