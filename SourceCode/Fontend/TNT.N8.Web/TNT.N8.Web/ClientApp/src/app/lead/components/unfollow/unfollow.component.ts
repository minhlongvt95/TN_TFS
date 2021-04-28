import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';

import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { Router, ActivatedRoute } from '@angular/router';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { CommonService } from '../../../shared/services/common.service';
import { LeadService } from "../../services/lead.service";
import { TranslateService } from '@ngx-translate/core';
import { CategoryService } from "../../../shared/services/category.service";
import { EmployeeService } from "../../../employee/services/employee.service";
import { NoteService } from '../../../shared/services/note.service';

import { LeadModel } from '../../models/lead.model';
import { ContactModel } from "../../../shared/models/contact.model";
import { NoteModel } from '../../../shared/models/note.model';

import { WarningComponent } from '../../../shared/toast/warning/warning.component';
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { ApproveRejectPopupComponent } from '../../../shared/components/approverejectpopup/approverejectpopup.component';

@Component({
  selector: 'app-unfollow',
  templateUrl: './unfollow.component.html',
  styleUrls: ['./unfollow.component.css']
})
export class UnfollowComponent implements OnInit {

  //Bắt đầu khai báo các biến
  controls = new FormControl();
  //Khai báo các biến cho Gridview
  displayedColumns = ['select', 'fullName', 'email', 'phonenumber', 'personInChargeFullName', 'statusName'];
  dataSource: MatTableDataSource<LeadModel>;
  selection: SelectionModel<LeadModel>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  dialogPopup: MatDialogRef<PopupComponent>;
  dialogApproveReject: MatDialogRef<ApproveRejectPopupComponent>;

  //Khai báo các array để lưu master data
  requirements: Array<string> = [];
  status: Array<any> = [];
  potentials: Array<string> = [];
  employees: Array<string> = [];
  listLead: Array<LeadModel> = [];
  unfollowLeadIdList: Array<any> = [];
  messages: any;

  //Khai báo các CategoryType
  private objectType: string = "LEA";
  private productService: string = "GPS";
  private potentialCode: string = "PT";
  private code: string = "LEAD_STT";

  //Khai bao cac permission
  userPermission: any = localStorage.getItem("UserPermission").split(",");
  isManager: boolean = localStorage.getItem("IsManager") === 'true';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  unFollowPermisison: string = "lead/unfollow";

  //Khai báo các biến Autocomplete
  selectable: boolean = true;
  removable: boolean = true;
  addOnBlur: boolean = true;
  filteredStatus: Observable<string[]>;
  filteredRequirement: Observable<string[]>;
  filteredPotential: Observable<string[]>;
  filteredPic: Observable<string[]>;
  statusCtrl = new FormControl();
  requirementCtrl = new FormControl();
  potentialCtrl = new FormControl();
  picCtrl = new FormControl();
  selectedStatus: Array<any> = [];
  selectedRequirement: Array<any> = [];
  selectedPotential: Array<any> = [];
  selectedPic: Array<any> = [];
  @ViewChild('statusInput', { static: true }) statusInput: ElementRef;
  @ViewChild('requirementInput') requirementInput: ElementRef;
  @ViewChild('potentialInput') potentialInput: ElementRef;
  @ViewChild('picInput') picInput: ElementRef;

  //Khai báo các model
  leadModel = new LeadModel();
  contactModel = new ContactModel();
  noteModel = new NoteModel();

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  warningConfig: MatSnackBarConfig = { panelClass: 'warning-dialog', horizontalPosition: 'end', duration: 5000 };
  //Kết thúc khai báo các biến

