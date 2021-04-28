import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { MatTableDataSource } from '@angular/material/table';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatPaginator } from '@angular/material/paginator';
import { MatSelectChange } from '@angular/material/select';

import { SelectionModel } from '@angular/cdk/collections';
import { CategoryService } from '../../../../shared/services/category.service';
import { TranslateService } from '@ngx-translate/core';
import { OrganizationService } from '../../../../shared/services/organization.service';
import { EmployeeService } from '../../../../employee/services/employee.service';
import { PaymentRequestService } from '../../../services/payment-request.service';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { OrganizationpopupComponent } from '../../../../shared/components/organizationpopup/organizationpopup.component';

@Component({
  selector: 'app-payment-request-list',
  templateUrl: './payment-request-list.component.html',
  styleUrls: ['./payment-request-list.component.css']
})
export class PaymentRequestListComponent implements OnInit {
  requestCode: string = '';
  startDate: Date;
  endDate: Date;
  selectedOrgid: string = '';
  paymentId: string = '';
  empOrganizationNameDisplay: string = '';
  selectedEmpId: string = '';
  displayedColumns = ['requestCode', 'requestName', 'requestContent', 'org', 'sum', 'createDate', 'payment', 'status'];
  dataSourceListPaymentRequest: MatTableDataSource<any>;
  dialogOrg: MatDialogRef<OrganizationpopupComponent>;
  errors: any = { isError: false, errorMessage: '' };
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  selection: SelectionModel<any>;

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  paymentRequestCreatePermission = 'payment-request/create';
  paymentRequestEditPermission = 'payment-request/update';
  paymentRequestDetailPermission = 'payment-request/detail';

  paymentList: Array<any> = [];
  requestList: Array<any> = [];

  //khai báo các biến cho danh sách options Status
  selectableStatus: boolean = true;
  removableStatus: boolean = true;
  selectedGroupStatus: Array<any> = [];
  listStatus: Array<any> = [];

  groupCtrlStatus = new FormControl();
  paymentControl = new FormControl();
  orgControl = new FormControl({ value: '', disabled: true });
  employeeNameControl = new FormControl();
  @ViewChild('groupInputStatus') groupInputStatus: ElementRef;

  paymentCode: string = 'PTO';
  statusCode: string = 'DDU';
  positionCode: string = 'NV';
  emptyString: string = '';
  employeeList: Array<any> = [];
  filteredEmployee: Observable<string[]>;

  constructor(
    private translate: TranslateService,
    private catService: CategoryService,
    private orgService: OrganizationService,
    private employeeService: EmployeeService,
    public dialog: MatDialog,
    private route: ActivatedRoute,
    private router: Router,
    private requestService: PaymentRequestService
  ) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.getMasterData();
    this.searchPaymentRequest();
  }
  
  searchPaymentRequest() {
    this.requestService.findRequestPayment(this.requestCode, this.selectedGroupStatus, this.startDate, this.endDate,
      this.selectedEmpId, this.paymentId, this.selectedOrgid, this.auth.UserId).subscribe(response => {
        var result = <any>response;
        if (result.requestList !== null) {
          this.requestList = result.requestList;
          this.dataSourceListPaymentRequest = new MatTableDataSource<any>(this.requestList);
          this.selection = new SelectionModel<any>(true, []);
          this.paginationFunction();
        }
      }, error => { });
  }

  paginationFunction() {
    this.dataSourceListPaymentRequest.paginator = this.paginator;
    this.paginator._intl.itemsPerPageLabel = 'Số yêu cầu mỗi trang: ';
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

  getMasterData() {
    /*Payment*/
    this.catService.getAllCategoryByCategoryTypeCode(this.paymentCode).subscribe(response => {
      let result = <any>response;
      this.paymentList = result.category;
    }, error => { });

    this.catService.getAllCategoryByCategoryTypeCode(this.statusCode).subscribe(response => {
      let result = <any>response;
      this.listStatus = result.category;
    }, error => { });

    this.employeeService.getEmployeeByPositionCode(this.positionCode).subscribe(response => {
      let result = <any>response;
      this.employeeList = result.employeeList;
      this.filteredEmployee = this.employeeNameControl.valueChanges.pipe(
        startWith(''),
        map((item: string) => item ? this._filterEmployee(item, this.employeeList) : this.employeeList.slice()));
    }, error => { });
  }

  private _filterEmployee(value: any, array: any): string[] {
    const filterValue = value;
    return array.filter(item => item.employeeName.toLowerCase().indexOf(filterValue.toLowerCase()) !== -1);
  }

  selectEmployee(event: MatAutocompleteSelectedEvent): void {
    this.employeeNameControl.setValue(event.option.viewValue);
    this.selectedEmpId = event.option.value;
  }

  // Open dialog select org
  openOrgDialog() {
    this.dialogOrg = this.dialog.open(OrganizationpopupComponent,
      {
        width: '500px',
        autoFocus: false,
        data: { selectedOrgId: this.selectedOrgid }
      });

    this.dialogOrg.afterClosed().subscribe(result => {
      if (result.selectedOrgId != undefined && result.selectedOrgId != null) {
        this.selectedOrgid = result.selectedOrgId;
        this.empOrganizationNameDisplay = result.selectedOrgName;
      }
    });
  }

  compareTwoDate(event: MatDatepickerInputEvent<Date>) {
    if (this.endDate == null || this.startDate == null) {
      this.errors = { isError: false, errorMessage: '' };
    } else {
      if (this.startDate >= this.endDate) {
        this.errors = { isError: true, errorMessage: "Từ ngày không được lớn hơn đến ngày" };
      } else {
        this.errors = { isError: false, errorMessage: '' };
      }
    }
  }

  goToRequest(id: string) {
    this.router.navigate(['/accounting/payment-request-detail', { requestId: id }]);
  }

  //hàm xử lý khi chọn một option trong chip list
  selectedFn(event: MatSelectChange, type: string) {
    if (type == 'Status') {
      this.selectedGroupStatus = event.value;
    }
  }

  refreshParameter() {
    this.requestCode = '';
    this.selectedGroupStatus = [];
    this.selectedOrgid = '';
    this.paymentId = '';
    this.selectedEmpId = '';
    this.startDate = null;
    this.endDate = null;
    this.searchPaymentRequest();
  }
}
