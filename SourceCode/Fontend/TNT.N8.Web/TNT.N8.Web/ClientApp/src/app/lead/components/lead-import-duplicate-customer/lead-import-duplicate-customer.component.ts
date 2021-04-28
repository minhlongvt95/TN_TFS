import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';


import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";
import { SelectionModel } from '@angular/cdk/collections';
import { ContactModel } from '../../../shared/models/contact.model';
import { LeadService } from '../../services/lead.service';
import { EmployeeService } from "../../../employee/services/employee.service";

export interface IDialogData {
  lstcontactContactDuplicate: Array<ContactModel>;
  //lstcontactLeadDuplicate: Array<LeadModel>;
  isTrue: Boolean;
}

@Component({
  selector: 'app-lead-import-duplicate-customer',
  templateUrl: './lead-import-duplicate-customer.component.html',
  styleUrls: ['./lead-import-duplicate-customer.component.css']
})
export class CustomerImportDuplicateComponent implements OnInit {

  displayedColumns: string[] = ['select', 'name', 'email', 'phone', 'address'];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };

  selection: SelectionModel<any>;
  selectedContact: Array<ContactModel> = [];
  //selectedLead: Array<LeadModel> = [];

  selectedRowIndex: string;


  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  listContactModel: Array<ContactModel> = [];
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

  constructor(
    public snackBar: MatSnackBar,
    private employeeService: EmployeeService,
    private leadService: LeadService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<CustomerImportDuplicateComponent>,
    public dialogPop: MatDialog,
  ) { }

  ngOnInit() {

    this.selection = new SelectionModel(true, []);
    this.dataSource = new MatTableDataSource<any>(this.data.lstcontactContactDuplicate);
    this.masterToggle();

  }

  ngAfterViewInit() {
    this.paginationFunction();
  }

  paginationFunction() {
    this.dataSource.paginator = this.paginator;
    this.paginator._intl.itemsPerPageLabel = 'Số khách hàng mỗi trang: ';
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

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
      this.selectedContact = [];
      //this.selectedLead = [];
    } else {

      this.selectedContact = [];
      //this.selectedLead = [];
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selection.select(item);
        this.selectedContact.push(item);
        //var Lead = this.data.lstcontactLeadDuplicate.find(row => row.LeadId == item.ObjectId);
        //if (typeof Lead !== "undefined") {
        //  this.selectedLead.push(Lead);
        //}

      });
    }
  }

  rowCheckboxClick(event: MatCheckboxChange, row) {
    if (event) {
      this.selection.toggle(row);
    }
    if (event.checked) {
      this.selectedContact.push(row);
      //var Lead = this.data.lstcontactLeadDuplicate.find(item => item.LeadId == row.ObjectId);
      //if (typeof Lead !== "undefined") {
      //  this.selectedLead.push(Lead);
      //}

    } else {
      this.selectedContact.splice(this.selectedContact.indexOf(row), 1);
      //var Lead = this.data.lstcontactLeadDuplicate.find(item => item.LeadId == row.ObjectId);
      //if (typeof Lead !== "undefined") {
      //  this.selectedLead.splice(this.selectedLead.indexOf(Lead), 1);
      //}
    }
  }

  onCancelClick() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }

  onSaveClick() {
    this.loading = true;

    var listCheck: any = this.selection.selected;
    var excelExport = [];
    let excel = {
      'STT': '',
      'Tên': '',
      'Email': '',
      'SĐT': '',
      'Địa chỉ': '',
    };

    for (var i = 0; i < listCheck.length; i++) {
      let stt = i + 1;
      
      excel = {
        'STT': '' + stt,
        'Tên': listCheck[i].FirstName + listCheck[i].LastName,
        'Email': listCheck[i].Email,
        'SĐT': listCheck[i].Phone,
        'Địa chỉ': listCheck[i].Address,
      }
      excelExport.push(excel);
    }
    if (excelExport.length == 0) excelExport.push(excel);
    this.employeeService.exportExcel(excelExport, 'Excel_Danh sách khách hàng');
    this.loading = false;

    //if (this.selectedLead.length > 0) {
    //  this.leadService.updateLeadDuplicate(this.selectedLead, this.selectedContact, this.auth.UserId).subscribe(response => {
    //    const result = <any>response;
    //    if (result.statusCode === 202 || result.statusCode === 200) {
    //      this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
    //    }
    //    else {
    //      this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
    //    }
    //    this.loading = false;
    //  }, error => { })
    //}
    //else {
    //  this.snackBar.openFromComponent(FailComponent, { data: "Cần lựa chọn một bản ghi trước khi ghi đè!.", ...this.failConfig });
    //}
  }

}
