import { Component, OnInit, ElementRef, Inject, HostListener, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatPaginator } from '@angular/material/paginator';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatTableDataSource } from '@angular/material/table';
import { MatChipInputEvent } from '@angular/material/chips';
import { TranslateService } from '@ngx-translate/core';
import { FailComponent } from '../../../../../shared/toast/fail/fail.component';
import { WorkflowModel } from '../../models/workflow.model';
import { WarningComponent } from '../../../../../shared/toast/warning/warning.component';
import { WorkflowService } from '../../services/workflow.service';
import { map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { SelectionModel } from '@angular/cdk/collections';
import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { EmployeeService } from '../../../../../employee/services/employee.service';
import { GetPermission } from '../../../../../shared/permission/get-permission';

@Component({
  selector: 'app-workflow-list',
  templateUrl: './workflow-list.component.html',
  styleUrls: ['./workflow-list.component.css']
})
export class WorkflowListComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @HostListener('document:keyup', ['$event'])
  handleDeleteKeyboardEvent(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.searchWorkflow();
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
  displayedColumns = ['workflowname', 'systemfeature', 'status'];
  dataSource: MatTableDataSource<any>;
  selection: SelectionModel<any>;
  workflowName: string = '';
  systemFeatureIdList: Array<string> = [];

  auth: any = JSON.parse(localStorage.getItem("auth"));
  pageFirstLoad: boolean = true;
  messages: any;
  listWorkflow: Array<WorkflowModel> = [];
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };

  filteredServiceLevel: Observable<string[]>;
  serviceLevels: Array<any> = [];
  serviceLevelCtrl = new FormControl();
  selectedServiceLevel: Array<any> = [];
  @ViewChild('serviceLevelInput', { static: true }) serviceLevelInput: ElementRef;
  selectable: boolean = true;
  removable: boolean = true;
  addOnBlur: boolean = true;

  actionAdd: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  constructor(
    private translate: TranslateService,
    private getPermission: GetPermission,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private router: Router,
    private el: ElementRef,
    private formBuilder: FormBuilder,
    private workflowService: WorkflowService
  ) { }

  ngOnInit() {
    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.loading = true;
        this.workflowService.getAllSystemFeature().subscribe(response => {
          let result = <any>response;
          this.serviceLevels = result.systemFeatureList;
          this.filteredServiceLevel = this.serviceLevelCtrl.valueChanges.pipe(
            startWith(null),
            map((item: string) => item ? this._filterServiceLevel(item, this.serviceLevels) : this.serviceLevels.slice()));
        }, error => { });

        this.searchWorkflow();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/workflow/workflow-list/";
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
          this.loading = true;
          this.workflowService.getAllSystemFeature().subscribe(response => {
            let result = <any>response;
            this.serviceLevels = result.systemFeatureList;
            this.filteredServiceLevel = this.serviceLevelCtrl.valueChanges.pipe(
              startWith(null),
              map((item: string) => item ? this._filterServiceLevel(item, this.serviceLevels) : this.serviceLevels.slice()));
          }, error => { });

          this.searchWorkflow();
        }
      }
    }, error => { });
  }

  selectedRowIndex: string = '';
  highlight(row) {
    this.selectedRowIndex = row.workflowId;
  }

  addServiceLevel(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.selectedServiceLevel.push(value.trim());
    }

    if (input) {
      input.value = '';
    }

    this.serviceLevelCtrl.setValue(null);
  }

  removeServiceLevel(item: string): void {
    const index = this.selectedServiceLevel.indexOf(item);

    if (index >= 0) {
      this.selectedServiceLevel.splice(index, 1);
      this.serviceLevels.push(item);
      this.serviceLevels.sort((a: any, b: any) => {
        let x = a.name.toLowerCase().trim();
        let y = b.name.toLowerCase().trim();
        return (x.localeCompare(y) === -1 ? -1 : 1);
      });
    }
  }

  selectedServiceLevelFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedServiceLevel.push(event.option.value);
    this.serviceLevelInput.nativeElement.value = '';
    this.serviceLevelCtrl.setValue(null);
    this.serviceLevels.splice(this.serviceLevels.indexOf(event.option.value), 1);
  }

  private _filterServiceLevel(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.systemFeatureId.indexOf(filterValue) === 0);
  }

  searchWorkflow() {
    this.loading = true;
    this.workflowService.searchWorkflow(this.workflowName, this.selectedServiceLevel.map(item => item.systemFeatureId), this.auth.UserId).subscribe(response => {
      let result = <any>response;
      this.listWorkflow = result.workflowList;

      if (result.workflowList.length === 0) {
        if (!this.pageFirstLoad) {
          this.snackBar.openFromComponent(WarningComponent, { data: result.messageCode, ... this.warningConfig });
        }
      }
      this.dataSource = new MatTableDataSource<any>(this.listWorkflow);
      this.selection = new SelectionModel<any>(true, []);
      this.paginationFunction();
      this.pageFirstLoad = false;
      this.loading = false;
    }, error => {
      this.loading = false;
    });
  }

  paginationFunction() {
    this.dataSource.paginator = this.paginator;
    this.paginator._intl.itemsPerPageLabel = 'Số quy trình mỗi trang: ';
    this.paginator._intl.getRangeLabel = (page, pageSize, length) => {
      if (length === 0 || pageSize === 0) {
        return '0 trên ' + length;
      }
      length = Math.max(length, 0);
      const startIndex = page * pageSize;
      // If the start index exceeds the list length, do not try and fix the end index to the end.
      const endIndex = startIndex < length ?
        Math.min(startIndex + pageSize, length) :
        startIndex + pageSize;
      return startIndex + 1 + ' - ' + endIndex + ' trên ' + length;
    };
  }

  goToCreate() {
    this.router.navigate(['/admin/workflow/workflow-create']);
  }

  onViewDetail(id) {
    this.router.navigate(['/admin/workflow/workflow-detail', { workflowId: id }]);
  }

  refreshParameter() {
    this.selectedServiceLevel = [];
    this.workflowName = '';
    this.searchWorkflow();
  }

  //Function sap xep du lieu
  isAscending: boolean = false;
  sort(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.listWorkflow.sort((a: any, b: any) => {
      let x: any = '';
      let y: any = '';
      switch (property) {
        case 'workflowname':
          x = a.name.toLowerCase().trim();
          y = b.name.toLowerCase().trim();
          break;
        case 'systemfeature':
          x = a.systemFeatureName.toLowerCase().trim();
          y = b.systemFeatureName.toLowerCase().trim();
          break;
        case 'status':
          x = a.statusName.toLowerCase().trim();
          y = b.statusName.toLowerCase().trim();
          break;
        default:
          break;
      }

      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataSource = new MatTableDataSource<any>(this.listWorkflow);
    this.dataSource.paginator = this.paginator;
  }
  //Ket thuc
}
