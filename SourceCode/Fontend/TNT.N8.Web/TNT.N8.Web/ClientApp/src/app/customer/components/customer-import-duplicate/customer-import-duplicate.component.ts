import { Component, OnInit, ViewChild, Inject, ElementRef } from '@angular/core';

import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatCheckboxChange } from '@angular/material/checkbox';

import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { CustomerService } from '../../services/customer.service';
import { SelectionModel } from '@angular/cdk/collections';

import { CustomerModel } from '../../models/customer.model';
import { ContactModel } from '../../../shared/models/contact.model';

import { ngxLoadingAnimationTypes, NgxLoadingComponent } from "ngx-loading";



export interface IDialogData {
  lstcontactContactDuplicate: Array<ContactModel>;
  lstcontactContact_CON_Duplicate: Array<ContactModel>;
  lstcontactCustomerDuplicate: Array<CustomerModel>;
  isTrue: Boolean;
}

@Component({
  selector: 'app-customer-import-duplicate',
  templateUrl: './customer-import-duplicate.component.html',
  styleUrls: ['./customer-import-duplicate.component.css']
})
export class CustomerImportDuplicateComponent implements OnInit {
  displayedColumns: string[] = ['select', 'name', 'email', 'phone', 'address'];
  auth: any = JSON.parse(localStorage.getItem("auth"));
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };

  selection: SelectionModel<any>;
  selectedContact: Array<ContactModel> = [];
  selectedCustomer: Array<CustomerModel> = [];
  selectedContact_Con: Array<ContactModel> = [];

  selectedRowIndex: string;


  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  listContachModel: Array<ContactModel> = [];

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
    private customerService: CustomerService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<CustomerImportDuplicateComponent>,
    public dialogPop: MatDialog,
  ) { }

  ngOnInit() {
    for (var i = 0; i < this.data.lstcontactContactDuplicate.length; ++i) {
      var contact = this.data.lstcontactContactDuplicate[i];
      if (contact.FirstName == null && contact.LastName == null) {
        var Customer = this.data.lstcontactCustomerDuplicate.find(item => item.CustomerId == contact.ObjectId);
        if (Customer != null) {
          this.data.lstcontactContactDuplicate[i].FirstName = Customer.CustomerName;
          //this.data.lstcontactContactDuplicate[i].lastName = Customer.lastName;
        }
      }
    }
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
      this.selectedCustomer = [];
      this.selectedContact_Con = [];
    } else {

      this.selectedContact = [];
      this.selectedCustomer = [];
      this.selectedContact_Con = [];
      this.dataSource.data.forEach(rows => {
        const item: any = rows;
        this.selection.select(item);
        this.selectedContact.push(item);
        var Customer = this.data.lstcontactCustomerDuplicate.find(row => row.CustomerId == item.ObjectId);
        if (typeof Customer !== "undefined") {
          this.selectedCustomer.push(Customer);
        }
        var contact_con = this.data.lstcontactContact_CON_Duplicate.find(row => row.ObjectId == item.ObjectId);
        if (typeof contact_con !== "undefined") {
          this.selectedContact_Con.push(contact_con);
        }
      });
    }
  }

  rowCheckboxClick(event: MatCheckboxChange, row) {
    if (event) {
      this.selection.toggle(row);
    }
    if (event.checked) {
      this.selectedContact.push(row);
      var Customer = this.data.lstcontactCustomerDuplicate.find(item => item.CustomerId == row.ObjectId);
      if (typeof Customer !== "undefined") {
        this.selectedCustomer.push(Customer);
      }
      var contact_con = this.data.lstcontactContact_CON_Duplicate.find(item => item.ObjectId == row.ObjectId);
      if (typeof contact_con !== "undefined") {
        this.selectedContact_Con.push(contact_con);
      }

    } else {
      this.selectedContact.splice(this.selectedContact.indexOf(row), 1);
      var Customer = this.data.lstcontactCustomerDuplicate.find(item => item.CustomerId == row.ObjectId);
      if (typeof Customer !== "undefined") {
        this.selectedCustomer.splice(this.selectedCustomer.indexOf(Customer), 1);
      }
      var contact_con = this.data.lstcontactContact_CON_Duplicate.find(item => item.ObjectId == row.ObjectId);
      if (typeof contact_con !== "undefined") {
        this.selectedContact_Con.splice(this.selectedContact_Con.indexOf(contact_con), 1);
      }
    }
  }

  onCancelClick() {
    this.data.isTrue = false;
    this.dialogRef.close(this.data);
  }

  onSaveClick() {
    this.loading = true;
    if (this.selectedCustomer.length > 0) {
      this.customerService.updateCustomerDuplicate(this.selectedCustomer, this.selectedContact, this.selectedContact_Con, this.auth.UserId).subscribe(response => {
        const result = <any>response;
        if (result.statusCode === 202 || result.statusCode === 200) {
          this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
        }
        else {
          this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
        }
        this.loading = false;
      }, error => { })
    }
    else {
      this.snackBar.openFromComponent(FailComponent, { data: "Cần lựa chọn một bản ghi trước khi ghi đè!.", ...this.failConfig });
    }

  }

}