  constructor(private translate: TranslateService,
    private commonService: CommonService,
    private categoryService: CategoryService,
    public dialog: MatDialog,
    private leadService: LeadService,
    private employeeService: EmployeeService,
    private noteService: NoteService,
    private route: ActivatedRoute,
    private router: Router,
    public snackBar: MatSnackBar) {
    translate.setDefaultLang('vi');
    this.filteredStatus = this.statusCtrl.valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filter(item, this.status) : this.status.slice()));
    this.filteredRequirement = this.requirementCtrl.valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filter(item, this.requirements) : this.requirements.slice()));
    this.filteredPotential = this.potentialCtrl.valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filter(item, this.potentials) : this.potentials.slice()));
    this.filteredPic = this.picCtrl.valueChanges.pipe(
      startWith(null),
      map((item: string) => item ? this._filterEmployee(item, this.employees) : this.employees.slice()));
  }
  
  ngOnInit() {
    //if (this.checkUserPermission(this.unFollowPermisison)) {
    //  this.searchLead();
    //  this.getMasterData();
    //} else {
    //  let _title = "THÔNG BÁO";
    //  let _content = "Bạn không có quyền xem Danh sách ngừng theo dõi.";
    //  this.dialogPopup = this.dialog.open(PopupComponent,
    //    {
    //      width: '500px',
    //      height: '250px',
    //      autoFocus: false,
    //      data: { title: _title, content: _content }
    //    });
    //  this.dialogPopup.afterClosed().subscribe(result => {
    //    this.router.navigate(['/lead/dashboard']);
    //  });
    //}
    this.searchLead();
    this.getMasterData();
  }

  //Các function xử lý event autocomplete
  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.selectedStatus.push(value.trim());
    }

    if (input) {
      input.value = "";
    }

    this.statusCtrl.setValue(null);
  }
  addRequirement(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.selectedRequirement.push(value.trim());
    }

    if (input) {
      input.value = "";
    }

    this.requirementCtrl.setValue(null);
  }
  addPotential(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.selectedPotential.push(value.trim());
    }

    if (input) {
      input.value = "";
    }

    this.potentialCtrl.setValue(null);
  }
  addPic(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.selectedPic.push(value.trim());
    }

    if (input) {
      input.value = "";
    }

    this.picCtrl.setValue(null);
  }

  remove(item: string): void {
    const index = this.selectedStatus.indexOf(item);
    if (index >= 0) {
      this.selectedStatus.splice(index, 1);
    }
  }
  removeRequirement(item: string): void {
    const index = this.selectedRequirement.indexOf(item);

    if (index >= 0) {
      this.selectedRequirement.splice(index, 1);
    }
  }
  removePotential(item: string): void {
    const index = this.selectedPotential.indexOf(item);

    if (index >= 0) {
      this.selectedPotential.splice(index, 1);
    }
  }
  removePic(item: string): void {
    const index = this.selectedPic.indexOf(item);

    if (index >= 0) {
      this.selectedPic.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.selectedStatus.push(event.option.value);
    this.statusInput.nativeElement.value = '';
    this.statusCtrl.setValue(null);
  }
  selectedRequirementFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedRequirement.push(event.option.value);
    this.requirementInput.nativeElement.value = '';
    this.requirementCtrl.setValue(null);
  }
  selectedPotentialFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedPotential.push(event.option.value);
    this.potentialInput.nativeElement.value = '';
    this.potentialCtrl.setValue(null);
  }
  selectedPicFn(event: MatAutocompleteSelectedEvent): void {
    this.selectedPic.push(event.option.value);
    this.picInput.nativeElement.value = '';
    this.picCtrl.setValue(null);
  }

  private _filter(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.categoryId.indexOf(filterValue) === 0);
  }

  private _filterEmployee(value: any, array: any): string[] {
    const filterValue = value;

    return array.filter(item => item.employeeId.indexOf(filterValue) === 0);
  }
  //Kết thúc các function xử lý event autocomplete

  //Hiển thị danh sách tất cả các Lead
  searchLead() {
    this.leadService.getLeadByStatus("Unfollowed").subscribe(response => {
      let result = <any>response;
      this.listLead = result.listLead;
      this.dataSource = new MatTableDataSource<any>(this.listLead);
      this.selection = new SelectionModel<LeadModel>(true, []);
      this.dataSource.paginator = this.paginator;
    });
  }

  //Hiển thị màu theo trạng thái của Lead
  checkColor(status) {
    switch (status.toLowerCase()) {
      case 'new':
        return 'status orange';
      case 'inprogress':
        return 'status green';
      case 'unfollowed':
        return 'status gray';
      case 'quotation':
        return 'status blue';
      case 'signed':
        return 'status red';
      case 'waiting':
        return 'status deep-blue';
    }
  }

  //Kiểm tra PIC có null hay không
  checkPersonInCharge(person) {
    if (person == null || person.trim() === "") {
      return false;
    } else {
      return true;
    }
  }

  //Tìm kiếm Lead theo các parameter truyền vào
  searchLeadByParameter() {
    this.leadModel.StatusId = this.selectedStatus.map(item => item.categoryId);
    this.leadModel.InterestedGroupId = this.selectedRequirement.map(item => item.categoryId);
    this.leadModel.PotentialId = this.selectedPotential.map(item => item.categoryId);
    this.leadModel.PersonInChargeId = this.selectedPic.map(item => item.employeeId);
    this.leadService.getLeadByStatus("Unfollowed").subscribe(response => {
      let result = <any>response;
      this.listLead = result.listLead;
      this.dataSource = new MatTableDataSource<any>(this.listLead);
      this.selection = new SelectionModel<LeadModel>(true, []);
      this.dataSource.paginator = this.paginator;
    });
  }

  /*Xac nhan hoac tu choi request chuyen trang thai Lead thanh Ngung theo doi*/
  approveOrReject(isApprove: boolean) {
    if (this.unfollowLeadIdList.length === 0) {
      this.translate.get('lead.select_no_item').subscribe(value => { this.messages = value; });
      this.snackBar.openFromComponent(WarningComponent, { data: this.messages, ...this.successConfig });
    } else {
      this.dialogApproveReject = this.dialog.open(ApproveRejectPopupComponent,
        {
          width: '500px',
          autoFocus: false,
          data: { isApprove: isApprove }
        });

      this.dialogApproveReject.afterClosed().subscribe(resultUnf => {
        if (resultUnf.ok) {
          this.leadService.approveRejectUnfollowLead(this.unfollowLeadIdList, isApprove).subscribe(response => {
            let result = <any>response;
            if (result.statusCode === 202 || result.statusCode === 200) {
              this.noteModel.Type = 'UNF';
              this.noteModel.NoteTitle = resultUnf.isApprove ? 'đã phê duyệt Ngừng theo dõi' : 'đã từ chối phê duyệt Ngừng theo dõi';
              this.noteModel.Description = resultUnf.rejectMessage;
              this.unfollowLeadIdList.forEach(leadId => {
                this.noteService.createNote(this.noteModel, leadId, null, this.auth.UserId).subscribe(response => { }, error => { });
              });
              
              this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
              this.searchLead();
            }
          }, error => { });
        }
      });
    }
  }
  /*Ket thuc*/

  //Các function chọn checkbox trên Grid
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
      this.unfollowLeadIdList = [];
    } else {
      this.dataSource.data.forEach(rows => {
        var item: any = rows;
        this.selection.select(item);
        this.unfollowLeadIdList.push(item.leadId);
      });
    }
  }

  selectedRowIndex: string = '';
  highlight(row) {
    this.selectedRowIndex = row.leadId;
  }

  rowCheckboxClick(event: MatCheckboxChange, row) {
    if (event) {
      this.selection.toggle(row);
    }

    if (event.checked) {
      this.unfollowLeadIdList.push(row.leadId);
    } else {
      this.unfollowLeadIdList.splice(this.unfollowLeadIdList.indexOf(row.leadId), 1);
    }
  }
  //Kết thúc Các function chọn checkbox trên Grid

  onViewDetail(leadId, contactId) {
    
  }

  getMasterData() {
    this.categoryService.getAllCategoryByCategoryTypeCode(this.productService).subscribe(response => {
      let result = <any>response;
      this.requirements = result.category;
    },
      error => { });
    this.categoryService.getAllCategoryByCategoryTypeCode(this.potentialCode).subscribe(response => {
      let result = <any>response;
      this.potentials = result.category;
    },
      error => { });
    this.categoryService.getAllCategoryByCategoryTypeCode(this.code).subscribe(response => {
      let result = <any>response;
      this.status = result.category;
    },
      error => { });
    this.employeeService.getAllEmployee().subscribe(response => {
      let result = <any>response;
      this.employees = result.employeeList;
    },
      error => { });
  }

  refreshParameter() {
    this.contactModel.FirstName = '';
    this.contactModel.LastName = '';
    this.contactModel.Phone = '';
    this.contactModel.Email = '';
    this.leadModel.StatusId = '';
    this.leadModel.InterestedGroupId = '';
    this.leadModel.PotentialId = '';
    this.leadModel.PersonInChargeId = '';
    this.selectedPotential = [];
    this.selectedPic = [];
    this.selectedRequirement = [];
    this.selectedStatus = [];

    this.searchLead();
  }

  //Function sap xep du lieu
  isAscending: boolean = false;
  sort(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.listLead.sort(function (a: any, b: any) {
      let x: any = "";
      let y: any = "";
      switch (property) {
        case "fullName":
          x = a.fullName.toLowerCase().trim();
          y = b.fullName.toLowerCase().trim();
          break;
        case "email":
          x = a.email.toLowerCase().trim();
          y = b.email.toLowerCase().trim();
          break;
        case "personInChargeFullName":
          x = a.personInChargeFullName.toLowerCase().trim();
          y = b.personInChargeFullName.toLowerCase().trim();
          break;
        case "statusName":
          x = a.statusName.toLowerCase().trim();
          y = b.statusName.toLowerCase().trim();
          break;
        default:
          break;
      }

      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataSource = new MatTableDataSource<LeadModel>(this.listLead);
    this.dataSource.paginator = this.paginator;
  }
  //Ket thuc
}
